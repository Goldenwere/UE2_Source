﻿using UnityEngine;

namespace Goldenwere.Unity
{
    /// <summary>
    /// Extensions to the Unity API
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Find the child with a specific name of a gameobject
        /// </summary>
        /// <param name="parent">The parent gameobject to search from</param>
        /// <param name="name">The name that the child must match</param>
        /// <returns>The found GameObject or null</returns>
        public static GameObject FindChild(this GameObject parent, string name)
        {
            if (parent == null)
                throw new System.ArgumentNullException();

            for (int i = 0; i < parent.transform.childCount; i++)
            {
                GameObject child = parent.transform.GetChild(i).gameObject;
                if (child.name == name)
                    return child;
            }

            return null;
        }

        /// <summary>
        /// Find the child with a specific name of a gameobject, recursively searching through all gameobjects from topmost level down
        /// </summary>
        /// <param name="parent">The parent gameobject to search from</param>
        /// <param name="name">The name that the child must match</param>
        /// <returns>The found GameObject or null</returns>
        public static GameObject FindChildRecursively(this GameObject parent, string name)
        {
            if (parent == null)
                throw new System.ArgumentNullException();

            for (int i = 0; i < parent.transform.childCount; i++)
            {
                GameObject child = parent.transform.GetChild(i).gameObject;
                if (child.name == name)
                    return child;

                else if (child.transform.childCount > 0)
                {
                    GameObject foundTest = child.FindChildRecursively(name);
                    if (foundTest != null)
                        return foundTest;
                }
            }

            return null;
        }

        /// <summary>
        /// Find the child with a specific tag of a gameobject
        /// </summary>
        /// <param name="parent">The parent gameobject to search from</param>
        /// <param name="tag">The tag that the child must match</param>
        /// <returns>The found GameObject or null</returns>
        public static GameObject FindChildWithTag(this GameObject parent, string tag)
        {
            if (parent == null)
                throw new System.ArgumentNullException();

            for (int i = 0; i < parent.transform.childCount; i++)
            {
                GameObject child = parent.transform.GetChild(i).gameObject;
                if (child.tag == tag)
                    return child;
            }

            return null;
        }

        /// <summary>
        /// Clamps a quaternion's vertical rotation
        /// </summary>
        /// <param name="parent">The quaternion to clamp</param>
        /// <param name="min">The lowest possible vertical rotation in degrees</param>
        /// <param name="max">The highest possible vertical rotation in degrees</param>
        /// <returns>The clamped quaternion</returns>
        public static Quaternion VerticalClampEuler(this Quaternion parent, float min, float max)
        {
            parent.x /= parent.w;
            parent.y /= parent.w;
            parent.z /= parent.w;
            parent.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(parent.x);

            angleX = Mathf.Clamp(angleX, min, max);

            parent.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return parent;
        }
    }
}