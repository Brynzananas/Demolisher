using BrynzaAPI;
using HarmonyLib;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using R2API;
using R2API.Utils;
using Rewired;
using RoR2;
using RoR2.CharacterAI;
using RoR2.UI;
using RoR2BepInExPack.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using static UnityEngine.SendMouseEvents;

namespace Demolisher
{
    public static class Hooks
    {
        public static FieldReference FieldReference;
        public static TypeDefinition ThatFuckingStructThatIHate;
        public static void SetHooks()
        {
            On.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess;
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            IL.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess1;
            IL.RoR2.UI.HUD.Update += HUD_Update;
            On.RoR2.GlobalEventManager.OnCharacterHitGroundServer += GlobalEventManager_OnCharacterHitGroundServer;
            On.RoR2.BodyCatalog.SetBodyPrefabs += BodyCatalog_SetBodyPrefabs;
            On.RoR2.GlobalEventManager.IsImmuneToFallDamage += GlobalEventManager_IsImmuneToFallDamage;
            RoR2Application.onLoadFinished += OnRoR2Loaded;
            On.RoR2.UI.CharacterSelectController.OnEnable += CharacterSelectController_OnEnable;
            On.RoR2.UI.CharacterSelectController.OnDisable += CharacterSelectController_OnDisable; ;
        }

        private static void CharacterSelectController_OnDisable(On.RoR2.UI.CharacterSelectController.orig_OnDisable orig, CharacterSelectController self)
        {
            orig(self);
            CanvasGroup canvasGroup = self.GetComponent<CanvasGroup>();
            if (canvasGroup) canvasGroups.Add(canvasGroup);
        }

        public static List<CanvasGroup> canvasGroups = [];
        private static void CharacterSelectController_OnEnable(On.RoR2.UI.CharacterSelectController.orig_OnEnable orig, CharacterSelectController self)
        {
            orig(self);
            CanvasGroup canvasGroup = self.GetOrAddComponent<CanvasGroup>();
            canvasGroups.Add(canvasGroup);
        }

        private static bool GlobalEventManager_IsImmuneToFallDamage(On.RoR2.GlobalEventManager.orig_IsImmuneToFallDamage orig, GlobalEventManager self, CharacterBody body)
        {
            if (body.bodyIndex == Assets.DemolisherBodyIndex) return true;
            return orig(self, body);
        }

