using UnityEngine;

[CreateAssetMenu(fileName = "Shop Upgrades", menuName = "Shop upgrade object", order = 2)]
public class ShopUpgrades : ScriptableObject
{
    [Header("Gun upgrades")]
    public float[] damageValues;
    public int[] damagePrice;

    public float[] cooldownValues;
    public int[] cooldownPrice;

    public float[] overheatValues;
    public int[] overheatPrice;

    [Header("Hull upgrades")]
    public float[] maxHealthValues;
    public int[] maxHealthPrice;
    public int maxHullRepairPrice;
}
