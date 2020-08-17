#pragma warning disable 0649

using System.Collections.Generic;
using UnityEngine;
using Entity2.Core;

namespace Entity2.Character
{
    /// <summary>
    /// Attach this component to all meshes/physical colliders of an ICharacter and associate the parent ICharacter with it; Used for weapon raycasting/area of effect
    /// </summary>
    public class SendDamageUpward : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour iChar;

        /// <summary>
        /// Sends damage to the ICharacter object
        /// </summary>
        /// <param name="amt">The damage that ICharacter is to take</param>
        /// <param name="modifiers">The modifiers that are applied to the base damage amount</param>
        public void SendDamage(float amt, Dictionary<AttackModifier, float> modifiers)
        {
            ICharacter c = (ICharacter)iChar;
            c.TakeDamage(amt, modifiers);
        }

        /// <summary>
        /// Sends damage to the ICharacter object
        /// </summary>
        /// <param name="amt">The damage that ICharacter is to take</param>
        /// <param name="t">The duration of the damage</param>
        /// <param name="effect">The type of damage being sent</param>
        public void SendDamage(float amt, float t, StatusEffect effect)
        {
            ICharacter c = (ICharacter)iChar;
            c.TakeDamage(effect, t, amt);
        }
    }
}