using System;
using Mirror;
using UnityEngine;
namespace _Scripts.Map
{
    public class Hook : NetworkBehaviour
    {
       // public NetworkIdentity Identity;

        private Rigidbody2D _rb;
        private Rigidbody2D _target;
        public Vector2 Offset;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();

            if (!isServer)
                _rb.isKinematic = true;
        }

        public void SetFollowTarget(NetworkConnectionToClient conn)
        {
            _target = conn.identity.GetComponent<Rigidbody2D>();

        }

        private void FixedUpdate()
        {
            if(!isServer)
                return;
            
            if(_target != null)
                _rb.position = _target.position + Offset;
        }
    }
}
