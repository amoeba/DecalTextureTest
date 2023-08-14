using System;
using System.IO;
using System.Reflection;

using Decal.Adapter;
using Decal.Adapter.Wrappers;
using ImGuiNET;
using static DecalTextureTest.LandcellTracker;
using Timer = System.Timers.Timer;

namespace DecalTextureTest
{
    [FriendlyName("DecalTextureTest")]
    public class PluginCore : PluginBase
    {
        public Timer timer;
        public static int duration_ms = 3000;

        // Imgui
        private PluginUI pluginUI;
        private DebugUI debugUI;
        public static ImFontPtr font;
        public static bool isDemoOpen = false;
        public static bool isDebugMode = false;
        public static bool showRulers = false;
        public static bool isDebugUIOpen = false;
        public static bool isEnabled = false;

        // Tracking
        LandcellTracker tracker;

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
                // UI
                SetUpImgui();
                pluginUI = new PluginUI();

                // Tracker
                tracker = new LandcellTracker();

                // Events
                CoreManager.Current.CharacterFilter.LoginComplete += CharacterFilter_LoginComplete;
                tracker.LandcellChangedEvent += Tracker_LandcellChangedEvent; ;
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
                // Events
                CoreManager.Current.CharacterFilter.LoginComplete -= CharacterFilter_LoginComplete;
                tracker.LandcellChangedEvent -= Tracker_LandcellChangedEvent;

                // UI
                if (pluginUI != null) pluginUI.Dispose();
                CleanUpImgui();

                // Tracker
                if (timer != null) timer.Dispose();
                if (tracker!= null) tracker.Dispose();
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        private void CharacterFilter_LoginComplete(object sender, EventArgs e)
        {
            try {
                // NYE
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        private void Tracker_LandcellChangedEvent(object sender, LandcellChangedEventArgs e)
        {
            if (!isEnabled)
            {
                return;
            }

            ShowMessage((e.Landcell * 1).ToString());
        }

        public static void ShowMessage(string message, bool destroy=false)
        {
            try
            {
                // TODO: Handle the case where ShowMessage is called more often than the timer
                BannerUI tempHud = new BannerUI(message);

                if (!destroy) { return; }

                Timer timer = new Timer(duration_ms);

                timer.Elapsed += (s, e) =>
                {
                    timer.Stop();
                    timer.Dispose();
                    tempHud.Dispose();
                };

                timer.Start();
            } catch (Exception ex)
            {
                Log(ex);
            }
        }

        private void SetUpImgui()
        {
            try
            {
                // Temporary fix for UB not dealing with fonts right
                // Don't load our custom font is this is a hot reload
                if (CoreManager.Current.CharacterFilter.LoginStatus >= 1)
                {
                    Log("Skipping ImGui setup (font loading) because HMR.");

                    return;
                }

                string font_path = AssemblyDirectory + "\\HyliaSerifBeta-Regular.otf";
                ImGuiIOPtr io = ImGui.GetIO();
                font = io.Fonts.AddFontFromFileTTF(font_path, 46.0f);
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        private void CleanUpImgui()
        {
            try
            {
                font.Destroy();
            }           
            catch (Exception ex)
            {
                Log(ex);
            }
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
