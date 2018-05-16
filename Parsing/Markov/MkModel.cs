﻿// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "MkModel.cs",
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
// "Librainian/Librainian/MkModel.cs" was last cleaned by Protiguous on 2018/05/15 at 10:49 PM.

namespace Librainian.Parsing.Markov {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Extensions;
    using JetBrains.Annotations;
    using Maths;
    using Persistence;

    public class MkModel {

        private readonly ConcurrentDictionary<String, List<String>> _markovChains = new ConcurrentDictionary<String, List<String>>();

        public readonly String Name;

        public MkModel() => throw new NotImplementedException();

        public MkModel( String name ) => this.Name = name;

        public String GenerateRandomCorpus( Int32 numberOfWords ) {
            if ( !this._markovChains.Any() ) { return String.Empty; }

            var startWord = this._markovChains.OrderBy( o => Randem.Next() ).FirstOrDefault().Key;
            var newCorpus = new StringBuilder( startWord );

            while ( numberOfWords > 0 ) {
                var word = startWord;
                var randomChain = this.Nexts( word: word ).OrderBy( o => Randem.Next() );

                foreach ( var w in randomChain ) {
                    newCorpus.Append( $"{w} " );

                    if ( String.IsNullOrEmpty( w ) ) { continue; }

                    startWord = w;
                    numberOfWords -= 1;
                }
            }

            return newCorpus.ToString();
        }

        /// <summary>
        ///     Need to use JSON loader here..
        /// </summary>
        /// <returns></returns>
        public Boolean Load() => this.Name.Loader<MkModel>( source => source.DeepClone( destination: this ) );

        /// <summary>
        ///     Return the list of strings found after this <paramref name="word" />.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public IEnumerable<String> Nexts( [CanBeNull] String word ) {
            if ( word is null ) { return Enumerable.Empty<String>(); }

            if ( this._markovChains.ContainsKey( word ) ) { return this._markovChains[word]; }

            return Enumerable.Empty<String>();
        }

        /// <summary>
        ///     Need to use JSON saver here..
        /// </summary>
        /// <returns></returns>
        public Boolean Save() => this.Saver( this.Name );

        public void Train( String corpus, Int32 level = 3 ) {
            var words = corpus.ToWords().AsParallel().ToArray();

            Parallel.For( 0, words.Length, ( i, state ) => this._markovChains.TryAdd( words[i], words.Skip( i + 1 ).Take( level ).ToList() ) );
        }
    }
}