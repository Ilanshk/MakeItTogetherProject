using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace GameFramework.Network.Movement
{
    public class TransformState : INetworkSerializable, IEquatable<TransformState>
    {
        public int Tick;
        public Vector3 Position;
        public Quaternion Rotation;
        public bool HasStartedMoving;

        public override bool Equals(object obj)
        {
            return Equals(obj as TransformState);
        }

        public bool Equals(TransformState other)
        {
            return other is not null &&
                   Tick == other.Tick &&
                   Position.Equals(other.Position) &&
                   Rotation.Equals(other.Rotation) &&
                   HasStartedMoving == other.HasStartedMoving;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Tick, Position, Rotation, HasStartedMoving);
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if(serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out Tick);
                reader.ReadValueSafe(out Position);
                reader.ReadValueSafe(out Rotation);
                reader.ReadValueSafe(out HasStartedMoving);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(Tick);
                writer.WriteValueSafe(Position);
                writer.WriteValueSafe(Rotation);
                writer.WriteValueSafe(HasStartedMoving);
            }
        }
    }
}
