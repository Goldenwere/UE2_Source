#pragma warning disable 0649

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Entity2.Core
{
    #region Delegates
    public delegate void UIElementString(UIElement elem, string content);
    public delegate void UIElementFloat(UIElement elem, float content);
    public delegate void UIElementState(UIElement elem, UIState state, float content);
    public delegate void UIElementEnum(UIElement elem, Enum content);
    public delegate void ObjectiveDelegate(Objective obj);
    public delegate void GameStateChangeDelegate(GameState prev, GameState curr);
    public delegate void GameObjectDelegate(GameObject obj);
    public delegate void UpgradeDelegate(UpgradeElement elem, int newVal);
    public delegate void GenericDelegate();
    #endregion

    /// <summary>
    /// Manages communication between game systems
    /// </summary>
    public class GameEvents : MonoBehaviour
    {
        #region Fields & Properties
        /// <summary>
        /// The current GameState of the game
        /// </summary>
        public        GameState     CurrState   { get; private set; }
        public static GameEvents    Instance    { get; private set; }
        private int                 workingSceneCount;
        #endregion

        #region Events
        /// <summary>
        /// Use this event when a game scene (non-menu) was loaded
        /// </summary>
        public static event GenericDelegate         GameSceneLoaded;

        /// <summary>
        /// Use this event to do work when an objective is completed
        /// </summary>
        public static event ObjectiveDelegate       ObjectiveCompleted;

        /// <summary>
        /// Use this event to do work when the player dies
        /// </summary>
        public static event GenericDelegate         PlayerDeath;

        /// <summary>
        /// Use this event to do work when the player spawns
        /// </summary>
        public static event GameObjectDelegate      PlayerSpawned;

        /// <summary>
        /// Use this event to do work when the player requests an upgrade
        /// </summary>
        public static event UpgradeDelegate         PlayerUpgrade;

        /// <summary>
        /// Use this event to do work when a weapon is switched
        /// </summary>
        public static event GameObjectDelegate      SwitchWeapon;

        /// <summary>
        /// Use this event to do work when game controls are updated
        /// </summary>
        public static event GenericDelegate         UpdateGameControls;

        /// <summary>
        /// Use this event to do work when game settings (audio, graphics, and post-processing) are updated
        /// </summary>
        public static event GenericDelegate         UpdateGameSettings;

        /// <summary>
        /// Use this event to do work when the GameState is changed
        /// </summary>
        public static event GameStateChangeDelegate UpdateGameState;

        /// <summary>
        /// When a UI element dependant on an enum needs updated, use this event
        /// </summary>
        public static event UIElementEnum           UpdateUIElement_Enum;

        /// <summary>
        /// When a UI element with number-based content needs updated, use this event
        /// </summary>
        public static event UIElementFloat          UpdateUIElement_Num;

        /// <summary>
        /// When a UI element with its own states and number-based content needs updated, use this event
        /// </summary>
        public static event UIElementState          UpdateUIElement_State;

        /// <summary>
        /// When a UI element with string-based content needs updated, use this event
        /// </summary>
        public static event UIElementString         UpdateUIElement_Str;
        #endregion

        #region Methods
        /// <summary>
        /// Set the GameState at Start
        /// </summary>
        private void Awake()
        {
            CurrState = GameState.menus;
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
        /// Subscribe to scene loads
        /// </summary>
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>
        /// Unsubscribe from scene loads
        /// </summary>
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// When a scene is loaded, listen in to it
        /// </summary>
        /// <param name="scene">The scene that was loaded</param>
        /// <param name="mode">The mode that the scene was loaded with</param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name.Contains(GameConstants.ScenePrefixSL2))
            {
                if (workingSceneCount == 0)
                    StartCoroutine(WaitUntilScenesLoaded(GameConstants.SceneCountSL2));
                workingSceneCount++;
            }
        }

        /// <summary>
        /// Use this to notify handlers that an objective was completed
        /// </summary>
        /// <param name="obj">The objective that was completed</param>
        public void CompleteObjective(Objective obj)
        {
            ObjectiveCompleted?.Invoke(obj);
        }

        /// <summary>
        /// Use this to notify handlers that the player has died
        /// </summary>
        public void CallPlayerDeath()
        {
            PlayerDeath?.Invoke();
            CallUpdateGameState(GameState.death);
        }

        /// <summary>
        /// Use this to notify handlers that the player's weapon was switched
        /// </summary>
        /// <param name="newWeapon">The new weapon that the player is using</param>
        public void CallSwitchWeapon(GameObject newWeapon)
        {
            SwitchWeapon?.Invoke(newWeapon);
        }

        /// <summary>
        /// Use this to notify handlers that the game controls have been changed
        /// </summary>
        public void CallUpdateGameControls()
        {
            UpdateGameControls?.Invoke();
        }

        /// <summary>
        /// Use this to notify handlers that the game settings have been changed
        /// </summary>
        public void CallUpdateGameSettings()
        {
            UpdateGameSettings?.Invoke();
        }

        /// <summary>
        /// Use this to notify handlers that the GameState should be changed
        /// </summary>
        /// <param name="newState">The new GameState</param>
        public void CallUpdateGameState(GameState newState)
        {
            UpdateGameState?.Invoke(CurrState, newState);
            CurrState = newState;
        }

        /// <summary>
        /// Use this to notify handlers that the player was spawned
        /// </summary>
        /// <param name="player">The player's object</param>
        public void PlayerSpawn(GameObject player)
        {
            PlayerSpawned?.Invoke(player);
        }

        /// <summary>
        /// Use this to notify handlers that an upgrade has happened
        /// </summary>
        /// <param name="elem">The element that was upgraded</param>
        /// <param name="newAmount">The element's new value</param>
        public void SendUpgrade(UpgradeElement elem, int newAmount)
        {
            PlayerUpgrade?.Invoke(elem, newAmount);
        }

        /// <summary>
        /// Use this to notify handlers that the console was toggled
        /// </summary>
        public void ToggleConsole()
        {
            if (CurrState == GameState.consl)
                CallUpdateGameState(GameState.gplay);
            else
                CallUpdateGameState(GameState.consl);
        }

        /// <summary>
        /// Use this to notify handlers that the game was paused/unpaused
        /// </summary>
        public void TogglePause()
        {
            if (CurrState == GameState.pause)
                CallUpdateGameState(GameState.gplay);
            else
                CallUpdateGameState(GameState.pause);
        }

        /// <summary>
        /// Use this to update UI elements
        /// </summary>
        /// <param name="elem">The element to update</param>
        /// <param name="content">The text that gets put into TMPro</param>
        public void UpdateUI(UIElement elem, string content)
        {
            UpdateUIElement_Str?.Invoke(elem, content);
        }

        /// <summary>
        /// Use this to update UI elements
        /// </summary>
        /// <param name="elem">The element to update</param>
        /// <param name="content">The number that is used for either a TMPro .value or .text</param>
        public void UpdateUI(UIElement elem, float content)
        {
            UpdateUIElement_Num?.Invoke(elem, content);
        }

        /// <summary>
        /// Use this to update UI elements
        /// </summary>
        /// <param name="elem">The element to update</param>
        /// <param name="state">The state of the UI element</param>
        /// <param name="content">The number that is used for either a TMPro .value or .text</param>
        public void UpdateUI(UIElement elem, UIState state, float content)
        {
            UpdateUIElement_State?.Invoke(elem, state, content);
        }

        /// <summary>
        /// Use this to update UI elements
        /// </summary>
        /// <param name="elem">The element to update</param>
        /// <param name="content">A generic enum that is down-casted in event handlers</param>
        public void UpdateUI(UIElement elem, Enum content)
        {
            UpdateUIElement_Enum?.Invoke(elem, content);
        }

        /// <summary>
        /// Coroutine for waiting until all game scenes are loaded before invoking GameSceneLoaded
        /// </summary>
        /// <param name="requiredCount">The number of scenes to wait for</param>
        private IEnumerator WaitUntilScenesLoaded(int requiredCount)
        {
            while (workingSceneCount < requiredCount)
                yield return null;

            GameSceneLoaded?.Invoke();
            CallUpdateGameState(GameState.gplay);
            workingSceneCount = 0;
        }
        #endregion
    }
}