        private static void BodyCatalog_SetBodyPrefabs(On.RoR2.BodyCatalog.orig_SetBodyPrefabs orig, GameObject[] newBodyPrefabs)
        {
            orig(newBodyPrefabs);
            Assets.DemolisherBodyIndex = BodyCatalog.FindBodyIndex("DemolisherBody");
        }
        public static float stompNeededYVelocity = 48f;
        public static float stompNeededVelocity = 48f;
        public static float stompForce = 100f;
        public static float stompBaseRadius = 3f;
        public static float stompBaseDamageCoefficient = 1f;
        public static float stompVelocityDamageCoefficient = 0.2f;
        public static float stompVelocityRadiusMultiplier = 0.5f;
        public static float stompNextJumpReductionCoefficient = 0.75f;
        public static BlastAttack.FalloffModel stompFalloff = BlastAttack.FalloffModel.Linear;
        public static float stompProcCoefficient = 1f;
        private static void GlobalEventManager_OnCharacterHitGroundServer(On.RoR2.GlobalEventManager.orig_OnCharacterHitGroundServer orig, GlobalEventManager self, CharacterBody characterBody, CharacterMotor.HitGroundInfo hitGroundInfo)
        {
            orig(self, characterBody, hitGroundInfo);
            if (characterBody.bodyIndex == Assets.DemolisherBodyIndex)
            {
                float magnitude = hitGroundInfo.velocity.magnitude;
                //float num = Mathf.Abs(hitGroundInfo.velocity.y);
                if (magnitude >= stompNeededVelocity)
                {
                    BlastAttack blastAttack = new BlastAttack
                    {
                        attacker = characterBody.gameObject,
                        attackerFiltering = AttackerFiltering.Default,
                        baseDamage = characterBody.damage * stompBaseDamageCoefficient + characterBody.damage * stompVelocityDamageCoefficient * magnitude,
                        baseForce = stompForce,
                        crit = characterBody.RollCrit(),
                        damageColorIndex = DamageColorIndex.Default,
                        falloffModel = stompFalloff,
                        inflictor = characterBody.gameObject,
                        position = hitGroundInfo.position,
                        damageType = new DamageTypeCombo(DamageType.Stun1s, DamageTypeExtended.Generic, DamageSource.NoneSpecified),
                        procCoefficient = stompProcCoefficient,
                        radius = stompBaseRadius + magnitude * stompVelocityRadiusMultiplier,
                        teamIndex = characterBody.teamComponent ? characterBody.teamComponent.teamIndex : TeamIndex.Neutral,
                    };
                    blastAttack.Fire();
                    EffectData effectData = new()
                    {
                        origin = blastAttack.position,
                        scale = blastAttack.radius,
                    };
                    EffectManager.SpawnEffect(Assets.Explosion.prefab, effectData, true);
                    CharacterMotor characterMotor = characterBody.characterMotor;
                    if (characterMotor)
                    {
                        PhysForceInfo physForceInfo = new PhysForceInfo
                        {
                            disableAirControlUntilCollision = false,
                            force = characterBody.characterMotor.velocity * -1f,
                            massIsOne = true,
                        };
                        characterMotor.ApplyForceImpulse(physForceInfo);
                    }
                }
            }
        }

        private static void OnRoR2Loaded()
        {
            int count = VoicelineDef.voicelineDefs.Count;
            for (int i = 0; i < count; i++)
            {
                VoicelineDef voicelineDef = VoicelineDef.voicelineDefs[i];
                voicelineDef.id = i;
                foreach (VoicelineDef.VoicelineType voicelineType in Enum.GetValues(typeof(VoicelineDef.VoicelineType)))
                {
                    if (voicelineDef.voicelineType.HasFlag(voicelineType))
                    {
                        if (VoicelineDef.voicelinesByType.ContainsKey(voicelineType))
                        {
                            VoicelineDef.voicelinesByType[voicelineType].Add(voicelineDef);
                        }
                        else
                        {
                            VoicelineDef.voicelinesByType.Add(voicelineType, [voicelineDef]);
                        }
                    }
                }
            }
        }

