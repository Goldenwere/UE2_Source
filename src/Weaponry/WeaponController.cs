#pragma warning disable 0649
using UnityEngine;
using Entity2.Character;
using Entity2.Core;

namespace Entity2.Weaponry
{
    /// <summary>
    /// Separate controller for weapons; must be attached to a child object of a character; weapon prefabs should be childed to the object this is attached to
    /// </summary>
    public class WeaponController : MonoBehaviour
    {
        #region Fields
        [SerializeField]    private string[]       desiredTargets;
        [SerializeField]    private bool           isPlayerController;
        [SerializeField]    private Transform[]    outSource;
        [SerializeField]    private Weapon[]       weaponInventory;
        /**************/    private ICharacter     character;
        #endregion

        #region Methods
        /// <summary>
        /// Used for weapon initialization and for finding the character that parents the controller
        /// </summary>
        private void Awake()
        {
            character = GetComponentInParent<ICharacter>();
            foreach (Weapon w in weaponInventory)
                w.Initialize(outSource, isPlayerController);
        }

        /// <summary>
        /// Used for subscribing to the fire event
        /// </summary>
        private void OnEnable()
        {
            character.CallWeaponFire += FireWeapon;
            character.CallWeaponSwitch += SwitchWeapon;
        }

        /// <summary>
        /// Used for unsubscribing from the fire event
        /// </summary>
        private void OnDisable()
        {
            character.CallWeaponFire -= FireWeapon;
            character.CallWeaponSwitch -= SwitchWeapon;
        }

        /// <summary>
        /// Handles the fire event and calls Fire on all weapons an instance of WeaponController controls
        /// </summary>
        private void FireWeapon()
        {
            foreach (Weapon w in weaponInventory)
                w.FireWeapon(isPlayerController, desiredTargets);
        }

        /// <summary>
        /// Handles the desire of a character to switch weapons
        /// <para>Currently unimplemented other than to hook up UI</para>
        /// </summary>
        private void SwitchWeapon()
        {
            if (isPlayerController)
            {
                weaponInventory[0].gameObject.SetActive(true);
                if (weaponInventory[0].WeaponAnimator != null)
                    weaponInventory[0].WeaponAnimator.Play("Weapon_" + weaponInventory[0].WeaponType.ToString() + "_Equip");
                GameEvents.Instance.CallSwitchWeapon(weaponInventory[0].gameObject);
            }
        }
        #endregion
    }
}
