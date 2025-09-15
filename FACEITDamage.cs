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

    private void ApplyFaceitLikeBulletFF()
    {
        Server.ExecuteCommand(CfgFriendlyFire);
        Server.ExecuteCommand(CfgBulletFFMultiplier);
        Server.ExecuteCommand(CfgBulletPenetration);
    }
}
