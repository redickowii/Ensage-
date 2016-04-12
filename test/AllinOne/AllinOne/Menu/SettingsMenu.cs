namespace AllinOne.Menu
{
    using AllinOne.CameraDistance;
    using AllinOne.Menu;
    using Ensage.Common.Menu;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class SettingsMenu
    {
        private static readonly Slider CameraDistance = new Slider(1500, 1134, 2000);
        private static readonly Slider Frequency = new Slider(50, 10, 1000);

        public static void Load()
        {
            MainMenu.MenuSettings.AddItem(new MenuItem("dodge", "Dodge ?").SetValue(true));
            MainMenu.MenuSettings.AddItem(new MenuItem("dodgefrequency", "Dodge Frequency").SetValue(Frequency));
            MainMenu.MenuSettings.AddItem(new MenuItem("showatkrange", "Show attack range ?").SetValue(false));
            MainMenu.MenuSettings.AddItem(
                new MenuItem("expRange", "Exp Range").SetValue(false).SetTooltip("Show your experience range."));

            MainMenu.MenuSettings.AddItem(new MenuItem("cameradistance", "Camera Distance").SetValue(CameraDistance))
                .ValueChanged += (sender, arg) => { Zoom.Slider_ValueChanged(arg); };
        }

        public static void Update()
        {
            MenuVar.DodgeEnable = MainMenu.MenuSettings.Item("dodge").GetValue<bool>();
            MenuVar.DodgeFrequency = MainMenu.MenuSettings.Item("dodgefrequency").GetValue<Slider>().Value;
        }
    }
}