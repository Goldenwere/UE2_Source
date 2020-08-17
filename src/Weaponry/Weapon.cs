#pragma warning disable 0649

using System.Collections.Generic;
using UnityEngine;
using Entity2.Character;
using Entity2.Core;

namespace Entity2.Weaponry
{
    /// <summary>
    /// Represents a weapon of a defined weaponType and keeps track of its firing
    /// </summary>
    public class Weapon : MonoBehaviour
    {
        #region Fields & Properties
        [SerializeField]    private GameObject                          outParticleSystem;
        [SerializeField]    private AudioSource                         outSoundEffect;
        [SerializeField]    private Animator                            weaponAnimator;
        [SerializeField]    private WeaponType                          weaponType;
        /**************/    private Transform[]                         outSource;
        /**************/    private int                                 outSourceIndex;
        /**************/    private Dictionary<AttackModifier, float>   weaponModifiers;
        /**************/    private float                               weaponRateTimer;
        /**************/    private WeaponProperties                    weaponProperties;

        public WeaponType   WeaponType      { get { return weaponType; } }
        public Animator     WeaponAnimator  { get { return weaponAnimator; } }
        #endregion

        #region Methods

        /// <summary>
        /// Increase the fire rate timer each frame
        /// </summary>
        private void Update()
        {
            weaponRateTimer += Time.deltaTime;
        }

        /// <summary>
        /// Use this to set up the weapon and define its controller
        /// </summary>
        /// <param name="source">The fire source for raycasting</param>
        /// <param name="isPlayer">Whether the player is controlling the weapon or not</param>
        public void Initialize(Transform[] source, bool isPlayer)
        {
            weaponProperties = WeaponProperties.WeaponDefinitions(weaponType);
            weaponModifiers = new Dictionary<AttackModifier, float>()
            {
                { AttackModifier.Armor,  weaponProperties.ModArmor  },
                { AttackModifier.Health, weaponProperties.ModBio    },
                { AttackModifier.Shield, weaponProperties.ModShield }
            };
            outSource = source;
            if (!isPlayer)
                weaponProperties.IsInfinite = true;
        }

        /// <summary>
        /// Called upon by the WeaponController, this handles the gun's firing mechanism
        /// </summary>
        /// <param name="isPlayer">Whether or not the weapon is being fired from the player</param>
        /// <param name="desiredTargets">The gameobject tags that the weapon can affect</param>
        public void FireWeapon(bool isPlayer, string[] desiredTargets)
        {
            if (GameEvents.Instance.CurrState == GameState.gplay && weaponRateTimer >= weaponProperties.RateOfFire && weaponProperties.CurrAmmo > 0)
            {
                outSourceIndex++;
                if (outSourceIndex >= outSource.Length)
                    outSourceIndex = 0;

                if (!weaponProperties.IsInfinite)
                    weaponProperties.CurrAmmo--;
                if(isPlayer)
                    GameEvents.Instance.UpdateUI(UIElement.HUDAmmo, weaponProperties.CurrAmmo);

                GameObject p = Instantiate(outParticleSystem, outSource[outSourceIndex].transform.position, Quaternion.identity, outSource[outSourceIndex]);
                p.transform.forward = outSource[outSourceIndex].transform.forward;
                p.transform.SetParent(null);
                outSoundEffect.Play();
                if (weaponAnimator != null)
                    weaponAnimator.SetTrigger(weaponType.ToString() + "_Fire");
                weaponRateTimer = 0;

                foreach (string desiredTarget in desiredTargets)
                    if (TestWeaponFireCollision(weaponProperties.IsAreaOfEffect, desiredTarget, out Collider c))
                        DoDamage(c.gameObject.GetComponent<SendDamageUpward>());
            }
        }

        /// <summary>
        /// Attempts to see if weapon fire has hit an object
        /// </summary>
        /// <param name="isAreaOfEffect">Whether to use an area-of-effect test or projectile test (overlap sphere versus raycast)</param>
        /// <param name="desiredTarget">The gameobject tag that the test must match against</param>
        /// <param name="col">The collider that was hit</param>
        /// <returns>Whether the collider matches the desired target</returns>
        private bool TestWeaponFireCollision(bool isAreaOfEffect, string desiredTarget, out Collider col)
        {
            if (isAreaOfEffect)
            {
                Collider[] hit = Physics.OverlapSphere(outSource[outSourceIndex].transform.position, weaponProperties.Range);

                foreach (Collider c in hit)
                {
                    if (c.gameObject.tag == desiredTarget)
                    {
                        col = c;
                        return true;
                    }
                }

                col = null;
                return false;
            }

            else
            {
                Ray ray = new Ray(outSource[outSourceIndex].transform.position, outSource[outSourceIndex].transform.forward);

                if (Physics.Raycast(ray, out RaycastHit hit, weaponProperties.Range) && hit.collider.CompareTag(desiredTarget))
                {
                    col = hit.collider;
                    return true;
                }

                else
                {
                    col = null;
                    return false;
                }
            }
        }

