using UnityEngine;
using Entity2.Core;
using Entity2.Data;
using Goldenwere.Unity;

namespace Entity2.UI
{
    /// <summary>
    /// A type of interface that deals with repairable objects
    /// </summary>
    public class RepairableObject : InteractableObject
    {
        #region Fields
        [SerializeField]    private Canvas      canvas;
        [SerializeField]    private RepairType  repairType;
        /**************/    private Vector3     startPosition;
        /**************/    private Transform   player;
        /**************/    private float       distanceToPlayer;
        #endregion

        #region Methods
        /// <summary>
        /// Setup initial values at Start
        /// </summary>
        protected override void Start()
        {
            startPosition = canvas.transform.position;
            base.Start();
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
        /// Face the player when the player is near
        /// </summary>
        protected override void Update()
        {
            if (player != null)
            {
                distanceToPlayer = Vector3.Distance(player.position, transform.position);

                if (distanceToPlayer < GameConstants.MinimumPlayerInteractionDistance)
                    if (!canvas.enabled)
                        canvas.enabled = true;
                    else
                    {
                        canvas.transform.rotation = Quaternion.Slerp(
                            canvas.transform.rotation,
                            Quaternion.LookRotation(canvas.transform.position - player.position),
                            Time.deltaTime * GameConstants.CanvasRotationSpeed);

                        canvas.transform.position = Vector3.Lerp(
                            canvas.transform.position,
                            Vector3.Lerp(startPosition, player.position, 0.5f),
                            Time.deltaTime * GameConstants.CanvasPositionSpeed);
                    }

                else if (canvas.enabled)
                    canvas.enabled = false;
            }

            base.Update();
        }

        /// <summary>
        /// When the player is spawned, reference its transform
        /// </summary>
        /// <param name="obj">The player that was spawned</param>
        private void OnPlayerSpawned(GameObject obj)
        {
            player = obj.FindChildRecursively("Camera").transform;
        }

        /// <summary>
        /// Needed to ensure this item's Complete is called and not InteractableObject's
        /// </summary>
        public override void OnInteract()
        {
            if (!actionIsInstant)
            {
                actionProgress += Time.deltaTime;
                interfaceProgressSlider.value = actionProgress;
                if (actionProgress > actionTimeLength)
                    Complete();
            }

            else
                Complete();
        }

        /// <summary>
        /// Handle completing of object's action
        /// </summary>
        protected void Complete()
        {
            if (!actionWasInteractedWith)
            {
                PlayerUpgradeData.Instance.PlayerExp += GameConstants.RepairExperienceValue(repairType);
                Complete(true);
            }

            else
                Complete(false);
        }
        #endregion
    }
}
