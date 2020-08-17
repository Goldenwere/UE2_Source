using UnityEngine;

namespace Entity2.Obj
{
    /// <summary>
    /// Used for destroying particles at runtime to prevent against a buildup of particle gameobjects
    /// </summary>
    public class ParticleDestroyer : MonoBehaviour
    {
        [SerializeField]    private float   keepAlive;
        /**************/    private float   timer;

        /// <summary>
        /// Called once per frame; used to keep track of whether to destroy this object or not
        /// </summary>
        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= keepAlive)
                Destroy(gameObject);
        }
    }
}