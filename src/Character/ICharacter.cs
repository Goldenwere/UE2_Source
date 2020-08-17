using System.Collections.Generic;
using Entity2.Core;
using UnityEngine;

namespace Entity2.Character
{
    public delegate void CallWeaponFireDelegate();
    public delegate void CallWeaponSwitchDelegate();

    /// <summary>
    /// Interface for all characters in the game that can take damage and have certain stats
    /// </summary>
    public interface ICharacter
    {
        #region Properties
        GameObject  CurrentRoom { get; }
        float       CurrArmor   { get; }
        float       CurrHealth  { get; }
        float       CurrShield  { get; }
        bool        IsGodMode   { get; set; }
        float       MaxArmor    { get; }
        float       MaxHealth   { get; }
        float       MaxShield   { get; }

        /// <summary>
        /// Use this event to listen in on when a character wants to fire its weapons
        /// </summary>
        event CallWeaponFireDelegate CallWeaponFire;

        /// <summary>
        /// Use this event to listen in on when a character wants to switch weapons
        /// </summary>
        event CallWeaponSwitchDelegate CallWeaponSwitch;
        #endregion

        #region Methods
        // All characters can take damage - either instantaneous damage or iterative damage

        /// <summary>
        /// Used for taking incoming instantaneous damage from another character
        /// </summary>
        /// <param name="damage">The damage that was done</param>
        /// <param name="modifiers">Any modifiers to apply to the damage</param>
        void TakeDamage(float damage, Dictionary<AttackModifier, float> modifiers);

        /// <summary>
        /// Used for taking incoming iterative damage from another character
        /// </summary>
        /// <param name="effect">The damage's effect type</param>
        /// <param name="time">The time the effect lasts</param>
        /// <param name="intensity">The damage strength of the effect</param>
        void TakeDamage(StatusEffect effect, float time, float intensity);
        #endregion
    }
}