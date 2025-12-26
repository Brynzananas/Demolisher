using BepInEx.Configuration;
using EmotesAPI;
using RiskOfOptions;
using RiskOfOptions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Demolisher
{
    public static class ModCompatabilities
    {
        public static class EmoteCompatability
        {
            public const string GUID = "com.weliveinasociety.CustomEmotesAPI";
            public static void Init()
            {
                CustomEmotesAPI.ImportArmature(Assets.DemolisherBody, Assets.DemolisherEmote);
                CustomEmotesAPI.animChanged += CustomEmotesAPI_animChanged;
            }
            private static void CustomEmotesAPI_animChanged(string newAnimation, BoneMapper mapper)
            {
                if (mapper.name == "DemolisherEmotes")
                {
                    DemolisherModel demolisherModel = mapper.transform.parent.GetComponent<DemolisherModel>();
                    if (demolisherModel == null) return;
                    demolisherModel.emoting = !(newAnimation == "none");
                    onDemolisherEmote?.Invoke(newAnimation, demolisherModel);
                }
            }
            public static Action<string, DemolisherModel> onDemolisherEmote;
        }
        public static class RiskOfOptionsCompatability
        {
            public const string GUID = "com.rune580.riskofoptions";
            public static void Init()
            {
                ModSettingsManager.SetModIcon(Assets.assetBundle.LoadAsset<Sprite>("Assets/Demolisher/Textures/DemoIconTS.png"));
            }
            public static void AddConfig<T>(T config) where T : ConfigEntryBase
            {
                if (config is ConfigEntry<float>)
                {
                    ModSettingsManager.AddOption(new FloatFieldOption(config as ConfigEntry<float>));
                    return;
                }
                if (config is ConfigEntry<bool>)
                {
                    ModSettingsManager.AddOption(new CheckBoxOption(config as ConfigEntry<bool>));
                    return;
                }
                if (config is ConfigEntry<int>)
                {
                    ModSettingsManager.AddOption(new IntFieldOption(config as ConfigEntry<int>));
                    return;
                }
                if (config is ConfigEntry<string>)
                {
                    ModSettingsManager.AddOption(new StringInputFieldOption(config as ConfigEntry<string>));
                    return;
                }
                ModSettingsManager.AddOption(new ChoiceOption(config));
            }
        }
    }
}
