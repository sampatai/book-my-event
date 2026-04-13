using System;

namespace Domain.ValueObjects
{
    public struct PanditId
    {
        private readonly Guid _value;

        public PanditId(Guid value)
        {
            _value = value;
        }

        public static implicit operator Guid(PanditId panditId) => panditId._value;

        public static explicit operator PanditId(Guid value) => new PanditId(value);

        public override bool Equals(object obj)
        {
            if (obj is PanditId panditId)
                return _value.Equals(panditId._value);
            return false;
        }

        public override int GetHashCode() => _value.GetHashCode();

        public override string ToString() => _value.ToString();
    }
}