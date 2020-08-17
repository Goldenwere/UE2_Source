#pragma warning disable 0649

using System;
using UnityEngine;
using Entity2.Core;
using Entity2.Data;
using Goldenwere.Unity;

namespace Entity2.UI
{
    /// <summary>
    /// Filters UI-related events and manages various game UI accordingly
    /// </summary>
    public class GameInterface : MonoBehaviour
    {
        [SerializeField]    private GameObject  hudPrefab;
        /**************/    private GameHUD     hud;
        /**************/    private bool        fpsCounterEnabled;
        /**************/    private string      fpsCounterText;
        /**************/    private float       fpsCounterTimer;

        /// <summary>
        /// Used for subscribing to events
        /// </summary>
        private void OnEnable()
        {
            GameEvents.UpdateUIElement_Str += UpdateUI;
            GameEvents.UpdateUIElement_Num += UpdateUI;
            GameEvents.UpdateUIElement_State += UpdateUI;
            GameEvents.UpdateUIElement_Enum += UpdateUI;
            GameEvents.UpdateGameState += OnUpdateGameState;
            GameEvents.SwitchWeapon += OnWeaponSwitched;
            GameEvents.PlayerSpawned += OnPlayerSpawned;
            GameEvents.ObjectiveCompleted += OnObjectiveComplete;
            GameEvents.PlayerUpgrade += OnUpgrade;
        }

        /// <summary>
        /// Used for unsubscribing from events
        /// </summary>
        private void OnDisable()
        {
            GameEvents.UpdateUIElement_Str -= UpdateUI;
            GameEvents.UpdateUIElement_Num -= UpdateUI;
            GameEvents.UpdateUIElement_State -= UpdateUI;
            GameEvents.UpdateUIElement_Enum -= UpdateUI;
            GameEvents.UpdateGameState -= OnUpdateGameState;
            GameEvents.SwitchWeapon -= OnWeaponSwitched;
            GameEvents.PlayerSpawned -= OnPlayerSpawned;
            GameEvents.ObjectiveCompleted -= OnObjectiveComplete;
            GameEvents.PlayerUpgrade -= OnUpgrade;
        }

        /// <summary>
        /// Render FPS label if toggled
        /// </summary>
        private void OnGUI()
        {
            fpsCounterTimer += Time.deltaTime;
            if (fpsCounterEnabled)
            {
                GUI.Label(
                    new Rect(10, 10, 100, 20), fpsCounterText, new GUIStyle { normal = new GUIStyleState { textColor = Color.white }, fontSize = 32 });

                if (fpsCounterTimer > GameConstants.GUIRefreshRate)
                {
                    fpsCounterTimer = 0;
                    fpsCounterText = string.Format("{0:N0} FPS ({1:F2} ms)", 1.0f / Time.unscaledDeltaTime, Time.unscaledDeltaTime * 1000);
                }
            }
        }

        /// <summary>
        /// On the UpdateUIElement_Str event, update UI that depend on string-based content
        /// </summary>
        /// <param name="elem">The element to update</param>
        /// <param name="content">The content of the UI element</param>
        private void UpdateUI(UIElement elem, string content)
        {
            switch (elem)
            {
                // nothing atm uses string overload
            }
        }

        /// <summary>
        /// On the UpdateUIElement_Num event, update UI that depend on number-based content
        /// </summary>
        /// <param name="elem">The element to update</param>
        /// <param name="content">The content of the UI element</param>
        private void UpdateUI(UIElement elem, float content)
        {
            switch (elem)
            {
                case UIElement.HUDHealth:
                    hud.UpdatePlayerCurrHealth(content);
                    break;
                case UIElement.HUDMaxHealth:
                    hud.UpdatePlayerMaxHealth(content);
                    break;
                case UIElement.HUDArmor:
                    hud.UpdatePlayerCurrArmor(content);
                    break;
                case UIElement.HUDMaxArmor:
                    hud.UpdatePlayerMaxArmor(content);
                    break;
                case UIElement.HUDShield:
                    hud.UpdatePlayerCurrShield(content);
                    break;
                case UIElement.HUDMaxShield:
                    hud.UpdatePlayerMaxShield(content);
                    break;
                case UIElement.HUDStamina:
                    hud.UpdatePlayerCurrStamina(content);
                    break;
                case UIElement.HUDMaxStamina:
                    hud.UpdatePlayerMaxStamina(content);
                    break;
                case UIElement.HUDAmmo:
                    hud.UpdateAmmoText(content);
                    break;
                case UIElement.HUDEntityMaxHealth:
                    hud.UpdateEntMaxHealth(content);
                    break;
                case UIElement.HUDEntityHealth:
                    hud.UpdateEntCurrHealth(content);
                    break;
                case UIElement.HUDExperience:
                    hud.UpdateExpText(content.ToString());
                    break;
            }
        }

