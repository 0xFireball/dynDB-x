﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IridiumIon.JsonFlat2.Internal
{
    /// <summary>
    /// Based on https://codedump.io/share/w4CmxtkbwSGD/1/how-to-unflatten-flattened-json-in-c
    /// </summary>
    internal class JsonUnflattenerHelper
    {
        private enum JsonKind
        {
            Object, Array
        }

        public static JObject UnflattenJObject(Dictionary<string, string> flattenedObj)
        {
            JContainer result = new JObject();
            var settings = new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Merge
            };
            foreach (var pathValue in flattenedObj)
            {
                result.Merge(UnflattenSingle(pathValue), settings);
            }
            return result as JObject;
        }

        private static JContainer UnflattenSingle(KeyValuePair<string, string> keyValue)
        {
            string path = keyValue.Key;
            string value = keyValue.Value;
            var pathSegments = SplitPath(path);

            JContainer lastItem = null;
            // build from leaf to root
            foreach (var pathSegment in pathSegments.Reverse())
            {
                var type = GetJsonType(pathSegment);
                switch (type)
                {
                    case JsonKind.Object:
                        var obj = new JObject();
                        if (null == lastItem)
                        {
                            obj.Add(pathSegment, value);
                        }
                        else
                        {
                            obj.Add(pathSegment, lastItem);
                        }
                        lastItem = obj;
                        break;

                    case JsonKind.Array:
                        var array = new JArray();
                        var index = GetArrayIndex(pathSegment);
                        array = FillEmpty(array, index);
                        if (lastItem == null)
                        {
                            array[index] = value;
                        }
                        else
                        {
                            array[index] = lastItem;
                        }
                        lastItem = array;
                        break;
                }
            }
            return lastItem;
        }

        private static IList<string> SplitPath(string path)
        {
            var result = new List<string>();
            var reg = new Regex(@"(?!\.)([^. ^\[\]]+)|(?!\[)(\d+)(?=\])");
            foreach (Match match in reg.Matches(path))
            {
                result.Add(match.Value);
            }
            return result;
        }

        private static JArray FillEmpty(JArray array, int index)
        {
            for (int i = 0; i <= index; i++)
            {
                array.Add(null);
            }
            return array;
        }

        private static JsonKind GetJsonType(string pathSegment)
        {
            int x;
            return int.TryParse(pathSegment, out x) ? JsonKind.Array : JsonKind.Object;
        }

        private static int GetArrayIndex(string pathSegment)
        {
            int result;
            if (int.TryParse(pathSegment, out result))
            {
                return result;
            }
            throw new FormatException("Unable to parse array index: " + pathSegment);
        }
    }
}