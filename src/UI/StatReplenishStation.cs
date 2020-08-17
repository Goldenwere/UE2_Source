using Entity2.Core;
using Entity2.Character;
using UnityEngine;
using TMPro;

namespace Entity2.UI
{
    /// <summary>
    /// An interactable object that replenishes player stats
    /// </summary>
    public class StatReplenishStation : InteractableObject
    {
        #region Fields
        [SerializeField]    private UIElement       elementReplenishes;
        [SerializeField]    private float           replenishAmount;
        [SerializeField]    private TextMeshProUGUI statusIndicator;
        /**************/    private bool            textNeedsUpdated;
        /**************/    private PlayerStats     player;
        #endregion

        #region Methods
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
        /// Update cooldown if needed
        /// </summary>
        protected override void Update()
        {
            if (textNeedsUpdated && !actionWasInteractedWith)
            {
                textNeedsUpdated = false;
                statusIndicator.text = "Ready";
            }

            base.Update();
        }

        /// <summary>
        /// When the player is spawned, reference its PlayerStats
        /// </summary>
        /// <param name="obj">The player that was spawned</param>
        private void OnPlayerSpawned(GameObject obj)
        {
            player = obj.GetComponent<PlayerStats>();
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
                if (player != null)
                    player.Heal(elementReplenishes, replenishAmount);

                statusIndicator.text = "Not ready...";
                textNeedsUpdated = true;

                Complete(true);
            }

            else
                Complete(false);
        }
        #endregion
    }
}
