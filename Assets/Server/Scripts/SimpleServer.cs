using DarkRift;
using DarkRift.Server;
using DarkRift.Server.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PAJV
{
    public class SimpleServer : MonoBehaviour
    {
        [SerializeField] private XmlUnityServer riftServer;

        private struct PlayerData
        {
            public ushort ID;
            public float R, G, B;
            public float X, Y, Z;
            public float RotY;
        }

        private Dictionary<ushort, PlayerData> connectedPlayers = new Dictionary<ushort, PlayerData>();

        private void Start()
        {
            Application.runInBackground = true;
            riftServer.Server.ClientManager.ClientConnected += HandleClientConnected;
            riftServer.Server.ClientManager.ClientDisconnected += HandleClientDisconnected;
        }

        private void HandleClientConnected(object sender, ClientConnectedEventArgs args)
        {

            float r = UnityEngine.Random.value;
            float g = UnityEngine.Random.value;
            float b = UnityEngine.Random.value;

            PlayerData newPlayer = new PlayerData
            {
                ID = args.Client.ID,
                R = r,
                G = g,
                B = b,
                X = 0,
                Y = 1,
                Z = 0,
                RotY = 0
            };

            connectedPlayers.Add(args.Client.ID, newPlayer);

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(newPlayer.ID);
                writer.Write(newPlayer.R); writer.Write(newPlayer.G); writer.Write(newPlayer.B);

                using (Message msg = Message.Create(0, writer))
                    args.Client.SendMessage(msg, SendMode.Reliable);
            }

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(newPlayer.ID);
                writer.Write(newPlayer.R); writer.Write(newPlayer.G); writer.Write(newPlayer.B);
                writer.Write(newPlayer.X); writer.Write(newPlayer.Y); writer.Write(newPlayer.Z);

                using (Message msg = Message.Create(0, writer))
                {
                    foreach (IClient connectedClient in riftServer.Server.ClientManager.GetAllClients())
                    {
                        if (connectedClient.ID != args.Client.ID)
                            connectedClient.SendMessage(msg, SendMode.Reliable);
                    }
                }
            }

           
            foreach (var existingPlayer in connectedPlayers.Values)
            {
                if (existingPlayer.ID == args.Client.ID) continue;

                using (DarkRiftWriter writer = DarkRiftWriter.Create())
                {
                    writer.Write(existingPlayer.ID);
                    writer.Write(existingPlayer.R); writer.Write(existingPlayer.G); writer.Write(existingPlayer.B);
                    writer.Write(existingPlayer.X); writer.Write(existingPlayer.Y); writer.Write(existingPlayer.Z);

                    using (Message msg = Message.Create(0, writer))
                        args.Client.SendMessage(msg, SendMode.Reliable);
                }
            }

            args.Client.MessageReceived += HandleClientMessageReceived;

            Debug.Log($"[Server] Client with ID {args.Client.ID} connected!");
            Debug.Log($"[Server] Total connected clients: {riftServer.Server.ClientManager.GetAllClients().Length}");
        }

        private void HandleClientDisconnected(object sender, ClientDisconnectedEventArgs args)
        {
            connectedPlayers.Remove(args.Client.ID);

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(args.Client.ID);
                using (Message msg = Message.Create(3, writer))
                {
                    foreach (IClient client in riftServer.Server.ClientManager.GetAllClients())
                        client.SendMessage(msg, SendMode.Reliable);
                }
            }

            Debug.Log($"[Server] Client with ID {args.Client.ID} disconnected!");
            Debug.Log($"[Server] Total connected clients: {riftServer.Server.ClientManager.GetAllClients().Length}");
        }

        private void HandleClientMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            using (Message message = args.GetMessage())
            using (DarkRiftReader reader = message.GetReader())
            {
                if (message.Tag == 1)
                {
                    float x = reader.ReadSingle();
                    float y = reader.ReadSingle();
                    float z = reader.ReadSingle();
                    float rotY = reader.ReadSingle();

                    if (connectedPlayers.ContainsKey(args.Client.ID))
                    {
                        var p = connectedPlayers[args.Client.ID];
                        p.X = x; p.Y = y; p.Z = z; p.RotY = rotY;
                        connectedPlayers[args.Client.ID] = p;
                    }

           
                    using (DarkRiftWriter writer = DarkRiftWriter.Create())
                    {
                        writer.Write(args.Client.ID);
                        writer.Write(x); writer.Write(y); writer.Write(z);
                        writer.Write(rotY);

                        using (Message outMsg = Message.Create(1, writer))
                        {
                            foreach (IClient client in riftServer.Server.ClientManager.GetAllClients())
                            {
                                if (client.ID != args.Client.ID)
                                    client.SendMessage(outMsg, SendMode.Unreliable);
                            }
                        }
                    }

                    Debug.Log($"Movement from {args.Client.ID}: Pos({x}, {y}, {z}) RotY({rotY})");
                }
                else if (message.Tag == 2)
                {
                    string text = reader.ReadString();
                    Debug.Log($"Chat from {args.Client.ID}: {text}");

                  
                    using (DarkRiftWriter writer = DarkRiftWriter.Create())
                    {
                        writer.Write(args.Client.ID);
                        writer.Write(text);     

                        using (Message outMsg = Message.Create(2, writer))
                        {
                            foreach (IClient client in riftServer.Server.ClientManager.GetAllClients())
                                client.SendMessage(outMsg, SendMode.Reliable);
                        }
                    }

                    Debug.Log($"Broadcasted chat from {args.Client.ID}");
                }
            }
        }
    }
}

//Tag 0: Spawn, Tag 1: Movement, Tag 2: Chat, Tag 3: Disconnect