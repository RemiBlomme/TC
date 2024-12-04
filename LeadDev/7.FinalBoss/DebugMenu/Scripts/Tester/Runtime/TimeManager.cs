using DebugMenu.Runtime;
using UnityEngine;

namespace Tester.Runtime
{
    public class TimeManager : MonoBehaviour
    {
        [DebugMenu("Time/Increase Time Scale")]
        private void IncreaseTimeScale() => UpdateTimeScale(Time.timeScale + .2f);

        [DebugMenu("Time/Decrease Time Scale")]
        private void DecreaseTimeScale() => UpdateTimeScale(Time.timeScale - .2f);

        private void UpdateTimeScale(float scale)
        {
            if (scale < 0 || scale > 2) return;
            Time.timeScale = scale;
        }
    }
}