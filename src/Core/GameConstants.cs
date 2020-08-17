using UnityEngine;

namespace Entity2.Core
{
    /// <summary>
    /// Collection of constants used by the game
    /// </summary>
    public struct GameConstants
    {
        public const string     AudioParamEffects                   = "VolEffects";
        public const string     AudioParamEnvironment               = "VolEnvironment";
        public const string     AudioParamInterface                 = "VolInterface";
        public const string     AudioParamMaster                    = "VolMaster";
        public const string     AudioParamMusic                     = "VolMusic";
        public const float      CanvasMinimizeSpeed                 = 0.5f;
        public const float      CanvasRotationSpeed                 = 5.0f;
        public const float      CanvasPositionSpeed                 = 10.0f;
        public const string     DataPathControls                    = "/controls.xml";
        public const string     DataPathSettings                    = "/settings.xml";
        public const float      DelayRechargeArmor                  = 6.0f;
        public const float      DelayRechargeHealth                 = 8.0f;
        public const float      DelayRechargeShield                 = 5.0f;
        public const float      DelayRechargeStamina                = 4.0f;
        public const float      DoorCleanupTimer                    = 4.0f;
        public const string     ErrorInvalidParam                   = "Error: Invalid option supploed for parameter: ";
        public const string     ErrorMissingParam                   = "Error: Missing argument for parameter: ";
        public const string     ErrorInvalidObject                  = "Error: Invalid command for selected object";
        public const string     ErrorMissingObject                  = "Error: No object selected for command";
        public const float      GUIRefreshRate                      = 0.5f;
        public const float      HUDFadeInDuration                   = 1f;
        public const float      HUDFadeOutDuration                  = 3f;
        public const float      HUDMotionScale                      = 0.8f;
        public const float      HUDMotionSpeedMovement              = 2f;
        public const float      HUDMotionSpeedRevert                = 4f;
        public const float      HUDTimeUntilDeactivation            = 5f;
        public const float      MenuControlsFontSizeChar            = 9f;
        public const float      MenuControlsFontSizeMiniWord        = 5.5f;
        public const float      MenuControlsFontSizeWord            = 4f;
        public const int        MaxUpgradeLevel                     = 5;
        public const float      MinimumPlayerInteractionDistance    = 3.0f;
        public const int        SceneCountSL2                       = 7;
        public const string     ScenePrefixSL2                      = "SL2";
        public const float      SliderTransitionLength              = 0.25f;
        public const float      SliderLengthBetweenUpdates          = 0.5f;
        public const float      StaminaDepletionRate                = 10.0f;

        public static Color             CrosshairEntity()
        {
            return new Color32(164, 28, 255, 255);
        }

        public static Color             CrosshairInterface()
        {
            return Color.yellow;
        }

        public static Color             CrosshairNormal()
        {
            return Color.gray;
        }

        public static AnimationCurve    CurveHUDFade()
        {
            return new AnimationCurve(new Keyframe(0, 0, 0, 5), new Keyframe(1, 1));
        }

        public static string InputNameToChar(string name)
        {
            switch (name)
            {
                case "backquote":
                    return "`";
                case "leftBracket":
                    return "[";
                case "rightBracket":
                    return "]";
                case "minus":
                    return "-";
                case "plus":
                    return "+";
                case "equals":
                    return "=";
                case "semicolon":
                    return ";";
                case "quote":
                    return "'";
                case "backslash":
                    return "\\";
                case "comma":
                    return ",";
                case "period":
                    return ".";
                case "slash":
                    return "/";
                case "leftCtrl":
                    return "L CTRL";
                case "rightCtrl":
                    return "R CTRL";
                case "leftAlt":
                    return "L ALT";
                case "rightAlt":
                    return "R ALT";
                case "insert":
                    return "INS";
                case "delete":
                    return "DEL";
                case "home":
                    return "HOME";
                case "end":
                    return "END";
                case "pageUp":
                    return "PgUp";
                case "pageDown":
                    return "PgDn";
                case "escape":
                    return "ESC";
                case "numpadMultiply":
                    return "NUM*";
                case "numpadDivide":
                    return "NUM/";
                case "numpadPlus":
                    return "NUM+";
                case "numpadMinus":
                    return "NUM-";
                case "numpadEquals":
                    return "NUM=";
                case "numpadPeriod":
                    return "NUM.";
                case "numpadEnter":
                    return "NUM\nEnter";
                case "numpad1":
                    return "NUM1";
                case "numpad2":
                    return "NUM2";
                case "numpad3":
                    return "NUM3";
                case "numpad4":
                    return "NUM4";
                case "numpad5":
                    return "NUM5";
                case "numpad6":
                    return "NUM6";
                case "numpad7":
                    return "NUM7";
                case "numpad8":
                    return "NUM8";
                case "numpad9":
                    return "NUM9";
                case "numpad0":
                    return "NUM0";
                case "contextMenu":
                    return "Context\nMenu";
                default:
                    return name;
            }
        }

