using UnityEngine;
using static DamageTypeEnum;

[CreateAssetMenu(fileName = "NewArmor", menuName = "Equipment System/New Armor")]
public class ArmorSOBase : ScriptableObject, IEquipment
{
    [Header("Armor Info"), Space(5)]
    public string armorName;
    [TextArea(3, 6)]
    public string armorDescription;

    [Header("Armor Type"), Space(5)]
    [SerializeField] private ArmorTypes _armorType;

    [Header("Resists"), Space(5)]
    [SerializeField, HideInInspector] private int _baseArmorValue;
    [SerializeField, HideInInspector] private DamageTypes _armorResistances;
    [SerializeField, HideInInspector] private int _physResist;
    [SerializeField, HideInInspector] private int _iceResist;
    [SerializeField, HideInInspector] private int _fireResist;
    [SerializeField, HideInInspector] private int _holyResist;
    [SerializeField, HideInInspector] private int _unholyResist;

    public string GetName => armorName;

    public ArmorTypes GetArmorType => _armorType;
    public int GetBaseArmor => _baseArmorValue;
    public DamageTypes GetDamageResistTypes => _armorResistances;
    public int GetPhysResist => _physResist;
    public int GetIceResist => _iceResist;
    public int GetFireResist => _fireResist;
    public int GetHolyResist => _holyResist;
    public int GetUnholyResist => _unholyResist;


    public void SpawnEquipmentPrefab(Transform tranform, Vector3 pos)
    {
        //No armor prefabs implemented
    }
}
