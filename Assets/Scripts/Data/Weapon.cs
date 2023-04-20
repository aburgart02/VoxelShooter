using UnityEngine;

namespace Data
{
    public class Weapon
    {
        public readonly bool IsAutomatic;
        public readonly float TimeBetweenShooting;
        public readonly float BaseRecoil;
        public readonly float StepRecoil;
        public readonly float Range;
        public readonly float ReloadTime;
        public readonly int MagazineSize;
        public int TotalBullets;
        public float RecoilModifier;
        public readonly float ResetTimeRecoil;
        public int BulletsInMagazine;
        public bool IsReady;
        public readonly GameObject Prefab;
        public readonly int BulletsPerShot;
        public readonly int Damage;
        public readonly Sprite Icon;
        public readonly int ID;
        public bool IsReloading;
        public readonly float HeadMultiplier;
        public readonly float ChestMultiplier;
        public readonly float LegMultiplier;
        public readonly float ArmMultiplier;
        public AudioClip ShootingAudioClip;
        public float ShootingVolume;
        public AudioClip ReloadingAudioClip;
        public float ReloadingVolume;

        public Weapon(WeaponItem primaryWeapon)
        {
            Damage = primaryWeapon.damage;
            HeadMultiplier = primaryWeapon.headMultiplier;
            ChestMultiplier = primaryWeapon.chestMultiplier;
            LegMultiplier = primaryWeapon.legMultiplier;
            ArmMultiplier = primaryWeapon.armMultiplier;
            ReloadTime = primaryWeapon.reloadTime;
            BaseRecoil = primaryWeapon.baseRecoil;
            StepRecoil = primaryWeapon.stepRecoil;
            BulletsInMagazine = primaryWeapon.magazineSize;
            MagazineSize = primaryWeapon.magazineSize;
            IsReady = true;
            TotalBullets = primaryWeapon.totalBullets;
            BulletsPerShot = primaryWeapon.bulletsPerTap;
            TimeBetweenShooting = primaryWeapon.timeBetweenShooting;
            ResetTimeRecoil = primaryWeapon.resetTimeRecoil;
            IsAutomatic = primaryWeapon.isAutomatic;
            Damage = primaryWeapon.damage;
            Prefab = primaryWeapon.prefab;
            Icon = primaryWeapon.inventoryIcon;
            ID = primaryWeapon.id;
            Range = primaryWeapon.range;
            ShootingAudioClip = primaryWeapon.shootingAudioClip;
            ShootingVolume = primaryWeapon.shootingVolume;
            ReloadingAudioClip = primaryWeapon.reloadingAudioClip;
            ReloadingVolume = primaryWeapon.reloadingVolume;
        }
    }
}