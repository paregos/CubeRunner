using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        public Vector3 initialVelocity;
        public float accelerationRate;

        private Vector3 _playerMovementDirection;
        private Rigidbody _Rigidbody;

        void Start()
        {
            _Rigidbody = GetComponent<Rigidbody>();
            _Rigidbody.velocity = initialVelocity;            
            //_Rigidbody.AddForce(initialAcceleration, ForceMode.Acceleration);
        }

        void FixedUpdate()
        {
            Debug.Log("Player velocity = x" + _Rigidbody.velocity.x + " z " + _Rigidbody.velocity.z);

            if (Input.touchCount == 1)
            {
                var touch = Input.touches[0];
                if (touch.position.x < Screen.width / 2)
                {
                    _Rigidbody.velocity = new Vector3(_Rigidbody.velocity.x, 0, Mathf.Abs(_Rigidbody.velocity.z));
                }
                else if (touch.position.x > Screen.width / 2)
                {
                    _Rigidbody.velocity = new Vector3(_Rigidbody.velocity.x, 0, Mathf.Abs(_Rigidbody.velocity.z) * -1);
                }
            }

            if (Input.GetKey("left"))
            {
                _Rigidbody.velocity = new Vector3(_Rigidbody.velocity.x, 0, Mathf.Abs(_Rigidbody.velocity.z));
            }

            if (Input.GetKey("right"))
            {
                _Rigidbody.velocity = new Vector3(_Rigidbody.velocity.x, 0, Mathf.Abs(_Rigidbody.velocity.z) * -1);
            }

            //accelerate
            //_Rigidbody.velocity = new Vector3(_Rigidbody.velocity.x + accelerationRate, 0, _Rigidbody.velocity.z + accelerationRate);
        }

        void Update()
        {
        }
    }
}