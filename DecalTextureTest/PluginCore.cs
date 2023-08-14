﻿using System;
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
        public static int duration_ms = 3000;
        public static bool isInPortalSpace = false;

        // Imgui
        private PluginUI pluginUI;
        private static DebugUI debugUI;
        public static ImFontPtr font;
        internal static bool isDemoOpen = false;
        internal static bool isDebugUIEnabled = false;
        internal static bool isEnabled = false;
        internal static bool isDebugModeEnabled;

        // Tracking
        LandcellTracker tracker;

        // Banner
        internal static int bannerWidth = 600;
        internal static int bannerHeight = 300;

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

        private void Tracker_LandcellChangedEvent(object sender, LandcellChangedEventArgs e)
        {
            if (!isEnabled)
            {
                return;
            }

            if (isInPortalSpace)
            {
                WriteToChat("isInPortalSpace");
                return;
            }

            ShowMessage((e.Landcell * 1).ToString());
        }

        public static void ShowMessage(string message, bool destroy=false)
        {
            try
            {
                // TODO: Handle the case where ShowMessage is called more often than the pollTimer
                BannerUI hud = new BannerUI(message);

                Timer timer = new Timer(duration_ms);

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

        internal static void EnableDebugUI()
        {
            try
            {
                debugUI = new DebugUI();
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        internal static void DisableDebugUI()
        {
            try
            {
                if (debugUI != null)
                {
                    debugUI.Dispose();
                }

                debugUI = null;
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }
    }
}
