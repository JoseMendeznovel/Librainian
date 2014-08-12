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
// "Librainian/TickingClock.cs" was last cleaned by Rick on 2014/08/11 at 12:39 AM
#endregion

namespace Librainian.Measurement.Time.Clocks {
    using System;
    using System.Timers;
    using Annotations;

    /// <summary>
    ///     <para>Starts a forward-ticking clock at the given time with settable events.</para>
    ///     <para>Should be threadsafe.</para>
    ///     <para>
    ///         Settable events are:
    ///         <para>
    ///             <see cref="OnHourTick" />
    ///         </para>
    ///         <para>
    ///             <see cref="OnMinuteTick" />
    ///         </para>
    ///         <para>
    ///             <see cref="OnSecondTick" />
    ///         </para>
    ///     </para>
    /// </summary>
    public class TickingClock : IStandardClock {
        /// <summary>
        /// </summary>
        [CanBeNull]
        private Timer _timer;

        public enum Granularity {
            Milliseconds, Seconds, Minutes, Hours
        }

        public TickingClock( DateTime time, Granularity granularity = Granularity.Seconds ) {
            this.Hour = new Hour( time.Hour );
            this.Minute = new Minute( time.Minute );
            this.Second = new Second( time.Second );
            this.ResetTimer();
        }

        public TickingClock( Time time, Granularity granularity = Granularity.Seconds ) {
            this.Hour = new Hour( time.Hour );
            this.Minute = new Minute( time.Minute );
            this.Second = new Second( time.Second );
            this.ResetTimer();
        }

        [CanBeNull]
        public Action OnHourTick { get; set; }

        [CanBeNull]
        public Action OnMinuteTick { get; set; }

        [CanBeNull]
        public Action OnSecondTick { get; set; }

        [CanBeNull]
        public Action OnMillisecondTick { get; set; }

        /// <summary>
        /// </summary>
        public Hour Hour { get; private set; }

        /// <summary>
        /// </summary>
        public Minute Minute { get; private set; }

        /// <summary>
        /// </summary>
        public Second Second { get; private set; }

        /// <summary>
        /// </summary>
        public Millisecond Millisecond { get; private set; }

        public void ResetTimer( Granularity granularity ) {
            if ( null != this._timer ) {
                using ( this._timer ) {
                    this._timer.Stop();
                }
            }
            switch ( granularity ) {
                case Granularity.Milliseconds:
                    this._timer = new Timer( interval: ( Double )Milliseconds.One.Value ) { AutoReset = true };
                    this._timer.Elapsed += this.OnMillisecondElapsed;
                    break;
                case Granularity.Seconds:
                    this._timer = new Timer( interval: ( Double )Seconds.One.Value ) { AutoReset = true };
                    this._timer.Elapsed += this.OnSecondElapsed;
                    break;
                case Granularity.Minutes:
                    this._timer = new Timer( interval: ( Double )Minutes.One.Value ) { AutoReset = true };
                    this._timer.Elapsed += this.OnMinuteElapsed;
                    break;
                case Granularity.Hours:
                    this._timer = new Timer( interval: ( Double )Hours.One.Value ) { AutoReset = true };
                    this._timer.Elapsed += this.OnHourElapsed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException( "granularity" );
            }

            this._timer.Start();
        }


        //These functions can be collasped I think.
        private void OnMillisecondElapsed( object sender, ElapsedEventArgs e ) {
            Boolean ticked;

            this.Millisecond = this.Millisecond.Next( out ticked );
            if ( !ticked ) {
                return;
            }

            var onMillisecondTick = this.OnMillisecondTick;
            if ( onMillisecondTick != null ) {
                onMillisecondTick();
            }

            this.Second = this.Second.Next( out ticked );
            if ( !ticked ) {
                return;
            }

            var onSecondTick = this.OnSecondTick;
            if ( onSecondTick != null ) {
                onSecondTick();
            }

            this.Minute = this.Minute.Next( out ticked );
            if ( !ticked ) {
                return;
            }

            var onMinuteTick = this.OnMinuteTick;
            if ( onMinuteTick != null ) {
                onMinuteTick();
            }

            this.Hour = this.Hour.Next( out ticked );
            if ( !ticked ) {
                return;
            }

            var onHourTick = this.OnHourTick;
            if ( onHourTick != null ) {
                onHourTick();
            }
        }

        private void OnSecondElapsed( object sender, ElapsedEventArgs e ) {
            Boolean ticked;

            this.Second = this.Second.Next( out ticked );
            if ( !ticked ) {
                return;
            }

            var onSecondTick = this.OnSecondTick;
            if ( onSecondTick != null ) {
                onSecondTick();
            }

            this.Minute = this.Minute.Next( out ticked );
            if ( !ticked ) {
                return;
            }

            var onMinuteTick = this.OnMinuteTick;
            if ( onMinuteTick != null ) {
                onMinuteTick();
            }

            this.Hour = this.Hour.Next( out ticked );
            if ( !ticked ) {
                return;
            }

            var onHourTick = this.OnHourTick;
            if ( onHourTick != null ) {
                onHourTick();
            }
        }

        private void OnMinuteElapsed( object sender, ElapsedEventArgs e ) {
            Boolean ticked;

            this.Minute = this.Minute.Next( out ticked );
            if ( !ticked ) {
                return;
            }

            var onMinuteTick = this.OnMinuteTick;
            if ( onMinuteTick != null ) {
                onMinuteTick();
            }

            this.Hour = this.Hour.Next( out ticked );
            if ( !ticked ) {
                return;
            }

            var onHourTick = this.OnHourTick;
            if ( onHourTick != null ) {
                onHourTick();
            }
        }

        private void OnHourElapsed( object sender, ElapsedEventArgs e ) {

            Boolean ticked;

            this.Hour = this.Hour.Next( out ticked );
            if ( !ticked ) {
                return;
            }

            var onHourTick = this.OnHourTick;
            if ( onHourTick != null ) {
                onHourTick();
            }
        }

        public Boolean IsAM() {
            return !this.IsPM();
        }

        public Boolean IsPM() {
            return this.Hour.Value >= 12;
        }

        public Time GetTime() {
            try {
                var timer = this._timer;
                if ( timer != null ) {
                    timer.Stop(); //stop the timer so the seconds don't tick while we get the values.
                }
                return new Time( hour: this.Hour.Value, minute: this.Minute.Value, second: this.Second.Value );
            }
            finally {
                var timer = this._timer;
                if ( timer != null ) {
                    timer.Start();
                }
            }
        }
    }
}
