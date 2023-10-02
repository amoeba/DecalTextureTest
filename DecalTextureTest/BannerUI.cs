using System;
using System.Numerics;

using ImGuiNET;
using UtilityBelt.Service;
using UtilityBelt.Service.Views;
using static System.Net.Mime.MediaTypeNames;

namespace DecalTextureTest
{
    internal class BannerUI : IDisposable
    {
        private readonly Hud hud;
        private string message;
        private double t0;


        public BannerUI(string _message)
        {
            message = _message;

            hud = UBService.Huds.CreateHud("ExampelUI");
            hud.ShowInBar = true;
            hud.Visible = true;

            if (!Settings.IsPluginInDebugMode)
            {
                hud.WindowSettings = ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoDecoration;
            }

            hud.OnRender += Hud_OnRender;
            hud.OnPreRender += Hud_OnPreRender;

            t0 = ImGui.GetTime();
        }

        private void Hud_OnPreRender(object sender, EventArgs e)
        {
            try
            {
                var v = ImGui.GetMainViewport();
                var vs = v.Size;
                var c = v.GetCenter();
                ImGui.SetNextWindowPos(new Vector2(c.X - Settings.BannerWidthPx / 2, vs.Y / 5));
                ImGui.SetNextWindowSize(new Vector2(Settings.BannerWidthPx, Settings.BannerHeightPx), ImGuiCond.Appearing);
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
                double s = Settings.BannderDurationMS / 1000;
                double t = Math.Min(Math.Max((Math.Sin(t1) * s) / s, 0.0f), 1.0f);
                uint c = ImGui.Vec4ToCol(new Vector4(1.0f, 1.0f, 1.0f, (float)t));

                // Center text
                ImGui.PushFont(PluginCore.font);
                Vector2 text_size = ImGui.CalcTextSize(message);
                ImGui.PopFont(); 
                Vector2 center = p0 + (p1 - p0) / 2;
                Vector2 offset = new Vector2(-text_size.X / 2, -text_size.Y / 2);

                // Debug mode
                if (Settings.IsPluginInDebugMode)
                {
                    drawList.AddLine(new Vector2(p0.X + (p1.X - p0.X) / 2, p0.Y), new Vector2(p0.X + (p1.X - p0.X) / 2, p1.Y), 0xFFFFFFFF);
                    drawList.AddLine(new Vector2(p0.X, p0.Y + (p1.Y - p0.Y) / 2), new Vector2(p1.X, p0.Y + (p1.Y - p0.Y) / 2), 0xFFFFFFFF);
                }

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