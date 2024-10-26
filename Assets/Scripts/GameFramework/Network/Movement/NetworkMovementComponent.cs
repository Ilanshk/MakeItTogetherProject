using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace GameFramework.Network.Movement
{
    public class NetworkMovementComponent : NetworkBehaviour
    {
        [SerializeField] private UnityEngine.CharacterController _cc;

        [SerializeField] private float _speed;
        [SerializeField] private float _turnSpeed;

        [SerializeField] private Transform _camSocket;
        [SerializeField] private GameObject _vcam;
        [SerializeField] private Vector2 _minMaxRotation;

        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private Color _color;

        private Transform _vcamTransform;

        private float _cameraAngle;

        private int _tick = 0;
        private float _tickRate = 1f / 120f;
        private float _tickDeltaTime = 0;

        private const int BUFFER_SIZE = 1024;
        private InputState[] _inputStates = new InputState[BUFFER_SIZE];
        private TransformState[] _transformStates = new TransformState[BUFFER_SIZE];

        public NetworkVariable<TransformState> ServerTransformState = new NetworkVariable<TransformState>();
        public TransformState _previousTransformState;

        private void OnEnable()
        {
            ServerTransformState.OnValueChanged += OnValueChanged;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _vcamTransform = _vcam.transform;
        }

        private void OnValueChanged(TransformState previousvalue, TransformState serverState)
        {
            if (!IsLocalPlayer) return;

            if(_previousTransformState == null)
            {
                _previousTransformState = serverState;
            }
            TransformState calculatedState = _transformStates.First(localState => localState.Tick == serverState.Tick);
            if(calculatedState.Position != serverState.Position)
            {
                Debug.Log("Correcting client position");
                //Teleport the player to the server position
                TeleportPlayer(serverState);
                //Replay the inputs that happened after
                IEnumerable<InputState> inputs = _inputStates.Where(input => input.Tick > serverState.Tick);
                inputs = from input in inputs orderby input.Tick select input;

                foreach(InputState inputState in inputs)
                {
                    MovePlayer(inputState.MovementInput);
                    RotatePlayer(inputState.LookInput);

                    TransformState newTransformState = new TransformState()
                    {
                        Tick = inputState.Tick,
                        Position = transform.position,
                        Rotation = transform.rotation,
                        HasStartedMoving = true
                    };

                    for(int i = 0; i < _transformStates.Length; i++)
                    {
                        if (_transformStates[i].Tick == inputState.Tick)
                        {
                            _transformStates[i] = newTransformState;
                            break;
                        }
                    }
                }
            }
        }

        private void TeleportPlayer(TransformState serverState)
        {
            _cc.enabled = false;
            transform.position = serverState.Position;
            transform.rotation = serverState.Rotation;
            _cc.enabled = true;

            for(int i = 0; i < _transformStates.Length; i++)
            {
                if (_transformStates[i].Tick == serverState.Tick)
                {
                    _transformStates[i] = serverState;
                    break;
                }
            }
        }

        public void ProcessLocalPlayerMovement(Vector2 movementInput, Vector2 lookInput)
        {
            _tickDeltaTime += Time.deltaTime;
            if(_tickDeltaTime > _tickRate)
            {
                int bufferIndex = _tick % BUFFER_SIZE;

                if(!IsServer)
                {
                    MovePlayerServerRPC(_tick, movementInput, lookInput);
                    MovePlayer(movementInput);
                    RotatePlayer(lookInput);
                    SaveState(movementInput, lookInput, bufferIndex);
                }
                else
                {
                    MovePlayer(movementInput);
                    RotatePlayer(lookInput);

                    TransformState state = new TransformState()
                    {
                        Tick = _tick,
                        Position = transform.position,
                        Rotation = transform.rotation,
                        HasStartedMoving = true
                    };

                    SaveState(movementInput, lookInput, bufferIndex);

                    _previousTransformState = ServerTransformState.Value;
                    ServerTransformState.Value = state;

                }
                _tickDeltaTime -= _tickRate;
                _tick++;
            }
        }

        

        public void ProcessSimulatedPlayerNovement()
        {
            _tickDeltaTime += Time.deltaTime;
            if(_tickDeltaTime > _tickRate)
            {
                if(ServerTransformState.Value.HasStartedMoving)
                {
                    transform.position = ServerTransformState.Value.Position;
                    transform.rotation = ServerTransformState.Value.Rotation;
                }

                _tickDeltaTime -= _tickRate;
                _tick++;
            }
        }

        private void SaveState(Vector2 movementInput, Vector2 lookInput, int bufferIndex)
        {
            InputState inputState = new InputState()
            {
                Tick = _tick,
                MovementInput = movementInput,
                LookInput = lookInput
            };

            TransformState transformState = new TransformState()
            {
                Tick = _tick,
                Position = transform.position,
                Rotation = transform.rotation,
                HasStartedMoving = true
            };

            _inputStates[bufferIndex] = inputState;
            _transformStates[bufferIndex] = transformState;
        }

        private void MovePlayer(Vector2 movementInput)
        {
            Vector3 movement = movementInput.x * _vcamTransform.right + movementInput.y * _vcamTransform.forward;

            movement.y = 0;
            if(!_cc.isGrounded)
            {
                movement.y = Physics.gravity.y;
            }

            _cc.Move(movement * _speed * _tickRate);
        }

        private void RotatePlayer(Vector2 lookInput)
        {
            _cameraAngle = Vector3.SignedAngle(transform.forward, _vcamTransform.forward, _vcamTransform.right);
            float cameraRotationAmount = lookInput.y * _turnSpeed * Time.deltaTime;
            float newCameraAngle = _cameraAngle - cameraRotationAmount;
            if (newCameraAngle <= _minMaxRotation.x && newCameraAngle >= _minMaxRotation.y)
            {
                _vcamTransform.RotateAround(_vcamTransform.position, _vcamTransform.right, -lookInput.y * _turnSpeed * Time.deltaTime);
            }
            transform.RotateAround(transform.position, transform.up, lookInput.x * _turnSpeed * _tickRate);
        }


        [ServerRpc]
        private void MovePlayerServerRPC(int tick, Vector2 movementInput, Vector2 lookInput)
        {
            MovePlayer(movementInput);
            RotatePlayer(lookInput);

            TransformState state = new TransformState()
            {
                Tick = tick,
                Position = transform.position,
                Rotation = transform.rotation,
                HasStartedMoving = true
            };

            _previousTransformState = ServerTransformState.Value;
            ServerTransformState.Value = state;
        }
    }
}
