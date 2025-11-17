using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;
using static DamageTypeEnum;

public class EquipmentController : MonoBehaviour
{
    [SerializeField] private WeaponSOBase _mainHand;
    [SerializeField] private WeaponSOBase _offHand;
    [SerializeField] private float _weaponOffsetFromCenter;
    [SerializeField] private float _swingSpeed;
    [SerializeField, ReadOnlyInt] private int _physDamage;
    [SerializeField, ReadOnlyInt] private int _iceDamage;
    [SerializeField, ReadOnlyInt] private int _fireDamage;
    [SerializeField, ReadOnlyInt] private int _holyDamage;
    [SerializeField, ReadOnlyInt] private int _unholyDamage;

    [SerializeField] private ArmorSOBase _headArmor;
    [SerializeField] private ArmorSOBase _chestArmor;
    [SerializeField] private ArmorSOBase _legArmor;
    [SerializeField] private ArmorSOBase _footArmor;

    [SerializeField, ReadOnlyInt] private int _physResist;
    [SerializeField, ReadOnlyInt] private int _iceResist;
    [SerializeField, ReadOnlyInt] private int _fireResist;
    [SerializeField, ReadOnlyInt] private int _holyResist;
    [SerializeField, ReadOnlyInt] private int _unholyResist;

    private Dictionary<WeaponSOBase, bool> _weapons = new();
    private Dictionary<ArmorSOBase, bool> _armor = new();

    private Transform _mainHandAnchor;
    private Transform _offHandAnchor;
    private bool _isSwinging = false;

    private UnitScript unit;

    public int GetPhysDamage => _physDamage;
    public int GetIceDamage => _iceDamage;
    public int GetFireDamage => _fireDamage;
    public int GetHolyDamage => _holyDamage;
    public int GetUnholyDamage => _unholyDamage;

    public int GetPhysResist => _physResist;
    public int GetIceResist => _iceResist;
    public int GetFireResist => _fireResist;
    public int GetHolyResist => _holyResist;
    public int GetUnholyResist => _unholyResist;

    public Dictionary<WeaponSOBase, bool> GetWeapons => _weapons;
    public Dictionary<ArmorSOBase, bool> GetArmor => _armor;

    private void Start()
    {
        SetEquippedArmor();
        SetEquippedWeapons();

        CalculateResistances();
        
        _mainHandAnchor = new GameObject("MainHandAnchor").transform;
        _mainHandAnchor.parent = transform;
        _mainHandAnchor.localPosition = Vector3.zero;
        _offHandAnchor = new GameObject("OffHandAnchor").transform;
        _offHandAnchor.parent = transform;
        _offHandAnchor.localPosition = Vector3.zero;

        SpawnEquipment();
    }
    private void Update()
    {
        if (unit.GetUnitType != UnitTypes.Player) return;

        if (Input.GetMouseButtonDown(0) && !_isSwinging)
            StartCoroutine(SwingWeapons());
    }

    //take in current equipment and spawn each sprite/prefab
    public void SpawnEquipment()
    {
        for (int i = 0; i < _weapons.Count; i++)
        {
            var element = _weapons.ElementAt(i);

            if (element.Value == true)
                element.Key?.SpawnEquipmentPrefab(i == 0 ? _mainHandAnchor : _offHandAnchor, i == 0 ? _weaponOffsetFromCenter * Vector3.right : _weaponOffsetFromCenter * Vector3.left);
        }
    }

    public bool[] SetEquippedWeapons()
    {
        unit = GetComponent<UnitScript>(); 

        Dictionary<WeaponSOBase, bool> weaponDict = new();
        WeaponSOBase[] equippedWeapons = new WeaponSOBase[] { _mainHand, _offHand };
        List<bool> tempBoolList = new();
        for (int i = 0; i < equippedWeapons.Length; i++)
        {
            WeaponSOBase weapon = equippedWeapons[i];
            if (weapon == null) continue;

            bool isValid = unit.GetAllowedWeapons.Contains(equippedWeapons[i].GetWeaponType.ToString()) ||
                unit.GetAllowedWeapons.Contains("All");

            tempBoolList.Add(isValid);
            weaponDict.Add(weapon, isValid);
        }

        _weapons = weaponDict;
        CalculateWeaponStats();
        return tempBoolList.ToArray();
    }
    public bool[] SetEquippedArmor()
    {
        unit = GetComponent<UnitScript>();

        Dictionary<ArmorSOBase, bool> armorDict = new();
        ArmorSOBase[] equippedArmor = new ArmorSOBase[] { _headArmor, _chestArmor, _legArmor, _footArmor };
        List<bool> tempBoolList = new();
        for (int i = 0; i < equippedArmor.Length; i++)
        {
            ArmorSOBase armor = equippedArmor[i];
            if (armor == null) continue;

            bool isValid = unit.GetAllowedArmor.Contains(equippedArmor[i].GetArmorType.ToString()) ||
                unit.GetAllowedArmor.Contains("All");

            tempBoolList.Add(isValid);
            armorDict.Add(equippedArmor[i], isValid);
        }

        _armor = armorDict;
        CalculateResistances();
        return tempBoolList.ToArray();
    }
    public void ResetArmorStats()
    {
        _physResist = 0;
        _iceResist = 0;
        _fireResist = 0;
        _holyResist = 0;
        _unholyResist = 0;
    }
    public void CalculateWeaponStats()
    {
        ResetWeaponStats();

        foreach (KeyValuePair<WeaponSOBase, bool> kvp in _weapons)
        {
            var weap = kvp.Key;
            _physDamage += weap.GetPhysDamage;
            _iceDamage += weap.GetIceDamage;
            _fireDamage += weap.GetFireDamage;
            _holyDamage += weap.GetHolyDamage;
            _unholyDamage += weap.GetUnholyDamage;
        }
    }

