#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/UBigInteger.cs" was last cleaned by Rick on 2014/08/23 at 12:47 AM

#endregion License & Information

namespace Librainian.Maths {

    using System;
    using System.Globalization;
    using System.Numerics;
    using Annotations;
    using Extensions;
    using FluentAssertions;
    using Numerics;

    /// <summary>
    ///     <para>Unsigned biginteger class.</para>
    /// </summary>
    [Immutable]
    public struct UBigInteger : IComparable, IComparable<UBigInteger> {

        /// <summary>
        ///     <para>
        ///         The lowest <see cref="UBigInteger" /> that is higher than <see cref="Zero" />.
        ///     </para>
        ///     <para>Should be "1".</para>
        /// </summary>
        public static readonly UBigInteger Epsilon = new UBigInteger( 1 );

        /// <summary>
        ///     1
        /// </summary>
        public static readonly UBigInteger One = new UBigInteger( 1 );

        /// <summary>
        ///     2
        /// </summary>
        public static readonly UBigInteger Two = new UBigInteger( 2 );

        /// <summary>
        ///     0
        /// </summary>
        public static readonly UBigInteger Zero = new UBigInteger( 0 );

        private readonly BigInteger _internalValue;

        public UBigInteger( UInt64 value ) {
            this._internalValue = value;
        }

        public UBigInteger( [NotNull] byte[] bytes ) {
            // http: //stackoverflow.com/questions/5649190/byte-to-unsigned-biginteger
            if ( bytes == null ) {
                throw new ArgumentNullException( "bytes" );
            }
            var bytesWith00attheendnd = new byte[ bytes.Length + 1 ];
            bytes.CopyTo( bytesWith00attheendnd, 0 );
            bytesWith00attheendnd[ bytes.Length ] = 0;
            this._internalValue = new BigInteger( bytesWith00attheendnd );

            this._internalValue.Should().BeGreaterOrEqualTo( 0 );
            if ( this._internalValue < 0 ) {
                throw new ArgumentOutOfRangeException();
            }
        }

        public UBigInteger( Int64 value ) {
            value.Should().BeGreaterOrEqualTo( 0 );
            if ( value < 0 ) {
                throw new ArgumentOutOfRangeException();
            }
            this._internalValue = value;
        }

        private UBigInteger( BigInteger value ) {
            value.Should().BeGreaterOrEqualTo( BigInteger.Zero );
            if ( value < BigInteger.Zero ) {
                throw new ArgumentOutOfRangeException();
            }
            this._internalValue = value;
        }

        [Pure]
        public int CompareTo( [NotNull] object obj ) {
            if ( obj == null ) {
                throw new ArgumentNullException( "obj" );
            }
            if ( !( obj is UBigInteger ) ) {
                throw new InvalidCastException();
            }
            return this._internalValue.CompareTo( ( UBigInteger )obj );
        }

        [Pure]
        public int CompareTo( UBigInteger number ) {
            return this._internalValue.CompareTo( number._internalValue );
        }

        public static UBigInteger Add( UBigInteger left, UBigInteger right ) {
            return new UBigInteger( BigInteger.Add( left._internalValue, right._internalValue ) );
        }

        public static explicit operator Int32( UBigInteger number ) {
            return ( Int32 )number._internalValue;
        }

        public static explicit operator Int64( UBigInteger number ) {
            return ( Int64 )number._internalValue;
        }

        public static explicit operator UInt64( UBigInteger number ) {
            return ( UInt64 )number._internalValue;
        }

        public static explicit operator Decimal( UBigInteger number ) {
            return ( Decimal )number._internalValue;
        }

        public static implicit operator BigInteger( UBigInteger number ) {
            return number._internalValue;
        }

        public static implicit operator UBigInteger( long number ) {
            return new UBigInteger( number );
        }

        public static UBigInteger Multiply( UBigInteger left, UBigInteger right ) {
            return new UBigInteger( BigInteger.Multiply( left._internalValue, right._internalValue ) );
        }

