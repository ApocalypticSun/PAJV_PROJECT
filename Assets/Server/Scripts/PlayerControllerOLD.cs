using UnityEngine;
using DarkRift;
using DarkRift.Client.Unity;

namespace PAJV
{
    public class PlayerController : MonoBehaviour
    {
        private UnityClient client;
        private float moveSpeed = 5f;
        private float rotSpeed = 150f;
        private float jumpForce = 5f;
        private Rigidbody rb;

        private Vector3 lastSentPos;
        private float lastSentRot;

        public bool InputDisabled = false;

        public void Initialize(UnityClient c)
        {
            client = c;
            rb = GetComponent<Rigidbody>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        void Update()
        {
            if (InputDisabled) 
                return;

            float move = 0;
            if (Input.GetKey(KeyCode.W)) move = 1;
            if (Input.GetKey(KeyCode.S)) move = -1;

            float rotate = 0;
            if (Input.GetKey(KeyCode.E)) rotate = 1;
            if (Input.GetKey(KeyCode.Q)) rotate = -1;

            transform.Translate(Vector3.forward * move * moveSpeed * Time.deltaTime);
            transform.Rotate(Vector3.up * rotate * rotSpeed * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }

      
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