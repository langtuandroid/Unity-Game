using LobsterFramework.Utility;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace LobsterFramework
{
    [RequireComponent (typeof(Rigidbody2D))]
    public class MovementController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private RefFloat maxSpeed;
        [SerializeField] private RefFloat rotateSpeed;

        /// <summary>
        /// Event that occurs when movement status is changed
        /// </summary>
        public Action<bool> onMovementBlocked;
        /// <summary>
        /// Manages movement blocking & unblocking. Add true values to block movement, remove all true values to unblock.
        /// </summary>
        public readonly BaseOr movementLock = new(false);
        /// <summary>
        /// Modifies movespeed, base value is 1
        /// </summary>
        public readonly FloatProduct moveSpeedModifier = new(1, true);
        /// <summary>
        /// Modifies rotate speed, base value is 1
        /// </summary>
        public readonly FloatProduct rotateSpeedModifier = new(1, true);

        private Rigidbody2D _rigidBody;
        private Collider2D _collider;
        new private Transform transform;
        
        private Vector2 steering;
        private Vector2 targetVelocity;

        public float Speed { get; private set; }
        public float RotateSpeed { get; private set; }

        public Rigidbody2D RigidBody { get { return _rigidBody; } }
        public bool MovementBlocked { get { return movementLock.Value; } }

        private void Start() { 
            transform = GetComponent<Transform>();
            _rigidBody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            Speed = maxSpeed.Value * moveSpeedModifier.Value;
            RotateSpeed = rotateSpeed.Value * rotateSpeedModifier.Value;
            movementLock.onValueChanged += OnMovementStatusChanged;
            moveSpeedModifier.onValueChanged += OnMoveSpeedChanged;
            rotateSpeedModifier.onValueChanged += OnRotateSpeedChanged;
        }

        private void FixedUpdate()
        {
            if (targetVelocity != Vector2.zero) {
                _rigidBody.AddForce(ComputeVelocityForce(), ForceMode2D.Impulse);
                targetVelocity = Vector2.zero;
            }

            if (steering != Vector2.zero)
            {
                _rigidBody.AddForce(transform.rotation * steering);
            }
        }
        #region MovementUtils
        private void OnMovementStatusChanged(bool blocked)
        {
            if (blocked)
            {
                onMovementBlocked?.Invoke(true);
            }
            else {
                onMovementBlocked?.Invoke(false);
            }
        }
        private void OnMoveSpeedChanged(float modifier) {
            Speed = maxSpeed * modifier;
        }

        private void OnRotateSpeedChanged(float modifier) {
            RotateSpeed = rotateSpeed * modifier;
        }

        public void DisableCollider() {
            _collider.enabled = false;
        }

        public void EnableCollider() { 
            _collider.enabled = true;
        }

        public void IgnoreCollision(Collider2D collider, bool ignore=true) { 
            Physics2D.IgnoreCollision(_collider, collider, ignore);
        }

        public void KinematicBody(bool isKinematic) {
             _rigidBody.isKinematic = isKinematic;
        }
        #endregion

        #region MovementMethods
        /// <summary>
        /// Attempt to rotate the entity towards the specified direction. If the specified angle is larger than the max rotation speed,
        /// the entity will rotate towards target angle will max speed. Will fail if Movement blocked. 
        /// </summary>
        /// <param name="direction">The target direction to rotate towards</param>
        public void RotateTowards(Vector2 direction)
        {
            if (MovementBlocked)
            {
                return;
            }
            float angle = Vector2.SignedAngle(transform.up, direction);
            Quaternion target = Quaternion.Euler(0, 0, angle);
            float maxRotate = RotateSpeed * Time.deltaTime;
            if (Math.Abs(angle) > maxRotate)
            {
                target = Quaternion.Lerp(Quaternion.identity, target, maxRotate / Math.Abs(angle));
            }
            transform.rotation = target * transform.rotation;
        }
        /// <summary>
        /// Attempt to rotate the entity by the specified degree. If the specified angle is larger than the max rotation speed,
        /// the entity will rotate towards target angle will max speed. Will fail if Movement blocked. 
        /// </summary>
        /// <param name="degree">The degree to rotate the entity by</param>
        public void RotateByDegrees(float degree)
        {
            if (MovementBlocked)
            {
                return;
            }
            float maxDegree = RotateSpeed * Time.deltaTime;
            if (Math.Abs(degree) > maxDegree)
            {
                if (degree < 0)
                {
                    degree = -maxDegree;
                }
                else
                {
                    degree = maxDegree;
                }
            }
            transform.rotation = Quaternion.AngleAxis(degree, transform.forward) * transform.rotation;
        }

        /// <summary>
        /// Start moving the entity towards the specified direction, will fail if Movement is blocked on this entity
        /// </summary>
        /// <param name="direction"></param>
        public void Move(Vector2 direction, float acceleration = -1)
        {
            if (MovementBlocked)
            {
                steering = Vector2.zero;
                return;
            }
            if (acceleration <= 0 || acceleration > Speed)
            {
                acceleration = Speed;
            }
            steering = direction.normalized * acceleration;
        }

        public void SetVelocityImmediate(Vector2 velocity)
        {
            _rigidBody.velocity = velocity;
        }

        public void SetVelocity(Vector2 velocity) {
            targetVelocity = velocity;
            steering = Vector2.zero;
        }

        private Vector2 ComputeVelocityForce() {
            Vector2 force = (targetVelocity - _rigidBody.velocity) * _rigidBody.mass;
            float mag = force.magnitude;
            float max = Speed * Time.deltaTime;
            if (mag > max)
            {
                mag = max;
            }
            return force.normalized * mag;
        }

        public void ApplyForce(Vector2 direction, float magnitude)
        {
            _rigidBody.AddForce(direction.normalized * magnitude, ForceMode2D.Impulse);
        }
        #endregion
    }
}
