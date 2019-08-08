using System;
using System.Globalization;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using On.Celeste;

namespace Celeste.Mod.Liquid
{
    public class LiquidMod : EverestModule
    {

        // Only one alive module instance can exist at any given time.
        public static LiquidMod Instance;

        private static Color permcolor;

        private static FieldInfo UsedHairColor = typeof(Player).GetField("UsedHairColor");
        private static FieldInfo NormalHairColor = typeof(Player).GetField("NormalHairColor");
        private static FieldInfo TwoDashesHairColor = typeof(Player).GetField("TwoDashesHairColor");

        public LiquidMod()
        {
            Instance = this;
        }

        // If you don't need to store any settings, => null
        public override Type SettingsType => typeof(LiquidSettings);
        public static LiquidSettings Settings => (LiquidSettings)Instance._Settings;

        // If you don't need to store any save data, => null
        public override Type SaveDataType => null;
        //public static SolidSaveData SaveData => (SolidSaveData)Instance._SaveData;

        // Set up any hooks, event handlers and your mod in general here.
        // Load runs before Celeste itself has initialized properly.
        public override void Load()
        {
            On.Celeste.Player.GetTrailColor += GetTrailColor;
            //On.Celeste.PlayerHair.GetHairColor += GetHairColor;
            On.Celeste.TrailManager.Add_Entity_Color_float += AddTrail;
            On.Celeste.DeathEffect.Draw += Death;
            On.Celeste.Player.IntroRespawnBegin += Player_Respawn;
            On.Celeste.Player.Update += Player_Update;
            On.Celeste.PlayerHair.AfterUpdate += PlayerHair_AfterUpdate;
            On.Celeste.PlayerHair.Render += PlayerHair_Render;;
            //    On.Monocle.ParticleSystem.Emit_ParticleType_Vector2_float += ParticleSystem_Emit_ParticleType_Vector2_Float;
        }

        void PlayerHair_Render(On.Celeste.PlayerHair.orig_Render orig, PlayerHair self)
        {
            //int oldNum = self.Sprite.HairCount;
            //int newNum = oldNum - 4 + Settings.HairLength;
            //Player player = self.Entity as Player;
            //if (Settings.Enabled && newNum <= countCache)
            //{
                //self.Sprite.HairCount = newNum;
            //}
            orig(self);
            //self.Sprite.HairCount = oldNum;
        }


        //void ParticleSystem_Emit_ParticleType_Vector2_Float(On.Monocle.ParticleSystem.orig_Emit_ParticleType_Vector2_float orig, ParticleSystem self, ParticleType type, Vector2 position, float direction)
        //{
        //    if (Settings.Enabled)
        //    {
        //        Player.P_DashA.Color = Calc.HexToColor(Settings.Dash0Color);
        //        Player.P_DashB.Color = Calc.HexToColor(Settings.Dash1Color);
        //    }
        //    else
        //    {
        //        Player.P_DashA.Color = Calc.HexToColor("44B7FF");
        //        Player.P_DashB.Color = Calc.HexToColor("AC3232");
        //    }
        //    orig(self, type, position, direction);
        //}


        private void Player_Added(On.Celeste.Player.orig_Render orig, Player self)
        {
            //Logger.Log("A", self.GetType().ToString());
            //PlayerSpriteMode mode = self.Sprite.Mode;
            //self.Remove(self.Sprite);
            //if (Settings.Badeline)
            //{
            //    self.Sprite = new PlayerSprite(PlayerSpriteMode.Badeline);
            //}
            //else
            //{
            //    self.Sprite = new PlayerSprite(mode);
            //}
            //self.Remove(self.Hair);
            //self.Sprite.HairCount = getHairCount(self);
            //self.Add(self.Hair = new PlayerHair(self.Sprite));
            //self.Add(self.Sprite);

            orig(self);
        }

        static int countCache = 4;

