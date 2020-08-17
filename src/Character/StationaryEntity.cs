#pragma warning disable 0649

using UnityEngine;
using System.Collections.Generic;
using Entity2.Core;
using Goldenwere.Unity;

namespace Entity2.Character
{
    /// <summary>
    /// Represents enemies that stay in one spot
    /// </summary>
    public class StationaryEntity : Entity
    {
        #region Fields
        [SerializeField]    private StationaryEntityType    entityType;
        [SerializeField]    private Vector3                 clampRotation;
        [SerializeField]    private Transform[]             joints;
        /**************/    private const float             delaySeek = 5f;
        /**************/    private float                   timerSeek;
        /**************/    private Vector3                 workingRandomIdleRotation;
        /**************/    private Quaternion              workingHostileRotation;

        public override event CallWeaponFireDelegate        CallWeaponFire;
        public override event CallWeaponSwitchDelegate      CallWeaponSwitch;
        #endregion

        #region Methods
        /// <summary>
        /// Called before the first frame update; used for initializing the entity's properties and finding the player
        /// </summary>
        private void Start()
        {
            EntityProperties p = EntityProperties.EntityDefinitions(entityType);
            MaxHealth = p.Health;
            MaxShield = p.Shield;
            MaxArmor = p.Armor;
            CurrHealth = MaxHealth;
            CurrShield = MaxShield;
            CurrArmor = MaxArmor;
            speedIdle = p.SpeedIdle;
            speedHostile = p.SpeedTrack;
            verticallyUnlocked = p.VerticallyUnlocked;
            rangePartialAware = p.RangePA;
            rangeFullAware = p.RangeFA;
        }

        /// <summary>
        /// Subscribe to the PlayerSpawned event OnEnable
        /// </summary>
        private void OnEnable()
        {
            GameEvents.PlayerSpawned += OnPlayerSpawned;
        }

        /// <summary>
        /// Unsubscribe from the PlayerSpawned event OnDisable
        /// </summary>
        private void OnDisable()
        {
            GameEvents.PlayerSpawned -= OnPlayerSpawned;
        }

        /// <summary>
        /// Called once per frame; used for AI
        /// </summary>
        private void Update()
        {
            timerIdle += Time.deltaTime;
            timerLineOfSightLoss += Time.deltaTime;

            if (player != null)
            {
                distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
                if (distanceToPlayer <= rangePartialAware)
                {
                    timerSeek += Time.deltaTime;

                    // Fire at the player
                    if (distanceToPlayer <= rangeFullAware || playerIChar.CurrentRoom == CurrentRoom)
                    {
                        workingHostileRotation = Quaternion.LookRotation(playerHeightAdjust.transform.position - joints[0].position);
                        joints[0].rotation = Quaternion.Slerp(joints[0].rotation, workingHostileRotation, Time.deltaTime * speedHostile);

                        Physics.Raycast(joints[joints.Length - 1].position, joints[joints.Length - 1].forward, out RaycastHit hit, 100f);
                        if (hit.collider != null && hit.collider.CompareTag("Player"))
                            timerLineOfSightLoss = 0;

                        if (timerLineOfSightLoss <= delayLineOfSightLoss)
                            CallWeaponFire?.Invoke();
                    }

                    // Animate as if trying to find the player
                    else
                    {
                        if (timerSeek >= delaySeek)
                        {
                            workingHostileRotation = Quaternion.LookRotation(playerHeightAdjust.transform.position - joints[0].position);
                            Vector3 euler = workingHostileRotation.eulerAngles;
                            euler.x += Random.Range(-(clampRotation.x / 10f), (clampRotation.x / 10f));
                            euler.y += Random.Range(-(clampRotation.y / 10f), (clampRotation.y / 10f));
                            workingHostileRotation = Quaternion.Euler(euler);
                            timerSeek = Random.Range(-2f, 2f);
                        }

                        joints[0].rotation = Quaternion.Slerp(joints[0].rotation, workingHostileRotation, Time.deltaTime * speedIdle);
                    }
                }

                // Idle-y animate
                else
                {
                    if (timerIdle >= delayIdle)
                        Idle();

                    joints[0].rotation = Quaternion.Slerp(joints[0].rotation, Quaternion.Euler(workingRandomIdleRotation), Time.deltaTime * speedIdle);
                }
            }

            // Idle-y animate
            else
            {
                if (timerIdle >= delayIdle)
                    Idle();

                joints[0].rotation = Quaternion.Slerp(joints[0].rotation, Quaternion.Euler(workingRandomIdleRotation), Time.deltaTime * speedIdle);
            }
        }

        /// <summary>
        /// Used for taking incoming instantaneous damage from another character
        /// </summary>
        /// <param name="damage">The damage that was done</param>
        /// <param name="modifiers">Any modifiers to apply to the damage</param>
        public override void TakeDamage(float damage, Dictionary<AttackModifier, float> modifiers)
        {
            if (!IsGodMode)
            {
                CurrHealth -= damage;
                if (CurrHealth <= 0)
                    OnDeath();
            }
        }

        /// <summary>
        /// Used for taking incoming iterative damage from another character
        /// </summary>
        /// <param name="effect">The damage's effect entityType</param>
        /// <param name="time">The time the effect lasts</param>
        /// <param name="intensity">The damage strength of the effect</param>
        public override void TakeDamage(StatusEffect effect, float time, float intensity)
        {

        }

        /// <summary>
        /// StationaryEntity's version of idling (when the player is not nearby) is looking around
        /// </summary>
        protected override void Idle()
        {
            workingRandomIdleRotation = new Vector3(
                Random.Range(-clampRotation.x, clampRotation.x),
                Random.Range(-clampRotation.y, clampRotation.y),
                Random.Range(-clampRotation.z, clampRotation.z));

            timerIdle = 0;
        }

        /// <summary>
        /// Handle entity death
        /// </summary>
        protected override void OnDeath()
        {
            Instantiate(onDeathParticles, gameObject.transform.position, gameObject.transform.rotation);
            StopAllCoroutines();
            Destroy(gameObject);
        }

        /// <summary>
        /// When the player is spawned, reference its transform
        /// </summary>
        /// <param name="obj">The player that was spawned</param>
        protected override void OnPlayerSpawned(GameObject obj)
        {
            player = obj;
            playerIChar = obj.GetComponent<ICharacter>();
            playerHeightAdjust = player.FindChild("PlayerHeightAdjust");

            GetSpawnedRoom();
            reflectionProbe.RenderProbe();
        }
        #endregion
    }
}
