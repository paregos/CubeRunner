using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        public float initialVelocity;
        public float currentVelocity;
        public Vector3 initialPosition;
        public Color initialColor;
        public Color highlightColor;

        private int _numberOfRowsPassed;
        private Rigidbody _Rigidbody;
        private Vector3 _nextPlayerVelocity;

        private bool _stopped = true;
        private bool _canSwitchSides = true;

        //TODO REMOVE DEBUG FUNCTION

        public void SetInitialVelocity(Slider slider)
        {
            currentVelocity = slider.value;
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
            currentVelocity = initialVelocity;
            transform.position = initialPosition;
            _numberOfRowsPassed = 0;
            _Rigidbody.velocity = new Vector3(initialVelocity, 0, initialVelocity);
            _nextPlayerVelocity = _Rigidbody.velocity;
            _canSwitchSides = true;
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

            if (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Began)
            {
                    _nextPlayerVelocity = new Vector3(currentVelocity, 0, _Rigidbody.velocity.z > 0 ? currentVelocity * - 1 : currentVelocity);
            }

            if (Input.GetKey("a"))
            {
                    _nextPlayerVelocity = new Vector3(currentVelocity, 0, currentVelocity);
            }

            if (Input.GetKey("d"))
            {
                _nextPlayerVelocity = new Vector3(currentVelocity, 0, currentVelocity * -1);
            }

            //accelerate
            //_Rigidbody.velocity = new Vector3(_Rigidbody.velocity.x + accelerationRate, 0, _Rigidbody.velocity.z + accelerationRate);
        }

        void Update()
        {
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("rebound");
            if (other.gameObject.tag == "Wall" && _canSwitchSides)
            {
                Debug.Log("rebound in");
                
                // return if already going right direction
                if (_Rigidbody.velocity.z < 0 && other.gameObject.transform.position.z > 1 ||
                    _Rigidbody.velocity.z > 0 && other.gameObject.transform.position.z < 1)
                {
                    return;
                }

                _nextPlayerVelocity = new Vector3(currentVelocity, 0, _Rigidbody.velocity.z > 0 ? currentVelocity * -1 : currentVelocity);

                ChangeDirectionsIfNeeded(
                    other.gameObject.transform.position.x - GameConstants.halfblockWidth,
                    other.gameObject.transform.position.z > 1 ? other.gameObject.transform.position.z - GameConstants.halfblockWidth : other.gameObject.transform.position.z + GameConstants.halfblockWidth);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ChangeDirectionsIfNeeded(float changeAtXposition, float changeAtZPosition)
        {
            if (!_canSwitchSides)
            {
                return;
            }
            if (_Rigidbody.velocity == _nextPlayerVelocity)
            {
                return;
            }

            _canSwitchSides = false;

            ChangePlayerDirection(changeAtXposition, changeAtZPosition);
        }

        private void ChangePlayerDirection(float changeAtXposition, float changeAtZPosition)
        {
            Debug.Log("changeside");

            var rotation = _nextPlayerVelocity.z > 0 ? 45 : 135;

            transform.DORotate(new Vector3(0, rotation, 0), 0.15f, RotateMode.Fast);
            transform.position = new Vector3(changeAtXposition, gameObject.transform.position.y, changeAtZPosition);
            _Rigidbody.velocity = _nextPlayerVelocity;
        }

        public void IncrementNumberOfRowsPassed()
        {
            Debug.Log("row++ "+transform.position.x);
            _canSwitchSides = true;
            _numberOfRowsPassed++;
        }

        public int GetNumberOfRowsPassed()
        {
            return _numberOfRowsPassed;
        }

        public void SetColor(Color color)
        {
            var material = gameObject.GetComponent<Renderer>().material;
            material.color = color;
            material.SetColor("_EmissionColor", color);
        }
    }
}