using System;
using System.Collections.Generic;
using System.Net;
using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using UnityEngine;
using TMPro;


namespace PAJV
{
    public class SimpleClient : MonoBehaviour
    {
        [Header("Networking")]
        [SerializeField] private UnityClient riftClient;
        [SerializeField] private string ipAddress = "127.0.0.1";

        [Header("Prefabs")]
        [SerializeField] private GameObject playerPrefab; 

        [Header("Chat References")]
        [SerializeField] private TMP_InputField chatInput;
        [SerializeField] private TextMeshProUGUI chatHistoryText;

        private Dictionary<ushort, GameObject> spawnedPlayers = new Dictionary<ushort, GameObject>();

        private List<string> chatMessages = new List<string>();

        private AdvancedMovementControls localInputControls;

        private void Start()
        {
            
            if (chatInput != null)
            {
                chatInput.onSubmit.RemoveAllListeners();
                chatInput.onSubmit.AddListener(SendChatMessage);
            }

            riftClient.ConnectInBackground(IPAddress.Parse(ipAddress), 4296, 4297, true, OnConnected);
        }

        private void Update()
        {
        
            if (localInputControls != null && chatInput != null)
            {
                bool isChatting = chatInput.isFocused;
                
                if (localInputControls.enabled == isChatting)
                {
                    localInputControls.enabled = !isChatting;
                }
            }
        }

        private void OnConnected(Exception e)
        {
            if (riftClient.ConnectionState == ConnectionState.Connected)
            {
                Debug.Log("Connected to server!");
                riftClient.MessageReceived += HandleMessage;
            }
        }

        private void SendChatMessage(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(text);
                using (Message msg = Message.Create(2, writer))
                    riftClient.SendMessage(msg, SendMode.Reliable);
            }

            chatInput.text = "";
            chatInput.ActivateInputField();
        }

        private void HandleMessage(object sender, MessageReceivedEventArgs args)
        {
            using (Message message = args.GetMessage())
            using (DarkRiftReader reader = message.GetReader())
            {
            
                if (message.Tag == 0)
                {
                    ushort id = reader.ReadUInt16();
                    float r = reader.ReadSingle(); float g = reader.ReadSingle(); float b = reader.ReadSingle();

                    // 1. Instantiem Radacina
                    GameObject rootObj = Instantiate(playerPrefab);
                    rootObj.name = $"PlayerRoot_{id}";

                    // 2. Gasim componentele critice in ierarhie
                    Rigidbody capsuleRb = rootObj.GetComponentInChildren<Rigidbody>();

                    if (capsuleRb == null)
                    {
                        Debug.LogError("Nu am gasit Rigidbody in copiii player-ului! Verifica Prefab-ul.");
                        return;
                    }

                    GameObject capsuleObj = capsuleRb.gameObject;

                    // Gasim camera pentru a o dezactiva la ceilalti
                    Transform cameraCtrlTrans = rootObj.transform.Find("PlayerMovement/CameraController");
                    if (cameraCtrlTrans == null)
                    {
                        var camComp = rootObj.GetComponentInChildren<Camera>();
                        if (camComp) cameraCtrlTrans = camComp.transform.parent;
                    }

                    // Setam culoarea jucatorului
                    var playerColorScript = rootObj.GetComponentInChildren<PlayerColor>();
                    Color networkColor = new Color(r, g, b);

                    if (playerColorScript != null)
                    {
                        playerColorScript.SetColor(networkColor);
                    }
                    else
                    {
                       
                        var renderers = rootObj.GetComponentsInChildren<Renderer>();
                        foreach (var rend in renderers) rend.material.color = networkColor;
                    }

                    // init
                    if (id == riftClient.ID)
                    {
                        // eu
                        Debug.Log("Spawning ME.");

                        // 1. Controale
                        localInputControls = rootObj.GetComponentInChildren<AdvancedMovementControls>();
                        if (localInputControls) localInputControls.enabled = true;

                        var moveCtrl = rootObj.GetComponentInChildren<AdvancedMovementController>();
                        if (moveCtrl) moveCtrl.enabled = true;

                        // 2. Network Sender (pe capsula)
                        var senderScript = capsuleObj.AddComponent<NetworkPlayerSender>();
                        senderScript.Initialize(riftClient);

                        // 3. Pozitionare la un Spawn Point aleator
                        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

                        if (spawnPoints.Length > 0)
                        {
                            int randomIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
                            Transform chosenSpawn = spawnPoints[randomIndex].transform;

                            
                            capsuleObj.transform.position = chosenSpawn.position;
                            capsuleObj.transform.rotation = chosenSpawn.rotation;

                            Debug.Log($"Spawned at: {chosenSpawn.name}");
                        }
                        else
                        {
                            Debug.LogWarning("No SpawnPoints found! Spawning at default (0, 2, 0).");
                            capsuleObj.transform.position = new Vector3(0, 2, 0);
                        }
                    }
                    else
                    {
                        // adversar
                        Debug.Log($"Spawning REMOTE Player {id}.");

                        // 1. DEZACTIVAM CAMERA
                        if (cameraCtrlTrans != null)
                            cameraCtrlTrans.gameObject.SetActive(false);
                        else
                        {
                            foreach (var c in rootObj.GetComponentsInChildren<Camera>()) c.gameObject.SetActive(false);
                            foreach (var al in rootObj.GetComponentsInChildren<AudioListener>()) Destroy(al);
                            foreach (var cm in rootObj.GetComponentsInChildren<CinemachineMovement>()) Destroy(cm);
                        }

                        // 2. Curatam scripturile inutile
                        var inputCtrl = rootObj.GetComponentInChildren<AdvancedMovementControls>();
                        if (inputCtrl) Destroy(inputCtrl);

                        var moveCtrl = rootObj.GetComponentInChildren<AdvancedMovementController>();
                        if (moveCtrl) Destroy(moveCtrl);

                        // 3. Fizica Kinematica
                        capsuleRb.isKinematic = true;

                        // 4. Pozitia de la Server (Initiala)
                        float x = reader.ReadSingle(); float y = reader.ReadSingle(); float z = reader.ReadSingle();
                        capsuleObj.transform.position = new Vector3(x, y, z);
                    }

                    spawnedPlayers.Add(id, rootObj);
                }


                else if (message.Tag == 1)
                {
                    ushort id = reader.ReadUInt16();
                    float x = reader.ReadSingle(); float y = reader.ReadSingle(); float z = reader.ReadSingle();
                    float rotY = reader.ReadSingle();

                    if (spawnedPlayers.ContainsKey(id))
                    {
                        var rb = spawnedPlayers[id].GetComponentInChildren<Rigidbody>();
                        if (rb != null)
                        {
                            rb.MovePosition(new Vector3(x, y, z));
                            rb.MoveRotation(Quaternion.Euler(0, rotY, 0));
                        }
                    }
                }

  
                else if (message.Tag == 2)
                {
                    ushort senderId = reader.ReadUInt16();
                    string text = reader.ReadString();
                    UpdateChatUI($"Player {senderId}: {text}");
                }

   
                else if (message.Tag == 3)
                {
                    ushort id = reader.ReadUInt16();
                    if (spawnedPlayers.ContainsKey(id))
                    {
                        Destroy(spawnedPlayers[id]);
                        spawnedPlayers.Remove(id);
                    }
                }
            }
        }

        private void UpdateChatUI(string newMsg)
        {
            chatMessages.Add(newMsg);
            if (chatMessages.Count > 5) chatMessages.RemoveAt(0);
            chatHistoryText.text = string.Join("\n", chatMessages);
        }
    }
}