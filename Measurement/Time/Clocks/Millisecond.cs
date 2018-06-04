// Copyright � 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Millisecond.cs" belongs to Rick@AIBrain.org and
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
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "Millisecond.cs" was last formatted by Protiguous on 2018/06/04 at 4:12 PM.

namespace Librainian.Measurement.Time.Clocks {

	using System;
	using System.Linq;
	using Extensions;
	using Newtonsoft.Json;

	/// <summary>A simple struct for a <see cref="Millisecond" />.</summary>
	[JsonObject]
	[Immutable]
	public struct Millisecond : IClockPart {

		/// <summary></summary>
		public static Millisecond Maximum { get; } = new Millisecond( MaximumValue );

		/// <summary>999</summary>
		public static UInt16 MaximumValue { get; } = ValidMilliseconds.Max();

		/// <summary></summary>
		public static Millisecond Minimum { get; } = new Millisecond( MinimumValue );

		/// <summary>0</summary>
		public static UInt16 MinimumValue { get; } = ValidMilliseconds.Min();

		public static readonly UInt16[] ValidMilliseconds = Enumerable.Range( 0, Milliseconds.InOneSecond ).Select( u => ( UInt16 ) u ).OrderBy( u => u ).ToArray();

		[JsonProperty]
		public readonly UInt16 Value;

		public Millisecond( UInt16 value ) {
			if ( !ValidMilliseconds.Contains( value ) ) { throw new ArgumentOutOfRangeException( nameof( value ), $"The specified value ({value}) is out of the valid range of {MinimumValue} to {MaximumValue}." ); }

			this.Value = value;
		}

		/// <summary>Allow this class to be visibly cast to an <see cref="Int16" />.</summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static explicit operator Int16( Millisecond value ) => ( Int16 ) value.Value;

		public static implicit operator Millisecond( UInt16 value ) => new Millisecond( value );

		public static implicit operator UInt16( Millisecond value ) => value.Value;

		/// <summary>Provide the next <see cref="Millisecond" />.</summary>
		public Millisecond Next( out Boolean ticked ) {
			ticked = false;
			var next = this.Value + 1;

			if ( next > MaximumValue ) {
				next = MinimumValue;
				ticked = true;
			}

			return ( UInt16 ) next;
		}

		/// <summary>Provide the previous <see cref="Millisecond" />.</summary>
		public Millisecond Previous( out Boolean ticked ) {
			ticked = false;
			var next = this.Value - 1;

			if ( next < MinimumValue ) {
				next = MaximumValue;
				ticked = true;
			}

			return ( UInt16 ) next;
		}

	}

}