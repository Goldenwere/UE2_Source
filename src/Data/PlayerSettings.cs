using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using Entity2.Core;

namespace Entity2.Data
{
    /// <summary>
    /// Handles data IO for Settings (audio, graphics, post-processing)
    /// </summary>
    public class PlayerSettings : MonoBehaviour
    {
        #region Properties
        public        SettingsData      Data        { get; private set; }
        public static PlayerSettings    Instance    { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Sets singleton instance and loads settings on MonoBehaviour.Awake()
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
                if (gameObject.transform.parent == null)
                    DontDestroyOnLoad(this);
            }
        }

        /// <summary>
        /// Loads settings on MonoBehaviour.Start()
        /// </summary>
        private void Start()
        {
            LoadSettings();
        }

        /// <summary>
        /// Loads SettingsData from persistentDataPath
        /// </summary>
        public void LoadSettings()
        {
            if (File.Exists(Application.persistentDataPath + GameConstants.DataPathSettings))
            {
                XmlSerializer xs = null;
                TextReader txtReader = null;

                try
                {
                    xs = new XmlSerializer(typeof(SettingsData));
                    txtReader = new StreamReader(Application.persistentDataPath + GameConstants.DataPathSettings);
                    Data = (SettingsData)xs.Deserialize(txtReader);
                }

                catch (Exception e)
                {
                    // TO-DO: singleton exception handler that opens a UI canvas outputting errors
                }

                finally
                {
                    if (txtReader != null)
                        txtReader.Close();
                }
            }

            else
                SaveSettings(new SettingsData(false));
        }

        /// <summary>
        /// Saves SettingsData to persistentDataPath
        /// </summary>
        /// <param name="dataToSave">The data to replace this.Data with and save to persistentDataPath</param>
        public void SaveSettings(SettingsData dataToSave)
        {
            Data = dataToSave;
            GameEvents.Instance.CallUpdateGameSettings();
            XmlSerializer xs = null;
            TextWriter txtWriter = null;

            try
            {
                xs = new XmlSerializer(typeof(SettingsData));
                txtWriter = new StreamWriter(Application.persistentDataPath + GameConstants.DataPathSettings);
                xs.Serialize(txtWriter, Data);
            }

            catch (Exception e)
            {
                // TO-DO: singleton exception handler that opens a UI canvas outputting errors
            }

            finally
            {
                if (txtWriter != null)
                    txtWriter.Close();
            }
        }
        #endregion
    }

    /// <summary>
    /// Defines all the settings that can be set and saved in the Settings menu
    /// </summary>
    public struct SettingsData
    {
        #region Audio
        /// <summary>
        /// Used for setting the effects audio level (must be set between 0 and 1)
        /// </summary>
        public float audioEffects;

        /// <summary>
        /// Used for setting the environment audio level (must be set between 0 and 1)
        /// <para>(labeled Ambient in menu due to space)</para>
        /// </summary>
        public float audioEnvironment;

        /// <summary>
        /// Used for setting the interface audio level (must be set between 0 and 1)
        /// </summary>
        public float audioInterface;

        /// <summary>
        /// Used for setting the master audio level (must be set between 0 and 1)
        /// </summary>
        public float audioMaster;

        /// <summary>
        /// Used for setting the music audio level (must be set between 0 and 1)
        /// </summary>
        public float audioMusic;
        #endregion

        #region Graphics
        /// <summary>
        /// Used for setting QualitySettings.ShadowResolution based off of slider in Graphics menu
        /// </summary>
        public ShadowResolution graphicsShadowRes;

        /// <summary>
        /// Used for setting Application.targetFrameRate based off of slider in Graphics menu
        /// </summary>
        public int graphicsTargetFramerate;

        /// <summary>
        /// Used for setting QualitySettings.vSyncCount based off of toggle in graphics menu (only exposing 0 and 1 setting)
        /// </summary>
        public bool graphicsUseVsync;
        #endregion

        #region Post-Processing
        /// <summary>
        /// Used for switching between Off (0), MSVO (1), or SAO (2) in post-processing
        /// </summary>
        public int  postprocessAmbientOcclusionMode;

        /// <summary>
        /// Used for switching between Off (0), FXAA (1), SMAA (2), and TAA (3)
        /// </summary>
        public int  postprocessAntialiasingMode;

        /// <summary>
        /// Used for toggling motion-blur in post-processing
        /// </summary>
        public bool postprocessBloomEnabled;

        /// <summary>
        /// Used for toggling chromatic-abberation in post-processing
        /// </summary>
        public bool postprocessChromAbEnabled;

        /// <summary>
        /// Used for toggling depth-of-field in post-processing
        /// </summary>
        public bool postprocessDOFEnabled;

        /// <summary>
        /// Used for toggling motion blur in post-processing
        /// </summary>
        public bool postprocessMBEnabled;

        /// <summary>
        /// Used for toggling screen-space reflections in post-processing
        /// </summary>
        public bool postprocessSSREnabled;
        #endregion

        /// <summary>
        /// Creates SettingsData based off of whether to use post processing or not
        /// </summary>
        /// <param name="usePostProcess">Sets all post-processing settings to this value</param>
        public SettingsData(bool usePostProcess)
        {
            audioEffects = 1;
            audioEnvironment = 1;
            audioInterface = 1;
            audioMaster = 1;
            audioMusic = 1;

            graphicsShadowRes = ShadowResolution.Low;
            graphicsTargetFramerate = 60;
            graphicsUseVsync = true;

            postprocessAmbientOcclusionMode = 0;
            postprocessAntialiasingMode = 0;
            postprocessBloomEnabled = usePostProcess;
            postprocessChromAbEnabled = usePostProcess;
            postprocessDOFEnabled = usePostProcess;
            postprocessMBEnabled = usePostProcess;
            postprocessSSREnabled = usePostProcess;
        }
    }
}