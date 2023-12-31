﻿using System;
using System.Drawing;
using System.Numerics;

using Decal.Adapter;
using Decal.Adapter.Wrappers;
using ImGuiNET;
using UtilityBelt.Service;
using UtilityBelt.Service.Views;

namespace DecalTextureTest
{
    internal class PluginUI : IDisposable
    {
        private readonly UtilityBelt.Service.Views.Hud hud;

        public PluginUI(bool _showInBar = true, bool _visible = true)
        {
            hud = UBService.Huds.CreateHud("DecalTextureTest");
            hud.ShowInBar = _showInBar;
            hud.Visible = _visible;

            hud.OnRender += Hud_OnRender;
            hud.OnPreRender += Hud_OnPreRender;
        }

        private void Hud_OnPreRender(object sender, EventArgs e)
        {
            try
            {
                ImGui.SetNextWindowPos(new Vector2(100, 100), ImGuiCond.Appearing);
                ImGui.SetNextWindowSize(new Vector2(300, 200), ImGuiCond.Appearing);
            }
            catch (Exception ex)
            {
                PluginCore.Log(ex);
            }
        }

        private void Hud_OnRender(object sender, EventArgs e)
        {
            try
            {

                //ImGui.ShowDemoWindow(ref isDemoOpen);
                ShowDebugUI(Settings.IsDebugUIOpen);

                if (ImGui.BeginTabBar("MainTabBar"))
                {
                    if (ImGui.BeginTabItem("Options"))
                    {
                        ImGui.Checkbox("Enable Plugin", ref Settings.IsPluginEnabled);
                        ImGui.Checkbox("Enable Banners", ref Settings.ShouldShowBanner);
                        ImGui.Checkbox("Notify on landblock change", ref Settings.ShouldNotifyOnLandblockChanged);
                        ImGui.Checkbox("Notify on landcell change", ref Settings.ShouldNotifyOnLandcellChanged);

                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Font"))
                    {
                        ImGui.Text("Font...");
                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Debug"))
                    {
                        ImGui.Checkbox("Enable Debug Mode", ref Settings.IsPluginInDebugMode);

                        if (ImGui.Button("Spawn Test Banner"))
                        {
                            PluginCore.ShowMessage("Example");
                        }

                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("About"))
                    {
                        ImGui.Text("About...");
                        ImGui.EndTabItem();
                    }
                    ImGui.EndTabBar();
                }
            }
            catch (Exception ex)
            {
                PluginCore.Log(ex);
            }
        }

        private void ShowDebugUI(bool isDebugUIOpen)
        {
            // TODO
        }

        public void Dispose()
        {
            hud.Dispose();
        }
    }
}