        public static UBigInteger operator -( UBigInteger number ) {
            return new UBigInteger( -number._internalValue );
        }

        public static UBigInteger operator -( UBigInteger left, UBigInteger right ) {
            return new UBigInteger( left._internalValue - right._internalValue );
        }

        public static UBigInteger operator %( UBigInteger dividend, UBigInteger divisor ) {
            return new UBigInteger( dividend._internalValue % divisor._internalValue );
        }

        public static UBigInteger operator &( UBigInteger left, UBigInteger right ) {
            return new UBigInteger( left._internalValue & right._internalValue );
        }

        public static UBigInteger operator *( UBigInteger left, UBigInteger right ) {
            return new UBigInteger( left._internalValue * right._internalValue );
        }

        public static UBigInteger operator /( UBigInteger left, UBigInteger right ) {
            return new UBigInteger( left._internalValue / right._internalValue );
        }

        public static Double operator /( Double left, UBigInteger right ) {
            right.Should().BeGreaterThan( Zero );
            var rational = new BigRational( numerator: new BigInteger( left ), denominator: right._internalValue );
            return ( Double )rational;
        }

        public static UBigInteger operator +( UBigInteger left, UBigInteger right ) {
            return new UBigInteger( left._internalValue + right._internalValue );
        }

        public static Boolean operator <( UBigInteger left, long right ) {
            return left._internalValue < right;
        }

        public static Boolean operator <( UBigInteger left, UBigInteger right ) {
            return left._internalValue < right._internalValue;
        }

        public static Boolean operator <( UBigInteger left, ulong right ) {
            return left._internalValue < right;
        }

        public static Boolean operator <( ulong left, UBigInteger right ) {
            return left < right._internalValue;
        }

        public static UBigInteger operator <<( UBigInteger number, int shift ) {
            return new UBigInteger( number._internalValue << shift );
        }

        public static Boolean operator <=( UBigInteger left, ulong right ) {
            return left._internalValue <= right;
        }

        public static Boolean operator <=( UBigInteger left, UBigInteger right ) {
            return left._internalValue <= right._internalValue;
        }

        public static Boolean operator >( UBigInteger left, long right ) {
            return left._internalValue > right;
        }

        public static Boolean operator >( UBigInteger left, UInt64 right ) {
            return left._internalValue > right;
        }

        public static Boolean operator >( UInt64 left, UBigInteger right ) {
            return left > right._internalValue;
        }

        public static Boolean operator >( UBigInteger left, UBigInteger right ) {
            return left._internalValue > right._internalValue;
        }

        public static Boolean operator >=( UBigInteger left, UInt64 right ) {
            return left._internalValue >= right;
        }

        public static Boolean operator >=( UBigInteger left, UBigInteger right ) {
            return left._internalValue >= right._internalValue;
        }

        public static UBigInteger Parse( [NotNull] string number, NumberStyles style ) {
            if ( number == null ) {
                throw new ArgumentNullException( "number" );
            }
            return new UBigInteger( value: BigInteger.Parse( number, style ) );
        }

        public static UBigInteger Pow( UBigInteger number, int exponent ) {
            return new UBigInteger( BigInteger.Pow( number._internalValue, exponent ) );
        }

        [Pure]
        public int CompareTo( long other ) {
            return this._internalValue.CompareTo( other );
        }

        [Pure]
        public int CompareTo( ulong other ) {
            return this._internalValue.CompareTo( other );
        }

        [Pure]
        public byte[] ToByteArray() {
            return this._internalValue.ToByteArray();
        }

        [Pure]
        public override string ToString() {
            return this._internalValue.ToString();
        }

        [Pure]
        public string ToString( string format ) {
            return this._internalValue.ToString( format );
        }

        //public static BigInteger Parse(string value)
        //{
        //    return new BigInteger(System.Numerics.BigInteger.Parse(value));
        //}
    }
}