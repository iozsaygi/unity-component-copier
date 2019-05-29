#if UNITY_EDITOR
namespace UnityComponentUtilities {
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;

    internal static class ComponentUtilities {
        private static List<Component> copiedComponents = new List<Component>();

        [MenuItem("GameObject/Component Utilities/Copy", priority = 10)]
        private static void Copy() {
            copiedComponents.Clear();
            Transform[] activeTransforms = Selection.transforms;

            for (int i = 0; i < activeTransforms.Length; i++) {
                Component[] componentsOfSelectedTransforms = activeTransforms[i].GetComponents<Component>();

                for (int j = 0; j < componentsOfSelectedTransforms.Length; j++) {
                    if (componentsOfSelectedTransforms[j] != null && !copiedComponents.Contains(componentsOfSelectedTransforms[j]) &&
                        componentsOfSelectedTransforms[j].GetType() != typeof(Transform))
                        copiedComponents.Add(componentsOfSelectedTransforms[j]);
                }
            }
        }

        [MenuItem("GameObject/Component Utilities/Paste")]
        private static void Paste() {
            Transform[] activeTransforms = Selection.transforms;

            if (copiedComponents.Count == 0)
                return;

            for (int i = 0; i < activeTransforms.Length; i++) {
                for (int j = 0; j < copiedComponents.Count; j++) {
                    ComponentUtility.CopyComponent(copiedComponents[j]);
                    ComponentUtility.PasteComponentAsNew(activeTransforms[i].gameObject);
                }
            }

            copiedComponents.Clear();
        }

        [MenuItem("GameObject/Component Utilities/Delete")]
        private static void Delete() {
            Transform[] activeTransforms = Selection.transforms;

            for (int i = 0; i < activeTransforms.Length; i++)
                UtilityBehaviour.Clear(activeTransforms[i].GetComponents<Component>());
        }

        [MenuItem("GameObject/Component Utilities/Separate")]
        private static void Separate() {
            // Will not work when multiple game objects are selected in editor.
            // Waiting for new engine versions to fix.
            foreach (Component component in Selection.activeGameObject.GetComponents<Component>()) {
                if (component != null && component.GetType() != typeof(Transform)) {
                    GameObject gameObject = new GameObject(component.GetType().Name);
                    ComponentUtility.CopyComponent(component);
                    ComponentUtility.PasteComponentAsNew(gameObject);
                }
            }
        }

        // To destroy components from game objects i needed to call "DestroyImmediate" function
        // which can only be called from "MonoBehaviour" class. So this class exists only for calling "DestroyImmediate" function.
        internal sealed class UtilityBehaviour : MonoBehaviour {

            public static void Clear(Component[] components) {
                for (int i = 0; i < components.Length; i++) {
                    if (components[i] != null && components[i].GetType() != typeof(Transform))
                        DestroyImmediate(components[i]);
                }
            }
        }
    }
} // namespace UnityComponentUtilities
#endif