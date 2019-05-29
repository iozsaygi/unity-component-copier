#if UNITY_EDITOR
namespace UnityComponentUtilities {
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;
    using System;

    internal static class ComponentUtilities {
        private static List<Component> copiedComponents = new List<Component>();
        // We need this because somehow Unity calls "MenuItem" functions more than once when working with
        // multiple game objects. Let's just hope it will be fixed with new engine versions and we'll stop doing this workaround.
        private static float functionCallInterval = 0.0f;

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

        [MenuItem("GameObject/Component Utilities/Merge")]
        private static void Merge() {
            if (Time.unscaledTime.Equals(functionCallInterval))
                return;

            if (Selection.transforms.Length > 1) {
                List<Type> componentsToMerge = new List<Type>();

                for (int i = 0; i < Selection.transforms.Length; i++) {
                    Component[] components = Selection.transforms[i].GetComponents<Component>();

                    for (int j = 0; j < components.Length; j++) {
                        if (components[j] != null && components[j].GetType() != typeof(Transform))
                            componentsToMerge.Add(components[j].GetType());
                    }
                }

                new GameObject("Merged", componentsToMerge.ToArray());
            }

            functionCallInterval = Time.unscaledTime;
        }

        [MenuItem("GameObject/Component Utilities/Separate")]
        private static void Separate() {
            if (Time.unscaledTime.Equals(functionCallInterval))
                return;

            for (int i = 0; i < Selection.transforms.Length; i++) {
                Component[] components = Selection.transforms[i].GetComponents<Component>();

                for (int j = 0; j < components.Length; j++) {
                    if (components[j] != null && components[j].GetType() != typeof(Transform)) {
                        GameObject gameObject = new GameObject(components[j].GetType().Name);
                        ComponentUtility.CopyComponent(components[j]);
                        ComponentUtility.PasteComponentAsNew(gameObject);
                    }
                }
            }

            functionCallInterval = Time.unscaledTime;
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