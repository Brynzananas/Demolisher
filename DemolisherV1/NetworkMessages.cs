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
            NetworkingAPI.RegisterMessageType<VehicleSeatSetPassengerAuthority>();
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
            if (characterMotor == null || characterMotor.isGrounded) return;
            DemolisherFeetEffectsHolder demolisherFeetEffectsHolder = characterMotor.gameObject.GetComponent<DemolisherFeetEffectsHolder>();
            if (demolisherFeetEffectsHolder) return;
            demolisherFeetEffectsHolder = characterMotor.gameObject.AddComponent<DemolisherFeetEffectsHolder>();
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
                //demolisherFeetEffect.characterMotor = characterMotor;
                gameObject.transform.SetParent(transform, false);
                demolisherFeetEffectsHolder.demolisherFeetEffects.Add(demolisherFeetEffect);
                //characterMotor.onHitGroundAuthority += demolisherFeetEffect.OnLanded;
            }
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(networkInstanceId);
        }
    }
    public class VehicleSeatSetPassengerAuthority : INetMessage
    {
        public NetworkIdentity vehicleNetworkIdenity;
        public NetworkIdentity bodyNetworkIdenity;
        public VehicleSeatSetPassengerAuthority()
        {

        }
        public VehicleSeatSetPassengerAuthority(NetworkIdentity vehicleNetworkIdenity, NetworkIdentity bodyNetworkIdenity)
        {
            this.vehicleNetworkIdenity = vehicleNetworkIdenity;
            this.bodyNetworkIdenity = bodyNetworkIdenity;
        }
        public void Deserialize(NetworkReader reader)
        {
            vehicleNetworkIdenity = reader.ReadNetworkIdentity();
            bodyNetworkIdenity = reader.ReadNetworkIdentity();
        }
        public void OnReceived()
        {
            if (!vehicleNetworkIdenity || !bodyNetworkIdenity) return;
            VehicleSeat vehicleSeat = vehicleNetworkIdenity.GetComponent<VehicleSeat>();
            if (!vehicleSeat) return;
            vehicleSeat.SetPassenger(bodyNetworkIdenity.gameObject);
        }
        public void Serialize(NetworkWriter writer)
        {
            writer.Write(vehicleNetworkIdenity);
            writer.Write(bodyNetworkIdenity);
        }
    }
}
