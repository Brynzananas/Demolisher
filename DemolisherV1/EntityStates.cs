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
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements.StyleSheets;
using static RoR2.BodyAnimatorSmoothingParameters;
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
        public bool fireUtilitySkill;
        public bool swapped;
        public bool swapping;
        public GenericSkill utilitySkill => skillLocator && skillLocator.utility ? skillLocator.utility : null;
        public GenericSkill secondarySkill => skillLocator && skillLocator.secondary ? skillLocator.secondary : null;
        public GenericSkill specialSkill => skillLocator && skillLocator.special ? skillLocator.special : null;
        public DemolisherComponent demolisherComponent;
        public override void OnEnter()
        {
            base.OnEnter();
            demolisherComponent = gameObject.GetComponent<DemolisherComponent>();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (inputBank)
            {
                InputBankTest.ButtonState utilityButtonState = inputBank.skill3;
                InputBankTest.ButtonState secondaryButtonState = inputBank.skill2;
                InputBankTest.ButtonState specialButtonState = inputBank.skill4;
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
                    if (utilitySkill)
                    {
                        if (CanExecuteSkill(utilitySkill) && utilitySkill.ExecuteIfReady()) utilityButtonState.hasPressBeenClaimed = true;
                    }
                }
            }
        }
        public void StartSwapping()
        {

        }
        public void StopSwapping()
        {

        }
        public override bool CanExecuteSkill(GenericSkill skillSlot)
        {
            if (skillSlot)
            {
                if (utilitySkill && skillSlot == utilitySkill && !fireUtilitySkill) return false;
                if (swapping && (skillSlot == secondarySkill || skillSlot == specialSkill)) return false;
            }
            return base.CanExecuteSkill(skillSlot);
        }
        public void SwapWeapons()
        {
            if (demolisherComponent == null) return;
            demolisherComponent.SwapWeapons();
        }
    }
    public class DemolisherBaseState : BaseSkillState
    {
        public DemolisherComponent demolisherComponent;
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
            demolisherComponent = gameObject.GetComponent<DemolisherComponent>();
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
        public BulletAttack bulletAttack;
        public override void OnEnter()
        {
            base.OnEnter();
        }
        public virtual void CreateBulletAttack()
        {
            bulletAttack = new BulletAttack
            {
                aimVector = Vector3.zero,
                allowTrajectoryAimAssist = false,
                bulletCount = 1,
                cheapMultiBullet = false,
                damage = 0f,
                damageColorIndex = DamageColorIndex.Default,
                damageType = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, damageSource),
                falloffModel = BulletAttack.FalloffModel.None,
                force = 0f,
                isCrit = false,
                maxSpread = 0f,
                minSpread = 0f,
                multiBulletOdds = 0f,
                origin = Vector3.zero,
                owner = gameObject,
                radius = 0f,
                maxDistance = 0f,
                procCoefficient = 0f,
                sniper = false,
                smartCollision = true,
                weapon = gameObject,
                trajectoryAimAssistMultiplier = 0f,
                spreadYawScale = 0f,
                spreadPitchScale = 0f,
            };
            bulletAttack.SetIgnoreHitTargets(true);
            object attack = bulletAttack;
            currentMeleeWeaponDef?.OneTimeModification(this, ref attack);
        }
        public virtual void UpdateBulletAttack(float damage, float procCoefficient, float force, bool crit, float radius, float distance, bool reset)
        {
            bulletAttack.damage = damage;
            bulletAttack.procCoefficient = procCoefficient;
            bulletAttack.force = force;
            bulletAttack.isCrit = crit;
            bulletAttack.radius = radius;
            bulletAttack.maxDistance = distance;
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
        public abstract float damageCoefficient { get; }
        public abstract float force { get; }
        public GameObject projectile => currentRangedWeaponDef ? currentRangedWeaponDef.projectile : Assets.GrenadeProjectile;
        public abstract DamageSource damageSource { get; }
        public virtual void FireProjectile(Ray ray, float damage, float force, bool crit)
        {
            StartAimMode(2f, true);
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
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
    public class FireGrenadeNetwork : BaseProjectileAttack, IStateTarget
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
    }
    public class FireGrenade : BaseProjectileAttack
    {
        public override float damageCoefficient => FireGrenadeConfig.damageCoefficient.Value;
        public override float force => FireGrenadeConfig.force.Value;
        public override DamageSource damageSource => GetDamageSource();

        public static float baseDuration => FireGrenadeConfig.baseDuration.Value;
        public float duration;
        public float stopwatch;
        public virtual void SetValues()
        {
            duration = baseDuration / characterBody.attackSpeed;
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
    public class FireGrenadeHold : BaseSkillState
    {
        public static float damageCoefficient => FireGrenadeConfig.damageCoefficient.Value;
        public static float force => FireGrenadeConfig.force.Value;
        public static float maxCharge = 1f;
        public float charge;

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            charge += Time.fixedDeltaTime * characterBody.attackSpeed;
            if (isAuthority && (!IsKeyDownAuthority() || charge >= maxCharge))
            {
                FireGrenadeHoldNetwork fireGrenadeHoldNetwork = EntityStateCatalog.InstantiateState(typeof(FireGrenadeHoldNetwork)) as FireGrenadeHoldNetwork;
                fireGrenadeHoldNetwork.charge = charge;
                fireGrenadeHoldNetwork.maxCharge = maxCharge;
                fireGrenadeHoldNetwork.damageCoefficientTransfer = damageCoefficient;
                fireGrenadeHoldNetwork.forceTransfer = force;
                fireGrenadeHoldNetwork.activatorSkillSlot = activatorSkillSlot;
                outer.SetNextState(fireGrenadeHoldNetwork);
            }
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
    public class MediumMeleeAttack : BaseMeleeAttack
    {
        public static float damageCoefficient => MediumMeleeAttackConfig.damageCoefficient.Value;
        public static float procCoefficient => MediumMeleeAttackConfig.procCoefficient.Value;
        public static float baseAttackDuration => MediumMeleeAttackConfig.baseAttackDuration.Value;
        public static float baseDuration => MediumMeleeAttackConfig.baseDuration.Value;
        public static float radius => MediumMeleeAttackConfig.radius.Value;
        public static float force => MediumMeleeAttackConfig.force.Value;
        public static float maxDistance => MediumMeleeAttackConfig.maxDistance.Value;
        public static float swingUpCrossfade = 0.05f;
        public static float swingDownCrossfade = 0.05f;
        public static float bufferEmptyTransition = 0.05f;
        public override DamageSource damageSource => GetDamageSource();
        public bool instantSwing;
        public bool step;
        public float duration;
        public float stopwatch;
        public float firingStopwatch;
        public GameObject slash;
        public bool firing;
        public float attackDuration;
        public override void OnEnter()
        {
            base.OnEnter();
            SetValues();
            CreateBulletAttack();
            PlayCrossfade("Gesture, Override", "SwingUp1", "Slash.playbackRate", duration, swingUpCrossfade);
        }
        public virtual void SetValues()
        {
            attackDuration = baseAttackDuration / characterBody.attackSpeed;
            duration = baseDuration / characterBody.attackSpeed;
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
                stopwatch = duration;
                Chat.AddMessage("Reset 2");
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
            UpdateBulletAttack(characterBody.damage * damageCoefficient, procCoefficient, force, RollCrit(), radius, maxDistance, true);
            slash = GameObject.Instantiate(Assets.Slash);
            PlayCrossfade("Gesture, Override", step ? "SwingDown2" : "SwingDown1", "Slash.playbackRate", duration, swingDownCrossfade);
            Util.PlaySound("Play_DemoSwordSwing", gameObject);
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
            if (slash)
            {
                Ray ray = GetAimRay();
                slash.transform.position = ray.origin;
                Vector3 vector3 = Quaternion.LookRotation(ray.direction).eulerAngles;
                vector3.z += step ? -30f : 30f;
                float radius = bulletAttack.radius;
                float maxDistance = bulletAttack.maxDistance;
                slash.transform.localScale = new Vector3(step ? radius : -radius, radius, maxDistance);
                slash.transform.eulerAngles = vector3;
            }
        }
        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override", "BufferEmpty", "Slash.playbackRate", bufferEmptyTransition, bufferEmptyTransition);
        }
    }
    public class ShieldCharge : BaseMeleeAttack
    {
        public static float baseDuration = 1.5f;
        public static float baseWalkSpeedMultiplier = 3.5f;
        public static float shieldBashRadiusMultiplier = 6f;
        public static float shieldBashDistance = 3f;
        public static float shieldBashDamageCoefficient = 2f;
        public static float shieldBashSpeedDamageCoefficient = 0.5f;
        public static float shieldBashProcCoefficient = 1f;
        public static float shieldBashBaseForce = 200f;
        public static float shieldBashVelocityForce = 100f;
        public static float shieldBashTimer = 1f;
        public static float shieldBashVelocityForceMultiplier = 1f;
        public static float shieldBashGravityForceMultiplier = 1f;
        public static LayerMask shieldBashLayerMask = LayerIndex.world.mask;
        public Animator modelAnimator;
        public CharacterAnimParamAvailability characterAnimParamAvailability;
        public float duration;
        public float stopwatch;
        public float walkSpeedMultiplier;
        public Vector3 direction;
        public Vector3 directionVisual;
        private SphereCollider sphereCollider;
        private List<HitTarget> hitTargets = [];
        private AimAnimator aimAnimator;

        public override DamageSource damageSource => GetDamageSource();

        public virtual void SetValues()
        {
            duration = baseDuration / characterBody.attackSpeed;
            walkSpeedMultiplier = baseWalkSpeedMultiplier * characterBody.attackSpeed;
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.fixedDeltaTime;
            direction = GetAimRay().direction;
            direction.y = 0;
            direction = direction.normalized;
            Vector3 velocity = direction * characterBody.moveSpeed * walkSpeedMultiplier;
            if (characterMotor)
            {
                velocity.y = characterMotor.velocity.y;
                characterMotor.SetVelocityOverride(velocity);
            }
            else if (rigidbody)
            {
                velocity.y = rigidbody.velocity.y;
                rigidbody.velocity = velocity;
            }
            //if (NetworkServer.active)
            ShieldBash();
            //int count = hitTargets.Count;
            //for (int i = 0; i < count; i++)
            //{
            //    HitTarget hitTarget = hitTargets[i];
            //    hitTarget.timer -= Time.fixedDeltaTime;
            //    if (hitTarget.characterBody == null || hitTarget.timer <= 0f) hitTargets.Remove(hitTarget);
            //}
            if (stopwatch >= duration && isAuthority) outer.SetNextStateToMain();
        }
        public virtual void ShieldBash()
        {
            ContinueFireMeleeAttack(new Ray(characterBody.corePosition, aimDirectionGrounded));
            Vector3 velocity = Vector3.zeroVector;
            if (characterMotor)
            {
                velocity = characterMotor.velocity;
            }
            else if (rigidbody)
            {
                velocity = rigidbody.velocity;
            }
            //float velocityMagnitude = velocity.magnitude;
            Vector3 force = velocity * shieldBashVelocityForceMultiplier + (Physics.gravity * -1f * shieldBashGravityForceMultiplier);
            bulletAttack.SetBonusForce(force);
            UpdateBulletAttack(characterBody.damage * shieldBashDamageCoefficient, shieldBashProcCoefficient, 0f, RollCrit(), characterBody.radius * shieldBashRadiusMultiplier, shieldBashDistance, false);

            /*            Collider[] colliders = Physics.OverlapSphere(characterBody.corePosition + direction * characterBody.radius, characterBody.radius * shieldBashRadiusMultiplier, LayerIndex.CommonMasks.characterBodies + LayerIndex.CommonMasks.fakeActorLayers);
            foreach (Collider collider in colliders)
            {
                CharacterBody characterBody = collider.GetComponent<CharacterBody>();
                if (characterBody == null) continue;
                Vector3 vector3 = characterBody.corePosition - this.characterBody.corePosition;
                if (characterBody.teamComponent && teamComponent && characterBody.teamComponent.teamIndex == teamComponent.teamIndex) continue;
                if (hitTargets.Find(x => x.characterBody == characterBody).characterBody == characterBody) continue;
                if (characterBody.healthComponent)
                {
                    DamageInfo damageInfo = new()
                    {
                        attacker = gameObject,
                        crit = RollCrit(),
                        damage = this.characterBody.damage * shieldBashDamageCoefficient + this.characterBody.damage * velocityMagnitude * shieldBashSpeedDamageCoefficient,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = new DamageTypeCombo(DamageType.Stun1s, DamageTypeExtended.Generic, GetDamageSource()),
                        position = characterBody.corePosition,
                        inflictor = gameObject,
                        procCoefficient = shieldBashProcCoefficient,
                    };
                    characterBody.healthComponent.TakeDamageProcess(damageInfo);
                    Vector3 force = velocity * shieldBashVelocityForceMultiplier + Physics.gravity * -1f * shieldBashGravityForceMultiplier;
                    if (characterBody.characterMotor)
                    {
                        PhysForceInfo physForceInfo = new PhysForceInfo
                        {
                            ignoreGroundStick = true,
                            disableAirControlUntilCollision = true,
                            massIsOne = true,
                            force = force
                        };
                        characterBody.characterMotor.ApplyForceImpulse(physForceInfo);
                    }
                    else if (characterBody.rigidbody)
                    {
                        characterBody.rigidbody.AddForce(force, ForceMode.Impulse);
                    }
                    hitTargets.Add(new HitTarget { characterBody = characterBody, timer = shieldBashTimer });
                    EffectManager.SimpleSoundEffect(Assets.ShieldBashSound.index, characterBody.corePosition, true);
                }
            }*/
        }
        public override void OnEnter()
        {
            base.OnEnter();
            SetValues();
            CreateBulletAttack();
            bulletAttack.SetForceMassIsOne(true);
            bulletAttack.SetForceAlwaysApply(true);
            bulletAttack.SetForceDisableAirControlUntilCollision(true);
            UpdateBulletAttack(characterBody.damage * shieldBashDamageCoefficient, shieldBashProcCoefficient, shieldBashBaseForce, RollCrit(), characterBody.radius * shieldBashRadiusMultiplier, shieldBashDistance, true);
            ConstantUpdateBulletAttack(new Ray(characterBody.corePosition, aimDirectionGrounded));
            //Util.PlaySound("Play_stickybomblauncher_det", gameObject);
            if (characterMotor)
            {
                characterMotor.walkSpeedPenaltyCoefficient *= walkSpeedMultiplier;
            }
            modelAnimator = GetModelAnimator();
            if (modelAnimator)
            {
                aimAnimator = modelAnimator.GetComponent<AimAnimator>();
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
            PlayAnimation("Gesture, Override", "ShieldChargeStart");
        }
        public override void Update()
        {
            base.Update();
            directionVisual = GetAimRay().direction;
            directionVisual.y = 0f;
            directionVisual.Normalize();
            if (modelAnimator)
            {
                modelAnimator.SetBool(AnimationParameters.isGrounded, isGrounded);
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
            PlayAnimation("Gesture, Override", "ShieldChargeEnd");
            if (characterMotor)
            {
                float walkSpeed = characterMotor.walkSpeed;
                characterMotor.walkSpeedPenaltyCoefficient /= walkSpeedMultiplier;
                characterMotor.SetVelocityOverride(Vector3.zero);
                Vector3 velocity = characterMotor.velocity;
                float y = velocity.y;
                velocity.y = 0f;
                if (velocity.sqrMagnitude > walkSpeed * walkSpeed)
                {
                    velocity = Vector3.ClampMagnitude(velocity, walkSpeed);
                    velocity.y = y;
                    characterMotor.velocity = velocity;
                }
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
        public struct HitTarget
        {
            public CharacterBody characterBody;
            public float timer;
        }
    }
    public class Detonate : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            ProjectileDetonator projectileDetonator = gameObject.GetOrAddComponent<ProjectileDetonator>();
            if (NetworkServer.active)
                if (projectileDetonator) projectileDetonator.DetonateAll();
            Util.PlaySound("Play_stickybomblauncher_det", gameObject);
            outer.SetNextStateToMain();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
    public class WhirlwindMelee : BaseMeleeAttack
    {
        public static float damageCoefficient = 2f;
        public static float procCoefficient = 1f;
        public static float maxDistance = 9f;
        public static float force = 300f;
        public static float radius = 3f;
        public static float maxDuration = 5f;
        public static float spinDuration = 0.25f;
        public static float spinEnterCrossfade = 0.05f;
        public static float spinExitCrossfade = 0.05f;
        public override DamageSource damageSource => GetDamageSource();
        public static float baseDegreesPerSecond = 90f;
        public static float baseRotationsPerSecond = 5f;
        public float duration;
        public float degreesPerSecond;
        public float rotationsPerSecond;
        public float interval;
        public float stopwatch;
        public Vector3 direction;
        public Vector3 rotation;
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            ContinueFireMeleeAttack(new Ray { direction = rotation, origin = characterBody.corePosition });
            stopwatch += Time.fixedDeltaTime;
            if (stopwatch >= interval)
            {
                UpdateBulletAttack(characterBody.damage * damageCoefficient, procCoefficient, force, RollCrit(), radius, maxDistance, true);
                stopwatch = 0f;
            }
            direction = Vector3.RotateTowards(direction, aimDirectionGrounded, degreesPerSecond / 57f * Time.fixedDeltaTime, 0f);
            rotation = Quaternion.AngleAxis(rotationsPerSecond * 360f * Time.fixedDeltaTime, Vector3.up) * rotation;
            if (characterDirection)
            {
                characterDirection.forward = rotation;
            }
            if (characterMotor)
            {
                characterMotor.moveDirection = direction;
            }
            else if (rigidbody)
            {
                rigidbody.velocity += direction * characterBody.moveSpeed;
            }
            if (fixedAge >= duration && isAuthority) outer.SetNextStateToMain();
        }
        public virtual void SetValues()
        {
            degreesPerSecond = baseDegreesPerSecond;
            rotationsPerSecond = baseRotationsPerSecond * characterBody.attackSpeed;
            interval = 1f / rotationsPerSecond;
            duration = maxDuration;
        }
        public override void OnEnter()
        {
            base.OnEnter();
            direction = aimDirectionGrounded;
            rotation = direction;
            PlayCrossfade("FullBody, Override", "Spin", "Slash.playbackRate", spinDuration, spinEnterCrossfade);
            Util.PlaySound("Play_DemoWhirlwind", gameObject);
            SetValues();
            CreateBulletAttack();
            UpdateBulletAttack(damageCoefficient * characterBody.damage, procCoefficient, force, RollCrit(), radius, maxDistance, true);
        }
        public override void OnExit()
        {
            base.OnExit();
            PlayAnimation("FullBody, Override", "SpinEnd");
            Util.PlaySound("Stop_DemoWhirlwind", gameObject);
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
    public class Parry : BaseSkillState
    {
        public static Dictionary<GameObject, List<Parry>> activeParries = new Dictionary<GameObject, List<Parry>>();
        public static float baseParryWindow = 1f;
        public static float invincibilityTime = 1f;
        public static float damageCoefficient = 2f;
        public static float procCoefficient = 1f;
        public static float force = 300f;
        public static float radius = 18f;
        public static float effectScale = 2f;
        public static float baseMovementStart = 24f;
        public static float baseMovementEnd = 300f;
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
            if (activeParries.ContainsKey(gameObject)) activeParries[gameObject].Remove(this);
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
            if (characterMotor) characterMotor.ApplyForce(ray.direction * -1f * baseMovementEnd, true);
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
    public class FireCollapse : BaseState
    {
        public static float bulletDamageCoefficient = 100f;
        public static float bulletProcCoefficient = 1f;
        public static float explosionDamageCoefficient = 100f;
        public static float explosionProcCoefficient = 1f;
        public static float bulletForce = 3000f;
        public static float explosionForce = 3000f;
        public static float bulletRadius = 1f;
        public static float explosionRadius = 24f;
        public static float fireANimationDuration = 0.5f;

        public override void OnEnter()
        {
            base.OnEnter();
            Fire();
        }
        public void Fire()
        {
            PlayAnimation("Gesture, Override", "CollapseFire", "Slash.playbackRate", fireANimationDuration / attackSpeedStat);
            Util.PlaySound("Play_tacky_grenadier_shoot_crit", gameObject);
            ChildLocator childLocator = modelLocator && modelLocator.modelTransform ? modelLocator.modelTransform.GetComponent<ChildLocator>() : null;
            if (childLocator)
            {
                Transform power = childLocator.FindChild("PowerL");
                if (power) power.gameObject.SetActive(false);
            }
            if (isAuthority)
            {
                Ray ray = GetAimRay();
                BulletAttack bulletAttack = new BulletAttack
                {
                    aimVector = ray.direction,
                    allowTrajectoryAimAssist = false,
                    bulletCount = 1,
                    cheapMultiBullet = false,
                    damage = characterBody.damage * bulletDamageCoefficient,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.Special),
                    falloffModel = BulletAttack.FalloffModel.None,
                    force = bulletForce,
                    isCrit = RollCrit(),
                    maxSpread = 0f,
                    minSpread = 0f,
                    multiBulletOdds = 0f,
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
                    muzzleName = "PowerL"
                };
                bulletAttack.Fire();
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
                    cheapMultiBullet = false,
                    damage = damageCoefficient * damageStat,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, GetDamageSource()),
                    falloffModel = BulletAttack.FalloffModel.None,
                    force = force,
                    isCrit = RollCrit(),
                    maxSpread = 0f,
                    minSpread = 0f,
                    multiBulletOdds = 0f,
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
        public static float baseDuration = 0.5f;
        public static float damageCoefficient = 2f;
        public static float force = 300f;
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
            PlayAnimation("Gesture, Override", "GroundSlashFire", "Slash.playbackRate", 0.5f / attackSpeedStat);
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
    public class Slicing : DemolisherBaseState
    {
        public static int count;
        public static GameObject ppEffect;
        public static float damageCoefficient = 5f;
        public static float procCoefficient = 1f;
        public static float force = 0f;
        public static float radius = 2f;
        public static float baseTimeDivisionMultiplier = 10f;
        public static float baseDistance = 24f;
        public static float baseDuration = 12f;
        public static int stockMultiplier = 4;
        public float timeDivisionMultiplier;
        public float duration;
        public CharacterMaster characterMaster;
        public PlayerCharacterMasterController playerCharacterMasterController;
        public BaseAI[] baseAIs;
        public BulletAttack bulletAttack;
        public GameObject cracksTrail;
        public virtual void SetValues()
        {
            timeDivisionMultiplier = baseTimeDivisionMultiplier * characterBody.attackSpeed;
            duration = baseDuration;
        }
        public override void OnEnter()
        {
            base.OnEnter();
            SetValues();
            cracksTrail = GameObject.Instantiate(Assets.CracksTrailEffect, characterBody.coreTransform);
            if (isAuthority)
            {
                BrynzaAPI.Utils.ChangeTimescaleForAllClients(Time.timeScale / timeDivisionMultiplier);
                if (activatorSkillSlot) activatorSkillSlot.stock = activatorSkillSlot.stock * stockMultiplier;
            }
            characterMaster = characterBody.master;
            if (characterMaster)
            {
                playerCharacterMasterController = characterMaster.playerCharacterMasterController;
                baseAIs = characterMaster.AiComponents;
            }
            count++;
        }
        public override void OnExit()
        {
            base.OnExit();
            if (isAuthority)
            {
                BrynzaAPI.Utils.ChangeTimescaleForAllClients(Time.timeScale * timeDivisionMultiplier);
                if (activatorSkillSlot) activatorSkillSlot.stock = activatorSkillSlot.stock / stockMultiplier;
            }
            count--;
            if (cracksTrail)
            {
                ParticleSystem particleSystem = cracksTrail.transform.Find("trail").GetComponent<ParticleSystem>();
                cracksTrail.transform.SetParent(null, true);
                particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                GameObject.Destroy(cracksTrail, 1f);
                //cracksTrail.GetComponent<DestroyOnParticleEndAndNoParticles>().enabled = true;
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            //duration -= Time.fixedUnscaledDeltaTime;
            //if(duration <= 0 && isAuthority) outer.SetStateToMain();
        }
        public virtual void CreateBulletAttack()
        {
            bulletAttack = new BulletAttack
            {
                aimVector = Vector3.zero,
                allowTrajectoryAimAssist = false,
                bulletCount = 1,
                cheapMultiBullet = false,
                damage = 0f,
                damageColorIndex = DamageColorIndex.Default,
                damageType = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, GetDamageSource()),
                falloffModel = BulletAttack.FalloffModel.None,
                force = force,
                isCrit = false,
                maxSpread = 0f,
                minSpread = 0f,
                multiBulletOdds = 0f,
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
                //tracerEffectPrefab = Assets.DemolisherTracer.prefab
            };
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
        public virtual void Slice()
        {
            Ray ray = GetAimRay();
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
            if (bulletAttack == null) CreateBulletAttack();
            UpdateBulletAttack(ray, distance, characterBody.damage, RollCrit());
            bulletAttack.Fire();
            TeleportHelper.TeleportBody(characterBody, destination);
            if (characterMotor) characterMotor.velocity = Vector3.zero;
            if (characterDirection)
            {
                characterDirection.forward = ray.direction;
                characterDirection.moveVector = ray.direction;
            }
            if (activatorSkillSlot)
            {
                activatorSkillSlot.stock--;
                if (activatorSkillSlot.stock <= 0) outer.SetStateToMain();
            }
        }
        public override void Update()
        {
            base.Update();
            if (playerCharacterMasterController) playerCharacterMasterController.PollButtonInput();
            if (baseAIs != null) foreach (BaseAI baseAI in baseAIs) baseAI.UpdateBodyInputs();
            if (isAuthority)
            {
                if (inputBank.skill1.justPressed)
                {
                    Slice();
                }
            }
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
    public class ChainDash : BaseSkillState
    {
        public static float baseStartWindow = 0.2f;
        public static float baseEndWindow = 0.4f;
        public static float speedMultiplier = 5f;
        public static float moveVectorSmoothTime = 0.5f;
        public static float baseEffectDuration = 0.1f;
        public static float effectScale = 1f;
        public float effectDuration;
        public float startWindow;
        public float endWindow;
        private bool effectApplied;
        private float stopwatch;
        private bool keyDown;
        private bool wasKeyDown;
        private Vector3 moveVector;
        private Vector3 moveVectorVelocity;
        private Animator modelAnimator;
        private int chainCount;
        private BodyAnimatorSmoothingParameters.SmoothingParameters smoothingParameters;
        private DemolisherModel demolisherModel;
        private bool keyPressed => keyDown && !wasKeyDown;
        public override void OnEnter()
        {
            base.OnEnter();
            Transform modelTransform = GetModelTransform();
            if (modelTransform)
            {
                demolisherModel = modelTransform.GetComponent<DemolisherModel>();
            }
            GetBodyAnimatorSmoothingParameters(out smoothingParameters);
            SetValues();
            wasKeyDown = true;
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.fixedDeltaTime;
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
            }
            moveVector = Vector3.SmoothDamp(moveVector, Vector3.zero, ref moveVectorVelocity, moveVectorSmoothTime, float.MaxValue, Time.fixedDeltaTime);
            if (characterMotor)
            {
                Vector3 vector3 = moveVector;
                vector3.y = characterMotor.velocity.y;
                characterMotor.velocity = vector3;
            }
            else if (rigidbody)
            {
                Vector3 vector3 = moveVector;
                vector3.y = rigidbody.velocity.y;
                rigidbody.velocity = vector3;
            }
            keyDown = IsKeyDownAuthority();
            if (keyPressed)
            {
                if (stopwatch >= startWindow && stopwatch <= endWindow)
                {
                    SetValues();
                }
                else
                {
                    outer.SetNextStateToMain();
                }
            }
            if (stopwatch > endWindow) outer.SetNextStateToMain();
            wasKeyDown = keyDown;
        }
        public override void OnExit()
        {
            base.OnExit();
            if (activatorSkillSlot) activatorSkillSlot.stock--;
            if (modelAnimator)
            {
                modelAnimator.SetBool("isStep", false);
            }
            if (effectApplied)
            {
                demolisherModel.devilCount--;
            }
            if (NetworkServer.active)
            {
                characterBody.SetBuffCount(Assets.InstantMeleeSwing.buffIndex, 0);
            }
        }
        public virtual void SetValues()
        {
            chainCount++;
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
                CharacterAnimatorWalkParamCalculator characterAnimatorWalkParamCalculator = new CharacterAnimatorWalkParamCalculator();
                characterAnimatorWalkParamCalculator.Update(moveVector.normalized, aimDirection, smoothingParameters, stopwatch);
                modelAnimator.SetFloat(AnimationParameters.forwardSpeed, characterAnimatorWalkParamCalculator.animatorWalkSpeed.x);
                modelAnimator.SetFloat(AnimationParameters.rightSpeed, characterAnimatorWalkParamCalculator.animatorWalkSpeed.y);
                modelAnimator.SetFloat(AnimationParameters.upSpeed, 0f);
                //modelAnimator.SetFloat("aimPitchCycle", 0f);
                //modelAnimator.SetFloat("aimYawCycle", 0f);
                modelAnimator.SetBool(AnimationParameters.isMoving, false);
                modelAnimator.SetBool(AnimationParameters.isGrounded, true);
                modelAnimator.SetBool("isStep", true);
                modelAnimator.SetBool(AnimationParameters.isSprinting, false);
                modelAnimator.SetFloat(AnimationParameters.turnAngle, 0f);
            }
            if (NetworkServer.active)
            {
                characterBody.AddBuff(Assets.InstantMeleeSwing);

            }
            else
            {
                characterBody.AddBuffAuthotiry(Assets.InstantMeleeSwing);
            }
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
        public override void OnEnter()
        {
            base.OnEnter();
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
    }
    public class Open : DemolisherElevatorBaseState
    {
        public static float damageCoefficient = 2f;
        public static float radius = 2f;
        public static float procCoefficient = 1f;
        public static float force = 300f;
        public static float openAnimationTime = 0.2f;
        public static float exitSpeed = 12f;
        public static BlastAttack.FalloffModel falloffModel = BlastAttack.FalloffModel.Linear;
        public override void OnEnter()
        {
            base.OnEnter();
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
        }
        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
