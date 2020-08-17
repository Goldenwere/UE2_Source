#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Entity2.Core;
using Entity2.Data;
using Goldenwere.Unity.Controller;

namespace Entity2.Character
{
    /// <summary>
    /// PlayerStats handles the player's health, shields, stamina, and armor, as well as player input not related to movement (e.g. weapon calls, as ICharacters are what fire weapon events)
    /// </summary>
    public class PlayerStats : MonoBehaviour, ICharacter
    {
        #region Fields & Properties
        [SerializeField]    private FirstPersonController   controller;
        /**************/    private bool                    armorCanRepair;
        /**************/    private Coroutine               coldCoroutine;
        /**************/    private bool                    coldRunning;
        /**************/    private float                   coldTimer;
        /**************/    private Coroutine               electricalCoroutine;
        /**************/    private bool                    electricalRunning;
        /**************/    private float                   electricalTimer;
        /**************/    private Coroutine               heatCoroutine;
        /**************/    private bool                    heatRunning;
        /**************/    private float                   heatTimer;
        /**************/    private IEnumerator             statusCoroutine;
        /**************/    private float                   timeSinceTookDamage;
        /**************/    private float                   timeSinceSprinted;
        /**************/    private Coroutine               toxinCoroutine;
        /**************/    private bool                    toxinRunning;
        /**************/    private float                   toxinTimer;
        /**************/    private bool                    weaponControlIsHeld;
        
        public GameObject   CurrentRoom             { get; private set; }
        public float        CurrArmor               { get; private set; }
        public float        CurrHealth              { get; private set; }
        public float        CurrShield              { get; private set; }
        public float        CurrStamina             { get; private set; }
        public bool         IsGodMode               { get; set; }
        public float        MaxArmor                { get; private set; }
        public float        MaxHealth               { get; private set; }
        public float        MaxShield               { get; private set; }
        public float        MaxStamina              { get; private set; }

        public event CallWeaponFireDelegate     CallWeaponFire;
        public event CallWeaponSwitchDelegate   CallWeaponSwitch;
        #endregion

        #region Methods
        /// <summary>
        /// Initialization code
        /// </summary>
        private void Start()
        {
            electricalRunning = false;
            heatRunning = false;
            toxinRunning = false;
            coldRunning = false;

            electricalTimer = 0;
            heatTimer = 0;
            toxinTimer = 0;
            coldTimer = 0;

            MaxArmor = GameConstants.MaxArmor(1);
            MaxHealth = GameConstants.MaxHealth(1);
            MaxShield = GameConstants.MaxShield(1);
            MaxStamina = GameConstants.MaxStamina(1);
            CurrArmor = MaxArmor;
            CurrHealth = MaxHealth;
            CurrShield = MaxShield;
            CurrStamina = MaxStamina;
            armorCanRepair = true;
            timeSinceTookDamage = 0;

            GameEvents.Instance.UpdateUI(UIElement.HUDMaxHealth, MaxHealth);
            GameEvents.Instance.UpdateUI(UIElement.HUDHealth, CurrHealth);
            GameEvents.Instance.UpdateUI(UIElement.HUDMaxShield, MaxShield);
            GameEvents.Instance.UpdateUI(UIElement.HUDShield, CurrShield);
            GameEvents.Instance.UpdateUI(UIElement.HUDMaxArmor, MaxArmor);
            GameEvents.Instance.UpdateUI(UIElement.HUDArmor, CurrArmor);
            GameEvents.Instance.UpdateUI(UIElement.HUDMaxStamina, MaxStamina);
            GameEvents.Instance.UpdateUI(UIElement.HUDStamina, CurrStamina);

            controller.SettingsCamera.cameraSensitivity = PlayerControls.Instance.Data.settingRotationSensitivity;
            controller.SettingsCamera.cameraFOVShiftingEnabled = PlayerControls.Instance.Data.settingFOVShift;
            controller.SettingsCamera.cameraLookInvertedVertical = PlayerControls.Instance.Data.mouseLookInverted;
            controller.GetComponent<ControllerAnimationSystem>().AttachedAnimators[0].enabled = PlayerControls.Instance.Data.settingHeadbob;

            GetSpawnedRoom();
        }

