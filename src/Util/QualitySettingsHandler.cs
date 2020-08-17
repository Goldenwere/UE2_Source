using Entity2.Core;
using Entity2.Data;
using UnityEngine;

namespace Entity2.Util
{
    /// <summary>
    /// Handles changes to quality settings
    /// </summary>
    public class QualitySettingsHandler : MonoBehaviour
    {
        /// <summary>
        /// Sets graphics settings on MonoBehaviour.Start
        /// </summary>
        private void Start()
        {
            UpdateGraphicsSettings();
        }

        /// <summary>
        /// Subscribes to the UpdateGameSettings event OnEnable
        /// </summary>
        private void OnEnable()
        {
            GameEvents.UpdateGameSettings += UpdateGraphicsSettings;
        }

        /// <summary>
        /// Unsubscribes from the UpdateGameSettings event OnDisable
        /// </summary>
        private void OnDisable()
        {
            GameEvents.UpdateGameSettings -= UpdateGraphicsSettings;
        }

        /// <summary>
        /// Updates certain quality settings on UpdateGameSettings
        /// </summary>
        private void UpdateGraphicsSettings()
        {
            Application.targetFrameRate = PlayerSettings.Instance.Data.graphicsTargetFramerate;
            QualitySettings.shadowResolution = PlayerSettings.Instance.Data.graphicsShadowRes;
            if (PlayerSettings.Instance.Data.graphicsUseVsync)
                QualitySettings.vSyncCount = 1;
            else
                QualitySettings.vSyncCount = 0;
        }
    }
}