using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OccaSoftware.Altos.Runtime;
using System.Linq;

namespace OccaSoftware.Altos.Editor
{
    public class CreateGameObjectContextMenus : EditorWindow
    {
        /// <summary>
        /// Sets up a Skybox Director object stack in the current scene's hierarchy.
        /// </summary>
        [MenuItem("GameObject/Altos/Sky Director", false, 15)]
        private static void CreateDirectorWithChildren()
        {
            if (FindObjectOfType<AltosSkyDirector>() != null)
			{
                Debug.Log("Altos Sky Director already exists in scene.");
                return;
            }
            GameObject director = CreateSkyboxDirector();
            CreateSkyObject();
            CreateSkyObject();
            Selection.activeObject = director;
        }

        public static GameObject CreateSkyboxDirector()
        {
            List<System.Type> types = new List<System.Type>()
            {
                typeof(AltosSkyDirector)
            };

            GameObject skyDirector = new GameObject("Sky Director", types.ToArray());

            return skyDirector;
        }

        

        [MenuItem("GameObject/Altos/Sky Object", false, 15)]
        public static void CreateSkyObject()
		{
            AltosSkyDirector skyDirector = FindObjectOfType<AltosSkyDirector>();
			if (skyDirector == null)
			{
                CreateDirectorWithChildren();
                return;
			}

            List<SkyObject> skyObjects = FindObjectsOfType<SkyObject>().ToList();

            List<System.Type> types = new List<System.Type>()
            {
                typeof(SkyObject)
            };

            
            bool sunAlreadyExists = CheckIfAnySuns(skyObjects);
            string name = sunAlreadyExists ? "Moon" : "Sun";
            float orbitOffset = sunAlreadyExists ? 180f : 0f;
            SkyObject.ObjectType objectType = sunAlreadyExists ? SkyObject.ObjectType.Other : SkyObject.ObjectType.Sun;

            GameObject newSkyObject = new GameObject(name, types.ToArray());
            newSkyObject.transform.SetParent(skyDirector.transform);
            newSkyObject.GetComponent<SkyObject>().type = objectType;
            newSkyObject.GetComponent<SkyObject>().orbitOffset = orbitOffset;
            newSkyObject.GetComponent<SkyObject>().SetIcon();
            Selection.activeObject = newSkyObject;
        }

        private static bool CheckIfAnySuns(List<SkyObject> skyObjects)
		{
            foreach(SkyObject skyObject in skyObjects)
			{
                if (skyObject.type == SkyObject.ObjectType.Sun)
                    return true;
			}
            return false;
		}
    }

}
