using static Demolisher.Keywords;

namespace Demolisher
{
    public static class Language
    {
        public static void Init()
        {
            InitCharacter();
            InitAnchievements();
            InitSharpness();
            InitSoftness();
            InitChaos();
            InitMediumMelee();
            InitShieldBash();
            InitChainDash();
            InitBoots();
            InitGrenadeLauncher();
            InitBombLauncher();
            InitHookLauncher();
            InitStickyLauncher();
            InitDemolisherLauncher();
            InitSwordPillar();
            InitParry();
            InitDetonate();
            InitWhirlwind();
            InitSlicing();
            InitCollapse();
            InitLaser();
            InitFly();
        }
        public static void InitCharacter()
        {
            AddLanguageToken(Assets.DemolisherCharacterBody.baseNameToken, "Demolisher");
            AddLanguageToken(Assets.DemolisherCharacterBody.baseNameToken, "Разрывник", "ru");
            AddLanguageToken(Assets.DemolisherCharacterBody.subtitleNameToken, "Escape of Hell");
            AddLanguageToken(Assets.DemolisherCharacterBody.subtitleNameToken, "Сбежавший с Ада", "ru");
            AddLanguageToken(Assets.Demolisher.displayNameToken, "Demolisher");
            AddLanguageToken(Assets.Demolisher.displayNameToken, "Разрывник", "ru");
            AddLanguageToken(Assets.Demolisher.descriptionToken, "Demolisher is a powerfull character that can switch between melee and ranged styles at any moment by pressing sprint button.\r\n\r\nPassive allows Demolisher for harmless landing and quick descent. His explosives have knockback that can be used as a quick position relocation.\r\n\r\nSword attacks has a small radius of attack, so you must aim at the target you want to hit, however they compensates it with their high burst damage and attack speed.\r\n\r\nIn ranged style swords are replaced with sticky trap launcher. On impact they stick to the surface and wait for detonation signal.\r\n\r\nSecondaries give quick high damage bursts for both styles.\r\n\r\nCharge can be used to push enemies away and quickly escape from any situation.\r\n\r\nIn ranged style charge is replaced with trap detonation.\r\n\r\nSpecials for both styles offer a massive damage output, however they have long recharge cooldown.");
            AddLanguageToken(Assets.Demolisher.descriptionToken, "Разрывник довольно сильный персонаж который может переключаться между ближним и дальним стилем боя в любой момент нажимаяя на кнопку бега.\r\n\r\nПассивный скилл позволяет Разрывнику быстро спускаться на землю и не получать урон от падения. Его взрывчатка хорошо отталкивает пользователя и его союзников, позволяя им быстро перемещаться.\r\n\r\nАтаки мечом имеет малый радиус атаки, так что метко цельтесь на врагов которые вы хотите повредить, однако они компенсируют своим высоким уроном и скоростью атаки.\r\n\r\nВ дальнем стиле мечи меняются на гранатомет липких ловушек. При столкновении они прилипают к поверхности и ждут сигнал детонации.\r\n\r\nВторостепенные умения дают быстрый способ нанести высокий урон для обоих стилей.\r\n\r\nРывок может быть использован чтобы толкать врагов и выбираться из трудной ситуации.\r\n\r\nВ дальнем стилее рывок заменяется на детонацию ловушек.\r\n\r\nОсобые умения для обоих стилей предлагают массивный урона, однако занимают долгое время для перезарядки.", "ru");
            AddLanguageToken(Assets.Demolisher.mainEndingEscapeFailureFlavorToken, "...and so he left, escaping eternal torment");
            AddLanguageToken(Assets.Demolisher.mainEndingEscapeFailureFlavorToken, "...и он ушел, ушел от бесконечных мучений", "ru");
            AddLanguageToken(Assets.Demolisher.outroFlavorToken, "...and so he vanished, leaving nothing behind");
            AddLanguageToken(Assets.Demolisher.outroFlavorToken, "...и он исчез, оставив ничего позади", "ru");
            AddLanguageToken("DEMOLISHER_SKILL_WEAPON", "Weapon");
            AddLanguageToken("DEMOLISHER_SKILL_WEAPON", "Оружие", "ru");
            AddLanguageToken("DEMOLISHER_SKIN_DEMON", "Hell Slave");
            AddLanguageToken("DEMOLISHER_SKIN_DEMON", "Раб Ада", "ru");
            AddLanguageToken("DEMOLISHER_SKIN_DRILLER", "Driller");
            AddLanguageToken("DEMOLISHER_SKIN_DRILLER", "Бурильщик", "ru");
            AddLanguageToken("DEMOLISHER_SKIN_NUCLEAR", "Metal Gear");
            AddLanguageToken("DEMOLISHER_SKIN_NUCLEAR", "Метал Гир", "ru");
            AddLanguageToken("DEMOLISHER_SKIN_ALTFEM", "Taurus");
            AddLanguageToken("DEMOLISHER_SKIN_ALTFEM", "Телец", "ru");
            AddLanguageToken("DEMOLISHER_SKIN_FEM", "Fem Demolisher");
            AddLanguageToken("DEMOLISHER_SKIN_FEM", "Фем Рарывник", "ru");
        }
        public static void InitAnchievements()
        {
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERSPEEDRUN_NAME", "Demolisher: From A to D skipping B and C");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERSPEEDRUN_NAME", "Разрывник: От А до Г пропуская Б и В", "ru");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERSPEEDRUN_DESCRIPTION", $"As Demolisher, touch teleporter under {Hooks.stagetRequieredTime} seconds after stage enter.");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERSPEEDRUN_DESCRIPTION", $"Играя за Разрывника, наступите на телепорт менее чем за {Hooks.stagetRequieredTime} секунд после телепортации на этап.", "ru");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERWORLDKILL_NAME", "Demolisher: He Who Celt It");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERWORLDKILL_NAME", "Разрывник: Тот, который надеялся", "ru");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERWORLDKILL_DESCRIPTION", $"As Demolisher, kill a boss by dropping it out of stage.");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERWORLDKILL_DESCRIPTION", $"Играя за Разрывника, убейте босса сбросив в яму.", "ru");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERFLYINGENEMYKILL_NAME", "Demolisher: Flight Control");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERFLYINGENEMYKILL_NAME", "Разрывник: Контроль Полета", "ru");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERFLYINGENEMYKILL_DESCRIPTION", $"As Demolisher, kill {DemolisherFlyingEnemyKillAchievement.count} flying enemies with your melee weapon midair.");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERFLYINGENEMYKILL_DESCRIPTION", $"Играя за Разрывника, убейте {DemolisherFlyingEnemyKillAchievement.count} летающих врагов умениями ближнего боя в воздухе.", "ru");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERRANGEDFARKILL_NAME", "Demolisher: Diamond Eye");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERRANGEDFARKILL_NAME", "Разрывник: Глаз Алмаз", "ru");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERRANGEDFARKILL_DESCRIPTION", $"As Demolisher, kill an enemy with a trap from {DemolisherRangedFarKillAchievement.requiredDistance} meters or higher.");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERRANGEDFARKILL_DESCRIPTION", $"Играя за Разрывника, убей врага ловушкой за {DemolisherRangedFarKillAchievement.requiredDistance} метров или дальше.", "ru");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERCHARGEDISTANCE_NAME", "Demolisher: Running to Narnia");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERCHARGEDISTANCE_NAME", "Разрывник: Побег в Нарнию", "ru");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERCHARGEDISTANCE_DESCRIPTION", $"As Demolisher, achieve the speed of {DemolisherChargeDistanceAchievement.requiredDistance} meters per second or higher in Charge state.");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERCHARGEDISTANCE_DESCRIPTION", $"Играя за Разрывника, достичте скорости {DemolisherChargeDistanceAchievement.requiredDistance} метров в секунду или выше во время Рывка.", "ru");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERMASTERY_NAME", "Demolisher: Mastery");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERMASTERY_NAME", "Разрывник: Мастерство", "ru");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERMASTERY_DESCRIPTION", "As Demolisher, beat the game or obliterate on Monsoon or higher.");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERMASTERY_DESCRIPTION", "Играя за Разрывника, пройдите игру или уничтожьтесь на уровне сложности «Сезон дождей» или выше.", "ru");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERFUELARRAYCELLWIN_NAME", "Demolisher: Grand Mastery");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERFUELARRAYCELLWIN_NAME", "Разрывник: Великое Мастерство", "ru");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERFUELARRAYCELLWIN_DESCRIPTION", "As Demolisher, beat the game on main ending on Monsoon or higher with Fuel Array Cell equiped.");
            AddLanguageToken("ACHIEVEMENT_DEMOLISHERFUELARRAYCELLWIN_DESCRIPTION", "Играя за Разрывника, пройдите игру на основную концовку на уровне сложности «Сезон дождей» или выше имея Топливный Элемент.", "ru");
        }
        public static void InitSharpness()
        {
            AddLanguageToken(Assets.Sharpness.skillNameToken, SharpnessName);
            AddLanguageToken(Assets.Sharpness.skillNameToken, SharpnessNameRu, "ru");
            AddLanguageToken(Assets.Sharpness.skillDescriptionToken, $"First melee hit deals {damagePrefix}{Hooks.SharpnessDamageMultiplier * 100f}% more damage{endPrefix}. Each melee hit increases melee attack crit chance by {damagePrefix}{Hooks.SharpnessCritAddition}%{endPrefix} that resets on crit.");
            AddLanguageToken(Assets.Sharpness.skillDescriptionToken, $"Первый удар мечом наносит {damagePrefix}{Hooks.SharpnessDamageMultiplier * 100f}% больше урона{endPrefix}. Каждый удар мечом повышает шанс критического урона меча на {damagePrefix}{Hooks.SharpnessCritAddition}%{endPrefix} который сбрасывается при критическом ударе.", "ru");
        }
        public static void InitSoftness()
        {
            AddLanguageToken(Assets.Softness.skillNameToken, SoftnessName);
            AddLanguageToken(Assets.Softness.skillNameToken, SoftnessNameRu, "ru");
            AddLanguageToken(Assets.Softness.skillDescriptionToken, $"Each melee hit heals you for {healingPrefix}{Hooks.SoftnessHealOnHitPercentage}% max health{endPrefix}.\nEach melee kill heals you for {healingPrefix}{Hooks.SoftnessHealOnKillPercentage}% max health{endPrefix}.");
            AddLanguageToken(Assets.Softness.skillDescriptionToken, $"Каждый удар мечом исцеляет на {healingPrefix}{Hooks.SoftnessHealOnHitPercentage}% максимального здоровья{endPrefix}.\nКаждое убийство мечом исцеляет на {healingPrefix}{Hooks.SoftnessHealOnKillPercentage}% максимального здоровья{endPrefix}.", "ru");
        }
        public static void InitChaos()
        {
            AddLanguageToken(Assets.Chaos.skillNameToken, ChaosName);
            AddLanguageToken(Assets.Chaos.skillNameToken, ChaosNameRu, "ru");
            AddLanguageToken(Assets.Chaos.skillDescriptionToken, $"On melee hit create an explosion that deals {damagePrefix}{Hooks.ChaosDamageCoefficient * 100f}% base damage{endPrefix}. Reharges after {Hooks.ChaosCooldown} seconds.");
        }
        public static void InitMediumMelee()
        {
            AddLanguageToken(Assets.MediumMelee.skillNameToken, MediumMeleeAttackName);
            AddLanguageToken(Assets.MediumMelee.skillNameToken, MediumMeleeAttackNameRu, "ru");
            AddLanguageToken(Assets.MediumMelee.skillDescriptionToken, $"{damagePrefix}Melee{endPrefix}. Swing in the direction you are looking for {damagePrefix}{MediumMeleeAttack.damageCoefficient * 100f}% base damage{endPrefix}");
            AddLanguageToken(Assets.MediumMelee.skillDescriptionToken, $"{damagePrefix}Меч{endPrefix}. Рубящий удар в направлении взгляда, наносящий {damagePrefix}{MediumMeleeAttack.damageCoefficient * 100f}% урона{endPrefix}", "ru");
        }
        public static void InitShieldBash()
        {
            AddLanguageToken(Assets.ShieldBash.skillNameToken, ShieldChargeName);
            AddLanguageToken(Assets.ShieldBash.skillNameToken, ShieldChargeNameRu, "ru");
            AddLanguageToken(Assets.ShieldBash.skillDescriptionToken, $"Charge forward for {utilityPrefix}{ShieldCharge.baseDuration} seconds{endPrefix}, bashing though enemies dealing {damagePrefix}{ShieldCharge.shieldBashDamageCoefficient * 100f}% base damage{endPrefix}.");
            AddLanguageToken(Assets.ShieldBash.skillDescriptionToken, $"Рывнись вперед на {utilityPrefix}{ShieldCharge.baseDuration} секунд{endPrefix}, толкая врагов на {damagePrefix}{ShieldCharge.shieldBashDamageCoefficient * 100f}% урона{endPrefix}.", "ru");
        }
        public static void InitChainDash()
        {
            AddLanguageToken(Assets.ChainDash.skillNameToken, ChainDashName);
            AddLanguageToken(Assets.ChainDash.skillNameToken, ChainDashNameRu, "ru");
            AddLanguageToken(Assets.ChainDash.skillDescriptionToken, $"Make a quick dash. Press skill button between {utilityPrefix}{ChainDash.baseStartWindow} and {ChainDash.baseEndWindow} seconds{endPrefix} to {utilityPrefix}chain dash{endPrefix}. Succesfull chain dash will {utilityPrefix}reset current melee attack{endPrefix}");
            AddLanguageToken(Assets.ChainDash.skillDescriptionToken, $"Соверши быстрый уклон. Нажмите кнопку умения между {utilityPrefix}{ChainDash.baseStartWindow} и {ChainDash.baseEndWindow} секунд{endPrefix} чтобы сделать {utilityPrefix}цепной уклон{endPrefix}. Успешный цепной уклон {utilityPrefix}сбрасывает текущую атаку мечем{endPrefix}", "ru");
        }
        public static void InitBoots()
        {
            AddLanguageToken(Assets.Boots.skillNameToken, BootsName);
            AddLanguageToken(Assets.Boots.skillNameToken, BootsNameRu, "ru");
            AddLanguageToken(Assets.Boots.skillDescriptionToken, $"{damagePrefix}Heavy{endPrefix}. Negates fall damage. Landing with enough velocity will create an explosion, dealing {damagePrefix}{Hooks.stompBaseDamageCoefficient * 100f}% base damage{endPrefix}. Hold jump button while midair to {utilityPrefix}pull yourself down{endPrefix}.");
            AddLanguageToken(Assets.Boots.skillDescriptionToken, $"{damagePrefix}Пробивание{endPrefix}. Предотвращает урон от падения. Приземление с достаточной скоростью вызовет взрыв, наносящий {damagePrefix}{Hooks.stompBaseDamageCoefficient * 100f}% урона{endPrefix}. Держите кнопку прыжка чтобы {utilityPrefix}падать быстрее{endPrefix}.", "ru");
        }
        public static void InitGrenadeLauncher()
        {
            AddLanguageToken(Assets.GrenadeLauncher.skillNameToken, "Impact Grenade");
            AddLanguageToken(Assets.GrenadeLauncher.skillNameToken, "Ударная Граната", "ru");
            AddLanguageToken(Assets.GrenadeLauncher.skillDescriptionToken, $"Fire grenade that explodes on impact for {damagePrefix}{FireGrenadeConfig.damageCoefficient.Value * (Assets.GrenadeLauncher.demolisherWeaponDef as DemolisherProjectileWeaponDef).damageMultiplier * 100f}% base damage{endPrefix}.");
            AddLanguageToken(Assets.GrenadeLauncher.skillDescriptionToken, $"Выстрелите гранатой которая взрывается при столкновении наносящая {damagePrefix}{FireGrenadeConfig.damageCoefficient.Value * (Assets.GrenadeLauncher.demolisherWeaponDef as DemolisherProjectileWeaponDef).damageMultiplier * 100f}% урона{endPrefix}.", "ru");
        }
        public static void InitBombLauncher()
        {
            AddLanguageToken(Assets.BombLauncher.skillNameToken, "Heavy Bomb");
            AddLanguageToken(Assets.BombLauncher.skillNameToken, "Тяжелая Бомба", "ru");
            AddLanguageToken(Assets.BombLauncher.skillDescriptionToken, $"Fire bomb that damages on collision and explodes after time for {damagePrefix}{FireGrenadeConfig.damageCoefficient.Value * (Assets.BombLauncher.demolisherWeaponDef as DemolisherProjectileWeaponDef).damageMultiplier * 100f}% base damage{endPrefix}. Hold down skill button to {utilityPrefix}reduce detonation time{endPrefix}.");
            AddLanguageToken(Assets.BombLauncher.skillDescriptionToken, $"Выстрелите тяжелой бомбой которая наносит урон при столкновении и взрывается через время на {damagePrefix}{FireGrenadeConfig.damageCoefficient.Value * (Assets.BombLauncher.demolisherWeaponDef as DemolisherProjectileWeaponDef).damageMultiplier * 100f}% урона{endPrefix}. Держите кнопку умения чтобы {utilityPrefix}сократить время детонации{endPrefix}.", "ru");
        }
        public static void InitHookLauncher()
        {
            AddLanguageToken(Assets.HookLauncher.skillNameToken, "Grappling Hook");
            AddLanguageToken(Assets.HookLauncher.skillNameToken, "Крюк Кошка", "ru");
            AddLanguageToken(Assets.HookLauncher.skillDescriptionToken, $"Fire hook that {damagePrefix}moves hit enemies{endPrefix} or {utilityPrefix}pulls user{endPrefix} on terrain hit.");
            AddLanguageToken(Assets.HookLauncher.skillDescriptionToken, $"Выстрелите крюком который {damagePrefix}передвигает врагов{endPrefix} или {utilityPrefix}тегает пользователя{endPrefix} при столкновении с землей.", "ru");

        }
        public static void InitStickyLauncher()
        {
            AddLanguageToken(Assets.StickyLauncher.skillNameToken, "Sticky Trap");
            AddLanguageToken(Assets.StickyLauncher.skillNameToken, "Липкая Ловушка", "ru");
            AddLanguageToken(Assets.StickyLauncher.skillDescriptionToken, $"Fire sticky trap that sticks to enemies and surface and explodes on remote detonation for {damagePrefix}{FireGrenadeConfig.damageCoefficient.Value * (Assets.StickyLauncher.demolisherWeaponDef as DemolisherProjectileWeaponDef).damageMultiplier * 100f}% base damage{endPrefix}.");
            AddLanguageToken(Assets.StickyLauncher.skillDescriptionToken, $"Выстрелите липкой ловушкой которая прилипает к врагам и земле и взрывается при ручной детонации наносящая {damagePrefix}{FireGrenadeConfig.damageCoefficient.Value * (Assets.StickyLauncher.demolisherWeaponDef as DemolisherProjectileWeaponDef).damageMultiplier * 100f}% урона{endPrefix}.", "ru");
        }
        public static void InitDemolisherLauncher()
        {
            AddLanguageToken(Assets.DemolisherLauncher.skillDescriptionToken, $"Fire Demolisher that explodes on impact for {damagePrefix}{FireGrenadeConfig.damageCoefficient.Value * (Assets.DemolisherLauncher.demolisherWeaponDef as DemolisherProjectileWeaponDef).damageMultiplier * 100f}% base damage{endPrefix}.");
        }
        public static void InitSwordPillar()
        {
            AddLanguageToken(Assets.SwordPillar.skillNameToken, FireTallSwordName);
            AddLanguageToken(Assets.SwordPillar.skillNameToken, FireTallSwordNameRu, "ru");
            AddLanguageToken(Assets.SwordPillar.skillDescriptionToken, $"{damagePrefix}Melee{endPrefix}. Fire tall projection of your sword that slices through enemies, impaling them for {damagePrefix}{FireTallSword.damageCoefficient * 100f}% base damage{endPrefix} and comes back.");
            AddLanguageToken(Assets.SwordPillar.skillDescriptionToken, $"{damagePrefix}Меч{endPrefix}. Выстрелите высокой прекцией вашего мечя который прорубает сквозь врагов, {damagePrefix}{FireTallSword.damageCoefficient * 100f}% base damage{endPrefix} и возвращается.", "ru");
        }
        public static void InitParry()
        {
            AddLanguageToken(Assets.Parry.skillNameToken, ParryName);
            AddLanguageToken(Assets.Parry.skillNameToken, ParryNameRu, "ru");
            AddLanguageToken(Assets.Parry.skillDescriptionToken, $"Parry an incoming attack and create and explosion that deals {damagePrefix}{Parry.damageCoefficient * 100f}% base damage{endPrefix} on succesfull parry.");
        }
        public static void InitDetonate()
        {
            AddLanguageToken(Assets.Detonate.skillNameToken, "Remote Detonation");
            AddLanguageToken(Assets.Detonate.skillNameToken, "Ручная Детонация", "ru");
            AddLanguageToken(Assets.Detonate.skillDescriptionToken, $"Detonates all placed traps.");
            AddLanguageToken(Assets.Detonate.skillDescriptionToken, $"Детонирует все размещенные ловушки.", "ru");
        }
        public static void InitWhirlwind()
        {
            AddLanguageToken(Assets.Whirlwind.skillNameToken, WhirlwindMeleeName);
            AddLanguageToken(Assets.Whirlwind.skillNameToken, WhirlwindMeleeNameRu, "ru");
            AddLanguageToken(Assets.Whirlwind.skillDescriptionToken, $"{damagePrefix}Melee{endPrefix}. Hold to spin for {damagePrefix}{WhirlwindMelee.damageCoefficient * WhirlwindMelee.baseRotationsPerSecond * 100f}% base damage{endPrefix} per second.");
            AddLanguageToken(Assets.Whirlwind.skillDescriptionToken, $"{damagePrefix}Меч{endPrefix}. Зажимайте чтобы вертеться нанося {damagePrefix}{WhirlwindMelee.damageCoefficient * WhirlwindMelee.baseRotationsPerSecond * 100f}% урона{endPrefix} в секунду.", "ru");
        }
        public static void InitSlicing()
        {
            AddLanguageToken(Assets.Slicing.skillNameToken, SlicingName);
            AddLanguageToken(Assets.Slicing.skillNameToken, SlicingNameRu, "ru");
            AddLanguageToken(Assets.Slicing.skillDescriptionToken, $"{damagePrefix}Melee{endPrefix}. Stop time and enter slicing flow. Press primary attack to slice through enemies for {damagePrefix}{Slice.damageCoefficient * 100f}% base damage{endPrefix}. Press skill button to exit the flow.");
            AddLanguageToken(Assets.Slicing.skillDescriptionToken, $"{damagePrefix}Меч{endPrefix}. Остановите время и войдите в режущий поток. Нажмите кнопку основной атаки чтобы резать сквозь врагов {damagePrefix}{Slice.damageCoefficient * 100f}% урона{endPrefix}. Нажмите кнопку умения чтобы выйти из потока", "ru");
        }
        public static void InitCollapse()
        {
            AddLanguageToken(Assets.Collapse.skillNameToken, CollapseName);
            AddLanguageToken(Assets.Collapse.skillNameToken, CollapseNameRu, "ru");
            AddLanguageToken(Assets.Collapse.skillDescriptionToken, $"Fire beam of collapse that explodes for {damagePrefix}{FireCollapse.explosionDamageCoefficient * 100f}% base damage{endPrefix}.");
            AddLanguageToken(Assets.Collapse.skillDescriptionToken, $"Выстрелите луч коллапса который взрывается нанося {damagePrefix}{FireCollapse.explosionDamageCoefficient * 100f}% урона{endPrefix}.", "ru");
        }
        public static void InitLaser()
        {
            AddLanguageToken(Assets.Laser.skillNameToken, LaserName);
            AddLanguageToken(Assets.Laser.skillNameToken, LaserNameRu, "ru");
            AddLanguageToken(Assets.Laser.skillDescriptionToken, $"Hold to fire continuous beam of pressure for {damagePrefix}{Laser.damageCoefficient * (1f / Laser.hitInterval) * 100f}% base damage{endPrefix} per second.");
            AddLanguageToken(Assets.Laser.skillDescriptionToken, $"Зажимайте чтобы стрелять непрерывным лучом давления наносящий {damagePrefix}{Laser.damageCoefficient * (1f / Laser.hitInterval) * 100f}% урона{endPrefix} в секунду.", "ru");
        }
        public static void InitFly()
        {
            AddLanguageToken(Assets.Fly.skillNameToken, FlyName);
            AddLanguageToken(Assets.Fly.skillNameToken, FlyName, "ru");
            AddLanguageToken(Assets.Fly.skillDescriptionToken, $"{damagePrefix}Heavy{endPrefix}. Turn into a missile, dealing {damagePrefix}{Fly.stompBaseDamageCoefficient * 100f}% base damage{endPrefix} on impact.");
            AddLanguageToken(Assets.Fly.skillDescriptionToken, $"{damagePrefix}Пробивание{endPrefix}. Превратитесь в ракету, нанося {damagePrefix}{Fly.stompBaseDamageCoefficient * 100f}% урона{endPrefix} при столкновении.", "ru");
        }
        public static void AddLanguageToken(string token, string text) => AddLanguageToken(token, text, "en");
        public static void AddLanguageToken(string token, string text, string lang)
        {
            RoR2.Language language = RoR2.Language.languagesByName[lang];
            if (language == null) return;
            if (language.stringsByToken.ContainsKey(token))
            {
                language.stringsByToken[token] = text;
            }
            else
            {
                language.stringsByToken.Add(token, text);
            }
        }
        public const string damagePrefix = "<style=cIsDamage>";
        public const string keywordPrefix = "<style=cKeywordName>";
        public const string subPrefix = "<style=cSub>";
        public const string stackPrefix = "<style=cStack>";
        public const string utilityPrefix = "<style=cIsUtility>";
        public const string healingPrefix = "<style=cIsHealing>";
        public const string endPrefix = "</style>";
    }
    
}
