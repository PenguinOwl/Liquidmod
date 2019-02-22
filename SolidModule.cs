using System;
using System.Collections;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using Calc = Monocle.Calc;

namespace Celeste.Mod.Solid
{
    public class SolidModule : EverestModule
    {
        // Only one alive module instance can exist at any given time.
        public static SolidModule Instance;

        private const int FlyHairCountDifference = 3;
        private const int TwoDashHairCountDifference = 1;

        private readonly Color _origUsedHairColor = Player.UsedHairColor;
        private readonly Color _origNormalHairColor = Player.NormalHairColor;
        private readonly Color _origTwoDashesHairColor = Player.TwoDashesHairColor;

        private readonly FieldInfo _usedHairColor = typeof(Player).GetField("UsedHairColor");
        private readonly FieldInfo _normalHairColor = typeof(Player).GetField("NormalHairColor");
        private readonly FieldInfo _twoDashesHairColor = typeof(Player).GetField("TwoDashesHairColor");

        private bool _enabled;
        private bool _badeline;
        private bool _floating;

        private string _dash0Color;
        private string _dash1Color;
        private string _dash2Color;

        public SolidModule()
        {
            Instance = this;
        }

        // If you don't need to store any settings, => null
        public override Type SettingsType => typeof(SolidSettings);
        public static SolidSettings Settings => (SolidSettings) Instance._Settings;

        // If you don't need to store any save data, => null
        public override Type SaveDataType => null;
        //public static SolidSaveData SaveData => (SolidSaveData)Instance._SaveData;

        // Set up any hooks, event handlers and your mod in general here.
        // Load runs before Celeste itself has initialized properly.
        public override void Load()
        {
            On.Celeste.Player.Update += Player_Update;
            On.Celeste.Player.GetTrailColor += PlayerOnGetTrailColor;
            On.Celeste.Player.ctor += PlayerOnCtor;
            On.Celeste.PlayerHair.Update += PlayerHairOnUpdate;
            On.Celeste.TrailManager.Add_Entity_Color_float += TrailManagerOnAddEntityColorFloat;
            On.Monocle.Sprite.Play += Sprite_Play;
        }

        // Optional, initialize anything after Celeste has initialized itself properly.
        public override void Initialize()
        {
            _dash0Color = Settings.Dash0Color;
            _dash1Color = Settings.Dash1Color;
            _dash2Color = Settings.Dash2Color;
        }


        // Optional, do anything requiring either the Celeste or mod content here.
        //public override void LoadContent() { }

        // Unload the entirety of your mod's content, remove any event listeners and undo all hooks.
        public override void Unload()
        {
            On.Celeste.Player.Update -= Player_Update;
            On.Celeste.Player.GetTrailColor -= PlayerOnGetTrailColor;
            On.Celeste.Player.ctor -= PlayerOnCtor;
            On.Celeste.PlayerHair.Update -= PlayerHairOnUpdate;
            On.Monocle.Sprite.Play -= Sprite_Play;
        }
        
        
        // Avoid badeline color changes
        private void TrailManagerOnAddEntityColorFloat(On.Celeste.TrailManager.orig_Add_Entity_Color_float orig, Entity entity, Color color, float duration)
        {

            if (entity is Player)
            {
                orig(entity, color, duration);
                return;
            }

            if (color == (Color) _normalHairColor.GetValue(null))
            {
                color = _origNormalHairColor;
            }
            else if(color == (Color) _twoDashesHairColor.GetValue(null))
            {
                color = _origTwoDashesHairColor;
            }

            orig(entity, color, duration);
        }

        private void PlayerOnCtor(On.Celeste.Player.orig_ctor orig, Player self, Vector2 position,
            PlayerSpriteMode spriteMode)
        {
            orig(self, position, spriteMode);

            self.Add(new Coroutine(SetFlyHairCount(self)));
            self.Add(new Coroutine(PreventBadelineStuckInFeather(self)));

            if (Settings.Enabled && Settings.Badeline)
                ResetSprite(self, true);
        }

        private IEnumerator SetFlyHairCount(Player player)
        {
            while (true)
            {
                while (player.StateMachine.State != Player.StStarFly)
                    yield return null;

                while (player.Sprite.CurrentAnimationID == "startStarFly")
                    yield return null;

                while (player.Speed != Vector2.Zero)
                    yield return null;

                yield return 0.1f;

                if (Settings.Enabled)
                    player.Sprite.HairCount = Settings.HairLength + FlyHairCountDifference;

                while (player.StateMachine.State == Player.StStarFly)
                    yield return null;
            }
        }

