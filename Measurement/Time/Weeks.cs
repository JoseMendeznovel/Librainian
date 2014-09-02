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
// "Librainian/Weeks.cs" was last cleaned by Rick on 2014/09/02 at 5:11 AM

#endregion License & Information

namespace Librainian.Measurement.Time {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Annotations;
    using FluentAssertions;
    using Parsing;

    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    public struct Weeks : IComparable<Weeks> {

        /// <summary>
        ///     52.4
        /// </summary>
        public const Decimal InOneCommonYear = 52.14m;

        /// <summary>
        ///     4.345
        /// </summary>
        public const Decimal InOneMonth = 4.345m;

        /// <summary>
        ///     One <see cref="Weeks" /> .
        /// </summary>
        public static readonly Weeks One = new Weeks( 1 );

        /// <summary>
        /// </summary>
        public static readonly Weeks Ten = new Weeks( 10 );

        /// <summary>
        /// </summary>
        public static readonly Weeks Thousand = new Weeks( 1000 );

        /// <summary>
        ///     Zero <see cref="Weeks" />
        /// </summary>
        public static readonly Weeks Zero = new Weeks( 0 );

        [DataMember]
        public readonly Decimal Value;

        static Weeks() {
            Zero.Should().BeLessThan( One );
            One.Should().BeGreaterThan( Zero );
            One.Should().Be( One );
            One.Should().BeLessThan( Months.One );
            One.Should().BeGreaterThan( Days.One );
        }

        public Weeks( Decimal weeks )
            : this() {
            this.Value = weeks;
        }

        public Weeks( long value ) {
            this.Value = value;
        }

        public Weeks( BigInteger value ) {
            value.ThrowIfOutOfDecimalRange();
            this.Value = ( Decimal )value;
        }

        [UsedImplicitly]
        private string DebuggerDisplay {
            get {
                return this.ToString();
            }
        }

        public static Weeks Combine( Weeks left, Weeks right ) {
            return new Weeks( left.Value + right.Value );
        }

        public static Weeks Combine( Weeks left, Decimal weeks ) {
            return new Weeks( left.Value + weeks );
        }

        public static Weeks Combine( Weeks left, BigInteger weeks ) {
            return new Weeks( ( BigInteger )left.Value + weeks );
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Weeks left, Weeks right ) {
            return left.Value == right.Value;
        }

        /// <summary>
        ///     Implicitly convert the number of <paramref name="weeks" /> to <see cref="Days" />.
        /// </summary>
        /// <param name="weeks"></param>
        /// <returns></returns>
        public static implicit operator Days( Weeks weeks ) {
            return weeks.ToDays();
        }

        public static implicit operator Months( Weeks weeks ) {
            return weeks.ToMonths();
        }

        public static implicit operator Span( Weeks weeks ) {
            return new Span( weeks: weeks.Value );
        }

        public static Weeks operator -( Weeks days ) {
            return new Weeks( days.Value * -1 );
        }

        public static Weeks operator -( Weeks left, Weeks right ) {
            return Combine( left: left, right: -right );
        }

        public static Boolean operator !=( Weeks left, Weeks right ) {
            return !Equals( left, right );
        }

        public static Weeks operator +( Weeks left, Weeks right ) {
            return Combine( left, right );
        }

        public static Weeks operator +( Weeks left, Decimal weeks ) {
            return Combine( left, weeks );
        }

        public static Weeks operator +( Weeks left, BigInteger weeks ) {
            return Combine( left, weeks );
        }

        public static Boolean operator <( Weeks left, Weeks right ) {
            return left.Value < right.Value;
        }

        public static Boolean operator <( Weeks left, Days right ) {
            return left < ( Weeks )right;
        }

        public static Boolean operator <( Weeks left, Months right ) {
            return ( Months )left < right;
        }

        public static Boolean operator ==( Weeks left, Weeks right ) {
            return Equals( left, right );
        }

        public static bool operator >( Weeks left, Months right ) {
            return ( Months )left > right;
        }

        public static Boolean operator >( Weeks left, Days right ) {
            return left > ( Weeks )right;
        }

        public static Boolean operator >( Weeks left, Weeks right ) {
            return left.Value > right.Value;
        }

        public int CompareTo( Weeks other ) {
            return this.Value.CompareTo( other.Value );
        }

        public Boolean Equals( Weeks other ) {
            return Equals( this, other );
        }

        public override Boolean Equals( object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Weeks && this.Equals( ( Weeks )obj );
        }

        [Pure]
        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        [Pure]
        public Days ToDays() {
            return new Days( this.Value * Days.InOneWeek );
        }

        [Pure]
        public Months ToMonths() {
            return new Months( this.Value / InOneMonth );
        }

        [Pure]
        public BigInteger ToPlanckTimes() {
            return BigInteger.Multiply( PlanckTimes.InOneWeek, new BigInteger( this.Value ) );
        }

        public override string ToString() {
            return String.Format( "{0} {1}", this.Value, this.Value.PluralOf( "week" ) );
        }
    }
}