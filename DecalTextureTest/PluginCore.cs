﻿using System;
using System.IO;
using System.Reflection;
using System.Timers;
using AcClient;
using Decal.Adapter;
using Decal.Adapter.Wrappers;
using ImGuiNET;
using Timer = System.Timers.Timer;

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
                CoreManager.Current.CharacterFilter.ChangePortalMode += CharacterFilter_ChangePortalMode;

                SetUpImgui();
                ui = new DebugUI();
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
                CoreManager.Current.CharacterFilter.ChangePortalMode -= CharacterFilter_ChangePortalMode;

                if (ui != null) ui.Dispose();
                if (timer != null) timer.Dispose();
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

        private void CharacterFilter_ChangePortalMode(object sender, ChangePortalModeEventArgs e)
        {
            if (e.Type == PortalEventType.EnterPortal)
            {
                Log("Portal Entered");
            }
            else if (e.Type == PortalEventType.ExitPortal)
            {
                Log("Portal exited");

                // FIXME: This is just temporary until Tracker.cs is built out
                Location l = new Location(CoreManager.Current.Actions.Landcell);

                if (l.IsIndoors())
                {
                    ShowMessage("Holtburg Town Hall");
                } else
                {
                    ShowMessage("Holtburg");
                }
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
            };
            timer.Start();
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
