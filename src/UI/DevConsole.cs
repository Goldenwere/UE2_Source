using TMPro;
using System;
using System.Reflection;
using UnityEngine;
using Entity2.Character;
using Entity2.Obj;
using Entity2.Core;

namespace Entity2.UI
{
    /// <summary>
    /// Enables functionality for the developers console
    /// </summary>
    public class DevConsole : MonoBehaviour
    {
        #region Fields
        [SerializeField]    private TextMeshProUGUI                         focusText;
        [SerializeField]    private TMP_InputField                          inputField;
        [SerializeField]    private RectTransform                           outputParent;
        [SerializeField]    private TextMeshProUGUI                         outputText;
        /**************/    private GameObject                              focusObject;
        /**************/    private bool                                    isActive;
        /**************/    private string                                  output;
        /**************/    private TextMeshProUGUI                         outputLatest;
        /**************/    private GameObject                              player;
        /**************/    private ICharacter                              playerIChar;
        /**************/    private bool                                    showFPS;
        #endregion

        #region Methods
        /// <summary>
        /// On every frame, listen for player clicking on focus objects
        /// </summary>
        private void Update()
        {
            if (isActive)
            {
                // Set focus object on left mouse click
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
                    {
                        ReferenceToParent r = hit.collider.gameObject.GetComponent<ReferenceToParent>();

                        if (r != null)
                            focusObject = r.Parent;
                        else
                            focusObject = hit.collider.gameObject;

                        focusText.text = "Object: " + focusObject.name + " | ID: " + focusObject.GetInstanceID();
                    }
                }

                // Reset focus object on right mouse click
                else if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    focusObject = null;
                    focusText.text = "No object selected";
                }

