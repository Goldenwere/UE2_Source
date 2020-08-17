#pragma warning disable 0649

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using Entity2.Core;
using Entity2.Data;
using Goldenwere.Unity;
using Goldenwere.Unity.UI;

namespace Entity2.UI
{
    /// <summary>
    /// Controls the HUD and windows that are present when playing the game
    /// </summary>
    public class GameHUD : MonoBehaviour
    {
        #region Fields
        [SerializeField]    private PlayerInput                                     attachedControls;
        [SerializeField]    private EventSystem                                     eventSystem;
        [SerializeField]    private GameObject                                      firstSelectedConsole;
        [SerializeField]    private GameObject                                      firstSelectedDeath;
        [SerializeField]    private GameObject                                      firstSelectedPause;
        [SerializeField]    private HUDPlayerStats                                  hudPlayerStats;
        [SerializeField]    private HUDPlayerEffects                                hudPlayerEffects;
        [SerializeField]    private HUDCanvases                                     hudCanvases;
        [SerializeField]    private HUDOtherElements                                hudOtherElements;
        /**************/    private Dictionary<CanvasGroup, float>                  elementLastUpdatedTimers;
        /**************/    private Dictionary<CanvasGroup, FadeInOutElements>      elementsFadeInOut;
        /**************/    private bool                                            motionRotation;
        /**************/    private bool                                            settingHUDMotion;
        /**************/    private bool                                            settingHUDPersistent;
        /**************/    private Dictionary<Slider, SliderTransitionExtension>   transitionedSliderElements;
        #endregion

        #region Inline Classes
        /// <summary>
        /// Elements of the HUD dealing with player stats
        /// </summary>
        [Serializable] public class HUDPlayerStats
        {
            [SerializeField]    private CanvasGroup         groupArmor;
            [SerializeField]    private CanvasGroup         groupHealth;
            [SerializeField]    private CanvasGroup         groupShield;
            [SerializeField]    private CanvasGroup         groupStamina;
            [SerializeField]    private TextMeshProUGUI     textArmor;
            [SerializeField]    private TextMeshProUGUI     textHealth;
            [SerializeField]    private TextMeshProUGUI     textShield;
            [SerializeField]    private TextMeshProUGUI     textStamina;
            [SerializeField]    private Slider              sliderArmor;
            [SerializeField]    private Slider              sliderHealth;
            [SerializeField]    private Slider              sliderShield;
            [SerializeField]    private Slider              sliderStamina;

            public CanvasGroup      Group_Armor             { get { return groupArmor; } }
            public CanvasGroup      Group_Health            { get { return groupHealth; } }
            public CanvasGroup      Group_Shield            { get { return groupShield; } }
            public CanvasGroup      Group_Stamina           { get { return groupStamina; } }
            public TextMeshProUGUI  Text_Armor              { get { return textArmor; } }
            public TextMeshProUGUI  Text_Health             { get { return textHealth; } }
            public TextMeshProUGUI  Text_Shield             { get { return textShield; } }
            public TextMeshProUGUI  Text_Stamina            { get { return textStamina; } }
            public Slider           Slider_Armor            { get { return sliderArmor; } }
            public Slider           Slider_Health           { get { return sliderHealth; } }
            public Slider           Slider_Shield           { get { return sliderShield; } }
            public Slider           Slider_Stamina          { get { return sliderStamina; } }
        }

        /// <summary>
        /// Elements of the HUD dealing with player status effects
        /// </summary>
        [Serializable] public class HUDPlayerEffects
        {
            [SerializeField]    private GameObject          statusElectrical;
            [SerializeField]    private GameObject          statusHeat;
            [SerializeField]    private GameObject          statusToxin;
            [SerializeField]    private GameObject          statusCold;
            [SerializeField]    private TextMeshProUGUI     timeElectrical;
            [SerializeField]    private TextMeshProUGUI     timeHeat;
            [SerializeField]    private TextMeshProUGUI     timeToxin;
            [SerializeField]    private TextMeshProUGUI     timeCold;

            public GameObject       Status_Electrical       { get { return statusElectrical; } }
            public GameObject       Status_Heat             { get { return statusHeat; } }
            public GameObject       Status_Toxin            { get { return statusToxin; } }
            public GameObject       Status_Cold             { get { return statusCold; } }
            public TextMeshProUGUI  Time_Electrical         { get { return timeElectrical; } }
            public TextMeshProUGUI  Time_Heat               { get { return timeHeat; } }
            public TextMeshProUGUI  Time_Toxin              { get { return timeToxin; } }
            public TextMeshProUGUI  Time_Cold               { get { return timeCold; } }
        }

