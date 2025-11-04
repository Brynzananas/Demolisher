using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Demolisher
{
    public class NetworkMessages
    {
        public static void Init()
        {
            NetworkingAPI.RegisterMessageType<FeetEffectNetMessage>();
        }
    }
    public class FeetEffectNetMessage : INetMessage
    {
        public NetworkInstanceId networkInstanceId;
        public FeetEffectNetMessage()
        {

        }
        public FeetEffectNetMessage(NetworkInstanceId networkInstanceId)
        {
            this.networkInstanceId = networkInstanceId;
        }
        public void Deserialize(NetworkReader reader)
        {
            networkInstanceId = reader.ReadNetworkId();
        }
        public void OnReceived()
        {
            GameObject bodyObject = Util.FindNetworkObject(networkInstanceId);
            CharacterBody characterBody = bodyObject ? bodyObject.GetComponent<CharacterBody>() : null;
            if (characterBody == null) return;
            CharacterMotor characterMotor = characterBody.characterMotor;
            if (characterMotor == null) return;
            GameObject modelObject = characterBody.modelLocator?.modelTransform?.gameObject;
            if (modelObject == null) return;
            ChildLocator childLocator = modelObject.GetComponent<ChildLocator>();
            if (childLocator == null) return;
            Transform footR = childLocator.FindChild("FootR");
            Transform footL = childLocator.FindChild("FootL");
            if (footR) AddEffect(footR);
            if (footL) AddEffect(footL);
            void AddEffect(Transform transform)
            {
                if (transform.Find(Assets.FeetEffect.name)) return;
                GameObject gameObject = GameObject.Instantiate(Assets.FeetEffect);
                gameObject.name = Assets.FeetEffect.name;
                DemolisherFeetEffect demolisherFeetEffect = gameObject.GetComponent<DemolisherFeetEffect>();
                demolisherFeetEffect.characterMotor = characterMotor;
                gameObject.transform.SetParent(transform, false);
                characterMotor.onHitGroundAuthority += demolisherFeetEffect.OnLanded;
            }
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(networkInstanceId);
        }
    }
    public class DemolisherTallSwordNetMessage : INetMessage
    {
        private DemolisherSwordPillarProjectile _projectileController;
        private int _skillIndex;
        public DemolisherTallSwordNetMessage()
        {

        }
        public DemolisherTallSwordNetMessage(DemolisherSwordPillarProjectile projectileController, DemolisherWeaponSkillDef demolisherWeaponSkillDef)
        {
            _projectileController = projectileController;
            _skillIndex = demolisherWeaponSkillDef.skillIndex;
        }
        public void Deserialize(NetworkReader reader)
        {
            _projectileController = reader.ReadGameObject().GetComponent<DemolisherSwordPillarProjectile>();
            _skillIndex = reader.ReadInt32();
        }

        public void OnReceived()
        {
            SkillDef skillDef = SkillCatalog.GetSkillDef(_skillIndex);
            if (!skillDef) return;
            DemolisherWeaponSkillDef demolisherWeaponSkillDef = skillDef as DemolisherWeaponSkillDef;
            if (!demolisherWeaponSkillDef) return;
            DemolisherBulletAttackWeaponDef demolisherBulletAttackWeaponDef = demolisherWeaponSkillDef.demolisherWeaponDef ? demolisherWeaponSkillDef.demolisherWeaponDef as DemolisherBulletAttackWeaponDef : null;
            if (!demolisherBulletAttackWeaponDef) return;
            object attack = (object)_projectileController.bulletAttack;
            demolisherBulletAttackWeaponDef.OneTimeModification(_projectileController.projectileController, ref attack);
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(_projectileController.gameObject);
            writer.Write(_skillIndex);
        }
    }
    public class VoicelineNetMessage : INetMessage
    {
        int id;
        public void Deserialize(NetworkReader reader)
        {
            throw new NotImplementedException();
        }

        public void OnReceived()
        {
            VoicelineDef voicelineDef = VoicelineDef.voicelineDefs[id];
            if (voicelineDef == null) return;
        }

        public void Serialize(NetworkWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
