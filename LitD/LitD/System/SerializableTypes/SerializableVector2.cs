using Microsoft.Xna.Framework;
using ProtoBuf;

namespace LitD.System.SerializableTypes
{
    [ProtoContract]
    internal class SerializableVector2
    {
        [ProtoMember(1)]
        public float X {  get; set; }
        [ProtoMember(2)]
        public float Y { get; set; }

        public SerializableVector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public SerializableVector2(Vector2 v)
        {
            X = v.X;
            Y = v.Y;
        }

        private SerializableVector2()
        {}

        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }

        
        public static implicit operator SerializableVector2(Vector2 v)
        {
            return new SerializableVector2(v);
        }

        
        public static implicit operator Vector2(SerializableVector2 sv)
        {
            return new Vector2(sv.X, sv.Y);
        }
    }
}
