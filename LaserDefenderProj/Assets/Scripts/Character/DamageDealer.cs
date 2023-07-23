using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    private const int DEFAULT_DAMAGE = 1;

    [SerializeField] private int damage = DEFAULT_DAMAGE;

    public int GetDamage()
    {
        return damage;
    }
}
