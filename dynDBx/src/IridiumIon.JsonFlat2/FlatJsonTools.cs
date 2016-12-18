using System;
using System.Collections.Generic;
using System.Linq;

namespace IridiumIon.JsonFlat2
{
    public static class FlatJsonTools
    {
        /// <summary>
        /// Remove all nodes (including children!) that match the path. Path should not end with ., suffix will be auto-added
        /// </summary>
        /// <param name="path"></param>
        /// <param name="flattenedJObj"></param>
        public static void RemoveNode(string path, Dictionary<string, string> flattenedJObj)
        {
            // We could use LINQ, but this is more readable
            var removeKeys = new List<string>();
            var pathMatches = new[] { path + ".", path + "[" };
            foreach (var kvp in flattenedJObj)
            {
                if (kvp.Key.StartsWithAny(pathMatches, StringComparison.CurrentCulture))
                {
                    // This item is to be removed.
                    removeKeys.Add(kvp.Key);
                }
                else
                {
                    // Hi
                }
            }
            // Remove listed keys
            foreach (var rk in removeKeys)
            {
                flattenedJObj.Remove(rk);
            }
        }
    }
}