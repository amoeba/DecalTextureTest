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
                ImGui.SetNextWindowSize(new Vector2(200, 200), ImGuiCond.Appearing);
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
                ImGui.Text("Debug UI");
                if (ImGui.Button("Spawn text"))
                {
                    CoreManager.Current.Actions.AddChatText("Spawned", 1);
                    PluginCore.ShowMessage("Test!");

                }
                Vector2 size_one = ImGui.CalcTextSize("Hello World");
                ImGui.PushFont(PluginCore.font);
                Vector2 size_two = ImGui.CalcTextSize("Hello World");
                ImGui.Text("Text in Custom Font");
                ImGui.PopFont();
                ImGui.Text("Size One is " + size_one.X.ToString());
                ImGui.Text("Size Two is " + size_two.X.ToString());
            }
            catch (Exception ex)
            {
                PluginCore.Log(ex);
            }
        }

        public void Dispose()
        {
            hud.Dispose();
        }
    }
}