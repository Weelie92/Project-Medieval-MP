using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.Yapp
{
    /// <summary>
    /// Physics should only run on the YAPP objects.
    /// This class collects all objects of the scene which are affected by physics.
    /// While the YAPP simulation runs the physics settings of those objects are disabled.
    /// When the physics simulation stops the phyiscs settings are re-enabled again.
    /// </summary>
    public class PhysicsFilter
    {
        /// <summary>
        /// All rigidbodies which had isKinematic = false.
        /// We need to restore these
        /// </summary>
        private static List<Rigidbody> kinematicFalseList = new List<Rigidbody>();

        /// <summary>
        /// All rigidbodies which had detectCollisions = true.
        /// We need to restore these
        /// </summary>
        private static List<Rigidbody> detectCollisionsTrueList = new List<Rigidbody>();

        /// <summary>
        /// List of all YAPP added rigidbodies which are currently used in the simulation.
        /// </summary>
        private static List<Rigidbody> simulatedRigidBodies = new List<Rigidbody>();

        ///// <summary>
        ///// List of all YAPP added gameobjects which are currently used in the simulation.
        ///// </summary>
        //private static List<Transform> allSimulatedGameObjects = new List<Transform>();

        /// <summary>
        /// Backup can be called multiple times. We want the physics to continue while children of the same container keep on being added while the simulation runs.
        /// </summary>
        /// <param name="simulatedGameObjects"></param>
        public static void Backup(Transform[] simulatedGameObjects)
        {
            // add all rigidbodies of the simulated gameobjects to the collection
            foreach (Transform transform in simulatedGameObjects)
            {
                //// add the current transform
                //allSimulatedGameObjects.Add(transform);

                // collect all rigidbodies
                Rigidbody[] rigidbodies = transform.GetComponentsInChildren<Rigidbody>();

                foreach (Rigidbody rigidbody in rigidbodies)
                {
                    simulatedRigidBodies.Add(rigidbody);
                }
            }

            // get all rigidbodies of the scene; this includes the simulated ones
            Rigidbody[]  allRigidBodies = (Rigidbody[])GameObject.FindObjectsOfType(typeof(Rigidbody));

            foreach (Rigidbody rigidbody in allRigidBodies)
            {
                // skip the currently simulated ones
                if (simulatedRigidBodies.Contains(rigidbody))
                    continue;

                // register kinematic, but only if they aren't already registered; only first registration counts, later one the settings may have been modified
                if (!kinematicFalseList.Contains(rigidbody))
                {
                    if (rigidbody.isKinematic == false)
                    {
                        kinematicFalseList.Add(rigidbody);
                    }
                }

                // register collisions, but only if they aren't already registered; only first registration counts, later one the settings may have been modified
                if (!detectCollisionsTrueList.Contains(rigidbody))
                {
                    if (rigidbody.detectCollisions == true)
                    {
                        detectCollisionsTrueList.Add(rigidbody);
                    }
                }

                //
                // modify: disable physics on the object
                //
                try
                {
                    rigidbody.isKinematic = true;
                    rigidbody.detectCollisions = false;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Can't modify rigidbody setting: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Restore is called once at the end of the physics simulation
        /// </summary>
        public static void Restore()
        {
            //// unregister all YAPP generated rigidbodies
            //foreach (Transform transform in allSimulatedGameObjects)
            //{
            //    Rigidbody[] rigidbodies = transform.GetComponentsInChildren<Rigidbody>();

            //    foreach (Rigidbody rigidbody in rigidbodies)
            //    {
            //        simulatedRigidBodies.Remove(rigidbody);
            //    }
            //}

            // restore kinematic setting
            foreach (Rigidbody rigidbody in kinematicFalseList)
            {
                try
                {
                    rigidbody.isKinematic = false; // TODO: try-catch
                } 
                catch( System.Exception ex)
                {
                    Debug.LogError("Can't restore kinematic setting: " + ex.Message);
                }
            }

            // restore collision setting
            foreach (Rigidbody rigidbody in detectCollisionsTrueList)
            {
                try
                {
                    rigidbody.detectCollisions = true;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Can't restore collision setting: " + ex.Message);
                }
            }

            simulatedRigidBodies.Clear();
            kinematicFalseList.Clear();
            detectCollisionsTrueList.Clear();
        }

    }
}