using System;
using System.Numerics;

using ImGuiNET;
using UtilityBelt.Service;
using UtilityBelt.Service.Views;
using static System.Net.Mime.MediaTypeNames;

namespace DecalTextureTest
{
    internal class TextDebug : IDisposable
    {
        private readonly Hud hud;
        private int width = 300;
        private int height = 200;

        public TextDebug()
        {

            hud = UBService.Huds.CreateHud("TextDebug");
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
                var v = ImGui.GetMainViewport();
                var c = v.GetCenter();
                ImGui.SetNextWindowPos(new Vector2(c.X - width / 2, c.Y - width / 2));
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
                Vector2 size = ImGui.GetContentRegionAvail();
                ImGui.InvisibleButton("renderbutton", size);
                Vector2 p0 = ImGui.GetItemRectMin();
                Vector2 p1 = ImGui.GetItemRectMax();
                ImDrawListPtr drawList = ImGui.GetWindowDrawList();
                drawList.PushClipRect(p0, p1);

                // Do things
                string text = "Test Message";

                // Text size
                ImGui.PushFont(PluginCore.font);
                Vector2 text_size = ImGui.CalcTextSize(text);
                ImGui.PopFont();

                Vector2 center = p0 + (p1 - p0) / 2;
                Vector2 offset = new Vector2(-text_size.X / 2, -text_size.Y / 2);
                
                // DEBUG: Find center
                drawList.AddLine(new Vector2(p0.X + (p1.X-p0.X)/2, p0.Y), new Vector2(p0.X + (p1.X - p0.X) / 2, p1.Y), 0xFFFFFFFF);
                drawList.AddLine(new Vector2(p0.X, p0.Y + (p1.Y - p0.Y)/2), new Vector2(p1.X, p0.Y + (p1.Y - p0.Y) / 2), 0xFFFFFFFF);

                drawList.AddText(PluginCore.font, PluginCore.font.FontSize, center + offset, 0xFFFFFFFF, text);

                drawList.PopClipRect();
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