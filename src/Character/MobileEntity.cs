#pragma warning disable 0649

using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Entity2.Core;
using Goldenwere.Unity;

namespace Entity2.Character
{
    /// <summary>
    /// Represents enemies that use NavMeshAgents
    /// </summary>
    public class MobileEntity : Entity
    {
        #region Fields
        [SerializeField]    private MobileEntityType    entityType;
        [SerializeField]    private NavMeshAgent        entityNavAgent;
        [SerializeField]    private Animator            entityAnimator;
        [SerializeField]    private Transform           selfHeightOffset;
        /**************/    private float               alertOthersDelay = 10f;
        /**************/    private int                 chanceForWander;
        /**************/    protected const float       lostTrackDelay = 5f;
        /**************/    private bool                playerWentOutOfRange;
        /**************/    private const float         rangeForAssistance = 50f;
        /**************/    private float               rangeWeapons;
        /**************/    private const float         roamRadius = 15f;
        /**************/    private float               timerAlertedOthers;
        /**************/    private float               timerAwareness;
        /**************/    private float               timerLostTrack;

        public override event CallWeaponFireDelegate    CallWeaponFire;
        public override event CallWeaponSwitchDelegate  CallWeaponSwitch;
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
            rangeKeepDistance = p.RangeKD;
            rangeWeapons = p.RangeWP;
            playerWentOutOfRange = true;
            timerAlertedOthers = alertOthersDelay;
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
            timerAlertedOthers += Time.deltaTime;

            if (player != null)
            {
                if (playerIChar.CurrentRoom == CurrentRoom && entityNavAgent.velocity.sqrMagnitude >= 0.01f)
                    reflectionProbe.RenderProbe();

                distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
                if (distanceToPlayer <= rangePartialAware)
                {
                    timerLostTrack = 0;
                    timerAwareness += Time.deltaTime;

                    // Fire at the player
                    if (distanceToPlayer <= rangeFullAware)
                    {
                        entityAnimator.SetBool("isHostile", true);
                        entityNavAgent.speed = speedHostile;
                        if (playerWentOutOfRange)
                            playerWentOutOfRange = false;

                        // Move if haven't moved for a bit
                        if (timerAwareness >= delayReaction * distanceToPlayer)
                            UpdateDestination();

                        // Get initial look rotation
                        Vector3 dir = (playerHeightAdjust.transform.position - selfHeightOffset.position).normalized;
                        Quaternion lookRot = Quaternion.LookRotation(dir);

                        // Face the player by rotating along y-axis
                        Quaternion lookRotYAxis = Quaternion.Euler(Vector3.Scale(lookRot.eulerAngles, Vector3.up));
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotYAxis, Time.deltaTime * entityNavAgent.angularSpeed);

                        // If vertically unlocked, rotate height-offset (which contains body) along x-axis
                        if (verticallyUnlocked)
                        {
                            Quaternion lookRotXAxis = Quaternion.Euler(Vector3.Scale(lookRot.eulerAngles, Vector3.right));
                            selfHeightOffset.localRotation = Quaternion.Slerp(selfHeightOffset.localRotation, lookRotXAxis, Time.deltaTime * entityNavAgent.angularSpeed);
                        }

                        Physics.Raycast(selfHeightOffset.position, selfHeightOffset.forward, out RaycastHit hit, 999f);

                        if (hit.collider != null && hit.collider.CompareTag("Player"))
                            timerLineOfSightLoss = 0;

                        if (distanceToPlayer < rangeWeapons)
                            if (timerLineOfSightLoss <= delayLineOfSightLoss || !verticallyUnlocked)
                                CallWeaponFire?.Invoke();
                    }

                    // Look for the player
                    else
                    {
                        entityNavAgent.speed = speedIdle;
                        if (!playerWentOutOfRange)
                        {
                            entityAnimator.SetBool("isMoving", true);
                            entityAnimator.SetBool("isHostile", false);
                            playerWentOutOfRange = true;
                            UpdateDestination();
                        }

                        else if (timerAwareness > delayReaction * distanceToPlayer)
                        {
                            entityAnimator.SetBool("isMoving", true);
                            UpdateDestination();
                        }
                    }
                }

