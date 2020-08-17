namespace Entity2.Core
{
    /// <summary>
    /// Modifiers to player/entity damage
    /// </summary>
    public enum AttackModifier
    {
        Shield,
        Armor,
        Health,
        None
    }

    /// <summary>
    /// Describes what the player is targetting
    /// </summary>
    public enum CrosshairTarget
    {
        none,
        entity,
        ui
    }

    /// <summary>
    /// Used for defining and determining the current state of the game
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// Game menus (main, etc.); Used along with MenuState
        /// </summary>
        menus,

        /// <summary>
        /// Pause menu in-game
        /// </summary>
        pause,

        /// <summary>
        /// Death menu in-game
        /// </summary>
        death,

        /// <summary>
        /// Console window open in-game
        /// </summary>
        consl,

        /// <summary>
        /// In-game without any menus open
        /// </summary>
        gplay
    }

    /// <summary>
    /// Defines main game objectives/quests
    /// </summary>
    public enum Objective
    {
        EmergencyPower
    }

    /// <summary>
    /// Defines categories of things that can be repaired
    /// </summary>
    public enum RepairType
    {
        Default
    }

    /// <summary>
    /// Player afflictions that do damage over time
    /// </summary>
    public enum StatusEffect
    {
        Electrical,
        Heat,
        Toxin,
        Cold,
        None
    }

    /// <summary>
    /// Gameobject(s) that describe a specific part of the UI
    /// </summary>
    public enum UIElement
    {
        HUDHealth,
        HUDMaxHealth,
        HUDArmor,
        HUDMaxArmor,
        HUDShield,
        HUDMaxShield,
        HUDStamina,
        HUDMaxStamina,
        HUDStatusElectrical,
        HUDStatusHeat,
        HUDStatusToxin,
        HUDStatusCold,
        HUDAmmo,
        HUDEntityHealth,
        HUDEntityMaxHealth,
        HUDCrosshair,
        HUDExperience,
        GUIFramerate
    }

    /// <summary>
    /// The state of a UI element
    /// </summary>
    public enum UIState
    {
        enabled,
        disabled
    }

    /// <summary>
    /// Defines aspects that the player can upgrade
    /// </summary>
    public enum UpgradeElement
    {
        PlayerArmorAmount,
        PlayerHealthAmount,
        PlayerShieldAmount,
        PlayerStaminaAmount,

        PlayerArmorRecharge,
        PlayerHealthRecharge,
        PlayerShieldRecharge,
        PlayerStaminaRecharge
    }
}