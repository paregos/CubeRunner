using UnityEngine;

namespace Assets.Scripts.Player
{
    public class CameraFollow : MonoBehaviour {

        public GameObject player;


        void Update()
        {
            // Position
            Vector3 pos = player.transform.position;
            pos.y = transform.position.y;
            pos.z = transform.position.z;
            pos.x += 3.3f;
            transform.position = pos;
        }
    }
}
