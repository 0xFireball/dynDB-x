using System.Collections.Generic;

namespace IridiumIon.JsonFlat2.Internal
{
    internal static class DictMerger
    {
        /// <summary>
        /// Merge some dictionaries, overwriting contents. Later dictionaries have higher priority and will overwrite entries
        /// in earlier dictionaries.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="dicts"></param>
        /// <returns></returns>
        public static Dictionary<T1, T2> Merge<T1, T2>(params Dictionary<T1, T2>[] dicts)
        {
            var resultDict = new Dictionary<T1, T2>();
            resultDict.MergeInto(dicts);
            return resultDict;
        }

        /// <summary>
        /// Merge dictionaries into a dictionary. Later dictionaries will overwite earlier entries.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="currentDict"></param>
        /// <param name="dicts"></param>
        public static void MergeInto<T1, T2>(this Dictionary<T1, T2> currentDict, params Dictionary<T1, T2>[] dicts)
        {
            foreach (var dict in dicts)
            {
                foreach (var kvp in dict)
                {
                    if (!currentDict.ContainsKey(kvp.Key))
                    {
                        // Doesn't exist yet
                        currentDict.Add(kvp.Key, kvp.Value);
                    }
                    else
                    {
                        // Overwrite
                        currentDict[kvp.Key] = kvp.Value;
                    }
                }
            }
        }
    }
}