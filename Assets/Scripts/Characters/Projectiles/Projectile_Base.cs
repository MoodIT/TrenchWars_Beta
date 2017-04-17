using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Base : MonoBehaviour
{
    [SerializeField]
    protected int damage = 1;

    [Header("Sounds")]
    [SerializeField]
    protected List<AudioClip> shootSounds = null;

    [SerializeField]
    protected List<AudioClip> hitSounds = null;

    public Character_Base Owner { get; set; }
    public Vector3 Direction { get; set; }
}
