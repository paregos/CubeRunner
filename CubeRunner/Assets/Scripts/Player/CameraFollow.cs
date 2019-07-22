using UnityEngine;

namespace Assets.Scripts.Player
{
    public class CameraFollow : MonoBehaviour {

        public GameObject player;
        public Vector3 initialPosition;

        private Vector3 velocity = Vector3.zero;

        void Update()
        {
            // Position
            Vector3 pos = player.transform.position;
            pos.y = transform.position.y;
            pos.z = transform.position.z;
            pos.x += 3.3f;
            transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, 0.7F);
        }

        public void Reset()
        {
            transform.position = initialPosition;
        }
    }
}
