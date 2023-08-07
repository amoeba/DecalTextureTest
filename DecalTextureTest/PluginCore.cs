using System;
using System.IO;
using System.Reflection;
using System.Timers;

using Decal.Adapter;

namespace DecalTextureTest
{
    [FriendlyName("DecalTextureTest")]
    public class PluginCore : PluginBase
    {
        private DebugUI ui;
        public Timer timer;

        public static string AssemblyDirectory { get; internal set; }

        protected void FilterSetup(string assemblyDirectory)
        {
            AssemblyDirectory = assemblyDirectory;
        }

        /// <summary>
        /// Called when your plugin is first loaded.
        /// </summary>
        protected override void Startup()
        {
            try
            { 
                CoreManager.Current.CharacterFilter.LoginComplete += CharacterFilter_LoginComplete;

                ui = new DebugUI();
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        private void CharacterFilter_LoginComplete(object sender, EventArgs e)
        {
            try {
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        protected override void Shutdown()
        {
            try
            {
                CoreManager.Current.CharacterFilter.LoginComplete -= CharacterFilter_LoginComplete;

                ui.Dispose();
                timer.Dispose();
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        public static void ShowMessage(string message)
        {
            ExampleUI tempHud = new ExampleUI(message);
            Timer timer = new Timer(3000);
            timer.Elapsed += (s, e) =>
            {
                timer.Stop();
                timer.Dispose();
                tempHud.Dispose();

                CoreManager.Current.Actions.AddChatText("Elapsed", 1);
            };
            timer.Start();
        }

        #region logging
        /// <summary>
        /// Log an exception to log.txt in the same directory as the plugin.
        /// </summary>
        /// <param name="ex"></param>
        internal static void Log(Exception ex)
        {
            Log(ex.ToString());
        }

        /// <summary>
        /// Log a string to log.txt in the same directory as the plugin.
        /// </summary>
        /// <param name="message"></param>
        internal static void Log(string message)
        {
            try
            {
                File.AppendAllText(System.IO.Path.Combine(AssemblyDirectory, "log.txt"), $"{message}\n");
            }
            catch
            {
                try
                {
                    CoreManager.Current.Actions.AddChatText(Assembly.GetAssembly(typeof(PluginCore)).Location, 1);
                    CoreManager.Current.Actions.AddChatText(message, 1);
                }
                catch { }
            }
        }
        #endregion // logging
    }
}
