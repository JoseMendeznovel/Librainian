// Copyright � 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Section.cs",
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
// "Librainian/Librainian/Section.cs" was last cleaned by Protiguous on 2018/05/15 at 10:49 PM.

namespace Librainian.Persistence {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Collections;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    ///     <para>
    ///         This just wraps a <see cref="ConcurrentDictionary{TKey,TValue}" /> so we can index the <see cref="Data" />
    ///         without throwing exceptions on missing or null keys.
    ///     </para>
    ///     <para>Does not throw <see cref="ArgumentNullException" /> on null keys passed to the indexer.</para>
    /// </summary>
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    public class Section : IEquatable<Section> {

        [JsonProperty( IsReference = false, ItemIsReference = false )]
        private ConcurrentDictionary<String, String> Data { get; } = new ConcurrentDictionary<String, String>();

        /// <summary>
        ///     Automatically remove any key where there is no value. Defaults to true.
        /// </summary>
        public Boolean AutoCleanup { get; set; } = true;

        [JsonIgnore]
        [NotNull]
        public IReadOnlyList<String> Keys => ( IReadOnlyList<String> )this.Data.Keys;

        [JsonIgnore]
        [NotNull]
        public IReadOnlyList<String> Values => ( IReadOnlyList<String> )this.Data.Values;

        [JsonIgnore]
        [CanBeNull]
        public String this[[CanBeNull] String key] {
            [CanBeNull]
            get {
                if ( key is null ) { return null; }

                return this.Data.TryGetValue( key, out var value ) ? value : null;
            }

            set {
                if ( key is null ) { return; }

                if ( value is null && this.AutoCleanup ) {
                    this.Data.TryRemove( key, out _ ); //a little cleanup
                }
                else { this.Data[key] = value; }
            }
        }

        /// <summary>
        ///     Static comparison. Checks references and then keys and then values.
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] Section left, [CanBeNull] Section right ) {
            if ( ReferenceEquals( left, right ) ) { return true; }

            if ( left is null || right is null ) { return false; }

            if ( ReferenceEquals( left.Data, right.Data ) ) { return true; }

            return left.Data.OrderBy( pair => pair.Key ).ThenBy( pair => pair.Value ).SequenceEqual( right.Data.OrderBy( pair => pair.Key ).ThenBy( pair => pair.Value ) );
        }

        public static Boolean operator !=( [CanBeNull] Section left, [CanBeNull] Section right ) => !Equals( left: left, right: right );

        public static Boolean operator ==( [CanBeNull] Section left, [CanBeNull] Section right ) => Equals( left: left, right: right );

        /// <summary>
        ///     Remove any key where there is no value.
        /// </summary>
        /// <returns></returns>
        public async Task Cleanup() {
            await Task.Run( () => {
                foreach ( var key in this.Keys.Where( String.IsNullOrEmpty ) ) {
                    if ( this.Data.TryRemove( key, out var value ) && !String.IsNullOrEmpty( value ) ) {
                        this[key] = value; //whoops, re-add value. Cause: other threads.
                    }
                }
            } ).ConfigureAwait( false );
        }

        public Boolean Equals( [CanBeNull] Section other ) => Equals( left: this, right: other );

        public override Boolean Equals( [CanBeNull] Object obj ) => Equals( left: this, right: obj as Section );

        public override Int32 GetHashCode() => this.Data.GetHashCode();

        /// <summary>
        ///     Merges (adds keys and overwrites values) <see cref="Data" /> into <see cref="this" />.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public async Task<Boolean> Read( [NotNull] TextReader reader ) {
            if ( reader is null ) { throw new ArgumentNullException( paramName: nameof( reader ) ); }

            try {
                var that = await reader.ReadLineAsync().ConfigureAwait( false );

                return await Task.Run( () => {
                    if ( JsonConvert.DeserializeObject( that, this.Data.GetType() ) is ConcurrentDictionary<String, String> other ) {
                        Parallel.ForEach( other, pair => this[pair.Key] = pair.Value );

                        return true;
                    }

                    return false;
                } ).ConfigureAwait( false );
            }
            catch ( Exception exception ) { exception.More(); }

            return false;
        }

        public override String ToString() => $"{this.Keys.Take( 25 ).ToStrings()}";

        /// <summary>
        ///     Write this <see cref="Section" /> to the <paramref name="writer" />.
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        public async Task<Boolean> Write( [NotNull] TextWriter writer ) {
            if ( writer is null ) { throw new ArgumentNullException( paramName: nameof( writer ) ); }

            try {
                var me = JsonConvert.SerializeObject( this, Formatting.None );
                await writer.WriteLineAsync( me ).ConfigureAwait( false );

                return true;
            }
            catch ( Exception exception ) { exception.More(); }

            return false;
        }
    }
}