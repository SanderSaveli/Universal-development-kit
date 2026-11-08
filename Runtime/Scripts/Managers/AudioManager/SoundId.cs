using System;

namespace SanderSaveli.UDK.Audio
{
    public readonly struct SoundId : IEquatable<SoundId>
    {
        public string Value => _value ?? string.Empty;

        private readonly string _value;

        public SoundId(string value)
        {
            _value = value;
        }

        public bool Equals(SoundId other)
        {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is SoundId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(Value);
        }

        public override string ToString()
        {
            return Value;
        }

        public static bool operator ==(SoundId left, SoundId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SoundId left, SoundId right)
        {
            return !left.Equals(right);
        }
    }
}