                // Clicking loses focus on input field - reset focus
                if (!inputField.isFocused)
                    inputField.ActivateInputField();
            }
        }

        /// <summary>
        /// Used when the HUD is first attached to the player
        /// </summary>
        /// <param name="player">The player that was spawned</param>
        public void Initialize(GameObject player)
        {
            this.player = player;
            playerIChar = player.GetComponent<ICharacter>();
        }

        /// <summary>
        /// Parses console input for commands
        /// </summary>
        /// <param name="input">The string inputted into the inputField</param>
        public void ParseInput(string input)
        {
            // Ensure there's input in the first place 
            // (this method is fired when the player doesn't press enter sometimes, 
            // clearing console and using this check ensures preventing a false error reported in output
            if (input.Length > 0)
            {
                string[] split = input.Split(' ');

                string cmdstr = split[0].ToLower();
                if (Enum.TryParse(cmdstr, out ConsoleCommand cmd))
                {
                    switch (cmd)
                    {
                        #region command get
                        case ConsoleCommand.get:
                            switch (split.Length)
                            {
                                case 3:
                                    if (focusObject == null)
                                        output = GameConstants.ErrorMissingObject;
                                    else
                                        output = Get(split[1], split[2], focusObject);
                                    break;
                                case 2:
                                    output = GameConstants.ErrorMissingParam + "[Property]";
                                    break;
                                case 1:
                                default:
                                    output = "get [Component] [Property] : Gets a value from the specified component";
                                    break;
                            }
                            break;
                        #endregion

                        #region command getself
                        case ConsoleCommand.getself:
                            switch (split.Length)
                            {
                                case 3:
                                    if (ValidatePlayerComponent(split[1], out Component comp))
                                        output = GetVal(comp, split[2]);
                                    else
                                        output = GameConstants.ErrorInvalidParam + "[Component]";
                                    break;
                                case 2:
                                    output = GameConstants.ErrorMissingParam + "[Property]";
                                    break;
                                case 1:
                                default:
                                    output = "getself [Component] [Property] : Gets the value of specified property of a specified player-attached component (PlayerStats, PlayerController)";
                                    break;
                            }
                            break;
                        #endregion

                        #region command set
                        case ConsoleCommand.set:
                            switch (split.Length)
                            {
                                case 4:
                                    if (focusObject == null)
                                        output = GameConstants.ErrorMissingObject;
                                    else
                                        output = Set(split[1], split[2], split[3], focusObject);
                                    break;
                                case 3:
                                    output = GameConstants.ErrorMissingParam + "[Value]";
                                    break;
                                case 2:
                                    output = GameConstants.ErrorMissingParam + "[Property]";
                                    break;
                                case 1:
                                default:
                                    output = "set [Component] [Property] [Value] : Sets a value on the specified component";
                                    break;
                            }
                            break;
                        #endregion

                        #region command setself
                        case ConsoleCommand.setself:
                            switch (split.Length)
                            {
                                case 4:
                                    if (ValidatePlayerComponent(split[1], out Component comp))
                                    {
                                        PropertyInfo info = comp.GetType().GetProperty(split[2]);
                                        if (info == null)
                                            output = GameConstants.ErrorInvalidParam + "[Property]";
                                        else
                                            output = SetVal(comp, info, split[3]);
                                    }
                                    else
                                        output = GameConstants.ErrorInvalidParam + "[Component]";
                                    break;
                                case 3:
                                    output = GameConstants.ErrorMissingParam + "[Value]";
                                    break;
                                case 2:
                                    output = GameConstants.ErrorMissingParam + "[Property]";
                                    break;
                                case 1:
                                default:
                                    output = "setself [Component] [Property] [Value] : Sets the value of specified property of a specified player-attached component (PlayerStats, PlayerController)";
                                    break;
                            }
                            break;
                        #endregion

                        #region command god
                        case ConsoleCommand.god:
                            bool desired;
                            bool self;

                            switch (split.Length)
                            {
                                case 3:
                                    if (focusObject == null)
                                        output = GameConstants.ErrorMissingObject;

                                    else if (!ValidateBool(split[2].ToLower(), out self) || !ValidateBool(split[1].ToLower(), out desired))
                                        output = GameConstants.ErrorInvalidParam + "(on self?)";

                                    else
                                    {
                                        if (self)
                                        {
                                            playerIChar.IsGodMode = desired;
                                            output = "Player's godmode switched to: " + playerIChar.IsGodMode;
                                        }

                                        else
                                        {
                                            ICharacter c;
                                            if (GetChar(focusObject, out c))
                                            {
                                                c.IsGodMode = desired;
                                                output = "Godmode switched to: " + c.IsGodMode + " on selected gameobject/its prefab parent (if applicable)";
                                            }

                                            else
                                                output = GameConstants.ErrorInvalidObject;
                                        }
                                    }
                                    break;

                                case 2:
                                    if (ValidateBool(split[1].ToLower(), out desired))
                                    {
                                        playerIChar.IsGodMode = desired;
                                        output = "Player's godmode switched to: " + playerIChar.IsGodMode;
                                    }
                                    else
                                        output = GameConstants.ErrorInvalidParam + "(desired state)";
                                    break;

                                case 1:
                                default:
                                    playerIChar.IsGodMode = !playerIChar.IsGodMode;
                                    output = "Player's godmode toggled to: " + playerIChar.IsGodMode;
                                    break;
                            }
                            break;
                        #endregion

                        #region command destroy
                        case ConsoleCommand.destroy:
                            if (focusObject == null)
                                output = GameConstants.ErrorMissingObject;
                            else
                            {
                                Destroy(focusObject);
                                output = "Object destroyed";
                            }
                            break;
                        #endregion

                        #region command disable
                        case ConsoleCommand.disable:
                            if (focusObject == null)
                                output = GameConstants.ErrorMissingObject;
                            else
                            {
                                focusObject.SetActive(false);
                                output = "Object disabled";
                            }
                            break;
                        #endregion

                        #region command enable
                        case ConsoleCommand.enable:
                            if (focusObject == null)
                                output = GameConstants.ErrorMissingObject;
                            else
                            {
                                focusObject.SetActive(true);
                                output = "Object enabled";
                            }
                            break;
                        #endregion

                        #region showfps
                        case ConsoleCommand.showfps:
                            showFPS = !showFPS;
                            if (showFPS)
                                GameEvents.Instance.UpdateUI(UIElement.GUIFramerate, UIState.enabled);
                            else
                                GameEvents.Instance.UpdateUI(UIElement.GUIFramerate, UIState.disabled);
                            break;
                        #endregion

                        #region command help
                        case ConsoleCommand.help:
                        default:
                            output = "Available commands: " +
                                "\n\tGet [Component] [Property] : Gets a value of a specified component" +
                                "\n\tGetSelf [Component] [Property] : Gets the value of specified property of a " +
                                "\n\t\tspecified player-attached component (PlayerStats, PlayerController)" +
                                "\n\tSet [Component] [Property] [Value] : Sets a value of a specified component" +
                                "\n\tSetSelf [Component] [Property] [Value] : Sets the value of specified property of a " +
                                "\n\t\tspecified player-attached component (PlayerStats, PlayerController)" +
                                "\n\tEnable : Enables a gameobject" +
                                "\n\tDisable : Disables a gameobject" +
                                "\n\tDestroy : Permanently destroys a gameobject" +
                                "\n\tGod [true/false] [on self?]" +
                                "\n\tShowFPS : Toggles FPS counter";
                            break;
                            #endregion
                    }
                }

                else
                    output = "Error: Unknown entered command '" + cmdstr + "'";

                CreateNewText(output.Split('\n'));

                // As stated toward top of method, clear input field to prevent firing the same command when changing focus
                inputField.text = "";
            }
        }

        /// <summary>
        /// Use this when setting the console window to active/inactive
        /// </summary>
        /// <param name="active">Whether to set this as active or not</param>
        public void SetActive(bool active)
        {
            isActive = active;
            if (active)
            {
                outputText.text = "Enter a command, or type 'help' for a list of commands";
                output = "";
                outputLatest = outputText;
            }
        }

        /// <summary>
        /// Creates new text elements in the output field
        /// </summary>
        /// <param name="content">The content split</param>
        private void CreateNewText(string[] content)
        {
            foreach (string s in content)
            {
                // Create the new text
                TextMeshProUGUI newTextElement = Instantiate(outputText);
                newTextElement.transform.SetParent(outputParent, false);
                newTextElement.text = s;

                // Position the new text
                Vector3 pos = outputLatest.rectTransform.localPosition;
                pos.y -= outputText.rectTransform.sizeDelta.y;
                newTextElement.rectTransform.localPosition = pos;

                // Update height of parent
                Vector2 size = outputParent.sizeDelta;
                size.y += outputText.rectTransform.sizeDelta.y;
                outputParent.sizeDelta = size;

                outputLatest = newTextElement;
            }
        }

        /// <summary>
        /// Used by the 'get' command, finds the value of a property of a component
        /// </summary>
        /// <param name="comp">The component to search for</param>
        /// <param name="prop">The property desired to be found</param>
        /// <param name="focus">The gameobject to search the component on</param>
        /// <returns></returns>
        private string Get(string comp, string prop, GameObject focus)
        {
            // Try to find the component
            Component c = focus.GetComponent(comp);
            if (c == null)
                return "Error: Component " + comp + " not found";

            else
                return GetVal(c, prop);
        }

        /// <summary>
        /// Finds an ICharacter component on an object or its parental reference (if applicable) 
        /// </summary>
        /// <param name="focus">The GameObject to start the search from</param>
        /// <param name="iChar">The ICharacter that may be on the GameObject</param>
        /// <returns>Whether an ICharacter component was found</returns>
        private bool GetChar(GameObject focus, out ICharacter iChar)
        {
            iChar = focus.GetComponent<ICharacter>();

            if (iChar == null)
                return false;

            else
                return true;
        }

        /// <summary>
        /// Gets the value of a component in the scene
        /// </summary>
        /// <param name="comp">The component found with the console</param>
        /// <param name="prop">The property desired to be found</param>
        /// <returns>Output string</returns>
        private string GetVal(Component comp, string prop)
        {
            PropertyInfo info = comp.GetType().GetProperty(prop);
            if (info == null)
                return "Error: Property " + prop + " not found";
            else
            {
                object o = info.GetValue(comp);
                if (o == null)
                    return "Error: Property " + prop + " not found";
                else
                    return "Value " + prop + " of " + comp.ToString() + ": " + o.ToString();
            }
        }

        /// <summary>
        /// Used by the 'set' command, finds the value of a property of a component
        /// </summary>
        /// <param name="comp">The component to search for</param>
        /// <param name="prop">The property desired to be found</param>
        /// <param name="val">The value to set prop to</param>
        /// <param name="focus">The gameobject to search the component on</param>
        /// <returns></returns>
        private string Set(string comp, string prop, string val, GameObject focus)
        {
            // Try to find the component. If null first, determine if it has a parental reference to search on
            Component c = focus.GetComponent(comp);
            if (c == null)
                return "Error: Component " + comp + " not found";

            else
            {
                PropertyInfo info = c.GetType().GetProperty(prop);
                if (info == null)
                    return "Error: Property " + prop + " not found";

                else
                    return SetVal(c, info, val);
            }
        }

        /// <summary>
        /// Sets the value of a component in the scene
        /// </summary>
        /// <param name="comp">The component found with the console</param>
        /// <param name="prop">The property desired to be found</param>
        /// <param name="val">The value to set the property to</param>
        /// <returns>Output string</returns>
        private string SetVal(Component comp, PropertyInfo prop, string val)
        {
            if (prop.GetValue(comp).GetType() == typeof(float))
            {
                try
                {
                    float.TryParse(val, out float f);
                    prop.SetValue(comp, f);
                    return "Value set successfully";
                }
                catch (Exception e)
                {
                    return "Error: " + e.Message;
                }
            }

            else if (prop.GetValue(comp).GetType() == typeof(int))
            {
                try
                {
                    int.TryParse(val, out int i);
                    prop.SetValue(comp, i);
                    return "Value set successfully";
                }
                catch (Exception e)
                {
                    return "Error: " + e.Message;
                }
            }

            else if (prop.GetValue(comp).GetType() == typeof(bool))
            {
                if (ValidateBool(val, out bool b))
                {
                    try
                    {
                        prop.SetValue(comp, b);
                        return "Value set successfully";
                    }
                    catch (Exception e)
                    {
                        return "Error: " + e.Message;
                    }
                }

                else
                    return "Error: Property " + prop + " is of type bool; supplied value is not a valid bool";
            }

            else if (prop.GetValue(comp).GetType() == typeof(string))
            {
                try
                {
                    prop.SetValue(comp, val);
                    return "Value set successfully";
                }
                catch (Exception e)
                {
                    return "Error: " + e.Message;
                }
            }

            else
                return "Error: Property " + prop + " cannot be set - it is not a primitive type";
        }

        /// <summary>
        /// Parses console input for a bool
        /// </summary>
        /// <param name="input">The input to parse</param>
        /// <param name="option">The desired bool</param>
        /// <returns>Whether the input can validly be translated into a bool</returns>
        private bool ValidateBool(string input, out bool option)
        {
            bool isValid = false;
            option = false;
            input = input.ToLower();

            if (input == "false" || input == "f" || input == "0" || input == "off" || input == "disabled" || input == "disable" || input == "n" || input == "no")
            {
                isValid = true;
                option = false;
            }

            else if (input == "true" || input == "t" || input == "1" || input == "on" || input == "enabled" || input == "enable" || input == "yes" || input == "y")
            {
                isValid = true;
                option = true;
            }

            return isValid;
        }

        /// <summary>
        /// Parses console input for a valid component attached to the player
        /// </summary>
        /// <param name="input">The input to parse</param>
        /// <param name="comp">The desired component</param>
        /// <returns>Whether the input can validly be translated into a player-attached component</returns>
        private bool ValidatePlayerComponent(string input, out Component comp)
        {
            bool isValid = false;
            comp = null;

            input = input.ToLower();
            if (input == "playerstats")
            {
                isValid = true;
                comp = player.GetComponent("PlayerStats");
            }

            else if (input == "playercontroller")
            {
                isValid = true;
                comp = player.GetComponent("PlayerController");
            }

            return isValid;
        }
        #endregion
    }

    /// <summary>
    /// Defines commands for the console
    /// </summary>
    public enum ConsoleCommand
    {
        get,
        set,
        god,
        destroy,
        disable,
        enable,
        help,
        getself,
        setself,
        showfps
    }
}