        private void PlayerHair_AfterUpdate(On.Celeste.PlayerHair.orig_AfterUpdate orig, PlayerHair self)
        {
            //if ((self.Entity as BadelineOldsite) == null && ((self.Entity as Player) != null && getHairCount(self.Entity as Player) != HairCount))
            //{
            //    Player player = self.Entity as Player;
            //    player.Sprite.HairCount = getHairCount(self.Entity as Player);
            //    HairCount = getHairCount(self.Entity as Player);
            //    player.Remove(player.Hair);
            //    PlayerSpriteMode mode = self.Sprite.Mode;
            //    player.Remove(player.Sprite);
            //    player.Add(player.Hair = new PlayerHair(player.Sprite));
            //    player.Add(player.Sprite);
            //}
            //if ((self.Entity as BadelineOldsite) == null && (self.Entity as Player) != null)
            //{
            //    self.Sprite.HairCount = getHairCount(self.Entity as Player);
            //}


            //int oldNum = self.Sprite.HairCount;
            //int newNum = oldNum - 4 + Settings.HairLength;
            //Player player = self.Entity as Player;
            //if (Settings.Enabled)
            //{
            //    self.Sprite.HairCount = newNum;
            //}
            //orig(self);
            //self.Sprite.HairCount = oldNum;
            //countCache = newNum;
            Player player = self.Entity as Player;
            if (Settings.Enabled && player != null) {
                if (player.StateMachine.State == 5)
                {
                    player.Sprite.HairCount = 1;
                }
                else if (player.StateMachine.State != 19)
                {
                    player.Sprite.HairCount = ((player.Dashes > 1) ? 5 : 4);
                    player.Sprite.HairCount += Settings.HairLength - 4;
                }
                else if (player.StateMachine.State == 19)
                {
                    player.Sprite.HairCount = 7;
                }
            }
            orig(self);
        }


        private void Player_Update(On.Celeste.Player.orig_Update orig, Player self)
        {
            if (self.GetType().Name == "Ghost")
            {
                orig(self);
                return;
            }
            if (Settings.Enabled)
            {
                UsedHairColor.SetValue(null, Calc.HexToColor(Settings.Dash0Color));
                NormalHairColor.SetValue(null, Calc.HexToColor(Settings.Dash1Color));
                TwoDashesHairColor.SetValue(null, Calc.HexToColor(Settings.Dash2Color));
                Player.P_DashA.Color = Calc.HexToColor(Settings.Dash0Color);
                Player.P_DashB.Color = Calc.HexToColor(Settings.Dash1Color);
            }
            else
            {
                NormalHairColor.SetValue(null, Calc.HexToColor("AC3232"));
                TwoDashesHairColor.SetValue(null, Calc.HexToColor("FF6DEF"));
                UsedHairColor.SetValue(null, Calc.HexToColor("44B7FF"));
                Player.P_DashA.Color = Calc.HexToColor("44B7FF");
                Player.P_DashB.Color = Calc.HexToColor("AC3232");
            }
            orig(self);
        }
        //    if (((Entity)self as BadelineOldsite) != null)
        //    {
        //        self.Sprite.HairCount = getHairCount(self);
        //    }
        //    if (getHairCount(self) != HairCount && self.Sprite.Entity as Player != null)
        //    {
        //        self.Sprite.HairCount = getHairCount(self);
        //        HairCount = getHairCount(self);
        //        self.Remove(self.Hair);
        //        PlayerSpriteMode mode = self.Sprite.Mode;
        //        self.Remove(self.Sprite);
        //        self.Add(self.Hair = new PlayerHair(self.Sprite));
        //        self.Add(self.Sprite);
        //    }
        //    orig(self);
        //}

        //private static int getHairCount(Player player)
        //{
        //    int numb;
        //    if (Settings.Enabled)
        //    {
        //        numb = Settings.HairLength;
        //    }
        //    else
        //    {
        //        numb = 4;
        //    }
        //    if (player != null && player.Dashes == 1)
        //    {
        //        numb += 1;
        //    }
        //    return numb;
        //}


        private void Player_Respawn(On.Celeste.Player.orig_IntroRespawnBegin orig, Player self)
        {
            int dashes = (self).MaxDashes;

            if (dashes == 0)
                permcolor = ColorFromHex(Settings.Dash0Color);

            if (dashes == 1)
                permcolor = ColorFromHex(Settings.Dash1Color);

            if (dashes == 2)
                permcolor = ColorFromHex(Settings.Dash2Color);

            orig(self);
        }

        private void Death(On.Celeste.DeathEffect.orig_Draw orig, Vector2 position, Color color, float ease)
        {
            if (Settings.Enabled)
            {
                color = permcolor;
            }
            orig(position, color, ease);
        }

        // Optional, initialize anything after Celeste has initialized itself properly.
        public override void Initialize()
        {
        }

        // Optional, do anything requiring either the Celeste or mod content here.
        //public override void LoadContent()
        //{
        //}

