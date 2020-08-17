using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using Entity2.Core;

namespace Entity2.Data
{
    /// <summary>
    /// Handles data IO for Controls (player controller, gameplay, etc.)
    /// </summary>
    public class PlayerControls : MonoBehaviour
    {
        #region Fields & Properties
        [SerializeField]    private InputActionAsset        defaultActionMap;

        public        ControlsData      Data                { get; private set; }
        public        InputActionMap    DefaultActionMap    { get { return defaultActionMap.actionMaps[0]; } }
        public static PlayerControls    Instance            { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Sets singleton instance and loads data on MonoBehaviour.Awake()
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
        /// Loads controls on MonoBehaviour.Start()
        /// </summary>
        private void Start()
        {
            LoadControls();
        }

        /// <summary>
        /// Loads ControlsData from persistentDataPath
        /// </summary>
        public void LoadControls()
        {
            if (File.Exists(Application.persistentDataPath + GameConstants.DataPathControls))
            {
                XmlSerializer xs = null;
                TextReader txtReader = null;

                try
                {
                    xs = new XmlSerializer(typeof(ControlsData));
                    txtReader = new StreamReader(Application.persistentDataPath + GameConstants.DataPathControls);
                    Data = (ControlsData)xs.Deserialize(txtReader);
                    SetInputOverrides();
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
                SaveControls(new ControlsData(defaultActionMap.actionMaps[0]));
        }

        /// <summary>
        /// Saves ControlsData to persistentDataPath
        /// </summary>
        /// <param name="dataToSave">The data to replace this.Data with and save to persistentDataPath</param>
        public void SaveControls(ControlsData dataToSave)
        {
            Data = dataToSave;
            GameEvents.Instance.CallUpdateGameControls();
            XmlSerializer xs = null;
            TextWriter txtWriter = null;

            try
            {
                xs = new XmlSerializer(typeof(ControlsData));
                txtWriter = new StreamWriter(Application.persistentDataPath + GameConstants.DataPathControls);
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

        /// <summary>
        /// Ensures that the current input action map uses the correct set of controls
        /// (called after loading controls and after exiting controls menu (in case controls weren't saved))
        /// </summary>
        public void SetInputOverrides()
        {
            foreach (ControlBinding cb in Data.controlsGamepad)
            {
                InputAction action = ControlBinding.ControlToAction(cb.control, "Gamepad", out int i);
                if (i > -1)
                {
                    InputBinding binding = action.actionMap.bindings[i];
                    binding.overridePath = cb.path;
                    DefaultActionMap.ApplyBindingOverride(i, binding);
                }
                else
                {
                    InputBinding binding = action.bindings[1];
                    binding.overridePath = cb.path;
                    DefaultActionMap.ApplyBindingOverride(DefaultActionMap.bindings.IndexOf(b => b.action == action.name), binding);
                }
            }
            foreach (ControlBinding cb in Data.controlsKeyboard)
            {
                InputAction action = ControlBinding.ControlToAction(cb.control, "Keyboard", out int i);
                if (i > -1)
                {
                    InputBinding binding = action.actionMap.bindings[i];
                    binding.overridePath = cb.path;
                    DefaultActionMap.ApplyBindingOverride(i, binding);
                }
                else
                {
                    InputBinding binding = action.bindings[0];
                    binding.overridePath = cb.path;
                    DefaultActionMap.ApplyBindingOverride(DefaultActionMap.bindings.IndexOf(b => b.action == action.name), binding);
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Defines all the controls/settings that can be set and saved in the Controls menu
    /// </summary>
    public struct ControlsData
    {
        public ControlBinding[] controlsGamepad;
        public ControlBinding[] controlsKeyboard;
        public bool             mouseLookInverted;
        public bool             settingFOVShift;
        public bool             settingHeadbob;
        public bool             settingHUDMotion;
        public bool             settingPersistentHUD;
        public float            settingRotationSensitivity;

        /// <summary>
        /// Creates ControlsData based off of existing action map
        /// </summary>
        /// <param name="actionMap">The actionmap to read from</param>
        public ControlsData(InputActionMap actionMap)
        {
            mouseLookInverted = false;
            settingFOVShift = false;
            settingHeadbob = false;
            settingHUDMotion = false;
            settingPersistentHUD = false;
            settingRotationSensitivity = 3;

            // Index 0 is for keyboard/mouse; Index 1 is for gamepad
            controlsGamepad = new ControlBinding[]
            {
                new ControlBinding(Control.Gameplay_Console,        ControlBinding.GetPath(actionMap, "Console", 1)),
                new ControlBinding(Control.Gameplay_Interact,       ControlBinding.GetPath(actionMap, "Interact", 1)),
                new ControlBinding(Control.Gameplay_Jump,           ControlBinding.GetPath(actionMap, "Jump", 1)),
                new ControlBinding(Control.Gameplay_Pause,          ControlBinding.GetPath(actionMap, "Pause", 1)),
                new ControlBinding(Control.Gameplay_Sprint,         ControlBinding.GetPath(actionMap, "ModifierMovementFast", 1)),
                new ControlBinding(Control.Gameplay_WeaponFire,     ControlBinding.GetPath(actionMap, "FireWeapon", 1)),
                new ControlBinding(Control.Gameplay_WeaponSwitch,   ControlBinding.GetPath(actionMap, "SwitchWeapon", 1)),
                new ControlBinding(Control.Movement_Forward,        ControlBinding.GetPath(actionMap, "Movement", "Gamepad", "up")),
                new ControlBinding(Control.Movement_Left,           ControlBinding.GetPath(actionMap, "Movement", "Gamepad", "left")),
                new ControlBinding(Control.Movement_Right,          ControlBinding.GetPath(actionMap, "Movement", "Gamepad", "right")),
                new ControlBinding(Control.Movement_Reverse,        ControlBinding.GetPath(actionMap, "Movement", "Gamepad", "down")),
                new ControlBinding(Control.Rotation_Down,           ControlBinding.GetPath(actionMap, "Rotation", "Gamepad", "down")),
                new ControlBinding(Control.Rotation_Left,           ControlBinding.GetPath(actionMap, "Rotation", "Gamepad", "left")),
                new ControlBinding(Control.Rotation_Right,          ControlBinding.GetPath(actionMap, "Rotation", "Gamepad", "right")),
                new ControlBinding(Control.Rotation_Up,             ControlBinding.GetPath(actionMap, "Rotation", "Gamepad", "up"))
            };

            controlsKeyboard = new ControlBinding[]
            {
                new ControlBinding(Control.Gameplay_Console,        ControlBinding.GetPath(actionMap, "Console", 0)),
                new ControlBinding(Control.Gameplay_Interact,       ControlBinding.GetPath(actionMap, "Interact", 0)),
                new ControlBinding(Control.Gameplay_Jump,           ControlBinding.GetPath(actionMap, "Jump", 0)),
                new ControlBinding(Control.Gameplay_Pause,          ControlBinding.GetPath(actionMap, "Pause", 0)),
                new ControlBinding(Control.Gameplay_Sprint,         ControlBinding.GetPath(actionMap, "ModifierMovementFast", 0)),
                new ControlBinding(Control.Gameplay_WeaponFire,     ControlBinding.GetPath(actionMap, "FireWeapon", 0)),
                new ControlBinding(Control.Gameplay_WeaponSwitch,   ControlBinding.GetPath(actionMap, "SwitchWeapon", 0)),
                new ControlBinding(Control.Movement_Forward,        ControlBinding.GetPath(actionMap, "Movement", "Keyboard", "up")),
                new ControlBinding(Control.Movement_Left,           ControlBinding.GetPath(actionMap, "Movement", "Keyboard", "left")),
                new ControlBinding(Control.Movement_Right,          ControlBinding.GetPath(actionMap, "Movement", "Keyboard", "right")),
                new ControlBinding(Control.Movement_Reverse,        ControlBinding.GetPath(actionMap, "Movement", "Keyboard", "down"))
            };
        }
    }

    /// <summary>
    /// Structure for associating a control to a path
    /// </summary>
    public struct ControlBinding
    {
        public Control  control;
        public string   path;

        /// <summary>
        /// Creates a defined ControlBinding
        /// </summary>
        /// <param name="_control">The control being set</param>
        /// <param name="_path">The path being associated to the control</param>
        public ControlBinding(Control _control, string _path)
        {
            control = _control;
            path = _path;
        }

        /// <summary>
        /// Converts a control to InputAction
        /// </summary>
        /// <param name="control">The control being converted</param>
        /// <param name="pathStart">Index indicating whether to use keyboard/mouse set of bindings (0) or gamepad (1)</param>
        /// <param name="index">Composite actions have an associated index for a specific part of the composite; this is otherwise -1 for non-composites</param>
        public static InputAction ControlToAction(Control control, string pathStart, out int index)
        {
            switch (control)
            {
                case Control.Movement_Forward:
                    index = GetIndex(PlayerControls.Instance.DefaultActionMap, "Movement", pathStart, "up");
                    return PlayerControls.Instance.DefaultActionMap.FindAction("Movement");
                case Control.Movement_Left:
                    index = GetIndex(PlayerControls.Instance.DefaultActionMap, "Movement", pathStart, "left");
                    return PlayerControls.Instance.DefaultActionMap.FindAction("Movement");
                case Control.Movement_Reverse:
                    index = GetIndex(PlayerControls.Instance.DefaultActionMap, "Movement", pathStart, "down");
                    return PlayerControls.Instance.DefaultActionMap.FindAction("Movement");
                case Control.Movement_Right:
                    index = GetIndex(PlayerControls.Instance.DefaultActionMap, "Movement", pathStart, "right");
                    return PlayerControls.Instance.DefaultActionMap.FindAction("Movement");
                case Control.Rotation_Down:
                    index = GetIndex(PlayerControls.Instance.DefaultActionMap, "Rotation", pathStart, "down");
                    return PlayerControls.Instance.DefaultActionMap.FindAction("Rotation");
                case Control.Rotation_Left:
                    index = GetIndex(PlayerControls.Instance.DefaultActionMap, "Rotation", pathStart, "left");
                    return PlayerControls.Instance.DefaultActionMap.FindAction("Rotation");
                case Control.Rotation_Right:
                    index = GetIndex(PlayerControls.Instance.DefaultActionMap, "Rotation", pathStart, "right");
                    return PlayerControls.Instance.DefaultActionMap.FindAction("Rotation");
                case Control.Rotation_Up:
                    index = GetIndex(PlayerControls.Instance.DefaultActionMap, "Rotation", pathStart, "up");
                    return PlayerControls.Instance.DefaultActionMap.FindAction("Rotation");
                case Control.Gameplay_Console:
                    index = -1;
                    return PlayerControls.Instance.DefaultActionMap.FindAction("Console");
                case Control.Gameplay_Jump:
                    index = -1;
                    return PlayerControls.Instance.DefaultActionMap.FindAction("Jump");
                case Control.Gameplay_Pause:
                    index = -1;
                    return PlayerControls.Instance.DefaultActionMap.FindAction("Pause");
                case Control.Gameplay_Sprint:
                    index = -1;
                    return PlayerControls.Instance.DefaultActionMap.FindAction("ModifierMovementFast");
                case Control.Gameplay_WeaponFire:
                    index = -1;
                    return PlayerControls.Instance.DefaultActionMap.FindAction("FireWeapon");
                case Control.Gameplay_WeaponSwitch:
                    index = -1;
                    return PlayerControls.Instance.DefaultActionMap.FindAction("SwitchWeapon");
                case Control.Gameplay_Interact:
                default:
                    index = -1;
                    return PlayerControls.Instance.DefaultActionMap.FindAction("Interact");
            }
        }

        /// <summary>
        /// Utility function to get the index of a composite action
        /// </summary>
        /// <param name="map">The action map being searched</param>
        /// <param name="action">The action being found</param>
        /// <param name="pathStart">String indicating whether to use keyboard/mouse set of bindings (0) or gamepad (1)</param>
        /// <param name="compositeName">The specific part of the composite (e.g. Vector2 --> up, down, left, right)</param>
        /// <returns>The index of a specific binding of a composite</returns>
        public static int GetIndex(InputActionMap map, string action, string pathStart, string compositeName)
        {
            return map.FindAction(action).bindings.IndexOf(b => b.isPartOfComposite && b.name == compositeName && b.path.Contains(pathStart));
        }

        /// <summary>
        /// Utility function to get the path of a button action
        /// </summary>
        /// <param name="map">The action map being searched</param>
        /// <param name="action">The action being found</param>
        /// <param name="pathStart">Index indicating whether to use keyboard/mouse set of bindings (0) or gamepad (1)</param>
        /// <returns>The full input path of a binding</returns>
        public static string GetPath(InputActionMap map, string action, int pathStart)
        {
            return map.FindAction(action).bindings[pathStart].path;
        }

        /// <summary>
        /// Utility function to get the path of a composite action
        /// </summary>
        /// <param name="map">The action map being searched</param>
        /// <param name="action">The action being found</param>
        /// <param name="pathStart">String indicating whether to use keyboard/mouse set of bindings (0) or gamepad (1)</param>
        /// <param name="compositeName">The specific part of the composite (e.g. Vector2 --> up, down, left, right)</param>
        /// <returns>The full input path of a binding</returns>
        public static string GetPath(InputActionMap map, string action, string pathStart, string compositeName)
        {
            int i = map.FindAction(action).bindings.IndexOf(b => b.isPartOfComposite && b.name == compositeName && b.path.Contains(pathStart));
            return map.FindAction(action).bindings[i].path;
        }
    }

    /// <summary>
    /// Controls defined in the controls menu
    /// <para>Note: Comments indicate action map path in ControllerActions</para>
    /// </summary>
    public enum Control
    {
        /// <summary>
        /// Actions/Console
        /// </summary>
        Gameplay_Console,

        /// <summary>
        /// Actions/Interact
        /// </summary>
        Gameplay_Interact,

        /// <summary>
        /// Actions/Jump
        /// </summary>
        Gameplay_Jump,

        /// <summary>
        /// Actions/Pause
        /// </summary>
        Gameplay_Pause,

        /// <summary>
        /// Actions/ModifierMovementFast
        /// </summary>
        Gameplay_Sprint,

        /// <summary>
        /// Actions/FireWeapon
        /// </summary>
        Gameplay_WeaponFire,

        /// <summary>
        /// Actions/SwitchWeapon
        /// </summary>
        Gameplay_WeaponSwitch,

        /// <summary>
        /// Actions/Movement/Up
        /// </summary>
        Movement_Forward,

        /// <summary>
        /// Actions/Movement/Left
        /// </summary>
        Movement_Left,

        /// <summary>
        /// Actions/Movement/Right
        /// </summary>
        Movement_Right,

        /// <summary>
        /// Actions/Movement/Down
        /// </summary>
        Movement_Reverse,

        /// <summary>
        /// Actions/Rotation/Down
        /// </summary>
        Rotation_Down,

        /// <summary>
        /// Actions/Rotation/Left
        /// </summary>
        Rotation_Left,

        /// <summary>
        /// Actions/Rotation/Right
        /// </summary>
        Rotation_Right,

        /// <summary>
        /// Actions/Rotation/Up
        /// </summary>
        Rotation_Up
    }
}