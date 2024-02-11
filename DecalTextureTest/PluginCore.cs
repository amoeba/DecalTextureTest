using System;
using System.IO;
using System.Reflection;

using Decal.Adapter;
using Decal.Adapter.Wrappers;
using ImGuiNET;

using Timer = System.Timers.Timer;

namespace DecalTextureTest
{
    [FriendlyName("DecalTextureTest")]
    public class PluginCore : PluginBase
    {
        public static bool isInPortalSpace = false;

        // Imgui
        private PluginUI pluginUI;
        public static ImFontPtr font;

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
                CoreManager.Current.CharacterFilter.ChangePortalMode += CharacterFilter_ChangePortalMode;
                tracker.LandcellChangedEvent += Tracker_LandcellChangedEvent; ;
                tracker.LandblockChangedEvent += Tracker_LandblockChangedEvent; ; ;
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
                CoreManager.Current.CharacterFilter.ChangePortalMode -= CharacterFilter_ChangePortalMode;
                tracker.LandcellChangedEvent -= Tracker_LandcellChangedEvent;

                // UI
                if (pluginUI != null) pluginUI.Dispose();
                CleanUpImgui();

                // Tracker
                if (tracker != null) tracker.Dispose();
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

        private void CharacterFilter_ChangePortalMode(object sender, ChangePortalModeEventArgs e)
        {
            if (e.Type == PortalEventType.EnterPortal)
            {
                isInPortalSpace = true;
            }
            else if (e.Type == PortalEventType.ExitPortal)
            {
                isInPortalSpace = false;

                Location l = new Location(CoreManager.Current.Actions.Landcell);

                if (l.IsIndoors())
                {
                    ShowMessage("Dungeon: " + l.ToString());
                }
                else
                {
                    ShowMessage("Landscape: " + l.ToString());
                }
            }
        }

        private void Tracker_LandblockChangedEvent(object sender, LandblockChangedEventArgs e)
        {
            if (!Settings.IsPluginEnabled)
            {
                return;
            }

            if (isInPortalSpace)
            {
                return;
            }

            if (Settings.IsPluginInDebugMode)
            {
                WriteToChat("Tracker_LandblockChangedEvent: 0x" + e.Landblock.ToString("X8"));
            }

            if (!Settings.ShouldShowBanner || !Settings.ShouldNotifyOnLandblockChanged)
            {
                return;
            }

            ShowMessage("Landblock: 0x" + e.Landblock.ToString("X8"));
        }

        private void Tracker_LandcellChangedEvent(object sender, LandcellChangedEventArgs e)
        {
            if (!Settings.IsPluginEnabled)
            {
                return;
            }

            if (isInPortalSpace)
            {
                return;
            }

            if (Settings.IsPluginInDebugMode)
            {
                WriteToChat("Tracker_LandcellChangedEvent: 0x" + e.Landcell.ToString("X8"));
            }

            if (!Settings.ShouldShowBanner || !Settings.ShouldNotifyOnLandcellChanged)
            {
                return;
            }

            ShowMessage("Landcell: 0x" + e.Landcell.ToString("X8"));
        }

        public static void ShowMessage(string message, bool destroy=false)
        {
            try
            {
                // TODO: Handle the case where ShowMessage is called more often than the pollTimer
                BannerUI hud = new BannerUI(message);

                Timer timer = new Timer(Settings.BannderDurationMS);

                timer.Elapsed += (s, e) =>
                {
                    timer.Stop();
                    timer.Dispose();
                    timer = null;
                    hud.Dispose();
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
                Log("SetUpImgui, start:");

                SetUpImguiFonts();
                //UtilityBelt.Service.UBService

                Log("SetUpImgui, end.");
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        private void SetUpImguiFonts()
        {
            try
            {
                string font_tahoma = AssemblyDirectory + "\\resources\\fonts\\Tahoma.ttf";
                string font_hylia = AssemblyDirectory + "\\resources\\fonts\\HyliaSerifBeta-Regular.otf";

                SetUpImguiFontLoad(font_tahoma);
                SetUpImguiFontLoad(font_hylia);
            } 
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        private void SetUpImguiFontLoad(string path)
        {
            if (!File.Exists(path))
            {
                Log("Skipped trying to load font " + path + " because the file didn't exist.");

                return;
            }

            UtilityBelt.Service.UBService.Huds.FontManager.RegisterFont(path);
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

        public static void WriteToChat(string message)
        {
            CoreManager.Current.Actions.AddChatText(message, 19);
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
