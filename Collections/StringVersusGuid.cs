#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// "Librainian2/StringVersusGuid.cs" was last cleaned by Rick on 2014/08/08 at 2:25 PM

#endregion License & Information

namespace Librainian.Collections {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Librainian.Extensions;
    using Parsing;

    /// <summary>
    /// Contains Words and their guids. Persisted to and from storage? Thread-safe?
    /// </summary>
    /// <remarks>i can see places where the tables are locked independantly.. could cause issues??</remarks>
    [DataContract( IsReference = true )]
    public class StringVersusGuid {

        /// <summary>
        /// </summary>
        /// <remarks>Two dictionaries for speed, one class to rule them all.</remarks>
        [DataMember]
        [OptionalField]
        public readonly ConcurrentDictionary<Guid, String> Guids = new ConcurrentDictionary<Guid, String>();

        /// <summary>
        /// </summary>
        /// <remarks>Two dictionaries for speed, one class to rule them all.</remarks>
        [DataMember]
        [OptionalField]
        public readonly ConcurrentDictionary<String, Guid> Words = new ConcurrentDictionary<String, Guid>();

        public IEnumerable<Guid> EachGuid { get { return this.Guids.Keys; } }

        public IEnumerable<String> EachWord { get { return this.Words.Keys; } }

        /// <summary>
        /// Get or set the guid for this word.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Guid this[ String key ] {
            get {
                if ( !String.IsNullOrEmpty( key ) ) {
                    Guid result;
                    if ( this.Words.TryGetValue( key, out result ) ) {
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
                this.Words.AddOrUpdate( key: key, addValue: guid, updateValueFactory: ( s, g ) => guid );
                this.Guids.AddOrUpdate( key: guid, addValue: key, updateValueFactory: ( g, s ) => key );
            }
        }

        /// <summary>
        /// Get or set the word for this guid.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public String this[ Guid key ] {
            get { return Guid.Empty.Equals( key ) ? String.Empty : this.Guids[ key ]; }

            set {
                if ( Guid.Empty.Equals( key ) ) {
                    return;
                }
                this.Guids.AddOrUpdate( key: key, addValue: value, updateValueFactory: ( g, s ) => value );
                this.Words.AddOrUpdate( key: value, addValue: key, updateValueFactory: ( s, g ) => key );
            }
        }

        public void Clear() {
            this.Words.Clear();
            this.Guids.Clear();
        }

        /// <summary>
        /// Returns true if the word is contained in the collections.
        /// </summary>
        /// <param name="daword"></param>
        /// <returns></returns>
        public Boolean Contains( String daword ) {
            if ( String.IsNullOrEmpty( daword ) ) {
                return false;
            }
            Guid value;
            return this.Words.TryGetValue( key: daword, value: out value );
        }

        /// <summary>
        /// Returns true if the guid is contained in the collection.
        /// </summary>
        /// <param name="daguid"></param>
        /// <returns></returns>
        public Boolean Contains( Guid daguid ) {
            String value;
            return this.Guids.TryGetValue( key: daguid, value: out value );
        }

        private void InternalTest() {
            var guid = new Guid( @"bddc4fac-20b9-4365-97bf-c98e84697012" );
            this[ "AIBrain" ] = guid;
            this[ guid ].Same( "AIBrain" ).DebugAssert();
        }
    }
}