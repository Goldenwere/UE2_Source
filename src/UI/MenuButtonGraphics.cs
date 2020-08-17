using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Entity2.Core;

namespace Entity2.UI
{
    /// <summary>
    /// Changes text colors depending on different states of button
    /// </summary>
    public class MenuButtonGraphics : MonoBehaviour
    {
        #region Fields
        [SerializeField]    private TextMeshProUGUI attachedButtonText;
        /**************/    private bool isHover;
        /**************/    private bool isSelected;
        /**************/    private bool isActive;
        #endregion

        #region Methods
        /// <summary>
        /// On the Deselect event, invert text if mouse is also no longer hovering over button
        /// </summary>
        public void OnDeselect()
        {
            isSelected = false;
            if (!isHover)
                ToggleActiveOnText();
        }

        /// <summary>
        /// On the PointerEnter event, invert text if not already active
        /// </summary>
        public void OnPointerEnter()
        {
            isHover = true;
            if (!isActive)
                ToggleActiveOnText();
        }

        /// <summary>
        /// On the PointerExit event, invert text if button is also no longer selected
        /// </summary>
        public void OnPointerExit()
        {
            isHover = false;
            if (!isSelected)
                ToggleActiveOnText();
        }

        /// <summary>
        /// On the Select event, invert text if not already active
        /// </summary>
        public void OnSelect()
        {
            isSelected = true;
            if (!isActive)
                ToggleActiveOnText();
        }

        /// <summary>
        /// When the button has been clicked, reset the text
        /// </summary>
        public void ResetButton()
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(WaitToReset());
            else
            {
                isHover = false;
                isSelected = false;
                isActive = false;
                attachedButtonText.color = GameConstants.MenuButtonTextInactive();
            }

        }

        /// <summary>
        /// Inverts text color on attached button text
        /// </summary>
        private void ToggleActiveOnText()
        {
            isActive = !isActive;

            if (isActive)
                attachedButtonText.color = GameConstants.MenuButtonTextActive();
            else
                attachedButtonText.color = GameConstants.MenuButtonTextInactive();
        }

        /// <summary>
        /// Adds a pause before resetting a button
        /// </summary>
        private IEnumerator WaitToReset()
        {
            yield return new WaitForSeconds(GameConstants.CanvasMinimizeSpeed - Time.fixedDeltaTime);
            isHover = false;
            isSelected = false;
            isActive = false;
            attachedButtonText.color = GameConstants.MenuButtonTextInactive();
            yield return null;
        }
        #endregion
    }
}