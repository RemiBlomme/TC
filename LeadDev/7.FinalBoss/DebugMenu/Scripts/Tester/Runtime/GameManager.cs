using DebugMenu.Runtime;
using TMPro;
using UnityEngine;

namespace Tester.Runtime
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        private void Awake() => UpateText();

        private void Update()
        {
            if (_difficulty != _lastValue)
            {
                _lastValue = _difficulty;
                UpateText();
            }
        }

        private void UpateText()
        {
            _text.SetText($"Difficulty : {_difficulty}");

            switch (_difficulty)
            {
                case DifficultyEnum.Easy:
                    _text.color = Color.green;
                    break;
                case DifficultyEnum.Normal:
                    _text.color = Color.yellow;
                    break;
                case DifficultyEnum.Hard:
                    _text.color = Color.red;
                    break;
            }
        }

        [DebugMenu("Game/Difficulty")] private DifficultyEnum _difficulty;
        private DifficultyEnum _lastValue;
    }
}