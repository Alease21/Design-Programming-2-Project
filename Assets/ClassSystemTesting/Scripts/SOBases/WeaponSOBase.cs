using UnityEngine;
using static DamageTypeEnum;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Equipment System/New Weapon")]
public class WeaponSOBase : ScriptableObject, IEquipment
{
    [Header("Weapon Info"), Space(5)]
    public string weaponName;
    [TextArea(3, 6)]
    public string weaponDescription;

    [Header("Weapon Type"), Space(5)]
    [SerializeField] private WeaponTypes _weaponType;
    [SerializeField] private GameObject _weaponPrefab;

    [Header("Damage"), Space(5)]
    [SerializeField, HideInInspector] private DamageTypes _damageTypes;
    [SerializeField, HideInInspector] private int _physDamage;
    [SerializeField, HideInInspector] private int _iceDamage;
    [SerializeField, HideInInspector] private int _fireDamage;
    [SerializeField, HideInInspector] private int _holyDamage;
    [SerializeField, HideInInspector] private int _unholyDamage;

    public string GetName => weaponName;

    public WeaponTypes GetWeaponType => _weaponType;
    public DamageTypes GetDamageTypes => _damageTypes;
    public int GetPhysDamage => _physDamage;
    public int GetIceDamage => _iceDamage;
    public int GetFireDamage => _fireDamage;
    public int GetHolyDamage => _holyDamage;
    public int GetUnholyDamage => _unholyDamage;

    public void SpawnEquipmentPrefab(Transform tranform, Vector3 pos)
    {
        GameObject weapon = Instantiate(_weaponPrefab, tranform);
        weapon.transform.localPosition = pos;
        weapon.transform.localRotation = Quaternion.identity;
    }
}
