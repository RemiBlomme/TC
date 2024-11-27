using UnityEngine;

namespace InputManager.Runtime
{
    public struct InputStates
    {
        public Vector2 Move;
        public bool Attack;
    }

    public class InputStateSetter
    {
        public InputStates DefaultInputSate;
        public InputStates InputStates;

        private CustomInput _input;


        public void Enable()
        {
            _input = new CustomInput();
            _input.Enable();

            _input.Player.Attack.started += _ => InputStates.Attack = true;
            _input.Player.Attack.canceled += _ => InputStates.Attack = false;

            _input.Player.Movement.performed += context => InputStates.Move = context.ReadValue<Vector2>();
        }

        public void Disable()
        {
            _input.Player.Attack.started -= _ => InputStates.Attack = true;
            _input.Player.Attack.canceled -= _ => InputStates.Attack = false;

            _input.Player.Movement.performed -= context => InputStates.Move = context.ReadValue<Vector2>();
            _input.Disable();
        }

    }
}