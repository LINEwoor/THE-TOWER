using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float maxHp;
    public float hp;
    public float dmg;
    public float range;

    private void Start()
    {
        hp =  maxHp;
    }
    
    
}
