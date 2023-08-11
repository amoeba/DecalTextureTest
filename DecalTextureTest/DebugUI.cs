using System;
using System.Numerics;

using Decal.Adapter;
using ImGuiNET;
using UtilityBelt.Service;
using UtilityBelt.Service.Views;

namespace DecalTextureTest
{
    internal class DebugUI : IDisposable
    {
        private readonly Hud hud;
        private bool isDemoOpen = false;
        private bool isDebugMode = false;
        private bool showRulers = false;
        private bool isDebugUIOpen = false;

        public DebugUI()
        {
            hud = UBService.Huds.CreateHud("DecalTextureTest");
            hud.ShowInBar = true;
            hud.Visible = true;
            //hud.WindowSettings = ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoDecoration;

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

                ImGui.ShowDemoWindow(ref isDemoOpen);
                ShowDebugUI(isDebugUIOpen);

                if (ImGui.BeginTabBar("MainTabBar"))
                {
                    if (ImGui.BeginTabItem("Options"))
                    {
                        ImGui.Text("TODO");
                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Font"))
                    {
                        ImGui.Text("Font...");
                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Debug"))
                    {
                        ImGui.Checkbox("Debug mode", ref isDebugMode);

                        if (ImGui.Button("Show Debug UI"))
                        {
                            isDebugUIOpen = false;
                        }

                        ImGui.Checkbox("Show rulers", ref showRulers);
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