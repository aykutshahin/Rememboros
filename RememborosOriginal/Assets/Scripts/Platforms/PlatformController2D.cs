using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rememboros
{
    [RequireComponent(typeof(PlatformMotor2D))]
    public class PlatformController2D : MonoBehaviour
    {
        [System.Serializable]
        class MovementSettings
        {
            public float Speed = 1;
        }

        [Header("Reference")]
        private PlatformMotor2D m_Motor;
        private BaseInputDriver m_InputDriver;

        [Header("Settings")]

        [SerializeField]
        private MovementSettings m_Movement = new MovementSettings();

        #region Monobehaviour

        private void Awake()
        {
            m_Motor = GetComponent<PlatformMotor2D>();
            m_InputDriver = GetComponent<BaseInputDriver>();
        }

        // Temporary Start
        private void Start()
        {
            Init();
        }

        // Temporary Update
        private void FixedUpdate()
        {
            _Update(Time.fixedDeltaTime);
        }

        #endregion

        public void Init()
        {
        }

        public void _Update(float timeStep)
        {
            // read input from input driver
            Vector2 input = new Vector2(m_InputDriver.Horizontal, m_InputDriver.Vertical);

            Vector3 velocity = new Vector3(input.x, input.y, 0) * m_Movement.Speed;

            m_Motor.Move(velocity);
        }

    }
}

