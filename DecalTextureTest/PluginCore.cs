using System;
using System.IO;
using System.Reflection;
using System.Timers;

using Decal.Adapter;
using ImGuiNET;

namespace DecalTextureTest
{
    [FriendlyName("DecalTextureTest")]
    public class PluginCore : PluginBase
    {
        public Timer timer;
        public static int duration_ms = 3000;

        // Imgui
        private DebugUI ui;
        public static ImFontPtr font;

        // Misc
        public static string AssemblyDirectory { get; internal set; }

        protected void FilterSetup(string assemblyDirectory)
        {
            AssemblyDirectory = assemblyDirectory;
        }

        protected override void Startup()
        {
            try
            { 
                CoreManager.Current.CharacterFilter.LoginComplete += CharacterFilter_LoginComplete;

                SetUpImgui();
                ui = new DebugUI();
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        private void SetUpImgui()
        {
            try
            {
                Log("Font is "+ font.ToString());
                string font_path = AssemblyDirectory + "\\HyliaSerifBeta-Regular.otf";
                ImGuiIOPtr io = ImGui.GetIO();
                font = io.Fonts.AddFontFromFileTTF(font_path, 46.0f);
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

                if (ui != null) ui.Dispose();
                if (timer != null) timer.Dispose();
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        public static void ShowMessage(string message)
        {
            ExampleUI tempHud = new ExampleUI(message);
            Timer timer = new Timer(duration_ms);
            timer.Elapsed += (s, e) =>
            {
                timer.Stop();
                timer.Dispose();
                tempHud.Dispose();

                CoreManager.Current.Actions.AddChatText("Elapsed", 1);
            };
            timer.Start();
        }

        internal static void Log(Exception ex)
        {
            Log(ex.ToString());
        }

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
    }
}