        private IEnumerator PreventBadelineStuckInFeather(Player player)
        {
            while (true)
            {
                while (player.StateMachine.State != Player.StStarFly)
                    yield return null;

                float stuckTime = 0;

                while (player.Sprite.CurrentAnimationID == "startStarFly" && stuckTime < 0.3f)
                {
                    yield return null;
                    stuckTime += Engine.DeltaTime;
                }

                if (Settings.Badeline)
                    player.Sprite.Stop();

                while (player.StateMachine.State == Player.StStarFly)
                    yield return null;
            }
        }

        private static void PlayerHairOnUpdate(On.Celeste.PlayerHair.orig_Update orig, PlayerHair self)
        {
            orig(self);
            if (Settings.Enabled && self.Entity is Player player && player.StateMachine.State != Player.StStarFly)
            {
                self.Sprite.HairCount = Settings.HairLength;
                if (player.Dashes > 1)
                    self.Sprite.HairCount += TwoDashHairCountDifference;
            }
        }

        private static Color PlayerOnGetTrailColor(On.Celeste.Player.orig_GetTrailColor orig, Player self, bool wasDashB)
        {
            // same color as BadelineOldsite 
            if (Settings.Enabled && Settings.Badeline)
                return wasDashB ? Player.NormalBadelineHairColor : Player.NormalHairColor;

            return orig(self, wasDashB);
        }

        private static void Sprite_Play(On.Monocle.Sprite.orig_Play orig, Monocle.Sprite self, string id, bool restart,
            bool randomizeFrame)
        {
            try
            {
                orig(self, id, restart, randomizeFrame);
            }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
            catch { }
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
        }

        private void Player_Update(On.Celeste.Player.orig_Update orig, Player self)
        {
            if (Settings.Enabled != _enabled || Settings.Badeline != _badeline || Settings.Dash0Color != _dash0Color ||
                Settings.Dash1Color != _dash1Color || Settings.Dash2Color != _dash2Color)
            {
                _enabled = Settings.Enabled;
                _badeline = Settings.Badeline;
                _dash0Color = Settings.Dash0Color;
                _dash1Color = Settings.Dash1Color;
                _dash2Color = Settings.Dash2Color;
                if (_enabled && !_badeline)
                {
                    _usedHairColor.SetValue(null, Calc.HexToColor(_dash0Color));
                    _normalHairColor.SetValue(null, Calc.HexToColor(_dash1Color));
                    _twoDashesHairColor.SetValue(null, Calc.HexToColor(_dash2Color));
                }
                else
                {
                    _usedHairColor.SetValue(null, _origUsedHairColor);
                    _normalHairColor.SetValue(null, _origNormalHairColor);
                    _twoDashesHairColor.SetValue(null, _origTwoDashesHairColor);
                }

                ResetSprite(self, _enabled && _badeline);
            }

            if (!Settings.Enabled)
            {
                orig(self);
                return;
            }

            if (Settings.Badeline != _badeline)
            {
                _badeline = Settings.Badeline;
                ResetSprite(self, _badeline);
            }

            if (Settings.BadelineFloat)
            {
                if (MInput.Keyboard.Check(Keys.A))
                {
                    _floating = true;
                    //self.Collidable = false;
                    //((Level)self.Scene).SolidTiles.Collidable = false;
                    self.DummyGravity = true;

                    self.Speed.Y = -15f;
                    if (Keyboard.GetState().IsKeyDown(Keys.P)) self.Speed.Y = Calc.Approach(self.Speed.Y, -120f, 360f);

                    if (Keyboard.GetState().IsKeyDown(Keys.L))
                    {
                        self.Speed.X = Calc.Approach(self.Speed.X, -120f, 360f);
                        self.Facing = Facings.Left;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.OemQuotes))
                    {
                        self.Speed.X = Calc.Approach(self.Speed.X, 120f, 360f);
                        self.Facing = Facings.Right;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.OemSemicolon))
                        self.Speed.Y = Calc.Approach(self.Speed.Y, 120f, 360f);
                }
                else
                {
                    if (_floating)
                    {
                        //self.Collidable = true;
                        _floating = false;
                        self.DummyGravity = false;
                        //((Level)self.Scene).SolidTiles.Collidable = true;
                    }
                }
            }

            orig(self);
        }

        private static void ResetSprite(Player player, bool badeline)
        {
            PlayerSpriteMode mode;
            if (badeline || SaveData.Instance.Assists.PlayAsBadeline)
            {
                mode = PlayerSpriteMode.Badeline;
            }
            else if (player.SceneAs<Level>().Session.Inventory.Backpack)
            {
                mode = PlayerSpriteMode.Madeline;
            }
            else
            {
                mode = PlayerSpriteMode.MadelineNoBackpack;
            }

            player.Remove(player.Sprite);
            player.Add(player.Sprite = new PlayerSprite(mode));
            player.Hair.Sprite = player.Sprite;
        }
    }
}