using UnDirty;
using UnityEngine;

namespace InputManager.Runtime
{
    public class UInputManager : UBehaviour
    {
        public static UInputManager Instance { get; private set; }
        public InputStates InputStates;

        private CustomInput _input;


        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                throw new System.Exception("[INPUT]: InputManager has already an singleton instance.");
            }
            Instance = this;
        }

        public void OnEnable()
        {
            _input = new CustomInput();
            _input.Enable();

            _input.Player.Attack.started += _ => InputStates.Attack = true;
            _input.Player.Attack.canceled += _ => InputStates.Attack = false;

            _input.Player.Movement.performed += context => InputStates.Move = context.ReadValue<Vector2>();
        }

        public void OnDisable()
        {
            _input.Player.Attack.started -= _ => InputStates.Attack = true;
            _input.Player.Attack.canceled -= _ => InputStates.Attack = false;

            _input.Player.Movement.performed -= context => InputStates.Move = context.ReadValue<Vector2>();
            _input.Disable();
        }
    }
}