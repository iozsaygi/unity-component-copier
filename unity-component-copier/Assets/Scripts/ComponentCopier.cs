namespace UnityComponentCopier {
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;

    internal static class ComponentCopier {

        private static GameObject clickedGameObject = null;
        private static List<Component> components = new List<Component>();

        [MenuItem("GameObject/Component Copier/Copy Component(s)", validate = false, priority = 10)]
        private static void CopyComponents() {
            AssignClickedObject();

            if (clickedGameObject == null) {
                EditorUtility.DisplayDialog("Unity Component Copier", "Can not copy components of null game object!", "Ok");
                return;
            }

            Component[] cmps = clickedGameObject.GetComponents<Component>();

            if (cmps.Length == 0) {
                EditorUtility.DisplayDialog("Unity Component Copier", "There are not enough components to copy on game object!", "Ok");
                return;
            }

            components = cmps.ToList();
            clickedGameObject = null;
        }

        [MenuItem("GameObject/Component Copier/Paste Component(s)")]
        private static void PasteComponents() {
            if (components.Count == 0) {
                EditorUtility.DisplayDialog("Unity Component Copier", "There are not enough components to paste!", "Ok");
                return;
            }

            AssignClickedObject();

            foreach (Component component in components) {
                ComponentUtility.CopyComponent(component);
                ComponentUtility.PasteComponentAsNew(clickedGameObject);
            }

            Reset();
        }

        private static void AssignClickedObject() {
            // clicked object -> the one that is active on inspector.
            clickedGameObject = Selection.activeGameObject;
        }

        private static void Reset() {
            components.Clear();
            clickedGameObject = null;
        }
    }
} // namespace UnityComponentCopier