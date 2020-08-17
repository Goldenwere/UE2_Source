using UnityEngine;

namespace Entity2.Obj
{
    /// <summary>
    /// Script used to randomize the starting point of an animation on MonoBehaviour.Start()
    /// </summary>
    public class AnimRandomStart : MonoBehaviour
    {
        [SerializeField]    private Animator    anim;
        [SerializeField]    private string      animToPlay;

        /// <summary>
        /// On MonoBehaviour.Start(), play animToPlay on anim
        /// </summary>
        private void Start()
        {
            anim.Play(animToPlay, -1, Random.Range(0.0f, 1.0f));
        }
    }
}