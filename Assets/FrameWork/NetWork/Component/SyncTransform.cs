
using Riptide;
using UnityEngine;

namespace FrameWork
{
    [RequireComponent(typeof(Identity))]
    public class SyncTransform : MonoBehaviour
    { 
        public float positionSyncSpeed=10;
        public float rotationSyncSpeed = 1;
        public float range=0.05f;
        
        
        private Identity _identity;
        private Vector3 _curLoc;
        private Vector3 _syncLoc;

        private Vector3 _curRo;
        private Vector3 _syncRo;
        private float _lerpPosition;
        private float _lerpRotation;


        private Vector3 _loc;
        
        private float _sqRange;
        
        private void Awake()
        {
            _identity = GetComponent<Identity>();
            _sqRange = range * range;
        }

        private void OnEnable()
        {
            NetWorkSystem.OnTransform += SyncLoc;
        }


        private void OnDisable()
        {
            NetWorkSystem.OnTransform -= SyncLoc;
        }
        

        private void FixedUpdate()
        {
            
            if (_identity.IsLocalSpawn())
            {
                var msg=NetWorkSystem.CreateMessage(MessageSendMode.Unreliable, ClientToServerMessageType.Transform);
                msg.AddUShort(_identity.GetId());
                msg.AddVector3(transform.position);
                msg.AddVector3(transform.eulerAngles);
                //_loc = loc;
                NetWorkSystem.Send(msg);
            }
        }


        private void Update()
        {
            if (!_identity.IsLocalSpawn())
            {
                transform.position = Vector3.Lerp(_curLoc, _syncLoc, _lerpPosition);
                transform.rotation=Quaternion.Lerp(Quaternion.Euler(_curRo), Quaternion.Euler(_syncRo), _lerpRotation);
                _lerpPosition += Time.deltaTime*positionSyncSpeed;
                _lerpRotation += Time.deltaTime * rotationSyncSpeed;
            }
        }
        
        
        private void SyncLoc(ushort tick,ushort id,Vector3 loc,Vector3 ro)
        {
            //if (NetWorkSystem.GetClientId().Equals(id))return;
            if(_identity.GetId()!=id)return;
            if (_identity.IsLocalSpawn()) return;
            var ti = NetWorkSystem.serverTick;
            if (ti>=tick&& tick>=ti-2)
            {
                _curLoc = transform.position;
                _syncLoc = loc;

                _curRo = transform.eulerAngles;
                _syncRo = ro;
                
                _lerpPosition = 0;
                _lerpRotation = 0;
            }
        }
    }
}