#if UNITY_EDITOR
namespace ComponentUtilities
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Assertions;

    internal static class Actions
    {
        /// <summary>
        /// Holds the copied components. Will be cleared when user triggers "Paste" action.
        /// </summary>
        private static readonly List<Component> copiedComponents = new List<Component>();

        /// <summary>
        /// Copies the all of the components from selected game object(s).
        /// Will not copy the "Transform" component.
        /// </summary>
        [MenuItem("GameObject/Component Utilities/Copy &c")]
        private static void Copy()
        {
            foreach (var transform in Selection.transforms)
            {
                foreach (var component in transform.GetComponents<Component>())
                {
                    // Do not copy if the component is "Transform" or "RectTransform".
                    if (IsValidComponent(component))
                        copiedComponents.Add(component);
                }
            }
        }

        /// <summary>
        /// Pastes all the copied components to active transform(s).
        /// Will not work if there are no copied components.
        /// </summary>
        [MenuItem("GameObject/Component Utilities/Paste &v")]
        private static void Paste()
        {
            // Are we really doing work?
            Assert.IsTrue(copiedComponents.Count > 0, "There are no components to paste!");

            foreach (var transform in Selection.transforms)
            {
                foreach (var component in copiedComponents)
                {
                    // Check if the component that we are trying to paste is already exists on target(s).
                    if (transform.GetComponent(component.GetType()) != null)
                        MonoBehaviour.DestroyImmediate(transform.GetComponent(component.GetType()));

                    ComponentUtility.CopyComponent(component);
                    ComponentUtility.PasteComponentAsNew(transform.gameObject);
                }
            }

            copiedComponents.Clear();
        }

        /// <summary>
        /// Deletes all the components (except "Transform" and "RectTransform") from active game object(s).
        /// </summary>
        [MenuItem("GameObject/Component Utilities/Delete &d")]
        private static void Delete()
        {
            foreach (var transform in Selection.transforms)
            {
                foreach (var component in transform.GetComponents<Component>())
                {
                    // Only "delete" component if it is not "Transform" or "RectTransform".
                    if (IsValidComponent(component))
                        Undo.DestroyObjectImmediate(component);
                }
            }
        }

        /// <summary>
        /// Checks if given component belongs to "Transform" or "RectTransform" type.
        /// </summary>
        /// <param name="component">The component to check.</param>
        /// <returns>True if component does not belongs to "Transform" or "RectTransform".</returns>
        private static bool IsValidComponent(Component component)
        {
            return component.GetType() != typeof(Transform) && component.GetType() != typeof(RectTransform);
        }
    }
}
#endif