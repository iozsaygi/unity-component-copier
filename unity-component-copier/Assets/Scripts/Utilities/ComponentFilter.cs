namespace UnityComponentCopier.Utilities {
    using System.Linq;
    using UnityEngine;

    internal static class ComponentFilter {

        public static Component[] Filter(Component[] components) {
            if (components != null)
                return components.Where(c => c.GetType() != typeof(Transform)).ToArray();
            else
                return null;
        }
    }
}
