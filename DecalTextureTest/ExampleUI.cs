using System;
using System.Numerics;

using ImGuiNET;
using UtilityBelt.Service;
using UtilityBelt.Service.Views;


namespace DecalTextureTest
{
    internal class ExampleUI : IDisposable
    {
        private readonly Hud hud;
        private string message;

        public ExampleUI(string _message)
        {
            message = _message;

            hud = UBService.Huds.CreateHud("ExampelUI");
            hud.ShowInBar = true;
            hud.Visible = true;
            hud.WindowSettings = ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoDecoration;

            hud.OnRender += Hud_OnRender;
            hud.OnPreRender += Hud_OnPreRender;
        }

        private void Hud_OnPreRender(object sender, EventArgs e)
        {
            try
            {
                // TODO: This works, but factor out and cleanup
                float w = 300;
                float h = 200;
                var io = ImGui.GetIO();
                var v = ImGui.GetMainViewport();
                var c = v.GetCenter();
                ImGui.SetNextWindowPos(new Vector2(c.X - w/2, c.Y - w/2));
                ImGui.SetNextWindowSize(new Vector2(w, h), ImGuiCond.Appearing);
                
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
                ImGui.InvisibleButton("renderbutton", ImGui.GetContentRegionAvail());
                Vector2 p0 = ImGui.GetItemRectMin();
                Vector2 p1 = ImGui.GetItemRectMax();
                ImDrawListPtr drawList = ImGui.GetWindowDrawList();
                drawList.AddText(ImGui.GetFont(), 40, new Vector2(p0.X, p0.Y), 0xFFFFFFFF, message);
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