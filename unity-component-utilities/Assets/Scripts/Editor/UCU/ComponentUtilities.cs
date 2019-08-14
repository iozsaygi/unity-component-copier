#if UNITY_EDITOR
namespace UnityComponentUtilities {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    internal static class ComponentUtilities {
        private static List<Component> copiedComponents = new List<Component>();
        // We need this "functionTriggerInterval" because of weird editor bug.
        // When working with "Merge & Separate" functions the code runs more than once for some reason.
        // So this variable is here to make sure that we are running the code only once.
        private static float functionTriggerInterval = 0.0f;

        [MenuItem("GameObject/Component Utilities/Common/Copy", priority = 10)]
        private static void Copy() {
            copiedComponents.Clear();
            Transform[] activeTransforms = Selection.transforms;

            if (activeTransforms.Length == 0) {
                EditorUtility.DisplayDialog("Nothing selected!", "Please select at least 1 GameObject to copy components from!", "Ok");
                return;
            }

            for (int i = 0; i < activeTransforms.Length; i++) {
                Component[] componentsOfSelectedTransforms = activeTransforms[i].GetComponents<Component>();

                for (int j = 0; j < componentsOfSelectedTransforms.Length; j++) {
                    if (componentsOfSelectedTransforms[j] != null && !copiedComponents.Contains(componentsOfSelectedTransforms[j]) &&
                        componentsOfSelectedTransforms[j].GetType() != typeof(Transform))
                        copiedComponents.Add(componentsOfSelectedTransforms[j]);
                }
            }
        }

        [MenuItem("GameObject/Component Utilities/Common/Paste/On GameObject(s)")]
        private static void PasteOnGameObject() {
            Transform[] activeTransforms = Selection.transforms;

            if (activeTransforms.Length == 0) {
                EditorUtility.DisplayDialog("Nothing selected!", "Please select at least 1 GameObject to paste copied components!", "Ok");
                copiedComponents.Clear();
                return;
            }

            if (copiedComponents.Count == 0) {
                EditorUtility.DisplayDialog("Nothing copied!", "Not enough components are copied for pasting! Copy at least 1 component.", "Ok");
                return;
            }

            for (int i = 0; i < activeTransforms.Length; i++) {
                for (int j = 0; j < copiedComponents.Count; j++) {
                    ComponentUtility.CopyComponent(copiedComponents[j]);
                    ComponentUtility.PasteComponentAsNew(activeTransforms[i].gameObject);
                }
            }

            copiedComponents.Clear();
        }

        [MenuItem("GameObject/Component Utilities/Common/Paste/As Child")]
        private static void PasteAsChild() {
            bool result = EditorUtility.DisplayDialog("Choose pasting mode!", "Do you want to create new game object for each copied component?", "Yes", "No");
            Transform[] activeTransforms = Selection.transforms;

            switch (result) {
                case true:
                    for (int i = 0; i < activeTransforms.Length; i++) {
                        for (int j = 0; j < copiedComponents.Count; j++) {
                            GameObject separatedChild = new GameObject("Pasted Component");
                            separatedChild.transform.SetParent(activeTransforms[i], false);
                            ComponentUtility.CopyComponent(copiedComponents[j]);
                            ComponentUtility.PasteComponentAsNew(separatedChild);
                        }
                    }
                    break;

                case false:
                    GameObject gameObject = new GameObject("Pasted Component");

                    for (int i = 0; i < activeTransforms.Length; i++) {
                        gameObject.transform.SetParent(activeTransforms[i], false);

                        for (int j = 0; j < copiedComponents.Count; j++) {
                            ComponentUtility.CopyComponent(copiedComponents[j]);
                            ComponentUtility.PasteComponentAsNew(gameObject);
                        }
                    }
                    break;
            }

            copiedComponents.Clear();
        }

        [MenuItem("GameObject/Component Utilities/Common/Delete")]
        private static void Delete() {
            Transform[] activeTransforms = Selection.transforms;

            if (activeTransforms.Length == 0) {
                EditorUtility.DisplayDialog("Nothing selected!", "Please select at least 1 GameObject to delete component(s)", "Ok");
                return;
            }

            for (int i = 0; i < activeTransforms.Length; i++)
                UtilityBehaviour.Clear(activeTransforms[i].GetComponents<Component>());
        }

        private static void Merge(Component[] components) {
            if (components.Length == 0) {
                EditorUtility.DisplayDialog("Nothing selected!", "Please select at least 1 GameObject to perform merge!", "Ok");
                return;
            }

            Type[] componentsToMerge = new Type[components.Length];

            for (int i = 0; i < components.Length; i++) {
                if (components[i] != null && components[i].GetType() != typeof(Transform))
                    componentsToMerge[i] = components[i].GetType();
            }

            new GameObject("Merged", componentsToMerge);
        }

        [MenuItem("GameObject/Component Utilities/Merge/Keep Old")]
        private static void MergeKeepOld() {
            if (Time.unscaledTime.Equals(functionTriggerInterval))
                return;

            List<Component> components = new List<Component>();

            for (int i = 0; i < Selection.transforms.Length; i++) {
                Component[] componentsOnTransform = Selection.transforms[i].GetComponents<Component>();

                for (int j = 0; j < componentsOnTransform.Length; j++)
                    components.Add(componentsOnTransform[j]);
            }

            Merge(components.ToArray());

            functionTriggerInterval = Time.unscaledTime;
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
            if (components.Length == 0) {
                EditorUtility.DisplayDialog("Nothing selected!", "Please select at least 1 GameObject to separate components!", "Ok");
                return;
            }

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
            if (Time.unscaledTime.Equals(functionTriggerInterval))
                return;

            List<Component> components = new List<Component>();

            for (int i = 0; i < Selection.transforms.Length; i++) {
                Component[] componentsOnTransform = Selection.transforms[i].GetComponents<Component>();

                for (int j = 0; j < componentsOnTransform.Length; j++) {
                    if (componentsOnTransform[j] != null && componentsOnTransform[j].GetType() != typeof(Transform))
                        components.Add(componentsOnTransform[j]);
                }
            }

            Separate(components.ToArray());

            functionTriggerInterval = Time.unscaledTime;
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

        [MenuItem("GameObject/Component Utilities/Sort/By Name")]
        private static void SortByName() {
            if (Time.unscaledTime.Equals(functionTriggerInterval))
                return;

            Transform[] activeTransforms = Selection.transforms;

            if (activeTransforms.Length == 0) {
                EditorUtility.DisplayDialog("Nothing selected!", "Please select at least 1 GameObject to sort it's components!", "Ok");
                return;
            }

            if (EditorUtility.DisplayDialog("Warning!", "This action will cause data loss on components! Do you want to perform this action?", "Yes", "No")) {
                for (int i = 0; i < activeTransforms.Length; i++) {
                    Component[] components = activeTransforms[i].GetComponents<Component>().Where(x => x.GetType() != typeof(Transform)).ToArray();
                    List<Component> sortedComponents = components.ToList();

                    sortedComponents.Sort((x, y) => x.GetType().Name.CompareTo(y.GetType().Name));

                    UtilityBehaviour.Clear(components);

                    foreach (Component component in sortedComponents)
                        activeTransforms[i].gameObject.AddComponent(component.GetType());
                }

                functionTriggerInterval = Time.unscaledTime;
            }
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