        /// <summary>
        /// Make a specified character take damage
        /// </summary>
        /// <param name="charDmg">The character to damage</param>
        private void DoDamage(SendDamageUpward charDmg)
        {
            if (weaponProperties.IsAreaOfEffect)
                charDmg.SendDamage(weaponProperties.Damage, weaponProperties.EffectDuration, weaponProperties.Effect);
            else
                charDmg.SendDamage(weaponProperties.Damage, weaponModifiers);
        }
        #endregion
    }

    /// <summary>
    /// Describes a weapon weaponType so that weapons can initialize based on weaponType
    /// </summary>
    public enum WeaponType
    {
        [InspectorName("Resurvatarian Standard Pistol")]    R_StdPistol,
        [InspectorName("Entity Turret Gun")]                E_TurretGun,
        [InspectorName("Entity Blaster")]                   E_Blaster,
        [InspectorName("Entity Flamer")]                    E_Flamer,
        [InspectorName("Entity Toxin Sprayer")]             E_ToxinSprayer,
        [InspectorName("Entity Cold Cannon")]               E_ColdCannon,
        [InspectorName("Entity Energy Cannon")]             E_EnergyCannon
    }

    /// <summary>
    /// Fully defines the weaponProperties of the various weapons in the game
    /// </summary>
    public struct WeaponProperties
    {
        public float            RateOfFire;
        public float            Damage;
        public float            Accuracy;
        public float            Range;
        public bool             IsAreaOfEffect;
        public float            CurrAmmo;
        public float            MaxAmmo;
        public bool             IsInfinite;
        public float            ModShield;
        public float            ModArmor;
        public float            ModBio;
        public StatusEffect     Effect;
        public float            EffectDuration;

        /// <summary>
        /// Assists in weapon initialization by setting up a weapon's weaponProperties
        /// <para>This overload is for Stationary entities</para>
        /// </summary>
        /// <param name="t">The weaponProperties of a weapon depend on its weaponType</param>
        /// <returns>A WeaponProperties based off of the pre-defined weaponProperties of weapons</returns>
        public static WeaponProperties WeaponDefinitions(WeaponType t)
        {
            WeaponProperties p = new WeaponProperties();
            switch(t)
            {
                #region Turret Gun
                case WeaponType.E_TurretGun:
                    p.RateOfFire = 0.2f;
                    p.Damage = 5f;
                    p.Accuracy = 0.1f;
                    p.MaxAmmo = 1000;
                    p.Effect = StatusEffect.None;
                    p.IsAreaOfEffect = false;
                    p.Range = 200f;
                    p.ModArmor = 0.25f;
                    break;
                #endregion

                #region Entity Blaster
                case WeaponType.E_Blaster:
                    p.RateOfFire = 0.5f;
                    p.Damage = 5f;
                    p.Accuracy = 0.2f;
                    p.MaxAmmo = 1000;
                    p.Effect = StatusEffect.None;
                    p.IsAreaOfEffect = false;
                    p.Range = 200f;
                    p.ModShield = 0.25f;
                    break;
                #endregion

                #region Cold Cannon
                case WeaponType.E_ColdCannon:
                    p.RateOfFire = 9f;
                    p.EffectDuration = 8f;
                    p.Damage = 3f;
                    p.MaxAmmo = 1000;
                    p.Effect = StatusEffect.Cold;
                    p.IsAreaOfEffect = true;
                    p.Range = 5f;
                    break;
                #endregion

                #region Energy Cannon
                case WeaponType.E_EnergyCannon:
                    p.RateOfFire = 4f;
                    p.EffectDuration = 3f;
                    p.Damage = 8f;
                    p.MaxAmmo = 1000;
                    p.Effect = StatusEffect.Electrical;
                    p.IsAreaOfEffect = true;
                    p.Range = 5f;
                    break;
                #endregion

                #region Flamer
                case WeaponType.E_Flamer:
                    p.RateOfFire = 7f;
                    p.EffectDuration = 6f;
                    p.Damage = 4f;
                    p.MaxAmmo = 1000;
                    p.Effect = StatusEffect.Heat;
                    p.IsAreaOfEffect = true;
                    p.Range = 5f;
                    break;
                #endregion

                #region Toxin Sprayer
                case WeaponType.E_ToxinSprayer:
                    p.RateOfFire = 5f;
                    p.EffectDuration = 4f;
                    p.Damage = 6f;
                    p.MaxAmmo = 1000;
                    p.Effect = StatusEffect.Toxin;
                    p.IsAreaOfEffect = true;
                    p.Range = 5f;
                    break;
                #endregion

                #region Standard Pistol
                case WeaponType.R_StdPistol:
                default:
                    p.RateOfFire = 0.5f;
                    p.Damage = 25f;
                    p.Accuracy = 0.1f;
                    p.MaxAmmo = 1000;
                    p.Effect = StatusEffect.None;
                    p.IsAreaOfEffect = false;
                    p.Range = 200f;
                    break;
                #endregion
            }

            p.CurrAmmo = p.MaxAmmo;
            return p;
        }
    }
}
