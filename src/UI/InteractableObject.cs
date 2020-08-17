using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Entity2.UI
{
    /// <summary>
    /// Listens to player targetting and handles interactivity accordingly
    /// </summary>
    public class InteractableObject : MonoBehaviour
    {
        #region Fields
        [SerializeField]    protected float             actionCooldownLength;
        [SerializeField]    protected bool              actionIsInstant;
        [SerializeField]    protected bool              actionIsOneTime;
        [SerializeField]    protected float             actionTimeLength;
        [SerializeField]    protected AudioSource       interfaceAudioFailure;
        [SerializeField]    protected AudioSource       interfaceAudioSuccess;
        [SerializeField]    protected TextMeshProUGUI   interfaceIsOneTimeNotif;
        [SerializeField]    protected Slider            interfaceProgressSlider;
        /**************/    protected float             actionCooldownTimer;
        /**************/    protected bool              actionWasInteractedWith;
        /**************/    protected float             actionProgress;
        #endregion

        #region Methods
        /// <summary>
        /// Ensure things are properly set at Start
        /// </summary>
        protected virtual void Start()
        {
            actionProgress = 0;
            if (!actionIsInstant)
            {
                interfaceProgressSlider.maxValue = actionTimeLength;
                interfaceProgressSlider.minValue = 0;
                interfaceProgressSlider.value = 0;
            }
        }

        /// <summary>
        /// Update actionCooldownTimer if needed
        /// </summary>
        protected virtual void Update()
        {
            if (actionWasInteractedWith && !actionIsOneTime)
            {
                actionCooldownTimer += Time.deltaTime;
                if (actionCooldownTimer > actionCooldownLength)
                {
                    actionCooldownTimer = 0;
                    actionWasInteractedWith = false;
                    if (!actionIsInstant)
                    {
                        interfaceProgressSlider.value = 0;
                        actionProgress = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Handle player interaction
        /// </summary>
        public virtual void OnInteract()
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
        protected void Complete(bool actionWasSuccessful = true)
        {
            if (!actionWasInteractedWith)
            {
                if (actionIsOneTime)
                {
                    interfaceProgressSlider.gameObject.SetActive(false);
                    interfaceIsOneTimeNotif.gameObject.SetActive(true);
                }

                actionWasInteractedWith = true;
            }

            if (actionWasSuccessful && !interfaceAudioSuccess.isPlaying)
                interfaceAudioSuccess.Play();
            else if (!interfaceAudioFailure.isPlaying && !interfaceAudioSuccess.isPlaying)
                interfaceAudioFailure.Play();
        }
        #endregion
    }
}