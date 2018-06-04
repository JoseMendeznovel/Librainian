﻿// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "DeserializeReportStats.cs" belongs to Rick@AIBrain.org and
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
// File "DeserializeReportStats.cs" was last formatted by Protiguous on 2018/06/04 at 4:21 PM.

namespace Librainian.Persistence {

	using System;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Magic;
	using Measurement.Time;
	using Threading;

	public sealed class DeserializeReportStats : ABetterClassDispose {

		public Boolean Enabled { get; set; }

		public TimeSpan Timing { get; }

		public Int64 Total { get; set; }

		private ThreadLocal<Int64> Gains { get; } = new ThreadLocal<Int64>( trackAllValues: true );

		private Action<DeserializeReportStats> Handler { get; }

		private ThreadLocal<Int64> Losses { get; } = new ThreadLocal<Int64>( trackAllValues: true );

		/// <summary>
		///     Perform a Report.
		/// </summary>
		private async Task Report() {
			if ( !this.Enabled ) { return; }

			var handler = this.Handler;

			if ( handler is null ) { return; }

			handler( this );

			if ( this.Enabled ) {
				await this.Timing.Then( async () => await this.Report() ); //TODO is this correct?
			}
		}

		public void AddFailed( Int64 amount = 1 ) => this.Losses.Value += amount;

		public void AddSuccess( Int64 amount = 1 ) => this.Gains.Value += amount;

		/// <summary>
		///     Dispose any disposable members.
		/// </summary>
		public override void DisposeManaged() {
			this.Gains.Dispose();
			this.Losses.Dispose();
		}

		public Int64 GetGains() => this.Gains.Values.Sum( arg => arg );

		public Int64 GetLoss() => this.Losses.Values.Sum( arg => arg );

		public async Task StartReporting() {
			this.Enabled = true;
			await this.Timing.Then( async () => await this.Report() );
		}

		public void StopReporting() => this.Enabled = false;

		public DeserializeReportStats( Action<DeserializeReportStats> handler, TimeSpan? timing = null ) {
			this.Gains.Values.Clear();
			this.Gains.Value = 0;

			this.Losses.Values.Clear();
			this.Losses.Value = 0;

			this.Total = 0;
			this.Handler = handler;
			this.Timing = timing ?? Milliseconds.ThreeHundredThirtyThree;
		}

	}

}