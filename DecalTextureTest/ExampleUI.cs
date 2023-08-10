using System;
using System.Numerics;

using ImGuiNET;
using UtilityBelt.Service;
using UtilityBelt.Service.Views;
using static System.Net.Mime.MediaTypeNames;

namespace DecalTextureTest
{
    internal class ExampleUI : IDisposable
    {
        private readonly Hud hud;
        private string message;
        private double t0;
        private int width = 600;
        private int height = 300;

        public ExampleUI(string _message)
        {
            message = _message;

            hud = UBService.Huds.CreateHud("ExampelUI");
            hud.ShowInBar = true;
            hud.Visible = true;
            hud.WindowSettings = ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoDecoration;

            hud.OnRender += Hud_OnRender;
            hud.OnPreRender += Hud_OnPreRender;

            t0 = ImGui.GetTime();
        }

        private void Hud_OnPreRender(object sender, EventArgs e)
        {
            try
            {
                // TODO: This works, but factor out and cleanup
                var io = ImGui.GetIO();
                var v = ImGui.GetMainViewport();
                var vs = v.Size;
                var c = v.GetCenter();
                ImGui.SetNextWindowPos(new Vector2(c.X - width / 2, vs.Y / 5));
                ImGui.SetNextWindowSize(new Vector2(width, height), ImGuiCond.Appearing);
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

                // Text fading
                // FIXME: Factor out most of this
                // FIXME: Figure out if we can properly center our text (i.e. do 
                //        (font metrics)
                double t1 = ImGui.GetTime() - t0;

                // Messy bit of code to smoothly move from 0-1 and back to 0
                double s = PluginCore.duration_ms / 1000;
                double t = Math.Min(Math.Max((Math.Sin(t1) * s) / s, 0.0f), 1.0f);
                uint c = ImGui.Vec4ToCol(new Vector4(1.0f, 1.0f, 1.0f, (float)t));

                // Center text
                ImGui.PushFont(PluginCore.font);
                Vector2 text_size = ImGui.CalcTextSize(message);
                ImGui.PopFont(); 
                Vector2 center = p0 + (p1 - p0) / 2;
                Vector2 offset = new Vector2(-text_size.X / 2, -text_size.Y / 2);

                // Draw text
                drawList.AddText(PluginCore.font, 40, center + offset, c, message);
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