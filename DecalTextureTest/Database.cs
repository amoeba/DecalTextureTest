using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading;
using System.Globalization;

namespace DecalTextureTest
{
    internal static class Database
    {
        public static bool IsInitialzed = false;
        public static bool IsLoaded = false;
        public static bool IsLoading = false;
        public static JObject o = null; // TODO: Eventually get rid of this once I figure out parsing
        public static Dictionary<UInt32, Dictionary<UInt32, UInt32>> Mapping { get; set; }
        public static UInt32 lastStringMappingKey = 0;
        public static Dictionary<string, UInt32> ReverseStringMappings { get; set; }

        public static void Init() {
            Mapping = new Dictionary<UInt32, Dictionary<UInt32, UInt32>>();
            ReverseStringMappings = new Dictionary<string, UInt32>();
        }

        public static void Load()
        {
            if (!IsInitialzed)
            {
                Init();
            }

            string loadPath = null;
            try
            {
                loadPath = Path.Combine(PluginCore.AssemblyDirectory, "data/test.json");
            }
            catch (Exception ex) {
                PluginCore.Log(ex);
            }

            if (loadPath == null) {
                PluginCore.WriteToChat("Database at path " + loadPath + " doesn't exist. Database not loaded.");
                return;
            }
            
            try
            {
                new Thread(() => {
                    Thread.CurrentThread.IsBackground = true;

                    IsLoading = true;
                    o = DoLoad(loadPath);
                    IsLoading = false;

                    // WIP
                    if (o == null)
                    {
                        PluginCore.WriteToChat("Failed to load for some reason");
                        IsLoaded = false;
                    }
                    else
                    {
                        PluginCore.WriteToChat("Database loaded!");
                        IsLoaded = true;
                    }
                }).Start();

            }
            catch (Exception ex)
            {
                PluginCore.Log(ex);
            }
        }

        private static JObject DoLoad(string path)
        {
            JObject retval = null;

            try
            {
                using (StreamReader reader = File.OpenText(path))
                {
                    var elapsed = System.Diagnostics.Stopwatch.StartNew();

                    retval = (JObject)JToken.ReadFrom(new JsonTextReader(reader));

                    elapsed.Stop();
                    var ms = elapsed.ElapsedMilliseconds;
                    PluginCore.WriteToChat("Successfully parsed JSON database in " + ms.ToString() + "ms.");

                    return retval;
                }
            } catch (Exception ex) { 
                PluginCore.Log(ex);
            }

            return retval;
        }

        public static void Unload()
        {
            Init();
            IsLoaded = false;
            IsLoading = false;
        }

        public static void Parse()
        {
            // TODO;
            int max = 10;
            UInt32 key; // Re-used as we parse

            try
            {
                foreach (var item in o.Properties())
                {
                    max--;

                    if (max <= 0)
                    {
                        break;
                    }

                    // Do second-level first
                    JObject child = (JObject)o[item.Name];
                    Dictionary<UInt32, UInt32> sub = new Dictionary<uint, uint>();

                    foreach (var subitem in child.Properties())
                    {
                        key = UInt32.Parse(subitem.Name, NumberStyles.HexNumber);
                        PluginCore.WriteToChat("Adding sub-level mapping for 0x" + key.ToString("X8"));

                        // Get array members for this item
                        JArray x = (JArray)subitem.Value;

                        if (x.Count != 1)
                        {
                            PluginCore.WriteToChat("Unexpected number of items in array. Skipping.");
                            continue;
                        }

                        // Add to StringMapping and use ID
                        // TODO: Is ToString the best way to get the value out?
                        sub.Add(key, GetOrSetStringMapping(x[0].ToString()));

                        // Increment ahead of our next value
                        lastStringMappingKey++;
                    }

                    // Then add to top level
                    key = UInt32.Parse(item.Name, NumberStyles.HexNumber);
                    PluginCore.WriteToChat("Adding top-level mapping for 0x" + key.ToString("X8"));
                    Mapping.Add(key, sub);
                }
            }
            catch (Exception ex)
            {
                PluginCore.Log(ex);
            }

            PluginCore.WriteToChat("Mapping.Count after parse is " + Mapping.Count.ToString());
            PluginCore.WriteToChat("ReverseStringMappings.Count after parse is " + ReverseStringMappings.Count.ToString());
        }

        private static UInt32 GetOrSetStringMapping(string value)
        {
            if (ReverseStringMappings.ContainsKey(value))
            {
                return ReverseStringMappings[value];
            }

            ReverseStringMappings.Add(value, lastStringMappingKey++);

            return lastStringMappingKey;
        }

        internal static void Cleanup()
        {
            Mapping.Clear();
            ReverseStringMappings.Clear();
        }
    }
}
