using TMPro;
using Entity2.Core;
using Entity2.Data;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Goldenwere.Unity;
using Goldenwere.Unity.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Entity2.UI
{
    /// <summary>
    /// Used for handling UI calls from buttons on the game's main menu and sub-menus
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        #region Fields
        [SerializeField]    private DefaultImages                   defaultImages;
        [SerializeField]    private MenuControls                    elementsControls;
        [SerializeField]    private MenuSettings                    elementsSettings;
        [SerializeField]    private EventSystem                     eventSystem;
        [SerializeField]    private GameObject                      firstSelectedControls;
        [SerializeField]    private GameObject                      firstSelectedMain;
        [SerializeField]    private GameObject                      firstSelectedQuit;
        [SerializeField]    private GameObject                      firstSelectedSettings;
        [SerializeField]    private GameObject                      menuCanvasControls;
        [SerializeField]    private GameObject                      menuCanvasMain;
        [SerializeField]    private GameObject                      menuCanvasQuit;
        [SerializeField]    private GameObject                      menuCanvasSettings;
        /**************/    private Dictionary<GameObject, Vector3> menuInitialPositions;
        /**************/    private Dictionary<GameObject, Vector3> menuInitialScales;
        /**************/    private ControlsData                    workingControlsData;
        /**************/    private SettingsData                    workingSettingsData;
        #endregion

        #region Inline Classes
        /// <summary>
        /// Default sprites for controls
        /// </summary>
        [Serializable] public class DefaultImages
        {
            #region Serialized Fields
            [SerializeField]    private Sprite  gamepad_button_east;
            [SerializeField]    private Sprite  gamepad_button_north;
            [SerializeField]    private Sprite  gamepad_button_select;
            [SerializeField]    private Sprite  gamepad_button_south;
            [SerializeField]    private Sprite  gamepad_button_start;
            [SerializeField]    private Sprite  gamepad_button_west;
            [SerializeField]    private Sprite  gamepad_dpad_down;
            [SerializeField]    private Sprite  gamepad_dpad_left;
            [SerializeField]    private Sprite  gamepad_dpad_right;
            [SerializeField]    private Sprite  gamepad_dpad_up;
            [SerializeField]    private Sprite  gamepad_lthumbstick_down;
            [SerializeField]    private Sprite  gamepad_lthumbstick_left;
            [SerializeField]    private Sprite  gamepad_lthumbstick_press;
            [SerializeField]    private Sprite  gamepad_lthumbstick_right;
            [SerializeField]    private Sprite  gamepad_lthumbstick_up;
            [SerializeField]    private Sprite  gamepad_lshoulder;
            [SerializeField]    private Sprite  gamepad_ltrigger;
            [SerializeField]    private Sprite  gamepad_rthumbstick_down;
            [SerializeField]    private Sprite  gamepad_rthumbstick_left;
            [SerializeField]    private Sprite  gamepad_rthumbstick_press;
            [SerializeField]    private Sprite  gamepad_rthumbstick_right;
            [SerializeField]    private Sprite  gamepad_rthumbstick_up;
            [SerializeField]    private Sprite  gamepad_rshoulder;
            [SerializeField]    private Sprite  gamepad_rtrigger;

            [SerializeField]    private Sprite  keyboard_backspace;
            [SerializeField]    private Sprite  keyboard_caps;
            [SerializeField]    private Sprite  keyboard_down;
            [SerializeField]    private Sprite  keyboard_enter;
            [SerializeField]    private Sprite  keyboard_left;
            [SerializeField]    private Sprite  keyboard_lshift;
            [SerializeField]    private Sprite  keyboard_num;
            [SerializeField]    private Sprite  keyboard_right;
            [SerializeField]    private Sprite  keyboard_rshift;
            [SerializeField]    private Sprite  keyboard_scr;
            [SerializeField]    private Sprite  keyboard_space;
            [SerializeField]    private Sprite  keyboard_tab;
            [SerializeField]    private Sprite  keyboard_up;

            [SerializeField]    private Sprite  mouse_back;
            [SerializeField]    private Sprite  mouse_forward;
            [SerializeField]    private Sprite  mouse_left;
            [SerializeField]    private Sprite  mouse_middle;
            [SerializeField]    private Sprite  mouse_right;
            #endregion

            #region Properties
            public Sprite   Gamepad_button_east         { get { return gamepad_button_east; } }
            public Sprite   Gamepad_button_north        { get { return gamepad_button_north; } }
            public Sprite   Gamepad_button_select       { get { return gamepad_button_select; } }
            public Sprite   Gamepad_button_south        { get { return gamepad_button_south; } }
            public Sprite   Gamepad_button_start        { get { return gamepad_button_start; } }
            public Sprite   Gamepad_button_west         { get { return gamepad_button_west; } }
            public Sprite   Gamepad_dpad_down           { get { return gamepad_dpad_down; } }
            public Sprite   Gamepad_dpad_left           { get { return gamepad_dpad_left; } }
            public Sprite   Gamepad_dpad_right          { get { return gamepad_dpad_right; } }
            public Sprite   Gamepad_dpad_up             { get { return gamepad_dpad_up; } }
            public Sprite   Gamepad_lthumbstick_down    { get { return gamepad_lthumbstick_down; } }
            public Sprite   Gamepad_lthumbstick_left    { get { return gamepad_lthumbstick_left; } }
            public Sprite   Gamepad_lthumbstick_press   { get { return gamepad_lthumbstick_press; } }
            public Sprite   Gamepad_lthumbstick_right   { get { return gamepad_lthumbstick_right; } }
            public Sprite   Gamepad_lthumbstick_up      { get { return gamepad_lthumbstick_up; } }
            public Sprite   Gamepad_lshoulder           { get { return gamepad_lshoulder; } }
            public Sprite   Gamepad_ltrigger            { get { return gamepad_ltrigger; } }
            public Sprite   Gamepad_rthumbstick_down    { get { return gamepad_rthumbstick_down; } }
            public Sprite   Gamepad_rthumbstick_left    { get { return gamepad_rthumbstick_left; } }
            public Sprite   Gamepad_rthumbstick_press   { get { return gamepad_rthumbstick_press; } }
            public Sprite   Gamepad_rthumbstick_right   { get { return gamepad_rthumbstick_right; } }
            public Sprite   Gamepad_rthumbstick_up      { get { return gamepad_rthumbstick_up; } }
            public Sprite   Gamepad_rshoulder           { get { return gamepad_rshoulder; } }
            public Sprite   Gamepad_rtrigger            { get { return gamepad_rtrigger; } }

            public Sprite   Keyboard_backspace          { get { return keyboard_backspace; } }
            public Sprite   Keyboard_caps               { get { return keyboard_caps; } }
            public Sprite   Keyboard_down               { get { return keyboard_down; } }
            public Sprite   Keyboard_enter              { get { return keyboard_enter; } }
            public Sprite   Keyboard_left               { get { return keyboard_left; } }
            public Sprite   Keyboard_lshift             { get { return keyboard_lshift; } }
            public Sprite   Keyboard_num                { get { return keyboard_num; } }
            public Sprite   Keyboard_right              { get { return keyboard_right; } }
            public Sprite   Keyboard_rshift             { get { return keyboard_rshift; } }
            public Sprite   Keyboard_scr                { get { return keyboard_scr; } }
            public Sprite   Keyboard_space              { get { return keyboard_space; } }
            public Sprite   Keyboard_tab                { get { return keyboard_tab; } }
            public Sprite   Keyboard_up                 { get { return keyboard_up; } }

            public Sprite   Mouse_back                  { get { return mouse_back; } }
            public Sprite   Mouse_forward               { get { return mouse_forward; } }
            public Sprite   Mouse_left                  { get { return mouse_left; } }
            public Sprite   Mouse_middle                { get { return mouse_middle; } }
            public Sprite   Mouse_right                 { get { return mouse_right; } }
            #endregion
        }

        /// <summary>
        /// Elements on the controls menu
        /// </summary>
        [Serializable] public class MenuControls
        {
            #region Serialized Fields
            [SerializeField]    private Button                  button_Gameplay_Console;
            [SerializeField]    private Button                  button_Gameplay_Interact;
            [SerializeField]    private Button                  button_Gameplay_Jump;
            [SerializeField]    private Button                  button_Gameplay_Pause;
            [SerializeField]    private Button                  button_Gameplay_Sprint;
            [SerializeField]    private Button                  button_Gameplay_WeaponFire;
            [SerializeField]    private Button                  button_Gameplay_WeaponSwitch;
            [SerializeField]    private Button                  button_Movement_Forward;
            [SerializeField]    private Button                  button_Movement_Left;
            [SerializeField]    private Button                  button_Movement_Right;
            [SerializeField]    private Button                  button_Movement_Reverse;
            [SerializeField]    private Button                  button_Rotation_Down;
            [SerializeField]    private Button                  button_Rotation_Left;
            [SerializeField]    private Button                  button_Rotation_Right;
            [SerializeField]    private Button                  button_Rotation_Up;
            [SerializeField]    private TMP_Dropdown            dropdown_InputOption;
            [SerializeField]    private CanvasGroup             group_AwaitInput;           // Open this when listening for input
            [SerializeField]    private CanvasGroup             group_RotationGamepad;      // Toggle this when dropdown has gamepad selected
            [SerializeField]    private CanvasGroup             group_RotationMouse;        // Toggle this when dropdown has keyboard/mouse selected
            [SerializeField]    private SliderTextLoadExtension slider_Sensitivity;
            [SerializeField]    private TMP_Text                text_AwaitInputAction;
            [SerializeField]    private Toggle                  toggle_MouseLook_Inverted;
            [SerializeField]    private Toggle                  toggle_Setting_FOVShift;
            [SerializeField]    private Toggle                  toggle_Setting_Headbob;
            [SerializeField]    private Toggle                  toggle_Setting_HUDMotion;
            [SerializeField]    private Toggle                  toggle_Setting_PersistentHUD;
            #endregion

            #region Properties
            public Button                   Button_Gameplay_Console         { get { return button_Gameplay_Console; } }
            public Button                   Button_Gameplay_Interact        { get { return button_Gameplay_Interact; } }
            public Button                   Button_Gameplay_Jump            { get { return button_Gameplay_Jump; } }
            public Button                   Button_Gameplay_Pause           { get { return button_Gameplay_Pause; } }
            public Button                   Button_Gameplay_Sprint          { get { return button_Gameplay_Sprint; } }
            public Button                   Button_Gameplay_WeaponFire      { get { return button_Gameplay_WeaponFire; } }
            public Button                   Button_Gameplay_WeaponSwitch    { get { return button_Gameplay_WeaponSwitch; } }
            public Button                   Button_Movement_Forward         { get { return button_Movement_Forward; } }
            public Button                   Button_Movement_Left            { get { return button_Movement_Left; } }
            public Button                   Button_Movement_Right           { get { return button_Movement_Right; } }
            public Button                   Button_Movement_Reverse         { get { return button_Movement_Reverse; } }
            public Button                   Button_Rotation_Down            { get { return button_Rotation_Down; } }
            public Button                   Button_Rotation_Left            { get { return button_Rotation_Left; } }
            public Button                   Button_Rotation_Right           { get { return button_Rotation_Right; } }
            public Button                   Button_Rotation_Up              { get { return button_Rotation_Up; } }
            public TMP_Dropdown             Dropdown_InputOption            { get { return dropdown_InputOption; } }
            public CanvasGroup              Group_AwaitInput                { get { return group_AwaitInput; } }
            public CanvasGroup              Group_RotationGamepad           { get { return group_RotationGamepad; } }
            public CanvasGroup              Group_RotationMouse             { get { return group_RotationMouse; } }
            public SliderTextLoadExtension  Slider_Sensitivity              { get { return slider_Sensitivity; } }
            public TMP_Text                 Text_AwaitInputAction           { get { return text_AwaitInputAction; } }
            public Toggle                   Toggle_MouseLook_Inverted       { get { return toggle_MouseLook_Inverted; } }
            public Toggle                   Toggle_Setting_FOVShift         { get { return toggle_Setting_FOVShift; } }
            public Toggle                   Toggle_Setting_Headbob          { get { return toggle_Setting_Headbob; } }
            public Toggle                   Toggle_Setting_HUDMotion        { get { return toggle_Setting_HUDMotion; } }
            public Toggle                   Toggle_Setting_PersistentHUD    { get { return toggle_Setting_PersistentHUD; } }
            #endregion
        }

        /// <summary>
        /// Elements on the settings menu
        /// </summary>
        [Serializable] public class MenuSettings
        {
            #region Serialized Fields
            [SerializeField]    private Slider                  sliderAudio_Effects;
            [SerializeField]    private Slider                  sliderAudio_Environment;
            [SerializeField]    private Slider                  sliderAudio_Interface;
            [SerializeField]    private Slider                  sliderAudio_Master;
            [SerializeField]    private Slider                  sliderAudio_Music;
            [SerializeField]    private SliderTextLoadExtension sliderGFX_ShadowRes;
            [SerializeField]    private SliderTextLoadExtension sliderGFX_Framerate;
            [SerializeField]    private SliderTextLoadExtension sliderPP_AA;
            [SerializeField]    private SliderTextLoadExtension sliderPP_AO;
            [SerializeField]    private Toggle                  toggleGFX_Vsync;
            [SerializeField]    private Toggle                  togglePP_Bloom;
            [SerializeField]    private Toggle                  togglePP_ChromAb;
            [SerializeField]    private Toggle                  togglePP_DOF;
            [SerializeField]    private Toggle                  togglePP_MB;
            [SerializeField]    private Toggle                  togglePP_SSR;
            #endregion

            #region Properties
            public Slider                  SliderAudio_Effects     { get { return sliderAudio_Effects; } }
            public Slider                  SliderAudio_Environment { get { return sliderAudio_Environment; } }
            public Slider                  SliderAudio_Interface   { get { return sliderAudio_Interface; } }
            public Slider                  SliderAudio_Master      { get { return sliderAudio_Master; } }
            public Slider                  SliderAudio_Music       { get { return sliderAudio_Music; } }
            public SliderTextLoadExtension SliderGFX_ShadowRes     { get { return sliderGFX_ShadowRes; } }
            public SliderTextLoadExtension SliderGFX_Framerate     { get { return sliderGFX_Framerate; } }
            public SliderTextLoadExtension SliderPP_AA             { get { return sliderPP_AA; } }
            public SliderTextLoadExtension SliderPP_AO             { get { return sliderPP_AO; } }
            public Toggle                  ToggleGFX_Vsync         { get { return toggleGFX_Vsync; } }
            public Toggle                  TogglePP_Bloom          { get { return togglePP_Bloom; } }
            public Toggle                  TogglePP_ChromAb        { get { return togglePP_ChromAb; } }
            public Toggle                  TogglePP_DOF            { get { return togglePP_DOF; } }
            public Toggle                  TogglePP_MB             { get { return togglePP_MB; } }
            public Toggle                  TogglePP_SSR            { get { return togglePP_SSR; } }
            #endregion
        }
        #endregion

        #region Methods
        /// <summary>
        /// On MonoBehaviour.Start, grab the initial positions/scales
        /// </summary>
        private void Start()
        {
            menuInitialPositions = new Dictionary<GameObject, Vector3>
            {
                { menuCanvasControls,   menuCanvasControls.transform.localPosition  },
                { menuCanvasMain,       menuCanvasMain.transform.localPosition      },
                { menuCanvasQuit,       menuCanvasQuit.transform.localPosition      },
                { menuCanvasSettings,   menuCanvasSettings.transform.localPosition  }
            };

            menuInitialScales = new Dictionary<GameObject, Vector3>
            {
                { menuCanvasControls,   menuCanvasControls.transform.localScale     },
                { menuCanvasMain,       menuCanvasMain.transform.localScale         },
                { menuCanvasQuit,       menuCanvasQuit.transform.localScale         },
                { menuCanvasSettings,   menuCanvasSettings.transform.localScale     }
            };

            menuCanvasControls.SetActive(false);
            menuCanvasMain.SetActive(true);
            menuCanvasQuit.SetActive(false);
            menuCanvasSettings.SetActive(false);
        }

        #region Data IO
        /// <summary>
        /// Sets all the controls elements to the current Data in PlayerControls; called whenever entering the controls menu
        /// </summary>
        public void DataLoadControls()
        {
            workingControlsData = PlayerControls.Instance.Data;
            DataLoadControls(elementsControls.Dropdown_InputOption.value);
        }

        /// <summary>
        /// Updates controls elements based on the currently selected option from the input option dropdown
        /// </summary>
        /// <param name="mode">TMP_Dropdown.value to input mode (0 - keyboard/mouse, 1 - gamepad, mirrors ControllerActions and ControlsData)</param>
        public void DataLoadControls(int mode)
        {
            ControlBinding[] bindings = workingControlsData.controlsKeyboard;
            if (mode == 1)
            {
                bindings = workingControlsData.controlsGamepad;
                elementsControls.Group_RotationGamepad.gameObject.SetActive(true);
                elementsControls.Group_RotationMouse.gameObject.SetActive(false);
                SetDisplayedValue(elementsControls.Button_Rotation_Down, bindings.First(b => b.control == Control.Rotation_Down).path);
                SetDisplayedValue(elementsControls.Button_Rotation_Left, bindings.First(b => b.control == Control.Rotation_Left).path);
                SetDisplayedValue(elementsControls.Button_Rotation_Right, bindings.First(b => b.control == Control.Rotation_Right).path);
                SetDisplayedValue(elementsControls.Button_Rotation_Up, bindings.First(b => b.control == Control.Rotation_Up).path);
            }

            else
            {
                elementsControls.Group_RotationGamepad.gameObject.SetActive(false);
                elementsControls.Group_RotationMouse.gameObject.SetActive(true);
                elementsControls.Toggle_MouseLook_Inverted.SetIsOnWithoutNotify(workingControlsData.mouseLookInverted);
            }

            SetDisplayedValue(elementsControls.Button_Gameplay_Console, bindings.First(b => b.control == Control.Gameplay_Console).path);
            SetDisplayedValue(elementsControls.Button_Gameplay_Interact, bindings.First(b => b.control == Control.Gameplay_Interact).path);
            SetDisplayedValue(elementsControls.Button_Gameplay_Jump, bindings.First(b => b.control == Control.Gameplay_Jump).path);
            SetDisplayedValue(elementsControls.Button_Gameplay_Pause, bindings.First(b => b.control == Control.Gameplay_Pause).path);
            SetDisplayedValue(elementsControls.Button_Gameplay_Sprint, bindings.First(b => b.control == Control.Gameplay_Sprint).path);
            SetDisplayedValue(elementsControls.Button_Gameplay_WeaponFire, bindings.First(b => b.control == Control.Gameplay_WeaponFire).path);
            SetDisplayedValue(elementsControls.Button_Gameplay_WeaponSwitch, bindings.First(b => b.control == Control.Gameplay_WeaponSwitch).path);
            SetDisplayedValue(elementsControls.Button_Movement_Forward, bindings.First(b => b.control == Control.Movement_Forward).path);
            SetDisplayedValue(elementsControls.Button_Movement_Left, bindings.First(b => b.control == Control.Movement_Left).path);
            SetDisplayedValue(elementsControls.Button_Movement_Reverse, bindings.First(b => b.control == Control.Movement_Reverse).path);
            SetDisplayedValue(elementsControls.Button_Movement_Right, bindings.First(b => b.control == Control.Movement_Right).path);

            elementsControls.Toggle_Setting_FOVShift.SetIsOnWithoutNotify(workingControlsData.settingFOVShift);
            elementsControls.Toggle_Setting_Headbob.SetIsOnWithoutNotify(workingControlsData.settingHeadbob);
            elementsControls.Toggle_Setting_HUDMotion.SetIsOnWithoutNotify(workingControlsData.settingHUDMotion);
            elementsControls.Toggle_Setting_PersistentHUD.SetIsOnWithoutNotify(workingControlsData.settingPersistentHUD);
            elementsControls.Slider_Sensitivity.AssociatedSlider.SetValueWithoutNotify(workingControlsData.settingRotationSensitivity);
            elementsControls.Slider_Sensitivity.UpdateText(workingControlsData.settingRotationSensitivity);
            elementsControls.Group_AwaitInput.gameObject.SetActive(false);
        }

        /// <summary>
        /// Sets all the settings elements to the current Data in PlayerSettings; called whenever entering the settings menu
        /// </summary>
        public void DataLoadSettings()
        {
            elementsSettings.SliderAudio_Effects.SetValueWithoutNotify(PlayerSettings.Instance.Data.audioEffects);
            elementsSettings.SliderAudio_Environment.SetValueWithoutNotify(PlayerSettings.Instance.Data.audioEnvironment);
            elementsSettings.SliderAudio_Interface.SetValueWithoutNotify(PlayerSettings.Instance.Data.audioInterface);
            elementsSettings.SliderAudio_Master.SetValueWithoutNotify(PlayerSettings.Instance.Data.audioMaster);
            elementsSettings.SliderAudio_Music.SetValueWithoutNotify(PlayerSettings.Instance.Data.audioMusic);
            elementsSettings.SliderGFX_ShadowRes.UpdateText(PlayerSettings.Instance.Data.graphicsShadowRes.ToString());
            elementsSettings.SliderGFX_ShadowRes.AssociatedSlider.SetValueWithoutNotify((int)PlayerSettings.Instance.Data.graphicsShadowRes);
            elementsSettings.SliderGFX_Framerate.UpdateText(PlayerSettings.Instance.Data.graphicsTargetFramerate);
            elementsSettings.SliderGFX_Framerate.AssociatedSlider.SetValueWithoutNotify(PlayerSettings.Instance.Data.graphicsTargetFramerate);
            elementsSettings.SliderPP_AA.UpdateText(TranslateAntialias(PlayerSettings.Instance.Data.postprocessAntialiasingMode));
            elementsSettings.SliderPP_AA.AssociatedSlider.SetValueWithoutNotify(PlayerSettings.Instance.Data.postprocessAntialiasingMode);
            elementsSettings.SliderPP_AO.UpdateText(TranslateAmbientOcclusion(PlayerSettings.Instance.Data.postprocessAmbientOcclusionMode));
            elementsSettings.SliderPP_AO.AssociatedSlider.SetValueWithoutNotify(PlayerSettings.Instance.Data.postprocessAmbientOcclusionMode);
            elementsSettings.ToggleGFX_Vsync.isOn = PlayerSettings.Instance.Data.graphicsUseVsync;
            elementsSettings.TogglePP_Bloom.isOn = PlayerSettings.Instance.Data.postprocessBloomEnabled;
            elementsSettings.TogglePP_ChromAb.isOn = PlayerSettings.Instance.Data.postprocessChromAbEnabled;
            elementsSettings.TogglePP_DOF.isOn = PlayerSettings.Instance.Data.postprocessDOFEnabled;
            elementsSettings.TogglePP_MB.isOn = PlayerSettings.Instance.Data.postprocessMBEnabled;
            elementsSettings.TogglePP_SSR.isOn = PlayerSettings.Instance.Data.postprocessSSREnabled;

            workingSettingsData = PlayerSettings.Instance.Data;
        }

        /// <summary>
        /// Saves all the settings defined in workingControlsData
        /// </summary>
        public void DataSaveControls()
        {
            PlayerControls.Instance.SaveControls(workingControlsData);
        }

        /// <summary>
        /// Saves all the settings defined in workingSettingsData
        /// </summary>
        public void DataSaveSettings()
        {
            PlayerSettings.Instance.SaveSettings(workingSettingsData);
        }
        #endregion

        /// <summary>
        /// Temporary method to load the game
        /// </summary>
        public void LoadGame()
        {
            StartCoroutine(LoadSL2());
        }

        #region Menu Switching Button Handlers
        /// <summary>
        /// Used by the menu button on the controls menu
        /// </summary>
        public void LoadMainFromControls()
        {
            PlayerControls.Instance.SetInputOverrides();
            StartCoroutine(TransitionMenu(menuCanvasMain, false));
            StartCoroutine(TransitionMenu(menuCanvasControls, true));
            eventSystem.SetSelectedGameObject(firstSelectedMain);
        }

        /// <summary>
        /// Used by the menu button on the settings menu
        /// </summary>
        public void LoadMainFromSettings()
        {
            StartCoroutine(TransitionMenu(menuCanvasMain, false));
            StartCoroutine(TransitionMenu(menuCanvasSettings, true));
            eventSystem.SetSelectedGameObject(firstSelectedMain);
        }

        /// <summary>
        /// Used by the menu button on the quit prompt menu
        /// </summary>
        public void LoadMainFromQuit()
        {
            StartCoroutine(TransitionMenu(menuCanvasMain, false));
            StartCoroutine(TransitionMenu(menuCanvasQuit, true));
            eventSystem.SetSelectedGameObject(firstSelectedMain);
        }

        /// <summary>
        /// Used by the controls button on the main menu
        /// </summary>
        public void LoadMenuControls()
        {
            DataLoadControls();
            StartCoroutine(TransitionMenu(menuCanvasMain, true));
            StartCoroutine(TransitionMenu(menuCanvasControls, false));
            eventSystem.SetSelectedGameObject(firstSelectedControls);
        }

        /// <summary>
        /// Used by the settings button on the main menu
        /// </summary>
        public void LoadMenuSettings()
        {
            DataLoadSettings();
            StartCoroutine(TransitionMenu(menuCanvasMain, true));
            StartCoroutine(TransitionMenu(menuCanvasSettings, false));
            eventSystem.SetSelectedGameObject(firstSelectedSettings);
        }

        /// <summary>
        /// Used by the quit button on the main menu
        /// </summary>
        public void LoadMenuQuit()
        {
            StartCoroutine(TransitionMenu(menuCanvasMain, true));
            StartCoroutine(TransitionMenu(menuCanvasQuit, false));
            eventSystem.SetSelectedGameObject(firstSelectedQuit);
        }
        #endregion

        #region Controls Menu Handlers
        /// <summary>
        /// Update a control from control parameter
        /// </summary>
        /// <param name="controlBeingChanged">Valid control defined in Entity2.Data.Control</param>
        public void UpdateControls_CallAwaitInput(string controlBeingChanged)
        {
            if (Enum.TryParse(controlBeingChanged, out Control control))
            {
                elementsControls.Group_AwaitInput.gameObject.SetActive(true);
                elementsControls.Text_AwaitInputAction.text = control.ToString().Replace("_", " ").Replace("Gameplay", "");
                string pathStart = "Keyboard";
                if (elementsControls.Dropdown_InputOption.value == 1)
                    pathStart = "Gamepad";

                InputAction action = ControlBinding.ControlToAction(control, pathStart, out int index);
                InputActionRebindingExtensions.RebindingOperation rebindOp = action
                        .PerformInteractiveRebinding()
                        .WithControlsExcluding("Mouse/delta")
                        .WithExpectedControlType("Button")
                        .OnMatchWaitForAnother(0.1f);
                if (index > -1)
                    rebindOp.WithTargetBinding(index);

                if (elementsControls.Dropdown_InputOption.value == 1)
                {
                    rebindOp.WithControlsExcluding("Keyboard");
                    rebindOp.WithControlsExcluding("Mouse");
                    rebindOp.WithCancelingThrough("Keyboard/escape");
                }
                else
                {
                    rebindOp.WithControlsExcluding("Gamepad");
                    rebindOp.WithControlsExcluding("Joystick");
                }

                rebindOp.Start()
                    .OnCancel(callback =>
                    {
                        elementsControls.Group_AwaitInput.gameObject.SetActive(false);
                        rebindOp?.Dispose();
                    })
                    .OnComplete(callback =>
                    {
                        if (elementsControls.Dropdown_InputOption.value == 1)
                        {
                            ControlBinding cb = workingControlsData.controlsGamepad.First(b => b.control == control);
                            if (index > -1)
                                workingControlsData.controlsGamepad[Array.IndexOf(workingControlsData.controlsGamepad, cb)].path = action.actionMap.bindings[index].overridePath;
                            else
                                workingControlsData.controlsGamepad[Array.IndexOf(workingControlsData.controlsGamepad, cb)].path = action.bindings[1].overridePath;
                        }
                        else
                        {
                            ControlBinding cb = workingControlsData.controlsKeyboard.First(b => b.control == control);
                            if (index > -1)
                                workingControlsData.controlsKeyboard[Array.IndexOf(workingControlsData.controlsKeyboard, cb)].path = action.actionMap.bindings[index].overridePath;
                            else
                                workingControlsData.controlsKeyboard[Array.IndexOf(workingControlsData.controlsKeyboard, cb)].path = action.bindings[0].overridePath;
                        }
                        DataLoadControls(elementsControls.Dropdown_InputOption.value);
                        elementsControls.Group_AwaitInput.gameObject.SetActive(false);
                        rebindOp?.Dispose();
                    });
            }
        }

        /// <summary>
        /// Update mouselook inverted setting from toggle value
        /// </summary>
        /// <param name="newVal">The new value</param>
        public void UpdateControls_MouseLook_Inverted(bool newVal)
        {
            workingControlsData.mouseLookInverted = newVal;
        }

        /// <summary>
        /// Update FOV shifting setting from toggle value
        /// </summary>
        /// <param name="newVal">The new value</param>
        public void UpdateControls_Settings_FOVShift(bool newVal)
        {
            workingControlsData.settingFOVShift = newVal;
        }

        /// <summary>
        /// Update headbob setting from toggle value
        /// </summary>
        /// <param name="newVal">The new value</param>
        public void UpdateControls_Settings_Headbob(bool newVal)
        {
            workingControlsData.settingHeadbob = newVal;
        }

        /// <summary>
        /// Update HUD motion setting from toggle value
        /// </summary>
        /// <param name="newVal">The new value</param>
        public void UpdateControls_Settings_HUDMotion(bool newVal)
        {
            workingControlsData.settingHUDMotion = newVal;
        }

        /// <summary>
        /// Update persistent HUD setting from toggle value
        /// </summary>
        /// <param name="newVal">The new value</param>
        public void UpdateControls_Settings_PersistentHUD(bool newVal)
        {
            workingControlsData.settingPersistentHUD = newVal;
        }

        /// <summary>
        /// Update selected option for input manipulation (note: not an actual setting like the remaining members of the controls menu)
        /// </summary>
        /// <param name="option">The new option</param>
        public void UpdateControls_Settings_SelectedInput(int option)
        {
            DataLoadControls(option);
        }

        /// <summary>
        /// Update rotation sensitivity from slider value
        /// </summary>
        /// <param name="option">The new value</param>
        public void UpdateControls_Settings_Sensitivity(float newVal)
        {
            newVal = (float)Math.Round(newVal, 2);
            workingControlsData.settingRotationSensitivity = newVal;
            elementsControls.Slider_Sensitivity.UpdateText(newVal);
        }
        #endregion

        #region Settings Menu On Value Changed Handlers
        /// <summary>
        /// Dynamically update effects volume setting from slider value
        /// </summary>
        /// <param name="newVal">The new value</param>
        public void UpdateSetting_Audio_Effects(float newVal)
        {
            workingSettingsData.audioEffects = newVal;
        }

        /// <summary>
        /// Dynamically update environment volume setting from slider value
        /// </summary>
        /// <param name="newVal">The new value</param>
        public void UpdateSetting_Audio_Environment(float newVal)
        {
            workingSettingsData.audioEnvironment = newVal;
        }

        /// <summary>
        /// Dynamically update interface volume setting from slider value
        /// </summary>
        /// <param name="newVal">The new value</param>
        public void UpdateSetting_Audio_Interface(float newVal)
        {
            workingSettingsData.audioInterface = newVal;
        }

        /// <summary>
        /// Dynamically update master volume setting from slider value
        /// </summary>
        /// <param name="newVal">The new value</param>
        public void UpdateSetting_Audio_Master(float newVal)
        {
            workingSettingsData.audioMaster = newVal;
        }

        /// <summary>
        /// Dynamically update music volume setting from slider value
        /// </summary>
        /// <param name="newVal">The new value</param>
        public void UpdateSetting_Audio_Music(float newVal)
        {
            workingSettingsData.audioMusic = newVal;
        }

        /// <summary>
        /// Dynamically update shadow resolution setting from slider value
        /// </summary>
        /// <param name="newVal">The new value</param>
        public void UpdateSetting_Graphics_ShadowRes(float newVal)
        {
            workingSettingsData.graphicsShadowRes = (ShadowResolution)(int)newVal;
            elementsSettings.SliderGFX_ShadowRes.UpdateText(workingSettingsData.graphicsShadowRes.ToString());
        }

        /// <summary>
        /// Dynamically update target framerate setting from slider value
        /// </summary>
        /// <param name="newVal">The new value</param>
        public void UpdateSetting_Graphics_TargetFramerate(float newVal)
        {
            workingSettingsData.graphicsTargetFramerate = (int)newVal;
            elementsSettings.SliderGFX_Framerate.UpdateText(newVal);
        }

        /// <summary>
        /// Dynamically update vsync setting from toggle value
        /// </summary>
        /// <param name="newVal">The new value</param>
        public void UpdateSetting_Graphics_UseVsync(bool newVal)
        {
            workingSettingsData.graphicsUseVsync = newVal;
        }

        /// <summary>
        /// Dynamically update ambient occlusion setting from slider value
        /// </summary>
        /// <param name="newVal">The new value</param>
        public void UpdateSetting_PostProcess_AmbientOcclusionMode(float newVal)
        {
            workingSettingsData.postprocessAmbientOcclusionMode = (int)newVal;
            elementsSettings.SliderPP_AO.UpdateText(TranslateAmbientOcclusion((int)newVal));
        }

        /// <summary>
        /// Dynamically update antialias setting from slider value
        /// </summary>
        /// <param name="newVal">The new value</param>
        public void UpdateSetting_PostProcess_AntialiasMode(float newVal)
        {
            workingSettingsData.postprocessAntialiasingMode = (int)newVal;
            elementsSettings.SliderPP_AA.UpdateText(TranslateAntialias((int)newVal));
        }

        /// <summary>
        /// Dynamically update bloom setting from toggle value
        /// </summary>
        /// <param name="newVal">The new value</param>
        public void UpdateSetting_PostProcess_BloomEnabled(bool newVal)
        {
            workingSettingsData.postprocessBloomEnabled = newVal;
        }

        /// <summary>
        /// Dynamically update chromatic abberation setting from toggle value
        /// </summary>
        /// <param name="newVal">The new value</param>
        public void UpdateSetting_PostProcess_ChromAbEnabled(bool newVal)
        {
            workingSettingsData.postprocessChromAbEnabled = newVal;
        }

        /// <summary>
        /// Dynamically update depth of field setting from toggle value
        /// </summary>
        /// <param name="newVal">The new value</param>
        public void UpdateSetting_PostProcess_DOFEnabled(bool newVal)
        {
            workingSettingsData.postprocessDOFEnabled = newVal;
        }

        /// <summary>
        /// Dynamically update motion blur setting from toggle value
        /// </summary>
        /// <param name="newVal">The new value</param>
        public void UpdateSetting_PostProcess_MBEnabled(bool newVal)
        {
            workingSettingsData.postprocessMBEnabled = newVal;
        }

        /// <summary>
        /// Dynamically update screen-space reflections setting from toggle value
        /// </summary>
        /// <param name="newVal">The new value</param>
        public void UpdateSetting_PostProcess_SSREnabled(bool newVal)
        {
            workingSettingsData.postprocessSSREnabled = newVal;
        }
        #endregion

        /// <summary>
        /// Sets the displayed value of a button on the controls menu
        /// </summary>
        /// <param name="buttonToUpdate">The button being updated</param>
        /// <param name="pathToDisplay">The current control's full path</param>
        private void SetDisplayedValue(Button buttonToUpdate, string pathToDisplay)
        {
            string[] pathSplit = pathToDisplay.Split('/');
            TMP_Text text = buttonToUpdate.gameObject.FindChild("Text").GetComponent<TMP_Text>();
            Image image = buttonToUpdate.gameObject.FindChild("Image").GetComponent<Image>();
            GameObject textObj = buttonToUpdate.gameObject.FindChild("Text");
            GameObject imageObj = buttonToUpdate.gameObject.FindChild("Image");
            switch (pathSplit[0])
            {
                #region Gamepad
                case "<Gamepad>":
                    textObj.SetActive(false);
                    imageObj.SetActive(true);
                    switch (pathSplit[1])
                    {
                        case "buttonNorth":
                            image.sprite = defaultImages.Gamepad_button_north;
                            break;
                        case "buttonSouth":
                            image.sprite = defaultImages.Gamepad_button_south;
                            break;
                        case "buttonEast":
                            image.sprite = defaultImages.Gamepad_button_east;
                            break;
                        case "buttonWest":
                            image.sprite = defaultImages.Gamepad_button_west;
                            break;
                        case "select":
                            image.sprite = defaultImages.Gamepad_button_select;
                            break;
                        case "start":
                        case "systemButton":
                            image.sprite = defaultImages.Gamepad_button_start;
                            break;
                        case "leftStick":
                            switch (pathSplit[2])
                            {
                                case "up":
                                    image.sprite = defaultImages.Gamepad_lthumbstick_up;
                                    break;
                                case "down":
                                    image.sprite = defaultImages.Gamepad_lthumbstick_down;
                                    break;
                                case "left":
                                    image.sprite = defaultImages.Gamepad_lthumbstick_left;
                                    break;
                                case "right":
                                    image.sprite = defaultImages.Gamepad_lthumbstick_right;
                                    break;
                            }
                            break;
                        case "rightStick":
                            switch (pathSplit[2])
                            {
                                case "up":
                                    image.sprite = defaultImages.Gamepad_rthumbstick_up;
                                    break;
                                case "down":
                                    image.sprite = defaultImages.Gamepad_rthumbstick_down;
                                    break;
                                case "left":
                                    image.sprite = defaultImages.Gamepad_rthumbstick_left;
                                    break;
                                case "right":
                                    image.sprite = defaultImages.Gamepad_rthumbstick_right;
                                    break;
                            }
                            break;
                        case "dpad":
                            switch (pathSplit[2])
                            {
                                case "up":
                                    image.sprite = defaultImages.Gamepad_dpad_up;
                                    break;
                                case "down":
                                    image.sprite = defaultImages.Gamepad_dpad_down;
                                    break;
                                case "left":
                                    image.sprite = defaultImages.Gamepad_dpad_left;
                                    break;
                                case "right":
                                    image.sprite = defaultImages.Gamepad_dpad_right;
                                    break;
                            }
                            break;
                        case "leftShoulder":
                            image.sprite = defaultImages.Gamepad_lshoulder;
                            break;
                        case "rightShoulder":
                            image.sprite = defaultImages.Gamepad_rshoulder;
                            break;
                        case "leftTrigger":
                        case "leftTriggerButton":
                            image.sprite = defaultImages.Gamepad_ltrigger;
                            break;
                        case "rightTrigger":
                        case "rightTriggerButton":
                            image.sprite = defaultImages.Gamepad_rtrigger;
                            break;
                        case "leftStickPress":
                            image.sprite = defaultImages.Gamepad_lthumbstick_press;
                            break;
                        case "rightStickPress":
                            image.sprite = defaultImages.Gamepad_rthumbstick_press;
                            break;
                        default:
                            textObj.SetActive(true);
                            imageObj.SetActive(false);
                            text.text = "";
                            text.fontSize = GameConstants.MenuControlsFontSizeWord;
                            for (int i = 1; i < pathSplit.Length; i++)
                                text.text += pathSplit[i];
                            break;
                    }
                    break;
                #endregion

                #region Mouse
                case "<Mouse>":
                    textObj.SetActive(false);
                    imageObj.SetActive(true);
                    switch (pathSplit[1])
                    {
                        case "leftButton":
                            image.sprite = defaultImages.Mouse_left;
                            break;
                        case "middleButton":
                            image.sprite = defaultImages.Mouse_middle;
                            break;
                        case "rightButton":
                            image.sprite = defaultImages.Mouse_right;
                            break;
                        case "forward":
                            image.sprite = defaultImages.Mouse_forward;
                            break;
                        case "back":
                            image.sprite = defaultImages.Mouse_back;
                            break;
                        default:
                            textObj.SetActive(true);
                            imageObj.SetActive(false);
                            text.fontSize = GameConstants.MenuControlsFontSizeWord;
                            text.text = "Mouse"  + pathSplit[1];
                            break;
                    }
                    break;
                #endregion

                #region Keyboard
                case "<Keyboard>":
                default:
                    textObj.SetActive(true);
                    imageObj.SetActive(false);
                    // Captures most keys (letters, numbers in number row, and special characters)
                    if (pathSplit[1].Length == 1)
                    {
                        text.fontSize = GameConstants.MenuControlsFontSizeChar;
                        text.text = pathSplit[1].ToUpper();
                    }

                    else
                    {
                        #region Image-based keys
                        #region Shift Keys
                        if (pathSplit[1] == "leftShift")
                        {
                            textObj.SetActive(false);
                            imageObj.SetActive(true);
                            image.sprite = defaultImages.Keyboard_lshift;
                        }

                        else if (pathSplit[1] == "rightShift")
                        {
                            textObj.SetActive(false);
                            imageObj.SetActive(true);
                            image.sprite = defaultImages.Keyboard_rshift;
                        }
                        #endregion

                        #region Character Keys (bspace, space, enter, tab)
                        else if (pathSplit[1] == "space")
                        {
                            textObj.SetActive(false);
                            imageObj.SetActive(true);
                            image.sprite = defaultImages.Keyboard_space;
                        }

                        else if (pathSplit[1] == "backspace")
                        {
                            textObj.SetActive(false);
                            imageObj.SetActive(true);
                            image.sprite = defaultImages.Keyboard_backspace;
                        }

                        else if (pathSplit[1] == "enter")
                        {
                            textObj.SetActive(false);
                            imageObj.SetActive(true);
                            image.sprite = defaultImages.Keyboard_enter;
                        }

                        else if (pathSplit[1] == "tab")
                        {
                            textObj.SetActive(false);
                            imageObj.SetActive(true);
                            image.sprite = defaultImages.Keyboard_tab;
                        }
                        #endregion

                        #region Lock keys
                        else if (pathSplit[1] == "capsLock")
                        {
                            textObj.SetActive(false);
                            imageObj.SetActive(true);
                            image.sprite = defaultImages.Keyboard_caps;
                        }

                        else if (pathSplit[1] == "numLock")
                        {
                            textObj.SetActive(false);
                            imageObj.SetActive(true);
                            image.sprite = defaultImages.Keyboard_num;
                        }

                        else if (pathSplit[1] == "scrollLock")
                        {
                            textObj.SetActive(false);
                            imageObj.SetActive(true);
                            image.sprite = defaultImages.Keyboard_scr;
                        }
                        #endregion

                        #region Arrow keys
                        else if (pathSplit[1] == "downArrow")
                        {
                            textObj.SetActive(false);
                            imageObj.SetActive(true);
                            image.sprite = defaultImages.Keyboard_down;
                        }

                        else if (pathSplit[1] == "leftArrow")
                        {
                            textObj.SetActive(false);
                            imageObj.SetActive(true);
                            image.sprite = defaultImages.Keyboard_left;
                        }

                        else if (pathSplit[1] == "rightArrow")
                        {
                            textObj.SetActive(false);
                            imageObj.SetActive(true);
                            image.sprite = defaultImages.Keyboard_right;
                        }

                        else if (pathSplit[1] == "upArrow")
                        {
                            textObj.SetActive(false);
                            imageObj.SetActive(true);
                            image.sprite = defaultImages.Keyboard_up;
                        }
                        #endregion
                        #endregion

                        #region Remaining keys
                        string converted = GameConstants.InputNameToChar(pathSplit[1]);
                        text.text = converted;
                        if (converted.Length < 4)
                            text.fontSize = GameConstants.MenuControlsFontSizeChar;
                        else if (converted.Length < 8)
                            text.fontSize = GameConstants.MenuControlsFontSizeMiniWord;
                        else
                            text.fontSize = GameConstants.MenuControlsFontSizeWord;
                        #endregion
                    }
                    break;
                #endregion
            }
        }

        /// <summary>
        /// Used by the quit button on the quit prompt menu
        /// </summary>
        public void Quit()
        {
            Application.Quit();
        }

        /// <summary>
        /// Coroutine for loading SL2 scenes asyncronously
        /// </summary>
        private IEnumerator LoadSL2()
        {
            AsyncOperation[] async = new AsyncOperation[GameConstants.SceneCountSL2]
            {
                SceneManager.LoadSceneAsync("SL2_Base", LoadSceneMode.Single),
                SceneManager.LoadSceneAsync("SL2_Wing_EastHanger", LoadSceneMode.Additive),
                SceneManager.LoadSceneAsync("SL2_Wing_EmPwr", LoadSceneMode.Additive),
                SceneManager.LoadSceneAsync("SL2_Wing_Maint", LoadSceneMode.Additive),
                SceneManager.LoadSceneAsync("SL2_Wing_NorthCargo", LoadSceneMode.Additive),
                SceneManager.LoadSceneAsync("SL2_Wing_SouthCargo", LoadSceneMode.Additive),
                SceneManager.LoadSceneAsync("SL2_Wing_WestHanger", LoadSceneMode.Additive),
            };

            foreach (AsyncOperation a in async)
                while (!a.isDone)
                    yield return null;

            for (int i = async.Length - 1; i > 0; i--)
                async[i].allowSceneActivation = true;
        }

        /// <summary>
        /// Used for transitioning
        /// </summary>
        /// <param name="controlled">The menu that's being manipulated</param>
        /// <param name="minimizing">Whether it is minimizing or not</param>
        private IEnumerator TransitionMenu(GameObject controlled, bool minimizing)
        {
            if (minimizing)
            {
                float t = 0;
                while (t <= 1)
                {
                    controlled.transform.localPosition = Vector3.Lerp(menuInitialPositions[controlled], GameConstants.MenuMinimizedPosition(), Mathf.SmoothStep(0f, 1f, Mathf.SmoothStep(0f, 1f, t)));
                    controlled.transform.localScale = Vector3.Lerp(menuInitialScales[controlled], Vector3.zero, Mathf.SmoothStep(0f, 1f, Mathf.SmoothStep(0f, 1f, t)));
                    t += Time.deltaTime / GameConstants.CanvasMinimizeSpeed;
                    yield return null;
                }
                controlled.gameObject.SetActive(false);
            }

            else
            {
                controlled.gameObject.SetActive(true);
                float t = 0;
                while (t <= 1)
                {
                    controlled.transform.localPosition = Vector3.Lerp(GameConstants.MenuMinimizedPosition(), menuInitialPositions[controlled], Mathf.SmoothStep(0f, 1f, Mathf.SmoothStep(0f, 1f, t)));
                    controlled.transform.localScale = Vector3.Lerp(Vector3.zero, menuInitialScales[controlled], Mathf.SmoothStep(0f, 1f, Mathf.SmoothStep(0f, 1f, t)));
                    t += Time.deltaTime / GameConstants.CanvasMinimizeSpeed;
                    yield return null;
                }
            }
        }

        /// <summary>
        /// Used for displaying legible setting for ambient occlusion
        /// </summary>
        /// <param name="setting">The ambient occlusion setting (see PlayerSettings for numerical definition)</param>
        /// <returns>What the setting equates to</returns>
        private string TranslateAmbientOcclusion(int setting)
        {
            switch (setting)
            {
                case 2:
                    return "SAO";
                case 1:
                    return "MSVO";
                case 0:
                default:
                    return "Off";
            }
        }

        /// <summary>
        /// Used for displaying legible setting for antialias mode
        /// </summary>
        /// <param name="setting">The antialias setting (see PlayerSettings for numerical definition)</param>
        /// <returns>What the setting equates to</returns>
        private string TranslateAntialias(int setting)
        {
            switch (setting)
            {
                case 3:
                    return "TAA";
                case 2:
                    return "SMAA";
                case 1:
                    return "FXAA";
                case 0:
                default:
                    return "Off";
            }
        }
        #endregion
    }
}