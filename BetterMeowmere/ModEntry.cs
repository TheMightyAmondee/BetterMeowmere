using System.Collections.Generic;
using System.Collections;
using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using StardewValley.Tools;
using StardewValley.GameData.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace BetterMeowmere;

public class ModEntry
    : Mod
{
    private ModConfig config;

    public override void Entry(IModHelper helper)
    {
        helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        helper.Events.Content.AssetRequested += this.AssetRequested;

        try
        {
            this.config = helper.ReadConfig<ModConfig>();
        }
        catch
        {
            this.config = new ModConfig();
            this.Monitor.Log("Failed to parse config file, default options will be used.", LogLevel.Warn);
        }

        MeowmereProjectile.Initialise(this.Helper);
    }
    private void AssetRequested(object sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo("Data\\Weapons") && this.config.BuffAttack == true)
        {
            e.Edit(asset =>
            {
                var data = asset.Data as Dictionary<string, WeaponData>;
                if (data != null)
                {
                    data["65"].MinDamage = 100;
                    data["65"].MaxDamage = 120;
                }
            });
        }
    }

    private void ShootProjectile(Farmer user)
    {
        int bounces = 4;
        Random random = new Random();
        var soundtoplay = "terraria_meowmere";
        Game1.currentLocation.playSound(soundtoplay);
        string bouncesound = soundtoplay;

        if (this.config.LessAnnoyingProjectile == true)
        {
            bouncesound = "";
        }

        Vector2 velocity1 = TranslateVector(new Vector2(0, 10), user.FacingDirection);
        Vector2 startPos1 = TranslateVector(new Vector2(0, 96), user.FacingDirection);
        int damage = this.config.BuffAttack == true ? random.Next(50, 70) : random.Next(20, 40);
        Game1.currentLocation.projectiles.Add(new MeowmereProjectile(damage, velocity1.X, velocity1.Y, user.Position + new Vector2(0, -64) + startPos1, bounces, 6, bouncesound, user.currentLocation, user));
    }

    private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
        if (Context.IsWorldReady == false || Context.IsPlayerFree == false)
            return;

        var user = Game1.player;
        if (user.CurrentTool?.Name != "Meowmere")
        {
            return;
        }

        if (e.Button.IsUseToolButton() == true && this.config.ProjectileIsSecondaryAttack == false)
        {
            ShootProjectile(user);
        }

        else if (this.config.ProjectileIsSecondaryAttack == true)
        {
            if ((!e.Button.IsActionButton()) || (MeleeWeapon.defenseCooldown > 0))
            {
                return;
            }
            ShootProjectile(user);
        }

    }

    public static Vector2 TranslateVector(Vector2 vector, int facingDirection)
    {
        float outx = vector.X;
        float outy = vector.Y;
        switch (facingDirection)
        {
            case 2:
                break;
            case 3:
                outx = -vector.Y;
                outy = vector.X;
                break;
            case 0:
                outx = -vector.X;
                outy = -vector.Y;
                break;
            case 1:
                outx = vector.Y;
                outy = -vector.X;
                break;
        }
        return new Vector2(outx, outy);
    }
}