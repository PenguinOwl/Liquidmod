using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using On.Celeste;

namespace Celeste.Mod.Solid
{
    public class SolidModule : EverestModule
    {

        // Only one alive module instance can exist at any given time.
        public static SolidModule Instance;

        private static Color permcolor;

        private static bool Badeline;

        private static int HairCount;
        private bool Floating;

        public SolidModule()
        {
            Instance = this;
        }

        // If you don't need to store any settings, => null
        public override Type SettingsType => typeof(SolidSettings);
        public static SolidSettings Settings => (SolidSettings)Instance._Settings;

        // If you don't need to store any save data, => null
        public override Type SaveDataType => null;
        //public static SolidSaveData SaveData => (SolidSaveData)Instance._SaveData;

        // Set up any hooks, event handlers and your mod in general here.
        // Load runs before Celeste itself has initialized properly.
        public override void Load()
        {
            On.Celeste.Player.GetTrailColor += GetTrailColor;
            On.Celeste.PlayerHair.GetHairColor += GetHairColor;
            On.Celeste.TrailManager.Add_Entity_Color_float += AddTrail;
            On.Celeste.DeathEffect.Draw += Death;
            On.Celeste.Player.IntroRespawnBegin += Respawn;
            On.Celeste.Player.Update += Player_Update;
            On.Monocle.Sprite.Play += Sprite_Play;
            On.Celeste.PlayerHair.Render += PlayerHair_Render;
            On.Celeste.PlayerHair.Update += PlayerHair_Update;
        }

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
            //self.Sprite.HairCount = Settings.HairLength;
            //self.Add(self.Hair = new PlayerHair(self.Sprite));
            //self.Add(self.Sprite);

            orig(self);
        }

        void PlayerHair_Update(On.Celeste.PlayerHair.orig_Update orig, PlayerHair self)
        {
            if ((self.Entity as BadelineOldsite) == null && ((self.Entity as Player) != null && Settings.HairLength != HairCount))
            {
                Player player = self.Entity as Player;
                player.Sprite.HairCount = Settings.HairLength;
                HairCount = Settings.HairLength;
                player.Remove(player.Hair);
                PlayerSpriteMode mode = self.Sprite.Mode;
                player.Remove(player.Sprite);
                player.Add(player.Hair = new PlayerHair(player.Sprite));
                player.Add(player.Sprite);
            }
            if ((self.Entity as BadelineOldsite) == null && (self.Entity as Player) != null) {
                self.Sprite.HairCount = Settings.HairLength;
            }
            orig(self);
        }


        void PlayerHair_Render(On.Celeste.PlayerHair.orig_Render orig, PlayerHair self)
        {
            //self.Sprite.HairCount = Settings.HairLength;
            orig(self);
        }


        private void Sprite_Play(On.Monocle.Sprite.orig_Play orig, Sprite self, string id, bool restart, bool randomizeFrame)
        {
            if (Settings.Badeline)
            {
                try
                {
                    orig(self, id, restart, randomizeFrame);
                }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
                catch { }
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
            }
            else
            {
                orig(self, id, restart, randomizeFrame);
            }
        }


