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
// "Librainian2/Span.cs" was last cleaned by Rick on 2014/08/08 at 2:30 PM
#endregion

namespace Librainian.Measurement.Time {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Annotations;
    using Collections;
    using FluentAssertions;
    using Librainian.Extensions;
    using Maths;
    using Parsing;

    /// <summary>
    ///     <para><see cref="Span" /> represents the smallest (0) to large(!) duration of time.</para>
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Units_of_time" />
    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Serializable]
    [Immutable]
    public struct Span : IEquatable< Span >, IComparable< Span >, IComparable< TimeSpan > {
        /// <summary>
        /// </summary>
        public static readonly Span Zero = new Span( planckTimes: 0 );

        /// <summary>
        /// </summary>
        public static readonly Span MatrixID = new Span( 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 );

        public static readonly Span Infinity = new Span( milleniums: Decimal.MaxValue ); //not the largest Span possible, but anything larger.. wow. just wow.
        public static readonly Span Forever = new Span( years: Decimal.MaxValue ); //not the largest Span possible, but anything larger.. wow. just wow.

        /// <summary>
        /// </summary>
        public readonly Attoseconds Attoseconds;

        /// <summary>
        ///     How many <seealso cref="Centuries" /> does this <seealso cref="Span" /> span?
        /// </summary>
        public readonly Centuries Centuries;

        /// <summary>
        ///     How many <seealso cref="Days" /> does this <seealso cref="Span" /> span?
        /// </summary>
        public readonly Days Days;

        /// <summary>
        /// </summary>
        public readonly Femtoseconds Femtoseconds;

        /// <summary>
        ///     How many <seealso cref="Hours" /> does this <seealso cref="Span" /> span?
        /// </summary>
        public readonly Hours Hours;

        //[UsedImplicitly]
        //public readonly Boolean IsNormalized;

        /// <summary>
        ///     <para>A microsecond is an SI unit of time equal to one millionth (10−6 or 1/1,000,000) of a second.</para>
        ///     <para>Its symbol is μs.</para>
        /// </summary>
        /// <trivia>One microsecond is to one second as one second is to 11.574 days.</trivia>
        public readonly Microseconds Microseconds;

        /// <summary>
        ///     A millennium (plural millennia) is a period of time equal to one thousand years.
        /// </summary>
        /// <see cref="http://wikipedia.org/wiki/Millennium" />
        public readonly Milleniums Milleniums;

        /// <summary>
        ///     How many <seealso cref="Milliseconds" /> does this <seealso cref="Span" /> span?
        /// </summary>
        public readonly Milliseconds Milliseconds;

        /// <summary>
        ///     How many <seealso cref="Minutes" /> does this <seealso cref="Span" /> span?
        /// </summary>
        public readonly Minutes Minutes;

        /// <summary>
        ///     How many <seealso cref="Months" /> does this <seealso cref="Span" /> span?
        /// </summary>
        public readonly Months Months;

        /// <summary>
        /// </summary>
        public readonly Nanoseconds Nanoseconds;

        /// <summary>
        ///     A picosecond is an SI unit of time equal to 10E−12 of a second.
        /// </summary>
        /// <see cref="http://wikipedia.org/wiki/Picosecond" />
        public readonly Picoseconds Picoseconds;

        /// <summary>
        /// </summary>
        public readonly PlanckTimes PlanckTimes;

        /// <summary>
        ///     How many <seealso cref="Seconds" /> does this <seealso cref="Span" /> span?
        /// </summary>
        public readonly Seconds Seconds;

        /// <summary>
        ///     How many <seealso cref="Weeks" /> does this <seealso cref="Span" /> span?
        /// </summary>
        public readonly Weeks Weeks;

        /// <summary>
        ///     How many <seealso cref="Years" /> does this <seealso cref="Span" /> span?
        /// </summary>
        public readonly Years Years;

        /// <summary>
        /// </summary>
        public readonly Yoctoseconds Yoctoseconds;

        /// <summary>
        /// </summary>
        public readonly Zeptoseconds Zeptoseconds;

        /// <summary>
        ///     <para>This value is not calculated until needed.</para>
        /// </summary>
        private readonly Lazy< BigInteger > _lazyTotal;

        public Span( BigInteger planckTimes ) : this() {
            planckTimes.Should().BeGreaterOrEqualTo( BigInteger.Zero );

            if ( planckTimes < BigInteger.Zero ) {
                throw new ArgumentOutOfRangeException( "planckTimes", "Must be greater than or equal to 0" );
            }

            //NOTE the order here is mostly important. I think? maybe not. oh well.
            this.Milleniums = new Milleniums( PlanckTimes.InOneMillenium.PullPlancks( ref planckTimes ) );
            this.Centuries = new Centuries( PlanckTimes.InOneCentury.PullPlancks( ref planckTimes ) );
            this.Years = new Years( PlanckTimes.InOneYear.PullPlancks( ref planckTimes ) );
            this.Months = new Months( PlanckTimes.InOneMonth.PullPlancks( ref planckTimes ) );
            this.Weeks = new Weeks( PlanckTimes.InOneWeek.PullPlancks( ref planckTimes ) );
            this.Days = new Days( PlanckTimes.InOneDay.PullPlancks( ref planckTimes ) );
            this.Hours = new Days( PlanckTimes.InOneDay.PullPlancks( ref planckTimes ) );
            this.Minutes = new Minutes( PlanckTimes.InOneMinute.PullPlancks( ref planckTimes ) );
            this.Seconds = new Seconds( PlanckTimes.InOneSecond.PullPlancks( ref planckTimes ) );
            this.Milliseconds = new Milliseconds( PlanckTimes.InOneMillisecond.PullPlancks( ref planckTimes ) );
            this.Microseconds = new Microseconds( PlanckTimes.InOneMicrosecond.PullPlancks( ref planckTimes ) );
            this.Nanoseconds = new Nanoseconds( PlanckTimes.InOneNanosecond.PullPlancks( ref planckTimes ) );
            this.Picoseconds = new Picoseconds( PlanckTimes.InOnePicosecond.PullPlancks( ref planckTimes ) );
            this.Femtoseconds = new Femtoseconds( PlanckTimes.InOneFemtosecond.PullPlancks( ref planckTimes ) );
            this.Attoseconds = new Femtoseconds( PlanckTimes.InOneAttosecond.PullPlancks( ref planckTimes ) );
            this.Zeptoseconds = new Zeptoseconds( PlanckTimes.InOneZeptosecond.PullPlancks( ref planckTimes ) );
            this.Yoctoseconds = new Yoctoseconds( PlanckTimes.InOneYoctosecond.PullPlancks( ref planckTimes ) );

            planckTimes.ThrowIfOutOfDecimalRange();
            this.PlanckTimes += planckTimes;

            var tmpThis = this;
            this._lazyTotal = new Lazy< BigInteger >( valueFactory: () => tmpThis.CalcTotalPlanckTimes(), isThreadSafe: true );
        }

        /// <summary>
        ///     <para>
        ///         Negative parameters passed to this constructor will interpret as zero instead of throwing an
        ///         <see cref="ArgumentOutOfRangeException" />.
        ///     </para>
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <param name="normalize"></param>
        public Span( TimeSpan timeSpan, Boolean normalize = true ) : this( milliseconds: timeSpan.Milliseconds, seconds: timeSpan.Seconds, minutes: timeSpan.Minutes, hours: timeSpan.Hours, days: timeSpan.Days, normalize: normalize ) { }

        /// <summary>
        ///     <para>
        ///         Negative parameters passed to this constructor will interpret as zero instead of throwing an
        ///         <see cref="ArgumentOutOfRangeException" />.
        ///     </para>
        /// </summary>
        /// <param name="femtoseconds"></param>
        /// <param name="picoseconds"></param>
        /// <param name="attoseconds"></param>
        /// <param name="nanoseconds"></param>
        /// <param name="microseconds"></param>
        /// <param name="milliseconds"></param>
        /// <param name="seconds"></param>
        /// <param name="minutes"></param>
        /// <param name="hours"></param>
        /// <param name="days"></param>
        /// <param name="weeks"></param>
        /// <param name="months"></param>
        /// <param name="years"></param>
        /// <param name="centuries"></param>
        /// <param name="milleniums"></param>
        /// <param name="normalize"></param>
        /// <param name="yoctoseconds"></param>
        /// <param name="zeptoseconds"></param>
        public Span( /*PlanckTimes planckTimes = default( PlanckTimes ),*/ Yoctoseconds yoctoseconds = default( Yoctoseconds ), Zeptoseconds zeptoseconds = default( Zeptoseconds ), Attoseconds attoseconds = default( Attoseconds ), Femtoseconds femtoseconds = default( Femtoseconds ), Picoseconds picoseconds = default( Picoseconds ), Nanoseconds nanoseconds = default( Nanoseconds ), Microseconds microseconds = default( Microseconds ), Milliseconds milliseconds = default( Milliseconds ), Seconds seconds = default( Seconds ), Minutes minutes = default( Minutes ), Hours hours = default( Hours ), Days days = default( Days ), Weeks weeks = default( Weeks ), Months months = default( Months ), Years years = default( Years ), Centuries centuries = default( Centuries ), Milleniums milleniums = default( Milleniums ), Boolean normalize = true ) : this( /*planckTimes: planckTimes.Value,*/ yoctoseconds: yoctoseconds.Value, zeptoseconds: zeptoseconds.Value, attoseconds: attoseconds.Value, femtoseconds: femtoseconds.Value, picoseconds: picoseconds.Value, nanoseconds: nanoseconds.Value, microseconds: microseconds.Value, milliseconds: milliseconds.Value, seconds: seconds.Value, minutes: minutes.Value, hours: hours.Value, days: days.Value, weeks: weeks.Value, months: months.Value, years: years.Value, centuries: centuries.Value, milleniums: milleniums.Value, normalize: normalize ) { }

        /// <summary>
        ///     <para>
        ///         Negative parameters passed to this constructor will interpret as zero instead of throwing an
        ///         <see cref="ArgumentOutOfRangeException" />.
        ///     </para>
        /// </summary>
        /// <param name="yoctoseconds"></param>
        /// <param name="zeptoseconds"></param>
        /// <param name="femtoseconds"></param>
        /// <param name="picoseconds"></param>
        /// <param name="attoseconds"></param>
        /// <param name="nanoseconds"></param>
        /// <param name="microseconds"></param>
        /// <param name="milliseconds"></param>
        /// <param name="seconds"></param>
        /// <param name="minutes"></param>
        /// <param name="hours"></param>
        /// <param name="days"></param>
        /// <param name="weeks"></param>
        /// <param name="months"></param>
        /// <param name="years"></param>
        /// <param name="centuries"></param>
        /// <param name="milleniums"></param>
        /// <param name="normalize"></param>
        public Span( /*BigInteger planckTimes = default( BigInteger ),*/ Decimal yoctoseconds = 0, Decimal zeptoseconds = 0, Decimal attoseconds = 0, Decimal femtoseconds = 0, Decimal picoseconds = 0, Decimal nanoseconds = 0, Decimal microseconds = 0, Decimal milliseconds = 0, Decimal seconds = 0, Decimal minutes = 0, Decimal hours = 0, Decimal days = 0, Decimal weeks = 0, Decimal months = 0, Decimal years = 0, Decimal centuries = 0, Decimal milleniums = 0, Boolean normalize = true ) : this() {
            //TODO Unit testing needed to verify the math.

            //this.PlanckTimes = new PlanckTimes( planckTimes.IfLessThanZeroThenZero() );
            this.PlanckTimes = PlanckTimes.Zero;
            this.Yoctoseconds = new Yoctoseconds( yoctoseconds.IfLessThanZeroThenZero() );
            this.Zeptoseconds = new Zeptoseconds( zeptoseconds.IfLessThanZeroThenZero() );
            this.Attoseconds = new Attoseconds( attoseconds.IfLessThanZeroThenZero() );
            this.Femtoseconds = new Femtoseconds( femtoseconds.IfLessThanZeroThenZero() );
            this.Picoseconds = new Picoseconds( picoseconds.IfLessThanZeroThenZero() );
            this.Nanoseconds = new Nanoseconds( nanoseconds.IfLessThanZeroThenZero() );
            this.Microseconds = new Microseconds( microseconds.IfLessThanZeroThenZero() );
            this.Milliseconds = new Milliseconds( milliseconds.IfLessThanZeroThenZero() );
            this.Seconds = new Seconds( seconds.IfLessThanZeroThenZero() );
            this.Minutes = new Minutes( minutes.IfLessThanZeroThenZero() );
            this.Hours = new Hours( hours.IfLessThanZeroThenZero() );
            this.Days = new Days( days.IfLessThanZeroThenZero() );
            this.Weeks = new Weeks( weeks.IfLessThanZeroThenZero() );
            this.Months = new Months( months.IfLessThanZeroThenZero() );
            this.Years = new Years( years.IfLessThanZeroThenZero() );
            this.Centuries = new Centuries( centuries.IfLessThanZeroThenZero() );
            this.Milleniums = new Milleniums( milleniums.IfLessThanZeroThenZero() );

            if ( normalize ) {
                //NOTE the order here is important.

                //BUG the "while"s used below can probably be changed to "if"s because of the math, which should work the first time.

                while ( this.PlanckTimes.Value > PlanckTimes.InOneYoctosecond ) {
                    var truncate = this.PlanckTimes.Value/PlanckTimes.InOneYoctosecond;
                    this.PlanckTimes -= truncate*PlanckTimes.InOneYoctosecond;
                    truncate.ThrowIfOutOfDecimalRange();
                    this.Yoctoseconds += ( Decimal ) truncate;
                }

                while ( this.Yoctoseconds.Value > Yoctoseconds.InOneZeptosecond ) {
                    var truncate = Math.Truncate( this.Yoctoseconds.Value/Yoctoseconds.InOneZeptosecond );
                    this.Yoctoseconds -= truncate*Yoctoseconds.InOneZeptosecond;
                    this.Zeptoseconds += truncate;
                }

                while ( this.Zeptoseconds.Value > Zeptoseconds.InOneAttosecond ) {
                    var truncate = Math.Truncate( this.Zeptoseconds.Value/Zeptoseconds.InOneAttosecond );
                    this.Zeptoseconds -= truncate*Yoctoseconds.InOneZeptosecond;
                    this.Attoseconds += truncate;
                }

                while ( this.Attoseconds.Value > Attoseconds.InOneFemtosecond ) {
                    var truncate = Math.Truncate( this.Attoseconds.Value/Attoseconds.InOneFemtosecond );
                    this.Attoseconds -= truncate*Attoseconds.InOneFemtosecond;
                    this.Femtoseconds += truncate;
                }

                while ( this.Femtoseconds.Value > Femtoseconds.InOnePicosecond ) {
                    var truncate = Math.Truncate( this.Femtoseconds.Value/Femtoseconds.InOnePicosecond );
                    this.Femtoseconds -= truncate*Femtoseconds.InOnePicosecond;
                    this.Picoseconds += truncate;
                }

                while ( this.Picoseconds.Value > Picoseconds.InOneNanosecond ) {
                    var truncate = Math.Truncate( this.Picoseconds.Value/Picoseconds.InOneNanosecond );
                    this.Picoseconds -= truncate*Picoseconds.InOneNanosecond;
                    this.Nanoseconds += truncate;
                }

                while ( this.Nanoseconds.Value > Nanoseconds.InOneMicrosecond ) {
                    var truncate = Math.Truncate( this.Nanoseconds.Value/Nanoseconds.InOneMicrosecond );
                    this.Nanoseconds -= truncate*Nanoseconds.InOneMicrosecond;
                    this.Microseconds += truncate;
                }

                while ( this.Microseconds.Value > Microseconds.InOneMillisecond ) {
                    var truncate = Math.Truncate( this.Microseconds.Value/Microseconds.InOneMillisecond );
                    this.Microseconds -= truncate*Microseconds.InOneMillisecond;
                    this.Milliseconds += truncate;
                }

                while ( this.Milliseconds.Value > Milliseconds.InOneSecond ) {
                    var truncate = Math.Truncate( this.Milliseconds.Value/Milliseconds.InOneSecond );
                    this.Milliseconds -= truncate*Milliseconds.InOneSecond;
                    this.Seconds += truncate;
                }

                while ( this.Seconds.Value > Seconds.InOneMinute ) {
                    var truncate = Math.Truncate( this.Seconds.Value/Seconds.InOneMinute );
                    this.Seconds -= truncate*Seconds.InOneMinute;
                    this.Minutes += truncate;
                }

                while ( this.Minutes.Value > Minutes.InOneHour ) {
                    var truncate = Math.Truncate( this.Minutes.Value/Minutes.InOneHour );
                    this.Minutes -= truncate*Minutes.InOneHour;
                    this.Hours += truncate;
                }

                while ( this.Hours.Value > Hours.InOneDay ) {
                    var truncate = Math.Truncate( this.Hours.Value/Hours.InOneDay );
                    this.Hours -= truncate*Hours.InOneDay;
                    this.Days += truncate;
                }

                while ( this.Days.Value > Days.InOneWeek ) {
                    var truncate = Math.Truncate( this.Days.Value/Days.InOneWeek );
                    this.Days -= truncate*Days.InOneWeek;
                    this.Weeks += truncate;
                }

                while ( this.Months.Value > Months.InOneYear ) {
                    var truncate = Math.Truncate( this.Months.Value/Months.InOneYear );
                    this.Months -= truncate*Months.InOneYear;
                    this.Years += truncate;
                }

                while ( this.Years.Value > Years.InOneCentury ) {
                    var truncate = Math.Truncate( this.Years.Value/Years.InOneCentury );
                    this.Years -= truncate*Years.InOneCentury;
                    this.Centuries += truncate;
                }

                while ( this.Centuries.Value > Centuries.InOneMillenium ) {
                    var truncate = Math.Truncate( this.Centuries.Value/Centuries.InOneMillenium );
                    this.Centuries -= truncate*Centuries.InOneMillenium;
                    this.Milleniums += truncate;
                }

                //this.IsNormalized = true;
            }
            //else {
            //    //this.IsNormalized = false;
            //}

            var tmpThis = this;
            this._lazyTotal = new Lazy< BigInteger >( valueFactory: () => tmpThis.CalcTotalPlanckTimes(), isThreadSafe: true );
        }

        /// <summary>
        ///     TODO untested
        /// </summary>
        /// <param name="seconds"></param>
        public Span( Decimal seconds ) : this() {
            var bob = new Seconds( seconds );
            var jane = bob.ToPlanckTimes();
            var frank = new Span( jane );

            this.PlanckTimes = frank.PlanckTimes;
            this.Attoseconds = frank.Attoseconds;
            this.Centuries = frank.Centuries;
            this.Days = frank.Days;
            this.Femtoseconds = frank.Femtoseconds;
            this.Hours = frank.Hours;
            this.Microseconds = frank.Microseconds;
            this.Milleniums = frank.Milleniums;
            this.Milliseconds = frank.Milliseconds;
            this.Minutes = frank.Minutes;
            this.Months = frank.Months;
            this.Nanoseconds = frank.Nanoseconds;
            this.Picoseconds = frank.Picoseconds;
            this.Seconds = frank.Seconds;
            this.Weeks = frank.Weeks;
            this.Years = frank.Years;
            this.Yoctoseconds = frank.Yoctoseconds;
            this.Zeptoseconds = frank.Zeptoseconds;

            var tmpThis = this;
            this._lazyTotal = new Lazy< BigInteger >( valueFactory: () => tmpThis.CalcTotalPlanckTimes(), isThreadSafe: true );
        }

        /// <summary>
        /// </summary>
        public BigInteger TotalPlanckTimes { get { return this._lazyTotal.Value; } }

        [UsedImplicitly]
        private string DebuggerDisplay { get { return this.ToString(); } }

        public int CompareTo( Span other ) {
            return Compare( this, other );
        }

        public int CompareTo( TimeSpan other ) {
            return Compare( this, new Span( other ) );
        }

        Boolean IEquatable< Span >.Equals( Span other ) {
            return this.Equals( obj: other );
        }

        /// <summary>
        ///     <para>Given the <paramref name="left" /> <see cref="Span" />,</para>
        ///     <para>add (+) the <paramref name="right" /> <see cref="Span" />.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Span Combine( Span left, Span right ) {
            //TODO do some overflow handling with BigInteger math

            //var planckTimes = left.PlanckTimes + right.PlanckTimes;
            var yoctoseconds = left.Yoctoseconds + right.Yoctoseconds;
            var zeptoseconds = left.Zeptoseconds + right.Zeptoseconds;
            var attoseconds = left.Attoseconds + right.Attoseconds;
            var femtoseconds = left.Femtoseconds + right.Femtoseconds;
            var picoseconds = left.Picoseconds + right.Picoseconds;
            var nanoseconds = left.Nanoseconds + right.Nanoseconds;
            var microseconds = left.Microseconds + right.Microseconds;
            var milliseconds = left.Milliseconds + right.Milliseconds;
            var seconds = left.Seconds + right.Seconds;
            var minutes = left.Minutes + right.Minutes;
            var hours = left.Hours + right.Hours;
            var days = left.Days + right.Days;
            var months = left.Months + right.Months;
            var years = left.Years + right.Years;
            var centuries = left.Centuries + right.Centuries;
            var milleniums = left.Milleniums + right.Milleniums;

            return new Span( /*planckTimes: planckTimes,*/ yoctoseconds: yoctoseconds, zeptoseconds: zeptoseconds, attoseconds: attoseconds, femtoseconds: femtoseconds, picoseconds: picoseconds, nanoseconds: nanoseconds, microseconds: microseconds, milliseconds: milliseconds, seconds: seconds, minutes: minutes, hours: hours, days: days, months: months, years: years, centuries: centuries, milleniums: milleniums, normalize: true );
        }

        /// <summary>
        ///     <para>Compares two <see cref="Span" /> values, returning an <see cref="int" /> that indicates their relationship.</para>
        ///     <para>Returns 1 if <paramref name="left" /> is larger.</para>
        ///     <para>Returns -1 if <paramref name="right" /> is larger.</para>
        ///     <para>Returns 0 if <paramref name="left" /> and <paramref name="right" /> are equal.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static int Compare( Span left, Span right ) {
            //The order of comparisons here is important. All spans should have positive values, so comparisons will be easier.

            if ( left.Milleniums.CompareTo( right.Milleniums ) == 1 ) {
                return 1;
            }
            if ( left.Centuries.CompareTo( right.Centuries ) == 1 ) {
                return 1;
            }
            if ( left.Years.CompareTo( right.Years ) == 1 ) {
                return 1;
            }

            return 0; //they're equal in ALL respects
        }

        /// <summary>
        ///     <para>Static comparison of two <see cref="Span" />.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Span left, Span right ) {
            if ( left.Milleniums != right.Milleniums ) {
                return false;
            }
            if ( left.Centuries != right.Centuries ) {
                return false;
            }
            if ( left.Years != right.Years ) {
                return false;
            }
            if ( left.Months != right.Months ) {
                return false;
            }
            if ( left.Weeks != right.Weeks ) {
                return false;
            }
            if ( left.Days != right.Days ) {
                return false;
            }
            if ( left.Hours != right.Hours ) {
                return false;
            }
            if ( left.Minutes != right.Minutes ) {
                return false;
            }
            if ( left.Seconds != right.Seconds ) {
                return false;
            }
            if ( left.Milliseconds != right.Milliseconds ) {
                return false;
            }
            if ( left.Microseconds != right.Microseconds ) {
                return false;
            }

            if ( left.Nanoseconds != right.Nanoseconds ) {
                return false;
            }
            if ( left.Picoseconds != right.Picoseconds ) {
                return false;
            }
            if ( left.Femtoseconds != right.Femtoseconds ) {
                return false;
            }
            if ( left.Attoseconds != right.Attoseconds ) {
                return false;
            }
            if ( left.Zeptoseconds != right.Zeptoseconds ) {
                return false;
            }
            return left.Yoctoseconds == right.Yoctoseconds && left.PlanckTimes == right.PlanckTimes;
        }

        /// <summary>
        ///     <para>Given the <paramref name="left" /> <see cref="Span" />,</para>
        ///     <para>subtract (-) the <paramref name="right" /> <see cref="Span" />.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Span operator -( Span left, Span right ) {
            var microseconds = left.Microseconds;
            var milliseconds = left.Milliseconds;
            var seconds = left.Seconds;
            var minutes = left.Minutes;
            var hours = left.Hours;
            var days = left.Days;
            var months = left.Months;
            var years = left.Years;
            var centuries = left.Centuries;
            var milleniums = left.Milleniums;

            //TODO overflow checking

            unchecked {
                microseconds -= right.Microseconds;
                milliseconds -= right.Milliseconds;
                seconds -= right.Seconds;
                minutes -= right.Minutes;
                hours -= right.Hours;
                days -= right.Days;
                months -= right.Months;
                years -= right.Years;
                centuries -= right.Centuries;
                milleniums -= right.Milleniums;
            }

            return new Span( microseconds: microseconds, milliseconds: milliseconds, seconds: seconds, minutes: minutes, hours: hours, days: days, months: months, years: years, centuries: centuries, milleniums: milleniums );
        }

        //public Span( When min, When max ) {
        //    var difference = max - min; // difference.Value now has the total number of planckTimes since the big bang (difference.Value is a BigInteger).
        //    var bo = 5.850227064E+53;
        public static Boolean operator !=( Span t1, Span t2 ) {
            return !Equals( t1, t2 );
        }

        //    //BigInteger.DivRem
        //    //var  = difference % Attoseconds.One.Value;
        //}
        /// <summary>
        ///     <para>Given the <paramref name="left" /> <see cref="Span" />,</para>
        ///     <para>add (+) the <paramref name="right" /> <see cref="Span" />.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Span operator +( Span left, Span right ) {
            return Combine( left, right );
        }

        public static Boolean operator <( Span left, Span right ) {
            return left.TotalPlanckTimes < right.TotalPlanckTimes;
        }

        public static Boolean operator <=( Span left, Span right ) {
            return left.TotalPlanckTimes <= right.TotalPlanckTimes;
        }

        public static Boolean operator ==( Span left, Span right ) {
            return Equals( left, right );
        }

        public static Boolean operator >( Span left, Span right ) {
            return left.TotalPlanckTimes > right.TotalPlanckTimes;
        }

        public static Boolean operator >=( Span left, Span right ) {
            return left.TotalPlanckTimes >= right.TotalPlanckTimes;
        }

        public static Span TryParse( [CanBeNull] String text ) {
            try {
                if ( null == text ) {
                    return Zero;
                }
                text = text.Trim();
                if ( text.IsNullOrWhiteSpace() ) {
                    return Zero;
                }

                TimeSpan result;
                if ( TimeSpan.TryParse( text, out result ) ) {
                    return new Span( result ); //cheat and use the existing TimeSpan parsing code...
                }

                Decimal units;
                if ( text.IsJustNumbers( out units ) ) {
                    return new Span( milliseconds: units ); //assume milliseconds given
                }

                if ( text.EndsWith( "milliseconds", StringComparison.InvariantCultureIgnoreCase ) ) {
                    text = text.Before( "milliseconds" );
                    if ( text.IsJustNumbers( out units ) ) {
                        return new Span( milliseconds: units );
                    }
                }

                if ( text.EndsWith( "millisecond", StringComparison.InvariantCultureIgnoreCase ) ) {
                    text = text.Before( "millisecond" );
                    if ( text.IsJustNumbers( out units ) ) {
                        return new Span( milliseconds: units );
                    }
                }

                if ( text.EndsWith( "seconds", StringComparison.InvariantCultureIgnoreCase ) ) {
                    text = text.Before( "seconds" );
                    if ( text.IsJustNumbers( out units ) ) {
                        return new Span( seconds: units );
                    }
                }

                if ( text.EndsWith( "second", StringComparison.InvariantCultureIgnoreCase ) ) {
                    text = text.Before( "second" );
                    if ( text.IsJustNumbers( out units ) ) {
                        return new Span( seconds: units );
                    }
                }

                //TODO parse for more, even multiple  2 days, 3 hours, and 4 minutes etc...
            }
            catch ( ArgumentNullException ) { }
            catch ( ArgumentException ) { }
            catch ( OverflowException ) { }
            return Zero;
        }

        /// <summary>
        ///     <para>
        ///         Returns a <see cref="BigInteger" /> of all the years in this <see cref="Span" />, including
        ///         <see cref="Centuries" /> and <see cref="Milleniums" />.
        ///     </para>
        /// </summary>
        public BigInteger CalculateTotalYears() {
            var totalYears = ( BigInteger ) this.Years.Value;
            totalYears += new BigInteger( this.Centuries.Value*Years.InOneCentury );
            totalYears += new BigInteger( this.Milleniums.Value*Years.InOneCentury*Centuries.InOneMillenium );
            return totalYears;
        }

        public Boolean Equals( Span obj ) {
            return Equals( this, obj );
        }

        /// <summary>
        ///     Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        ///     true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param>
        /// <filterpriority>2</filterpriority>
        public override Boolean Equals( [CanBeNull] object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Span && Equals( this, ( Span ) obj );
        }

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode() {
            return this.PlanckTimes.GetHashMerge( this.Yoctoseconds.GetHashMerge( this.Zeptoseconds.GetHashMerge( this.Attoseconds.GetHashMerge( this.Femtoseconds.GetHashMerge( this.Picoseconds.GetHashMerge( this.Nanoseconds.GetHashMerge( this.Microseconds.GetHashMerge( this.Milliseconds.GetHashMerge( this.Seconds.GetHashMerge( this.Minutes.GetHashMerge( this.Hours.GetHashMerge( this.Days.GetHashMerge( this.Weeks.GetHashMerge( this.Months.GetHashMerge( this.Years.GetHashMerge( this.Centuries.GetHashMerge( this.Milleniums ) ) ) ) ) ) ) ) ) ) ) ) ) ) ) ) );
        }

        public override String ToString() {
            var bob = new Queue< string >( 20 );

            if ( this.Milleniums.Value != Decimal.Zero ) {
                bob.Enqueue( this.Milleniums.ToString() );
            }
            if ( this.Centuries.Value != Decimal.Zero ) {
                bob.Enqueue( this.Centuries.ToString() );
            }
            if ( this.Years.Value != Decimal.Zero ) {
                bob.Enqueue( this.Years.ToString() );
            }
            if ( this.Months.Value != Decimal.Zero ) {
                bob.Enqueue( this.Months.ToString() );
            }
            if ( this.Weeks.Value != Decimal.Zero ) {
                bob.Enqueue( this.Weeks.ToString() );
            }
            if ( this.Days.Value != Decimal.Zero ) {
                bob.Enqueue( this.Days.ToString() );
            }
            if ( this.Hours.Value != Decimal.Zero ) {
                bob.Enqueue( this.Hours.ToString() );
            }
            if ( this.Minutes.Value != Decimal.Zero ) {
                bob.Enqueue( this.Minutes.ToString() );
            }
            if ( this.Seconds.Value != Decimal.Zero ) {
                bob.Enqueue( this.Seconds.ToString() );
            }
            if ( this.Milliseconds.Value != Decimal.Zero ) {
                bob.Enqueue( this.Milliseconds.ToString() );
            }
            if ( this.Microseconds.Value != Decimal.Zero ) {
                bob.Enqueue( this.Microseconds.ToString() );
            }
            if ( this.Nanoseconds.Value != Decimal.Zero ) {
                bob.Enqueue( this.Nanoseconds.ToString() );
            }
            if ( this.Picoseconds.Value != Decimal.Zero ) {
                bob.Enqueue( this.Picoseconds.ToString() );
            }
            if ( this.Femtoseconds.Value != Decimal.Zero ) {
                bob.Enqueue( this.Femtoseconds.ToString() );
            }
            if ( this.Attoseconds.Value != Decimal.Zero ) {
                bob.Enqueue( this.Attoseconds.ToString() );
            }
            if ( this.Zeptoseconds.Value != Decimal.Zero ) {
                bob.Enqueue( this.Zeptoseconds.ToString() );
            }
            if ( this.Yoctoseconds.Value != Decimal.Zero ) {
                bob.Enqueue( this.Yoctoseconds.ToString() );
            }
            if ( this.PlanckTimes.Value != BigInteger.Zero ) {
                bob.Enqueue( this.PlanckTimes.ToString() );
            }

            return bob.ToStrings( ", " );
        }
    }
}