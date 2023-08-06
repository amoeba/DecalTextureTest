using System;
using System.Drawing;
using System.IO;
using System.Numerics;

using Decal.Adapter;
using ImGuiNET;
using Microsoft.DirectX.Direct3D;
using UtilityBelt.Service;
using UtilityBelt.Service.Views;

namespace DecalTextureTest
{
    internal class ExampleUI : IDisposable
    {
        /// <summary>
        /// The UBService Hud
        /// </summary>
        private readonly Hud hud;

        /// <summary>
        /// The default value for TestText.
        /// </summary>
        public const string DefaultTestText = "Some Test Text";

        /// <summary>
        /// Some test text. This value is used to the text input in our UI.
        /// </summary>
        public string TestText = DefaultTestText.ToString();

        public ManagedTexture texture;

        public ExampleUI()
        {
            CoreManager.Current.Actions.AddChatText("ExampleUI()", 1);

            // Create a new UBService Hud
            hud = UBService.Huds.CreateHud("DecalTextureTest");

            // set to show our icon in the UBService HudBar
            hud.ShowInBar = true;
            hud.Visible = true;
            hud.WindowSettings = ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoDecoration;

            // subscribe to the hud render event so we can draw some controls
            hud.OnRender += Hud_OnRender;
            hud.OnPreRender += Hud_OnPreRender;
        }

        private void Hud_OnPreRender(object sender, EventArgs e)
        {
            try
            {
                CoreManager.Current.Actions.AddChatText("Hud_OnPreRender()", 1);

                if (texture == null)
                {
                    using (Stream manifestResourceStream = GetType().Assembly.GetManifestResourceStream("DecalTextureTest.test.png"))
                    {
                        PluginCore.Log("Starting to enumermate manifest resources...");
                        foreach (string name in GetType().Assembly.GetManifestResourceNames())
                        {
                            PluginCore.Log(name);
                        }
                        PluginCore.Log("...Done enumerating manifest resourc streams;");

                        // WIP
                        Texture tx = new Texture(texture.TexturePtr);

                        using (var dbmp = new Bitmap(manifestResourceStream))
                        {
                            texture = new ManagedTexture(dbmp);
                        }
                    }
                } else
                {
                    CoreManager.Current.Actions.AddChatText(texture.TexturePtr.ToString(), 1);

                }
                ImGui.SetNextWindowPos(new Vector2(100, 100), ImGuiCond.Appearing);
                ImGui.SetNextWindowSize(new Vector2(200, 200), ImGuiCond.Appearing);
            }
            catch (Exception ex)
            {
                PluginCore.Log(ex);
            }
        }

        /// <summary>
        /// Called every time the ui is redrawing.
        /// </summary>
        private void Hud_OnRender(object sender, EventArgs e)
        {
            try
            {
                ImGui.Text("Title Goes Here");
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