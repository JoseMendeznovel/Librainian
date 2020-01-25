// Copyright � Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Second.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Second.cs" was last formatted by Protiguous on 2019/08/08 at 8:58 AM.

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Linq;
    using Extensions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>A simple struct for a <see cref="Second" />.</summary>
    [JsonObject]
    [Immutable]
    public sealed class Second : IClockPart {

        [JsonProperty]
        public SByte Value { get; }

        public static Second Maximum { get; } = new Second( MaxValue );

        public static Second Minimum { get; } = new Second( MinValue );

        public const SByte MaxValue = 59;

        public const SByte MinValue = 0;

        public static readonly Byte[] ValidSeconds = Enumerable.Range( 0, Seconds.InOneMinute ).Select( i => ( Byte ) i ).OrderBy( b => b ).ToArray();

        public Second( SByte value ) {
            if ( value < MinValue || value > MaxValue ) {
                throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinValue} to {MaxValue}." );
            }

            this.Value = value;
        }

        /// <summary>Allow this class to be read as a <see cref="Byte" />.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator Byte( [NotNull] Second value ) => ( Byte ) value.Value;

        /// <summary>Allow this class to be visibly cast to a <see cref="SByte" />.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator SByte( [NotNull] Second value ) => value.Value;

        [NotNull]
        public static implicit operator Second( SByte value ) => new Second( value );

        /// <summary>Provide the next second.</summary>
        [NotNull]
        public Second Next( out Boolean tocked ) {
            tocked = false;
            var next = this.Value + 1;

            if ( next > Maximum ) {
                next = Minimum;
                tocked = true;
            }

            return ( SByte ) next;
        }

        /// <summary>Provide the previous second.</summary>
        [NotNull]
        public Second Previous( out Boolean tocked ) {
            tocked = false;
            var next = this.Value - 1;

            if ( next < Minimum ) {
                next = Maximum;
                tocked = true;
            }

            return ( SByte ) next;
        }
    }
}