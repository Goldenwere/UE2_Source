using UnityEngine;
using System.Collections.Generic;
using Entity2.Core;

namespace Entity2.Character
{
    /// <summary>
    /// The entity class is the base class that all entity enemies inherit from
    /// </summary>
    public abstract class Entity : MonoBehaviour, ICharacter
    {
        #region Fields
        [SerializeField]    protected GameObject        onDeathParticles;
        [SerializeField]    protected ReflectionProbe   reflectionProbe;
        /**************/    protected const float       delayReaction = 0.05f;
        /**************/    protected const float       delayIdle = 5f;
        /**************/    protected const float       delayLineOfSightLoss = 2f;
        /**************/    protected float             distanceToPlayer;
        /**************/    protected GameObject        player;
        /**************/    protected ICharacter        playerIChar;
        /**************/    protected GameObject        playerHeightAdjust;
        /**************/    protected float             rangeFullAware;
        /**************/    protected float             rangePartialAware;
        /**************/    protected float             rangeKeepDistance;
        /**************/    protected float             speedIdle;
        /**************/    protected float             speedHostile;
        /**************/    protected float             timerIdle;
        /**************/    protected float             timerLineOfSightLoss;
        /**************/    protected bool              verticallyUnlocked;
        #endregion

        #region Properties
        public GameObject   CurrentRoom     { get; protected set; }
        public float        CurrArmor       { get; protected set; }
        public float        CurrHealth      { get; protected set; }
        public float        CurrShield      { get; protected set; }
        public bool         IsGodMode       { get; set; }
        public float        MaxArmor        { get; protected set; }
        public float        MaxHealth       { get; protected set; }
        public float        MaxShield       { get; protected set; }
        #endregion

        #region Events and Methods
        public abstract event CallWeaponFireDelegate    CallWeaponFire;
        public abstract event CallWeaponSwitchDelegate  CallWeaponSwitch;

        public abstract void    TakeDamage(float damage, Dictionary<AttackModifier, float> modifiers);
        public abstract void    TakeDamage(StatusEffect effect, float time, float intensity);

        /// <summary>
        /// Gets the room the entity spawned in when ready
        /// </summary>
        protected void GetSpawnedRoom()
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

        protected abstract void Idle();
        protected abstract void OnDeath();
        protected abstract void OnPlayerSpawned(GameObject obj);
        #endregion
    }

    /// <summary>
    /// Defines entities that do not use NavMeshAgents
    /// </summary>
    public enum StationaryEntityType
    {
        /// <summary>
        /// Rapid Fire Turret
        /// </summary>
        RFT
    }

    /// <summary>
    /// Defines entities that use NavMeshAgents
    /// </summary>
    public enum MobileEntityType
    {
        /// <summary>
        /// Prismatic Hovering Entity
        /// </summary>
        PHE,

        /// <summary>
        /// Spherical Cold Charge Entity
        /// </summary>
        SCCE,

        /// <summary>
        /// Rotating Electrical Rings
        /// </summary>
        RER,

        /// <summary>
        /// Organic Death Entity
        /// </summary>
        ODE,

        /// <summary>
        /// Fire Field Ring
        /// </summary>
        FFR
    }

    /// <summary>
    /// Fully defines the properties of the various entities in the game
    /// </summary>
    public struct EntityProperties
    {
        public float    Health;
        public float    Shield;
        public float    Armor;
        public float    RangeFA;        // Full aware
        public float    RangePA;        // Partial aware
        public float    RangeKD;        // Keep distance (mobile entities)
        public float    RangeWP;        // Weapons
        public float    SpeedIdle;
        public float    SpeedTrack;
        public bool     VerticallyUnlocked;

        /// <summary>
        /// Assists in entity initialization by setting up an entity's properties
        /// <para>This overload is for Stationary entities</para>
        /// </summary>
        /// <param name="t">The properties of an entity depend on its type</param>
        /// <returns>An EntityProperties based off of the pre-defined properties of entities</returns>
        public static EntityProperties EntityDefinitions(StationaryEntityType t)
        {
            EntityProperties p = new EntityProperties();
            switch(t)
            {
                case StationaryEntityType.RFT:
                    p.Health = 150f;
                    p.Shield = 0;
                    p.Armor = 0;
                    p.RangeFA = 10f;
                    p.RangePA = 15f;
                    p.SpeedIdle = 3f;
                    p.SpeedTrack = 5f;
                    p.VerticallyUnlocked = true;
                    break;
            }
            return p;
        }

        /// <summary>
        /// Assists in entity initialization by setting up an entity's properties
        /// <para>This overload is for Mobile entities</para>
        /// </summary>
        /// <param name="t">The properties of an entity depend on its type</param>
        /// <returns>An EntityProperties based off of the pre-defined properties of entities</returns>
        public static EntityProperties EntityDefinitions(MobileEntityType t)
        {
            EntityProperties p = new EntityProperties();
            switch (t)
            {
                case MobileEntityType.FFR:
                    p.Health = 150f;
                    p.Shield = 0;
                    p.Armor = 0;
                    p.RangeFA = 8f;
                    p.RangePA = 16f;
                    p.SpeedIdle = 2f;
                    p.SpeedTrack = 4f;
                    p.VerticallyUnlocked = false;
                    p.RangeKD = 6f;
                    p.RangeWP = 5f;
                    break;

                case MobileEntityType.ODE:
                    p.Health = 150f;
                    p.Shield = 0;
                    p.Armor = 0;
                    p.RangeFA = 8f;
                    p.RangePA = 16f;
                    p.SpeedIdle = 2f;
                    p.SpeedTrack = 4f;
                    p.VerticallyUnlocked = false;
                    p.RangeKD = 6f;
                    p.RangeWP = 5f;
                    break;

                case MobileEntityType.RER:
                    p.Health = 150f;
                    p.Shield = 0;
                    p.Armor = 0;
                    p.RangeFA = 8f;
                    p.RangePA = 16f;
                    p.SpeedIdle = 2f;
                    p.SpeedTrack = 4f;
                    p.VerticallyUnlocked = false;
                    p.RangeKD = 6f;
                    p.RangeWP = 5f;
                    break;

                case MobileEntityType.SCCE:
                    p.Health = 150f;
                    p.Shield = 0;
                    p.Armor = 0;
                    p.RangeFA = 8f;
                    p.RangePA = 16f;
                    p.SpeedIdle = 2f;
                    p.SpeedTrack = 4f;
                    p.VerticallyUnlocked = false;
                    p.RangeKD = 6f;
                    p.RangeWP = 5f;
                    break;

                case MobileEntityType.PHE:
                    p.Health = 100f;
                    p.Shield = 0;
                    p.Armor = 0;
                    p.RangeFA = 12f;
                    p.RangePA = 16f;
                    p.SpeedIdle = 2.5f;
                    p.SpeedTrack = 5f;
                    p.VerticallyUnlocked = true;
                    p.RangeKD = 6f;
                    p.RangeWP = 200f;
                    break;
            }
            return p;
        }
    }
}
