using BrynzaAPI;
using EmotesAPI;
using EntityStates;
using EntityStates.Bison;
using EntityStates.Engi.EngiWeapon;
using HG;
using R2API;
using RoR2;
using RoR2.Audio;
using RoR2.CharacterAI;
using RoR2.Navigation;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Xsl;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements.StyleSheets;
using static Rewired.ComponentControls.Effects.RotateAroundAxis;
using static RoR2.BodyAnimatorSmoothingParameters;
using static RoR2.CameraRigController;
using static UnityEngine.ParticleSystem.PlaybackState;
using static UnityEngine.SendMouseEvents;

namespace Demolisher
{
    public interface IStateTarget
    {
        public bool taken { get; set; }
        public EntityState entityState { get; }
    }
    public interface IStateCharge
    {
        public float charge { get; set; }
        public float maxCharge { get; set; }
        public float chargePercentage
        {
            get
            {
                return charge / (maxCharge == 0f ? 1f : maxCharge);
            }
        }
    }
    public class DemolisherMainState : GenericCharacterMain
    {
        public static float spread = 1.5f;
        public bool fireUtilitySkill;
        public bool swapped;
        public bool swapping;
        public GenericSkill utilitySkill => skillLocator && skillLocator.utility ? skillLocator.utility : null;
        public GenericSkill secondarySkill => skillLocator && skillLocator.secondary ? skillLocator.secondary : null;
        public GenericSkill specialSkill => skillLocator && skillLocator.special ? skillLocator.special : null;
        private DemolisherComponent _demolisherComponent;
        private bool demolisherComponentMissing;
        public DemolisherComponent demolisherComponent
        {
            get
            {
                if (demolisherComponentMissing) return null;
                if (!_demolisherComponent) _demolisherComponent = GetComponent<DemolisherComponent>();
                if (!_demolisherComponent) demolisherComponentMissing = true;
                return _demolisherComponent;
            }
        }
        public bool canSwapBySprint => characterBody.HasModdedBodyFlag(BrynzaAPI.Assets.SprintAllTime);
        public override void OnEnter()
        {
            base.OnEnter();
            DemolisherEntityStateMachine demolisherEntityStateMachine = outer as DemolisherEntityStateMachine;
            _demolisherComponent = demolisherEntityStateMachine ? demolisherEntityStateMachine.commonDemolisherComponents.demolisherComponent : null;
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (inputBank)
            {
                InputBankTest.ButtonState utilityButtonState = inputBank.skill3;
                InputBankTest.ButtonState secondaryButtonState = inputBank.skill2;
                InputBankTest.ButtonState specialButtonState = inputBank.skill4;
                //if (inputBank.sprint.justReleased && CanExecuteSkill(utilitySkill) && utilitySkill.ExecuteIfReady()) utilityButtonState.hasPressBeenClaimed = true;
                if (canSwapBySprint)
                {
                    if (inputBank.sprint.justPressed) SwapWeapons();
                }
                else
                {
                    if (utilityButtonState.justPressed)
                    {
                        StartSwapping();
                        fireUtilitySkill = false;
                        swapped = false;
                        swapping = true;
                    }
                    if (swapping && (secondaryButtonState.justPressed || specialButtonState.justPressed))
                    {
                        swapped = true;
                        SwapWeapons();
                    }
                    if (utilityButtonState.justReleased)
                    {
                        swapping = false;
                        if (!swapped) fireUtilitySkill = true;
                        StopSwapping();
                        if (utilitySkill) if (CanExecuteSkill(utilitySkill) && utilitySkill.ExecuteIfReady()) utilityButtonState.hasPressBeenClaimed = true;
                    }
                }
            }
        }
        public virtual void StartSwapping()
        {

        }
        public virtual void StopSwapping()
        {

        }
        public override bool CanExecuteSkill(GenericSkill skillSlot)
        {
            if (!canSwapBySprint && skillSlot)
            {
                if (utilitySkill && skillSlot == utilitySkill && !fireUtilitySkill) return false;
                if (swapping && (skillSlot == secondarySkill || skillSlot == specialSkill)) return false;
            }
            return base.CanExecuteSkill(skillSlot);
        }
        public virtual void SwapWeapons()
        {
            if (demolisherComponent == null) return;
            characterBody.AddSpreadBloom(spread);
            demolisherComponent.CallSwapWeapons();
        }
    }
    public class DemolisherBaseState : BaseSkillState
    {
        private DemolisherComponent _demolisherComponent;
        private bool demolisherComponentMissing;
        public DemolisherComponent demolisherComponent
        {
            get
            {
                if (demolisherComponentMissing) return null;
                if (!_demolisherComponent) _demolisherComponent = GetComponent<DemolisherComponent>();
                if (!_demolisherComponent) demolisherComponentMissing = true;
                return _demolisherComponent;
            }
        }
        private DemolisherVoicelinesComponent _demolisherVoicelinesComponent;
        private bool demolisherVoicelinesComponentMissing;
        public DemolisherVoicelinesComponent demolisherVoicelinesComponent
        {
            get
            {
                if (demolisherVoicelinesComponentMissing) return null;
                if (!_demolisherVoicelinesComponent) _demolisherVoicelinesComponent = GetComponent<DemolisherVoicelinesComponent>();
                if (!_demolisherVoicelinesComponent) demolisherVoicelinesComponentMissing = true;
                return _demolisherVoicelinesComponent;
            }
        }
        public DemolisherModelLocator demolisherModelLocator { get; private set; }
        private DemolisherModel _demolisherModel;
        private bool demolisherModelMissing;
        public DemolisherModel demolisherModel
        {
            get
            {
                if (demolisherModelMissing) return null;
                if (!_demolisherModel) _demolisherModel = GetModelTransform() ? GetModelTransform().GetComponent<DemolisherModel>() : null;
                if (!_demolisherModel) demolisherModelMissing = true;
                return _demolisherModel;
            }
        }
        public DemolisherBulletAttackWeaponDef currentMeleeWeaponDef;
        public DemolisherProjectileWeaponDef currentRangedWeaponDef;
        public GenericSkill meleeSkill;
        public GenericSkill rangedSkill;
        public Vector3 aimDirectionGrounded
        {
            get
            {
                Vector3 vector3 = GetAimRay().direction;
                vector3.y = 0;
                return vector3.normalized;
            }
        }
        public override void OnEnter()
        {
            base.OnEnter();
            DemolisherEntityStateMachine demolisherEntityStateMachine = outer as DemolisherEntityStateMachine;
            _demolisherComponent = demolisherEntityStateMachine ? demolisherEntityStateMachine.commonDemolisherComponents.demolisherComponent : null;
            _demolisherVoicelinesComponent = demolisherEntityStateMachine ? demolisherEntityStateMachine.commonDemolisherComponents.demolisherVoicelinesComponent : null;
            demolisherModelLocator = characterBody.modelLocator ? characterBody.modelLocator as DemolisherModelLocator : null;
            _demolisherModel = demolisherEntityStateMachine ? demolisherEntityStateMachine.commonDemolisherComponents.demolisherModel : null;
            AssignWeapons();
        }
        public virtual void AssignWeapons()
        {
            if (demolisherComponent)
            {
                currentMeleeWeaponDef = GetCurrentWeapon<DemolisherBulletAttackWeaponDef>(activatorSkillSlot, out meleeSkill) ?? GetCurrentWeapon<DemolisherBulletAttackWeaponDef>(demolisherComponent.meleeWeapon, out meleeSkill);
                currentRangedWeaponDef = GetCurrentWeapon<DemolisherProjectileWeaponDef>(activatorSkillSlot, out rangedSkill) ?? GetCurrentWeapon<DemolisherProjectileWeaponDef>(demolisherComponent.rangedWeapon, out rangedSkill);
            }
            //if (skillLocator)
            //    foreach (GenericSkill genericSkill in skillLocator.allSkills)
            //    {
            //        if (genericSkill == null) continue;
            //        if (genericSkill.baseSkill && genericSkill.baseSkill is DemolisherWeaponSkillDef)
            //        {
            //            DemolisherWeaponSkillDef demolisherSkillDef = genericSkill.baseSkill as DemolisherWeaponSkillDef;
            //            DemolisherWeaponDef demolisherWeaponDef = demolisherSkillDef.demolisherWeaponDef;
            //            if (demolisherWeaponDef == null) continue;
            //            if (currentMeleeWeaponDef == null && demolisherWeaponDef is DemolisherBulletAttackWeaponDef) currentMeleeWeaponDef = demolisherWeaponDef as DemolisherBulletAttackWeaponDef;
            //            if (currentRangedWeaponDef == null && demolisherWeaponDef is DemolisherProjectileWeaponDef) currentRangedWeaponDef = demolisherWeaponDef as DemolisherProjectileWeaponDef;
            //        }
            //    }
        }
        public T GetCurrentWeapon<T>(GenericSkill genericSkill, out GenericSkill genericSkill1) where T : DemolisherWeaponDef
        {
            genericSkill1 = genericSkill;
            return genericSkill.baseSkill && genericSkill.baseSkill is DemolisherWeaponSkillDef && (genericSkill.baseSkill as DemolisherWeaponSkillDef).demolisherWeaponDef ? (genericSkill.baseSkill as DemolisherWeaponSkillDef).demolisherWeaponDef as T : null;
        }
        public DamageSource GetDamageSource()
        {
            if (activatorSkillSlot == null || skillLocator == null) return DamageSource.NoneSpecified;
            if (skillLocator.primary == activatorSkillSlot) return DamageSource.Primary;
            if (skillLocator.secondary == activatorSkillSlot) return DamageSource.Secondary;
            if (skillLocator.utility == activatorSkillSlot) return DamageSource.Utility;
            if (skillLocator.special == activatorSkillSlot) return DamageSource.Special;
            return DamageSource.NoneSpecified;
        }
    }
    public abstract class BaseMeleeAttack : DemolisherBaseState
    {
        public abstract DamageSource damageSource { get; }
        public DemolisherBulletAttack bulletAttack;
        public override void OnEnter()
        {
            base.OnEnter();
        }
        public virtual void CreateBulletAttack(bool ignoreHitTargets = true)
        {
            bulletAttack = new DemolisherBulletAttack
            {
                bulletCount = 1,
                damageColorIndex = DamageColorIndex.Default,
                damageType = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, damageSource),
                falloffModel = BulletAttack.FalloffModel.None,
                owner = gameObject,
                smartCollision = true,
                weapon = gameObject,
                stopperMask = LayerIndex.ui.mask,
                hitMask = LayerIndex.entityPrecise.mask,
            };
            bulletAttack.SetIgnoreHitTargets(ignoreHitTargets);
            object attack = bulletAttack;
            currentMeleeWeaponDef?.OneTimeModification(this, ref attack);
        }
        public virtual void UpdateBulletAttack(float damage, float procCoefficient, float effectCoefficient, float force, bool crit, float radius, float distance, bool reset)
        {
            bulletAttack.damage = damage;
            bulletAttack.procCoefficient = procCoefficient;
            bulletAttack.force = force;
            bulletAttack.isCrit = crit;
            bulletAttack.radius = radius;
            bulletAttack.maxDistance = distance;
            bulletAttack.effectCoefficient = effectCoefficient;
            if (reset) bulletAttack.ResetIgnoredHealthComponents();
            object attack = bulletAttack;
            currentMeleeWeaponDef?.ModifyAttack(this, ref attack);
        }
        public virtual void ConstantUpdateBulletAttack(Ray ray)
        {
            bulletAttack.aimVector = ray.direction;
            bulletAttack.origin = ray.origin;
        }
        public virtual void ContinueFireMeleeAttack(Ray ray)
        {
            ConstantUpdateBulletAttack(ray);
            if (isAuthority) bulletAttack.Fire();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
    public abstract class BaseProjectileAttack : DemolisherBaseState
    {
        public static float recoil = 1f;
        public static float spread = 1.5f;
        public abstract float damageCoefficient { get; }
        public abstract float force { get; }
        public GameObject projectile => currentRangedWeaponDef ? currentRangedWeaponDef.projectile : Assets.GrenadeProjectile;
        public abstract DamageSource damageSource { get; }
        public abstract float fuse { get; }
        public virtual void FireProjectile(Ray ray, float damage, float force, bool crit)
        {
            AddRecoil(-recoil, -recoil, 0f, 0f);
            characterBody.AddSpreadBloom(spread);
            StartAimMode(2f, !characterBody.isSprinting);
            if (currentRangedWeaponDef.fireSound != null) Util.PlaySound(currentRangedWeaponDef.fireSound, gameObject);
            if (skillLocator && skillLocator.primary == activatorSkillSlot)
            {
                PlayAnimation("Gesture, Override", "ShootGun");
            }
            else
            {
                PlayAnimation("Gun, Override", "ShootGun");
            }
            if (base.isAuthority)
            {
                DamageTypeCombo damageTypeCombo = new DamageTypeCombo(currentRangedWeaponDef ? currentRangedWeaponDef.damageType : DamageType.Generic, currentRangedWeaponDef ? currentRangedWeaponDef.damageTypeExtended : DamageTypeExtended.Generic, damageSource);
                if (currentRangedWeaponDef && currentRangedWeaponDef.moddedDamageTypes != null) foreach (DamageAPI.ModdedDamageType moddedDamageType in currentRangedWeaponDef.moddedDamageTypes) damageTypeCombo.AddModdedDamageType(moddedDamageType);
                //TrajectoryAimAssist.ApplyTrajectoryAimAssist(ref ray, projectile, gameObject, 1f);
                FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                {
                    projectilePrefab = projectile,
                    position = ray.origin,
                    rotation = Util.QuaternionSafeLookRotation(ray.direction),
                    owner = gameObject,
                    damage = damage * damageCoefficient,
                    force = force,
                    crit = crit,
                    fuseOverride = fuse,
                    damageTypeOverride = new DamageTypeCombo?(damageTypeCombo),
                };
                this.ModifyProjectileInfo(ref fireProjectileInfo);
                ProjectileManager.instance.FireProjectile(fireProjectileInfo);
            }
        }
        public virtual void ModifyProjectileInfo(ref FireProjectileInfo fireProjectileInfo)
        {
            object attack = fireProjectileInfo;
            if (currentRangedWeaponDef) currentRangedWeaponDef.ModifyAttack(this, ref attack);
            if (attack is FireProjectileInfo fireProjectileInfo2)
            {
                fireProjectileInfo = fireProjectileInfo2;
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
    /*public class FireGrenadeNetwork : BaseProjectileAttack, IStateTarget
    {
        public override float damageCoefficient => damageCoefficientTransfer;
        public override float force => forceTransfer;
        public override DamageSource damageSource => GetDamageSource();
        private bool _taken;
        public bool taken { get => _taken; set => _taken = value; }
        public EntityState entityState => this;

        public float damageCoefficientTransfer;
        public float forceTransfer;
        public float baseDurationTransfer;
        public float duration;
        public float stopwatch;
        public virtual void SetValues()
        {
            duration = baseDurationTransfer / characterBody.attackSpeed;
        }
        public override void OnEnter()
        {
            base.OnEnter();
            Ray ray = GetAimRay();
            FireProjectile(ray, characterBody.damage, force, RollCrit());
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.fixedDeltaTime;
            if (stopwatch >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }
        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            damageCoefficientTransfer = reader.ReadSingle();
            forceTransfer = reader.ReadSingle();
            baseDurationTransfer = reader.ReadSingle();
        }
        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(damageCoefficientTransfer);
            writer.Write(forceTransfer);
            writer.Write(baseDurationTransfer);
        }
    }
    public class FireGrenadeHoldNetwork : FireGrenadeNetwork, IStateCharge
    {
        private float _charge;
        private float _maxCharge;
        public float charge { get => _charge; set => _charge = value; }
        public float maxCharge { get => _maxCharge; set => _maxCharge = value; }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            charge = reader.ReadSingle();
            maxCharge = reader.ReadSingle();
        }
        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(charge);
            writer.Write(maxCharge);
        }
    }*/
    public class FireGrenade : BaseProjectileAttack
    {
        public override float damageCoefficient => FireGrenadeConfig.damageCoefficient.Value;
        public override float force => FireGrenadeConfig.force.Value;
        public override DamageSource damageSource => GetDamageSource();

        public static float baseDuration => FireGrenadeConfig.baseDuration.Value;
        public override float fuse => fuseNew;
        public float fuseNew = -1f;
        public float duration;
        public float stopwatch;
        public virtual void SetValues()
        {
            duration = baseDuration / characterBody.attackSpeed;
            if (currentRangedWeaponDef) duration /= currentRangedWeaponDef.attackSpeedMultiplier;
        }
        public override void OnEnter()
        {
            base.OnEnter();
            SetValues();
            Ray ray = GetAimRay();
            FireProjectile(ray, characterBody.damage, force, RollCrit());
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.fixedDeltaTime;
            if (stopwatch >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }
    }
    public class FireGrenadeHold : DemolisherBaseState
    {
        public static float damageCoefficient => FireGrenadeConfig.damageCoefficient.Value;
        public static float force => FireGrenadeConfig.force.Value;
        public static float maxCharge = 1f;
        public float charge;
        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound("Play_loose_cannon_charge", gameObject);
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            //charge += Time.fixedDeltaTime * characterBody.attackSpeed;
            if (demolisherComponent) demolisherComponent.overrideGeneralMeter = 1f - (fixedAge / maxCharge);
            if (isAuthority && (!IsKeyDownAuthority() || fixedAge >= maxCharge))
            {
                outer.SetNextState(new FireGrenade { activatorSkillSlot = activatorSkillSlot, fuseNew = maxCharge - fixedAge });
            }
        }
        public override void OnExit()
        {
            base.OnExit();
            Util.PlaySound("Stop_loose_cannon_charge", gameObject);
            if (demolisherComponent) demolisherComponent.overrideGeneralMeter = 1f;
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
    public class FireGrenadeAndHold : BaseProjectileAttack, IStateTarget
    {
        public override float damageCoefficient => FireGrenadeConfig.damageCoefficient.Value;
        public override float force => FireGrenadeConfig.force.Value;
        public override DamageSource damageSource => GetDamageSource();
        public bool _taken;
        public bool taken { get => _taken; set => _taken = value; }
        public EntityState entityState => this;
        public override float fuse => -1f;

        public override void OnEnter()
        {
            base.OnEnter();
            Ray ray = GetAimRay();
            FireProjectile(ray, characterBody.damage, force, RollCrit());
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority && !IsKeyDownAuthority())
            {
                outer.SetNextStateToMain();
            }
        }
        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(taken);
        }
        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            taken = reader.ReadBoolean();
        }
    }
    public class MediumMeleeAttackWindUp : DemolisherBaseState, SteppedSkillDef.IStepSetter
    {
        public static float baseDuration => MediumMeleeAttackConfig.baseDuration.Value;
        public static float swingUpCrossfade = 0.2f;
        public float duration;
        public bool noStep;
        public bool fired;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            if (currentMeleeWeaponDef) duration /= currentMeleeWeaponDef.attackSpeedMultiplier;
            PlayCrossfade("Gesture, Override", noStep ? "SwingUp1" : "SwingUp2", "Slash.playbackRate", duration, swingUpCrossfade / attackSpeedStat);
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            StartAimMode();
            if (!isAuthority) return;
            if (fixedAge >= duration || characterBody.GetClientBuffCount(Assets.InstantMeleeSwing) > 0)
            {
                fired = true;
                outer.SetState(new MediumMeleeAttackWindDown { activatorSkillSlot = activatorSkillSlot, noStep = noStep });
            }
        }
        public override void OnExit()
        {
            base.OnExit();
            if (!fired) PlayAnimation("Gesture, Override", "BufferEmpty");
        }
        public void SetStep(int i) => noStep = i % 2 == 0;
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
    public class MediumMeleeAttackWindDown : BaseMeleeAttack
    {
        public static float damageCoefficient => MediumMeleeAttackConfig.damageCoefficient.Value;
        public static float procCoefficient => MediumMeleeAttackConfig.procCoefficient.Value;
        public static float effectCoefficient => MediumMeleeAttackConfig.effectCoefficient.Value;
        public static float baseAttackDuration => MediumMeleeAttackConfig.baseAttackDuration.Value;
        public static float radius => MediumMeleeAttackConfig.radius.Value;
        public static float force => MediumMeleeAttackConfig.force.Value;
        public static float maxDistance => MediumMeleeAttackConfig.maxDistance.Value;
        public static float hitJump => MediumMeleeAttackConfig.hitJump.Value;
        public override DamageSource damageSource => GetDamageSource();
        public static float recoil = 1f;
        public static float spread = 1.5f;
        public static float attackSpeedRampUpRate = 0.25f;
        public static float maxAttackSpeedRampUp = 3f;
        public static float swingDownCrossfade = 0.05f;
        public static float bufferEmptyTransition = 0.2f;
        public static float effectRotation = 35f;
        private bool hitTarget;
        public bool instantSwing;
        public bool noStep;
        public float stopwatch;
        public float firingStopwatch;
        public GameObject slash;
        public bool firing;
        public float attackDuration;
        public Transform effectTransform;
        public override void OnEnter()
        {
            base.OnEnter();
            attackDuration = baseAttackDuration / attackSpeedStat;
            if (currentMeleeWeaponDef) attackDuration /= currentMeleeWeaponDef.attackSpeedMultiplier;
            CreateBulletAttack();
            UpdateBulletAttack(characterBody.damage * damageCoefficient, procCoefficient, effectCoefficient, force, RollCrit(), radius, maxDistance, true);
            Transform transform = characterBody.aimOriginTransform ?? characterBody.transform;
            EffectData effectData = new EffectData
            {
                rootObject = transform.gameObject,
                genericFloat = 1f / attackDuration
            };
            EffectManager.SpawnEffect(Assets.SlashEffect.index, effectData, false);
            effectTransform = effectData.GetEffectInstance() ? effectData.GetEffectInstance().transform : null;
            PlayCrossfade("Gesture, Override", noStep ? "SwingDown1" : "SwingDown2", "Slash.playbackRate", attackDuration, swingDownCrossfade);
            Util.PlaySound("Play_HorseMan_SpearWoosh", gameObject);
            //Util.PlaySound("Play_DemoSwordSwing", gameObject);
            AddRecoil(recoil, recoil, noStep ? -recoil : recoil, noStep ? -recoil : recoil);
            characterBody.AddSpreadBloom(spread);
            if (!isAuthority) return;
            characterBody.SetClientBuffCount(Assets.InstantMeleeSwing, 0);
        }
        public override void Update()
        {
            base.Update();
            if (effectTransform)
            {
                Ray ray = GetAimRay();
                Vector3 vector3 = Quaternion.LookRotation(ray.direction).eulerAngles;
                vector3.z += noStep ? -effectRotation : effectRotation;
                float radius = bulletAttack.radius;
                float maxDistance = bulletAttack.maxDistance;
                effectTransform.localScale = new Vector3(noStep ? radius : -radius, radius, maxDistance);
                effectTransform.eulerAngles = vector3;
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            StartAimMode();
            Ray ray = GetAimRay();
            ContinueFireMeleeAttack(ray);
            if (!isAuthority) return;
            if (fixedAge >= attackDuration || characterBody.GetClientBuffCount(Assets.InstantMeleeSwing) > 0)
            {
                outer.SetNextStateToMain();
            }
        }
        public override void CreateBulletAttack(bool ignoreHitTargets = true)
        {
            base.CreateBulletAttack(ignoreHitTargets);
            bulletAttack.hitCallback += OnHitCallBack;
        }
        private bool OnHitCallBack(BulletAttack bulletAttack, ref BulletAttack.BulletHit hitInfo)
        {
            if (hitTarget || !hitInfo.hitHurtBox || hitJump <= 0f) return false;
            HealthComponent healthComponent = hitInfo.hitHurtBox.healthComponent;
            if (!healthComponent || healthComponent == this.healthComponent || !healthComponent.wasAlive) return false;
            hitTarget = true;
            float y = hitJump * Physics.gravity.y * -1f;
            if (characterMotor)
            {
                if (characterMotor.isGrounded) return false;
                if (characterMotor.velocity.y > y)
                {
                    y = characterMotor.velocity.y;
                }
                else if (characterMotor.velocity.y < 0f)
                {

                }
                else
                {
                    y = Mathf.Lerp(y, characterMotor.velocity.y, characterMotor.velocity.y / y);
                }
                characterMotor.velocity.y = y;
            }
            else if (rigidbody)
            {
                if (rigidbody.velocity.y > y)
                {
                    y = rigidbody.velocity.y;
                }
                else if (rigidbody.velocity.y < 0f)
                {

                }
                else
                {
                    y = Mathf.Lerp(y, characterMotor.velocity.y, rigidbody.velocity.y / y);
                }
                Vector3 vector3 = rigidbody.velocity;
                vector3.y = y;
                rigidbody.velocity = vector3;
            }
            return false;
        }
    }
    public class MediumMeleeAttack : BaseMeleeAttack
    {
        public static float damageCoefficient => MediumMeleeAttackConfig.damageCoefficient.Value;
        public static float procCoefficient => MediumMeleeAttackConfig.procCoefficient.Value;
        public static float effectCoefficient => MediumMeleeAttackConfig.effectCoefficient.Value;
        public static float baseAttackDuration => MediumMeleeAttackConfig.baseAttackDuration.Value;
        public static float baseDuration => MediumMeleeAttackConfig.baseDuration.Value;
        public static float radius => MediumMeleeAttackConfig.radius.Value;
        public static float force => MediumMeleeAttackConfig.force.Value;
        public static float maxDistance => MediumMeleeAttackConfig.maxDistance.Value;
        public static float hitJump => MediumMeleeAttackConfig.hitJump.Value;
        public static float recoil = 1f;
        public static float spread = 1.5f;
        public static float attackSpeedRampUpRate = 0.25f;
        public static float maxAttackSpeedRampUp = 3f;
        public static float swingUpCrossfade = 0.2f;
        public static float swingDownCrossfade = 0.05f;
        public static float bufferEmptyTransition = 0.2f;
        public static float effectRotation = 35f;
        public override DamageSource damageSource => GetDamageSource();
        private bool hitTarget;
        public bool instantSwing;
        public bool step;
        public float duration;
        public float stopwatch;
        public float firingStopwatch;
        public GameObject slash;
        public bool firing;
        public float attackDuration;
        public Transform effectTransform;
        public override void OnEnter()
        {
            base.OnEnter();
            //ChildLocator childLocator = GetModelChildLocator();
            //if (childLocator) effectTransform = childLocator.FindChild("Rotator");
            //if (!effectTransform) effectTransform = characterBody.coreTransform;
            SetValues();
            CreateBulletAttack();
            PlayCrossfade("Gesture, Override", "SwingUp1", "Slash.playbackRate", duration, swingUpCrossfade / characterBody.attackSpeed);
        }
        public virtual void SetValues()
        {
            float rampUp = currentMeleeWeaponDef ? currentMeleeWeaponDef.attackSpeedMultiplier : 1f; //Mathf.Min((fixedAge * attackSpeedRampUpRate) + 1f, maxAttackSpeedRampUp);
            attackDuration = baseAttackDuration / characterBody.attackSpeed / rampUp;
            duration = baseDuration / characterBody.attackSpeed / rampUp;
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            StartAimMode();
            stopwatch += Time.fixedDeltaTime;
            if (characterBody.HasBuff(Assets.InstantMeleeSwing))
            {
                StopFiring();
                FireMeleeAttack(attackDuration);
                //stopwatch = duration;
                if (NetworkServer.active) characterBody.SetBuffCount(Assets.InstantMeleeSwing.buffIndex, 0);
            }
            if (!firing)
            {
                if (stopwatch >= duration)
                {
                    FireMeleeAttack(attackDuration);
                }
            }
            else
            {
                if (stopwatch >= attackDuration)
                {
                    StopFiring();
                }
                else
                {
                    ContinueFireMeleeAttack(GetAimRay());
                }
            }
            if (!isAuthority || IsKeyDownAuthority() || stopwatch > 0f || firing)
            {
                return;
            }
            outer.SetNextStateToMain();
        }

        public virtual void FireMeleeAttack(float duration)
        {
            firing = true;
            attackDuration = duration;
            UpdateBulletAttack(characterBody.damage * damageCoefficient, procCoefficient, effectCoefficient, force, RollCrit(), radius, maxDistance, true);
            hitTarget = false;
            //Vector3 rotationEuler = Util.QuaternionSafeLookRotation(bulletAttack.aimVector).eulerAngles;
            //rotationEuler = new Vector3(rotationEuler.x, rotationEuler.y, rotationEuler.z + (step ? effectRotation : 180f - effectRotation));
            Transform transform = characterBody.aimOriginTransform ?? characterBody.transform;
            EffectData effectData = new EffectData
            {
                rootObject = transform.gameObject,
                genericFloat = 1f / duration
            };
            EffectManager.SpawnEffect(Assets.SlashEffect.index, effectData, false);
            effectTransform = effectData.GetEffectInstance() ? effectData.GetEffectInstance().transform : null;
            //slash = GameObject.Instantiate(Assets.Slash);
            PlayCrossfade("Gesture, Override", step ? "SwingDown2" : "SwingDown1", "Slash.playbackRate", duration, swingDownCrossfade);
            Util.PlaySound("Play_DemoSwordSwing", gameObject);
            AddRecoil(recoil, recoil, step ? recoil : -recoil, step ? recoil : -recoil);
            characterBody.AddSpreadBloom(spread);
            //slash.Init(45f, false, bulletAttack.radius, bulletAttack.maxDistance, duration * 2f);
            stopwatch = 0f;
            step = !step;
            SetValues();
        }
        public virtual void StopFiring()
        {
            firing = false;
            stopwatch = 0f;
            PlayCrossfade("Gesture, Override", step ? "SwingUp2" : "SwingUp1", "Slash.playbackRate", duration, swingUpCrossfade);
        }
        public override void Update()
        {
            base.Update();
            if (effectTransform)
            {
                Ray ray = GetAimRay();
                Vector3 vector3 = Quaternion.LookRotation(ray.direction).eulerAngles;
                vector3.z += step ? -effectRotation : effectRotation;
                float radius = bulletAttack.radius;
                float maxDistance = bulletAttack.maxDistance;
                effectTransform.localScale = new Vector3(step ? radius : -radius, radius, maxDistance);
                effectTransform.eulerAngles = vector3;
            }
        }
        public override void OnExit()
        {
            base.OnExit();
            if (!firing) PlayCrossfade("Gesture, Override", "BufferEmpty", "Slash.playbackRate", bufferEmptyTransition, bufferEmptyTransition);
        }
        public override void CreateBulletAttack(bool ignoreHitTargets = true)
        {
            base.CreateBulletAttack(ignoreHitTargets);
            bulletAttack.hitCallback += OnHitCallBack;
        }
        private bool OnHitCallBack(BulletAttack bulletAttack, ref BulletAttack.BulletHit hitInfo)
        {
            if (hitTarget || !hitInfo.hitHurtBox || hitJump <= 0f) return false;
            HealthComponent healthComponent = hitInfo.hitHurtBox.healthComponent;
            if (!healthComponent || healthComponent == this.healthComponent || !healthComponent.wasAlive) return false;
            hitTarget = true;
            float y = hitJump * Physics.gravity.y * -1f;
            if (characterMotor)
            {
                if (characterMotor.isGrounded) return false;
                if (characterMotor.velocity.y > y)
                {
                    y = characterMotor.velocity.y;
                }
                else if (characterMotor.velocity.y < 0f)
                {

                }
                else
                {
                    y = Mathf.Lerp(y, 0f, characterMotor.velocity.y / y);
                }
                characterMotor.velocity.y = y;
            }
            else if (rigidbody)
            {
                if (rigidbody.velocity.y > y)
                {
                    y = rigidbody.velocity.y;
                }
                else if (rigidbody.velocity.y < 0f)
                {

                }
                else
                {
                    y = Mathf.Lerp(y, 0f, rigidbody.velocity.y / y);
                }
                Vector3 vector3 = rigidbody.velocity;
                vector3.y = y;
                rigidbody.velocity = vector3;
            }
            return false;
        }
    }
    public class ShieldCharge : BaseMeleeAttack
    {
        public static float baseDuration => ShieldChargeConfig.baseDuration.Value;
        public static float baseWalkSpeedMultiplier => ShieldChargeConfig.baseWalkSpeedMultiplier.Value;
        public static float shieldBashRadiusMultiplier => ShieldChargeConfig.shieldBashRadiusMultiplier.Value;
        public static float shieldBashDistance => ShieldChargeConfig.shieldBashDistance.Value;
        public static float shieldBashDamageCoefficient => ShieldChargeConfig.shieldBashDamageCoefficient.Value;
        public static float shieldBashSpeedDamageCoefficient => ShieldChargeConfig.shieldBashSpeedDamageCoefficient.Value;
        public static float shieldBashProcCoefficient => ShieldChargeConfig.shieldBashProcCoefficient.Value;
        //public static float shieldBashBaseForce = 200f;
        //public static float shieldBashVelocityForce = 100f;
        //public static float shieldBashTimer = 1f;
        public static float baseVelocityLerpTime = 0.5f;
        public static float shieldBashVelocityForceMultiplier = 1f;
        public static float shieldBashGravityForceMultiplier = 1f;
        public static float extraGroundingDistance = 4f;
        public static float extraStepOffset = 4f;
        public static Vector3 effectScale = new Vector3(1f, 1f, 3f);
        public static Action<float> onChargeEndGiveSqrTraveledDistance;
        public Animator modelAnimator;
        public CharacterAnimParamAvailability characterAnimParamAvailability;
        public float duration;
        public float velocityLerpTime;
        public float walkSpeedMultiplier;
        public Vector3 direction;
        public Vector3 directionVisual;
        private AimAnimator aimAnimator;
        public float traveledDistance;
        public Vector3 previousPosition;
        private bool playedAltVoiceline;
        private EffectComponent effectInstance;
        private Vector3 previousVelocity;
        private float previousVelocityMagnitude;
        private float speed;
        public override DamageSource damageSource => GetDamageSource();

        public virtual void SetValues()
        {
            previousPosition = characterBody.footPosition;
            duration = baseDuration;
            velocityLerpTime = baseVelocityLerpTime / attackSpeedStat;
            walkSpeedMultiplier = baseWalkSpeedMultiplier;
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            //traveledDistance += (characterBody.footPosition - previousPosition).sqrMagnitude;
            direction = GetAimRay().direction;
            if (!characterBody.isFlying) direction.y = 0;
            direction.Normalize();
            speed = characterBody.moveSpeed * walkSpeedMultiplier + previousVelocityMagnitude;
            Vector3 velocity = (previousVelocity == Vector3.zero ? direction : Vector3.Lerp(previousVelocity.normalized, direction, fixedAge / (velocityLerpTime / previousVelocityMagnitude))) * speed;
            if (characterMotor)
            {
                if (isAuthority) onChargeEndGiveSqrTraveledDistance?.Invoke(characterMotor.velocity.sqrMagnitude);
                if (!characterBody.isFlying) velocity.y = characterMotor.velocity.y;
                characterMotor.SetVelocityOverride(velocity);
            }
            else if (rigidbody)
            {
                if (!characterBody.isFlying) velocity.y = rigidbody.velocity.y;
                rigidbody.velocity = velocity;
            }
            if (demolisherComponent) demolisherComponent.overrideMeleeUtilityMeter = 1f - (fixedAge / duration);
            //if (NetworkServer.active)
            ShieldBash();
            previousPosition = characterBody.footPosition;
            if (!isAuthority) return;
            if (inputBank)
            {
                if (inputBank.skill1.justPressed || fixedAge >= duration) outer.SetNextStateToMain();
                if (skillLocator)
                {
                    GenericSkill secondarySkill = skillLocator.secondary;
                    if (secondarySkill && inputBank.skill2.justPressed && secondarySkill.ExecuteIfReady()) inputBank.skill2.hasPressBeenClaimed = true;
                }
                
            }
        }
        public virtual void ShieldBash()
        {
            Vector3 force = direction * speed + (Physics.gravity * -1f * shieldBashGravityForceMultiplier);
            bulletAttack.SetBonusForce(force);
            UpdateBulletAttack(characterBody.damage * shieldBashDamageCoefficient, shieldBashProcCoefficient, 0f, 0f, RollCrit(), shieldBashRadiusMultiplier, shieldBashDistance, false);
            ContinueFireMeleeAttack(new Ray(characterBody.corePosition, aimDirectionGrounded));
        }
        public override void OnEnter()
        {
            base.OnEnter();
            SetValues();
            CreateBulletAttack(true);
            bulletAttack.SetForceMassIsOne(true);
            bulletAttack.SetForceAlwaysApply(true);
            bulletAttack.SetForceDisableAirControlUntilCollision(true);
            bulletAttack.physForceFlags = PhysForceFlags.massIsOne | PhysForceFlags.ignoreGroundStick | PhysForceFlags.disableAirControlUntilCollision;
            UpdateBulletAttack(characterBody.damage * shieldBashDamageCoefficient, shieldBashProcCoefficient, 0f, 0f, RollCrit(), shieldBashRadiusMultiplier, shieldBashDistance, true);
            ConstantUpdateBulletAttack(new Ray(characterBody.corePosition, aimDirectionGrounded));
            if (characterMotor)
            {
                previousVelocity = characterMotor.velocity;
                if (!characterMotor.isFlying) previousVelocity.y = 0f;
                //characterMotor.walkSpeedPenaltyCoefficient *= walkSpeedMultiplier;
                if (characterMotor.Motor) characterMotor.Motor.GroundDetectionExtraDistance += extraGroundingDistance;
                characterMotor.stepOffset += extraStepOffset;
                Vector3 velocity = direction * characterBody.moveSpeed * walkSpeedMultiplier;
                velocity.y = Mathf.Max(characterMotor.velocity.y, 0f);
                characterMotor.velocity = velocity;

            }
            else if (rigidbody)
            {
                previousVelocity = rigidbody.velocity;
            }
            previousVelocityMagnitude = previousVelocity.magnitude;
            if (demolisherModel)
            {
                demolisherModel.devilCount++;
                demolisherModel.trailCount++;
            }
            PlayAnimation("Gesture, Override", "BufferEmpty");
            EffectData effectData = new EffectData
            {
                rootObject = characterBody.coreTransform.gameObject ?? characterBody.gameObject,
            };
            effectData.SetScale(effectScale);
            EffectManager.SpawnEffect(Assets.ShieldChargeEffect.index, effectData, false);
            effectInstance = effectData.GetEffectInstance();
            Util.PlaySound("Play_HorseMan_AngryYell", gameObject);
            //if (VisualsConfig.DemolisherVoicelines.Value && demolisherVoicelinesComponent) demolisherVoicelinesComponent.PlayVoiceline(VoicelineDef.VoicelineType.Landing);
            Util.PlaySound("Play_HorseMan_ChargeUp", gameObject);
            modelAnimator = GetModelAnimator();
            if (modelAnimator)
            {
                aimAnimator = modelAnimator.GetComponent<AimAnimator>();
                modelAnimator.SetBool("isCharging", true);
                //characterAnimParamAvailability = CharacterAnimParamAvailability.FromAnimator(modelAnimator);
                //int layerIndex = this.modelAnimator.GetLayerIndex("Body");
                //modelAnimator.CrossFadeInFixedTime("Sprint", 0.1f, layerIndex);
                modelAnimator.SetFloat(AnimationParameters.forwardSpeed, 1f);
                modelAnimator.SetFloat(AnimationParameters.rightSpeed, 0f);
                modelAnimator.SetFloat(AnimationParameters.upSpeed, 0f);
                modelAnimator.SetBool(AnimationParameters.isMoving, true);
                modelAnimator.SetBool(AnimationParameters.isGrounded, true);
                modelAnimator.SetBool(AnimationParameters.isSprinting, true);
                modelAnimator.SetFloat(AnimationParameters.turnAngle, 0f);
            }
            if (!isAuthority) return;
            characterBody.AddClientBuff(Assets.InstantMeleeSwing);
        }
        public override void Update()
        {
            base.Update();
            directionVisual = GetAimRay().direction;
            directionVisual.y = 0f;
            directionVisual.Normalize();
            if (effectInstance) effectInstance.transform.forward = directionVisual;
            if (modelAnimator)
            {
                modelAnimator.SetFloat(AnimationParameters.walkSpeed, characterBody.moveSpeed);
            }
            if (characterDirection)
            {
                characterDirection.forward = directionVisual;
            }

        }
        public override void OnExit()
        {
            base.OnExit();
            //if (playedAltVoiceline) Util.PlaySound("Stop_HorseMan_AngryYell", gameObject);
            Util.PlaySound("Stop_HorseMan_ChargeUp", gameObject);
            if (effectInstance)
            {
                BrynzaAPI.DestroyOnParticleEndAndNoParticles destroyOnParticleEndAndNoParticles = effectInstance.GetComponent<DestroyOnParticleEndAndNoParticles>();
                if (destroyOnParticleEndAndNoParticles && destroyOnParticleEndAndNoParticles.trackedParticleSystem)
                {
                    destroyOnParticleEndAndNoParticles.trackedParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
                else
                {
                    Destroy(effectInstance.gameObject);
                }
            }
            if (demolisherComponent) demolisherComponent.overrideMeleeUtilityMeter = -1f;
            if (demolisherModel)
            {
                demolisherModel.devilCount--;
                demolisherModel.trailCount--;
            }
            if (modelAnimator)
            {
                modelAnimator.SetBool("isCharging", false);
            }
            if (characterMotor)
            {
                characterMotor.stepOffset -= extraStepOffset;
                if (characterMotor.Motor) characterMotor.Motor.GroundDetectionExtraDistance -= extraGroundingDistance;
                //float walkSpeed = characterMotor.walkSpeed;
                //characterMotor.walkSpeedPenaltyCoefficient /= walkSpeedMultiplier;
                characterMotor.SetVelocityOverride(Vector3.zero);
                if (characterMotor.isGrounded && characterMotor.velocity.sqrMagnitude > characterMotor.walkSpeed * characterMotor.walkSpeed) characterMotor.velocity = characterMotor.velocity.normalized * characterMotor.walkSpeed;
            }
            if (inputBank) inputBank.skill1.PushState(true);
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
    public class Detonate : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            if (isAuthority) outer.SetNextStateToMain();
            Util.PlaySound("Play_stickybomblauncher_det", gameObject);
            ProjectileDetonator projectileDetonator = gameObject.GetOrAddComponent<ProjectileDetonator>();
            if (NetworkServer.active)
                if (projectileDetonator) projectileDetonator.DetonateAll();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
    public class WhirlwindMelee : BaseMeleeAttack
    {
        public static float damageCoefficient => WhirlwindMeleeConfig.damageCoefficient.Value;
        public static float procCoefficient => WhirlwindMeleeConfig.procCoefficient.Value;
        public static float effectCoefficient => MediumMeleeAttackConfig.effectCoefficient.Value;
        public static float maxDistance => WhirlwindMeleeConfig.maxDistance.Value;
        public static float force => WhirlwindMeleeConfig.force.Value;
        public static float radius => WhirlwindMeleeConfig.radius.Value;
        public static float maxDuration => WhirlwindMeleeConfig.maxDistance.Value;
        public static float spinDuration = 0.25f;
        public static float spinEnterCrossfade = 0.05f;
        public static float spinExitCrossfade = 0.05f;
        public static float baseDegreesPerSecond => WhirlwindMeleeConfig.baseDegreesPerSecond.Value;
        public static float baseRotationsPerSecond => WhirlwindMeleeConfig.baseRotationsPerSecond.Value;
        public static PhysForceFlags pullInForceFlags = PhysForceFlags.ignoreGroundStick | PhysForceFlags.massIsOne | PhysForceFlags.resetVelocity;
        public static float pullInForce = 24f;
        public static float pullInRadius = 2f;
        public static float controlReduction = 90f;
        public override DamageSource damageSource => GetDamageSource();
        public float duration;
        public float degreesPerSecond;
        public float rotationsPerSecond;
        public float interval;
        public float stopwatch;
        public Vector3 direction;
        public Vector3 rotation;
        public Animator animator;
        public float magnitude;
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            ContinueFireMeleeAttack(new Ray { direction = transform.up, origin = characterBody.footPosition });
            stopwatch += Time.fixedDeltaTime;
            if (stopwatch >= interval)
            {
                UpdateBulletAttack(characterBody.damage * damageCoefficient, procCoefficient, effectCoefficient, force, RollCrit(), radius, maxDistance, true);
                stopwatch = 0f;
                if (isAuthority && (activatorSkillSlot ? activatorSkillSlot.stock <= 0 : fixedAge >= duration) || !IsKeyDownAuthority())
                {
                    outer.SetNextStateToMain();
                    return;
                }
                SpawnEffect();
                if (activatorSkillSlot) activatorSkillSlot.stock--;
            }
            Ray ray = GetAimRay();
            Vector3 vector3 = inputBank ? inputBank.moveVector : ray.direction;
            vector3.y = ray.direction.y;
            direction = Vector3.MoveTowards(direction, vector3, (degreesPerSecond * Time.fixedDeltaTime * characterBody.attackSpeed / controlReduction) + characterBody.moveSpeed * Time.fixedDeltaTime);
            //direction = Vector3.RotateTowards(direction, vector3, degreesPerSecond / 57f * Time.fixedDeltaTime * characterBody.attackSpeed, 0f);
            rotation = Quaternion.AngleAxis(rotationsPerSecond * 360f * Time.fixedDeltaTime, Vector3.up) * rotation;
            if (characterDirection)
            {
                characterDirection.forward = rotation;
            }
            if (!isAuthority) return;
            BlastAttack blastAttack = new BlastAttack
            {
                attacker = null,
                attackerFiltering = AttackerFiltering.Default,
                baseDamage = 0f,
                baseForce = -pullInForce * Time.fixedDeltaTime,
                canRejectForce = false,
                damageColorIndex = DamageColorIndex.Default,
                falloffModel = BlastAttack.FalloffModel.None,
                inflictor = null,
                position = characterBody.corePosition,
                damageType = DamageTypeCombo.Generic,
                procCoefficient = 0f,
                radius = radius * pullInRadius,
                teamIndex = GetTeam(),
                physForceFlags = pullInForceFlags,
            };
            //blastAttack.SetForceMassIsOne(true);
            //blastAttack.SetForceAlwaysApply(true);
            blastAttack.Fire();
            if (characterMotor)
            {
                characterMotor.velocity = direction * Mathf.Max(characterMotor.walkSpeed, magnitude);
            }
            else if (rigidbody)
            {
                rigidbody.velocity = direction * Mathf.Max(characterMotor.walkSpeed, magnitude);
            }
        }
        public virtual void SetValues()
        {
            degreesPerSecond = baseDegreesPerSecond;
            rotationsPerSecond = baseRotationsPerSecond * characterBody.attackSpeed;
            if (currentMeleeWeaponDef) rotationsPerSecond *= currentMeleeWeaponDef.attackSpeedMultiplier;
            interval = 1f / rotationsPerSecond;
            duration = maxDuration;
        }
        public override void OnEnter()
        {
            base.OnEnter();
            direction = inputBank ? (inputBank.moveVector == Vector3.zero ? aimDirectionGrounded : inputBank.moveVector) : aimDirectionGrounded;
            rotation = direction;
            animator = GetModelAnimator();
            if (animator) animator.SetBool("isSpinning", true);
            SetValues();
            if (demolisherModel) demolisherModel.devilCount++;
            SpawnEffect();
            //Util.PlaySound("Play_DemoSwordSwing", gameObject);
            CreateBulletAttack();
            UpdateBulletAttack(damageCoefficient * characterBody.damage, procCoefficient, effectCoefficient, force, RollCrit(), radius, maxDistance, true);
            if (NetworkServer.active) characterBody.AddBuff(RoR2Content.Buffs.ArmorBoost);
            if (characterMotor)
            {
                magnitude = characterMotor.velocity.magnitude;
            }
            else if (rigidbody)
            {
                magnitude = rigidbody.velocity.magnitude;
            }
        }
        public void SpawnEffect()
        {
            EffectData effectData = new EffectData
            {
                scale = radius,
                rootObject = gameObject,
                genericFloat = interval
            };
            EffectManager.SpawnEffect(Assets.WhirlwindEffect.index, effectData, false);
            Util.PlaySound("Play_HorseMan_SpearWoosh", gameObject);
        }
        public override void OnExit()
        {
            base.OnExit();
            if (demolisherModel) demolisherModel.devilCount--;
            if (animator) animator.SetBool("isSpinning", false);
            if (NetworkServer.active) characterBody.RemoveBuff(RoR2Content.Buffs.ArmorBoost);
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
    public class Parry : DemolisherBaseState
    {
        public static Dictionary<GameObject, List<Parry>> activeParries = new Dictionary<GameObject, List<Parry>>();
        public static float baseParryWindow => ParryConfig.baseParryWindow.Value;
        public static float invincibilityTime => ParryConfig.invincibilityTime.Value;
        public static float damageCoefficient => ParryConfig.damageCoefficient.Value;
        public static float procCoefficient => ParryConfig.procCoefficient.Value;
        public static float force => ParryConfig.force.Value;
        public static float radius => ParryConfig.radius.Value;
        public static float effectScale = 2f;
        public static float baseMovementStart => ParryConfig.baseMovementStart.Value;
        public static float baseMovementEnd => ParryConfig.baseMovementEnd.Value;
        public Ray ray;
        public float parryWindow;
        public virtual void SetValues()
        {
            parryWindow = baseParryWindow;
            ray = GetAimRay();
            Vector3 vector3 = ray.direction;
            vector3.y = 0f;
            ray.direction = vector3;
        }
        public override void OnEnter()
        {
            base.OnEnter();
            if (demolisherModel) demolisherModel.devilCount++;
            SetValues();
            PlayAnimation("Gesture, Override", "Parry", "Slash.playbackRate", parryWindow);
            if (activeParries.ContainsKey(gameObject))
            {
                activeParries[gameObject].Add(this);
            }
            else
            {
                activeParries.Add(gameObject, [this]);
            }
            if (isAuthority)
            {
                if (characterMotor)
                {
                    characterMotor.velocity = ray.direction * baseMovementStart;
                }
                else if (rigidbody)
                {
                    rigidbody.velocity = ray.direction * baseMovementStart;
                }
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= parryWindow) outer.SetNextStateToMain();
        }
        public override void OnExit()
        {
            base.OnExit();
            if (demolisherModel) demolisherModel.devilCount--;
            if (activeParries.ContainsKey(gameObject))
            {
                activeParries[gameObject].Remove(this);
                if (activeParries[gameObject].Count <= 0) activeParries.Remove(gameObject);
            }
        }
        public virtual void OnParry(DamageInfo damageInfo)
        {
            BlastAttack blastAttack = new BlastAttack
            {
                attacker = gameObject,
                attackerFiltering = AttackerFiltering.Default,
                baseDamage = damageInfo.damage + characterBody.damage * damageCoefficient,
                baseForce = force,
                canRejectForce = true,
                crit = damageInfo.crit ? damageInfo.crit : RollCrit(),
                damageColorIndex = DamageColorIndex.Default,
                falloffModel = BlastAttack.FalloffModel.None,
                inflictor = damageInfo.inflictor,
                position = damageInfo.position,
                damageType = damageInfo.damageType,
                procCoefficient = procCoefficient,
                radius = radius,
                teamIndex = GetTeam(),
            };
            blastAttack.Fire();
            EffectData effectData = new EffectData
            {
                origin = ray.origin + ray.direction * characterBody.radius,
                rotation = Quaternion.LookRotation(ray.direction),
                scale = effectScale
            };
            EffectManager.SpawnEffect(Assets.ParryEffect.prefab, effectData, true);
            characterBody.AddTimedBuff(DLC2Content.Buffs.HiddenRejectAllDamage, invincibilityTime);
            Util.CleanseBody(characterBody, true, false, true, true, true, true);
            if (characterMotor)
            {
                Vector3 vector3 = characterMotor.velocity;
                vector3.x = 0f;
                vector3.z = 0f;
                PhysForceInfo physForceInfo = new PhysForceInfo
                {
                    force = ray.direction * -1f * baseMovementEnd + vector3 * -1f,
                    massIsOne = true,
                    ignoreGroundStick = true,
                    disableAirControlUntilCollision = false,
                };
                characterMotor.ApplyForceImpulse(physForceInfo);
            }
            activatorSkillSlot.AddOneStock();
            outer.SetNextStateToMain();
        }
    }
    public class ChargeCollapse : BaseSkillState
    {
        public static float maxCharge = 2f;
        public static float startAnimationDuration = 0.5f;
        public static float chargedAnimationDuration = 0.5f;
        public static float noneAnimationDuration = 0.5f;
        public static float rechargeReturnPercentage = 0.75f;
        public ChildLocator childLocator;
        public Transform power;
        public float charge;
        public bool reachedFullCharge;
        public bool fired;
        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound("Play_loose_cannon_charge", gameObject);
            PlayAnimation("Gesture, Override", "CollapseStart", "Slash.playbackRate", startAnimationDuration / attackSpeedStat);
            childLocator = modelLocator && modelLocator.modelTransform ? modelLocator.modelTransform.GetComponent<ChildLocator>() : null;
            if (childLocator)
            {
                power = childLocator.FindChild("PowerL");
            }
        }
        public override void OnExit()
        {
            base.OnExit();
            if (!reachedFullCharge)
            {
                Util.PlaySound("Stop_loose_cannon_charge", gameObject);
            }
            if (!fired)
            {
                PlayAnimation("Gesture, Override", "BufferEmpty", "Slash.playbackRate", noneAnimationDuration, noneAnimationDuration);
                if (power) power.gameObject.SetActive(false);
                if (activatorSkillSlot)
                {
                    activatorSkillSlot.rechargeStopwatch += charge / maxCharge * activatorSkillSlot.finalRechargeInterval * rechargeReturnPercentage;
                }
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            StartAimMode();
            if (charge < maxCharge)
                charge += Time.fixedDeltaTime * characterBody.attackSpeed;
            if (charge >= maxCharge && !reachedFullCharge)
            {
                Util.PlaySound("Stop_loose_cannon_charge", gameObject);
                Util.PlaySound("Play_Recharged", gameObject);
                PlayAnimation("Gesture, Override", "CollapseCharged", "Slash.playbackRate", chargedAnimationDuration / characterBody.attackSpeed);
                if (power) power.gameObject.SetActive(true);
                reachedFullCharge = true;
            }
            if (!isAuthority || IsKeyDownAuthority())
            {
                return;
            }
            if (charge >= maxCharge)
            {
                charge = maxCharge;
                FireCollapse fireCollapse = EntityStateCatalog.InstantiateState(typeof(FireCollapse)) as FireCollapse;
                fired = true;
                outer.SetNextState(fireCollapse);
            }
            else
            {

                outer.SetNextStateToMain();
            }
        }
    }
    public class FireCollapse : DemolisherBaseState
    {
        public static float bulletDamageCoefficient => CollapseConfig.bulletDamageCoefficient.Value;
        public static float bulletProcCoefficient => CollapseConfig.bulletProcCoefficient.Value;
        public static float explosionDamageCoefficient => CollapseConfig.explosionDamageCoefficient.Value;
        public static float explosionProcCoefficient => CollapseConfig.explosionProcCoefficient.Value;
        public static float bulletForce => CollapseConfig.bulletForce.Value;
        public static float explosionForce => CollapseConfig.explosionForce.Value;
        public static float bulletRadius => CollapseConfig.bulletRadius.Value;
        public static float explosionRadius => CollapseConfig.explosionRadius.Value;
        public static float selfForce => CollapseConfig.selfForce.Value;
        public static float selfForceGrounded => CollapseConfig.selfForceGrounded.Value;
        public static float spread = 1.5f;
        public static float fireAnimationDuration = 0.5f;
        public static float crossfade = 0.05f;
        public override void OnEnter()
        {
            base.OnEnter();
            Fire();
        }
        public void Fire()
        {
            if (demolisherModel) demolisherModel.AddTimedDevilCount(fireAnimationDuration / attackSpeedStat);
            characterBody.AddSpreadBloom(spread);
            StartAimMode(2f, true);
            PlayCrossfade("Gesture, Override", "FireChest", "Slash.playbackRate", fireAnimationDuration / attackSpeedStat, crossfade);
            Util.PlaySound("Play_tacky_grenadier_shoot_crit", gameObject);
            //ChildLocator childLocator = modelLocator && modelLocator.modelTransform ? modelLocator.modelTransform.GetComponent<ChildLocator>() : null;
            //if (childLocator)
            //{
            //    Transform power = childLocator.FindChild("PowerL");
            //    if (power) power.gameObject.SetActive(false);
            //}
            if (isAuthority)
            {
                Ray ray = GetAimRay();
                BulletAttack bulletAttack = new BulletAttack
                {
                    aimVector = ray.direction,
                    allowTrajectoryAimAssist = false,
                    bulletCount = 1,
                    damage = characterBody.damage * bulletDamageCoefficient,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, GetDamageSource()),
                    falloffModel = BulletAttack.FalloffModel.None,
                    force = bulletForce,
                    isCrit = RollCrit(),
                    maxSpread = 0f,
                    minSpread = 0f,
                    origin = ray.origin,
                    owner = gameObject,
                    radius = bulletRadius,
                    maxDistance = 99999f,
                    procCoefficient = bulletProcCoefficient,
                    sniper = false,
                    smartCollision = true,
                    weapon = gameObject,
                    trajectoryAimAssistMultiplier = 0f,
                    spreadYawScale = 0f,
                    spreadPitchScale = 0f,
                    hitCallback = FireExplosion,
                    tracerEffectPrefab = Assets.DemolisherTracer.prefab,
                    muzzleName = "UpperChest"
                };
                bulletAttack.Fire();
                if (characterMotor)
                {
                    PhysForceInfo physForceInfo = new PhysForceInfo
                    {
                        massIsOne = true,
                        ignoreGroundStick = true,
                        disableAirControlUntilCollision = false,
                        force = ray.direction * (characterMotor.isGrounded ? selfForceGrounded : selfForce) * -1f
                    };
                    characterMotor.ApplyForceImpulse(physForceInfo);
                }
                else if (rigidbody)
                {
                    rigidbody.AddForce(ray.direction * selfForceGrounded * -1f, ForceMode.VelocityChange);
                }
                outer.SetNextStateToMain();
            }
        }

        public bool FireExplosion(BulletAttack bulletAttack, ref BulletAttack.BulletHit hitInfo)
        {
            BlastAttack blastAttack = new BlastAttack
            {
                attacker = bulletAttack.owner,
                attackerFiltering = AttackerFiltering.Default,
                baseDamage = characterBody.damage * explosionDamageCoefficient,
                baseForce = explosionForce,
                canRejectForce = true,
                crit = bulletAttack.isCrit,
                damageColorIndex = bulletAttack.damageColorIndex,
                falloffModel = BlastAttack.FalloffModel.None,
                inflictor = bulletAttack.owner,
                position = hitInfo.point,
                damageType = bulletAttack.damageType,
                procCoefficient = explosionProcCoefficient,
                radius = explosionRadius,
                teamIndex = GetTeam(),
            };
            blastAttack.Fire();
            EffectData effectData = new EffectData
            {
                origin = blastAttack.position,
                scale = blastAttack.radius,
            };
            EffectManager.SpawnEffect(Assets.CollapseExplosion.prefab, effectData, true);
            return BulletAttack.DefaultHitCallbackImplementation(bulletAttack, ref hitInfo);
        }
    }
    public class PrepareGroundSword : GenericCharacterMain, ISkillState
    {
        public GenericSkill activatorSkillSlot { get; set; }
        public bool dontPlayAnimation;
        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("Gesture, Override", "PrepareGroundSlash", "Slash.playbackRate", 0.5f / attackSpeedStat);
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            StartAimMode();
            if (!isAuthority) return;
            if (inputBank && inputBank.skill1.justPressed)
            {
                EntityStateMachine[] entityStateMachines = gameObject.GetComponents<EntityStateMachine>();
                foreach (EntityStateMachine entityStateMachine in entityStateMachines)
                {
                    if (entityStateMachine == null) continue;
                    if (entityStateMachine.customName == "Weapon")
                    {
                        entityStateMachine.SetState(new FireTallSword() { activatorSkillSlot = activatorSkillSlot });
                        break;
                    }
                }
                dontPlayAnimation = true;
                outer.SetStateToMain();
            }
            if (!IsKeyDown())
            {
                dontPlayAnimation = true;
                outer.SetState(new GroundSlash() { activatorSkillSlot = activatorSkillSlot });
            }
        }
        public bool IsKeyDown()
        {
            if (skillLocator == null || inputBank == null) return true;
            if (skillLocator.primary == activatorSkillSlot) return inputBank.skill1.down;
            if (skillLocator.secondary == activatorSkillSlot) return inputBank.skill2.down;
            if (skillLocator.utility == activatorSkillSlot) return inputBank.skill3.down;
            if (skillLocator.special == activatorSkillSlot) return inputBank.skill4.down;
            return true;
        }
        public override void OnExit()
        {
            base.OnExit();
            if (!dontPlayAnimation) PlayAnimation("Gesture, Override", "BufferEmpty", "Slash.playbackRate", 1f, 1f);
        }
    }
    public class GroundSlash : DemolisherBaseState
    {
        public static float radius = 5f;
        public static float force = 300f;
        public static float damageCoefficient = 4f;
        public static float procCoefficient = 1f;
        public static float maxDistance = 24f;
        public GameObject cracksTrail;
        public BulletAttack bulletAttack;
        public override void OnEnter()
        {
            base.OnEnter();
            //cracksTrail = GameObject.Instantiate(Assets.CracksTrailEffect, characterBody.coreTransform);
            PlayAnimation("Gesture, Override", "GroundSlashDash", "Slash.playbackRate", 0.5f / attackSpeedStat);
            if (!isAuthority) return;
            Ray ray = GetAimRay();
            Vector3 direction = ray.direction;
            direction.y = 0f;
            direction.Normalize();
            Vector3 endPosition = ray.origin + direction * maxDistance;
            //if (Physics.Raycast(ray.origin, direction, out RaycastHit hitInfo, maxDistance, LayerIndex.world.mask))
            //{
            //    endPosition = hitInfo.point;
            //}
            //else
            //{
            //    endPosition = ray.origin + direction * maxDistance;
            //}
            NodeGraph groundNodes = SceneInfo.instance.groundNodes;
            NodeGraph.NodeIndex nodeIndex = groundNodes.FindClosestNode(endPosition, base.characterBody.hullClassification, float.PositiveInfinity);
            if (groundNodes.GetNodePosition(nodeIndex, out endPosition))
            {
                endPosition += characterBody.corePosition - characterBody.footPosition;
                Vector3 vector3 = endPosition - characterBody.corePosition;
                float distance = vector3.magnitude;
                vector3.Normalize();
                bulletAttack = new BulletAttack
                {
                    aimVector = vector3,
                    allowTrajectoryAimAssist = false,
                    bulletCount = 1,
                    damage = damageCoefficient * damageStat,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, GetDamageSource()),
                    falloffModel = BulletAttack.FalloffModel.None,
                    force = force,
                    isCrit = RollCrit(),
                    maxSpread = 0f,
                    minSpread = 0f,
                    origin = characterBody.corePosition,
                    owner = gameObject,
                    radius = radius,
                    maxDistance = distance,
                    procCoefficient = procCoefficient,
                    sniper = false,
                    smartCollision = true,
                    weapon = gameObject,
                    trajectoryAimAssistMultiplier = 0f,
                    spreadYawScale = 0f,
                    spreadPitchScale = 0f,
                    stopperMask = LayerIndex.noCollision.mask,
                    hitMask = LayerIndex.entityPrecise.mask,
                    tracerEffectPrefab = Assets.DemolisherTracer.prefab
                };
                object attack = bulletAttack;
                if (currentMeleeWeaponDef)
                {
                    currentMeleeWeaponDef.OneTimeModification(this, ref attack);
                    currentMeleeWeaponDef.ModifyAttack(this, ref attack);
                }
                bulletAttack.maxDistance = distance;
                bulletAttack.Fire();
                characterDirection.forward = direction;
                characterDirection.targetVector = direction;
                characterMotor.Motor.SetPositionAndRotation(endPosition, Quaternion.LookRotation(direction));
                outer.SetNextStateToMain();
            }
            else
            {
                outer.SetStateToMain();
            }
        }
        public override void OnExit()
        {
            base.OnExit();
            //if (cracksTrail) Destroy(cracksTrail);
        }
    }
    public class FireTallSword : DemolisherBaseState
    {
        public static float baseDuration => FireTallSwordConfig.baseDuration.Value;
        public static float damageCoefficient => FireTallSwordConfig.damageCoefficient.Value;
        public static float force => FireTallSwordConfig.force.Value;
        public static float recoil = 1f;
        public static float spread = 1.5f;
        public float duration;
        public bool stateTaken;
        public override void OnEnter()
        {
            base.OnEnter();
            SetValues();
            Fire(GetAimRay(), characterBody.damage * damageCoefficient, force, RollCrit());
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && isAuthority) outer.SetNextStateToMain();
        }
        public virtual void SetValues()
        {
            duration = baseDuration / characterBody.attackSpeed;
        }
        public void Fire(Ray ray, float damage, float force, bool crit)
        {
            StartAimMode(2f, true);
            Util.PlaySound("Play_GiantBlade_SwordSwing", gameObject);
            Util.PlaySound("Play_HorseMan_SpearSlide", gameObject);
            if (demolisherModel) demolisherModel.AddTimedDevilCount(0.25f / attackSpeedStat);
            PlayAnimation("Gesture, Override", "GroundSlashFire", "Slash.playbackRate", 0.5f / attackSpeedStat);
            AddRecoil(-recoil, -recoil, 0f, 0f);
            characterBody.AddSpreadBloom(spread);
            if (base.isAuthority)
            {
                //TrajectoryAimAssist.ApplyTrajectoryAimAssist(ref ray, projectile, gameObject, 1f);
                FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                {
                    projectilePrefab = Assets.SwordPillarProjectile,
                    position = characterBody.footPosition,
                    rotation = Util.QuaternionSafeLookRotation(ray.direction),
                    owner = gameObject,
                    damage = damage,
                    force = force,
                    crit = crit,
                    damageTypeOverride = new DamageTypeCombo?(new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, GetDamageSource()))
                };
                ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                //outer.SetNextStateToMain();
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
    public class Slice : DemolisherBaseState
    {
        public static int count;
        public static GameObject ppEffect;
        public static float damageCoefficient => SlicingConfig.damageCoefficient.Value;
        public static float procCoefficient => SlicingConfig.procCoefficient.Value;
        public static float effectCoefficient => SlicingConfig.effectCoefficient.Value;
        public static float force => SlicingConfig.force.Value;
        public static float radius => SlicingConfig.radius.Value;
        public static float baseTimeDivisionMultiplier => SlicingConfig.baseTimeDivisionMultiplier.Value;
        public static float baseDistance => SlicingConfig.baseDistance.Value;
        public static float baseDuration => SlicingConfig.baseDuration.Value;
        public int stockMultiplier;
        public float timeDivisionMultiplier;
        public float duration;
        public BulletAttack bulletAttack;
        public Animator animator;
        public bool alreadyUpdate;
        public bool alreadyStateUpdate;
        public AnimatorUpdateMode animatorUpdateMode;
        public CameraTargetParams.CameraParamsOverrideHandle cameraParamsOverrideHandle;
        public static int cameraOverridePriority = 6;
        public static float setCameraSmoothTime = 0.01f;
        public static float unsetCameraSmoothTime = 0.01f;
        private bool fired;

        public virtual void FireSlice()
        {
            Util.PlaySound("Play_HorseMan_SpearWoosh", gameObject);
            animator = GetModelAnimator();
            if (animator)
            {
                bool step = animator.GetBool("isSlicingStep");
                animator.SetBool("isSlicingStep", !step);
                PlayAnimation("FullBody, Override", step ? "SlashLeft" : "SlashRight");
            }
            Ray ray = GetAimRay();
            if (isAuthority)
            {
                Vector3 destination;
                float distance;
                if (Physics.Raycast(ray, out RaycastHit hitInfo, baseDistance, LayerIndex.world.mask, QueryTriggerInteraction.UseGlobal))
                {
                    destination = hitInfo.point;
                    distance = hitInfo.distance;
                }
                else
                {
                    destination = ray.origin + ray.direction * baseDistance;
                    distance = baseDistance;
                }
                EffectData effectData = new EffectData
                {
                    origin = destination,
                    start = ray.origin
                };
                EffectManager.SpawnEffect(Assets.DemolisherTracer.index, effectData, true);
                if (bulletAttack == null) CreateBulletAttack();
                UpdateBulletAttack(ray, distance, characterBody.damage, RollCrit());
                bulletAttack.Fire();
                TeleportHelper.TeleportBody(characterBody, destination, false);
                if (characterMotor)
                {
                    characterMotor.velocity = Vector3.zero;
                }
                else if (rigidbody)
                {
                    rigidbody.velocity = Vector3.zero;
                }
            }
            if (characterDirection)
            {
                characterDirection.forward = ray.direction;
                characterDirection.moveVector = ray.direction;
            }
            if (activatorSkillSlot)
            {
                activatorSkillSlot.stock--;
                if (isAuthority)
                    if (activatorSkillSlot.stock > 0)
                    {
                        fired = true;
                        outer.SetNextState(new Slicing { activatorSkillSlot = activatorSkillSlot, stockMultiplier = stockMultiplier, animatorUpdateMode = animatorUpdateMode, alreadyUpdate = alreadyUpdate, timeDivisionMultiplier = timeDivisionMultiplier, cameraParamsOverrideHandle = cameraParamsOverrideHandle, dontEnter = true, alreadyStateUpdate = alreadyStateUpdate });
                    }
                    else
                    {
                        outer.SetNextStateToMain();
                    }
            }
            else if (isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }
        public virtual void CreateBulletAttack()
        {
            bulletAttack = new DemolisherBulletAttack
            {
                aimVector = Vector3.zero,
                allowTrajectoryAimAssist = false,
                bulletCount = 1,
                damage = 0f,
                damageColorIndex = DamageColorIndex.Default,
                damageType = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, GetDamageSource()),
                falloffModel = BulletAttack.FalloffModel.None,
                force = force,
                isCrit = false,
                maxSpread = 0f,
                minSpread = 0f,
                origin = Vector3.zero,
                owner = gameObject,
                radius = radius,
                maxDistance = baseDistance,
                procCoefficient = 0f,
                sniper = false,
                smartCollision = true,
                weapon = gameObject,
                trajectoryAimAssistMultiplier = 0f,
                spreadYawScale = 0f,
                spreadPitchScale = 0f,
                stopperMask = LayerIndex.world.mask,
                hitMask = LayerIndex.entityPrecise.mask,
                effectCoefficient = effectCoefficient
                //tracerEffectPrefab = Assets.DemolisherTracer.prefab
            };
            bulletAttack.SetNoWeaponIfOwner(true);
            object attack = bulletAttack;
            currentMeleeWeaponDef?.OneTimeModification(this, ref attack);
        }
        public virtual void UpdateBulletAttack(Ray ray, float distance, float damage, bool crit)
        {
            bulletAttack.aimVector = ray.direction;
            bulletAttack.origin = ray.origin;
            bulletAttack.damage = damage * damageCoefficient;
            bulletAttack.force = force;
            bulletAttack.isCrit = crit;
            bulletAttack.radius = radius;
            bulletAttack.maxDistance = distance;
            object attack = bulletAttack;
            currentMeleeWeaponDef?.ModifyAttack(this, ref attack);
        }
        public override void OnEnter()
        {
            base.OnEnter();
            DemolisherPlugin.Log.LogMessage("EnteredSlice");
            FireSlice();
        }
        public override void OnExit()
        {
            base.OnExit();
            if (isAuthority && !fired)
            {
                fired = true;
                outer.SetNextState(new ExitSlicing { activatorSkillSlot = activatorSkillSlot, animatorUpdateMode = animatorUpdateMode, timeDivisionMultiplier = timeDivisionMultiplier, stockMultiplier = stockMultiplier, alreadyUpdate = alreadyUpdate, cameraParamsOverrideHandle = cameraParamsOverrideHandle, exit = true, alreadyStateUpdate = alreadyStateUpdate });
            }
        }
        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(stockMultiplier);
            writer.Write((int)animatorUpdateMode);
            writer.Write(alreadyUpdate);
            writer.Write(alreadyStateUpdate);
        }
        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            stockMultiplier = reader.ReadInt32();
            animatorUpdateMode = (AnimatorUpdateMode)reader.ReadInt32();
            alreadyUpdate = reader.ReadBoolean();
            alreadyStateUpdate = reader.ReadBoolean();
        }
    }
    public class ExitSlicing : DemolisherBaseState
    {
        public static float unsetCameraSmoothTime = 0.01f;
        public bool exit;
        public CameraTargetParams.CameraParamsOverrideHandle cameraParamsOverrideHandle;
        public float timeDivisionMultiplier;
        public int stockMultiplier;
        public AnimatorUpdateMode animatorUpdateMode;
        private Animator animator;
        public bool alreadyUpdate;
        public bool alreadyStateUpdate;
        public override void OnEnter()
        {
            base.OnEnter();
            if (!exit)
            {
                if (isAuthority)
                {
                    outer.SetNextStateToMain();
                }
                return;
            }
            if (!alreadyStateUpdate) outer.ShouldStateTransitionOnUpdate = false;
            Slicing.count--;
            animator = GetModelAnimator();
            if (demolisherModel)
            {
                demolisherModel.devilCount--;
                demolisherModel.trailCount--;
            }
            if (animator)
            {
                animator.SetBool("isSlicing", false);
                animator.SetBool("isSlicingStep", false);
                if (!alreadyUpdate) animator.updateMode = animatorUpdateMode;
            }
            if (activatorSkillSlot) activatorSkillSlot.stock = activatorSkillSlot.stock / stockMultiplier;
            if (NetworkServer.active) characterBody.RemoveBuff(RoR2Content.Buffs.ArmorBoost);
            if (isAuthority)
            {
                BrynzaAPI.Utils.ChangeTimescaleForAllClients(Time.timeScale * timeDivisionMultiplier);
                if (cameraTargetParams)
                {
                    cameraTargetParams.RemoveParamsOverride(cameraParamsOverrideHandle, unsetCameraSmoothTime);
                }
                outer.SetNextStateToMain();
            }
        }
        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(stockMultiplier);
            writer.Write((int)animatorUpdateMode);
            writer.Write(alreadyUpdate);
            writer.Write(exit);
            writer.Write(alreadyStateUpdate);
        }
        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            stockMultiplier = reader.ReadInt32();
            animatorUpdateMode = (AnimatorUpdateMode)reader.ReadInt32();
            alreadyUpdate = reader.ReadBoolean();
            exit = reader.ReadBoolean();
            alreadyStateUpdate = reader.ReadBoolean();
        }
    }
    public class Slicing : DemolisherBaseState
    {
        public static int count;
        public static GameObject ppEffect;
        public static float baseTimeDivisionMultiplier => SlicingConfig.baseTimeDivisionMultiplier.Value;
        public static float baseDuration => SlicingConfig.baseDuration.Value;
        public static int baseStockMultiplier => SlicingConfig.stockMultiplier.Value;
        public bool dontEnter;
        public int stockMultiplier;
        public float timeDivisionMultiplier;
        public float duration;
        public CharacterMaster characterMaster;
        public PlayerCharacterMasterController playerCharacterMasterController;
        public BaseAI[] baseAIs;
        public Animator animator;
        public bool alreadyUpdate;
        public bool alreadyStateUpdate;
        public AnimatorUpdateMode animatorUpdateMode;
        public CameraTargetParams.CameraParamsOverrideHandle cameraParamsOverrideHandle;
        public static int cameraOverridePriority = 6;
        public static float setCameraSmoothTime = 0.01f;

        private bool fired;
        public virtual void SetValues()
        {
            timeDivisionMultiplier = baseTimeDivisionMultiplier * characterBody.attackSpeed;
            stockMultiplier = baseStockMultiplier;
            duration = baseDuration;
        }
        public override void OnEnter()
        {
            base.OnEnter();
            characterMaster = characterBody.master;
            if (characterMaster)
            {
                playerCharacterMasterController = characterMaster.playerCharacterMasterController;
                baseAIs = characterMaster.AiComponents;
            }
            if (dontEnter) return;
            alreadyStateUpdate = outer.ShouldStateTransitionOnUpdate;
            if (!alreadyStateUpdate) outer.ShouldStateTransitionOnUpdate = true;
            count++;
            SetValues();
            animator = GetModelAnimator();
            if (animator)
            {
                animator.SetBool("isSlicing", true);
                alreadyUpdate = animator.updateMode == AnimatorUpdateMode.UnscaledTime;
                if (!alreadyUpdate)
                {
                    animatorUpdateMode = animator.updateMode;
                    animator.updateMode = AnimatorUpdateMode.UnscaledTime;
                }
            }
            if (demolisherModel)
            {
                demolisherModel.devilCount++;
                demolisherModel.trailCount++;
            }
            if (activatorSkillSlot) activatorSkillSlot.stock = activatorSkillSlot.stock * stockMultiplier;
            if (NetworkServer.active) characterBody.AddBuff(RoR2Content.Buffs.ArmorBoost);
            if (isAuthority)
            {
                BrynzaAPI.Utils.ChangeTimescaleForAllClients(Time.timeScale / timeDivisionMultiplier);
                if (cameraTargetParams)
                {
                    HG.BlendableTypes.BlendableVector3 blendableVector3 = cameraTargetParams.currentCameraParamsData.idealLocalCameraPos;
                    blendableVector3.value.y = 0f;
                    CharacterCameraParamsData characterCameraParamsData = new CharacterCameraParamsData
                    {
                        fov = cameraTargetParams.currentCameraParamsData.fov,
                        isFirstPerson = cameraTargetParams.currentCameraParamsData.isFirstPerson,
                        idealLocalCameraPos = blendableVector3,
                        maxPitch = cameraTargetParams.currentCameraParamsData.maxPitch,
                        minPitch = cameraTargetParams.currentCameraParamsData.minPitch,
                        overrideFirstPersonFadeDuration = cameraTargetParams.currentCameraParamsData.overrideFirstPersonFadeDuration,
                        pivotVerticalOffset = 0f,
                        wallCushion = cameraTargetParams.currentCameraParamsData.wallCushion
                    };
                    CameraTargetParams.CameraParamsOverrideRequest cameraParamsOverrideRequest = new CameraTargetParams.CameraParamsOverrideRequest
                    {
                        cameraParamsData = characterCameraParamsData,
                        priority = cameraOverridePriority
                    };
                    cameraParamsOverrideHandle = cameraTargetParams.AddParamsOverride(cameraParamsOverrideRequest, setCameraSmoothTime);
                }
            }
        }
        public override void OnExit()
        {
            base.OnExit();
            if (isAuthority && !fired)
            {
                fired = true;
                outer.SetNextState(new ExitSlicing { activatorSkillSlot = activatorSkillSlot, stockMultiplier = stockMultiplier, animatorUpdateMode = animatorUpdateMode, alreadyUpdate = alreadyUpdate, cameraParamsOverrideHandle = cameraParamsOverrideHandle, timeDivisionMultiplier = timeDivisionMultiplier, exit = true, alreadyStateUpdate = alreadyStateUpdate });
            }
        }
        public override void Update()
        {
            base.Update();
            if (isAuthority)
            {
                if (playerCharacterMasterController) playerCharacterMasterController.PollButtonInput();
                if (baseAIs != null) foreach (BaseAI baseAI in baseAIs) if (baseAI) baseAI.UpdateBodyInputs();
                if (inputBank)
                {
                    if (inputBank.skill1.justPressed)
                    {
                        fired = true;
                        outer.SetNextState(new Slice { activatorSkillSlot = activatorSkillSlot, stockMultiplier = stockMultiplier, animatorUpdateMode = animatorUpdateMode, alreadyUpdate = alreadyUpdate, cameraParamsOverrideHandle = cameraParamsOverrideHandle, timeDivisionMultiplier = timeDivisionMultiplier, alreadyStateUpdate = alreadyStateUpdate });
                    }
                }
            }
        }
        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(stockMultiplier);
            writer.Write((int)animatorUpdateMode);
            writer.Write(alreadyUpdate);
            writer.Write(dontEnter);
            writer.Write(alreadyStateUpdate);
        }
        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            stockMultiplier = reader.ReadInt32();
            animatorUpdateMode = (AnimatorUpdateMode)reader.ReadInt32();
            alreadyUpdate = reader.ReadBoolean();
            dontEnter = reader.ReadBoolean();
            alreadyStateUpdate = reader.ReadBoolean();
        }
    }
    public class Slam : BaseState
    {
        public static float gravityMultiplier = 3f;
        public static float verticalJumpBoost = 5f;
        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active)
            {
                characterBody.AddBuff(JunkContent.Buffs.IgnoreFallDamage);
            }
            if (characterMotor)
            {
                characterMotor.gravityScale *= gravityMultiplier;
                if (isAuthority)
                {
                    characterMotor.onHitGroundAuthority += CharacterMotor_onHitGroundAuthority;
                    characterMotor.velocity.y = verticalJumpBoost * Physics.gravity.y * -1f;
                }
            }
            else
            {
                outer.SetNextStateToMain();
            }
        }

        private void CharacterMotor_onHitGroundAuthority(ref CharacterMotor.HitGroundInfo hitGroundInfo)
        {
            outer.SetState(new SlamFire { fixedAge = this.fixedAge, velocity = MathF.Abs(characterMotor.velocity.y) });
        }

        public override void OnExit()
        {
            base.OnExit();
            if (NetworkServer.active)
            {
                characterBody.RemoveBuff(JunkContent.Buffs.IgnoreFallDamage);
                characterBody.AddTimedBuff(JunkContent.Buffs.IgnoreFallDamage, 1f);
            }
            if (characterMotor)
            {
                characterMotor.gravityScale /= gravityMultiplier;
                if (isAuthority)
                {
                    characterMotor.onHitGroundAuthority -= CharacterMotor_onHitGroundAuthority;
                }
            }
        }
    }
    public class SlamFire : BaseState
    {
        public static float baseRadius = 24f;
        public static float basePower = 5f;
        public float velocity;
        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active)
            {
                Collider[] colliders = Physics.OverlapSphere(characterBody.footPosition, baseRadius, LayerIndex.CommonMasks.characterBodies + LayerIndex.CommonMasks.fakeActorLayers, QueryTriggerInteraction.UseGlobal);
                foreach (Collider collider in colliders)
                {
                    CharacterBody characterBody = collider.GetComponent<CharacterBody>();
                    if (characterBody == null) continue;
                    Vector3 force = velocity * basePower * Physics.gravity * -1f;
                    if (characterBody.characterMotor)
                    {
                        //characterBody.characterMotor.velocity.y = velocity * basePower * Physics.gravity.y * -1f;
                        PhysForceInfo physForceInfo = new PhysForceInfo
                        {
                            disableAirControlUntilCollision = true,
                            force = force + characterBody.characterMotor.velocity * -1f,
                            massIsOne = true
                        };
                        characterBody.characterMotor.ApplyForceImpulse(physForceInfo);
                    }
                    else if (characterBody.rigidbody)
                    {
                        characterBody.rigidbody.velocity = force;
                    }
                }
            }
            if (isAuthority)
                outer.SetStateToMain();
        }
    }
    public class ChainDash : DemolisherBaseState
    {
        public static float baseStartWindow => ChainDashConfig.baseStartWindow.Value;
        public static float baseEndWindow => ChainDashConfig.baseEndWindow.Value;
        public static float speedMultiplier => ChainDashConfig.speedMultiplier.Value;
        public static float moveVectorSmoothTime => ChainDashConfig.moveVectorSmoothTime.Value;
        public static float movementSmoothLerpCoof = 1.5f;
        public static float extraGroundingDistance = 8f;
        public static float extraStepOffset = 8f;
        public static float baseEffectDuration = 0.1f;
        public static float effectScale = 1f;
        public float effectDuration;
        public float startWindow;
        public float endWindow;
        public bool effectApplied;
        public float stopwatch;
        public bool wasKeyDown;
        public Vector3 moveVector;
        public Vector3 moveVectorVelocity;
        public Animator modelAnimator;
        public BodyAnimatorSmoothingParameters.SmoothingParameters smoothingParameters;
        public bool success;
        private bool wasGrounded;
        private Vector3 previousVelocity;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound("Play_Yi_Dash", gameObject);
            //GetBodyAnimatorSmoothingParameters(out smoothingParameters);
            if (characterMotor)
            {
                previousVelocity = characterMotor.velocity;
                if (!characterMotor.isFlying) previousVelocity.y = 0f;
                characterMotor.stepOffset += extraStepOffset;
                //characterMotor.velocity.y = Mathf.Max(0f, characterMotor.velocity.y);
                if (characterMotor.Motor) characterMotor.Motor.GroundDetectionExtraDistance += extraGroundingDistance;
            }
            else if (rigidbody)
            {
                previousVelocity = rigidbody.velocity;
            }
            if (demolisherModel) demolisherModel.trailCount++;
            SetValues();
            wasKeyDown = true;
            wasGrounded = characterMotor.isGrounded;
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.fixedDeltaTime;
            if (demolisherComponent) demolisherComponent.overrideMeleeUtilityMeter = 1f - stopwatch / startWindow;
            if (effectApplied)
            {
                effectDuration -= Time.fixedDeltaTime;
                if (effectDuration < 0)
                {
                    demolisherModel.devilCount--;
                    effectApplied = false;
                }
            }
            if (!isAuthority) return;
            if (skillLocator && inputBank)
            {
                HandleSkill(skillLocator.primary, ref inputBank.skill1);
                HandleSkill(skillLocator.secondary, ref inputBank.skill2);
            }
            moveVector = Vector3.SmoothDamp(moveVector, Vector3.zero, ref moveVectorVelocity, moveVectorSmoothTime, float.MaxValue, Time.fixedDeltaTime);
            if (characterMotor)
            {
                if (!characterMotor.isGrounded && wasGrounded) characterMotor.velocity.y = 0f;
                Vector3 vector3 = moveVector;
                if (!characterMotor.isFlying) vector3.y = characterMotor.velocity.y;
                characterMotor.velocity = Vector3.Lerp(previousVelocity, vector3, stopwatch / (startWindow / movementSmoothLerpCoof));
                wasGrounded = characterMotor.isGrounded;
            }
            else if (rigidbody)
            {
                Vector3 vector3 = moveVector;
                //vector3.y = rigidbody.velocity.y;
                rigidbody.velocity = Vector3.Lerp(previousVelocity, vector3, stopwatch / (startWindow / movementSmoothLerpCoof));
            }
            if (IsKeyJustPressedAuthority())
            {
                if (stopwatch >= startWindow && stopwatch <= endWindow)
                {
                    success = true;
                    outer.SetNextState(new ChainDash { activatorSkillSlot = activatorSkillSlot });
                }
                else
                {
                    outer.SetNextStateToMain();
                }
            }
            if (stopwatch > endWindow) outer.SetNextStateToMain();
        }
        public override void OnExit()
        {
            base.OnExit();
            if (demolisherComponent) demolisherComponent.overrideMeleeUtilityMeter = -1f;
            if (characterMotor)
            {
                characterMotor.stepOffset -= extraStepOffset;
                if (characterMotor.Motor) characterMotor.Motor.GroundDetectionExtraDistance -= extraGroundingDistance;
            }
            if (demolisherModel) demolisherModel.trailCount--;
            if (modelAnimator && !success)
            {
                modelAnimator.SetBool("isStep", false);
            }
            if (effectApplied)
            {
                demolisherModel.devilCount--;
            }
        }
        public virtual void SetValues()
        {
            Ray ray = GetAimRay();
            Vector3 aimDirection = ray.direction;
            aimDirection.y = 0f;
            startWindow = baseStartWindow / characterBody.attackSpeed;
            endWindow = baseEndWindow;
            effectDuration = startWindow;
            EffectData effectData = new EffectData
            {
                scale = effectScale,
                genericFloat = effectDuration,
                rootObject = characterBody.mainHurtBox.gameObject,
                origin = characterBody.mainHurtBox.transform.position
            };
            EffectManager.SpawnEffect(Assets.Trail.index, effectData, false);
            if (!effectApplied && demolisherModel)
            {
                demolisherModel.devilCount++;
                effectApplied = true;
            }
            moveVector = (inputBank ? inputBank.moveVector : transform.forward) * (characterMotor ? characterMotor.walkSpeed : characterBody.moveSpeed) * speedMultiplier;
            if (characterDirection)
            {
                characterDirection.forward = aimDirection;
                characterDirection.moveVector = aimDirection;
            }
            modelAnimator = GetModelAnimator();
            if (modelAnimator)
            {
                Vector3 normalizedMoveVector = (inputBank ? inputBank.rawMoveData : transform.forward);//Quaternion.AngleAxis(modelAnimator.transform.eulerAngles.y, -modelAnimator.transform.up) * moveVector.normalized;
                //CharacterAnimatorWalkParamCalculator characterAnimatorWalkParamCalculator = new CharacterAnimatorWalkParamCalculator();
                //characterAnimatorWalkParamCalculator.Update(moveVector.normalized, ray.direction, smoothingParameters, stopwatch);
                modelAnimator.SetFloat(AnimationParameters.forwardSpeed, normalizedMoveVector.y);
                modelAnimator.SetFloat(AnimationParameters.rightSpeed, normalizedMoveVector.x);
                modelAnimator.SetFloat(AnimationParameters.upSpeed, 0f);
                //modelAnimator.SetFloat("aimPitchCycle", 0f);
                //modelAnimator.SetFloat("aimYawCycle", 0f);
                modelAnimator.SetBool(AnimationParameters.isMoving, false);
                modelAnimator.SetBool(AnimationParameters.isGrounded, true);
                modelAnimator.SetBool("isStep", true);
                modelAnimator.SetBool(AnimationParameters.isSprinting, false);
                modelAnimator.SetFloat(AnimationParameters.turnAngle, 0f);
            }
            if (isAuthority) characterBody.AddClientBuff(Assets.InstantMeleeSwing);
            stopwatch = 0f;
        }
        private void HandleSkill(GenericSkill skillSlot, ref InputBankTest.ButtonState buttonState)
        {
            if (!skillSlot) return;
            if (skillSlot.skillDef == null) return;
            if (!buttonState.down && skillSlot.skillDef) return;
            if (skillSlot.mustKeyPress && buttonState.hasPressBeenClaimed) return;
            if (skillSlot.ExecuteIfReady()) buttonState.hasPressBeenClaimed = true;
        }
        //public override void OnSerialize(NetworkWriter writer)
        //{
        //    base.OnSerialize(writer);
        //    writer.Write(success);
        //}
        //public override void OnDeserialize(NetworkReader reader)
        //{
        //    base.OnDeserialize(reader);
        //    success = reader.ReadBoolean();
        //}
    }
    public class Fly : DemolisherBaseState
    {
        public static float baseFlyVectorSmoothTime => FlyConfig.baseFlyVectorSmoothTime.Value;
        public static float baseFlyVectorVisualSmoothTime => FlyConfig.baseFlyVectorSmoothTime.Value;
        public static float baseSpeedMultiplier => FlyConfig.baseSpeedMultiplier.Value;
        public static float baseSpeedSmoothTime => FlyConfig.baseSpeedSmoothTime.Value;
        public static float groundPush => FlyConfig.groundPush.Value;
        public static float stompForce => FlyConfig.stompForce.Value;
        public static float stompBaseRadius => FlyConfig.stompBaseRadius.Value;
        public static float stompBaseDamageCoefficient => FlyConfig.stompBaseDamageCoefficient.Value;
        public static float stompVelocityDamageCoefficient => FlyConfig.stompVelocityDamageCoefficient.Value;
        public static float stompVelocityRadiusMultiplier => FlyConfig.stompVelocityRadiusMultiplier.Value;
        public static BlastAttack.FalloffModel stompFalloff => FlyConfig.stompFalloff.Value;
        public static float stompProcCoefficient => FlyConfig.stompProcCoefficient.Value;
        public static float minFixedAge = 1f;
        public static float setCameraSmoothTime = 0.2f;
        public static float unsetCameraSmoothTime = 0.2f;
        public static int cameraOverridePriority = 6;
        public static float baseShake = 0.2f;
        public float shake;
        public Vector3 flyVector;
        public Vector3 flyVectorVisual;
        public Vector3 flyVectorVelocity;
        public Vector3 flyVectorVisualVelocity;
        //public float flyVectorSmoothTime;
        //public float flyVectorVisualSmoothTime;
        public float maxSpeed;
        public float speed;
        public float speedVelocity;
        public Animator animator;
        public bool effectApplied;
        public Transform cameraTransform;
        public CameraTargetParams.CameraParamsOverrideHandle cameraParamsOverrideHandle;
        public Vector3 picotPosition;
        public float fixedAgeAddition;
        public override void OnEnter()
        {
            base.OnEnter();
            if (!characterMotor)
            {
                outer.SetNextStateToMain();
                return;
            }
            animator = GetModelAnimator();
            if (animator)
            {
                animator.SetBool("isFlying", true);
            }
            SetValues();
            effectApplied = true;
            if (demolisherModelLocator)
            {
                demolisherModelLocator.overrideTargetNormalCount++;
                demolisherModelLocator.overrideTargetNormal = flyVectorVisual;
            }
            if (demolisherModel)
            {
                demolisherModel.devilCount++;
                demolisherModel.shakeWeight += shake;
            }
            if (NetworkServer.active) characterBody.AddBuff(RoR2Content.Buffs.ArmorBoost);
            if (isAuthority)
            {
                if (cameraTargetParams)
                {
                    picotPosition = cameraTargetParams.cameraPivotTransform.localPosition;
                    cameraTargetParams.cameraPivotTransform.localPosition = Vector3.zero;
                    CharacterCameraParamsData characterCameraParamsData = new CharacterCameraParamsData
                    {
                        fov = cameraTargetParams.currentCameraParamsData.fov,
                        isFirstPerson = cameraTargetParams.currentCameraParamsData.isFirstPerson,
                        idealLocalCameraPos = cameraTargetParams.currentCameraParamsData.idealLocalCameraPos,
                        maxPitch = cameraTargetParams.currentCameraParamsData.maxPitch,
                        minPitch = cameraTargetParams.currentCameraParamsData.minPitch,
                        overrideFirstPersonFadeDuration = cameraTargetParams.currentCameraParamsData.overrideFirstPersonFadeDuration,
                        pivotVerticalOffset = 0f,
                        wallCushion = cameraTargetParams.currentCameraParamsData.wallCushion
                    };
                    CameraTargetParams.CameraParamsOverrideRequest cameraParamsOverrideRequest = new CameraTargetParams.CameraParamsOverrideRequest
                    {
                        cameraParamsData = characterCameraParamsData,
                        priority = cameraOverridePriority
                    };
                    cameraParamsOverrideHandle = cameraTargetParams.AddParamsOverride(cameraParamsOverrideRequest, setCameraSmoothTime);
                }
                characterMotor.onMovementHit += CharacterMotor_onMovementHit;
                if (characterMotor.isGrounded)
                {
                    characterMotor.Motor.ForceUnground();
                    characterMotor.velocity += characterMotor.estimatedGroundNormal * groundPush;
                }
                //foreach (CameraRigController cameraRigController in CameraRigController.readOnlyInstancesList)
                //{
                //    cameraRigController.SetOverrideCam(this, setCameraSmoothTime);
                //}
            }
        }

        private void CharacterMotor_onMovementHit(ref CharacterMotor.MovementHitInfo movementHitInfo)
        {
            if (!isAuthority) return;
            outer.SetNextStateToMain();
            float magnitude = characterMotor.velocity.magnitude;
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
                position = characterBody.corePosition,
                damageType = new DamageTypeCombo(DamageType.Stun1s, DamageTypeExtended.Generic, GetDamageSource()),
                procCoefficient = stompProcCoefficient,
                radius = stompBaseRadius + magnitude * stompVelocityRadiusMultiplier,
                teamIndex = GetTeam(),
            };
            blastAttack.Fire();
            EffectData effectData = new()
            {
                origin = blastAttack.position,
                scale = blastAttack.radius,
            };
            EffectManager.SpawnEffect(Assets.Explosion.prefab, effectData, true);
        }

        public void SetValues()
        {
            shake = baseShake;
            flyVector = Vector3.up;
            flyVectorVisual = flyVector;
            maxSpeed = baseSpeedMultiplier;
            if (!characterMotor) return;
            float magnitude = characterMotor.velocity.magnitude;
            if (magnitude > characterBody.moveSpeed)
            {
                speed = characterMotor.velocity.magnitude;
                flyVector = characterMotor.velocity.normalized;
                flyVectorVisual = flyVector;
                float newSpeed = speed;
                while (newSpeed >= 0f) // I am so bad at math :sob:
                {
                    newSpeed -= characterBody.moveSpeed * baseSpeedMultiplier / Mathf.Min(minFixedAge, fixedAgeAddition) * Time.fixedDeltaTime;
                    fixedAgeAddition += Time.fixedDeltaTime;
                }
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Ray ray = GetAimRay();
            if (NetworkServer.active) characterBody.AddTimedBuff(Assets.IgnoreBoots, 0.1f, 1);
            if (!isAuthority) return;
            //if (skillLocator && inputBank)
            //{
            //    HandleSkill(skillLocator.primary, ref inputBank.skill1);
            //    HandleSkill(skillLocator.secondary, ref inputBank.skill2);
            //    HandleSkill(skillLocator.utility, ref inputBank.skill3);
            //}
            //if (speed < maxSpeed) speed = Mathf.SmoothDamp(speed, maxSpeed, ref speedVelocity, baseSpeedSmoothTime / characterBody.attackSpeed, float.MaxValue, Time.fixedDeltaTime);
            speed += characterBody.moveSpeed * baseSpeedMultiplier / Mathf.Min(minFixedAge, fixedAge + fixedAgeAddition) * Time.fixedDeltaTime;
            flyVector = Vector3.SmoothDamp(flyVector, ray.direction, ref flyVectorVelocity, baseFlyVectorSmoothTime / characterBody.attackSpeed, float.MaxValue, Time.fixedDeltaTime);
            characterMotor.SetVelocityOverride(flyVector * speed);

        }
        public override void Update()
        {
            base.Update();
            if (demolisherModelLocator)
            {
                Ray ray = GetAimRay();
                flyVectorVisual = Vector3.SmoothDamp(flyVectorVisual, ray.direction, ref flyVectorVisualVelocity, baseFlyVectorVisualSmoothTime / characterBody.attackSpeed, float.MaxValue, Time.deltaTime);
                demolisherModelLocator.overrideTargetNormal = flyVectorVisual;
            }
            if (characterDirection)
            {
                characterDirection.forward = flyVectorVisual;
                characterDirection.moveVector = flyVectorVisual;
            }
        }
        public override void OnExit()
        {
            base.OnExit();
            if (effectApplied && demolisherModel)
            {
                demolisherModel.devilCount--;
                demolisherModel.shakeWeight -= shake;
            }
            if (animator)
            {
                animator.SetBool("isFlying", false);
            }
            if (effectApplied && demolisherModelLocator) demolisherModelLocator.overrideTargetNormalCount--;
            if (NetworkServer.active) characterBody.RemoveBuff(RoR2Content.Buffs.ArmorBoost);
            if (isAuthority)
            {
                //foreach (CameraRigController cameraRigController in CameraRigController.readOnlyInstancesList)
                //{
                //    cameraRigController.SetOverrideCam(null, unsetCameraSmoothTime);
                //}
                if (cameraTargetParams)
                {
                    cameraTargetParams.RemoveParamsOverride(cameraParamsOverrideHandle, unsetCameraSmoothTime);
                    cameraTargetParams.cameraPivotTransform.localPosition = picotPosition;
                }
                if (characterMotor)
                {
                    characterMotor.onMovementHit -= CharacterMotor_onMovementHit;
                    characterMotor.SetVelocityOverride(Vector3.zero);
                    characterMotor.velocity = Vector3.zero;
                }
            }

        }
        public void HandleSkill(GenericSkill skillSlot, ref InputBankTest.ButtonState buttonState)
        {
            if (!skillSlot) return;
            if (skillSlot.skillDef == null) return;
            if (!buttonState.down && skillSlot.skillDef) return;
            if (skillSlot.mustKeyPress && buttonState.hasPressBeenClaimed) return;
            if (skillSlot.ExecuteIfReady())
                buttonState.hasPressBeenClaimed = true;
        }
        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Skill;
    }
    public class Laser : BaseMeleeAttack
    {
        public static float damageCoefficient => LaserConfig.damageCoefficient.Value;
        public static float procCoefficient => LaserConfig.procCoefficient.Value;
        public static float force => LaserConfig.force.Value;
        public static float range => LaserConfig.range.Value;
        public static float radius => LaserConfig.radius.Value;
        public static float hitInterval => LaserConfig.hitInterval.Value;
        public static float hoverTimeSince = 0.5f;
        public static float hoverSmoothTime = 0.5f;
        public static float hoverTarget = 0f;
        public static float spread = 1.5f;
        public static float baseDuration = 6f;
        public static float baseShakeAddition = 0.2f;
        public static float visualLaserSmoothTime = 0.05f;
        public float newHitInterval;
        public Vector3 laserVelocity;
        public float shakeAddition;
        public Transform laserEffect;
        public float stopwatch;
        private float hoverVelocity;
        private float hoverStart;
        private float hover;
        private bool crit;
        public Animator animator;
        private LoopSoundManager.SoundLoopPtr loopPtr;
        private GameObject sfxObject;
        private Transform chestTransform;
        private Transform laserRotator;
        private bool floating;

        public override DamageSource damageSource => GetDamageSource();

        public override void OnEnter()
        {
            base.OnEnter();
            newHitInterval = hitInterval / characterBody.attackSpeed;
            chestTransform = FindModelChild("UpperChest");
            PlayAnimation("Gesture, Override", "BufferEmpty");
            StartAimMode(2f, true);
            sfxObject = new GameObject("sfxobject");
            sfxObject.transform.SetParent(transform, false);
            this.loopPtr = LoopSoundManager.PlaySoundLoopLocal(base.gameObject, EntityStates.VoidRaidCrab.SpinBeamAttack.loopSound);
            Util.PlaySound(EntityStates.VoidRaidCrab.SpinBeamAttack.enterSoundString, sfxObject);
            currentMeleeWeaponDef = null;
            shakeAddition = baseShakeAddition;
            if (demolisherModel)
            {
                demolisherModel.devilCount++;
                demolisherModel.shakeWeight += shakeAddition;
            }
            animator = GetModelAnimator();
            if (animator) animator.SetBool("isChestFiring", true);
            EffectData effectData = new EffectData
            {
                rootObject = chestTransform ? chestTransform.gameObject : gameObject
            };
            EffectManager.SpawnEffect(Assets.LaserEffect.index, effectData, false);
            laserEffect = effectData.GetEffectInstance() ? effectData.GetEffectInstance().transform : null;
            Ray ray = GetAimRay();
            if (laserEffect)
            {
                laserEffect.Find("laser").localScale = new Vector3(radius, radius, range);
                laserEffect.forward = ray.direction;
                if (chestTransform)
                {
                    laserRotator = new GameObject("LaserRotator").transform;
                    laserRotator.SetParent(transform, false);
                    laserRotator.forward = ray.direction;
                    CopyTransform copyTransform = laserEffect.GetComponent<CopyTransform>();
                    if (copyTransform) copyTransform.copyTransform = laserRotator;
                }
            }
            CreateBulletAttack();
            UpdateBulletAttack(characterBody.damage * damageCoefficient, procCoefficient, 0f, force, RollCrit(), radius, range, true);
            if (NetworkServer.active) characterBody.AddBuff(RoR2Content.Buffs.ArmorBoost);
            if (!isAuthority) return;
            
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            characterBody.AddSpreadBloom(spread);
            ContinueFireMeleeAttack(GetAimRay());
            stopwatch += Time.fixedDeltaTime;
            if (stopwatch >= newHitInterval)
            {
                stopwatch = 0f;
                UpdateBulletAttack(characterBody.damage * damageCoefficient, procCoefficient, 0f, force, RollCrit(), radius, range, true);
                newHitInterval = hitInterval / characterBody.attackSpeed;
                if (activatorSkillSlot) activatorSkillSlot.stock--;
            }
            if (!isAuthority) return;
            if (characterMotor && !characterMotor.isGrounded && characterMotor.lastGroundedTime.timeSince >= hoverTimeSince)
            {
                if (!floating)
                {
                    hoverStart = characterMotor.velocity.y;
                    hover = hoverStart;
                    floating = true;
                }
                hover = Mathf.SmoothDamp(hover, hoverTarget, ref hoverVelocity, hoverSmoothTime, float.MaxValue, Time.fixedDeltaTime);
                characterMotor.velocity.y = hover;
            }

            if ((activatorSkillSlot ? activatorSkillSlot.stock <= 0 : fixedAge >= baseDuration) || !IsKeyDownAuthority()) outer.SetNextStateToMain();
        }
        public override void Update()
        {
            base.Update();
            StartAimMode(2f, true);
            Ray ray = GetAimRay();
            if (laserEffect)
            {
                if (laserRotator)
                {
                    laserRotator.forward = Vector3.SmoothDamp(laserRotator.forward, ray.direction, ref laserVelocity, visualLaserSmoothTime, float.MaxValue, Time.deltaTime);
                }
                else
                {
                    laserEffect.forward = Vector3.SmoothDamp(laserEffect.forward, ray.direction, ref laserVelocity, visualLaserSmoothTime, float.MaxValue, Time.deltaTime);
                }
            }
            
        }
        public override void OnExit()
        {
            base.OnExit();
            LoopSoundManager.StopSoundLoopLocal(this.loopPtr);
            if (sfxObject)
            {
                Destroy(sfxObject);
            }
            //Util.PlaySound(EntityStates.VoidRaidCrab.SpinBeamAttack.enterSoundString.Replace("Play", "Stop"), base.gameObject);
            if (demolisherModel)
            {
                demolisherModel.devilCount--;
                demolisherModel.shakeWeight -= shakeAddition;
            }
            if (animator)
            {
                animator.SetBool("isChestFiring", false);
            }
            if (laserEffect)
            {
                EffectManagerHelper effectManagerHelper = laserEffect.GetComponent<EffectManagerHelper>();
                if (effectManagerHelper && effectManagerHelper.OwningPool != null)
                {
                    effectManagerHelper.OwningPool.ReturnObject(effectManagerHelper);
                }
                else
                {
                    Destroy(laserEffect.gameObject);
                }
                if (laserRotator) Destroy(laserRotator.gameObject);
            }
            if (NetworkServer.active) characterBody.RemoveBuff(RoR2Content.Buffs.ArmorBoost);
        }
    }
    public abstract class DemolisherElevatorBaseState : EntityState
    {
        public DemolisherElevatorController demolisherElevatorController { get; private set; }
        public VehicleSeat vehicleSeat => demolisherElevatorController.vehicleSeat;
        public ChildLocator childLocator => demolisherElevatorController.childLocator;
        public override void OnEnter()
        {
            base.OnEnter();
            demolisherElevatorController = gameObject.GetComponent<DemolisherElevatorController>();

        }
    }
    public class Ascend : DemolisherElevatorBaseState
    {
        public static float ascendAnimationTime = 4f;
        public float simulationSpeedVelocity;
        public float duration;
        public float duration2;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = ascendAnimationTime;
            duration2 = ascendAnimationTime;
            PlayAnimation("Base", "Ascend", "Elevator.playbackRate", duration);
        }
        public override void Update()
        {
            base.Update();
            duration2 -= Time.deltaTime;
            foreach (ParticleSystem particleSystem in demolisherElevatorController.chains)
            {
                if (!particleSystem) continue;
                particleSystem.playbackSpeed = duration2;
                //ParticleSystem.MainModule main = particleSystem.main;
                //float speed = Mathf.SmoothDamp(main.simulationSpeed, 0f, ref simulationSpeedVelocity, ascendAnimationTime, float.PositiveInfinity, Time.deltaTime);
                //main.simulationSpeed = speed;
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!isAuthority) return;
            duration -= Time.fixedDeltaTime;
            if (duration <= 0f) outer.SetState(new Arrived());
        }
    }
    public class Arrived : DemolisherElevatorBaseState
    {
        public float duration;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = 1f;
            //PlayAnimation("Base", "Idle");
            Util.PlaySound("Play_UI_podSteamLoop", gameObject);
            vehicleSeat.handleVehicleExitRequestServer.AddCallback(new CallbackCheck<bool, GameObject>.CallbackDelegate(HandleVehicleExitRequest));
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge > 0f) demolisherElevatorController.exitAllowed = true;
        }
        public void HandleVehicleExitRequest(GameObject gameObject, ref bool? result)
        {
            demolisherElevatorController.exitAllowed = false;
            outer.SetNextState(new Open());
            result = new bool?(true);
        }
        public override void OnExit()
        {
            base.OnExit();
            vehicleSeat.handleVehicleExitRequestServer.RemoveCallback(new CallbackCheck<bool, GameObject>.CallbackDelegate(HandleVehicleExitRequest));
            demolisherElevatorController.exitAllowed = false;
            Util.PlaySound("Play_UI_podSteamLoop", gameObject);
        }
        public override void Update()
        {
            base.Update();
            if (duration >= 0f)
            {
                duration -= Time.deltaTime;
                if (duration < 0f) duration = 0f;
            }
            foreach (ParticleSystem particleSystem in demolisherElevatorController.chains)
            {
                if (!particleSystem) continue;
                ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
                particleSystem.GetParticles(particles);
                for (int i = 0; i < particles.Length; i++)
                {
                    ref ParticleSystem.Particle particle = ref particles[i];
                    Color color = particle.color;
                    color.a = duration;
                    particle.color = color;
                }
            }
        }
    }
    public class Open : DemolisherElevatorBaseState
    {
        public static float damageCoefficient = 2f;
        public static float radius = 4f;
        public static float procCoefficient = 1f;
        public static float force = 300f;
        public static float openAnimationTime = 0.2f;
        public static float exitSpeed = 12f;
        public static BlastAttack.FalloffModel falloffModel = BlastAttack.FalloffModel.Linear;
        public override void OnEnter()
        {
            base.OnEnter();
            foreach (ParticleSystem particleSystem in demolisherElevatorController.chains)
            {
                if (!particleSystem) continue;
                particleSystem.gameObject.SetActive(false);
            }
            PlayAnimation("Doors", "Open", "Elevator.playbackRate", openAnimationTime);
            Transform exitTransform = childLocator?.FindChild("Exit");
            GameObject passenger = vehicleSeat?.passengerBodyObject;
            if (!demolisherElevatorController) return;
            if (NetworkServer.active && vehicleSeat && vehicleSeat.currentPassengerBody) vehicleSeat.EjectPassenger(vehicleSeat.currentPassengerBody.gameObject);
            if (passenger && exitTransform)
            {
                CharacterBody characterBody = passenger.GetComponent<CharacterBody>();
                if (isAuthority)
                {
                    BlastAttack blastAttack = new BlastAttack
                    {
                        attacker = characterBody.gameObject,
                        attackerFiltering = AttackerFiltering.Default,
                        baseDamage = characterBody.damage * damageCoefficient,
                        baseForce = force,
                        crit = characterBody.RollCrit(),
                        damageColorIndex = DamageColorIndex.Default,
                        falloffModel = falloffModel,
                        inflictor = characterBody.gameObject,
                        position = exitTransform.position,
                        damageType = new DamageTypeCombo(DamageType.Stun1s, DamageTypeExtended.Generic, DamageSource.NoneSpecified),
                        procCoefficient = procCoefficient,
                        radius = radius,
                        teamIndex = characterBody.teamComponent ? characterBody.teamComponent.teamIndex : TeamIndex.Neutral,
                    };
                    blastAttack.Fire();
                    EffectData effectData = new()
                    {
                        origin = blastAttack.position,
                        scale = blastAttack.radius,
                    };
                    EffectManager.SpawnEffect(Assets.Explosion.prefab, effectData, true);
                }
                Vector3 velocity = exitTransform.forward * exitSpeed;
                if (characterBody.netIdentity.hasAuthority)
                {
                    if (characterBody.characterDirection)
                    {
                        characterBody.characterDirection.forward = exitTransform.forward;
                        characterBody.characterDirection.moveVector = exitTransform.forward;
                    }
                    if (characterBody.characterMotor)
                    {
                        characterBody.characterMotor.velocity = velocity;
                    }
                    else if (characterBody.rigidbody)
                    {
                        characterBody.rigidbody.velocity = velocity;
                    }
                }

            }
            InstantiatePrefabBehavior instantiatePrefabBehavior = GetComponent<InstantiatePrefabBehavior>();
            if (!instantiatePrefabBehavior) return;
            Transform transform = instantiatePrefabBehavior.targetTransform;
            if (!transform) return;
            Transform transform1 = transform.Find("QuestVolatileBatteryWorldPickup(Clone)");
            if (!transform1) return;
            GenericPickupController genericPickupController = transform1.GetComponent<GenericPickupController>();
            if (genericPickupController) genericPickupController.enabled = true;
        }
        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
