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
// "Librainian/TimeSpanRange.cs" was last cleaned by Rick on 2014/09/02 at 5:11 AM

#endregion License & Information

namespace Librainian.Measurement.Time {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Maths;
    using Threading;

    /// <summary>
    ///     Represents a Single range with minimum and maximum values
    /// </summary>
    [DataContract( IsReference = true )]
    public struct TimeSpanRange {

        /// <summary>
        ///     Length of the range (difference between maximum and minimum values).
        /// </summary>
        [DataMember]
        
        public readonly TimeSpan Length;

        /// <summary>
        ///     Maximum value
        /// </summary>
        [DataMember]
        
        public readonly TimeSpan Max;

        /// <summary>
        ///     Minimum value
        /// </summary>
        [DataMember]
        
        public readonly TimeSpan Min;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TimeSpanRange" /> class
        /// </summary>
        /// <param name="min"> Minimum value of the range </param>
        /// <param name="max"> Maximum value of the range </param>
        public TimeSpanRange( TimeSpan min, TimeSpan max ) {
            if ( min < max ) {
                this.Min = min;
                this.Max = max;
            }
            else {
                this.Min = max;
                this.Max = min;
            }
            var δ = this.Max - this.Min;
            this.Length = δ;
        }

        public IEnumerable<Days> AllDays() => this.Min.TotalDays.To( this.Max.TotalDays ).Select( second => Days.One );

        public IEnumerable<Hours> AllHours() => this.Min.TotalHours.To( this.Max.TotalHours ).Select( second => Hours.One );

        public IEnumerable<Milliseconds> AllMilliseconds() => this.Min.TotalMilliseconds.To( this.Max.TotalMilliseconds ).Select( millsecond => Milliseconds.One );

        public IEnumerable<Minutes> AllMinutes() => this.Min.TotalMinutes.To( this.Max.TotalMinutes ).Select( minutes => Minutes.One );

        public IEnumerable<Seconds> AllSeconds() => this.Min.TotalSeconds.To( this.Max.TotalSeconds ).Select( second => Seconds.One );

        /// <summary>
        ///     Check if the specified range is inside this range
        /// </summary>
        /// <param name="range"> Range to check </param>
        /// <returns>
        ///     <b>True</b> if the specified range is inside this range or <b>false</b> otherwise.
        /// </returns>
        public Boolean IsInside( TimeSpanRange range ) => this.IsInside( range.Min ) && this.IsInside( range.Max );

        /// <summary>
        ///     Check if the specified value is inside this range
        /// </summary>
        /// <param name="x"> Value to check </param>
        /// <returns>
        ///     <b>True</b> if the specified value is inside this range or <b>false</b> otherwise.
        /// </returns>
        public Boolean IsInside( TimeSpan x ) => this.Min <= x && x <= this.Max;

        /// <summary>
        ///     Check if the specified range overlaps with this range
        /// </summary>
        /// <param name="range"> Range to check for overlapping </param>
        /// <returns>
        ///     <b>True</b> if the specified range overlaps with this range or <b>false</b> otherwise.
        /// </returns>
        public Boolean IsOverlapping( TimeSpanRange range ) => this.IsInside( range.Min ) || this.IsInside( range.Max );

        public TimeSpan Random() => Randem.NextTimeSpan( this.Min, this.Max );
    }
}