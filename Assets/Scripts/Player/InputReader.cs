using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ascent.Player
{
    public class InputReader : MonoBehaviour
    {
        [Header("Actions")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference lookAction;
        [SerializeField] private InputActionReference jumpAction;
        [SerializeField] private InputActionReference dashAction;
        [SerializeField] private InputActionReference groundPoundAction;
        
        public Vector2 MovementValue { get; private set; }
        public Vector2 LookValue { get; private set; }
        public event Action JumpEvent;
        public event Action JumpCanceledEvent;
        public event Action DashEvent;
        public event Action GroundPoundEvent;

        private void OnEnable()
        {
            if (moveAction != null)
            {
                moveAction.action.Enable();
                moveAction.action.performed += OnMove;
                moveAction.action.canceled += OnMove;
            }

            if (lookAction != null)
            {
                lookAction.action.Enable();
                lookAction.action.performed += OnLook;
                lookAction.action.canceled += OnLook;
            }

            if (jumpAction != null)
            {
                jumpAction.action.Enable();
                jumpAction.action.performed += OnJump;
                jumpAction.action.canceled += OnJumpCanceled;
            }

            if (dashAction != null)
            {
                dashAction.action.Enable();
                dashAction.action.performed += OnDash;
            }

            if (groundPoundAction != null)
            {
                groundPoundAction.action.Enable();
                groundPoundAction.action.performed += OnGroundPound;
            }
        }

        private void OnDisable()
        {
            if (moveAction != null)
            {
                moveAction.action.performed -= OnMove;
                moveAction.action.canceled -= OnMove;
                moveAction.action.Disable();
            }

            if (lookAction != null)
            {
                lookAction.action.performed -= OnLook;
                lookAction.action.canceled -= OnLook;
                lookAction.action.Disable();
            }

            if (jumpAction != null)
            {
                jumpAction.action.performed -= OnJump;
                jumpAction.action.canceled -= OnJumpCanceled;
                jumpAction.action.Disable();
            }

            if (dashAction != null)
            {
                dashAction.action.performed -= OnDash;
                dashAction.action.Disable();
            }

            if (groundPoundAction != null)
            {
                groundPoundAction.action.performed -= OnGroundPound;
                groundPoundAction.action.Disable();
            }
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            MovementValue = context.ReadValue<Vector2>();
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            LookValue = context.ReadValue<Vector2>();
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                JumpEvent?.Invoke();
            }
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                JumpCanceledEvent?.Invoke();
            }
        }

        private void OnDash(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                DashEvent?.Invoke();
            }
        }

        private void OnGroundPound(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                GroundPoundEvent?.Invoke();
            }
        }
    }
}
