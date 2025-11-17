using System;
using UnityEngine;

[Flags]
public enum AllowedWeaponTypes
{
    None = 0,
    Sword = 2,
    Mace = 4,
    Dagger = 8,
}
[Flags]
public enum AllowedArmorTypes
{
    None = 0,
    Cloth = 2,
    Leather = 4, 
    Plate = 8
}
[Flags]
public enum AllowedMagicTypes
{
    None = 0,
    Ice = 2,
    Fire = 4,
    Holy = 8,
    Unholy = 16
}

[CreateAssetMenu(fileName = "CustomClass", menuName = "Class System/New Custom Class")]
public class CharacterClassSOBase : ScriptableObject
{
    [Header("Class Info")]
    public string className;
    [TextArea(3,6)]
    public string classDescription;

    [Header("Class Restrictions")]
    [SerializeField] private AllowedWeaponTypes _allowedWeaponTypes;
    [SerializeField] private AllowedArmorTypes _allowedArmorTypes;
    [SerializeField] private AllowedMagicTypes _allowedMagicTypes;
    
    [Header("Stats")]
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _maxMana;
    [Space(5)]
    [Range(1, 20)] [SerializeField] private int _defaultStam = 10;
    [Range(1, 20)] [SerializeField] private int _defaultStr = 10;
    [Range(1, 20)] [SerializeField] private int _defaultDex = 10;
    [Range(1, 20)] [SerializeField] private int _defaultInt = 10;
    [Space(10)]
    [Range(1, 5)][SerializeField] private int _stamIncreasePerLevel;
    [Range(1, 5)][SerializeField] private int _strIncreasePerLevel;
    [Range(1, 5)][SerializeField] private int _dexIncreasePerLevel;
    [Range(1, 5)][SerializeField] private int _intIncreasePerLevel;

    [Header("Experience")]
    [SerializeField] private int _startingLevel;
    [SerializeField] private int _initExpToLevel = 100;
    [SerializeField] private float _expModifierOnLevelIncrease = 1.25f;

    public AllowedWeaponTypes GetAllowedWeapons => _allowedWeaponTypes;
    public AllowedArmorTypes GetAllowedArmor => _allowedArmorTypes;
    public AllowedMagicTypes GetAllowedMagic => _allowedMagicTypes;
    public int GetMaxHealth => _maxHealth;
    public int GetMaxMana => _maxMana;
    public int GetDefaultStam => _defaultStam;
    public int GetDefaultStr => _defaultStr;
    public int GetDefaultDex => _defaultDex;
    public int GetDefaulInt => _defaultInt;
    public int GetStamIncPerLevel => _stamIncreasePerLevel;
    public int GetStrIncPerLevel => _strIncreasePerLevel;
    public int GetDexIncPerLevel => _dexIncreasePerLevel;
    public int GetIntIncPerLevel => _intIncreasePerLevel;
    public int GetStartingLevel => _startingLevel;
    public int GetInitExpToLevel => _initExpToLevel;
    public float GetExpLevelModifier => _expModifierOnLevelIncrease;
}