        private void Player_Update(On.Celeste.Player.orig_Update orig, Player self)
        {
            //Logger.Log("w", self.GetType().ToString());
            if (self.GetType().Name == "Ghost")
            {
                Logger.Log("a", self.GetType().Name);
                orig(self);
                return;
            }
            if (((Entity)self as BadelineOldsite) != null) {
                self.Sprite.HairCount = Settings.HairLength;
            }
            if ((Settings.Badeline) != (self.Sprite.Mode == PlayerSpriteMode.Badeline))
            {
                PlayerSpriteMode mode = self.Sprite.Mode;
                self.Remove(self.Sprite);
                if (Settings.Badeline)
                {
                    Logger.Log("a",self.GetType().Name);
                    self.Sprite = new PlayerSprite(PlayerSpriteMode.Badeline);
                }
                else
                {
                    self.Sprite = new PlayerSprite(mode);
                }
                self.Remove(self.Hair);
                self.Sprite.HairCount = Settings.HairLength;
                self.Add(self.Hair = new PlayerHair(self.Sprite));
                self.Add(self.Sprite);
            }
            //if (Settings.Badeline != Badeline)
            //{
            //    if (Settings.Badeline)
            //    {
            //        self.Remove(self.Sprite);
            //        self.Sprite = new PlayerSprite(PlayerSpriteMode.Badeline);
            //        self.Remove(self.Hair);
            //        self.Add(self.Hair = new PlayerHair(self.Sprite));
            //        self.Add(self.Sprite);
            //        Logger.Log("w", self.GetType().ToString());
            //    }
            //    else
            //    {
            //        self.Remove(self.Sprite);
            //        self.Sprite = new PlayerSprite(PlayerSpriteMode.Madeline);
            //        self.Remove(self.Hair);
            //        self.Add(self.Hair = new PlayerHair(self.Sprite));
            //        self.Add(self.Sprite);
            //    }
            //    Badeline = Settings.Badeline;
            //}
            if (Settings.HairLength != HairCount && self.Sprite.Entity as Player != null)
            {
                self.Sprite.HairCount = Settings.HairLength;
                HairCount = Settings.HairLength;
                self.Remove(self.Hair);
                PlayerSpriteMode mode = self.Sprite.Mode;
                self.Remove(self.Sprite);
                self.Add(self.Hair = new PlayerHair(self.Sprite));
                self.Add(self.Sprite);
            }
            if (Settings.BadelineFloat)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    Floating = true;
                    //self.Collidable = false;
                    //((Level)self.Scene).SolidTiles.Collidable = false;
                    self.DummyGravity = true;

                    self.Speed.Y = -15f;
                    if (Keyboard.GetState().IsKeyDown(Keys.P))
                    {
                        self.Speed.Y = Calc.Approach(self.Speed.Y, -120f, 360f);
                    }
                    if(Keyboard.GetState().IsKeyDown(Keys.L))
                    {
                        self.Speed.X = Calc.Approach(self.Speed.X, -120f, 360f);
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.OemQuotes))
                    {
                        self.Speed.X = Calc.Approach(self.Speed.X, 120f, 360f);
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.OemSemicolon))
                    {
                        self.Speed.Y = Calc.Approach(self.Speed.Y, 120f, 360f);
                    }
                }
                else
                {
                    if (Floating)
                    {
                        //self.Collidable = true;
                        Floating = false;
                        self.DummyGravity = false;
                        //((Level)self.Scene).SolidTiles.Collidable = true;
                    }
                }
            }
            orig(self);
        }


        private void Respawn(On.Celeste.Player.orig_IntroRespawnBegin orig, Player self)
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
            On.Celeste.PlayerHair.GetHairColor -= GetHairColor;
            On.Celeste.Player.GetTrailColor -= GetTrailColor;
            On.Celeste.TrailManager.Add_Entity_Color_float -= AddTrail;
            On.Celeste.DeathEffect.Draw -= Death;
            On.Celeste.Player.IntroRespawnBegin -= Respawn;
            On.Celeste.Player.Update -= Player_Update;
            On.Monocle.Sprite.Play -= Sprite_Play;
            On.Celeste.PlayerHair.Render -= PlayerHair_Render;
            On.Celeste.PlayerHair.Update -= PlayerHair_Update;
        }

        public static Color GetHairColor(On.Celeste.PlayerHair.orig_GetHairColor orig, PlayerHair self, int index)
        {
            Color colorOrig = orig(self, index);
            if (!(self.Entity is Player) || self.GetSprite().Mode == PlayerSpriteMode.Badeline)
                return colorOrig;

            if ((self.Entity as Player).StateMachine.State == 19)
                return colorOrig;

            Color color = colorOrig;

            int dashes = ((Player)self.Entity).Dashes;

            if (dashes == 0)
                color = ColorFromHex(Settings.Dash0Color);

            if (dashes == 1)
                color = ColorFromHex(Settings.Dash1Color);

            if (dashes == 2)
                color = ColorFromHex(Settings.Dash2Color);

            permcolor = color;

            color.A = colorOrig.A;
            if (Settings.Enabled)
            {
                return color;
            }
            else
            {
                return colorOrig;
            }
        }

        public static void AddTrail(On.Celeste.TrailManager.orig_Add_Entity_Color_float orig, Entity self, Color color, float duration)
        {
            Color colorOrig = color;

            if (!(self is Player))
                return;

            if ((self as Player).Sprite.Mode == PlayerSpriteMode.Badeline)
            {
                orig(self, ColorFromHex("ff0019"), duration);
                return;
            }

            Color newColor = color;

            if ((self as Player).StateMachine.State == 19)
                return;

            if (self is Player)
            {

                int dashes = ((Player)self).Dashes;

                if (dashes == 0)
                    newColor = ColorFromHex(Settings.Dash0Color);

                if (dashes == 1)
                    newColor = ColorFromHex(Settings.Dash1Color);

                if (dashes == 2)
                    newColor = ColorFromHex(Settings.Dash2Color);

            }

            color.A = colorOrig.A;
            if (Settings.Enabled)
            {
                orig(self, newColor, duration);
            }
            else
            {
                orig(self, colorOrig, duration);
            }
        }

        public static Color GetTrailColor(On.Celeste.Player.orig_GetTrailColor orig, Player self, bool wasDashB)
        {
            Color colorOrig = orig(self, wasDashB);

            Color color = colorOrig;

            if (!(self is Player))
                return colorOrig;

            if ((self as Player).Sprite.Mode == PlayerSpriteMode.Badeline)
            {
                return colorOrig;
            }

            int dashes = self.Dashes;

            if (dashes == 0)
                color = ColorFromHex(Settings.Dash0Color);

            if (dashes == 1)
                color = ColorFromHex(Settings.Dash1Color);

            if (dashes == 2)
                color = ColorFromHex(Settings.Dash2Color);

            color.A = colorOrig.A;
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