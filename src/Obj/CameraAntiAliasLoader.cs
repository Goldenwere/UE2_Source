using Entity2.Core;
using Entity2.Data;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Entity2.Obj
{
    /// <summary>
    /// Attach this to any cameras that have a PostProcessLayer to load the AA setting at Start (and on UpdateGameSettings for the menu's camera)
    /// </summary>
    public class CameraAntiAliasLoader : MonoBehaviour
    {
        [SerializeField]    private int                 maxAAMode;
        [SerializeField]    private PostProcessLayer    ppLayer;

        /// <summary>
        /// Set the AA mode on MonoBehaviour.Start()
        /// </summary>
        private void Start()
        {
            ppLayer.antialiasingMode = (PostProcessLayer.Antialiasing)PlayerSettings.Instance.Data.postprocessAntialiasingMode;
            if ((int)ppLayer.antialiasingMode > maxAAMode)
                ppLayer.antialiasingMode = (PostProcessLayer.Antialiasing)maxAAMode;
        }

        /// <summary>
        /// Subscribe to the UpdateGameSettings event when enabled
        /// </summary>
        private void OnEnable()
        {
            GameEvents.UpdateGameSettings += OnUpdateGameSettings;
        }

        /// <summary>
        /// Unsubscribe from the UpdateGameSettings event when enabled
        /// </summary>
        private void OnDisable()
        {
            GameEvents.UpdateGameSettings -= OnUpdateGameSettings;
        }

        /// <summary>
        /// Handler for UpdateGameSettings, set the AA mode
        /// </summary>
        private void OnUpdateGameSettings()
        {
            ppLayer.antialiasingMode = (PostProcessLayer.Antialiasing)PlayerSettings.Instance.Data.postprocessAntialiasingMode;
            if ((int)ppLayer.antialiasingMode > maxAAMode)
                ppLayer.antialiasingMode = (PostProcessLayer.Antialiasing)maxAAMode;
        }
    }
}