#if UNITY_EDITOR
namespace UnityComponentUtilities {
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;
    using System;

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

        private static void Merge(Component[] components) {
            Type[] componentsToMerge = new Type[components.Length];

            for (int i = 0; i < components.Length; i++) {
                if (components[i] != null && components[i].GetType() != typeof(Transform))
                    componentsToMerge[i] = components[i].GetType();
            }

            new GameObject("Merged", componentsToMerge);
        }

        [MenuItem("GameObject/Component Utilities/Merge/Keep Old")]
        private static void MergeKeepOld() {
            List<Component> components = new List<Component>();

            for (int i = 0; i < Selection.transforms.Length; i++) {
                Component[] componentsOnTransform = Selection.transforms[i].GetComponents<Component>();

                for (int j = 0; j < componentsOnTransform.Length; j++)
                    components.Add(componentsOnTransform[j]);
            }

            Merge(components.ToArray());
        }

        [MenuItem("GameObject/Component Utilities/Merge/Delete Old")]
        private static void MergeDeleteOld() {
            List<Component> components = new List<Component>();
            Transform[] transformsToDelete = new Transform[Selection.transforms.Length];

            for (int i = 0; i < Selection.transforms.Length; i++) {
                Component[] componentsOnTransform = Selection.transforms[i].GetComponents<Component>();

                for (int j = 0; j < componentsOnTransform.Length; j++)
                    components.Add(componentsOnTransform[j]);

                transformsToDelete[i] = Selection.transforms[i];
            }

            Merge(components.ToArray());
            UtilityBehaviour.Clear(transformsToDelete);
        }

        private static void Separate(Component[] components) {
            for (int i = 0; i < components.Length; i++) {
                if (components[i] != null && components[i].GetType() != typeof(Transform)) {
                    GameObject gameObject = new GameObject(components[i].GetType().Name);
                    ComponentUtility.CopyComponent(components[i]);
                    ComponentUtility.PasteComponentAsNew(gameObject);
                }
            }
        }

        [MenuItem("GameObject/Component Utilities/Separate/Keep Old")]
        private static void SeparateKeepOld() {
            List<Component> components = new List<Component>();

            for (int i = 0; i < Selection.transforms.Length; i++) {
                Component[] componentsOnTransform = Selection.transforms[i].GetComponents<Component>();

                for (int j = 0; j < componentsOnTransform.Length; j++) {
                    if (componentsOnTransform[j] != null && componentsOnTransform[j].GetType() != typeof(Transform))
                        components.Add(componentsOnTransform[j]);
                }
            }

            Separate(components.ToArray());
        }

        [MenuItem("GameObject/Component Utilities/Separate/Delete Old")]
        private static void SeparateDeleteOld() {
            List<Component> components = new List<Component>();
            Transform[] transformsToDelete = new Transform[Selection.transforms.Length];

            for (int i = 0; i < Selection.transforms.Length; i++) {
                Component[] componentsOnTransform = Selection.transforms[i].GetComponents<Component>();

                for (int j = 0; j < componentsOnTransform.Length; j++)
                    components.Add(componentsOnTransform[j]);

                transformsToDelete[i] = Selection.transforms[i];
            }

            Separate(components.ToArray());
            UtilityBehaviour.Clear(transformsToDelete);
        }

        // To destroy components/objects i needed to call "DestroyImmediate" function
        // which can only be called from "MonoBehaviour" class. So this class exists only for calling "Destroy" functions.
        internal sealed class UtilityBehaviour : MonoBehaviour {

            public static void Clear(Component[] components) {
                for (int i = 0; i < components.Length; i++) {
                    if (components[i] != null && components[i].GetType() != typeof(Transform))
                        DestroyImmediate(components[i]);
                }
            }

            public static void Clear(Transform[] transforms) {
                for (int i = 0; i < transforms.Length; i++)
                    DestroyImmediate(transforms[i].gameObject);
            }
        }
    }
} // namespace UnityComponentUtilities
#endif