// Copyright � Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "App.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "App.cs" was last formatted by Protiguous on 2019/10/07 at 6:03 AM.

namespace Librainian {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime;
    using System.Threading;
    using System.Windows.Forms;
    using Collections.Extensions;
    using CommandLine;
    using Controls;
    using JetBrains.Annotations;
    using Logging;
    using NLog;
    using NLog.Common;
    using NLog.Config;
    using Error = CommandLine.Error;

    public static class App {

        /// <summary>
        ///     <para>Adds program-wide exception handlers.</para>
        ///     <para>Optimizes program startup.</para>
        ///     <para>Starts logging, Debug and Trace.</para>
        ///     <para>Performs a garbage cleanup.</para>
        ///     <para>And starts the form thread-loop.</para>
        /// </summary>
        public static void Run<TOpts>( [NotNull] Action<TOpts> runParsedOptions, String[] arguments ) where TOpts : IOptions {

            if ( runParsedOptions is null ) {
                throw new ArgumentNullException(  nameof( runParsedOptions ) );
            }

            if ( arguments is null ) {
                throw new ArgumentNullException(  nameof( arguments ) );
            }

            try {
                RunInternalCommon();

                Parser.Default?.ParseArguments<TOpts>( arguments ).WithParsed( runParsedOptions ).WithNotParsed( HandleErrors );
            }
            catch ( Exception exception ) {
                exception.Log( breakinto: true );
            }

            static void HandleErrors( IEnumerable<Error> errors ) {
                try {
                    if ( errors is null ) {
                        if ( Debugger.IsAttached ) {
                            Debug.WriteLine( "Unknown error." );
                            Debugger.Break();
                        }
                    }
                    else {
                        var message = errors.Select( error => error?.ToString() ).ToStrings( Environment.NewLine );

                        if ( Debugger.IsAttached ) {
                            Debug.WriteLine( message );
                            Debugger.Break();
                        }
                        else {
                            Console.WriteLine( $"Error parsing command line options.{Environment.NewLine}{message}" );
                        }
                    }
                }
                catch ( Exception exception ) {
                    exception.Log();
                }
            }
        }

        private static void RunInternalCommon() {
            AppDomain.CurrentDomain.UnhandledException += ( sender, e ) => ( e?.ExceptionObject as Exception )?.Log( breakinto: true );
            Application.ThreadException += ( sender, e ) => e?.Exception?.Log( breakinto: true );

            ProfileOptimization.SetProfileRoot( Application.ExecutablePath );
            ProfileOptimization.StartProfile( Application.ExecutablePath );

            Debug.AutoFlush = true;
            Trace.AutoFlush = true;

            Application.EnableVisualStyles();

            try {
                Application.SetCompatibleTextRenderingDefault( defaultValue: false );
            }
            catch ( InvalidOperationException exception ) {
                exception.Log();
            }

            try {
                Thread.CurrentThread.Name = "UI";
            }
            catch ( InvalidOperationException exception ) {
                exception.Log();
            }

            InternalLogger.Reset();

            if ( LogManager.Configuration is null ) {
                LogManager.Configuration = new LoggingConfiguration();
                InternalLogger.Trace( $"{nameof( Run )} created a new {nameof( LoggingConfiguration )}." );
            }

            Debug.Assert( LogLevel.Trace != null, "LogLevel.Trace != null" );
            Debug.Assert( LogLevel.Fatal != null, "LogLevel.Fatal != null" );
            Logging.Logging.Setup( LogLevel.Trace, LogLevel.Fatal, SomeTargets.TraceTarget.Value );

#if DEBUG
            LogManager.ThrowConfigExceptions = true;
            LogManager.ThrowExceptions = true;
#endif

            Compact();
            Thread.Yield();
            Compact();

            static void Compact() {
                GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                GC.Collect( 2, GCCollectionMode.Forced, true, true );
            }
        }

        public static void Run<TForm>( [CanBeNull] IEnumerable<String> arguments ) where TForm : Form, new() {
            RunInternalCommon();

            using ( var form = new TForm() ) {
                if ( arguments != null ) {
                    form.Tag = arguments.Where( s => !String.IsNullOrWhiteSpace( s ) );
                }

                form.SuspendLayout();
                form.WindowState = FormWindowState.Normal;
                form.StartPosition = FormStartPosition.WindowsDefaultBounds;
                form.LoadLocation();
                form.LoadSize();

                if ( !form.IsFullyVisibleOnAnyScreen() ) {
                    form.WindowState = FormWindowState.Normal;
                    form.StartPosition = FormStartPosition.CenterScreen;
                }

                form.ResumeLayout( true );
                form.LocationChanged += ( sender, args ) => form.SaveLocation();
                form.SizeChanged += ( sender, args ) => form.SaveSize();

                Application.Run( form );
            }
        }
    }
}