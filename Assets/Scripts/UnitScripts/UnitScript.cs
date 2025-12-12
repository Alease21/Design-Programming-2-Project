using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using static DamageTypeEnum;

[Flags]
public enum UnitStats
{
    None = 0,
    Stamina = 2,
    Strength = 4,
    Dexterity = 8,
    Intelligence = 16,
}
[Flags]
public enum UnitTypes
{
    Default = 0,
    Player = 2,
    Friendly = 4,
    Enemy = 8
}
public class UnitScript : MonoBehaviourPunCallbacks
{
    private PlayerDownedScript _playerDownedScript;
    private PlayerMultiplayerIdScript _playerMultiplayerIdScript;

    //Unit Info
    [SerializeField] private UnitTypes _unitType;
    [SerializeField] private CharacterClassSOBase _characterClass;
    [SerializeField] private int _expGiveOnDeath;
    [SerializeField] private float _expMultiplierOnLevel;

    //[SerializeField, ReadOnlyString] private string _allowedWeapons;
    //[SerializeField, ReadOnlyString] private string _allowedArmor;
    [SerializeField, ReadOnlyString] private string _allowedMagic;

    //Runtime Unit Stats
    [SerializeField, ReadOnlyInt] private int _level;
    [SerializeField, ReadOnlyInt] private int _curExp;
    [SerializeField, ReadOnlyInt] private int _maxExpToLevel;
    [Space(10)]
    [SerializeField, ReadOnlyInt] private int _health;
    [SerializeField, ReadOnlyInt] private int _mana;
    [Space(10)]
    [SerializeField, ReadOnlyInt] private int _adjustedStamina;
    [SerializeField, ReadOnlyInt] private int _adjustedStrength;
    [SerializeField, ReadOnlyInt] private int _adjustedDexterity;
    [SerializeField, ReadOnlyInt] private int _adjustedIntelligence;

    //Base Unit Stats
    [SerializeField, ReadOnlyInt] private int _initLevel = 1;
    [SerializeField, ReadOnlyInt] private int _initMaxExpToLevel;
    [Space(10)]
    [SerializeField, ReadOnlyInt] private int _maxHealth;
    [SerializeField, ReadOnlyInt] private int _maxMana;
    [Space(10)]
    [SerializeField, ReadOnlyInt] private int _baseStamina;
    [SerializeField, ReadOnlyInt] private int _baseStrength;
    [SerializeField, ReadOnlyInt] private int _baseDexterity;
    [SerializeField, ReadOnlyInt] private int _baseIntelligence;
   
    //Base Unit Resists
    [SerializeField, ReadOnlyInt] private int _physResist;
    [SerializeField, ReadOnlyInt] private int _magicResist;

    public UnitTypes GetUnitType => _unitType;
    public CharacterClassSOBase GetCharacterClass => _characterClass;
    public int GetStamina => _adjustedStamina;
    public int GetStrength => _adjustedStrength;
    public int GetDexterity => _adjustedDexterity;
    public int GetIntelligence => _adjustedIntelligence;
    public int GetPhysResist => _physResist;
    public int GetMagicResist => _magicResist;

    //public string GetAllowedArmor => _allowedArmor;
    //public string GetAllowedWeapons => _allowedWeapons;
    public string GetAllowedMagic => _allowedMagic;
    public int GetOnDeathEXP => _expGiveOnDeath;

    public Action<int> PlayerDowned;
    public Action PlayerClassLoaded;

    private void Start()
    {
        _playerMultiplayerIdScript = GetComponent<PlayerMultiplayerIdScript>();
        _playerDownedScript = GetComponent<PlayerDownedScript>();
        
        UnitInit();
    }

    [PunRPC]
    private void AssignClass(string classSpriteName = "Goblin")
    {
        if (_unitType == UnitTypes.Player && NetworkManager.instance.playersAndClass.ContainsKey(_playerMultiplayerIdScript.id))
            classSpriteName = NetworkManager.instance.playersAndClass[_playerMultiplayerIdScript.id];

        if (_unitType == UnitTypes.Player && !NetworkManager.instance.playersAndClass.ContainsKey(_playerMultiplayerIdScript.id))
            Debug.Log("player not in dict");

        CharacterClassSOBase tempCharacterClass = null; 

        switch (classSpriteName)
        {
            case "Goblin":
                tempCharacterClass = Resources.Load<CharacterClassSOBase>("Classes/Enemies/GoblinEnemyClass");
                break;
            case "Dwarf1":
                tempCharacterClass = Resources.Load<CharacterClassSOBase>("Classes/Players/DwarfAxeman");
                break;
            case "PlagueD1":
                tempCharacterClass = Resources.Load<CharacterClassSOBase>("Classes/Players/PlagueDoctor");
                break;
            case "Duelist1":
                tempCharacterClass = Resources.Load<CharacterClassSOBase>("Classes/Players/DuelistGirl");
                break;
        }

        _characterClass = tempCharacterClass;
        PlayerClassLoaded?.Invoke();
    }

