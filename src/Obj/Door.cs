using UnityEngine;
using Entity2.Core;
using System.Collections.Generic;

namespace Entity2.Obj
{
    /// <summary>
    /// Defines what is required to unlock the door; can be Keycard, or objectives (must exactly match Objective in order to work)
    /// </summary>
    public enum UnlockRequisites
    {
        EmergencyPower,
        Keycard
    }

    /// <summary>
    /// Doors are connections between rooms that can be locked/unlocked depending on certain conditions and open to any tag befitting an ICharacter
    /// </summary>
    public class Door : MonoBehaviour
    {
        #region Fields
        [SerializeField]    private Animator            doorAnimator;
        [SerializeField]    private MeshRenderer        doorLockstateIndicator;
        [SerializeField]    private bool                doorLockState;
        [SerializeField]    private ReflectionProbe     doorProbe;
        [SerializeField]    private AudioSource         doorSound;
        [SerializeField]    private Material            doorUnlockedMaterial;
        [SerializeField]    private int[]               lockMatchingKeycards;
        [SerializeField]    private UnlockRequisites[]  lockUnlockRequisites;
        /**************/    private float               cleanupTimer;
        /**************/    private bool                doorIsOpen;
        /**************/    private int                 doorObjectsEntered;
        /**************/    private List<GameObject>    doorObjectsEnteredCollection;
        /**************/    private bool[]              lockIsConditionStatisfied;
        #endregion

        #region Methods
        /// <summary>
        /// On MonoBehaviour.Awake, instantiate collections
        /// </summary>
        private void Awake()
        {
            lockIsConditionStatisfied = new bool[lockUnlockRequisites.Length];
            doorObjectsEnteredCollection = new List<GameObject>();
            // Offset door cleanup timer to prevent large amounts of door cleanup in a single frame
            cleanupTimer = Random.Range(-1f, 3f);
        }

        /// <summary>
        /// Subscribe to the objectives event on enable
        /// </summary>
        private void OnEnable()
        {
            GameEvents.ObjectiveCompleted += OnObjectiveCompleted;
            GameEvents.GameSceneLoaded += OnGameSceneLoaded;
        }

        /// <summary>
        /// Unsubscribe from the objectives event on disable
        /// </summary>
        private void OnDisable()
        {
            GameEvents.ObjectiveCompleted -= OnObjectiveCompleted;
            GameEvents.GameSceneLoaded -= OnGameSceneLoaded;
        }

        /// <summary>
        /// Perform cleanup on MonoBehaviour.Update as long as timer reaches limit
        /// </summary>
        private void Update()
        {
            cleanupTimer += Time.deltaTime;
            if (cleanupTimer >= GameConstants.DoorCleanupTimer)
            {
                cleanupTimer = 0;
                doorProbe.RenderProbe();
                ValidateEnteredObjects();
                if (doorIsOpen && doorObjectsEntered == 0)
                {
                    doorAnimator.Play("Door_Close");
                    doorSound.Play();
                    doorIsOpen = false;
                }
            }
        }

        /// <summary>
        /// When a character walks within the door zone, it should open, as long as the door is unlocked
        /// </summary>
        /// <param name="other">The object that triggered this method</param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Entity" || other.tag == "Player")
            {
                doorObjectsEntered++;
                doorObjectsEnteredCollection.Add(other.gameObject);
                ValidateEnteredObjects();
                if (!doorIsOpen && !doorLockState)
                {
                    doorAnimator.Play("Door_Open");
                    doorSound.Play();
                    doorIsOpen = true;
                }
                doorProbe.RenderProbe();
                cleanupTimer = 0;
            }
        }

        /// <summary>
        /// When a character exits the door zone, it should close, as long as there aren't other characters in the door zone
        /// </summary>
        /// <param name="other">The object that triggered this method</param>
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Entity" || other.tag == "Player")
            {
                doorObjectsEntered--;
                doorObjectsEnteredCollection.Remove(other.gameObject);
                ValidateEnteredObjects();
                if (doorIsOpen && doorObjectsEntered == 0)
                {
                    doorAnimator.Play("Door_Close");
                    doorSound.Play();
                    doorIsOpen = false;
                }
                doorProbe.RenderProbe();
                cleanupTimer = 0;
            }
        }

        /// <summary>
        /// When the scene loads, render the reflection probe
        /// </summary>
        private void OnGameSceneLoaded()
        {
            doorProbe.RenderProbe();
        }

        /// <summary>
        /// Handle objective completions that may satisfy prerequisites to unlock the door
        /// </summary>
        /// <param name="obj">The objective that was completed</param>
        private void OnObjectiveCompleted(Objective obj)
        {
            if (doorLockState && lockUnlockRequisites.Length > 0)
            {
                for (int i = 0; i < lockUnlockRequisites.Length; i++)
                    if (!lockIsConditionStatisfied[i] && lockUnlockRequisites[i].ToString() == obj.ToString())
                        lockIsConditionStatisfied[i] = true;

                int satisfied = 0;
                foreach (bool condition in lockIsConditionStatisfied)
                    if (condition)
                        satisfied++;

                if (satisfied >= lockIsConditionStatisfied.Length)
                {
                    doorLockState = false;
                    doorLockstateIndicator.material = doorUnlockedMaterial;
                }
            }
        }

        /// <summary>
        /// Use this to bypass issue where OnTriggerExit is not called on destruction of a gameobject
        /// </summary>
        private void ValidateEnteredObjects()
        {
            List<GameObject> copy = new List<GameObject>(doorObjectsEnteredCollection);
            foreach (GameObject go in copy)
            {
                if (go == null)
                {
                    doorObjectsEnteredCollection.Remove(go);
                    doorObjectsEntered--;
                }
            }
        }
        #endregion
    }
}