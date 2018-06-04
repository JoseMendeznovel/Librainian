// Copyright � 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Fuzzy.cs" belongs to Rick@AIBrain.org and
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
// File "Fuzzy.cs" was last formatted by Protiguous on 2018/06/04 at 4:03 PM.

namespace Librainian.Maths {

	using System;
	using Collections;
	using JetBrains.Annotations;
	using Newtonsoft.Json;
	using Numbers;

	public enum LowMiddleHigh {

		Low,

		Middle,

		High

	}

	/// <summary>
	///     A Double number, constrained between 0 and 1. Kinda thread-safe by Interlocked
	/// </summary>
	[JsonObject]
	public struct Fuzzy : ICloneable {

		/// <summary>
		///     ~25 to 75% probability.
		/// </summary>
		private static readonly PairOfDoubles Undecided = new PairOfDoubles( low: Combine( MinValue, HalfValue ), high: Combine( HalfValue, MaxValue ) );

		/// <summary>
		///     ONLY used in the getter and setter.
		/// </summary>
		[JsonProperty]
		private AtomicDouble _value;

		public Double Value {
			get => this._value;

			set {
				if ( value > MaxValue ) { value = MaxValue; }
				else if ( value < MinValue ) { value = MinValue; }

				this._value.Value = value;
			}
		}

		public const Double HalfValue = ( MinValue + MaxValue ) / 2D;

		/// <summary>
		///     1
		/// </summary>
		public const Double MaxValue = 1D;

		/// <summary>
		///     0
		/// </summary>
		public const Double MinValue = 0D;

		public static readonly Fuzzy Empty;

		//private static readonly Fuzzy Truer = Fuzzy.Combine( Undecided, MaxValue );
		//private static readonly Fuzzy Falser = Fuzzy.Combine( Undecided, MinValue );
		//private static readonly Fuzzy UndecidedUpper = Combine( Undecided, Truer);
		//private static readonly Fuzzy UndecidedLower = Combine( Undecided, Falser );
		/// <summary>
		///     Initializes a random number between 0 and 1
		/// </summary>
		public Fuzzy( Double? value = null ) : this() {
			if ( value.HasValue ) { this.Value = value.Value; }
			else { this.Randomize(); }
		}

		public static Double Combine( Double left, Double rhs ) {
			if ( !left.IsNumber() ) { throw new ArgumentOutOfRangeException( nameof( left ) ); }

			if ( !rhs.IsNumber() ) { throw new ArgumentOutOfRangeException( nameof( rhs ) ); }

			return ( left + rhs ) / 2D;
		}

		public static Fuzzy Parse( [CanBeNull] String value ) {
			if ( String.IsNullOrWhiteSpace( value ) ) { throw new ArgumentNullException( nameof( value ) ); }

			if ( Double.TryParse( value, out var result ) ) { return new Fuzzy( result ); }

			return Empty;
		}

		public void AdjustTowardsMax() => this.Value = ( this.Value + MaxValue ) / 2D;

		public void AdjustTowardsMin() => this.Value = ( this.Value + MinValue ) / 2D;

		public Object Clone() => new Fuzzy( this.Value );

		//public Boolean IsUndecided( Fuzzy anotherFuzzy ) { return !IsTruer( anotherFuzzy ) && !IsFalser( anotherFuzzy ); }
		public Boolean IsFalseish() => this.Value < Undecided.Low;

		public Boolean IsTrueish() => this.Value > Undecided.High;

		public Boolean IsUndecided() => !this.IsTrueish() && !this.IsFalseish();

		/// <summary>
		///     Initializes a random number between 0 and 1 within a range, defaulting to Middle range (~0.50)
		/// </summary>
		public void Randomize( LowMiddleHigh? lmh = LowMiddleHigh.Middle ) {
			switch ( lmh ) {
				case null:
					this.Value = Randem.NextDouble();

					break;

				default:

					switch ( lmh.Value ) {
						case LowMiddleHigh.Low:

							do { this.Value = Randem.NextDouble( 0.0D, 0.25D ); } while ( this.Value < MinValue || this.Value > 0.25D );

							break;

						case LowMiddleHigh.Middle:

							do { this.Value = Randem.NextDouble( 0.25D, 0.75D ); } while ( this.Value < 0.25D || this.Value > 0.75D );

							break;

						case LowMiddleHigh.High:

							do { this.Value = Randem.NextDouble(); } while ( this.Value < 0.75D || this.Value > MaxValue );

							break;

						default:
							this.Value = Randem.NextDouble();

							break;
					}

					break;
			}
		}

		public override String ToString() => $"{this.Value:R}";

	}

}