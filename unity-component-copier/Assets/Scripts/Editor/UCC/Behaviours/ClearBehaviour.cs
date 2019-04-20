#if UNITY_EDITOR
namespace UnityComponentCopier.Behaviours {
    using UnityEngine;
    using UnityComponentCopier.Utilities;

    // @just contains helper function(s) for us.
    internal sealed class ClearBehaviour : MonoBehaviour {

        public static void ClearComponents(Component[] components) {
            if (components != null && components.Length > 0) {
                // filtering to avoid transform
                components = ComponentFilter.Filter(components);

                foreach (Component component in components)
                    DestroyImmediate(component);
            }
        }
    }
}
#endif