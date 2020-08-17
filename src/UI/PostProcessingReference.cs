using Entity2.Core;
using Entity2.Data;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Entity2.UI 
{ 
    /// <summary>
    /// Holds references to the post processing volumes in the game and handles changes to settings
    /// </summary>
    public class PostProcessingReference : MonoBehaviour
    {
        #region Fields & Properties
        [SerializeField]    private PostProcessVolume ppDepth;
        [SerializeField]    private PostProcessVolume ppOverlay;

        public static PostProcessingReference   Instance    { get; private set; }
        public        PostProcessVolume         PPDepth     { get { return ppDepth; } }
        public        PostProcessVolume         PPOverlay   { get { return ppOverlay; } }
        #endregion

        #region Methods
        /// <summary>
        /// Sets this as a singleton on Monobehaviour.Awake()
        /// </summary>
        private void Awake()
        {
            if (Instance != null)
                Destroy(gameObject);
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Sets post-processing settings on MonoBehaviour.Start
        /// </summary>
        private void Start()
        {
            UpdatePostProcessingSettings();
        }

        /// <summary>
        /// Subscribes to the UpdateGameSettings event OnEnable
        /// </summary>
        private void OnEnable()
        {
            GameEvents.UpdateGameSettings += UpdatePostProcessingSettings;
        }

        /// <summary>
        /// Unsubscribes from the UpdateGameSettings event OnDisable
        /// </summary>
        private void OnDisable()
        {
            GameEvents.UpdateGameSettings -= UpdatePostProcessingSettings;
        }

        /// <summary>
        /// Updates post-processing settings on UpdateGameSettings
        /// </summary>
        private void UpdatePostProcessingSettings()
        {
            switch(PlayerSettings.Instance.Data.postprocessAmbientOcclusionMode)
            {
                case 2:
                    ppDepth.profile.GetSetting<AmbientOcclusion>().enabled.value = true;
                    ppDepth.profile.GetSetting<AmbientOcclusion>().mode.value = AmbientOcclusionMode.ScalableAmbientObscurance;
                    break;
                case 1:
                    ppDepth.profile.GetSetting<AmbientOcclusion>().enabled.value = true;
                    ppDepth.profile.GetSetting<AmbientOcclusion>().mode.value = AmbientOcclusionMode.MultiScaleVolumetricObscurance;
                    break;
                case 0:
                default:
                    ppDepth.profile.GetSetting<AmbientOcclusion>().enabled.value = false;
                    break;
            }
            ppOverlay.profile.GetSetting<Bloom>().enabled.value = PlayerSettings.Instance.Data.postprocessBloomEnabled;
            ppOverlay.profile.GetSetting<ChromaticAberration>().enabled.value = PlayerSettings.Instance.Data.postprocessChromAbEnabled;
            ppDepth.profile.GetSetting<DepthOfField>().enabled.value = PlayerSettings.Instance.Data.postprocessDOFEnabled;
            ppDepth.profile.GetSetting<MotionBlur>().enabled.value = PlayerSettings.Instance.Data.postprocessMBEnabled;
            ppDepth.profile.GetSetting<ScreenSpaceReflections>().enabled.value = PlayerSettings.Instance.Data.postprocessSSREnabled;
        }
        #endregion
    }
}