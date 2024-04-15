using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Scripts.Controllers.PlayerMoveSets
{
    public class RedBomb : Bomb
    {
        // Start is called before the first frame update
        protected override void Start()
        {
            speed = 7f;
            gravityScale = 0.02f;
            base.Start();
        }

    }
}
