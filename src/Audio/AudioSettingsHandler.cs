using Entity2.Core;
using Entity2.Data;
using UnityEngine;
using UnityEngine.Audio;

namespace Entity2.Audio
{
    /// <summary>
    /// Handles changes to audio settings
    /// </summary>
    public class AudioSettingsHandler : MonoBehaviour
    {
        [SerializeField]    private AudioMixer  gameAudioMixer;

        #region Methods
        /// <summary>
        /// Sets audio settings on MonoBehaviour.Start
        /// </summary>
        private void Start()
        {
            UpdateAudioSettings();
        }

        /// <summary>
        /// Subscribes to the UpdateGameSettings event OnEnable
        /// </summary>
        private void OnEnable()
        {
            GameEvents.UpdateGameSettings += UpdateAudioSettings;
        }

        /// <summary>
        /// Unsubscribes from the UpdateGameSettings event OnDisable
        /// </summary>
        private void OnDisable()
        {
            GameEvents.UpdateGameSettings -= UpdateAudioSettings;
        }

        /// <summary>
        /// Updates game audio levels on UpdateGameSettings
        /// </summary>
        private void UpdateAudioSettings()
        {
            gameAudioMixer.SetFloat(GameConstants.AudioParamEffects, Mathf.Log10(PlayerSettings.Instance.Data.audioEffects) * 20);
            gameAudioMixer.SetFloat(GameConstants.AudioParamEnvironment, Mathf.Log10(PlayerSettings.Instance.Data.audioEnvironment) * 20);
            gameAudioMixer.SetFloat(GameConstants.AudioParamInterface, Mathf.Log10(PlayerSettings.Instance.Data.audioInterface) * 20);
            gameAudioMixer.SetFloat(GameConstants.AudioParamMaster, Mathf.Log10(PlayerSettings.Instance.Data.audioMaster) * 20);
            gameAudioMixer.SetFloat(GameConstants.AudioParamMusic, Mathf.Log10(PlayerSettings.Instance.Data.audioMusic) * 20);
        }
        #endregion
    }
}