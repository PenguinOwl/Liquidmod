using Celeste;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Celeste.Mod.Solid
{
    // If no SettingName is applied, it defaults to
    // modoptions_[typename without settings]_title
    // The value is then used to look up the UI text in the dialog files.
    // If no dialog text can be found, Everest shows a prettified mod name instead.
    [SettingName("modoptions_solidmodule_title")]
    public class SolidSettings : EverestModuleSettings
    {
        public bool Enabled { get; set; } = true;

        public string Dash0Color { get; set; } = "ffffff";

        public string Dash1Color { get; set; } = "ffffff";

        public string Dash2Color { get; set; } = "ffffff";

/*
        public bool Badeline { get; set; } = false;

        public bool BadelineFloat { get; set; } = false;

        [SettingRange(1, 100)]
        public int HairLength { get; set; } = 4;
*/
        // SettingName also works on props, defaulting to
        // modoptions_[typename without settings]_[propname]

        //Solid ON / OFF property with a default value.
        //public bool SolidSwitch { get; set; } = false;

        //[SettingIgnore] // Hide from the options menu, but still load / save it.
        //public string SolidHidden { get; set; } = "";

        //[SettingRange(0, 10)] // Allow choosing a value from 0 (inclusive) to 10 (inclusive).
        //public int SolidSlider { get; set; } = 5;

        //[SettingRange(0, 10)]
        //[SettingInGame(false)] // Only show this in the main menu.
        //public int SolidMainMenuSlider { get; set; } = 5;

        //[SettingRange(0, 10)]
        //[SettingInGame(true)] // Only show this in the in-game menu.
        //public int SolidInGameSlider { get; set; } = 5;

        //[YamlIgnore] // Don't load / save it, but show it in the options menu.
        //[SettingNeedsRelaunch] // Tell the user to restart for changes to take effect.
        //public bool LaunchInDebugMode
        //{
        //    get
        //    {
        //        return Settings.Instance.LaunchInDebugMode;
        //    }
        //    set
        //    {
        //        Settings.Instance.LaunchInDebugMode = value;
        //    }
        //}

        //public int SomethingWeird { get; set; } = 42;

        //// Custom entry creation methods are always called Create[propname]Entry
        //// and offer an alternative to overriding CreateModMenuSection in your module class.
        //public void CreateSomethingWeirdEntry(TextMenu menu, bool inGame)
        //{
        //    // Create your own menu entry here.
        //    // Maybe you want to create a toggle for an int property?
        //}

    }
}