        public static SkillIcon altPrimary;
        public static SkillIcon altSecondary;
        public static SkillIcon altUtility;
        public static SkillIcon altSpecial;
        private static void HUD_Update(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            ILLabel iLLabel = null;
            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdloc(9),
                    x => x.MatchCall(typeof(UnityEngine.Object), "op_Implicit"),
                    x => x.MatchBrfalse(out iLLabel)
                ))
            {
                c.GotoLabel(iLLabel, MoveType.After);
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldloc_1);
                c.EmitDelegate(HandleDemolisherSkillIcons);
                void HandleDemolisherSkillIcons(HUD hUD, PlayerCharacterMasterController playerCharacterMasterController)
                {
                    DemolisherComponent demolisherComponent = hUD && hUD.targetBodyObject ? hUD.targetBodyObject.GetComponent<DemolisherComponent>() : null;
                    if (demolisherComponent)
                    {
                        for (int i = 0; i < hUD.skillIcons.Length; i++) HandleAltSkillIcon(i);
                    }
                    else
                    {
                        if (altPrimary) GameObject.Destroy(altPrimary.gameObject);
                        if (altSecondary) GameObject.Destroy(altSecondary.gameObject);
                        if (altUtility) GameObject.Destroy(altUtility.gameObject);
                        if (altSpecial) GameObject.Destroy(altSpecial.gameObject);
                    }
                    void HandleAltSkillIcon(int id)
                    {
                        SkillIcon altSkillIcon = null;
                        SkillIcon skillIcon = hUD.skillIcons.Length > id ? hUD.skillIcons[id] : null;
                        if (skillIcon == null) return;
                        switch (id)
                        {
                            case 0:
                                altSkillIcon = altPrimary;
                                break;
                            case 1:
                                altSkillIcon = altSecondary;
                                break;
                            case 2:
                                altSkillIcon = altUtility;
                                break;
                            case 3:
                                altSkillIcon = altSpecial;
                                break;
                            default:
                                break;
                        }
                        if (altSkillIcon == null)
                        {
                            altSkillIcon = GameObject.Instantiate(skillIcon, skillIcon.transform.parent);
                            Vector3 vector3 = altSkillIcon.transform.localPosition;
                            vector3.y += 200f;
                            altSkillIcon.transform.localPosition = vector3;
                        }
                        switch (id)
                        {
                            case 0:
                                altSkillIcon.targetSkill = demolisherComponent.holsterPrimary;
                                altSkillIcon.targetSkillSlot = SkillSlot.Primary;
                                altPrimary = altSkillIcon;
                                break;
                            case 1:
                                altSkillIcon.targetSkill = demolisherComponent.holsterSecondary;
                                altSkillIcon.targetSkillSlot = SkillSlot.Secondary;
                                altSecondary = altSkillIcon;
                                break;
                            case 2:
                                altSkillIcon.targetSkill = demolisherComponent.holsterUtility;
                                altSkillIcon.targetSkillSlot = SkillSlot.Utility;
                                altUtility = altSkillIcon;
                                break;
                            case 3:
                                altSkillIcon.targetSkill = demolisherComponent.holsterSpecial;
                                altSkillIcon.targetSkillSlot = SkillSlot.Special;
                                altSpecial = altSkillIcon;
                                break;
                            default:
                                break;
                        }
                        altSkillIcon.playerCharacterMasterController = playerCharacterMasterController;
                    }
                }
            }
            else
            {
                Debug.LogError(il.Method.Name + " IL Hook 1 failed!");
            }
        }

        public static float SharpnessCritAddition = 10f;
        public static float SharpnessDamageMultiplier = 3f;
        public static float SharpnessCooldown = 10f;
        public static float BombDoubleDonkDamageMultiplier = 2f;
        private static void HealthComponent_TakeDamageProcess1(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After,
                    x => x.MatchLdloc(0),
                    x => x.MatchLdfld(out FieldReference),
                    x => x.MatchCallvirt(typeof(CharacterBody).GetPropertyGetter(nameof(CharacterBody.master))),
                    x => x.MatchStloc(out _)
                ))
            {
                ThatFuckingStructThatIHate = FieldReference.DeclaringType.Resolve();
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldloc_0);
                c.Emit(OpCodes.Ldfld, ThatFuckingStructThatIHate.Fields[2]);
                c.Emit(OpCodes.Ldloc_0);
                c.Emit(OpCodes.Ldfld, ThatFuckingStructThatIHate.Fields[1]);
                c.Emit(OpCodes.Ldloc, 7);
                c.EmitDelegate(HandleSharpness);
                float HandleSharpness(HealthComponent healthComponent, DamageInfo damageInfo, CharacterBody attackerBody, float damage)
                {
                    CharacterBody victimBody = healthComponent.body;
                    if (damageInfo.HasModdedDamageType(Assets.BombDamageType))
                    {
                        if (victimBody.HasBuff(Assets.BombHit))
                        {
                            damage *= BombDoubleDonkDamageMultiplier;
                            EffectData effectData = new EffectData
                            {
                                origin = damageInfo.inflictor ? damageInfo.inflictor.transform.position : damageInfo.position,
                                scale = 3f
                            };
                            EffectManager.SpawnEffect(Assets.DoubleDonk.prefab, effectData, true);
                            victimBody.RemoveBuff(Assets.BombHit);
                        }
                    }
                    if (damageInfo.HasModdedDamageType(Assets.SharpnessDamageType))
                    {
                        if (!victimBody.HasBuff(Assets.SharpnessCooldown))
                        {
                            damage *= SharpnessDamageMultiplier;
                            victimBody.AddTimedBuff(Assets.SharpnessCooldown, SharpnessCooldown);
                        }
                        if (!damageInfo.crit)
                        {
                            int buffCount = attackerBody.GetBuffCount(Assets.SharpnessCritAddition);
                            bool crit = Util.CheckRoll(buffCount * SharpnessCritAddition);
                            if (crit)
                            {
                                attackerBody.SetBuffCount(Assets.SharpnessCritAddition.buffIndex, 0);
                                damageInfo.crit = true;
                            }
                            else
                            {
                                attackerBody.AddBuff(Assets.SharpnessCritAddition);
                            }
                        }
                    }
                    return damage;
                }
                c.Emit(OpCodes.Stloc, 7);
            }
            else
            {
                Debug.LogError(il.Method.Name + " IL Hook 1 failed!");
            }
        }

        public static float SoftnessHealOnHitPercentage = 5f;
        public static float SoftnessHealOnKillPercentage = 25f;
        public static float ChaosDamageCoefficient = 10f;
        public static float ChaosProcCoefficient = 1f;
        public static float ChaosRadius = 12f;
        public static float ChaosForce = 300f;
        public static float ChaosCooldown = 10f;
        
        private static void GlobalEventManager_onServerDamageDealt(DamageReport obj)
        {
            CharacterBody attackerBody = obj.attackerBody;
            CharacterBody victimBody = obj.victimBody;
            DamageInfo damageInfo = obj.damageInfo;
            if (attackerBody)
            {
                HealthComponent attackerHealthComponent = attackerBody.healthComponent;
                if (attackerHealthComponent) if (damageInfo.HasModdedDamageType(Assets.SoftnessDamageType)) attackerHealthComponent.OverhealFraction(SoftnessHealOnHitPercentage / 100f);
                if (damageInfo.HasModdedDamageType(Assets.ChaosDamageType) && !attackerBody.HasBuff(Assets.ChaosCooldown))
                {
                    BlastAttack blastAttack = new BlastAttack
                    {
                        attacker = obj.attacker,
                        attackerFiltering = AttackerFiltering.Default,
                        baseDamage = attackerBody.damage * ChaosDamageCoefficient,
                        baseForce = ChaosForce,
                        canRejectForce = true,
                        crit = damageInfo.crit,
                        damageColorIndex = DamageColorIndex.Default,
                        falloffModel = BlastAttack.FalloffModel.None,
                        inflictor = damageInfo.inflictor,
                        position = damageInfo.position,
                        damageType = DamageTypeCombo.Generic,
                        procCoefficient = ChaosProcCoefficient,
                        radius = ChaosRadius,
                        teamIndex = obj.attackerTeamIndex,
                    };
                    blastAttack.Fire();
                    attackerBody.AddTimedBuff(Assets.ChaosCooldown, ChaosCooldown);
                }
            }
        }
        private static void GlobalEventManager_onCharacterDeathGlobal(DamageReport obj)
        {
            CharacterBody attackerBody = obj.attackerBody;
            DamageInfo damageInfo = obj.damageInfo;
            if (attackerBody)
            {
                HealthComponent attackerHealthComponent = attackerBody.healthComponent;
                if (attackerHealthComponent) if (damageInfo.HasModdedDamageType(Assets.SoftnessDamageType)) attackerHealthComponent.OverhealFraction(SoftnessHealOnKillPercentage / 100f);
            }
        }

        private static void HealthComponent_TakeDamageProcess(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (damageInfo.attacker && Parry.activeParries.ContainsKey(self.gameObject))
            {
                List<Parry> parries = Parry.activeParries[self.gameObject];
                foreach (Parry parry in parries)
                {
                    parry.OnParry(damageInfo);
                }
                return;
            }
            orig(self, damageInfo);
        }
    }
}
