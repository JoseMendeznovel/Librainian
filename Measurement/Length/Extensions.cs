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
// "Librainian2/Extensions.cs" was last cleaned by Rick on 2014/08/08 at 2:28 PM
#endregion

namespace Librainian.Measurement.Length {
    using System;

    public static class Extensions {
        /// <summary>
        ///     How many <see cref="Millimeters" /> are in a single <see cref="Centimeters" /> ? (10)
        /// </summary>
        public const Decimal MillimetersInSingleCentimeter = 10m;

        /// <summary>
        ///     How many <see cref="Millimeters" /> are in a single <see cref="Inches" /> ? (25.4)
        /// </summary>
        public const Decimal MillimetersInSingleInch = 25.4m;

        /// <summary>
        ///     How many <see cref="Millimeters" /> are in a single <see cref="Meters" /> ? (1000)
        /// </summary>
        public const Decimal MillimetersInSingleMeter = CentimetersinSingleMeter*MillimetersInSingleCentimeter;

        /// <summary>
        ///     How many <see cref="Centimeters" /> are in a single <see cref="Meters" /> ? (100)
        /// </summary>
        public const Decimal CentimetersinSingleMeter = 100m;

        public static int Comparison( this Millimeters lhs, Millimeters rhs ) {
            return lhs.Value.CompareTo( rhs.Value );
        }

        public static int Comparison( this Millimeters millimeters, Centimeters centimeters ) {
            var lhs = new Centimeters( millimeters: millimeters ).Value; //upconvert. less likely to overflow.
            var rhs = centimeters.Value;
            return lhs.CompareTo( rhs );
        }
    }
}