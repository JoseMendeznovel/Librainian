// Copyright � Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "StringVersusGuid.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "StringVersusGuid.cs" was last formatted by Protiguous on 2019/08/08 at 6:38 AM.

namespace Librainian.Collections {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    ///     Contains Words and their guids. Persisted to and from storage? Thread-safe?
    /// </summary>
    /// <remarks>i can see places where the tables are locked independantly.. could cause issues??</remarks>
    [JsonObject]
    public class StringVersusGuid {

        [CanBeNull]
        public IReadOnlyCollection<Guid> EachGuid => this.Guids.Keys as IReadOnlyCollection<Guid>;

        [CanBeNull]
        public IReadOnlyCollection<String> EachWord => this.Words.Keys as IReadOnlyCollection<String>;

        /// <summary>
        /// </summary>
        /// <remarks>Two dictionaries for speed, one class to rule them all.</remarks>
        [JsonProperty]
        public ConcurrentDictionary<Guid, String> Guids { get; } = new ConcurrentDictionary<Guid, String>();

        /// <summary>
        /// </summary>
        /// <remarks>Two dictionaries for speed, one class to rule them all.</remarks>
        [JsonProperty]
        public ConcurrentDictionary<String, Guid> Words { get; } = new ConcurrentDictionary<String, Guid>();

        /// <summary>
        ///     Get or set the guid for this word.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Guid this[ [CanBeNull] String key ] {
            get {
                if ( !String.IsNullOrEmpty( key ) ) {
                    if ( this.Words.TryGetValue( key, out var result ) ) {
                        return result;
                    }

                    var newValue = Guid.NewGuid();
                    this[ key ] = newValue;

                    return newValue;
                }

                return Guid.Empty;
            }

            set {
                if ( String.IsNullOrEmpty( key ) ) {
                    return;
                }

                var guid = value;
                this.Words.AddOrUpdate( key, addValue: guid, updateValueFactory: ( s, g ) => guid );
                this.Guids.AddOrUpdate( guid, addValue: key, updateValueFactory: ( g, s ) => key );
            }
        }

        /// <summary>
        ///     Get or set the word for this guid.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public String this[ Guid key ] {
            get => Guid.Empty.Equals( g: key ) ? String.Empty : this.Guids[ key ];

            set {
                if ( Guid.Empty.Equals( g: key ) ) {
                    return;
                }

                this.Guids.AddOrUpdate( key, addValue: value, updateValueFactory: ( g, s ) => value );
                this.Words.AddOrUpdate( value, addValue: key, updateValueFactory: ( s, g ) => key );
            }
        }

        public static void InternalTest( [NotNull] StringVersusGuid stringVersusGuid ) {
            var guid = new Guid( g: @"bddc4fac-20b9-4365-97bf-c98e84697012" );
            stringVersusGuid[ "AIBrain" ] = guid;

            //stringVersusGuid[ guid ].Is( right: "AIBrain" ).BreakIfFalse();
        }

        public void Clear() {
            this.Words.Clear();
            this.Guids.Clear();
        }

        /// <summary>
        ///     Returns true if the word is contained in the collections.
        /// </summary>
        /// <param name="daword"></param>
        /// <returns></returns>
        public Boolean Contains( [CanBeNull] String daword ) {
            if ( String.IsNullOrEmpty( daword ) ) {
                return false;
            }

            return this.Words.TryGetValue( daword, out _ );
        }

        /// <summary>
        ///     Returns true if the guid is contained in the collection.
        /// </summary>
        /// <param name="daguid"></param>
        /// <returns></returns>
        public Boolean Contains( Guid daguid ) => this.Guids.TryGetValue( daguid, out var value );
    }
}