using R2API;
using RoR2;
using RoR2.Achievements;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Demolisher
{
    [RegisterAchievement("DemolisherSpeedrun", "DemolisherFlyUnlock", null, 3U, null)]
    public class DemolisherSpeedrunAchievement : BaseAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex() => Assets.DemolisherBodyIndex;
        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            Hooks.onGroundHitOnTime += CheckGround;
        }
        public void CheckGround(Collider collider)
        {
            TeleporterInteraction teleporterInteraction = collider.transform.parent ? collider.transform.parent.GetComponent<TeleporterInteraction>() : null;
            if (teleporterInteraction != null) Grant();
        }
        public override void OnBodyRequirementBroken()
        {
            Hooks.onGroundHitOnTime -= CheckGround;
            base.OnBodyRequirementBroken();
        }
    }
    [RegisterAchievement("DemolisherWorldKill", "DemolisherHookLauncherUnlock", null, 3U, typeof(OutOfBoundsBossKillServer))]
    public class DemolisherWorldKillAchievement : BaseAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex() => Assets.DemolisherBodyIndex;
        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            base.SetServerTracked(true);
        }
        public override void OnBodyRequirementBroken()
        {
            base.SetServerTracked(false);
            base.OnBodyRequirementBroken();
        }
        public class OutOfBoundsBossKillServer : BaseServerAchievement
        {
            public override void OnInstall()
            {
                base.OnInstall();
                GlobalEventManager.onCharacterDeathGlobal += OnCharacterDeath;
            }
            public override void OnUninstall()
            {
                GlobalEventManager.onCharacterDeathGlobal -= OnCharacterDeath;
                base.OnUninstall();
            }

            public void OnCharacterDeath(DamageReport damageReport)
            {
                if (!(damageReport.damageInfo.damageType.damageType.HasFlag(DamageType.OutOfBounds) || damageReport.damageInfo.damageType.damageTypeExtended.HasFlag(DamageTypeExtended.OutOfBounds)) || !damageReport.victim) return;
                CharacterBody body = damageReport.victim.body;
                if (!body || !body.isBoss) return;
                Grant();
            }
        }
    }
    [RegisterAchievement("DemolisherFlyingEnemyKill", "DemolisherSlicingUnlock", null, 3U, typeof(FlyingEnemyKillServer))]
    public class DemolisherFlyingEnemyKillAchievement : BaseAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex() => Assets.DemolisherBodyIndex;
        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            base.SetServerTracked(true);
        }
        public override void OnBodyRequirementBroken()
        {
            base.SetServerTracked(false);
            base.OnBodyRequirementBroken();
        }
        public static int count = 500;
        public class FlyingEnemyKillServer : BaseServerAchievement
        {
            public override void OnInstall()
            {
                base.OnInstall();
                GlobalEventManager.onCharacterDeathGlobal += OnCharacterDeath;
            }
            public override void OnUninstall()
            {
                GlobalEventManager.onCharacterDeathGlobal -= OnCharacterDeath;
                base.OnUninstall();
            }

            public void OnCharacterDeath(DamageReport damageReport)
            {
                if (!(damageReport.damageInfo.HasModdedDamageType(Assets.SharpnessDamageType) || damageReport.damageInfo.HasModdedDamageType(Assets.SoftnessDamageType) || damageReport.damageInfo.HasModdedDamageType(Assets.ChaosDamageType)) || !damageReport.victim) return;
                CharacterBody body = damageReport.victim.body;
                if (!body || !body.isFlying) return;
                count++;
                if (count < DemolisherFlyingEnemyKillAchievement.count) return;
                Grant();
            }
            public int count;
        }
    }
    [RegisterAchievement("DemolisherRangedFarKill", "DemolisherCollapseUnlock", null, 3U, typeof(RangedFarKillServer))]
    public class DemolisherRangedFarKillAchievement : BaseAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex() => Assets.DemolisherBodyIndex;
        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            base.SetServerTracked(true);
        }
        public override void OnBodyRequirementBroken()
        {
            base.SetServerTracked(false);
            base.OnBodyRequirementBroken();
        }
        public static float requiredDistance = 128f;
        public class RangedFarKillServer : BaseServerAchievement
        {
            public override void OnInstall()
            {
                base.OnInstall();
                GlobalEventManager.onCharacterDeathGlobal += OnCharacterDeath;
            }
            public override void OnUninstall()
            {
                GlobalEventManager.onCharacterDeathGlobal -= OnCharacterDeath;
                base.OnUninstall();
            }

            public void OnCharacterDeath(DamageReport damageReport)
            {
                if (!damageReport.attackerBody || !damageReport.damageInfo.inflictor || !damageReport.victim) return;
                CharacterBody body = damageReport.victim.body;
                if (!body) return;
                ProjectileRemoteDetonation projectileRemoteDetonation = damageReport.damageInfo.inflictor.GetComponent<ProjectileRemoteDetonation>();
                if (!projectileRemoteDetonation) return;
                Vector3 vector3 = damageReport.attackerBody.footPosition - body.footPosition;
                if (vector3.sqrMagnitude < requiredDistance * requiredDistance) return;
                Grant();
            }
        }
    }
    [RegisterAchievement("DemolisherChargeDistance", "DemolisherChaindashUnlock", null, 3U, null)]
    public class DemolisherChargeDistanceAchievement : BaseAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex() => Assets.DemolisherBodyIndex;
        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            ShieldCharge.onChargeEndGiveSqrTraveledDistance += CheckDistance;
        }

        private void CheckDistance(float obj)
        {
            if (obj >= requiredDistance * requiredDistance) Grant();
        }

        public override void OnBodyRequirementBroken()
        {
            ShieldCharge.onChargeEndGiveSqrTraveledDistance -= CheckDistance;
            base.OnBodyRequirementBroken();
        }
        public static float requiredDistance = 64f;
    }
    [RegisterAchievement("DemolisherMastery", "DemolisherMasteryUnlock", null, 3U, null)]
    public class DemolisherMasteryAchievement : BasePerSurvivorClearGameMonsoonAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex() => Assets.DemolisherBodyIndex;
    }
    [RegisterAchievement("DemolisherFuelArrayCellWin", "DemolisherFuelArrayCellWinUnlock", null, 3U, null)]
    public class DemolisherFuelArrayCellWinAchievement : BaseAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex() => Assets.DemolisherBodyIndex;
        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            Run.onClientGameOverGlobal += this.OnClientGameOverGlobal;
        }
        public override void OnBodyRequirementBroken()
        {
            Run.onClientGameOverGlobal -= this.OnClientGameOverGlobal;
            base.OnBodyRequirementBroken();
        }
        private void OnClientGameOverGlobal(Run run, RunReport runReport)
        {
            if (!runReport.gameEnding)
            {
                return;
            }
            if (runReport.gameEnding.isWin)
            {
                DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(runReport.ruleBook.FindDifficulty());
                if (runReport.gameEnding.cachedName == "MainEnding" && difficultyDef != null && difficultyDef.countsAsHardMode)
                {
                    foreach (RunReport.PlayerInfo playerInfo in runReport.playerInfos)
                    {
                        CharacterMaster characterMaster = playerInfo.master;
                        if (!characterMaster) continue;
                        if (!characterMaster.hasAuthority) continue;
                        Inventory inventory = characterMaster.inventory;
                        if (!inventory) continue;
                        foreach (EquipmentState[] equipmentStates in inventory._equipmentStateSlots) foreach (EquipmentState equipmentState in equipmentStates)  if (equipmentState.equipmentIndex == RoR2Content.Equipment.QuestVolatileBattery.equipmentIndex) Grant();
                    }
                }
            }
        }
    }
}
