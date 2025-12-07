using System;
using System.Collections.Generic;

// Static class for easy use of enum and relevant method(s).
public static class DamageTypeEnum
{
    [Flags]
    public enum DamageTypes
    {
        None = 0,
        Physical = 2,
        Ice = 4,
        Fire = 8,
        Holy = 16,
        Unholy = 32
    }

    // Create and return a list of all independent DamageTypes of a given enum flag.
    public static List<DamageTypes> GetAllTypesFromFlags(DamageTypes dmgType)
    {
        List<DamageTypes> allDmgTypes = new List<DamageTypes>();

        // Convert dmgType enum flag to binary.
        string binaryEnum = Convert.ToString((int)dmgType, 2).PadLeft(8, '0');

        // Loop through each character in the binaryEnum string and add relevant
        // DamageTypes to the list.
        for (int i = 0; i < binaryEnum.Length; i++)
        {
            switch (i)
            {
                case 0://not implemented
                    break;
                case 1://not implemented
                    break;
                case 2://Ice
                    if (binaryEnum[i] == '1')
                        allDmgTypes.Add(DamageTypes.Unholy);
                    break;
                case 3://Fire
                    if (binaryEnum[i] == '1')
                        allDmgTypes.Add(DamageTypes.Holy);
                    break;
                case 4://Pierce
                    if (binaryEnum[i] == '1')
                        allDmgTypes.Add(DamageTypes.Fire);
                    break;
                case 5://Blunt
                    if (binaryEnum[i] == '1')
                        allDmgTypes.Add(DamageTypes.Ice);
                    break;
                case 6://Slash
                    if (binaryEnum[i] == '1')
                        allDmgTypes.Add(DamageTypes.Physical);
                    break;
                case 7://None
                    break;
            }
        }
        return allDmgTypes;
    }
}
