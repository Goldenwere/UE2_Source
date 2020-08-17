using UnityEngine;
using Entity2.Core;
using Entity2.Data;

namespace Entity2.UI
{
    /// <summary>
    /// An interactable object that completes objectives
    /// </summary>
    public class ObjectiveBasedObject : InteractableObject
    {
        [SerializeField]    private Objective[] objectivesSatisfies;
        /**************/    private bool        objectivesSatisfied;

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
                foreach (Objective o in objectivesSatisfies)
                {
                    GameEvents.Instance.CompleteObjective(o);
                    PlayerUpgradeData.Instance.PlayerExp += GameConstants.ObjectiveExperienceValue(o);
                }
                objectivesSatisfied = true;
                Complete(true);
            }

            else
                Complete(false);
        }
    }
}