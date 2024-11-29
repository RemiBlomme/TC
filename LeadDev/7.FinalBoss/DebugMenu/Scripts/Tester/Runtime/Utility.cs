using DebugMenu.Runtime;
using System;
using System.IO;
using UnityEngine;

namespace Tester.Runtime
{
    public static class Utility
    {
        [DebugMenu("Utility/Take Screenshot")]
        private static void TakeScreenshot()
        {
            if (!Directory.Exists(TEMP_FOLDER_PATH)) Directory.CreateDirectory(TEMP_FOLDER_PATH);

            DateTime time = DateTime.Now;
            var screenshotName = $"Screenshot_{time.ToString(DATETIME_FORMAT)}.png";

            string fullName = Path.Combine(TEMP_FOLDER_PATH, screenshotName);
            ScreenCapture.CaptureScreenshot(fullName, 2);
            Debug.Log($"Screenshot saved at : {fullName}");
        }

        private const string TEMP_FOLDER_PATH = "C:/temp";
        private const string DATETIME_FORMAT = "dd-MM-yyyy-HH-mm-ss";
    }
}