    private void ResetWeaponStats()
    {
        _physDamage = 0;
        _iceDamage = 0;
        _fireDamage = 0;
        _holyDamage = 0;
        _unholyDamage = 0;
    }

    public void CalculateResistances()
    {
        ResetArmorStats();

        foreach (KeyValuePair<ArmorSOBase, bool> kvp in _armor)
        {
            var armor = kvp.Key;
            _physResist += armor.GetPhysResist + armor.GetBaseArmor;
            _iceResist += armor.GetIceResist;
            _fireResist += armor.GetFireResist;
            _holyResist += armor.GetHolyResist;
            _unholyResist += armor.GetUnholyResist;
        }
    }

    //Calculate total damage to deal
    public int CalculateDamage(List<DamageTypes> dmgTypes)
    {
        int totalDmg = 0;

        bool canUseMain = false;
        bool canUseOff = false;

        if (_mainHand != null)
            canUseMain = _weapons[_mainHand];
        if (_offHand != null)
            canUseOff = _weapons[_offHand];

        foreach (DamageTypes type in dmgTypes)
            switch (type)
            {
                case DamageTypes.Physical:
                    totalDmg += (canUseMain ? _mainHand.GetPhysDamage : 0) +
                                (canUseOff ? _offHand.GetPhysDamage : 0) + unit.GetStrength - 10; //subtract "baseline" strength stat from total strength to get modifier
                    break;
                case DamageTypes.Ice:
                    totalDmg += (canUseMain ? _mainHand.GetIceDamage : 0) +
                                (canUseOff ? _offHand.GetIceDamage : 0);
                    break;
                case DamageTypes.Fire:
                    totalDmg += (canUseMain ? _mainHand.GetFireDamage : 0) +
                                (canUseOff ? _offHand.GetFireDamage : 0);
                    break;
                case DamageTypes.Holy:
                    totalDmg += (canUseMain ? _mainHand.GetHolyDamage : 0) +
                                (canUseOff ? _offHand.GetHolyDamage : 0);
                    break;
                case DamageTypes.Unholy:
                    totalDmg += (canUseMain ? _mainHand.GetUnholyDamage : 0) +
                                (canUseOff ? _offHand.GetUnholyDamage : 0);
                    break;
            }
        return totalDmg;
    }
    public void DealWeaponDamage(UnitScript target)
    {
        if (target == null) return;

        var dmgTypes = CombineWeaponDamageTypes();

        int exp = target.ChangeHealth(CalculateDamage(dmgTypes), false, dmgTypes);
        if (exp != 0)
            unit.ChangeExp(exp, exp > 0 ? true : false);
    }
    public IEnumerator SwingWeapons()
    {
        if (_mainHand == null && _offHand == null) yield break;

        Transform mainhandWeap = null;
        Transform offhandWeap = null;

        _isSwinging = true;
        if (_mainHandAnchor.childCount > 0)
        {
            mainhandWeap = _mainHandAnchor.GetChild(0);
            mainhandWeap.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
        }
        if (_offHandAnchor.childCount > 0)
        {
            offhandWeap = _offHandAnchor.GetChild(0);
            offhandWeap.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
        }

        Vector3 forward = PlayerMovement2.instance.GetMouseDir;
        //Debug.DrawRay(transform.position, forward * 2, Color.red, 1f);

        Collider2D[] foundObjects = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), _weaponOffsetFromCenter + mainhandWeap.localScale.y, LayerMask.GetMask("Enemy"));
        foreach (Collider2D collider in foundObjects)
            if (Vector2.Angle(collider.transform.position, (Vector2)forward) > 90f && collider.TryGetComponent(out UnitScript unit))
                DealWeaponDamage(unit);

        for (float timer = 0f; timer < _swingSpeed; timer += Time.deltaTime)
        {
            float lerpRatio = timer / _swingSpeed;
            _mainHandAnchor.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0f, 90f, lerpRatio));
            _offHandAnchor.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0f, -90f, lerpRatio));
            yield return null;
        }

        _mainHandAnchor.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        _offHandAnchor.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        if (mainhandWeap != null)
            mainhandWeap.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        if (offhandWeap != null)
            offhandWeap.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        _isSwinging = false;
    }
    public List<DamageTypes> CombineWeaponDamageTypes()
    {
        List<DamageTypes> totalDmgTypes = new List<DamageTypes>();

        foreach (KeyValuePair<WeaponSOBase, bool> weapon in _weapons)
            if (weapon.Value)
                foreach (DamageTypes type in GetAllTypesFromFlags(_mainHand.GetDamageTypes))
                    if (!totalDmgTypes.Contains(type))
                        totalDmgTypes.Add(type);

        return totalDmgTypes;
    }

    // Calculate and return amount of damage reduction from armor for a given
    // damage type.
    public int ReduceDamageFromArmor(List<DamageTypes> dmgTypes)
    {
        int dmgReduction = 0;

        // Get list of all independent damage types from given enum flags.
        foreach (DamageTypes type in dmgTypes)
        {
            switch (type)
            {
                case DamageTypes.Physical:
                    dmgReduction += _physResist;
                    break;
                case DamageTypes.Ice:
                    dmgReduction += _iceResist;
                    break;
                case DamageTypes.Fire:
                    dmgReduction += _fireResist;
                    break;
                case DamageTypes.Holy:
                    dmgReduction += _holyResist;
                    break;
                case DamageTypes.Unholy:
                    dmgReduction += _unholyResist;
                    break;
            }
        }
        return dmgReduction;
    }
}
