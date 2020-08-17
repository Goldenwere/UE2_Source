using UnityEngine;
using Entity2.Data;
using Entity2.Core;

namespace Entity2.Util
{
    /// <summary>
    /// Handles spawning/despawning of the player depending on GameEvents / loading events
    /// </summary>
    public class PlayerSpawner : MonoBehaviour
    {
        #region Fields
        [SerializeField]    private GameObject  playerPrefab;
        /**************/    private GameObject  playerRef;
        #endregion

        #region Methods
        /// <summary>
        /// Subscribe to events on enable
        /// </summary>
        private void OnEnable()
        {
            PlayerLocationData.PlayerLocLoaded += Spawn;
            GameEvents.UpdateGameState += Despawn;
        }

        /// <summary>
        /// Unsubscribe from events on disable
        /// </summary>
        private void OnDisable()
        {
            PlayerLocationData.PlayerLocLoaded -= Spawn;
            GameEvents.UpdateGameState -= Despawn;
        }

        /// <summary>
        /// Spawn the player on the OnPlayerLocLoaded event
        /// </summary>
        /// <param name="loc"></param>
        private void Spawn(SaveLocation loc)
        {
            playerRef = Instantiate(playerPrefab, loc.Position, loc.Orientation);
            GameEvents.Instance.PlayerSpawn(playerRef);
        }

        /// <summary>
        /// Despawn the player when leaving to the menus
        /// </summary>
        /// <para>Likely will be changed, due to gamescene-to-gamescene loading (and this not handling that)</para>
        /// <param name="prev">unused</param>
        /// <param name="curr">If the new gamestate is the menus, destroy the player</param>
        private void Despawn(GameState prev, GameState curr)
        {
            if (curr == GameState.menus)
                Destroy(playerRef);
        }
        #endregion
    }
}