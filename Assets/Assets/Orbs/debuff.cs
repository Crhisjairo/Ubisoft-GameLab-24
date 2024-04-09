using UnityEngine;
using _Scripts.Controllers;

public class Debuff : Orb
{
    // Override the Awake method to set a different gravity scale
    protected override void Awake()
    {
        gravityScale = 0.22f;
        base.Awake();
    }

}
