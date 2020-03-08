﻿// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "WMIExtensions.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "WMIExtensions.cs" was last formatted by Protiguous on 2020/01/31 at 12:28 AM.

namespace Librainian.OperatingSystem.WMI {

    using System;
    using System.Management;
    using Converters;
    using JetBrains.Annotations;
    using Parsing;

    public static class WMIExtensions {

        [NotNull]
        public static String Identifier( [NotNull] String wmiClass, [NotNull] String wmiProperty, [NotNull] String wmiMustBeTrue ) {
            if ( String.IsNullOrWhiteSpace( wmiClass ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", nameof( wmiClass ) );
            }

            if ( String.IsNullOrWhiteSpace( wmiProperty ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", nameof( wmiProperty ) );
            }

            if ( String.IsNullOrWhiteSpace( wmiMustBeTrue ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", nameof( wmiMustBeTrue ) );
            }

            using ( var managementClass = new ManagementClass( wmiClass ) ) {
                var instances = managementClass.GetInstances();

                foreach ( var baseObject in instances ) {
                    if ( !( baseObject is ManagementObject managementObject ) || !managementObject[ wmiMustBeTrue ].ToBoolean() ) {
                        continue;
                    }

                    try {
                        return managementObject[ wmiProperty ].ToString();
                    }
                    catch {

                        // ignored
                    }
                }
            }

            return String.Empty;
        }

        [NotNull]
        public static String Identifier( [NotNull] String wmiClass, [NotNull] String wmiProperty ) {
            if ( String.IsNullOrWhiteSpace( wmiClass ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", nameof( wmiClass ) );
            }

            if ( String.IsNullOrWhiteSpace( wmiProperty ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", nameof( wmiProperty ) );
            }

            using ( var managementClass = new ManagementClass( wmiClass ) ) {
                var instances = managementClass.GetInstances();

                foreach ( var baseObject in instances ) {
                    try {
                        if ( baseObject is ManagementObject managementObject ) {
                            return managementObject[ wmiProperty ].ToString();
                        }
                    }
                    catch {

                        // ignored
                    }
                }
            }

            return String.Empty;
        }

        [NotNull]
        public static ManagementObjectCollection QueryWMI( [CanBeNull] String? machineName, [NotNull] String scope, [NotNull] String query ) {
            if ( String.IsNullOrWhiteSpace( query ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
            }

            if ( String.IsNullOrWhiteSpace( scope ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", nameof( scope ) );
            }

            var conn = new ConnectionOptions();
            var nameSpace = @"\\";
            machineName = machineName.Trimmed();
            nameSpace += machineName != String.Empty ? machineName : ".";
            nameSpace += $@"\root\{scope}";
            var managementScope = new ManagementScope( nameSpace, conn );
            var wmiQuery = new ObjectQuery( query );

            using var moSearcher = new ManagementObjectSearcher( managementScope, wmiQuery );

            return moSearcher.Get();
        }

        [NotNull]
        public static ManagementObjectCollection WmiQuery( [NotNull] String query ) {
            if ( String.IsNullOrWhiteSpace( query ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", nameof( query ) );
            }

            var oQuery = new ObjectQuery( query );

            using var oSearcher = new ManagementObjectSearcher( oQuery );

            return oSearcher.Get();
        }
    }
}