// Copyright � 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ZeroToOne.cs" belongs to Rick@AIBrain.org and
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
// "Librainian/Librainian/ZeroToOne.cs" was last formatted by Protiguous on 2018/05/24 at 7:23 PM.

namespace Librainian.Maths.Numbers {

    using System;
    using System.Diagnostics;
    using Extensions;
    using Newtonsoft.Json;

    /// <summary>
    ///     Restricts the value to between 0.0 and 1.0
    ///     <para>Uses the <see cref="float" /> type.</para>
    /// </summary>
    [Immutable]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject( memberSerialization: MemberSerialization.Fields )]
    public class ZeroToOne {

        /// <summary>
        ///     ONLY used in the getter and setter.
        /// </summary>
        [JsonProperty( "v" )]
        private volatile Single _value;

        public const Single MaxValue = 1f;

        public const Single MinValue = 0f;

        public const Single NeutralValue = MaxValue / 2.0f;

        public Single Value {
            get => this._value;

            set => this._value = value > MaxValue ? MaxValue : ( value < MinValue ? MinValue : value );
        }

        private ZeroToOne( Double value ) : this() => this.Value = ( Single )( value > MaxValue ? MaxValue : ( value < MinValue ? MinValue : value ) );

        private ZeroToOne( Single value ) : this( ( Single? )value ) { }

        /// <summary>
        ///     <para>Restricts the value to between 0.0 and 1.0.</para>
        ///     <para>If null is given, a random value (between 0.0 and 1.0) will be assigned.</para>
        /// </summary>
        /// <param name="value"></param>
        public ZeroToOne( Single? value = null ) {
            if ( !value.HasValue ) { value = Randem.NextSingle( min: MinValue, max: MaxValue ); }

            this.Value = value.Value;
        }

        /// <summary>
        ///     Return a new <see cref="ZeroToOne" /> with the value of <paramref name="value1" /> moved closer to the value of
        ///     <paramref name="value2" /> .
        /// </summary>
        /// <param name="value1">The current value.</param>
        /// <param name="value2">The value to move closer towards.</param>
        /// <returns>
        ///     Returns a new <see cref="ZeroToOne" /> with the value of <paramref name="value1" /> moved closer to the value
        ///     of <paramref name="value2" /> .
        /// </returns>
        public static ZeroToOne Combine( ZeroToOne value1, ZeroToOne value2 ) => new ZeroToOne( ( value1 + value2 ) / 2f );

        public static implicit operator Double( ZeroToOne special ) => special.Value;

        public static implicit operator Single( ZeroToOne special ) => special.Value;

        public static implicit operator ZeroToOne( Single value ) => new ZeroToOne( value );

        public static implicit operator ZeroToOne( Double value ) => new ZeroToOne( value );

        public static ZeroToOne Parse( String value ) => new ZeroToOne( Single.Parse( s: value ) );

        public override String ToString() => $"{this.Value:P}";
    }
}