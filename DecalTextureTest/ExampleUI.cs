﻿using System;
using System.Drawing;
using System.IO;
using System.Numerics;

using Decal.Adapter;
using ImGuiNET;
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
                        using (var dbmp = new Bitmap(manifestResourceStream))
                        {
                            texture = new ManagedTexture(dbmp);
                        }
                    }
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
                CoreManager.Current.Actions.AddChatText("Hud_OnRender()", 1);

                ImGui.InputTextMultiline("Test Text", ref TestText, 5000, new Vector2(400, 150));

                if (ImGui.Button("Print Test Text"))
                {
                    OnPrintTestTextButtonPressed();
                }

                ImGui.SameLine();

                if (ImGui.Button("Reset Test Text"))
                {
                    TestText = DefaultTestText;
                }

                ImGui.Image(texture.TexturePtr, new Vector2(200, 200));
            }
            catch (Exception ex)
            {
                PluginCore.Log(ex);
            }
        }

        /// <summary>
        /// Called when our print test text button is pressed
        /// </summary>
        private void OnPrintTestTextButtonPressed()
        {
            var textToShow = $"Test Text:\n{TestText}";

            CoreManager.Current.Actions.AddChatText(textToShow, 1);
            UBService.Huds.Toaster.Add(textToShow, ToastType.Info);
        }

        public void Dispose()
        {
            hud.Dispose();
        }
    }
}