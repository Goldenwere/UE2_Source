using UnityEngine;
using Entity2.Core;
using System.Collections.Generic;

namespace Entity2.Data
{
    /// <summary>
    /// Stores player upgrade information that can be used by certain classes
    /// </summary>
    public class PlayerUpgradeData : MonoBehaviour
    {
        #region Fields & Properties
        private int playerExp;

        public static PlayerUpgradeData                 Instance        { get; private set; }
        public        Dictionary<UpgradeElement, int>   UpgradeLevels   { get; private set; }

        /// <summary>
        /// The player's experience value
        /// </summary>
        public int PlayerExp
        {
            get { return playerExp; }
            set
            {
                // Cannot have negative exp
                if (value >= 0)
                    playerExp = value;
                GameEvents.Instance.UpdateUI(UIElement.HUDExperience, playerExp);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Set the GameState at Start
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
                if (gameObject.transform.parent == null)
                    DontDestroyOnLoad(this);
            }

            playerExp = 1000;

            UpgradeLevels = new Dictionary<UpgradeElement, int>()
            {
                { UpgradeElement.PlayerArmorAmount,     1 },
                { UpgradeElement.PlayerArmorRecharge,   1 },
                { UpgradeElement.PlayerHealthAmount,    1 },
                { UpgradeElement.PlayerHealthRecharge,  1 },
                { UpgradeElement.PlayerShieldAmount,    1 },
                { UpgradeElement.PlayerShieldRecharge,  1 },
                { UpgradeElement.PlayerStaminaAmount,   1 },
                { UpgradeElement.PlayerStaminaRecharge, 1 }
            };
        }

        /// <summary>
        /// Validates a desired upgrade (can be invalid if experience is not high enough or if the max level is reached)
        /// </summary>
        /// <param name="elem">The aspect to be upgraded</param>
        /// <returns>Whether the upgrade is valid or not</returns>
        public bool ValidateUpgrade(UpgradeElement elem)
        {
            if (UpgradeLevels[elem] < GameConstants.MaxUpgradeLevel)
            {
                int potentialCost = GameConstants.UpgradeCost(elem, UpgradeLevels[elem] + 1);
                if (playerExp >= potentialCost)
                {
                    UpgradeLevels[elem]++;
                    playerExp -= potentialCost;
                    GameEvents.Instance.SendUpgrade(elem, UpgradeLevels[elem]);
                    return true;
                }

                else
                    return false;
            }

            else
                return false;
        }
        #endregion
    }
}