        public static float             MaxArmor(int level)
        {
            switch(level)
            {
                case 5:
                    return 500f;
                case 4:
                    return 250f;
                case 3:
                    return 100f;
                case 2:
                    return 50f;
                case 1:
                default:
                    return 25f;
            }
        }

        public static float             MaxHealth(int level)
        {
            switch (level)
            {
                case 5:
                    return 1500f;
                case 4:
                    return 1250f;
                case 3:
                    return 1000f;
                case 2:
                    return 750f;
                case 1:
                default:
                    return 500f;
            }
        }

        public static float             MaxShield(int level)
        {
            switch (level)
            {
                case 5:
                    return 300f;
                case 4:
                    return 250f;
                case 3:
                    return 200f;
                case 2:
                    return 150f;
                case 1:
                default:
                    return 100f;
            }
        }

        public static float             MaxStamina(int level)
        {
            switch (level)
            {
                case 5:
                    return 200f;
                case 4:
                    return 175f;
                case 3:
                    return 150f;
                case 2:
                    return 125f;
                case 1:
                default:
                    return 100f;
            }
        }

        public static Color             MenuButtonTextActive()
        {
            return new Color32(66, 62, 61, 255);
        }

        public static Color             MenuButtonTextInactive()
        {
            return Color.white;
        }

        public static Vector3           MenuMinimizedPosition()
        {
            return new Vector3(0, 95);
        }

        public static int               ObjectiveExperienceValue(Objective o)
        {
            switch(o)
            {
                case Objective.EmergencyPower:
                    return 1000;
                default:
                    return 0;
            }
        }

        public static float             RechargeRateArmor(int level)
        {
            switch(level)
            {
                case 5:
                    return 0.75f;
                case 4:
                    return 0.65f;
                case 3:
                    return 0.5f;
                case 2:
                    return 0.4f;
                case 1:
                default:
                    return 0.33f;
            }
        }

        public static float             RechargeRateHealth(int level)
        {
            switch(level)
            {
                case 5:
                    return 0.25f;
                case 4:
                    return 0.2f;
                case 3:
                    return 0.15f;
                case 2:
                    return 0.1f;
                case 1:
                default:
                    return 0.05f;
            }
        }

        public static float             RechargeRateShield(int level)
        {
            switch(level)
            {
                case 5:
                    return 2.5f;
                case 4:
                    return 2f;
                case 3:
                    return 1.5f;
                case 2:
                    return 1f;
                case 1:
                default:
                    return 0.5f;
            }
        }

        public static float             RechargeRateStamina(int level)
        {
            switch(level)
            {
                case 5:
                    return 5f;
                case 4:
                    return 3.5f;
                case 3:
                    return 2.5f;
                case 2:
                    return 2f;
                case 1:
                    return 1.5f;
                default:
                    return 1f;
            }
        }

        public static int               RepairExperienceValue(RepairType r)
        {
            switch (r)
            {
                case RepairType.Default:
                default:
                    return 100;
            }
        }

        public static int               UpgradeCost(UpgradeElement elem, int level)
        {
            switch (elem)
            {
                case UpgradeElement.PlayerHealthAmount:
                default:
                    switch (level)
                    {
                        case 5:
                            return 500;
                        case 4:
                            return 350;
                        case 3:
                            return 200;
                        case 2:
                            return 100;
                        case 1:
                        default:
                            return 0;
                    }
            }
        }
    }
}
