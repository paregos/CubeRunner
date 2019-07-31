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
        private bool _shouldIncreaseSpeed = false;
        private int _rowsToIncreaseSpeedAt = 50;
        private float _speedIncrease = 0.3f;

        //TODO REMOVE DEBUG FUNCTION

        public void SetInitialVelocity(Slider slider)
        {
            //currentVelocity = slider.value;
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
            transform.DOKill();
            _stopped = false;
            currentVelocity = initialVelocity;
            transform.position = initialPosition;
            transform.rotation = Quaternion.Euler(0, 45, 0);
            _numberOfRowsPassed = 0;
            _Rigidbody.velocity = new Vector3(initialVelocity, 0, initialVelocity);
            _nextPlayerVelocity = _Rigidbody.velocity;
            _canSwitchSides = true;
            _shouldIncreaseSpeed = false;
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

            CheckIfPlayerNeedsToForceChangeDirection();

            //accelerate
            if (_shouldIncreaseSpeed)
            {
                currentVelocity = currentVelocity + _speedIncrease;
                _shouldIncreaseSpeed = false;
            }

            if (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Began)
            {
                    _nextPlayerVelocity = new Vector3(currentVelocity, 0, _Rigidbody.velocity.z > 0 ? currentVelocity * - 1 : currentVelocity);
            }

//            if (Input.GetKey("a"))
//            {
//                    _nextPlayerVelocity = new Vector3(currentVelocity, 0, currentVelocity);
//            }
//
//            if (Input.GetKey("d"))
//            {
//                _nextPlayerVelocity = new Vector3(currentVelocity, 0, currentVelocity * -1);
//            }
        }

        void Update()
        {
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
            var rotation = _nextPlayerVelocity.z > 0 ? 45 : 135;

            transform.DORotate(new Vector3(0, rotation, 0), 0.15f, RotateMode.Fast);
            transform.position = new Vector3(changeAtXposition, gameObject.transform.position.y, changeAtZPosition);
            _Rigidbody.velocity = _nextPlayerVelocity;
        }

        public void IncrementNumberOfRowsPassed()
        {
            _canSwitchSides = true;
            _numberOfRowsPassed++;
            if (_numberOfRowsPassed % _rowsToIncreaseSpeedAt == 0)
            {
                _shouldIncreaseSpeed = true;
            }
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

        public void FallDownHole()
        {
            _stopped = true;
            transform.DOMove(new Vector3(transform.position.x, transform.position.y - 10, transform.position.z), 3f);
        }

        public void GetSpiked()
        {
            _stopped = true;
            transform.DOShakeRotation(1.3f);
            transform.DOJump(transform.position, .5f, 1, 1f);
        }

        private void CheckIfPlayerNeedsToForceChangeDirection()
        {
            if (transform.position.z < GameConstants.blockWidth)
            {
                // need to move left
                ForcePlayerleft();
            }else if (transform.position.z > GameConstants.initialRowOddWidth * GameConstants.blockWidth)
            {
                ForcePlayerRight();
            }
        }

        private void ForcePlayerleft()
        {
            _Rigidbody.velocity = new Vector3(currentVelocity, 0, currentVelocity);
            transform.DORotate(new Vector3(0, 45, 0), 0.15f, RotateMode.Fast);
            _nextPlayerVelocity = _Rigidbody.velocity;
            _canSwitchSides = false;
        }

        private void ForcePlayerRight()
        {
            _Rigidbody.velocity = new Vector3(currentVelocity, 0, currentVelocity*-1);
            transform.DORotate(new Vector3(0, 135, 0), 0.15f, RotateMode.Fast);
            _nextPlayerVelocity = _Rigidbody.velocity;
            _canSwitchSides = false;
        }

        private void CheckKillZone()
        {
            if (transform.position.z < GameConstants.halfblockWidth || transform.position.z >
                GameConstants.initialRowOddWidth * GameConstants.blockWidth + GameConstants.halfblockWidth)
            {
                GameObject.Find("GameController").GetComponent<GameController>().StopGame();
            }
        }
    }
}