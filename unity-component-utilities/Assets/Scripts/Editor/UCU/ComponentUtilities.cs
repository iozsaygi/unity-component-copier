#if UNITY_EDITOR
namespace UnityComponentUtilities
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;

    internal sealed class UtilityBehaviour : MonoBehaviour
    {
        public static void Clear(Component[] components)
        {
            Component[] filteredComponents = Filter(components, Selection.activeGameObject.GetComponent<Transform>());

            foreach (Component component in filteredComponents)
                DestroyImmediate(component);
        }

        public static Component[] Filter(Component[] components, Component filterSource)
        {
            return components.Where(c => c.GetType() != filterSource.GetType()).ToArray();
        }
    }

    internal static class ComponentUtilities
    {
        private static GameObject selectedGameObject = null;
        private static List<Component> components = new List<Component>();

        [MenuItem("GameObject/Component Utilities/Copy", priority = 10)]
        private static void Copy()
        {
            selectedGameObject = Selection.activeGameObject;

            if (selectedGameObject != null)
            {
                components = UtilityBehaviour.Filter(selectedGameObject.GetComponents<Component>(), selectedGameObject.GetComponent<Transform>()).ToList();

                // This log will not work for single thread forced components like transform.
                Debug.Log("" + components.Count + " component(s) copied from " + selectedGameObject.name + "!");
                selectedGameObject = null;
            }
            else
            {
                EditorUtility.DisplayDialog("Unity Component Utilities", "Can not copy components of null game object!", "Ok");
                return;
            }
        }

        [MenuItem("GameObject/Component Utilities/Paste")]
        private static void Paste()
        {
            if (components.Count > 0)
            {
                selectedGameObject = Selection.activeGameObject;

                foreach (Component component in components)
                {
                    ComponentUtility.CopyComponent(component);
                    ComponentUtility.PasteComponentAsNew(selectedGameObject);
                }

                Debug.Log(components.Count + " component(s) pasted on " + selectedGameObject.name + "!");
                Reset();
            }
            else
            {
                EditorUtility.DisplayDialog("Unity Component Utilities", "There are not enough components to paste! Try to copy components before paste!", "Ok");
                return;
            }
        }

        [MenuItem("GameObject/Component Utilities/Delete")]
        private static void Delete()
        {
            GameObject activeGameObject = Selection.activeGameObject;

            if (activeGameObject == null)
            {
                EditorUtility.DisplayDialog("Unity Component Utilities", "Can not delete components of null game object!", "Ok");
                return;
            }

            Component[] localComponents = UtilityBehaviour.Filter(activeGameObject.GetComponents<Component>(), activeGameObject.GetComponent<Transform>());

            if (localComponents.Length > 0)
            {
                UtilityBehaviour.Clear(localComponents);
                Debug.Log(localComponents.Length + " component(s) removed from " + activeGameObject.name + "!");
            }
            else
            {
                EditorUtility.DisplayDialog("Unity Component Utilities", "Can not delete transform component!", "Ok");
            }

            Reset();
        }

        [MenuItem("GameObject/Component Utilities/Separate")]
        private static void Separate()
        {
            GameObject activeGameObject = Selection.activeGameObject;

            if (activeGameObject != null)
            {
                Component[] components = activeGameObject.GetComponents<Component>();
                Component[] filteredComponents = UtilityBehaviour.Filter(components, activeGameObject.GetComponent<Transform>());

                foreach (Component component in filteredComponents)
                {
                    GameObject gameObject = new GameObject(component.GetType().Name);
                    gameObject.AddComponent(component.GetType());
                }
            }
        }

        private static void Reset()
        {
            selectedGameObject = null;
            components.Clear();
        }
    } 
}
#endif