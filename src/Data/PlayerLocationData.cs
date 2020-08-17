using System;
using UnityEngine;
using Entity2.Core;

namespace Entity2.Data
{
    public delegate void PlayerLocLoadedDelegate(SaveLocation loc);

    /// <summary>
    /// Stores player location data for save files based on preset save positions
    /// </summary>
    public class PlayerLocationData : MonoBehaviour
    {
        [SerializeField]    private SceneLocation[]                         scenePositions;
        /**************/    public  static event PlayerLocLoadedDelegate    PlayerLocLoaded;

        /// <summary>
        /// Subscribe to the OnGameSceneLoaded event when enabled
        /// </summary>
        private void OnEnable()
        {
            GameEvents.GameSceneLoaded += OnGameSceneLoaded;
        }

        /// <summary>
        /// Unsubscribe from the OnGameSceneLoaded event when disabled
        /// </summary>
        private void OnDisable()
        {
            GameEvents.GameSceneLoaded -= OnGameSceneLoaded;
        }

        /// <summary>
        /// On the OnGamesceneLoaded event, load player location data
        /// </summary>
        private void OnGameSceneLoaded()
        {
            SaveLocation loc = new SaveLocation();
            loc.Position = scenePositions[0].Locations[0];
            loc.Orientation = scenePositions[0].Orientations[0];
            PlayerLocLoaded?.Invoke(loc);
        }
    }

    /// <summary>
    /// Defines preset save positions for a scene
    /// </summary>
    [Serializable]
    public struct SceneLocation
    {
        [SerializeField]    private string     sceneName;              // Not used in code, only used for labeling purposes in inspector
        [SerializeField]    private Vector3[]  locations;              // The locations themselves
        [SerializeField]    private float[]    yRotations;             // What direction (y-rotation angle in euler) the player faces at a point

        /// <summary>
        /// Returns the preset locations
        /// </summary>
        public Vector3[]    Locations { get { return locations; } }

        /// <summary>
        /// Returns quaternions based off of the presets' directional orientations (y-rotations)
        /// </summary>
        public Quaternion[] Orientations
        {
            get
            {
                Quaternion[] o = new Quaternion[yRotations.Length];
                for (int i = 0; i < yRotations.Length; i++)
                {
                    o[i] = Quaternion.Euler(0, yRotations[i], 0);
                }
                return o;
            }
        }
    }

    /// <summary>
    /// Used for sending position and orientation data back and fourth
    /// </summary>
    public struct SaveLocation
    {
        public Vector3      Position;
        public Quaternion   Orientation;
    }
}