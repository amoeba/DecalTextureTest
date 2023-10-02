using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading;

namespace DecalTextureTest
{
    internal static class Database
    {
        public static bool IsInitialzed = false;
        public static bool IsLoaded = false;
        public static bool IsLoading = false;
        public static JObject o = null; // TODO: Eventually get rid of this once I figure out parsing
        public static Dictionary<UInt32, Dictionary<UInt32, string>> Mapping { get; set; }
        public static Dictionary<UInt32, string> StringMappings { get; set; }

        public static void Init() {
            Mapping = new Dictionary<UInt32, Dictionary<UInt32, string>>();
            StringMappings = new Dictionary<UInt32, string>();
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
                loadPath = Path.Combine(PluginCore.AssemblyDirectory, "data/regions.json");
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
            foreach (var item in o.Properties())
            {
                PluginCore.WriteToChat(item.Name);
                max--;

                if (max <= 0)
                {
                    return;
                }
            }
        }
    }
}
