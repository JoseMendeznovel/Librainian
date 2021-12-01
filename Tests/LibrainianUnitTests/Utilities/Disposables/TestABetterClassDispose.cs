﻿// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "TestABetterClassDispose.cs" last touched on 2021-11-30 at 7:23 PM by Protiguous.

namespace LibrainianUnitTests.Utilities.Disposables;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Librainian;
using Librainian.Maths;
using Librainian.Measurement.Time;
using Librainian.Utilities.Disposables;
using NUnit.Framework;

[TestFixture]
public class TestABetterClassDispose {

	private static Int32 N => MathConstants.Sizes.OneMegaByte;

	private static Stopwatch SloppyBenchmarker { get; } = Stopwatch.StartNew();

	private static void ForceGC() => GC.Collect( 2, GCCollectionMode.Forced, true );

	[Test]
	public void TestDisposeScopedDispose() {
		ForceGC();

		SloppyBenchmarker.Restart();
		foreach ( var i in 1.To( N ) ) {
			using var testAbcd = new TestABCD( i );
		}

		GC.WaitForPendingFinalizers();

		SloppyBenchmarker.Stop();

		TestContext.WriteLine( $"Test {nameof( this.TestDisposeScopedDispose )} took {SloppyBenchmarker.Elapsed.Simpler()}" );
	}

	[Test]
	public void TestDisposeStatementWithDispose() {
		ForceGC();
		SloppyBenchmarker.Restart();

		foreach ( var i in 1.To( N ) ) {
			using var testAbcd = new TestABCD( i );
			testAbcd.DisposeManaged();
		}

		GC.WaitForPendingFinalizers();

		SloppyBenchmarker.Stop();

		TestContext.WriteLine( $"Test {nameof( this.TestDisposeStatementWithDispose )} took {SloppyBenchmarker.Elapsed.Simpler()}" );
	}

	[Test]
	[SuppressMessage( "ReSharper", "ConvertToUsingDeclaration" )]
	public void TestDisposeUsingStatement() {
		ForceGC();
		SloppyBenchmarker.Restart();
		foreach ( var i in 1.To( N ) ) {
			using ( var testAbcd = new TestABCD( i ) ) {
				testAbcd.DisposeManaged();
			}
		}

		GC.WaitForPendingFinalizers();
		SloppyBenchmarker.Stop();

		TestContext.WriteLine( $"Test {nameof( this.TestDisposeUsingStatementWithoutDispose )} took {SloppyBenchmarker.Elapsed.Simpler()}" );
	}

	[Test]
	[SuppressMessage( "ReSharper", "ConvertToUsingDeclaration" )]
	public void TestDisposeUsingStatementWithoutDispose() {
		ForceGC();
		SloppyBenchmarker.Restart();
		foreach ( var i in 1.To( N ) ) {
			using ( var testAbcd = new TestABCD( i ) ) {
				testAbcd.Nop();
			}
		}

		GC.WaitForPendingFinalizers();
		SloppyBenchmarker.Stop();

		TestContext.WriteLine( $"Test {nameof( this.TestDisposeUsingStatementWithoutDispose )} took {SloppyBenchmarker.Elapsed.Simpler()}" );
	}

	public class TestABCD : ABetterClassDispose {

		private readonly Int32 _value;

		public TestABCD( Int32 val ) : base( nameof( TestABCD ) ) => this._value = val;

		/// <summary>Dispose of any <see cref="IDisposable" /> (managed) fields or properties in this method.</summary>
		public override void DisposeManaged() {
			if ( this._value % 128 == 0 ) {
				this._value.Nop();
			}
		}
	}
}