    // Set initial values from class SO during editing (called from editor script)
    public void UpdateClassInfoInspector()
    {
        _expMultiplierOnLevel = _characterClass.GetExpLevelModifier;

        //_allowedWeapons = (int)_characterClass.GetAllowedWeapons == -1 ? "All Weapons" : _characterClass.GetAllowedWeapons.ToString();
        //_allowedArmor = (int)_characterClass.GetAllowedArmor == -1 ? "All Armor" : _characterClass.GetAllowedArmor.ToString();

        _maxHealth = _characterClass.GetMaxHealth;
        _maxMana = _characterClass.GetMaxMana;
        
        _initLevel = _characterClass.GetStartingLevel;
        _initMaxExpToLevel = _characterClass.GetInitExpToLevel;
        _expGiveOnDeath = _characterClass.GetExpGiveOnDeath;

        _baseStamina = _characterClass.GetDefaultStam;
        _baseStrength = _characterClass.GetDefaultStr;
        _baseDexterity = _characterClass.GetDefaultDex;
        _baseIntelligence = _characterClass.GetDefaulInt;
    }
    public void UpdateSpellInfo()
    {
        _allowedMagic = (int)_characterClass.GetAllowedMagic == -1 ? "All Magic" : _characterClass.GetAllowedMagic.ToString();
    }

    // Initialize unit stats
    private void UnitInit()
    {
        AssignClass();
        UpdateClassInfoInspector();

        _unitType = _characterClass.GetUnitType;
        _health = _maxHealth;
        _mana = _maxMana;
        _level = _initLevel;
        _maxExpToLevel = _initMaxExpToLevel;

        int tempLayer = -1;

        //Auto set layer based on unit type
        switch (_unitType)
        {
            case UnitTypes.Default:
                tempLayer = LayerMask.NameToLayer("Default");
                break;
            case UnitTypes.Player:
                tempLayer = LayerMask.NameToLayer("Player");
                break;
            case UnitTypes.Friendly:
                tempLayer = LayerMask.NameToLayer("Friendly");
                break;
            case UnitTypes.Enemy:
                tempLayer = LayerMask.NameToLayer("Enemy");
                break;
        }
        if (tempLayer != -1)
            gameObject.layer = tempLayer;
        else
            throw new Exception($"\"{_unitType}\" layer has not been created.");

        ResetStats();
    }

    // Increase or decrease unit health and call OnDeath if health reaches 0
    public int ChangeHealth(int amount, bool isGain, List<DamageTypes> dmgType = null)
    {
        if (_unitType == UnitTypes.Player && _playerDownedScript.IsDowned) return 0;

        if (!isGain && dmgType != null)
            amount -= DamageReduction(dmgType);

        _health += isGain ? amount : -amount;

        if (_health >= _maxHealth)
            _health = _maxHealth;
        else if (_health <= 0)
        {
            _health = 0;
            if (_unitType == UnitTypes.Player)
                PlayerDowned?.Invoke(GetComponent<PlayerMultiplayerIdScript>().id);
            else
                Invoke(nameof(OnDeath), 0.1f);
            return _expGiveOnDeath;
        }

        return 0;
    }

    //Calculate phys and magic resist 
    private int DamageReduction(List<DamageTypes> dmgType)
    {
        int reductionAmout = 0;

        if (dmgType.Contains(DamageTypes.Physical))
            reductionAmout += _physResist;
        if (dmgType.Contains(DamageTypes.Magic))
            reductionAmout += _magicResist;

        return reductionAmout;
    }

    //Death stuff
    public void OnDeath()
    {
        Destroy(gameObject);
    }
    public void OnRevive()
    {
        _health = _maxHealth;
        _mana = _maxMana;
    }

    // Increase or decrease unit mana until it reaches 0
    public bool ChangeMana(int amount, bool isGain)
    {
        if (amount > _mana && !isGain) return false;

        _mana += isGain ? amount : -amount;

        if (_mana <= 0)
            _mana = 0;
        else if (_mana >= _maxMana)
            _mana = _maxMana;

        return true;
    }
    // Increase unit EXP until it reaches max, then level up unit
    public void ChangeExp(int amount, bool isGain)
    {
        _curExp += isGain ? amount : -amount;

        if (_curExp <= 0)
            _curExp = 0;
        else if (_curExp >= _maxExpToLevel)
            UnitLevelUp();
    }

    //Increase unit's level by 1 and adjust stats based on class stat increases
    private void UnitLevelUp()
    {
        _level++;
        _curExp -= _maxExpToLevel;
        _maxExpToLevel = (int)(_maxExpToLevel * _expMultiplierOnLevel);
        int[] statIncreases= new int[] { _characterClass.GetStamIncPerLevel, 
                                         _characterClass.GetStrIncPerLevel, 
                                         _characterClass.GetDexIncPerLevel, 
                                         _characterClass.GetIntIncPerLevel};
        UpdateStats(statIncreases, true);
    }

    //Update stats with int array param values, increase or decrease stat based on incoming bool param
    public void UpdateStats(int[] values, bool isBuff)
    {
        _adjustedStamina += isBuff ? values[0] : -values[0];
        _adjustedStrength += isBuff ? values[1] : -values[1];
        _adjustedDexterity += isBuff ? values[2] : -values[2];
        _adjustedIntelligence += isBuff ? values[3] : -values[3];

        _physResist = _adjustedStamina - _characterClass.GetDefaultStatValue;
        _magicResist = _adjustedIntelligence - _characterClass.GetDefaultStatValue;
    }

    //Reset stats to default values (also used in initialization)
    public void ResetStats()
    {
        _adjustedStamina = _baseStamina;
        _adjustedStrength = _baseStrength;
        _adjustedDexterity = _baseDexterity;
        _adjustedIntelligence = _baseIntelligence;

        _physResist = 0;
        _magicResist = 0;
    }
}
