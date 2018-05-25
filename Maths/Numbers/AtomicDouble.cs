// Copyright � 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "AtomicDouble.cs" belongs to Rick@AIBrain.org and
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
// "Librainian/Librainian/AtomicDouble.cs" was last formatted by Protiguous on 2018/05/24 at 7:21 PM.

namespace Librainian.Maths.Numbers {

    using System;
    using System.ComponentModel;
    using System.Threading;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>A Double number thread-safe by <see cref="Interlocked" />.</summary>
    [JsonObject]
    [Description( "A Double number thread-safe by Interlocked." )]
    public struct AtomicDouble {

        /// <summary>ONLY used in the getter and setter.</summary>
        [JsonProperty]
        private Double _value;

        public Double Value {
            get => Interlocked.Exchange( ref this._value, this._value );

            set => Interlocked.Exchange( ref this._value, value );
        }

        public AtomicDouble( Double value ) : this() => this.Value = value;

        public static implicit operator Double( AtomicDouble special ) => special.Value;

        public static AtomicDouble operator -( AtomicDouble a1, AtomicDouble a2 ) => new AtomicDouble( a1.Value - a2.Value );

        public static AtomicDouble operator *( AtomicDouble a1, AtomicDouble a2 ) => new AtomicDouble( a1.Value * a2.Value );

        public static AtomicDouble operator +( AtomicDouble a1, AtomicDouble a2 ) => new AtomicDouble( a1.Value + a2.Value );

        public static AtomicDouble operator ++( AtomicDouble a1 ) {
            a1.Value++;

            return a1;
        }

        public static AtomicDouble Parse( [NotNull] String value ) {
            if ( value is null ) { throw new ArgumentNullException( nameof( value ) ); }

            return new AtomicDouble( Double.Parse( value ) );
        }

        /// <summary>Resets the value to zero if less than zero;</summary>
        public void CheckReset() {
            if ( this.Value < 0 ) { this.Value = 0; }
        }

        public override String ToString() => $"{this.Value:R}";
    }
}