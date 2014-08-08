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
// "Librainian2/Millimeters.cs" was last cleaned by Rick on 2014/08/08 at 2:29 PM
#endregion

namespace Librainian.Measurement.Length {
    using System;
    using System.Runtime.Serialization;

    [DataContract( IsReference = true )]
    public struct Millimeters {
        /// <summary>
        ///     One <see cref="Millimeters" /> .
        /// </summary>
        public static readonly Millimeters One = new Millimeters( millimeters: 1 );

        /// <summary>
        ///     Two <see cref="Millimeters" /> .
        /// </summary>
        public static readonly Millimeters Two = new Millimeters( millimeters: 2 );

        /// <summary>
        ///     About zero. :P
        /// </summary>
        public static readonly Millimeters MinValue = new Millimeters( millimeters: Decimal.MinValue );

        /// <summary>
        ///     About 584.9 million years.
        /// </summary>
        public static readonly Millimeters MaxValue = new Millimeters( millimeters: Decimal.MaxValue );

        [DataMember] public readonly Decimal Value;

        static Millimeters() {
            //Assert.That( One < Centimeter.One );
            //Assert.That( One < Inch.One );
            //Assert.That( One < Foot.One );
        }

        public Millimeters( Decimal millimeters ) {
            this.Value = millimeters;
        }

        public Millimeters( Centimeters centimeters ) {
            var val = centimeters.Value*Extensions.MillimetersInSingleCentimeter;
            this.Value = val < MinValue.Value ? MinValue.Value : ( val > MaxValue.Value ? MaxValue.Value : val );
        }

        public Millimeters( Meters meters ) {
            var val = meters.Value*Extensions.MillimetersInSingleMeter;
            this.Value = val < MinValue.Value ? MinValue.Value : ( val > MaxValue.Value ? MaxValue.Value : val );
        }

        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        //public static Boolean operator <( Millimeter lhs, Second rhs ) { return lhs.Comparison( rhs ) < 0; }

        //public static Boolean operator >( Millimeter lhs, Second rhs ) { return lhs.Comparison( rhs ) > 0; }

        //public static Boolean operator <( Millimeter lhs, Minute rhs ) { return lhs.Comparison( rhs ) < 0; }

        //public static Boolean operator >( Millimeter lhs, Minute rhs ) { return lhs.Comparison( rhs ) > 0; } 

        //public static implicit operator TimeSpan( Millimeter milliseconds ) {
        //    return TimeSpan.FromMilliseconds( value: milliseconds.Value );
        //}
    }
}