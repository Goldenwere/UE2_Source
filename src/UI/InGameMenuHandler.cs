using UnityEngine;
using UnityEngine.SceneManagement;
using Entity2.Core;
using System.Collections;

namespace Entity2.UI
{
    /// <summary>
    /// Used for handling UI calls from menus and in-game windows
    /// </summary>
    public class InGameMenuHandler : MonoBehaviour
    {
        /// <summary>
        /// Used by the quit button on the pause and death windows
        /// </summary>
        public void LoadMenu()
        {
            StartCoroutine(LoadMenuAsync());
        }

        /// <summary>
        /// Used by the resume button on the pause window
        /// </summary>
        public void Resume()
        {
            GameEvents.Instance.TogglePause();
        }

        private IEnumerator LoadMenuAsync()
        {
            GameEvents.Instance.CallUpdateGameState(GameState.menus);
            AsyncOperation async = SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
            while (!async.isDone)
                yield return null;

            async.allowSceneActivation = true;
        }
    }
}