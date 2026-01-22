using UnityEngine;
using DarkRift;
using DarkRift.Client.Unity;

namespace PAJV
{
    // Acest script se pune pe obiectul care are Rigidbody-ul (Capsule)
    public class NetworkPlayerSender : MonoBehaviour
    {
        private UnityClient client;
        private Vector3 lastSentPos;
        private float lastSentRot;

        public void Initialize(UnityClient c)
        {
            client = c;
        }

        void Update()
        {
            // Trimitem date doar daca avem client si ne-am miscat
            if (client == null || client.ConnectionState != ConnectionState.Connected) return;

            if (Vector3.Distance(transform.position, lastSentPos) > 0.05f ||
                Mathf.Abs(transform.eulerAngles.y - lastSentRot) > 1f)
            {
                SendMovement();
                lastSentPos = transform.position;
                lastSentRot = transform.eulerAngles.y;
            }
        }

        void SendMovement()
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                // Trimitem pozitia si rotatia CAPSULEI
                writer.Write(transform.position.x);
                writer.Write(transform.position.y);
                writer.Write(transform.position.z);
                writer.Write(transform.eulerAngles.y);

                using (Message msg = Message.Create(1, writer))
                {
                    client.SendMessage(msg, SendMode.Unreliable);
                }
            }
        }
    }
}