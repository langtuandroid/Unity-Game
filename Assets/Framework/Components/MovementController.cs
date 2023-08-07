using LobsterFramework.Utility.BufferedStats;
using System;
using System.Collections;
using System.Collections.Generic;
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

        public UnityAction<bool> onMovementBlocked;

        private Rigidbody2D _rigidBody;
        new private Transform transform;
        private BaseOr movementBlock = new(false);
        private Vector2 steering;

        private FloatProduct moveSpeedMultiplier = new(1, true);
        private FloatProduct rotateSpeedMultiplier = new(1, true);

        public float Speed { get; private set; }
        public float RotateSpeed { get; private set; }

        public Rigidbody2D RigidBody { get { return _rigidBody; } }
        public bool MovementBlocked { get { return movementBlock.Stat; } }

        private void Start() { 
            transform = GetComponent<Transform>();
            _rigidBody = GetComponent<Rigidbody2D>();
            Speed = maxSpeed.Value * moveSpeedMultiplier.Stat;
            RotateSpeed = rotateSpeed.Value * rotateSpeedMultiplier.Stat;
        }

        private void FixedUpdate()
        {
            if (steering != Vector2.zero)
            {
                _rigidBody.AddForce(transform.rotation * steering);
            }
        }
        #region MovementModifiers
        /// <summary>
        /// Add an effector to block movement of this entity. The movement of the entity will be blocked if there's at least 1 effector.
        /// </summary>
        /// <returns>The id of the newly added effector</returns>
        public int BlockMovement()
        {
            bool before = MovementBlocked;
            int key = movementBlock.AddEffector(true);
            if (onMovementBlocked != null && !before)
            {
                onMovementBlocked.Invoke(true);
            }
            return key;
        }

        /// <summary>
        /// Remove the effector that blocks movement with specified key. The movement of the entity will be unblocked if there's 0 effector left. 
        /// Will fail if the effector with specified key does not exist.
        /// </summary>
        /// <param name="key">The key of the effector to be remvoed</param>
        /// <returns> The status of this operation </returns>
        public bool UnblockMovement(int key)
        {
            if (movementBlock.RemoveEffector(key))
            {
                if (!MovementBlocked && onMovementBlocked != null)
                {
                    onMovementBlocked.Invoke(false);
                }
                return true;
            }
            return false;
        }

        public int ModifyMoveSpeed(float speedMultiplier)
        {
            int key = moveSpeedMultiplier.AddEffector(speedMultiplier);
            Speed = maxSpeed.Value * moveSpeedMultiplier.Stat;
            return key;
        }

        public bool UnmodifyMoveSpeed(int key)
        {
            if (moveSpeedMultiplier.RemoveEffector(key)) {
                Speed = maxSpeed.Value * moveSpeedMultiplier.Stat;
                return true;
            }
            return false;
        }

        public int ModifyRotationSpeed(float speedMultiplier)
        {
            int key = rotateSpeedMultiplier.AddEffector(speedMultiplier);
            RotateSpeed = rotateSpeed.Value * rotateSpeedMultiplier.Stat;
            return key;
        }

        public bool UnmodifyRotationSpeed(int key)
        {
            if (rotateSpeedMultiplier.RemoveEffector(key)) {
                RotateSpeed = rotateSpeed.Value * rotateSpeedMultiplier.Stat;
                return true;
            }
            return false;
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

        public void SetVelocity(Vector2 velocity)
        {
            Vector2 force = (velocity - _rigidBody.velocity) * _rigidBody.mass;
            float mag = force.magnitude;
            float max = Speed * Time.deltaTime;
            if (mag > max)
            {
                mag = max;
            }
            _rigidBody.AddForce(force.normalized * mag, ForceMode2D.Impulse);
            steering = Vector2.zero;
        }

        public void ApplyForce(Vector2 direction, float magnitude)
        {
            _rigidBody.AddForce(direction.normalized * magnitude, ForceMode2D.Impulse);
        }
        #endregion
    }
}
