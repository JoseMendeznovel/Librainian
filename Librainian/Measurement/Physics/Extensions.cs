﻿// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Extensions.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Extensions.cs" was last formatted by Protiguous on 2019/08/08 at 8:48 AM.

namespace Librainian.Measurement.Physics {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using JetBrains.Annotations;

    public static class Extensions {

        public static Expression<Func<Double, Double>> FahrenheitToCelsius = fahrenheit => ( fahrenheit - 32.0 ) * 5.0 / 9.0;

        [NotNull]
        public static String Simpler( this ElectronVolts volts ) {
            var list = new HashSet<String> {
                volts.ToTeraElectronVolts().ToString(),
                volts.ToGigaElectronVolts().ToString(),
                volts.ToMegaElectronVolts().ToString(),
                volts.ToElectronVolts().ToString(),
                volts.ToMilliElectronVolts().ToString()
            };

            return list.OrderBy( s => s.Length ).FirstOrDefault() ?? "n/a";
        }

        [NotNull]
        public static String Simpler( this KiloElectronVolts volts ) {
            var list = new HashSet<String> {
                volts.ToTeraElectronVolts().ToString(),
                volts.ToGigaElectronVolts().ToString(),
                volts.ToMegaElectronVolts().ToString(),
                volts.ToElectronVolts().ToString(),
                volts.ToMilliElectronVolts().ToString()
            };

            return list.OrderBy( s => s.Length ).FirstOrDefault() ?? "n/a";
        }

        [NotNull]
        public static String Simpler( this MegaElectronVolts volts ) {
            var list = new HashSet<String> {
                volts.ToTeraElectronVolts().ToString(),
                volts.ToGigaElectronVolts().ToString(),
                volts.ToMegaElectronVolts().ToString(),
                volts.ToElectronVolts().ToString(),
                volts.ToMilliElectronVolts().ToString()
            };

            return list.OrderBy( s => s.Length ).FirstOrDefault() ?? "n/a";
        }

        [NotNull]
        public static String Simpler( this GigaElectronVolts volts ) {
            var list = new HashSet<String> {
                volts.ToTeraElectronVolts().ToString(),
                volts.ToGigaElectronVolts().ToString(),
                volts.ToMegaElectronVolts().ToString(),
                volts.ToElectronVolts().ToString(),
                volts.ToMilliElectronVolts().ToString()
            };

            return list.OrderBy( s => s.Length ).FirstOrDefault() ?? "n/a";
        }

        [NotNull]
        public static String Simpler( this TeraElectronVolts volts ) {
            var list = new HashSet<String> {
                volts.ToTeraElectronVolts().ToString(),
                volts.ToGigaElectronVolts().ToString(),
                volts.ToMegaElectronVolts().ToString(),
                volts.ToElectronVolts().ToString(),
                volts.ToMilliElectronVolts().ToString()
            };

            return list.OrderBy( s => s.Length ).FirstOrDefault() ?? "n/a";
        }

        public static MegaElectronVolts Sum( [NotNull] this IEnumerable<ElectronVolts> volts ) {
            if ( volts == null ) {
                throw new ArgumentNullException( nameof( volts ) );
            }

            var result = volts.Aggregate( MegaElectronVolts.Zero, ( current, electronVolts ) => current + electronVolts.ToMegaElectronVolts() );

            return result;
        }

        public static GigaElectronVolts Sum( [NotNull] this IEnumerable<MegaElectronVolts> volts ) {
            if ( volts == null ) {
                throw new ArgumentNullException( nameof( volts ) );
            }

            return volts.Aggregate( GigaElectronVolts.Zero, ( current, megaElectronVolts ) => current + megaElectronVolts.ToGigaElectronVolts() );
        }

        public static TeraElectronVolts Sum( [NotNull] this IEnumerable<GigaElectronVolts> volts ) {
            if ( volts == null ) {
                throw new ArgumentNullException( nameof( volts ) );
            }

            return volts.Aggregate( TeraElectronVolts.Zero, ( current, gigaElectronVolts ) => current + gigaElectronVolts.ToTeraElectronVolts() );
        }
    }
}