namespace AllinOne.CameraDistance
{
    using AllinOne.Menu;
    using AllinOne.Update;
    using AllinOne.Variables;
    using Ensage;
    using Ensage.Common.Menu;
    using System;

    internal class Zoom
    {
        private const uint VkCtrl = 0x11;
        private const uint WmMousewheel = 0x020A;
        private static readonly ConVar ZoomVar = Game.GetConsoleVar("dota_camera_distance");
        private static readonly ConVar RenderVar = Game.GetConsoleVar("r_farz");

        public static void ChangeDistance(WndEventArgs args)
        {
            if (!OnUpdate.CanUpdate() || args.Msg != WmMousewheel || !Game.IsKeyDown(VkCtrl)) return;
            var delta = (short) ((args.WParam >> 16) & 0xFFFF);
            var zoomValue = ZoomVar.GetInt();
            if (delta < 0)
                zoomValue += 50;
            if (delta > 0)
                zoomValue -= 50;
            if (zoomValue < 1134)
                zoomValue = 1134;
            ZoomVar.SetValue(zoomValue);
            MenuVar.CameraDistance.SetValue(new Slider(zoomValue, 1134, 2000));
            args.Process = false;
        }

        public static void Slider_ValueChanged(OnValueChangeEventArgs e)
        {
            Game.GetConsoleVar("fog_enable").SetValue(0);
            ZoomVar.SetValue(e.GetNewValue<Slider>().Value);
            RenderVar.SetValue(2 * e.GetNewValue<Slider>().Value);
            ZoomVar.RemoveFlags(ConVarFlags.Cheat);
            RenderVar.RemoveFlags(ConVarFlags.Cheat);
        }

        public static void Load()
        {
            Game.GetConsoleVar("fog_enable").SetValue(0);
            ZoomVar.SetValue(MainMenu.MenuSettings.Item("cameradistance").GetValue<Slider>().Value);
            RenderVar.SetValue(2 * MainMenu.MenuSettings.Item("cameradistance").GetValue<Slider>().Value);

            ZoomVar.RemoveFlags(ConVarFlags.Cheat);
            RenderVar.RemoveFlags(ConVarFlags.Cheat);

            var var = Game.GetConsoleVar("dota_use_particle_fow");
            var.RemoveFlags(ConVarFlags.Cheat);
            var.SetValue(0);
        }
    }
}