using Entity2.Core;
using Entity2.Character;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entity2.UI
{
    /// <summary>
    /// Enables world-based UI interaction
    /// </summary>
    public class PlayerTargetting : MonoBehaviour
    {
        #region Fields
        [SerializeField]    private Transform   raycastSource;
        /**************/    private bool        inputInteractIsPressed;
        /**************/    private bool        prevFrameWasEntity;
        /**************/    private bool        prevFrameWasTargetted;
        #endregion

        #region Methods
        /// <summary>
        /// Update is called once per frame; fires a ray and determines what interactions are available or what the target crosshair should look like
        /// </summary>
        private void Update()
        {
            RaycastHit hit = new RaycastHit();
            Ray ray = new Ray(raycastSource.position, raycastSource.forward);
            if (Physics.Raycast(ray, out hit, 200f))
            {
                Collider c = hit.collider;
                if (c.tag == "Entity")
                {
                    GameEvents.Instance.UpdateUI(UIElement.HUDCrosshair, CrosshairTarget.entity);
                    GameEvents.Instance.UpdateUI(UIElement.HUDEntityHealth, c.GetComponentInParent<ICharacter>().CurrHealth);
                    if (!prevFrameWasEntity)
                    {
                        GameEvents.Instance.UpdateUI(UIElement.HUDEntityMaxHealth, c.GetComponentInParent<ICharacter>().MaxHealth);
                        prevFrameWasEntity = true;
                    }
                }

                else if (c.tag == "WorldSpaceUI" && hit.distance < GameConstants.MinimumPlayerInteractionDistance)
                {
                    GameEvents.Instance.UpdateUI(UIElement.HUDCrosshair, CrosshairTarget.ui);
                    if (inputInteractIsPressed)
                        c.gameObject.GetComponent<InteractableObject>().OnInteract();
                    if (!prevFrameWasTargetted)
                        prevFrameWasTargetted = true;
                }

                else if (prevFrameWasEntity)
                {
                    GameEvents.Instance.UpdateUI(UIElement.HUDEntityHealth, UIState.disabled, 0);
                    prevFrameWasEntity = false;
                    GameEvents.Instance.UpdateUI(UIElement.HUDCrosshair, CrosshairTarget.none);
                }

                else if (prevFrameWasTargetted)
                {
                    GameEvents.Instance.UpdateUI(UIElement.HUDCrosshair, CrosshairTarget.none);
                    prevFrameWasTargetted = false;
                }
            }
        }

        /// <summary>
        /// Handler for the Interact input event, update the inputInteractIsPressed variable
        /// </summary>
        /// <param name="context">The context associated with the input</param>
        public void OnInteract(InputAction.CallbackContext context)
        {
            inputInteractIsPressed = context.performed;
        }
        #endregion
    }
}
