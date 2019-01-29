using Celeste;
using Celeste.Mod;
using Monocle;
using System;
using System.Reflection;

namespace Celeste.Mod.Solid
{
    public class SolidModule : EverestModule
    {

        // Only one alive module instance can exist at any given time.
        public static SolidModule Instance;

        public SolidModule()
        {
            Instance = this;
        }

        // If you don't need to store any settings, => null
        public override Type SettingsType => typeof(SolidSettings);
        public static SolidSettings Settings => (SolidSettings)Instance._Settings;

        // If you don't need to store any save data, => null
        public override Type SaveDataType => null;

        private static FieldInfo UsedHairColor = typeof(Player).GetField("UsedHairColor");
        private static FieldInfo NormalHairColor = typeof(Player).GetField("NormalHairColor");
        private static FieldInfo TwoDashesHairColor = typeof(Player).GetField("TwoDashesHairColor");

        // Set up any hooks, event handlers and your mod in general here.
        // Load runs before Celeste itself has initialized properly.
        public override void Load()
        {
            On.Celeste.Player.Update += Player_Update;
        }

        public override void Unload()
        {
            On.Celeste.Player.Update -= Player_Update;
        }

        private void Player_Update(On.Celeste.Player.orig_Update orig, Player self)
        {
            if (Settings.Enabled) {
                UsedHairColor.SetValue(null, Calc.HexToColor(Settings.Dash0Color));
                NormalHairColor.SetValue(null, Calc.HexToColor(Settings.Dash1Color));
                TwoDashesHairColor.SetValue(null, Calc.HexToColor(Settings.Dash2Color));
            } else {
                NormalHairColor.SetValue(null, Calc.HexToColor("AC3232"));
                TwoDashesHairColor.SetValue(null, Calc.HexToColor("FF6DEF"));
                UsedHairColor.SetValue(null, Calc.HexToColor("44B7FF"));
            }
            orig(self);
        }

    }
}
