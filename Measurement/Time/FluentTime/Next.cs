﻿// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Next.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/Next.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.Measurement.Time.FluentTime {

    using System;

    public static class Next {

        private static DateTime GetNextOfDay( DayOfWeek dayOfWeek ) {
            var today = AdjustableCurrentTime.Today;
            var delta = dayOfWeek - today.DayOfWeek;

            var result = today.AddDays( delta <= 0 ? delta + 7 : delta );

            return result;
        }

        public static DateTime Friday() => GetNextOfDay( DayOfWeek.Friday );

        public static DateTime Monday() => GetNextOfDay( DayOfWeek.Monday );

        public static DateTime Saturday() => GetNextOfDay( DayOfWeek.Saturday );

        public static DateTime Sunday() => GetNextOfDay( DayOfWeek.Sunday );

        public static DateTime Thursday() => GetNextOfDay( DayOfWeek.Thursday );

        public static DateTime Tuesday() => GetNextOfDay( DayOfWeek.Tuesday );

        public static DateTime Wednesday() => GetNextOfDay( DayOfWeek.Wednesday );
    }
}