        /// <summary>
        /// Detect whether the player wants to fire their weapons or not
        /// </summary>
        private void Update()
        {
            #region Input and Timers
            if (!controller.MovementIsMovingFast && weaponControlIsHeld && GameEvents.Instance.CurrState == GameState.gplay)
                CallWeaponFire?.Invoke();

            timeSinceTookDamage += Time.deltaTime;
            timeSinceSprinted += Time.deltaTime;
            #endregion

            #region Recharging
            if (timeSinceTookDamage >= GameConstants.DelayRechargeShield && CurrShield < MaxShield)
            {
                CurrShield += GameConstants.RechargeRateArmor(PlayerUpgradeData.Instance.UpgradeLevels[UpgradeElement.PlayerShieldRecharge]) * Time.deltaTime;
                if (CurrShield > MaxShield)
                    CurrShield = MaxShield;
                GameEvents.Instance.UpdateUI(UIElement.HUDShield, CurrShield);
            }

            if (armorCanRepair && timeSinceTookDamage >= GameConstants.DelayRechargeArmor && CurrArmor < MaxArmor)
            {
                CurrArmor += GameConstants.RechargeRateArmor(PlayerUpgradeData.Instance.UpgradeLevels[UpgradeElement.PlayerArmorRecharge]) * Time.deltaTime;
                if (CurrArmor > MaxArmor)
                    CurrArmor = MaxArmor;
                GameEvents.Instance.UpdateUI(UIElement.HUDArmor, CurrArmor);
            }

            if (timeSinceTookDamage >= GameConstants.DelayRechargeHealth && CurrHealth < MaxHealth)
            {
                CurrHealth += GameConstants.RechargeRateArmor(PlayerUpgradeData.Instance.UpgradeLevels[UpgradeElement.PlayerHealthRecharge]) * Time.deltaTime;
                if (CurrHealth > MaxHealth)
                    CurrHealth = MaxHealth;
                GameEvents.Instance.UpdateUI(UIElement.HUDHealth, CurrHealth);
            }
            #endregion

            #region Stamina
            if (controller.MovementIsMovingFast)
            {
                timeSinceSprinted = 0;
                if (CurrStamina > 0)
                {
                    CurrStamina -= GameConstants.StaminaDepletionRate * Time.deltaTime;
                    if (CurrStamina <= 0)
                    {
                        CurrStamina = 0;
                        controller.DisableMovementSafely(MovementType.fast);
                    }
                    GameEvents.Instance.UpdateUI(UIElement.HUDStamina, CurrStamina);
                }
            }

            else if (timeSinceSprinted >= GameConstants.DelayRechargeStamina && CurrStamina < MaxStamina)
            {
                CurrStamina += GameConstants.RechargeRateArmor(PlayerUpgradeData.Instance.UpgradeLevels[UpgradeElement.PlayerStaminaRecharge]) * Time.deltaTime;
                if (CurrStamina > MaxStamina)
                    CurrStamina = MaxStamina;
                if (!controller.SettingsMovement.canMoveFast)
                    controller.SettingsMovement.canMoveFast = true;
                GameEvents.Instance.UpdateUI(UIElement.HUDStamina, CurrStamina);
            }
            #endregion
        }

        /// <summary>
        /// Subscribe to upgrade event
        /// </summary>
        private void OnEnable()
        {
            GameEvents.PlayerUpgrade += UpgradeStat;
            GameEvents.UpdateGameState += OnUpdateGameState;
        }

        /// <summary>
        /// Unsubscribe from upgrade event
        /// </summary>
        private void OnDisable()
        {
            GameEvents.PlayerUpgrade -= UpgradeStat;
            GameEvents.UpdateGameState -= OnUpdateGameState;
        }

        /// <summary>
        /// Handler for the Console input event
        /// </summary>
        /// <param name="context">The context associated with the input</param>
        public void OnConsole(InputAction.CallbackContext context)
        {
            if (context.performed)
                if (GameEvents.Instance.CurrState == GameState.gplay || GameEvents.Instance.CurrState == GameState.consl)
                    GameEvents.Instance.ToggleConsole();
        }

        /// <summary>
        /// Handler for the FireWeapon input event
        /// </summary>
        /// <param name="context">The context associated with the input</param>
        public void OnFireWeapon(InputAction.CallbackContext context)
        {
            weaponControlIsHeld = context.performed;
        }

