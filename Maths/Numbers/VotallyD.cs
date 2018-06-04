﻿// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "VotallyD.cs" belongs to Rick@AIBrain.org and
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
// File "VotallyD.cs" was last formatted by Protiguous on 2018/06/04 at 4:06 PM.

namespace Librainian.Maths.Numbers {

	using System;
	using System.Diagnostics;
	using System.Threading;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	/// <summary>
	///     <para>Keep track of votes for candidate A and candidate B.</para>
	/// </summary>
	[JsonObject]
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	public class VotallyD : ICloneable {

		Object ICloneable.Clone() => this.Clone();

		public Double A {
			get => Thread.VolatileRead( ref this._aVotes );

			private set => Thread.VolatileWrite( ref this._aVotes, value );
		}

		public Double B {
			get => Thread.VolatileRead( ref this._bVotes );

			private set => Thread.VolatileWrite( ref this._bVotes, value );
		}

		public Double ChanceB {
			get {
				var votes = this.Votes;

				return votes.Near( 0 ) ? 0 : this.B / votes;
			}
		}

		public Boolean IsAWinning => this.A > this.B;

		public Boolean IsBWinning => this.B > this.A;

		public Boolean IsLandslideA => this.IsAWinning && this.A > this.HalfOfVotes();

		public Boolean IsProtiguous => this.IsTied() && this.Votes > 1;

		/// <summary>
		///     <see cref="A" /> + <see cref="B" />
		/// </summary>
		public Double Votes => this.A + this.B;

		/// <summary>ONLY used in the getter and setter.</summary>
		[JsonProperty]
		private Double _aVotes;

		/// <summary>ONLY used in the getter and setter.</summary>
		[JsonProperty]
		private Double _bVotes;

		public static VotallyD Combine( [NotNull] VotallyD left, [NotNull] VotallyD right ) {
			if ( left is null ) { throw new ArgumentNullException( nameof( left ) ); }

			if ( right is null ) { throw new ArgumentNullException( nameof( right ) ); }

			var result = left;
			result.ForA( right.A );
			result.ForB( right.B );

			return result;
		}

		public Double ChanceA() {
			var votes = this.Votes;

			return votes.Near( 0 ) ? 0 : this.A / votes;
		}

		/// <summary>
		///     <para>Increments the votes for candidate <see cref="A" /> by <paramref name="votes" />.</para>
		/// </summary>
		public void ForA( Double votes = 1 ) {
			this.A += votes;

			if ( this.A <= 0 ) { this.A = 0; }
		}

		/// <summary>
		///     <para>Increments the votes for candidate <see cref="B" /> by <paramref name="votes" />.</para>
		/// </summary>
		public void ForB( Double votes = 1 ) {
			this.B += votes;

			if ( this.B <= 0 ) { this.B = 0; }
		}

		public Double HalfOfVotes() => this.Votes / 2;

		public Boolean IsTied() => this.A.Near( this.B );

		public override String ToString() => $"A has {this.ChanceA():P1} and B has {this.ChanceB:P1} of {this.Votes:F1} votes.";

		/// <summary>
		///     <para>Increments the votes for candidate <see cref="A" /> by <paramref name="votes" />.</para>
		/// </summary>
		public void WithdrawVoteForA( Double votes = 1 ) {
			this.A -= votes;

			if ( this.A <= 0 ) { this.A = 0; }
		}

		/// <summary>
		///     <para>Increments the votes for candidate <see cref="B" /> by <paramref name="votes" />.</para>
		/// </summary>
		public void WithdrawVoteForB( Double votes = 1 ) {
			this.B -= votes;

			if ( this.B <= 0 ) { this.B = 0; }
		}

		public VotallyD Clone() => new VotallyD( votesForA: this.A, votesForB: this.B );

		/// <summary>No vote for either.</summary>
		public static readonly VotallyD Zero = new VotallyD( votesForA: 0, votesForB: 0 );

		public VotallyD( Double votesForA = 0, Double votesForB = 0 ) {
			this.A = votesForA;
			this.B = votesForB;
		}

	}

}