        // Unload the entirety of your mod's content, remove any event listeners and undo all hooks.
        public override void Unload()
        {
            //On.Celeste.PlayerHair.GetHairColor -= GetHairColor;
            On.Celeste.Player.GetTrailColor -= GetTrailColor;
            On.Celeste.TrailManager.Add_Entity_Color_float -= AddTrail;
            On.Celeste.DeathEffect.Draw -= Death;
            On.Celeste.Player.IntroRespawnBegin -= Player_Respawn;
            On.Celeste.Player.Update -= Player_Update;
            On.Celeste.PlayerHair.AfterUpdate -= PlayerHair_AfterUpdate;
        }

        //public static Color GetHairColor(On.Celeste.PlayerHair.orig_GetHairColor orig, PlayerHair self, int index)
        //{
        //    Color colorOrig = orig(self, index);
        //    if (!(self.Entity is Player) || self.GetSprite().Mode == PlayerSpriteMode.Badeline)
        //        return colorOrig;

        //    if ((self.Entity as Player).StateMachine.State == 19)
        //        return colorOrig;

        //    Color color = colorOrig;

        //    int dashes = ((Player)self.Entity).Dashes;

        //    if (dashes == 0)
        //        color = ColorFromHex(Settings.Dash0Color);

        //    if (dashes == 1)
        //        color = ColorFromHex(Settings.Dash1Color);

        //    if (dashes == 2)
        //        color = ColorFromHex(Settings.Dash2Color);

        //    permcolor = color;

        //    color.A = colorOrig.A;
        //    if (Settings.Enabled)
        //    {
        //        return color;
        //    }
        //    else
        //    {
        //        return colorOrig;
        //    }
        //}

        public static void AddTrail(On.Celeste.TrailManager.orig_Add_Entity_Color_float orig, Entity self, Color color, float duration)
        {
            //if (Settings.Enabled)
            //{
            //    Color colorOrig = color;

            //    if (!(self is Player))
            //        return;

            //    if ((self as Player).Sprite.Mode == PlayerSpriteMode.Badeline)
            //    {
            //        orig(self, ColorFromHex("ff0019"), duration);
            //        return;
            //    }

            //    Color newColor = color;

            //    if ((self as Player).StateMachine.State == 19)
            //        return;

            //    if (self is Player)
            //    {

            //        int dashes = ((Player)self).Dashes;

            //        if (dashes == 0)
            //            newColor = ColorFromHex(Settings.Dash0Color);

            //        if (dashes == 1)
            //            newColor = ColorFromHex(Settings.Dash1Color);

            //        if (dashes == 2)
            //            newColor = ColorFromHex(Settings.Dash2Color);

            //    }

            //    color.A = colorOrig.A;
            //    if (Settings.Enabled)
            //    {
            //        orig(self, newColor, duration);
            //    }
            //    else
            //    {
            //        orig(self, colorOrig, duration);
            //    }
            //}
            orig(self, color, duration);
        }

        public static Color GetTrailColor(On.Celeste.Player.orig_GetTrailColor orig, Player self, bool wasDashB)
        {
            Color colorOrig = orig(self, wasDashB);

            Color color = colorOrig;

            //if (!(self is Player))
            //    return colorOrig;

            //if ((self as Player).Sprite.Mode == PlayerSpriteMode.Badeline)
            //{
            //    return colorOrig;
            //}

            //int dashes = self.Dashes;

            //if (dashes == 0)
            //    color = ColorFromHex(Settings.Dash0Color);

            //if (dashes == 1)
            //    color = ColorFromHex(Settings.Dash1Color);

            //if (dashes == 2)
            //    color = ColorFromHex(Settings.Dash2Color);

            //color.A = colorOrig.A;
            return color;
        }

        public static Color ColorFromHex(string colorcode)
        {
            colorcode = colorcode.TrimStart('#');

            Color col; // from System.Drawing or System.Windows.Media
            if (colorcode.Length == 6)
            {
                col = new Color(
                            int.Parse(colorcode.Substring(0, 2), NumberStyles.HexNumber),
                            int.Parse(colorcode.Substring(2, 2), NumberStyles.HexNumber),
                            int.Parse(colorcode.Substring(4, 2), NumberStyles.HexNumber),
                            255
                                );
            }
            else // assuming length of 8
                col = new Color(
                            int.Parse(colorcode.Substring(0, 2), NumberStyles.HexNumber),
                            int.Parse(colorcode.Substring(2, 2), NumberStyles.HexNumber),
                            int.Parse(colorcode.Substring(4, 2), NumberStyles.HexNumber),
                            int.Parse(colorcode.Substring(6, 2), NumberStyles.HexNumber)
            );
            return col;
        }

    }
}