        /// <summary>
        /// On the UpdateUIElement_State event, update UI that depend on states
        /// </summary>
        /// <param name="elem">The element to update</param>
        /// <param name="state">The state the element should be in</param>
        /// <param name="content">The content of the UI element</param>
        private void UpdateUI(UIElement elem, UIState state, float content)
        {
            // Enabled and disabled directly equate to true/false for some elements
            bool isActive = false;
            if (state == UIState.enabled)
                isActive = true;

            switch (elem)
            {
                case UIElement.HUDStatusElectrical:
                    hud.UpdateStatusEffect(StatusEffect.Electrical, isActive, content);
                    break;
                case UIElement.HUDStatusHeat:
                    hud.UpdateStatusEffect(StatusEffect.Heat, isActive, content);
                    break;
                case UIElement.HUDStatusToxin:
                    hud.UpdateStatusEffect(StatusEffect.Toxin, isActive, content);
                    break;
                case UIElement.HUDStatusCold:
                    hud.UpdateStatusEffect(StatusEffect.Cold, isActive, content);
                    break;
                case UIElement.HUDEntityHealth:
                    hud.UpdateEntCurrHealth();
                    break;
            }
        }

        /// <summary>
        /// On the OnUpdateUIElement_Enum event, update UI that depend on an enum
        /// </summary>
        /// <param name="elem">The element to update</param>
        /// <param name="content">An enum that defines the 'content' of the element</param>
        private void UpdateUI(UIElement elem, Enum content)
        {
            switch (elem)
            {
                case UIElement.HUDCrosshair:
                    hud.UpdateCrossHair((CrosshairTarget)content);
                    break;
                case UIElement.GUIFramerate:
                    if ((UIState)content == UIState.enabled)
                        fpsCounterEnabled = true;
                    else
                        fpsCounterEnabled = false;
                    break;
            }
        }

        /// <summary>
        /// On the OnUpdateGameState event, update canvases in the GameHUD
        /// </summary>
        /// <param name="prev">The previous GameState</param>
        /// <param name="curr">The current GameState</param>
        private void OnUpdateGameState(GameState prev, GameState curr)
        {
            hud.UpdateCanvases(curr);
            if (curr == GameState.menus && hud != null)
                Destroy(hud);
        }

        /// <summary>
        /// On the OnSwitchWeapon event, attach the new ammo text to the GameHUD
        /// </summary>
        /// <param name="weapon">The new weapon equipped</param>
        private void OnWeaponSwitched(GameObject weapon)
        {
            hud.UpdateAmmoTextObject(weapon);
        }

        /// <summary>
        /// On the OnPlayerSpawned event, attach hud to player
        /// </summary>
        private void OnPlayerSpawned(GameObject player)
        {
            GameObject cam = player.FindChildWithTag("UICamera");
            Camera c = cam.GetComponent<Camera>();

            hud = Instantiate(hudPrefab, cam.transform).GetComponent<GameHUD>();
            hud.Initialize(player);

            hud.gameObject.FindChild("PlayerHUD").GetComponent<Canvas>().worldCamera = c;
            hud.gameObject.FindChildRecursively("Pause").GetComponent<Canvas>().worldCamera = c;
            hud.gameObject.FindChildRecursively("DevConsole").GetComponent<Canvas>().worldCamera = c;
            hud.gameObject.FindChildRecursively("Death").GetComponent<Canvas>().worldCamera = c;

            hud.UpdateCanvases(GameState.gplay);
            hud.UpdateEntCurrHealth();
        }

        /// <summary>
        /// On the OnObjectiveCompletedd event, update the objective on the HUD
        /// </summary>
        /// <param name="obj">The objective that was completed</param>
        private void OnObjectiveComplete(Objective obj)
        {
            switch (obj)
            {
                case Objective.EmergencyPower:
                default:
                    hud.UpdateObjectiveText("Demo completed. Thanks for playing!");
                    break;
            }
        }

        /// <summary>
        /// On the OnUpgrade event, update the experience text element
        /// </summary>
        /// <param name="elem">Unused</param>
        /// <param name="newVal">Unused</param>
        private void OnUpgrade(UpgradeElement elem, int newVal)
        {
            hud.UpdateExpText(PlayerUpgradeData.Instance.PlayerExp.ToString());
        }
    }
}