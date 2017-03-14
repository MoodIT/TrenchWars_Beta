using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Base : MonoBehaviour
{
    [SerializeField]
    protected int damage = 1;

    [SerializeField]
    protected int speed = 1;

    public Character_Base Owner { get; set; }
    public Vector3 Direction { get; set; }
}
