﻿#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/TimeTimers.cs" was last cleaned by Rick on 2014/08/21 at 4:30 PM
#endregion

namespace Librainian.Measurement.Time {
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Annotations;
    using NUnit.Framework;
    using Threading;

    [TestFixture]
    public static class TimeSystemTimers {
        private static ulong _threadingCounter;

        public static ulong RunThreadingTimerTest( Span howLong ) {
            _threadingCounter = 0;
            try {
                var state = new Object();
                using ( var threadingTimer = new Timer( callback: Callback, state: state, dueTime: ( int )Milliseconds.One.Value, period: ( int )Milliseconds.One.Value ) ) {
                    var stopwatch = Stopwatch.StartNew();
                    while ( stopwatch.Elapsed < howLong ) {
                        Tasks.DoNothing();
                    }
                    stopwatch.Stop();

                    var mills = howLong.GetApproximateMilliseconds();
                    var millsPer = mills / _threadingCounter;
                    Debug.WriteLine( "System.Threading.TimerTest counted to {0} in {1} ({2})", _threadingCounter, howLong, millsPer );
                }
            }
            catch { }
            return _threadingCounter;
        }

        private static void Callback( object state ) {
            _threadingCounter++;
        }

        [Test, UsedImplicitly]
        public static void RunTests() {
            Console.WriteLine( RunSystemTimerTest( Milliseconds.OneThousandNine ) );
            Console.WriteLine( RunThreadingTimerTest( Milliseconds.OneThousandNine ) );
        }

        public static ulong RunSystemTimerTest( Span howLong ) {
            var counter = 0UL;
            try {
                using ( var systemTimer = new System.Timers.Timer( ( double ) Milliseconds.One ) { AutoReset = true } ) {
                    systemTimer.Elapsed += ( sender, args ) => { counter++; };

                    systemTimer.Start();
                    var stopwatch = Stopwatch.StartNew();
                    while ( stopwatch.Elapsed < howLong ) {
                        Tasks.DoNothing();
                    }

                    stopwatch.Stop();
                    systemTimer.Stop();

                    var mills = howLong.GetApproximateMilliseconds();
                    var millsPer = mills / counter;
                    Debug.WriteLine( "System.Timer.TimerTest counted to {0} in {1} ({2})", counter, howLong, millsPer );
                }
            }
            catch ( Exception ) { }
            return counter;
        }
    }
}