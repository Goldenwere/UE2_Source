using TMPro;
using UnityEngine;
using Entity2.Core;
using Entity2.Data;

namespace Entity2.UI
{
    /// <summary>
    /// Interactable store buttons
    /// </summary>
    public class StoreItem : InteractableObject
    {
        #region Fields
        [SerializeField]    private TextMeshProUGUI interfaceCostText;
        [SerializeField]    private UpgradeElement  upgradeElement;
        #endregion

        #region Methods
        /// <summary>
        /// Setup initial values at Start
        /// </summary>
        protected override void Start()
        {
            interfaceCostText.text = GameConstants.UpgradeCost(upgradeElement, PlayerUpgradeData.Instance.UpgradeLevels[upgradeElement] + 1).ToString();
            base.Start();
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
                if (PlayerUpgradeData.Instance.ValidateUpgrade(upgradeElement))
                {
                    if (PlayerUpgradeData.Instance.UpgradeLevels[upgradeElement] == GameConstants.MaxUpgradeLevel)
                        interfaceCostText.text = "MAX";
                    else
                        interfaceCostText.text = GameConstants.UpgradeCost(upgradeElement, PlayerUpgradeData.Instance.UpgradeLevels[upgradeElement] + 1).ToString();

                    Complete(true);
                }
                else
                    Complete(false);
            }
            else
                Complete(false);
        }
        #endregion
    }
}