using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        public Vector3 initialVelocity;
        public Vector3 initialPosition;
        public Color initialColor;
        public Color highlightColor;

        private int _numberOfRowsPassed;
        private Rigidbody _Rigidbody;
        private Vector3 _nextPlayerVelocity;

        private bool _stopped = true;

        //TODO REMOVE DEBUG FUNCTION

        public void SetInitialVelocity(Slider slider)
        {
            initialVelocity = new Vector3(slider.value, 0, slider.value);
        }

        //END TODO

        void Start()
        {
            _Rigidbody = GetComponent<Rigidbody>();
            transform.position = initialPosition;
            //_Rigidbody.AddForce(initialAcceleration, ForceMode.Acceleration);
        }

        public void ResetPlayer()
        {
            _stopped = false;
            transform.position = initialPosition;
            _numberOfRowsPassed = 0;
            _Rigidbody.velocity = initialVelocity;
            _nextPlayerVelocity = _Rigidbody.velocity;
        }

        public void StopPlayer()
        {
            _stopped = true;
            _Rigidbody.velocity = Vector3.zero;
            _nextPlayerVelocity = Vector3.zero;
        }

        void FixedUpdate()
        {
            if (_stopped)
            {
                return;
            }

            if (Input.touchCount == 1)
            {
                var touch = Input.touches[0];
                if (touch.position.x < Screen.width / 2)
                {
                    _nextPlayerVelocity = new Vector3(_Rigidbody.velocity.x, 0, Mathf.Abs(_Rigidbody.velocity.z));
                }
                else if (touch.position.x > Screen.width / 2)
                {
                    _nextPlayerVelocity = new Vector3(_Rigidbody.velocity.x, 0, Mathf.Abs(_Rigidbody.velocity.z) * -1);
                }
            }

            if (Input.GetKey("left"))
            {
                _nextPlayerVelocity = new Vector3(_Rigidbody.velocity.x, 0, Mathf.Abs(_Rigidbody.velocity.z));
            }

            if (Input.GetKey("right"))
            {
                _nextPlayerVelocity = new Vector3(_Rigidbody.velocity.x, 0, Mathf.Abs(_Rigidbody.velocity.z) * -1);
            }

            //accelerate
            //_Rigidbody.velocity = new Vector3(_Rigidbody.velocity.x + accelerationRate, 0, _Rigidbody.velocity.z + accelerationRate);
        }

        void Update()
        {
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Wall")
            {
                GameObject.Find("GameController").GetComponent<GameController>().StopGame();
            }
        }

        public void ChangeDirectionsIfNeeded(float changeAtXposition, float changeAtZPosition)
        {
            if (_Rigidbody.velocity == _nextPlayerVelocity)
            {
                return;
            }

            var rotation = _nextPlayerVelocity.z > 0 ? 45 : 135;

            transform.DORotate(new Vector3(0, rotation, 0), 0.15f, RotateMode.Fast);
            transform.position = new Vector3(changeAtXposition, gameObject.transform.position.y, changeAtZPosition);
            _Rigidbody.velocity = _nextPlayerVelocity;
        }

        public void IncrementNumberOfRowsPassed()
        {
            _numberOfRowsPassed++;
        }

        public int GetNumberOfRowsPassed()
        {
            return _numberOfRowsPassed;
        }
    }
}