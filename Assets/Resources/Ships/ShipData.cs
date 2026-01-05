using UnityEngine;

[CreateAssetMenu(fileName = "ShipData", menuName = "Game/Ship Data")]
public class ShipData : ScriptableObject
{
    [Header("Ship Info")]
    public string shipName = "Default Ship";
    public string description = "A balanced ship for beginners";
    public Sprite shipSprite;
    public GameObject shipPrefab;

    [Header("Stats")]
    public int maxHealth = 100;
    public int maxEnergy = 100;
    public float moveSpeed = 5f;
    public float fireRate = 0.2f;
    public int baseDamage = 10;
    public float specialAttackDamage = 30f;

    [Header("Special Attack")]
    public float laserDuration = 2f;
    public float laserCooldown = 5f;
    public float laserEnergyCost = 50f;

    [Header("Visuals")]
    public Color shipColor = Color.white;
    public GameObject bulletPrefab;
    public GameObject laserPrefab;
}