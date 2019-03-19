namespace UnityComponentCopier
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityComponentCopier.Behaviours;
    using UnityComponentCopier.Utilities;

    internal static class ComponentCopier
    {
        private static GameObject clickedGameObject = null;
        private static List<Component> components = new List<Component>();

        [MenuItem("GameObject/Component Copier/Copy Component(s)", validate = false, priority = 10)]
        private static void CopyComponents()
        {
            clickedGameObject = GetActiveGameObject();

            if (clickedGameObject == null)
            {
                EditorUtility.DisplayDialog("Unity Component Copier", "Can not copy components of null game object!", "Ok");
                return;
            }

            components = ComponentFilter.Filter(clickedGameObject.GetComponents<Component>()).ToList();

            if (components.Count == 0)
            {
                EditorUtility.DisplayDialog("Unity Component Copier", "There are not enough components to copy on game object! (Not including Transform)", "Ok");
                return;
            }

            // @this log will not work for single thread forced components like transform.
            Debug.Log("" + components.Count + " component(s) copied from " + clickedGameObject.name + "!");
            clickedGameObject = null;
        }


        [MenuItem("GameObject/Component Copier/Paste Component(s)")]
        private static void PasteComponents()
        {
            if (components.Count == 0)
            {
                EditorUtility.DisplayDialog("Unity Component Copier", "There are not enough components to paste! Try to copy components again.", "Ok");
                return;
            }

            clickedGameObject = GetActiveGameObject();

            foreach (Component component in components)
            {
                // if component is added before -> remove it and paste it as new
                ComponentUtility.CopyComponent(component);
                ComponentUtility.PasteComponentAsNew(clickedGameObject);
            }

            Debug.Log(components.Count + " component(s) pasted on " + clickedGameObject.name + "!");
            Reset();
        }

        [MenuItem("GameObject/Component Copier/Delete Component(s)")]
        private static void DeleteComponents()
        {
            GameObject activeGameObject = GetActiveGameObject();
            Component[] localCmps = ComponentFilter.Filter(activeGameObject.GetComponents<Component>());

            if (localCmps.Length > 0)
            {
                ClearBehaviour.ClearComponents(localCmps);
                Debug.Log(localCmps.Length + " component(s) removed from " + activeGameObject.name + "!");
            }
            else
            {
                EditorUtility.DisplayDialog("Unity Component Copier", "Can not delete transform component!", "Ok");
            }

            Reset();
        }

        private static GameObject GetActiveGameObject()
        {
            // clicked object -> the one that is active on inspector.
            return Selection.activeGameObject;
        }

        private static void Reset()
        {
            components.Clear();
            clickedGameObject = null;
        }
    }
}