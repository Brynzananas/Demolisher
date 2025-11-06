
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BrynzaAPI;
using R2API;
using R2API.Utils;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using UnityEngine;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[assembly: HG.Reflection.SearchableAttribute.OptIn]
[assembly: HG.Reflection.SearchableAttribute.OptInAttribute]
[module: UnverifiableCode]
#pragma warning disable CS0618
#pragma warning restore CS0618
namespace Demolisher
{
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [BepInDependency(R2API.R2API.PluginGUID, R2API.R2API.PluginVersion)]
    [BepInDependency(R2API.SkillsAPI.PluginGUID, SkillsAPI.PluginVersion)]
    [BepInDependency(R2API.Skins.PluginGUID, Skins.PluginVersion)]
    [BepInDependency(R2API.SoundAPI.PluginGUID, SoundAPI.PluginVersion)]
    [BepInDependency(R2API.DamageAPI.PluginGUID, DamageAPI.PluginVersion)]
    [BepInDependency(R2API.Networking.NetworkingAPI.PluginGUID)]
    [BepInDependency(BrynzaAPI.BrynzaAPI.ModGuid, BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency(EmoteCompatAbility.customEmotesApiGUID, BepInDependency.DependencyFlags.SoftDependency)]
    //[BepInDependency(RiskOfOptionsCompatability.riskOfOptionsGUID, BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    //[R2APISubmoduleDependency(nameof(CommandHelper))]
    [System.Serializable]
    public class DemolisherPlugin : BaseUnityPlugin
    {
        public const string ModGuid = "com.brynzananas.demolisher";
        public const string ModName = "Demolisher";
        public const string ModVer = "1.0.0";

        public static bool emotesEnabled { get; private set; }
        public static bool riskOfOptionsEnabled { get; private set; }
        public static BepInEx.PluginInfo PInfo { get; private set; }
        public static ConfigFile configFile { get; private set; }
        public static ManualLogSource Log {  get; private set; }
        public void Awake()
        {
            PInfo = Info;
            //Config = new ConfigFile(Paths.BepInExConfigPath, true);
            configFile = Config;
            //configFile.Reload();
            Log = Logger;
            emotesEnabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(ModCompatabilities.EmoteCompatability.GUID);
            riskOfOptionsEnabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(ModCompatabilities.RiskOfOptionsCompatability.GUID);
            Assets.Init();
            Demolisher.Config.Init();
            Hooks.SetHooks();
            NetworkMessages.Init();
        }
        public void Update()
        {
            if (Slicing.count > 0)
            {
                if (!Slicing.ppEffect) Slicing.ppEffect = Instantiate(Assets.TimestopEffect);
            }
            else
            {
                if (Slicing.ppEffect) Destroy(Slicing.ppEffect);
            }
        }
    }
}