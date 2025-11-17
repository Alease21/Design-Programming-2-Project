using System;
using UnityEngine;

public enum WeaponTypes
{
    Sword,
    Mace,
    Dagger
}
public enum ArmorTypes
{
    Cloth,
    Leather,
    Plate
}

public interface IEquipment
{
    public string GetName { get; }

    abstract void SpawnEquipmentPrefab(Transform tranform, Vector3 pos);
}