        /// <summary>
        /// Canvas gameobjects for the HUD/in-game menus
        /// </summary>
        [Serializable] public class HUDCanvases
        {
            [SerializeField]    private GameObject          hud;
            [SerializeField]    private GameObject          console;
            [SerializeField]    private GameObject          pause;
            [SerializeField]    private GameObject          death;
            [SerializeField]    private DevConsole          consoleComp;

            public GameObject       Canvas_HUD              { get { return hud; } }
            public GameObject       Canvas_Console          { get { return console; } }
            public GameObject       Canvas_Pause            { get { return pause; } }
            public GameObject       Canvas_Death            { get { return death; } }
            public DevConsole       Comp_Console            { get { return consoleComp; } }
        }

        /// <summary>
        /// Elements of the HUD that do not fit into other categories
        /// </summary>
        [Serializable] public class HUDOtherElements
        {
            [SerializeField]    private SVGImage            crosshair;
            [SerializeField]    private GameObject          entityHealthContainer;
            [SerializeField]    private Slider              entityHealthSlider;
            [SerializeField]    private TextMeshProUGUI     entityHealthText;
            [SerializeField]    private CanvasGroup         groupApplyHUDShake;
            [SerializeField]    private TextMeshProUGUI     textExperience;
            [SerializeField]    private TextMeshProUGUI     textObjective;

            public GameObject       Entity_HealthContainer  { get { return entityHealthContainer; } }
            public Slider           Entity_HealthSlider     { get { return entityHealthSlider; } }
            public TextMeshProUGUI  Entity_HealthText       { get { return entityHealthText; } }
            public CanvasGroup      Group_ApplyHUDShake     { get { return groupApplyHUDShake; } }
            public TextMeshProUGUI  Text_Ammo               { get; set; }
            public TextMeshProUGUI  Text_Experience         { get { return textExperience; } }
            public TextMeshProUGUI  Text_Objective          { get { return textObjective; } }
            public SVGImage         Crosshair               { get { return crosshair; } }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Ensures slider transition extensions are updated when MonoBehaviour.Update is called
        /// </summary>
        private void Update()
        {
            foreach (SliderTransitionExtension s in transitionedSliderElements.Values)
                s.Update();

            #region Update fade state of sliders
            if (!settingHUDPersistent && elementLastUpdatedTimers[hudPlayerStats.Group_Armor] >= 0)
            {
                if (elementLastUpdatedTimers[hudPlayerStats.Group_Armor] >= GameConstants.HUDTimeUntilDeactivation)
                {
                    elementsFadeInOut[hudPlayerStats.Group_Armor].FadeOut();
                    elementLastUpdatedTimers[hudPlayerStats.Group_Armor] = -1;
                }
                else
                    elementLastUpdatedTimers[hudPlayerStats.Group_Armor] += Time.deltaTime;
            }

            if (!settingHUDPersistent && elementLastUpdatedTimers[hudPlayerStats.Group_Health] >= 0)
            {
                if (elementLastUpdatedTimers[hudPlayerStats.Group_Health] >= GameConstants.HUDTimeUntilDeactivation)
                {
                    elementsFadeInOut[hudPlayerStats.Group_Health].FadeOut();
                    elementLastUpdatedTimers[hudPlayerStats.Group_Health] = -1;
                }
                else
                    elementLastUpdatedTimers[hudPlayerStats.Group_Health] += Time.deltaTime;
            }

            if (!settingHUDPersistent && elementLastUpdatedTimers[hudPlayerStats.Group_Shield] >= 0)
            {
                if (elementLastUpdatedTimers[hudPlayerStats.Group_Shield] >= GameConstants.HUDTimeUntilDeactivation)
                {
                    elementsFadeInOut[hudPlayerStats.Group_Shield].FadeOut();
                    elementLastUpdatedTimers[hudPlayerStats.Group_Shield] = -1;
                }
                else
                    elementLastUpdatedTimers[hudPlayerStats.Group_Shield] += Time.deltaTime;
            }

            if (!settingHUDPersistent && elementLastUpdatedTimers[hudPlayerStats.Group_Stamina] >= 0)
            {
                if (elementLastUpdatedTimers[hudPlayerStats.Group_Stamina] >= GameConstants.HUDTimeUntilDeactivation)
                {
                    elementsFadeInOut[hudPlayerStats.Group_Stamina].FadeOut();
                    elementLastUpdatedTimers[hudPlayerStats.Group_Stamina] = -1;
                }
                else
                    elementLastUpdatedTimers[hudPlayerStats.Group_Stamina] += Time.deltaTime;
            }
            #endregion

            if (motionRotation)
            {
                Vector2 value = attachedControls.actions["Rotation"].ReadValue<Vector2>();
                Quaternion q = Quaternion.Euler(-value.y * GameConstants.HUDMotionScale  * 2, value.x * GameConstants.HUDMotionScale, 0);
                hudOtherElements.Group_ApplyHUDShake.transform.localRotation = Quaternion.Slerp(
                    hudOtherElements.Group_ApplyHUDShake.transform.localRotation, q, Time.deltaTime * GameConstants.HUDMotionSpeedMovement);
            }

            else
            {
                hudOtherElements.Group_ApplyHUDShake.transform.localRotation = Quaternion.Slerp(
                    hudOtherElements.Group_ApplyHUDShake.transform.localRotation, Quaternion.identity, Time.deltaTime * GameConstants.HUDMotionSpeedRevert);
            }
        }

        /// <summary>
        /// Used when the HUD is first attached to the player
        /// </summary>
        /// <param name="player">The player that was spawned</param>
        public void Initialize(GameObject player)
        {
            settingHUDMotion = PlayerControls.Instance.Data.settingHUDMotion;
            settingHUDPersistent = PlayerControls.Instance.Data.settingPersistentHUD;
            hudCanvases.Comp_Console.Initialize(player);

            elementLastUpdatedTimers = new Dictionary<CanvasGroup, float>
            {
                { hudPlayerStats.Group_Armor, 0 },
                { hudPlayerStats.Group_Health, 0 },
                { hudPlayerStats.Group_Shield, 0 },
                { hudPlayerStats.Group_Stamina, 0 }
            };

            elementsFadeInOut = new Dictionary<CanvasGroup, FadeInOutElements>
            {
                {
                    hudPlayerStats.Group_Armor,
                    new FadeInOutElements(this, hudPlayerStats.Group_Armor, GameConstants.HUDFadeInDuration, GameConstants.HUDFadeOutDuration, GameConstants.CurveHUDFade())
                },
                {
                    hudPlayerStats.Group_Health,
                    new FadeInOutElements(this, hudPlayerStats.Group_Health, GameConstants.HUDFadeInDuration, GameConstants.HUDFadeOutDuration, GameConstants.CurveHUDFade())
                },
                {
                    hudPlayerStats.Group_Shield,
                    new FadeInOutElements(this, hudPlayerStats.Group_Shield, GameConstants.HUDFadeInDuration, GameConstants.HUDFadeOutDuration, GameConstants.CurveHUDFade())
                },
                {
                    hudPlayerStats.Group_Stamina,
                    new FadeInOutElements(this, hudPlayerStats.Group_Stamina, GameConstants.HUDFadeInDuration, GameConstants.HUDFadeOutDuration, GameConstants.CurveHUDFade())
                }
            };

            transitionedSliderElements = new Dictionary<Slider, SliderTransitionExtension>()
            {
                {
                    hudPlayerStats.Slider_Armor,
                    new SliderTransitionExtension(this, hudPlayerStats.Slider_Armor, GameConstants.SliderTransitionLength, GameConstants.SliderLengthBetweenUpdates)
                },
                {
                    hudPlayerStats.Slider_Health,
                    new SliderTransitionExtension(this, hudPlayerStats.Slider_Health, GameConstants.SliderTransitionLength, GameConstants.SliderLengthBetweenUpdates)
                },
                {
                    hudPlayerStats.Slider_Shield,
                    new SliderTransitionExtension(this, hudPlayerStats.Slider_Shield, GameConstants.SliderTransitionLength, GameConstants.SliderLengthBetweenUpdates)
                },
                {
                    hudPlayerStats.Slider_Stamina,
                    new SliderTransitionExtension(this, hudPlayerStats.Slider_Stamina, GameConstants.SliderTransitionLength, GameConstants.SliderLengthBetweenUpdates)
                },
                {
                    hudOtherElements.Entity_HealthSlider,
                    new SliderTransitionExtension(this, hudOtherElements.Entity_HealthSlider, GameConstants.SliderTransitionLength, GameConstants.SliderLengthBetweenUpdates)
                }
            };

            StopAllCoroutines();
        }

        /// <summary>
        /// Handler for the Rotation input event (defined in ControllerActions)
        /// </summary>
        /// <param name="context">The context associated with the input</param>
        public void OnRotation(InputAction.CallbackContext context)
        {
            if (settingHUDMotion)
                motionRotation = context.performed;
        }

        #region Non-player Stats
        /// <summary>
        /// Updates the current ammo value displayed on the weapon's text element
        /// </summary>
        /// <param name="num">The value to display</param>
        public void UpdateAmmoText(float num)
        {
            hudOtherElements.Text_Ammo.text = num.ToString();
        }

        /// <summary>
        /// Updates what the ammo text element variable is referencing to when a weapon was switched
        /// </summary>
        /// <param name="w">The newly equipped weapon</param>
        public void UpdateAmmoTextObject(GameObject w)
        {
            hudOtherElements.Text_Ammo = w.FindChildRecursively("AmmoText").GetComponent<TextMeshProUGUI>();
        }

        /// <summary>
        /// When the GameState changes, update HUD canvases accordingly
        /// </summary>
        /// <param name="gState">The updated GameState</param>
        public void UpdateCanvases(GameState gState)
        {
            switch (gState)
            {
                case GameState.consl:
                    hudCanvases.Canvas_Console.SetActive(true);
                    hudCanvases.Canvas_Death.SetActive(false);
                    hudCanvases.Canvas_Pause.SetActive(false);
                    hudCanvases.Canvas_HUD.SetActive(false);
                    hudCanvases.Comp_Console.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    Time.timeScale = 0;
                    eventSystem.SetSelectedGameObject(firstSelectedConsole);
                    break;

                case GameState.death:
                    hudCanvases.Canvas_Console.SetActive(false);
                    hudCanvases.Canvas_Death.SetActive(true);
                    hudCanvases.Canvas_Pause.SetActive(false);
                    hudCanvases.Canvas_HUD.SetActive(false);
                    hudCanvases.Comp_Console.SetActive(false);
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                    Time.timeScale = 0.1f;
                    eventSystem.SetSelectedGameObject(firstSelectedDeath);
                    break;

                case GameState.pause:
                    hudCanvases.Canvas_Console.SetActive(false);
                    hudCanvases.Canvas_Death.SetActive(false);
                    hudCanvases.Canvas_Pause.SetActive(true);
                    hudCanvases.Canvas_HUD.SetActive(false);
                    hudCanvases.Comp_Console.SetActive(false);
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                    Time.timeScale = 0;
                    eventSystem.SetSelectedGameObject(firstSelectedPause);
                    break;

                case GameState.gplay:
                    hudCanvases.Canvas_Console.SetActive(false);
                    hudCanvases.Canvas_Death.SetActive(false);
                    hudCanvases.Canvas_Pause.SetActive(false);
                    hudCanvases.Canvas_HUD.SetActive(true);
                    hudCanvases.Comp_Console.SetActive(false);
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    Time.timeScale = 1;
                    eventSystem.SetSelectedGameObject(null);
                    break;

                case GameState.menus:
                default:
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    Time.timeScale = 1;
                    break;
            }
        }

        /// <summary>
        /// Updates the color of the crosshair based on what the player is targetting
        /// </summary>
        /// <param name="target">The category of the target</param>
        public void UpdateCrossHair(CrosshairTarget target)
        {
            if (target == CrosshairTarget.none)
                hudOtherElements.Crosshair.color = GameConstants.CrosshairNormal();

            else if (target == CrosshairTarget.entity)
                hudOtherElements.Crosshair.color = GameConstants.CrosshairEntity();

            else if (target == CrosshairTarget.ui)
                hudOtherElements.Crosshair.color = GameConstants.CrosshairInterface();

            else
                hudOtherElements.Crosshair.color = GameConstants.CrosshairNormal();
        }

        /// <summary>
        /// Updates the entity health text/slider elements (This overload disables the parent object)
        /// </summary>
        public void UpdateEntCurrHealth()
        {
            hudOtherElements.Entity_HealthContainer.SetActive(false);
        }

        /// <summary>
        /// Updates the entity health text/slider elements
        /// </summary>
        /// <param name="num">The value to display</param>
        public void UpdateEntCurrHealth(float num)
        {
            hudOtherElements.Entity_HealthText.text = string.Format("{0:0}", num);
            transitionedSliderElements[hudOtherElements.Entity_HealthSlider].UpdateValue(num);
        }

        /// <summary>
        /// Updates the max value of the entity health slider
        /// </summary>
        /// <param name="num">The value to set Slider.maxValue to</param>
        public void UpdateEntMaxHealth(float num)
        {
            hudOtherElements.Entity_HealthSlider.maxValue = num;
            hudOtherElements.Entity_HealthContainer.SetActive(true);
        }

        /// <summary>
        /// Updates the experience text on the HUD
        /// </summary>
        /// <param name="content">The new value of experience to display</param>
        public void UpdateExpText(string content)
        {
            hudOtherElements.Text_Experience.text = "Experience: " + content;
        }

        /// <summary>
        /// Updates the objective text on the HUD
        /// </summary>
        /// <param name="content">The description of the new objective</param>
        public void UpdateObjectiveText(string content)
        {
            hudOtherElements.Text_Objective.text = content;
        }
        #endregion

        #region Player Stats
        /// <summary>
        /// Updates the player armor text/slider elements
        /// </summary>
        /// <param name="num">The value to display</param>
        public void UpdatePlayerCurrArmor(float num)
        {
            if (num < hudPlayerStats.Slider_Armor.value)
            {
                if (elementLastUpdatedTimers[hudPlayerStats.Group_Armor] <= 0)
                    elementsFadeInOut[hudPlayerStats.Group_Armor].FadeIn();
                elementLastUpdatedTimers[hudPlayerStats.Group_Armor] = 0;
            }

            hudPlayerStats.Text_Armor.text = string.Format("{0:0}", num);
            transitionedSliderElements[hudPlayerStats.Slider_Armor].UpdateValue(num);
        }

        /// <summary>
        /// Updates the player health text/slider elements
        /// </summary>
        /// <param name="num">The value to display</param>
        public void UpdatePlayerCurrHealth(float num)
        {
            if (num < hudPlayerStats.Slider_Health.value)
            {
                if (elementLastUpdatedTimers[hudPlayerStats.Group_Health] <= 0)
                    elementsFadeInOut[hudPlayerStats.Group_Health].FadeIn();
                elementLastUpdatedTimers[hudPlayerStats.Group_Health] = 0;
            }

            hudPlayerStats.Text_Health.text = string.Format("{0:0}", num);
            transitionedSliderElements[hudPlayerStats.Slider_Health].UpdateValue(num);
        }

        /// <summary>
        /// Updates the player shield text/slider elements
        /// </summary>
        /// <param name="num">The value to display</param>
        public void UpdatePlayerCurrShield(float num)
        {
            if (num < hudPlayerStats.Slider_Shield.value)
            {
                if (elementLastUpdatedTimers[hudPlayerStats.Group_Shield] <= 0)
                    elementsFadeInOut[hudPlayerStats.Group_Shield].FadeIn();
                elementLastUpdatedTimers[hudPlayerStats.Group_Shield] = 0;
            }

            hudPlayerStats.Text_Shield.text = string.Format("{0:0}", num);
            transitionedSliderElements[hudPlayerStats.Slider_Shield].UpdateValue(num);
        }

        /// <summary>
        /// Updates the player stamina text/slider elements
        /// </summary>
        /// <param name="num">The value to display</param>
        public void UpdatePlayerCurrStamina(float num)
        {
            if (num < hudPlayerStats.Slider_Stamina.value)
            {
                if (elementLastUpdatedTimers[hudPlayerStats.Group_Stamina] <= 0)
                    elementsFadeInOut[hudPlayerStats.Group_Stamina].FadeIn();
                elementLastUpdatedTimers[hudPlayerStats.Group_Stamina] = 0;
            }

            hudPlayerStats.Text_Stamina.text = string.Format("{0:0}", num);
            transitionedSliderElements[hudPlayerStats.Slider_Stamina].UpdateValue(num);
        }

        /// <summary>
        /// Updates the max value of the player armor slider
        /// </summary>
        /// <param name="num">The value to set Slider.maxValue to</param>
        public void UpdatePlayerMaxArmor(float num)
        {
            hudPlayerStats.Slider_Armor.maxValue = num;
        }

        /// <summary>
        /// Updates the max value of the player health slider
        /// </summary>
        /// <param name="num">The value to set Slider.maxValue to</param>
        public void UpdatePlayerMaxHealth(float num)
        {
            hudPlayerStats.Slider_Health.maxValue = num;
        }

        /// <summary>
        /// Updates the max value of the player shield slider
        /// </summary>
        /// <param name="num">The value to set Slider.maxValue to</param>
        public void UpdatePlayerMaxShield(float num)
        {
            hudPlayerStats.Slider_Shield.maxValue = num;
        }

        /// <summary>
        /// Updates the max value of the player stamina slider
        /// </summary>
        /// <param name="num">The value to set Slider.maxValue to</param>
        public void UpdatePlayerMaxStamina(float num)
        {
            hudPlayerStats.Slider_Stamina.maxValue = num;
        }

        /// <summary>
        /// Updates the status effects based on incoming status effect time and type
        /// </summary>
        /// <param name="effect">The effect type (determines which elements to effect)</param>
        /// <param name="state">The state of the status effect (use to disable the icon after hitting 0)</param>
        /// <param name="time">The time left for the status effect (displayed in the text element)</param>
        public void UpdateStatusEffect(StatusEffect effect, bool state, float time)
        {
            if (effect == StatusEffect.Electrical)
            {
                hudPlayerEffects.Status_Electrical.SetActive(state);
                hudPlayerEffects.Time_Electrical.text = time.ToString();
            }

            else if (effect == StatusEffect.Heat)
            {
                hudPlayerEffects.Status_Heat.SetActive(state);
                hudPlayerEffects.Time_Heat.text = time.ToString();
            }

            else if (effect == StatusEffect.Toxin)
            {
                hudPlayerEffects.Status_Toxin.SetActive(state);
                hudPlayerEffects.Time_Toxin.text = time.ToString();
            }

            else if (effect == StatusEffect.Cold)
            {
                hudPlayerEffects.Status_Cold.SetActive(state);
                hudPlayerEffects.Time_Cold.text = time.ToString();
            }
        }
        #endregion
        #endregion
    }
}