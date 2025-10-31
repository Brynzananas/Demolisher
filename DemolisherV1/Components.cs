using BrynzaAPI;
using EntityStates;
using HG;
using JetBrains.Annotations;
using R2API;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements.StyleSheets;

namespace Demolisher
{
    public interface IStateSeeker
    {
        public IStateTarget foundState { get; set; }
        public Func<bool> onStateFound { get;}
        public void FindState(CharacterBody characterBody)
        {
            foreach (GenericSkill genericSkill in characterBody.skillLocator.allSkills)
            {
                IStateTarget stateTarget = genericSkill == null || genericSkill.stateMachine == null || genericSkill.stateMachine.state == null ? null : genericSkill.stateMachine.state as IStateTarget;
                if (stateTarget == null || stateTarget.taken) continue;
                foundState = stateTarget;
                bool stateFound = onStateFound == null || onStateFound.Invoke();
                if (stateFound)
                {
                    foundState.taken = true;
                    break;
                }
            }
        }
    }
    public class DemolisherComponent : MonoBehaviour
    {
        public static float swapSpeed = 0.1f;
        public float test;
        public CharacterBody characterBody;
        public ModelLocator modelLocator;
        public SkillLocator skillLocator;
        public DemolisherModel demolisherModel;
        public GenericSkill meleeWeapon;
        public GenericSkill meleePrimary;
        public GenericSkill meleeSecondary;
        public GenericSkill meleeUtility;
        public GenericSkill meleeSpecial;
        public GenericSkill rangedWeapon;
        public GenericSkill rangedPrimary;
        public GenericSkill rangedSecondary;
        public GenericSkill rangedUtility;
        public GenericSkill rangedSpecial;
        [HideInInspector] public GenericSkill holsterPrimary;
        [HideInInspector] public GenericSkill holsterSecondary;
        [HideInInspector] public GenericSkill holsterUtility;
        [HideInInspector] public GenericSkill holsterSpecial;
        [HideInInspector] public SkillIcon altPrimarySkillIcon;
        [HideInInspector] public SkillIcon altSecondarySkillIcon;
        [HideInInspector] public SkillIcon altUtilitySkillIcon;
        [HideInInspector] public SkillIcon altSpecialSkillIcon;
        [HideInInspector] public ChildLocator childLocator;
        [HideInInspector] public Animator animator;
        [HideInInspector] public int rangedLayerIndex = -1;
        private bool swapping;
        private float swappingVelocity;
        public bool isSwapped
        {
            get
            {
                if (skillLocator == null) return false;
                return skillLocator.primary == rangedPrimary;
            }
        }
        public void Awake()
        {
            if (modelLocator && modelLocator.modelTransform)
            {
                animator = modelLocator.modelTransform.GetComponent<Animator>();
                childLocator = modelLocator.modelTransform.GetComponent<ChildLocator>();
                demolisherModel = modelLocator.modelTransform.GetComponent<DemolisherModel>();
                rangedLayerIndex = animator.GetLayerIndex("BodyRanged");
            }
            if (isSwapped)
            {
                holsterPrimary = meleePrimary;
                holsterSecondary = meleeSecondary;
                holsterUtility = meleeUtility;
                holsterSpecial = meleeSpecial;
                if (demolisherModel) demolisherModel.swordInvisibilityCount++;
            }
            else
            {
                holsterPrimary = rangedPrimary;
                holsterSecondary = rangedSecondary;
                holsterUtility = rangedUtility;
                holsterSpecial = rangedSpecial;
                if (demolisherModel) demolisherModel.gunInvisibilityCount++;
            }
        }
        public void SwapWeapons()
        {
            if (isSwapped)
            {
                if (skillLocator)
                {
                    skillLocator.primary = meleePrimary;
                    skillLocator.secondary = meleeSecondary;
                    skillLocator.utility = meleeUtility;
                    skillLocator.special = meleeSpecial;
                }
                holsterPrimary = rangedPrimary;
                holsterSecondary = rangedSecondary;
                holsterUtility = rangedUtility;
                holsterSpecial = rangedSpecial;
                swapping = false;
                if (demolisherModel)
                {
                    demolisherModel.swordInvisibilityCount--;
                    demolisherModel.gunInvisibilityCount++;
                }
                //ToggleWeapon("Sword", true);
                //ToggleWeapon("Launcher", false);
                EntityState.PlayAnimationOnAnimator(animator, "Gesture, Override", "RangedToMelee", "Slash.playbackRate", 1f / characterBody.attackSpeed);
                Util.PlaySound("Play_draw_sword", gameObject);
            }
            else
            {
                if (skillLocator)
                {
                    skillLocator.primary = rangedPrimary;
                    skillLocator.secondary = rangedSecondary;
                    skillLocator.utility = rangedUtility;
                    skillLocator.special = rangedSpecial;
                }
                holsterPrimary = meleePrimary;
                holsterSecondary = meleeSecondary;
                holsterUtility = meleeUtility;
                holsterSpecial = meleeSpecial;
                if (demolisherModel)
                {
                    demolisherModel.swordInvisibilityCount++;
                    demolisherModel.gunInvisibilityCount--;
                }
                swapping = true;
                //ToggleWeapon("Sword", false);
                //ToggleWeapon("Launcher", true);
                EntityState.PlayAnimationOnAnimator(animator, "Gesture, Override", "MeleeToRanged", "Slash.playbackRate", 1f / characterBody.attackSpeed);
                Util.PlaySound("Play_grenade_launcher_worldreload", gameObject);
            }
        }
        public void ToggleWeapon(string name, bool toggle)
        {
            if (childLocator == null) return;
            Transform transform = childLocator.FindChild(name);
            if (transform == null) return;
            transform.gameObject.SetActive(toggle);
        }
        public void Update()
        {
            if (rangedLayerIndex >= 0) animator.SetLayerWeight(rangedLayerIndex, Mathf.SmoothDamp(animator.GetLayerWeight(rangedLayerIndex), swapping ? 1f : 0f, ref swappingVelocity, swapSpeed));
        }
    }
    [RequireComponent(typeof(ProjectileController))]
    public class ProjectileRemoteDetonation : MonoBehaviour
    {
        public string identificator = "Sticky";
        public ProjectileDetonator projectileDetonator
        {
            get
            {
                if (_projectileDetonator)
                {
                    return _projectileDetonator;
                }
                _projectileDetonator = projectileController.owner ? projectileController.owner.GetOrAddComponent<ProjectileDetonator>() : null;
                return _projectileDetonator;
            }
        }
        private ProjectileDetonator _projectileDetonator;
        public ProjectileController projectileController;
        public ProjectileExplosion projectileExplosion;
        [HideInInspector] public bool detonating;
        public void OnEnable()
        {
            Add();
        }
        public void Add()
        {
            if (!projectileDetonator || (projectileDetonator.projectiles.ContainsKey(identificator) && projectileDetonator.projectiles[identificator].Contains(this))) return;
            projectileDetonator.AddProjectile(this);
        }
        public void Remove()
        {
            if (!projectileDetonator || (projectileDetonator.projectiles.ContainsKey(identificator) && !projectileDetonator.projectiles[identificator].Contains(this))) return;
            projectileDetonator.RemoveProjectile(this);
        }
        public void Start()
        {
            Add();
        }
        public void OnDisable()
        {
            Remove();
        }
        public void Detonate()
        {
            detonating = true;
            if (projectileExplosion)
            {
                projectileExplosion.Detonate();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
    public class ProjectileDetonator : NetworkBehaviour
    {
        public Dictionary<string, List<ProjectileRemoteDetonation>> projectiles = new Dictionary<string, List<ProjectileRemoteDetonation>>();
        public void AddProjectile(ProjectileRemoteDetonation projectileRemoteDetonation)
        {
            if (projectiles.ContainsKey(projectileRemoteDetonation.identificator))
            {
                projectiles[projectileRemoteDetonation.identificator].Add(projectileRemoteDetonation);
            }
            else
            {
                projectiles.Add(projectileRemoteDetonation.identificator, new List<ProjectileRemoteDetonation>() { projectileRemoteDetonation });
            }
        }
        public void RemoveProjectile(ProjectileRemoteDetonation projectileRemoteDetonation)
        {
            List<ProjectileRemoteDetonation> projectileRemoteDetonations = projectiles.ContainsKey(projectileRemoteDetonation.identificator) ? projectiles[projectileRemoteDetonation.identificator] : null;
            if (projectileRemoteDetonations != null && projectileRemoteDetonations.Contains(projectileRemoteDetonation)) projectileRemoteDetonations.Remove(projectileRemoteDetonation);
        }
        public void DetonateAll()
        {
            foreach (var projectileIdentificator in projectiles.Keys)
            {
                DetonateAll(projectileIdentificator);
            }
        }
        [Command]
        public void CmdDetonateAll(string identificator)
        {
            ServerDetonateAll(identificator);
        }
        public void ServerDetonateAll(string identificator)
        {
            if (!projectiles.ContainsKey(identificator)) return;
            List<ProjectileRemoteDetonation> projectileRemoteDetonations = projectiles[identificator];
            int nonDetonating = 0;
            do
            {
                nonDetonating = 0;
                for (int i = 0; i < projectileRemoteDetonations.Count; i++)
                {
                    ProjectileRemoteDetonation projectileRemoteDetonation = projectileRemoteDetonations[i];
                    if (!projectileRemoteDetonation) continue;
                    if (projectileRemoteDetonation.detonating) continue;
                    projectileRemoteDetonation.Detonate();
                    nonDetonating++;
                }
            } while (nonDetonating > 0);
        }
        public void DetonateAll(string identificator)
        {
            if (NetworkServer.active)
            {
                ServerDetonateAll(identificator);
            }
            else
            {
                CmdDetonateAll(identificator);
            }
        }
    }
    [RequireComponent(typeof(ProjectileDamage))]
    [RequireComponent(typeof(ProjectileController))]
    public class ProjectileBulletAttack : MonoBehaviour
    {
        public ProjectileController projectileController;
        public ProjectileDamage projectileDamage;
        public Vector3 aimVector = Vector3.up;
        public bool localRotation = false;
        public bool allowTrajectoryAimAssist = false;
        public int bulletCount = 1;
        public bool cheapMultiBullet = false;
        public BulletAttack.FalloffModel falloffModel = BulletAttack.FalloffModel.None;
        public float minSpread = 0f;
        public float maxSpread = 0f;
        public float spreadYawScale = 1f;
        public float spreadPitchScale = 1f;
        public float multiBulletOdds = 0f;
        public float radius = 1f;
        public float maxDistance = 1f;
        public float procCoefficient = 1f;
        public bool sniper = false;
        public bool smartCollision = true;
        public float trajectoryAimAssistMultiplier = 0f;
        public GameObject hitEffectPrefab;
        public GameObject tracerEffectPrefab;
        public LayerMask hitMask = LayerIndex.entityPrecise.mask;
        public LayerMask stopperMask = LayerIndex.noCollision.mask;
        public float fireInterval = 0f;
        [HideInInspector] public float fireStopwatch = 0f;
        public float updateInterval = 1f;
        [HideInInspector] public float updateStopwatch = 0f;
        public float resetInterval = 1f;
        [HideInInspector] public float resetStopwatch = 0f;
        public BulletAttack bulletAttack;
        public virtual void Awake()
        {
            if (projectileDamage == null) projectileDamage = GetComponent<ProjectileDamage>();
            if (projectileController == null) projectileController = GetComponent<ProjectileController>();
            bulletAttack = new BulletAttack
            {
                aimVector = aimVector,
                allowTrajectoryAimAssist = allowTrajectoryAimAssist,
                bulletCount = (uint)bulletCount,
                cheapMultiBullet = cheapMultiBullet,
                damage = projectileDamage.damage,
                damageColorIndex = projectileDamage.damageColorIndex,
                damageType = projectileDamage.damageType,
                falloffModel = falloffModel,
                force = projectileDamage.force,
                isCrit = projectileDamage.crit,
                maxSpread = maxSpread,
                minSpread = minSpread,
                multiBulletOdds = multiBulletOdds,
                origin = transform.position,
                owner = projectileController.owner,
                radius = radius,
                maxDistance = maxDistance,
                procCoefficient = procCoefficient,
                sniper = sniper,
                smartCollision = smartCollision,
                weapon = gameObject,
                trajectoryAimAssistMultiplier = trajectoryAimAssistMultiplier,
                spreadYawScale = spreadYawScale,
                spreadPitchScale = spreadPitchScale,
                hitEffectPrefab = hitEffectPrefab,
                tracerEffectPrefab = tracerEffectPrefab,
                hitMask = hitMask,
                stopperMask = stopperMask,
            };
            bulletAttack.SetIgnoreHitTargets(true);
        }
        public virtual void UpdateBulletAttack()
        {
            bulletAttack.allowTrajectoryAimAssist = allowTrajectoryAimAssist;
            bulletAttack.bulletCount = (uint)bulletCount;
            bulletAttack.cheapMultiBullet = cheapMultiBullet;
            bulletAttack.falloffModel = falloffModel;
            bulletAttack.maxSpread = maxSpread;
            bulletAttack.minSpread = minSpread;
            bulletAttack.multiBulletOdds = multiBulletOdds;
            bulletAttack.sniper = sniper;
            bulletAttack.smartCollision = smartCollision;
            bulletAttack.trajectoryAimAssistMultiplier = trajectoryAimAssistMultiplier;
            bulletAttack.spreadPitchScale = spreadPitchScale;
            bulletAttack.spreadYawScale = spreadYawScale;
            bulletAttack.procCoefficient = procCoefficient;
            bulletAttack.radius = radius;
            bulletAttack.maxDistance = maxDistance;
            bulletAttack.origin = transform.position;
            bulletAttack.damage = projectileDamage.damage;
            bulletAttack.damageColorIndex = projectileDamage.damageColorIndex;
            bulletAttack.damageType = projectileDamage.damageType;
            bulletAttack.force = projectileDamage.force;
            bulletAttack.isCrit = projectileDamage.crit;
            bulletAttack.owner = projectileController.owner;
            if (localRotation)
            {
                bulletAttack.aimVector = transform.rotation * aimVector;
            }
            else
            {
                bulletAttack.aimVector = aimVector;

            }
            updateStopwatch = 0f;
        }
        public virtual void FireBulletAttack()
        {
            if(NetworkServer.active)
            bulletAttack.Fire();
            fireStopwatch = 0f;
        }
        public virtual void Reset()
        {
            bulletAttack.ResetIgnoredHealthComponents();
            resetStopwatch = 0f;
        }
        public virtual void IncrementTimers(float deltaTime)
        {
            resetStopwatch += deltaTime;
            fireStopwatch += deltaTime;
            updateStopwatch += deltaTime;
        }
        public virtual void FixedUpdate()
        {
            IncrementTimers(Time.fixedDeltaTime);
            if (updateStopwatch >= updateInterval) UpdateBulletAttack();
            if (fireStopwatch >= fireInterval) FireBulletAttack();
            if (resetStopwatch >= resetInterval) Reset();
        }
    }
    [RequireComponent(typeof(CharacterController))]
    public class DemolisherSwordPillarProjectile : ProjectileBulletAttack
    {
        public CharacterController characterController;
        public float lifetime = 1f;
        public float gravity = 3f;
        public AnimationCurve velocityOverLifetime = AnimationCurve.Constant(0f, 1f, 100f);
        public float steering = 90f;
        [HideInInspector] public float timer;
        [HideInInspector] public DemolisherBulletAttackWeaponDef meleeWeapon;
        [HideInInspector] public FireTallSword fireTallSword;
        [HideInInspector] public InputBankTest inputBank;
        public override void Awake()
        {
            base.Awake();
            if(characterController == null) characterController = GetComponent<CharacterController>();
        }
        public void Start()
        {
            Vector3 vector3 = transform.forward;
            vector3.y = 0f;
            vector3.Normalize();
            transform.forward = vector3;
            if (projectileController == null || projectileController.owner == null) return;
            CharacterBody characterBody = projectileController.owner.GetComponent<CharacterBody>();
            if(characterBody && characterBody.skillLocator)
            {
                foreach (GenericSkill genericSkill in characterBody.skillLocator.allSkills)
                {
                    fireTallSword = genericSkill == null || genericSkill.stateMachine == null || genericSkill.stateMachine.state == null || !(genericSkill.stateMachine.state is FireTallSword) ? null : genericSkill.stateMachine.state as FireTallSword;
                    if (fireTallSword == null || fireTallSword.stateTaken) continue;
                    meleeWeapon = fireTallSword.currentMeleeWeaponDef;
                    if (meleeWeapon)
                    {
                        fireTallSword.stateTaken = true;
                        inputBank = fireTallSword.inputBank;
                        object attack = bulletAttack;
                        meleeWeapon.OneTimeModification(projectileController, ref attack);
                        break;
                    }
                }
            }
        }
        public override void UpdateBulletAttack()
        {
            base.UpdateBulletAttack();
            object attack = bulletAttack;
            if (meleeWeapon != null && fireTallSword != null)
            meleeWeapon.ModifyAttack(projectileController, ref attack);
        }
        public override void IncrementTimers(float deltaTime)
        {
            base.IncrementTimers(deltaTime);
            timer += deltaTime;
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active)
            {
                float velocity = velocityOverLifetime.Evaluate(timer / lifetime);
                transform.forward = Vector3.RotateTowards(transform.forward, Direction(), steering / 57.3f, 0f);
                Vector3 direction = (transform.forward * velocity) + (Physics.gravity * gravity);
                characterController.Move(direction * Time.fixedDeltaTime);
                if (timer >= lifetime) Destroy(gameObject);
            }
        }
        public Vector3 Direction()
        {
            if (inputBank)
            {
                Vector3 vector3 = inputBank.aimDirection;
                vector3.y = 0f;
                vector3.Normalize();
                return vector3;
                //return NearestPointOnLine(inputBank.aimOrigin, inputBank.aimDirection, transform.position) - transform.position;
            }
            else
            {
                return Vector3.zero;
            }
        }
        public static Vector3 NearestPointOnLine(Vector3 linePnt, Vector3 lineDir, Vector3 pnt)
        {
            lineDir.Normalize();
            var v = pnt - linePnt;
            var d = Vector3.Dot(v, lineDir);
            return linePnt + lineDir * d;
        }
    }
    public class DemolisherSwordPillarGhost : MonoBehaviour
    {
        public Transform swordPillarTransform;
        public ProjectileGhostController projectileGhostController;
        public DemolisherSwordPillarProjectile demolisherSwordPillarProjectile
        {
            get
            {
                if(_demolisherSwordPillarProjectile == null) _demolisherSwordPillarProjectile = projectileGhostController && projectileGhostController.authorityTransform ? projectileGhostController.authorityTransform.gameObject.GetComponent<DemolisherSwordPillarProjectile>() : null;
                return _demolisherSwordPillarProjectile;
            }
        }
        private DemolisherSwordPillarProjectile _demolisherSwordPillarProjectile;
        public void Awake()
        {
            if(swordPillarTransform) swordPillarTransform.SetParent(null, true);
        }
        public void OnEnable()
        {
            if (swordPillarTransform) swordPillarTransform.gameObject.SetActive(true);
        }
        public void LateUpdate()
        {
            if(demolisherSwordPillarProjectile == null || swordPillarTransform == null) return;
            BulletAttack bulletAttack = demolisherSwordPillarProjectile.bulletAttack;
            swordPillarTransform.position = transform.position;
            swordPillarTransform.rotation = Quaternion.LookRotation(bulletAttack.aimVector);
            float radius = bulletAttack.radius;
            swordPillarTransform.localScale = new Vector3(radius, radius, bulletAttack.maxDistance);
        }
        public void OnDisable()
        {
            if(swordPillarTransform) swordPillarTransform.gameObject.SetActive(false);
        }
        public void OnDestroy()
        {
            if(swordPillarTransform) Destroy(swordPillarTransform.gameObject);
        }
    }
    public class DemolisherSlash : MonoBehaviour
    {
        public ParticleSystem[] swipeParticles;
        //public ParticleSystem fireParticle;
        public void Init(float angle, bool flip, float radius, float distance, float duration)
        {
            Vector3 vector3 = transform.localEulerAngles;
            vector3.z = angle;
            transform.localEulerAngles = vector3;
            if (flip)
            {
                vector3 = transform.localScale;
                vector3.x *= -1f;
            }
            //swipeParticle.startSize = radius;
            //fireParticle.startSize = radius;
            //fireParticle.startSpeed = distance;
            //swipeParticle.playbackSpeed = 1f / duration;
            //fireParticle.playbackSpeed = 1f / duration * 2f;
        }
    }
    [RequireComponent(typeof(ProjectileController))]
    [RequireComponent(typeof(ProjectileStickOnImpact))]
    public class DemolisherHook : NetworkBehaviour, IStateSeeker
    {
        public static float hookPower = 4f;
        public static float hookAimPower = 4f;
        public static float hookPower2 = 8f;
        public ProjectileController projectileController;
        public ProjectileStickOnImpact projectileStickOnImpact;
        [HideInInspector] public CharacterBody ownerBody;
        [HideInInspector] public InputBankTest ownerInputBank;
        [HideInInspector] [SyncVar] public bool sticked;
        [HideInInspector] public CharacterBody hitBody;
        [HideInInspector] [SyncVar] public float hitRange;
        [HideInInspector] public EntityStateMachine entityStateMachine;
        private IStateTarget _foundState;
        public IStateSeeker stateSeeker => this;
        public IStateTarget foundState { get => _foundState; set => _foundState = value; }
        public Func<bool> onStateFound => SetEntityStateMachine;

        public void Awake()
        {
            if(projectileController == null) projectileController = gameObject.GetComponent<ProjectileController>();
            if(projectileStickOnImpact == null) projectileStickOnImpact = gameObject.GetComponent<ProjectileStickOnImpact>();
        }
        public void Start()
        {
            ownerBody = projectileController && projectileController.owner ? projectileController.owner.GetComponent<CharacterBody>() : null;
            if (ownerBody)
            {
                ownerInputBank = ownerBody.inputBank;
                if (ownerInputBank && ownerBody.skillLocator)
                {
                    stateSeeker.FindState(ownerBody);
                }
            }
        }
        public bool SetEntityStateMachine()
        {
            entityStateMachine = foundState.entityState.outer;
            return true;
        }
        public void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                if (ownerInputBank && entityStateMachine && entityStateMachine.state != foundState) Destroy(gameObject);
                if (projectileStickOnImpact)
                {
                    bool flag = projectileStickOnImpact.stuckTransform;
                    if (flag != sticked) sticked = flag;
                }
            }
            if (sticked)
            {
                Ray ray = new Ray
                {
                    direction = ownerBody.transform.eulerAngles,
                    origin = ownerBody.corePosition
                };
                if (ownerInputBank)
                {
                    ray = ownerInputBank.GetAimRay();
                }
                if (hitBody)
                {
                    Vector3 vector3 = ((ray.origin + ray.direction * hitRange) - hitBody.corePosition) * hookPower2;
                    HandleMovement(hitBody, vector3);
                }
                else
                {
                    Vector3 vector3 = ((transform.position - ownerBody.corePosition) * hookPower) + (ray.direction * hookAimPower);
                    HandleMovement(ownerBody, vector3);
                }
            }
        }
        public void HandleMovement(CharacterBody characterBody, Vector3 vector3)
        {
            if (!Util.HasEffectiveAuthority(characterBody.networkIdentity)) return;
            if (characterBody.characterMotor)
            {
                characterBody.characterMotor.velocity = vector3;
            }
            else if (characterBody.rigidbody)
            {
                characterBody.rigidbody.velocity = vector3;
            }
        }
        public void OnStick()
        {
            if (projectileStickOnImpact == null) return;
            hitBody = projectileStickOnImpact.stuckBody;
            hitRange = ownerBody ? (transform.position - ownerBody.corePosition).magnitude : 0f;
            sticked = true;
            if (hitBody) RpcOnStick(hitBody.networkIdentity);
        }
        [ClientRpc]
        public void RpcOnStick(NetworkIdentity networkIdentity)
        {
            if (NetworkServer.active) return;
            hitBody = networkIdentity.gameObject.GetComponent<CharacterBody>();
        }
    }
    [RequireComponent(typeof(ProjectileController))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(ProjectileDamage))]
    [RequireComponent(typeof(ProjectileImpactExplosion))]
    public class DemolisherBomb : MonoBehaviour, IProjectileImpactBehavior, IStateSeeker
    {
        public float damageCoefficient = 1f;
        public bool useProejctileDamage = true;
        public float procCoefficient = 1f;
        public float force = 1f;
        public float upForce = 0.5f;
        public float lifetimeMultiplier = 1f;
        public Vector3 velocityOnHit = Vector3.up * 3f;
        public Rigidbody rigidbody;
        public ProjectileController projectileController;
        public ProjectileImpactExplosion projectileImpactExplosion;
        public ProjectileDamage projectileDamage;
        [HideInInspector] public List<CharacterBody> hitBodies = new List<CharacterBody>();
        [HideInInspector] public bool hit;
        [HideInInspector] public CharacterBody ownerBody;
        public Vector3 forceVector
        {
            get
            {
                Vector3 vector3 = rigidbody.velocity;
                vector3.y = 0f;
                return vector3.normalized;
            }
        }
        private IStateTarget _foundState;
        public IStateTarget foundState { get => _foundState; set => _foundState = value; }
        public Func<bool> onStateFound => FindChargeState;

        public bool FindChargeState()
        {
            IStateCharge stateCharge = foundState as IStateCharge;
            if (stateCharge == null)
            {
                return false;
            }
            else
            {
                projectileImpactExplosion.lifetime = (stateCharge.maxCharge - stateCharge.chargePercentage) * lifetimeMultiplier;
                return true;
            }
        }
        public void Awake()
        {
            if (rigidbody == null) rigidbody = GetComponent<Rigidbody>();
            if (projectileController == null) projectileController = GetComponent<ProjectileController>();
            if (projectileImpactExplosion == null) projectileImpactExplosion = GetComponent<ProjectileImpactExplosion>();
            if (projectileDamage == null) projectileDamage = GetComponent<ProjectileDamage>();
        }
        public void Start()
        {
            if (projectileController.owner)
            {
                ownerBody = projectileController.owner.GetComponent<CharacterBody>();
                (this as IStateSeeker).FindState(ownerBody);
            }
        }

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            if(!NetworkServer.active) return;
            if (hit) return;
            hit = true;
            HurtBox hurtBox = impactInfo.collider.GetComponent<HurtBox>();
            CharacterBody characterBody = hurtBox && hurtBox.healthComponent ? hurtBox.healthComponent.body : impactInfo.collider.GetComponent<CharacterBody>();
            if(characterBody == null) return;
            if(!hitBodies.Contains(characterBody)) hitBodies.Add(characterBody);
            characterBody.AddTimedBuff(Assets.BombHit, 1f);
            if (projectileDamage)
            {
                DamageInfo damageInfo = new DamageInfo
                {
                    attacker = projectileController.owner,
                    canRejectForce = true,
                    crit = projectileDamage.crit,
                    damage = useProejctileDamage ? projectileDamage.damage * damageCoefficient : characterBody.damage * damageCoefficient,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageTypeCombo.Generic,
                    //force = 0f,
                    inflictor = gameObject,
                    position = impactInfo.estimatedPointOfImpact,
                    procCoefficient = procCoefficient
                };
                characterBody.healthComponent?.TakeDamageProcess(damageInfo);
            }
            CharacterMotor characterMotor = characterBody.characterMotor;
            if (characterMotor)
            {
                PhysForceInfo physForceInfo = new PhysForceInfo
                {
                    disableAirControlUntilCollision = true,
                    ignoreGroundStick = true,
                    massIsOne = true,
                    force = forceVector * force + Physics.gravity * upForce * -1f
                };
                characterMotor?.ApplyForceImpulse(physForceInfo);
            }
            rigidbody.velocity = velocityOnHit;

            //if (characterBody.characterMotor)
            //{
            //    characterBody.characterMotor.ApplyForce(forceVector * force + Physics.gravity * upForce * -1f, true);
            //}
        }
    }
    public class DemolisherFeetEffect : MonoBehaviour
    {
        public static RocketJumpComponent.OnRocketJumpApplied onRocketJumpApplied = AddFeetSmoke;
        public ParticleSystem particleSystem;
        [HideInInspector] public CharacterMotor characterMotor;
        public void OnLanded(ref CharacterMotor.HitGroundInfo hitGroundInfo)
        {
            //particleSystem.enableEmission = false;
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            transform.SetParent(null, true);
            characterMotor.onHitGroundAuthority -= OnLanded;
        }
        public static void AddFeetSmoke(RocketJumpComponent rocketJumpComponent, CharacterBody characterBody, Vector3 vector3)
        {
            new FeetEffectNetMessage(characterBody.netId).Send(R2API.Networking.NetworkDestination.Clients);
        }
    }
    [RequireComponent(typeof(ProjectileController))]
    public class DemolisherProjectileExplosionSpawner : MonoBehaviour , IOnProjectileExplosionDetonate
    {
        public ProjectileController projectileController;
        public GameObject master;
        public delegate void OnMasterSummon(DemolisherProjectileExplosionSpawner demolisherProjectileExplosionSpawner, CharacterMaster characterMaster);
        public int spawnCount = 1;
        public int spawnTimes = 1;
        public DelegateHolder<OnMasterSummon> onMasterSummon;
        public void OnProjectileExplosionDetonate(BlastAttack blastAttack, BlastAttack.Result result)
        {
            if(spawnTimes > 0)
            {
                Spawn(blastAttack.position);
                spawnTimes--;
            }
        }
        public void Spawn(Vector3 position)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                MasterSummon masterSummon = new MasterSummon
                {
                    ignoreTeamMemberLimit = true,
                    masterPrefab = master,
                    position = position,
                    rotation = Quaternion.identity,
                    summonerBodyObject = projectileController?.owner,
                    useAmbientLevel = true,
                };
                CharacterMaster characterMaster = masterSummon.Perform();
                onMasterSummon?.InvokeAll(this, characterMaster);
            }
        }
    }
    public class DemolisherProjectileHelper : MonoBehaviour
    {
        public Rigidbody rigidbody;
        public TeamFilter teamFilter;
        public float contradictGravity = 0.2f;
        public int teleportAmount = 1;
        public void Start()
        {
            if (rigidbody) rigidbody.velocity += Physics.gravity * -1f * 0.2f;
        }
        public void OnTriggerEnter(Collider collider)
        {
            if (teleportAmount <= 0) return;
            HurtBox hurtBox = collider.GetComponent<HurtBox>();
            if (hurtBox == null) return;
            bool teleport = true;
            if (teamFilter)
            {
                HealthComponent healthComponent = hurtBox.healthComponent;
                if (healthComponent && healthComponent.body && healthComponent.body.teamComponent) teleport = teamFilter.teamIndex != healthComponent.body.teamComponent.teamIndex;
            }
            if (teleport)
            {
                transform.position = collider.ClosestPoint(transform.position);
                teleportAmount--;
            }
        }
    }
    public class DemolisherVoicelinesComponent : NetworkBehaviour
    {
        public static float startTimer = 0.5f;
        public static float endTimer = 1f;
        public List<PendingVoiceline> pendingVoicelines = [];
        public void OnEnable()
        {
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
        }
        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport obj)
        {
            float timer = UnityEngine.Random.Range(startTimer, endTimer);
            if (obj.victim.gameObject == gameObject)
            {
                PlayVoiceline(VoicelineDef.VoicelineType.Death);
                return;
            }
            if (obj.attacker == null || obj.attacker != gameObject) return;
            ProjectileRemoteDetonation projectileRemoteDetonation = obj.damageInfo.inflictor ? obj.damageInfo.inflictor.GetComponent<ProjectileRemoteDetonation>() : null;
            if (projectileRemoteDetonation)
            {
                PlayVoiceline(VoicelineDef.VoicelineType.TrapKill, timer);
            }
            else
            {
                PlayVoiceline(VoicelineDef.VoicelineType.Kill, timer);
            }
        }
        public void FixedUpdate()
        {
            if (NetworkServer.active)
            for (int i = 0; i < pendingVoicelines.Count; i++)
            {
                PendingVoiceline pendingVoiceline = pendingVoicelines[i];
                pendingVoiceline.timer -= Time.fixedDeltaTime;
                if (pendingVoiceline.timer <= 0f)
                {
                    RpcPlayVoiceline(pendingVoiceline.voicelineDef);
                    pendingVoicelines.Remove(pendingVoiceline);
                }
            }
        }
        public void OnDisable()
        {
            GlobalEventManager.onCharacterDeathGlobal -= GlobalEventManager_onCharacterDeathGlobal;
        }
        public void PlayVoiceline(VoicelineDef.VoicelineType voicelineType, float timer = 0f)
        {
            if (!NetworkServer.active) return;
            if (!VoicelineDef.voicelinesByType.ContainsKey(voicelineType)) return;
            List<VoicelineDef> voicelineDefs = VoicelineDef.voicelinesByType[voicelineType];
            if (voicelineDefs == null) return;
            VoicelineDef voicelineDef = voicelineDefs[UnityEngine.Random.Range(0, voicelineDefs.Count)];
            if (voicelineDef == null) return;
            if (timer > 0f)
            {
                PendingVoiceline pendingVoiceline = new()
                {
                    timer = timer,
                    voicelineDef = voicelineDef,
                };
                pendingVoicelines.Add(pendingVoiceline);
            }
            else
            {
                RpcPlayVoiceline(voicelineDef);

            }
        }
        [ClientRpc]
        public void RpcPlayVoiceline(VoicelineDef voicelineDef) => PlayVoiceline(voicelineDef);
        public void PlayVoiceline(VoicelineDef voicelineDef) => voicelineDef.Play(gameObject);
        public class PendingVoiceline
        {
            public VoicelineDef voicelineDef;
            public float timer;
        }
    }
    public class DemolisherGroundSlamEffect : MonoBehaviour
    {
        public ParticleSystem particleSystem;
        public void Start()
        {
            ParticleSystem.ShapeModule shapeModule = particleSystem.shape;
            shapeModule.radius = transform.localScale.x;
        }
    }
    public class DemolisherModel : CharacterModel
    {
        public Material devilMaterial;
        public GameObject[] devilObjects;
        public GameObject[] nonDevilObjects;
        public GameObject swordObject;
        public GameObject gunObject;
        private TemporaryOverlay temporaryOverlay;
        private bool _emoting;
        public bool emoting
        {
            get => _emoting;
            set
            {
                if (emoting == value) return;
                _emoting = value;
                if (value)
                {
                    swordInvisibilityCount++;
                    gunInvisibilityCount++;
                }
                else
                {
                    swordInvisibilityCount--;
                    gunInvisibilityCount--;
                }
            }
        }
        private int _swordInvisibilityCount;
        public int swordInvisibilityCount
        {
            get => _swordInvisibilityCount;
            set
            {
                _swordInvisibilityCount = value;
                if (swordObject)
                if (value > 0)
                {
                    swordObject.SetActive(false);
                }
                else
                {
                    swordObject.SetActive(true);
                }
            }
        }
        private int _gunInvisibilityCount;
        public int gunInvisibilityCount
        {
            get => _gunInvisibilityCount;
            set
            {
                _gunInvisibilityCount = value;
                if (gunObject)
                if (value > 0)
                    {
                    gunObject.SetActive(false);
                }
                else
                {
                    gunObject.SetActive(true);
                }
            }
        }
        private int _devilCount;
        private bool devilCountApplied;
        public int devilCount
        {
            get => _devilCount;
            set
            {
                _devilCount = value;
                if (value > 0)
                {
                    if (devilCountApplied) return;
                    devilCountApplied = true;
                    foreach (GameObject devilObject in devilObjects) devilObject.SetActive(true);
                    foreach (GameObject nonDevilObject in nonDevilObjects) nonDevilObject.SetActive(false);
                    if (devilMaterial)
                    {
                        if (temporaryOverlay) Destroy(temporaryOverlay);
                        temporaryOverlay = gameObject.AddComponent<TemporaryOverlay>();
                        temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
                        temporaryOverlay.inspectorCharacterModel = this;
                        temporaryOverlay.originalMaterial = devilMaterial;
                        temporaryOverlay.AddToCharacerModel(this);
                    }

                }
                else
                {
                    if (!devilCountApplied) return;
                    devilCountApplied = false;
                    foreach (GameObject devilObject in devilObjects) devilObject.SetActive(false);
                    foreach (GameObject nonDevilObject in nonDevilObjects) nonDevilObject.SetActive(true);
                    if (temporaryOverlay) Destroy(temporaryOverlay);
                }
            }
        }
    }
    public class DemolisherModelLocator : ModelLocator
    {
        public int overrideTargetNormalCount;
        public Vector3 overrideTargetNormal;
    }
    public class DemolisherElevatorController : NetworkBehaviour, ICameraStateProvider
    {
        public EntityStateMachine stateMachine { get; private set; }
        public ChildLocator childLocator { get; private set; }
        [Tooltip("The bone which controls the camera during the entry animation.")]
        public Transform cameraBone;
        public bool exitAllowed;
        public EntityStateMachine characterStateMachine { get; private set; }
        public VehicleSeat vehicleSeat { get; set; }
        public ParticleSystem[] chains;
        public void Awake()
        {
            stateMachine = GetComponent<EntityStateMachine>();
            vehicleSeat = GetComponent<VehicleSeat>();
            childLocator = GetComponent<ChildLocator>();
            vehicleSeat.onPassengerEnter += OnPassengerEnter;
            vehicleSeat.onPassengerExit += OnPassengerExit;
            vehicleSeat.enterVehicleAllowedCheck.AddCallback(new CallbackCheck<Interactability, CharacterBody>.CallbackDelegate(CheckEnterAllowed));
            vehicleSeat.exitVehicleAllowedCheck.AddCallback(new CallbackCheck<Interactability, CharacterBody>.CallbackDelegate(CheckExitAllowed));
            foreach (ParticleSystem particleSystem in chains)
            {
                particleSystem.Play();
                particleSystem.Simulate(80f);
                particleSystem.Play();
            }
        }
        public void Start()
        {
            //foreach (ParticleSystem particleSystem in chains)
            //{
            //    ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
            //    if (particleSystem.GetParticles(particles) > 0)
            //    {
            //        for (int i = 0; i < particles.Length; i++)
            //        {
            //            ref ParticleSystem.Particle particle = ref particles[i];
            //            particle.lifetime = 80f;
            //        Debug.Log("true");
            //        }
            //    }
            //    else
            //    {
            //        Debug.Log("False");
            //    }
            //}
        }
        public void OnPassengerEnter(GameObject passenger)
        {
            UpdateCameras(passenger);
        }
        public void OnPassengerExit(GameObject passenger)
        {
            UpdateCameras(null);
            vehicleSeat.enabled = false;
        }
        public void CheckEnterAllowed(CharacterBody characterBody, ref Interactability? resultOverride)
        {
            resultOverride = new Interactability?(Interactability.Disabled);
        }
        public void CheckExitAllowed(CharacterBody characterBody, ref Interactability? resultOverride)
        {
            resultOverride = new Interactability?(exitAllowed ? Interactability.Available : Interactability.Disabled);
        }
        public void Update()
        {
            UpdateCameras(vehicleSeat.currentPassengerBody ? vehicleSeat.currentPassengerBody.gameObject : null);
        }

        public void UpdateCameras(GameObject characterBodyObject)
        {
            foreach (CameraRigController cameraRigController in CameraRigController.readOnlyInstancesList)
            {
                if (characterBodyObject && cameraRigController.target == characterBodyObject)
                {
                    cameraRigController.SetOverrideCam(this, 0f);
                }
                else if (cameraRigController.IsOverrideCam(this))
                {
                    cameraRigController.SetOverrideCam(null, 0.05f);
                }
            }
        }
        public void GetCameraState(CameraRigController cameraRigController, ref CameraState cameraState)
        {
            Vector3 rotation = cameraBone.eulerAngles;
            rotation.y += 180f;
            cameraState = new CameraState
            {
                position = cameraBone.position,
                rotation = Quaternion.Euler(rotation),
                fov = 60f
            };
        }
        public bool IsUserLookAllowed(CameraRigController cameraRigController)
        {
            return false;
        }
        public bool IsUserControlAllowed(CameraRigController cameraRigController)
        {
            return true;
        }
        public bool IsHudAllowed(CameraRigController cameraRigController)
        {
            return true;
        }
    }
    public class DemolisherLobbyController : MonoBehaviour
    {
        public static float phase1EffectScale = 1f;
        public static float phase2EffectScale = 1f;
        public static float phase3EffectScale = 1f;
        public static float phase4EffectScale = 3f;
        public static float pillarEffectScale = 2f;
        public static float uiAlphaDownSmoothTime = 0.25f;
        public static float uiAlphaDownAlpha = 0.45f;
        public static float red = 0.1f;
        public static float redSmoothTime = 0.25f;
        private float uiAlphaDownVelocity;
        private float redVelocity;
        public DemolisherModel demolisherModel;
        public ChildLocator childLocator;
        public Image redAsFuck;
        private bool uiAlphaDown;
        private GameObject pillar;
        public void Phase1(AnimationEvent animationEvent)
        {
            Transform chainR = childLocator.FindChild("ChainsR");
            if (chainR) chainR.gameObject.SetActive(false);
            Transform effect = childLocator.FindChild("Phase1Effect");
            if (effect)
            {
                EffectData effectData = new EffectData
                {
                    origin = effect.position,
                    scale = phase1EffectScale,
                };
                EffectManager.SpawnEffect(Assets.ChainsExplosion.prefab, effectData, false);
            }
        }
        public void Phase2(AnimationEvent animationEvent)
        {
            Transform chainR = childLocator.FindChild("ChainsL");
            if (chainR) chainR.gameObject.SetActive(false);
            Transform effect = childLocator.FindChild("Phase2Effect");
            if (effect)
            {
                EffectData effectData = new EffectData
                {
                    origin = effect.position,
                    scale = phase2EffectScale,
                };
                EffectManager.SpawnEffect(Assets.ChainsExplosion.prefab, effectData, false);
            }
        }
        public void Phase3(AnimationEvent animationEvent)
        {
            Transform chainR = childLocator.FindChild("Chains");
            Transform cross = childLocator.FindChild("Cross");
            if (chainR) chainR.gameObject.SetActive(false);
            if (cross) cross.gameObject.SetActive(false);
            if (demolisherModel) demolisherModel.devilCount++;
            Transform effect = childLocator.FindChild("Phase3Effect");
            if (effect)
            {
                EffectData effectData = new EffectData
                {
                    origin = effect.position,
                    scale = phase3EffectScale,
                };
                EffectManager.SpawnEffect(Assets.ChainsExplosion.prefab, effectData, false);
                pillar = Instantiate(Assets.PillarEffect, effect);
                pillar.transform.localScale = new Vector3(pillarEffectScale, pillarEffectScale, pillarEffectScale);
            }
            uiAlphaDown = true;
            redAsFuck = Instantiate(Assets.IamRedAsFuck).GetComponent<Image>();
            Util.PlaySound("Play_Demoman_mvm_m_autocappedintelligence02", gameObject);
        }
        public void Update()
        {
            if (redAsFuck)
            {
                Color color = redAsFuck.color;
                color.a = Mathf.SmoothDamp(color.a, uiAlphaDown ? red : 0f, ref redVelocity, redSmoothTime, float.MaxValue, Time.deltaTime);
                redAsFuck.color = color;
            }
            foreach (CanvasGroup canvasGroup in Hooks.canvasGroups)
            {
                if (!canvasGroup) continue;
                canvasGroup.alpha = Mathf.SmoothDamp(canvasGroup.alpha, uiAlphaDown ? uiAlphaDownAlpha : 1f, ref uiAlphaDownVelocity, uiAlphaDownSmoothTime, float.MaxValue, Time.deltaTime);
            }
        }
        public void Phase4()
        {
            Transform sword = childLocator.FindChild("Sword");
            Transform launcher = childLocator.FindChild("Launcher");
            Transform weaponL = childLocator.FindChild("WeaponL");
            if (sword) sword.gameObject.SetActive(true);
            if (launcher)
            {
                launcher.gameObject.SetActive(true);
                if (weaponL)
                {
                    launcher.SetParent(weaponL, false);
                    launcher.localEulerAngles = new Vector3(0f, 90f, 0f);
                }
            }
            if (demolisherModel) demolisherModel.devilCount--;
            Transform effect = childLocator.FindChild("Phase3Effect");
            if (effect)
            {
                EffectData effectData = new EffectData
                {
                    origin = effect.position,
                    scale = phase4EffectScale,
                };
                EffectManager.SpawnEffect(Assets.Rings.prefab, effectData, false);
                if (pillar)
                {
                    GameObject pillarExplosion = Instantiate(Assets.PillarExplosionEffect, effect);
                    pillarExplosion.transform.localScale = pillar.transform.localScale;
                    Destroy(pillar);
                }
            }
            uiAlphaDown = false;
            if (redAsFuck)
            {
                DestroyOnTimer destroyOnTimer = redAsFuck.GetComponent<DestroyOnTimer>();
                if (destroyOnTimer) destroyOnTimer.enabled = true;
            }
        }
    }
    public class ObjectScaleCurveEffectScale : MonoBehaviour
    {
        public EffectComponent effectComponent;
        public ObjectScaleCurve objectScaleCurve;
        public void Update()
        {
            if (!effectComponent || effectComponent.effectData == null) return;
            float time = effectComponent.effectData.genericFloat;
            objectScaleCurve.timeMax = time;
        }
    }
    public class ScaleTrailWidthWithLossyScale : MonoBehaviour
    {
        public TrailRenderer trailRenderer;
        public void Update()
        {
            float scale = (transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z) / 3f;
            trailRenderer.widthMultiplier = scale;
        }
    }
    public class DestroyOnLessThanScale : MonoBehaviour
    {
        private void Start()
        {
            if (!this.efh)
            {
                this.efh = base.GetComponent<EffectManagerHelper>();
            }
        }
        private void FixedUpdate()
        {
            float scale = ( lossy ? (transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z) : (transform.localScale.x + transform.localScale.y + transform.localScale.z)) / 3f;
            if (scale <= this.scale)
            {
                if (this.efh && this.efh.OwningPool != null)
                {
                    this.efh.OwningPool.ReturnObject(this.efh);
                    return;
                }
                global::UnityEngine.Object.Destroy(base.gameObject);
            }
        }
        public float scale;
        public bool lossy;
        private EffectManagerHelper efh;
    }
    public abstract class DemolisherWeaponDef : ScriptableObject
    {
        public delegate void ModificationDelegate(object source, ref object attack);
        public ModificationDelegate attackModification;
        public ModificationDelegate oneTimeModification;
        public virtual void OneTimeModification(object source, ref object attack)
        {
            oneTimeModification?.Invoke(source, ref attack);
        }
        public virtual void ModifyAttack(object source, ref object attack)
        {
            attackModification?.Invoke(source, ref attack);
        }
    }
    [CreateAssetMenu(menuName = "Demolisher/DemolisherWeaponDef/DemolisherBulletAttackWeaponDef")]
    public class DemolisherBulletAttackWeaponDef : DemolisherWeaponDef
    {
        public float damageMultiplier = 1f;
        public float procMultiplier = 1f;
        public float forceMultiplier = 1f;
        public float radiusMultiplier = 1f;
        public float distanceMultiplier = 1f;
        public bool customCrit = false;
        public DamageType damageType = DamageType.Generic;
        public DamageTypeExtended damageTypeExtended = DamageTypeExtended.Generic;
        public DamageAPI.ModdedDamageType[] moddedDamageTypes;
        public override void OneTimeModification(object source, ref object attack)
        {
            base.OneTimeModification(source, ref attack);
            BulletAttack bulletAttack = (BulletAttack)attack;
            bulletAttack.damageType.damageType = damageType;
            bulletAttack.damageType.damageTypeExtended = damageTypeExtended;
            if(moddedDamageTypes != null) foreach (DamageAPI.ModdedDamageType moddedDamageType in moddedDamageTypes) bulletAttack.AddModdedDamageType(moddedDamageType);
        }
        public override void ModifyAttack(object source, ref object attack)
        {
            base.ModifyAttack(source, ref attack);
            BulletAttack bulletAttack = (BulletAttack)attack;
            bulletAttack.damage *= damageMultiplier;
            bulletAttack.procCoefficient *= procMultiplier;
            bulletAttack.force *= forceMultiplier;
            bulletAttack.radius *= radiusMultiplier;
            bulletAttack.maxDistance *= distanceMultiplier;
        }
    }
    [CreateAssetMenu(menuName = "Demolisher/DemolisherWeaponDef/DemolisherProjectileWeaponDef")]
    public class DemolisherProjectileWeaponDef : DemolisherWeaponDef
    {
        public GameObject projectile = Assets.GrenadeProjectile;
        public float damageMultiplier = 1f;
        public float forceMultiplier = 1f;
        public float speedMultiplier = 1f;
        public string fireSound;
        public DamageType damageType = DamageType.Generic;
        public DamageTypeExtended damageTypeExtended = DamageTypeExtended.Generic;
        public DamageAPI.ModdedDamageType[] moddedDamageTypes;
        public override void OneTimeModification(object source, ref object attack)
        {
            base.OneTimeModification(source, ref attack);
            FireProjectileInfo fireProjectileInfo = (FireProjectileInfo)attack;
            fireProjectileInfo.projectilePrefab = projectile;
            DamageTypeCombo damageTypeCombo = fireProjectileInfo.damageTypeOverride.Value;
            damageTypeCombo.damageType = damageType;
            damageTypeCombo.damageTypeExtended = damageTypeExtended;
            if (moddedDamageTypes != null) foreach (DamageAPI.ModdedDamageType moddedDamageType in moddedDamageTypes) damageTypeCombo.AddModdedDamageType(moddedDamageType);
        }
        public override void ModifyAttack(object source, ref object attack)
        {
            base.ModifyAttack(source, ref attack);
            FireProjectileInfo fireProjectileInfo = (FireProjectileInfo)attack;
            fireProjectileInfo.damage *= damageMultiplier;
            fireProjectileInfo.speedOverride *= speedMultiplier;
            fireProjectileInfo.force *= forceMultiplier;
        }
    }
    [CreateAssetMenu(menuName = "Demolisher/DemolisherWeaponSkillDef")]
    public class DemolisherWeaponSkillDef : SkillDef
    {
        public DemolisherWeaponDef demolisherWeaponDef;
    }
    [CreateAssetMenu(menuName = "Demolisher/VoicelineDef")]
    public class VoicelineDef : ScriptableObject
    {
        public static List<VoicelineDef> voicelineDefs = [];
        public static Dictionary<VoicelineType, List<VoicelineDef>> voicelinesByType = [];
        public string audioString;
        public VoicelineType voicelineType;
        public int id;
        public void Register()
        {
            if (voicelineDefs.Contains(this)) return;
            voicelineDefs.Add(this);
        }
        public void Play(GameObject gameObject) => Util.PlaySound("Play_" + audioString, gameObject);
        public void Stop(GameObject gameObject) => Util.PlaySound("Stop_" + audioString, gameObject);
        [Flags]
        public enum VoicelineType
        {
            None = 0,
            Death = 1,
            Kill = 2,
            TrapKill = 4,
            Laugh = 8
        }
    }
}
