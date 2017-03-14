using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Base : Character_Base
{
    public override bool IsPlayer { get { return false; } }
}
