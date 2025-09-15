using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;

namespace FACEITDamage;

[MinimumApiVersion(80)]
public class FaceitDamagePlugin : BasePlugin, IPluginConfig<FaceitDamageConfig>
{
    public override string ModuleName => "FACEITDamage";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Absynthium/ChatGPT";
    public override string ModuleDescription => "Annule les dégâts de balles entre coéquipiers et réapplique les convars à chaque map.";

    private const string CfgFriendlyFire = "mp_friendlyfire 1";
    private const string CfgBulletFFMultiplier = "ff_damage_reduction_bullets 0";
    private const string CfgBulletPenetration = "ff_damage_bullet_penetration 1";

    private static readonly HashSet<string> NonBulletWeapons = new(StringComparer.OrdinalIgnoreCase)
    {
        "hegrenade",
        "molotov",
        "incgrenade",
        "inferno",
        "flashbang",
        "smokegrenade",
        "decoy",
        "tagrenade",
        "taser",
        "snowball",
        "breachcharge",
        "bumpmine",
        "axe",
        "hammer",
        "fists",
        "spanner",
        "shield"
    };

    private FaceitDamageConfig _config = new();

    public FaceitDamageConfig Config
    {
        get => _config;
        set => _config = value ?? new FaceitDamageConfig();
    }

    private string LogFilePath
    {
        get
        {
            var baseDirectory = Server.GameDirectory;
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                baseDirectory = AppContext.BaseDirectory ?? ".";
            }

            var cssFolder = Path.Combine(baseDirectory, "addons", "counterstrikesharp");
            return Path.Combine(cssFolder, "plugins", "FACEITDamage", "faceitdamage.log");
        }
    }

    public void OnConfigParsed(FaceitDamageConfig config)
    {
        Config = config;
        Log($"Configuration parsed. Logging enabled: {Config.LogEnabled}");
    }

    public override void Load(bool hotReload)
    {
        Log($"Load invoked (hotReload: {hotReload}).");
        ApplyFaceitLikeBulletFF("Load", hotReload);
    }

    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        Log("Round start detected, reapplying FACEIT damage configuration.");
        ApplyFaceitLikeBulletFF("RoundStart");
        return HookResult.Continue;
    }

    [GameEventHandler(HookMode.Pre)]
    public HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
        if (@event.Attacker == 0 || @event.Userid == 0 || @event.Attacker == @event.Userid)
        {
            return HookResult.Continue;
        }

        var victim = Utilities.GetPlayerFromUserid(@event.Userid);
        var attacker = Utilities.GetPlayerFromUserid(@event.Attacker);

        if (victim is null || attacker is null)
        {
            return HookResult.Continue;
        }

        if (!IsPlayerOnActiveTeam(attacker) || !IsPlayerOnActiveTeam(victim))
        {
            return HookResult.Continue;
        }

        if (attacker.Team != victim.Team)
        {
            return HookResult.Continue;
        }

        var weapon = @event.Weapon ?? "unknown";
        var originalHealthDamage = @event.DmgHealth;
        var originalArmorDamage = @event.DmgArmor;

        if (IsBulletDamage(weapon))
        {
            @event.DmgHealth = 0;
            @event.DmgArmor = 0;
            Log($"Blocked friendly {weapon} damage from {attacker.PlayerName} to {victim.PlayerName} (hitgroup: {@event.Hitgroup}, health: {originalHealthDamage}, armor: {originalArmorDamage}).");
            return HookResult.Changed;
        }

        var damageKind = IsGrenadeDamage(weapon) ? "grenade" : "non-bullet";
        Log($"Allowed friendly {damageKind} damage ({weapon}) from {attacker.PlayerName} to {victim.PlayerName} (health: {originalHealthDamage}, armor: {originalArmorDamage}).");
        return HookResult.Continue;
    }

    private void ApplyFaceitLikeBulletFF(string reason, bool? hotReload = null)
    {
        Server.ExecuteCommand(CfgFriendlyFire);
        Server.ExecuteCommand(CfgBulletFFMultiplier);
        Server.ExecuteCommand(CfgBulletPenetration);

        var hotReloadInfo = hotReload.HasValue ? $" (hotReload: {hotReload.Value})" : string.Empty;
        Log($"Applied FACEIT damage configuration during {reason}{hotReloadInfo}.");
    }

    private static bool IsPlayerOnActiveTeam(CCSPlayerController player)
    {
        return player.Team is CsTeam.Terrorist or CsTeam.CounterTerrorist;
    }

    private static bool IsBulletDamage(string weapon)
    {
        if (string.IsNullOrWhiteSpace(weapon))
        {
            return false;
        }

        if (NonBulletWeapons.Contains(weapon))
        {
            return false;
        }

        var normalized = weapon.ToLowerInvariant();
        if (normalized.Contains("knife"))
        {
            return false;
        }

        return true;
    }

    private static bool IsGrenadeDamage(string weapon)
    {
        if (string.IsNullOrWhiteSpace(weapon))
        {
            return false;
        }

        var normalized = weapon.ToLowerInvariant();
        return normalized.Contains("grenade") || normalized is "inferno" or "molotov";
    }

    private void Log(string message)
    {
        if (!Config.LogEnabled)
        {
            return;
        }

        try
        {
            var path = LogFilePath;
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var timestamp = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            File.AppendAllText(path, $"[{timestamp}] {message}{Environment.NewLine}");
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "Failed to write FACEITDamage log entry.");
        }
    }
}

public class FaceitDamageConfig : BasePluginConfig
{
    [JsonPropertyName("log_enabled")]
    public bool LogEnabled { get; set; } = true;
}