                else
                {
                    // Try to look for the player one last time
                    if (timerLostTrack < lostTrackDelay)
                    {
                        timerLostTrack += Time.deltaTime;
                        if (timerAwareness > delayReaction * distanceToPlayer)
                        {
                            entityAnimator.SetBool("isMoving", true);
                            UpdateDestination();
                        }
                    }

                    // Idle
                    else
                        TryWander();
                }
            }

            // Default to wander when no player in scene
            else if (timerIdle >= delayIdle)
                TryWander();

            if (entityNavAgent.velocity.sqrMagnitude <= 0.01)
                entityAnimator.SetBool("isMoving", false);
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
                else
                {
                    entityNavAgent.speed = speedHostile;
                    timerAwareness = 0;
                    timerLostTrack = 0;
                    UpdateDestination();
                    if (timerAlertedOthers > alertOthersDelay)
                    {
                        MobileEntity[] others = FindObjectsOfType<MobileEntity>();
                        foreach (MobileEntity other in others)
                            if (other != this && Vector3.Distance(transform.position, other.transform.position) <= rangeForAssistance)
                                other.Alert(transform.position);
                    }
                }
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
        /// Receive alerts from other entities being damaged
        /// </summary>
        /// <param name="position">The position to go to</param>
        public void Alert(Vector3 position)
        {
            // Only go if completely oblivious to start
            if (timerLostTrack > 0)
            {
                entityNavAgent.speed = speedHostile;
                timerLostTrack = -(rangeForAssistance - lostTrackDelay) / entityNavAgent.speed;
                timerAwareness = -(rangeForAssistance - lostTrackDelay) / entityNavAgent.speed;
                timerIdle = -(rangeForAssistance - lostTrackDelay) / entityNavAgent.speed;
                NavMesh.SamplePosition(position, out NavMeshHit hit, rangeKeepDistance, 1);
                entityNavAgent.SetDestination(hit.position);
            }
        }

        /// <summary>
        /// MobileEntity's version of idling (when the player is not nearby) is wandering
        /// </summary>
        protected override void Idle()
        {
            Vector3 randomDir = Random.insideUnitSphere * roamRadius;
            Vector3 startPos = transform.position;
            randomDir += startPos;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDir, out hit, roamRadius, 1);
            Vector3 finalPos = hit.position;
            entityNavAgent.destination = finalPos;
        }

        /// <summary>
        /// Handle entity death
        /// </summary>
        protected override void OnDeath()
        {
            Instantiate(onDeathParticles, transform.position, transform.rotation);
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
        }

        /// <summary>
        /// When the entity enters a room, reference it in CurrentRoom
        /// </summary>
        /// <param name="other">The trigger collider entered</param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Room"))
                CurrentRoom = other.gameObject;
        }

        /// <summary>
        /// Make an attempt at wandering
        /// </summary>
        private void TryWander()
        {
            entityNavAgent.speed = speedIdle;
            entityAnimator.SetBool("isMoving", true);
            entityAnimator.SetBool("isHostile", false);
            chanceForWander = Random.Range(1, 400);
            if (chanceForWander == 1)
            {
                timerIdle = 0;
                Idle();
            }
        }

        /// <summary>
        /// Move entity to a new destination based on player's position
        /// </summary>
        private void UpdateDestination()
        {
            Vector3 direction = Random.insideUnitSphere * rangeKeepDistance;
            direction += player.transform.position;

            NavMesh.SamplePosition(direction, out NavMeshHit hit, rangeKeepDistance, 1);

            if (!playerIChar.CurrentRoom.GetComponent<BoxCollider>().bounds.Contains(hit.position) || Mathf.Abs(hit.position.y - player.transform.position.y) > 1f)
                hit.position = player.transform.position;

            entityNavAgent.SetDestination(hit.position);

            timerAwareness = -Random.Range(1f, lostTrackDelay);
        }
        #endregion
    }
}
