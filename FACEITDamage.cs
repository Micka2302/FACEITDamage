using CounterStrikeSharp.API;                         
using CounterStrikeSharp.API.Core;                   
using CounterStrikeSharp.API.Core.Attributes;        
using CounterStrikeSharp.API.Core.Attributes.Registration;

namespace FACEITDamage;

[MinimumApiVersion(80)]
public class FaceitDamagePlugin : BasePlugin
{
    public override string ModuleName => "FACEITDamage";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Absynthium/ChatGPT";
    public override string ModuleDescription => "Annule les dégâts de balles entre coéquipiers et réapplique les convars à chaque map.";

    private const string CfgFriendlyFire       = "mp_friendlyfire 1";
    private const string CfgBulletFFMultiplier = "ff_damage_reduction_bullets 0";
    private const string CfgBulletPenetration  = "ff_damage_bullet_penetration 1";

    public override void Load(bool hotReload)
    {
        ApplyFaceitLikeBulletFF();
    }

    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        ApplyFaceitLikeBulletFF();
        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
        var attacker = @event.Attacker;
        var victim = @event.Userid;

        if (attacker is null || victim is null || !attacker.IsValid || !victim.IsValid)
        {
            return HookResult.Continue;
        }

        if (attacker.TeamNum != victim.TeamNum)
        {
            return HookResult.Continue;
        }

        if (IsGrenadeWeapon(@event.Weapon))
        {
            return HookResult.Continue;
        }

        @event.DmgHealth = 0;
        @event.DmgArmor = 0;
        return HookResult.Continue;
    }

    private void ApplyFaceitLikeBulletFF()
    {
        Server.ExecuteCommand(CfgFriendlyFire);
        Server.ExecuteCommand(CfgBulletFFMultiplier);
        Server.ExecuteCommand(CfgBulletPenetration);
    }

    private static bool IsGrenadeWeapon(string? weapon)
    {
        if (string.IsNullOrWhiteSpace(weapon))
        {
            return false;
        }

        var normalized = weapon.StartsWith("weapon_", StringComparison.OrdinalIgnoreCase)
            ? weapon["weapon_".Length..]
            : weapon;

        return normalized.Equals("hegrenade", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("molotov", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("incgrenade", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("firebomb", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("flashbang", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("smokegrenade", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("decoy", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("tagrenade", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("snowball", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("gasgrenade", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("diversion", StringComparison.OrdinalIgnoreCase);
    }
}