        /// <summary>
        /// Handler for the Pause input event
        /// </summary>
        /// <param name="context">The context associated with the input</param>
        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed)
                if (GameEvents.Instance.CurrState == GameState.gplay || GameEvents.Instance.CurrState == GameState.pause)
                    GameEvents.Instance.TogglePause();
        }

        /// <summary>
        /// Handler for the SwitchWeapon input event
        /// </summary>
        /// <param name="context">The context associated with the input</param>
        public void OnSwitchWeapon(InputAction.CallbackContext context)
        {
            if (context.performed && GameEvents.Instance.CurrState == GameState.gplay)
                CallWeaponSwitch?.Invoke();
        }

        /// <summary>
        /// Player takes damage based on incoming attack
        /// </summary>
        /// <param name="damage">The magnitude of the entity's attack</param>
        public void TakeDamage(float damage, Dictionary<AttackModifier, float> modifiers)
        {
            // As long as godmode is not enabled, take damage
            if (!IsGodMode)
            {
                timeSinceTookDamage = 0;

                if (CurrShield > 0)
                {
                    if (modifiers.ContainsKey(AttackModifier.Shield))
                        CurrShield -= damage + (damage * modifiers[AttackModifier.Shield]);
                    else
                        CurrShield -= damage;

                    if (CurrShield < 0)
                        CurrShield = 0;

                    GameEvents.Instance.UpdateUI(UIElement.HUDShield, CurrShield);
                }

                else
                {
                    if (CurrArmor > 0)
                    {
                        if (modifiers.ContainsKey(AttackModifier.Armor))
                            CurrArmor -= damage + (damage * modifiers[AttackModifier.Armor]);
                        else
                            CurrArmor -= damage;

                        if (CurrArmor < 0)
                            CurrArmor = 0;

                        GameEvents.Instance.UpdateUI(UIElement.HUDArmor, CurrArmor);
                    }

                    if (modifiers.ContainsKey(AttackModifier.Health))
                        CurrHealth -= (damage + (damage * modifiers[AttackModifier.Health])) * (1 - (CurrArmor / 1000));
                    else
                        CurrHealth -= damage * (1 - (CurrArmor / 1000));

                    GameEvents.Instance.UpdateUI(UIElement.HUDHealth, CurrHealth);

                    // If the player has died, toggle the restart canvas
                    if (CurrHealth <= 0)
                        GameEvents.Instance.CallPlayerDeath();
                }
            }
        }

        /// <summary>
        /// Player is afflicted with a status effect from an entity attack
        /// </summary>
        /// <param name="effect">The type of effect to afflict the player with</param>
        /// <param name="time">The duration of the effect</param>
        /// <param name="intensity">The intensity of the effect</param>
        public void TakeDamage(StatusEffect effect, float time, float intensity)
        {
            if (!IsGodMode)
            {
                // For each effect:
                    // 1: Handle timers and coroutines based on effect type
                    // 2: Determine if the status effect time is less than the new time; update the time
                    // 3: If the coroutine isn't running, update the reference and start it; otherwise, do nothing (electrical coroutine will continue going)

                if (effect == StatusEffect.Electrical)
                {
                    if (electricalTimer < time)
                    {
                        electricalTimer = time;

                        if (electricalCoroutine == null || !electricalRunning)
                        {
                            statusCoroutine = TakeStatusDamage(effect, intensity);
                            electricalCoroutine = StartCoroutine(statusCoroutine);
                        }
                    }
                }

                else if (effect == StatusEffect.Heat)
                {
                    if (heatTimer < time)
                    {
                        heatTimer = time;

                        if (heatCoroutine == null || !heatRunning)
                        {
                            statusCoroutine = TakeStatusDamage(effect, intensity);
                            heatCoroutine = StartCoroutine(statusCoroutine);
                        }
                    }
                }

                else if (effect == StatusEffect.Toxin)
                {
                    if (toxinTimer < time)
                    {
                        toxinTimer = time;

                        if (toxinCoroutine == null || !toxinRunning)
                        {
                            statusCoroutine = TakeStatusDamage(effect, intensity);
                            toxinCoroutine = StartCoroutine(statusCoroutine);
                        }
                    }
                }

                else if (effect == StatusEffect.Cold)
                {
                    if (coldTimer < time)
                    {
                        coldTimer = time;

                        if (coldCoroutine == null || !coldRunning)
                        {
                            statusCoroutine = TakeStatusDamage(effect, intensity);
                            coldCoroutine = StartCoroutine(statusCoroutine);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Replenishes a player's stats
        /// </summary>
        /// <param name="elem">UIElement that specifies what player stat; any non-player stat will simply fail</param>
        /// <param name="amt">The amount to heal by</param>
        public void Heal(UIElement elem, float amt)
        {
            switch (elem)
            {
                case UIElement.HUDHealth:
                case UIElement.HUDMaxHealth:
                    CurrHealth += amt;
                    if (CurrHealth > MaxHealth)
                        CurrHealth = MaxHealth;
                    StopAllCoroutines();
                    GameEvents.Instance.UpdateUI(UIElement.HUDStatusElectrical, UIState.disabled, 0);
                    GameEvents.Instance.UpdateUI(UIElement.HUDStatusHeat, UIState.disabled, 0);
                    GameEvents.Instance.UpdateUI(UIElement.HUDStatusToxin, UIState.disabled, 0);
                    GameEvents.Instance.UpdateUI(UIElement.HUDStatusCold, UIState.disabled, 0);
                    GameEvents.Instance.UpdateUI(UIElement.HUDHealth, CurrHealth);
                    break;

                case UIElement.HUDShield:
                case UIElement.HUDMaxShield:
                    CurrShield += amt;
                    if (CurrShield > MaxShield)
                        CurrShield = MaxShield;
                    break;
                case UIElement.HUDStamina:
                case UIElement.HUDMaxStamina:
                    CurrStamina += amt;
                    if (CurrStamina > MaxStamina)
                        CurrStamina = MaxStamina;
                    break;
                case UIElement.HUDArmor:
                case UIElement.HUDMaxArmor:
                    CurrArmor += amt;
                    if (CurrArmor > MaxArmor)
                        CurrArmor = MaxArmor;
                    break;

                case UIElement.HUDStatusToxin:
                    StopCoroutine(toxinCoroutine);
                    GameEvents.Instance.UpdateUI(UIElement.HUDStatusToxin, UIState.disabled, 0);
                    break;
                case UIElement.HUDStatusHeat:
                    StopCoroutine(heatCoroutine);
                    GameEvents.Instance.UpdateUI(UIElement.HUDStatusHeat, UIState.disabled, 0);
                    break;
                case UIElement.HUDStatusElectrical:
                    StopCoroutine(electricalCoroutine);
                    GameEvents.Instance.UpdateUI(UIElement.HUDStatusElectrical, UIState.disabled, 0);
                    break;
                case UIElement.HUDStatusCold:
                    StopCoroutine(coldCoroutine);
                    GameEvents.Instance.UpdateUI(UIElement.HUDStatusCold, UIState.disabled, 0);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Gets the room the player spawned in when ready
        /// </summary>
        private void GetSpawnedRoom()
        {
            GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");
            GameObject closest = null;
            float dist = Mathf.Infinity;
            foreach (GameObject room in rooms)
            {
                Vector3 diff = room.GetComponent<BoxCollider>().ClosestPointOnBounds(transform.position);
                float diffDist = Vector3.Distance(diff, transform.position);
                if (diffDist < dist)
                {
                    dist = diffDist;
                    closest = room;
                }
            }
            CurrentRoom = closest;
        }

        /// <summary>
        /// When the player enters a room, reference it in CurrentRoom
        /// </summary>
        /// <param name="other">The trigger collider entered</param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Room"))
                CurrentRoom = other.gameObject;
        }

        /// <summary>
        /// Ensures player controller can move or not depending on updated gamestate
        /// </summary>
        /// <param name="prev">The previous gamestate</param>
        /// <param name="curr">The current gamestate</param>
        private void OnUpdateGameState(GameState prev, GameState curr)
        {
            if (curr == GameState.gplay)
                controller.SettingsMovement.canMove = true;
            else
                controller.SettingsMovement.canMove = false;
        }

        /// <summary>
        /// Handles decrementing player health by status effect damage per second active
        /// </summary>
        /// <param name="effect">The effect the player is afflicted with</param>
        /// <param name="intensity">The intensity of the effect</param>
        private IEnumerator TakeStatusDamage(StatusEffect effect, float intensity)
        {
            #region Run effects
            // For each effect:
                // 1: Update the indicator of the status of this coroutine ensuring that duplicate coroutines don't run
                // 2: Update the HUD
                // 3: Handle player death
                // 4: Decrement the timer, pause for 1 second, then repeat
                // 5: Update the indicator of the status of this coroutine ensuring that duplicate coroutines don't run

            if (effect == StatusEffect.Electrical)
            {
                electricalRunning = true;

                while (electricalTimer > 0)
                {
                    timeSinceTookDamage = 0;
                    GameEvents.Instance.UpdateUI(UIElement.HUDStatusElectrical, UIState.enabled, electricalTimer);
                    TakeDamage(intensity, new Dictionary<AttackModifier, float> { { AttackModifier.Shield, 0.5f } });
                    GameEvents.Instance.UpdateUI(UIElement.HUDHealth, CurrHealth);

                    if (CurrHealth <= 0)
                        break;

                    electricalTimer--;
                    yield return new WaitForSeconds(1);
                }

                electricalRunning = false;
            }

            else if (effect == StatusEffect.Heat)
            {
                heatRunning = true;

                while (heatTimer > 0)
                {
                    timeSinceTookDamage = 0;
                    GameEvents.Instance.UpdateUI(UIElement.HUDStatusHeat, UIState.enabled, heatTimer);
                    TakeDamage(intensity, new Dictionary<AttackModifier, float> { { AttackModifier.Armor, 0.5f } });
                    GameEvents.Instance.UpdateUI(UIElement.HUDHealth, CurrHealth);

                    if (CurrHealth <= 0)
                        break;

                    heatTimer--;
                    yield return new WaitForSeconds(1);
                }

                heatRunning = false;
            }

            else if (effect == StatusEffect.Toxin)
            {
                toxinRunning = true;

                while (toxinTimer > 0)
                {
                    timeSinceTookDamage = 0;
                    GameEvents.Instance.UpdateUI(UIElement.HUDStatusToxin, UIState.enabled, toxinTimer);
                    CurrHealth -= intensity;    // toxin bypasses armor/shields
                    GameEvents.Instance.UpdateUI(UIElement.HUDHealth, CurrHealth);

                    if (CurrHealth <= 0)
                        break;

                    toxinTimer--;
                    yield return new WaitForSeconds(1);
                }

                toxinRunning = false;
            }

            else if (effect == StatusEffect.Cold)
            {
                coldRunning = true;

                while (coldTimer > 0)
                {
                    timeSinceTookDamage = 0;
                    GameEvents.Instance.UpdateUI(UIElement.HUDStatusCold, UIState.enabled, coldTimer);
                    CurrArmor -= intensity;     // cold bypasses shields and affects only armor
                    if (CurrArmor < 0)
                        CurrArmor = 0;
                    GameEvents.Instance.UpdateUI(UIElement.HUDHealth, CurrHealth);

                    if (CurrHealth <= 0)
                        break;

                    coldTimer--;
                    yield return new WaitForSeconds(1);
                }

                coldRunning = false;
            }
            #endregion

            #region End effects in UI
            switch (effect)
            {
                case StatusEffect.Cold:
                    GameEvents.Instance.UpdateUI(UIElement.HUDStatusCold, UIState.disabled, 0);
                    break;
                case StatusEffect.Electrical:
                    GameEvents.Instance.UpdateUI(UIElement.HUDStatusElectrical, UIState.disabled, 0);
                    break;
                case StatusEffect.Heat:
                    GameEvents.Instance.UpdateUI(UIElement.HUDStatusHeat, UIState.disabled, 0);
                    break;
                case StatusEffect.Toxin:
                    GameEvents.Instance.UpdateUI(UIElement.HUDStatusToxin, UIState.disabled, 0);
                    break;
            }
            #endregion
        }

        /// <summary>
        /// Upgrades one of the player's stats when the OnUpgrade event is called
        /// </summary>
        /// <param name="newVal">The new value</param>
        private void UpgradeStat(UpgradeElement elem, int newVal)
        {
            switch (elem)
            {
                case UpgradeElement.PlayerHealthAmount:
                    MaxHealth = GameConstants.MaxHealth(newVal);
                    CurrHealth = MaxHealth;
                    GameEvents.Instance.UpdateUI(UIElement.HUDHealth, CurrHealth);
                    GameEvents.Instance.UpdateUI(UIElement.HUDMaxHealth, MaxHealth);
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}