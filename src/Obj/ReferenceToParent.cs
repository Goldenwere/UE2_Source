using UnityEngine;

namespace Entity2.Obj
{
    /// <summary>
    /// References to the uppermost parent of a prefab or other gameobject, typically with a special component being searched for
    /// </summary>
    public class ReferenceToParent : MonoBehaviour
    {
        [SerializeField] private GameObject parent;

        public GameObject Parent
        {
            get { return parent; }
        }
    }
}