// Copyright � 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Extensions.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/Extensions.cs" was last formatted by Protiguous on 2018/05/24 at 7:26 PM.

namespace Librainian.Measurement.Length {

    using System;

    public static class Extensions {

        /// <summary>
        ///     How many <see cref="Centimeters" /> are in a single <see cref="Meters" /> ? (100)
        /// </summary>
        public const Decimal CentimetersinSingleMeter = 100m;

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
        public const Decimal MillimetersInSingleMeter = CentimetersinSingleMeter * MillimetersInSingleCentimeter;

        public static Int32 Comparison( this Millimeters left, Millimeters rhs ) => left.Value.CompareTo( rhs.Value );

        //public static Int32 Comparison( this Millimeters millimeters, Centimeters centimeters ) {
        //    var left = new Centimeters( millimeters: millimeters ).Value; //upconvert. less likely to overflow.
        //    var rhs = centimeters.Value;
        //    return left.CompareTo( rhs );
        //}
    }
}