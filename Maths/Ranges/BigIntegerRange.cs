// Copyright � 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "BigIntegerRange.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/BigIntegerRange.cs" was last cleaned by Protiguous on 2018/05/15 at 10:46 PM.

namespace Librainian.Maths.Ranges {

    using System;
    using System.Numerics;
    using Newtonsoft.Json;

    /// <summary>Represents a <see cref="BigInteger" /> range with minimum and maximum values.</summary>
    [JsonObject]
    public struct BigIntegerRange {

        /// <summary>Initializes a new instance of the <see cref="BigIntegerRange" /> class</summary>
        /// <param name="min">Minimum value of the range</param>
        /// <param name="max">Maximum value of the range</param>
        public BigIntegerRange( BigInteger min, BigInteger max ) {
            if ( min <= max ) {
                this.Min = min;
                this.Max = max;
            }
            else {
                this.Min = max;
                this.Max = min;
            }

            this.Length = this.Max - this.Min;
        }

        /// <summary>Length of the range (difference between maximum and minimum values).</summary>
        [JsonProperty]
        public BigInteger Length { get; }

        /// <summary>Maximum value</summary>
        [JsonProperty]
        public BigInteger Max { get; }

        /// <summary>Minimum value</summary>
        [JsonProperty]
        public BigInteger Min { get; }

        /// <summary>Check if the specified range is inside this range</summary>
        /// <param name="range">Range to check</param>
        /// <returns>
        ///     <b>True</b> if the specified range is inside this range or <b>false</b> otherwise.
        /// </returns>
        public Boolean IsInside( BigIntegerRange range ) => this.IsInside( range.Min ) && this.IsInside( range.Max );

        /// <summary>Check if the specified value is inside this range</summary>
        /// <param name="x">Value to check</param>
        /// <returns>
        ///     <b>True</b> if the specified value is inside this range or <b>false</b> otherwise.
        /// </returns>
        public Boolean IsInside( BigInteger x ) => this.Min <= x && x <= this.Max;

        /// <summary>Check if the specified range overlaps with this range</summary>
        /// <param name="range">Range to check for overlapping</param>
        /// <returns>
        ///     <b>True</b> if the specified range overlaps with this range or <b>false</b> otherwise.
        /// </returns>
        public Boolean IsOverlapping( BigIntegerRange range ) => this.IsInside( range.Min ) || this.IsInside( range.Max );
    }
}