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
// "Librainian/Extensions.cs" was last cleaned by Rick on 2014/08/11 at 12:36 AM
#endregion

namespace Librainian.Collections {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Numerics;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using Annotations;
    using FluentAssertions;
    using Maths;
    using NUnit.Framework;
    using Threading;

    public static class Extensions {
        public static readonly List<string> EmptyList = new List<string>();

        /// <summary>
        ///     <para>A list containing <see cref="bool.False" /> then <see cref="bool.True" />.</para>
        /// </summary>
        public static readonly Lazy<List<bool>> FalseThenTrue = new Lazy<List<bool>>( () => new List<bool>( new[] { false, true } ) );

        /// <summary>
        ///     <para>A list containing <see cref="Boolean.True" /> then <see cref="Boolean.False" />.</para>
        /// </summary>
        public static readonly Lazy<List<bool>> TrueThenFalse = new Lazy<List<bool>>( () => new List<bool>( new[] { true, false } ) );

        static Extensions() {
            FalseThenTrue.Value.Fix();
            TrueThenFalse.Value.Fix();
        }

        public static void Add<T>( [NotNull] this IProducerConsumerCollection<T> collection, [NotNull] IEnumerable<T> items ) {
            if ( collection == null ) {
                throw new ArgumentNullException( "collection" );
            }
            if ( items == null ) {
                throw new ArgumentNullException( "items" );
            }
            Parallel.ForEach( items, Randem.Parallelism, collection.Add );
        }

        public static void Add<T>( this IProducerConsumerCollection<T> collection, T item ) {
            if ( null == collection ) {
                throw new ArgumentNullException( "collection" );
            }
            if ( !Equals( item, default( T ) ) ) {
                collection.TryAdd( item );
            }
        }

        public static T Append<T>( [NotNull] this Enum type, T value ) where T : struct {
            if ( type == null ) {
                throw new ArgumentNullException( "type" );
            }
            return ( T )( ValueType )( ( ( int )( ValueType )type | ( int )( ValueType )value ) );
        }

        public static BigInteger CountBig<TType>( [NotNull] this IEnumerable<TType> items ) {
            if ( items == null ) {
                throw new ArgumentNullException( "items" );
            }
            return items.Aggregate( BigInteger.Zero, ( current, item ) => current + BigInteger.One );
        }

        /// <summary>
        ///     Returns duplicate items found in the <see cref="sequence" /> .
        /// </summary>
        public static HashSet<T> Duplicates<T>( this IEnumerable<T> sequence ) {
            if ( null == sequence ) {
                throw new ArgumentNullException( "sequence" );
            }
            var set = new HashSet<T>();
            return new HashSet<T>( sequence.Where( item => !set.Add( item: item ) ) );
        }

        public static IEnumerable<T> EnumerableFromArray<T>( [NotNull] IEnumerable<T> array ) {
            if ( array == null ) {
                throw new ArgumentNullException( "array" );
            }
            return array;
        }

        /// <summary>
        ///     The <seealso cref="List{T}.Capacity" /> is resized down to the <seealso cref="List{T}.Count" />.
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="collection"> </param>
        public static void Fix<T>( [NotNull] this List<T> collection ) {
            if ( collection == null ) {
                throw new ArgumentNullException( "collection" );
            }

            collection.Capacity = collection.Count();
        }

        public static Boolean Has<T>( [NotNull] this Enum type, T value ) where T : struct {
            if ( type == null ) {
                throw new ArgumentNullException( "type" );
            }
            return ( ( int )( ValueType )type & ( int )( ValueType )value ) == ( int )( ValueType )value;
        }

        public static Boolean HasDuplicates<T>( [NotNull] this IEnumerable<T> sequence ) {
            if ( sequence == null ) {
                throw new ArgumentNullException( "sequence" );
            }
            if ( Equals( sequence, null ) ) {
                throw new ArgumentNullException( "sequence" );
            }
            return sequence.Duplicates().Any();
        }

        public static Boolean In<T>( this T value, [NotNull] params T[] items ) {
            if ( items == null ) {
                throw new ArgumentNullException( "items" );
            }
            return items.Contains( value );
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        /// <see cref="http://stackoverflow.com/a/3562370/956364" />
        public static int IndexOfSequence<T>( [NotNull] this IEnumerable<T> source, [NotNull] IEnumerable<T> sequence ) {
            if ( source == null ) {
                throw new ArgumentNullException( "source" );
            }
            if ( sequence == null ) {
                throw new ArgumentNullException( "sequence" );
            }
            return source.IndexOfSequence( sequence, EqualityComparer<T>.Default );
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sequence"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        /// <see cref="http://stackoverflow.com/a/3562370/956364" />
        public static int IndexOfSequence<T>( [NotNull] this IEnumerable<T> source, [NotNull] IEnumerable<T> sequence, [NotNull] IEqualityComparer<T> comparer ) {
            if ( source == null ) {
                throw new ArgumentNullException( "source" );
            }
            if ( sequence == null ) {
                throw new ArgumentNullException( "sequence" );
            }
            if ( comparer == null ) {
                throw new ArgumentNullException( "comparer" );
            }

            var seq = sequence.ToArray();

            var p = 0; // current position in source sequence
            var i = 0; // current position in searched sequence
            var prospects = new List<int>(); // list of prospective matches
            foreach ( var item in source ) {
                // Remove bad prospective matches
                prospects.RemoveAll( k => !comparer.Equals( item, seq[ p - k ] ) );

                // Is it the start of a prospective match ?
                if ( comparer.Equals( item, seq[ 0 ] ) ) {
                    prospects.Add( p );
                }

                // Does current character continues partial match ?
                if ( comparer.Equals( item, seq[ i ] ) ) {
                    i++;
                    // Do we have a complete match ?
                    if ( i == seq.Length ) {
                        // Bingo !
                        return p - seq.Length + 1;
                    }
                }
                else // Mismatch
                {
                    // Do we have prospective matches to fall back to ?
                    if ( prospects.Count > 0 ) {
                        // Yes, use the first one
                        var k = prospects[ 0 ];
                        i = p - k + 1;
                    }
                    else {
                        // No, start from beginning of searched sequence
                        i = 0;
                    }
                }
                p++;
            }
            // No match
            return -1;
        }

        /// <summary>
        ///     <para>A list containing <see cref="Boolean.True" /> then <see cref="Boolean.False" />.</para>
        /// </summary>
        public static IEnumerable<bool> Infinitely( this Boolean value ) {
            do {
                yield return value;
            } while ( true );
            // ReSharper disable once FunctionNeverReturns
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        public static LinkedListNode<TType> NextOrFirst<TType>( [NotNull] this LinkedListNode<TType> current ) {
            if ( current == null ) {
                throw new ArgumentNullException( "current" );
            }
            return current.Next ?? current.List.First;
        }

        public static IEnumerable<IEnumerable<T>> Partition<T>( [NotNull] this IEnumerable<T> source, int size ) {
            if ( source == null ) {
                throw new ArgumentNullException( "source" );
            }
            T[] array = null;
            var count = 0;
            foreach ( var item in source ) {
                if ( array == null ) {
                    array = new T[ size ];
                }
                array[ count ] = item;
                count++;
                if ( count != size ) {
                    continue;
                }
                yield return new ReadOnlyCollection<T>( array );
                array = null;
                count = 0;
            }
            if ( array != null ) {
                Array.Resize( ref array, count );
                yield return new ReadOnlyCollection<T>( array );
            }
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        public static LinkedListNode<TType> PreviousOrLast<TType>( [NotNull] this LinkedListNode<TType> current ) {
            if ( current == null ) {
                throw new ArgumentNullException( "current" );
            }
            return current.Previous ?? current.List.Last;
        }

        public static IEnumerable<TU> Rank<T, TKey, TU>( [NotNull] this IEnumerable<T> source, [NotNull] Func<T, TKey> keySelector, [NotNull] Func<T, int, TU> selector ) {
            //if ( !source.Any() ) {
            //    yield break;
            //}

            if ( source == null ) {
                throw new ArgumentNullException( "source" );
            }
            if ( keySelector == null ) {
                throw new ArgumentNullException( "keySelector" );
            }
            if ( selector == null ) {
                throw new ArgumentNullException( "selector" );
            }
            var rank = 0;
            var itemCount = 0;
            var ordered = source.OrderBy( keySelector ).ToArray();
            var previous = keySelector( ordered[ 0 ] );
            foreach ( var t in ordered ) {
                itemCount += 1;
                var current = keySelector( t );
                if ( !current.Equals( previous ) ) {
                    rank = itemCount;
                }
                yield return selector( t, rank );
                previous = current;
            }
        }

        public static T Remove<T>( [NotNull] this IProducerConsumerCollection<T> collection ) {
            if ( collection == null ) {
                throw new ArgumentNullException( "collection" );
            }
            T result;
            return collection.TryTake( out result ) ? result : default( T );
        }

        public static T Remove<T>( [NotNull] this Enum type, T value ) where T : struct {
            if ( type == null ) {
                throw new ArgumentNullException( "type" );
            }
            return ( T )( ValueType )( ( ( int )( ValueType )type & ~( int )( ValueType )value ) );
        }

        /// <summary>
        ///     Removes the <paramref name="specificItem" /> from the <paramref name="collection" /> and returns how many
        ///     <paramref
        ///         name="specificItem" />
        ///     or null were removed.
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="collection"> </param>
        /// <param name="specificItem"> </param>
        /// <returns> </returns>
        public static int Remove<T>( this IProducerConsumerCollection<T> collection, T specificItem ) {
            if ( null == collection ) {
                throw new ArgumentNullException( "collection" );
            }
            if ( Equals( specificItem, null ) ) {
                throw new ArgumentNullException( "specificItem" );
            }

            var removed = 0;

            while ( collection.Contains( specificItem ) ) {
                T temp;
                if ( !collection.TryTake( out temp ) ) {
                    continue;
                }
                if ( Equals( temp, default( T ) ) || Equals( temp, specificItem ) ) {
                    removed += 1;
                    continue;
                }

                collection.TryAdd( temp );
            }

            return removed;
        }

        public static int RemoveAll<T>( [NotNull] this IProducerConsumerCollection<T> collection ) {
            if ( collection == null ) {
                throw new ArgumentNullException( "collection" );
            }
            T result;
            var removed = 0;
            while ( collection.TryTake( out result ) ) {
                removed++;
            }
            return removed;
        }

        public static IEnumerable<T> RemoveEach<T>( [NotNull] this IProducerConsumerCollection<T> collection ) {
            if ( collection == null ) {
                throw new ArgumentNullException( "collection" );
            }
            T result;
            while ( collection.TryTake( out result ) ) {
                yield return result;
            }
        }

        /// <summary>
        ///     <para>Shuffle an array[] in <paramref name="iterations"/>.</para>
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="array"> </param>
        /// <param name="iterations"></param>
        /// <example>Deck.Shuffle( 7 );</example>
        public static void Shuffle<T>( [NotNull] this T[] array, int iterations = 1 ) {
            if ( array == null ) {
                throw new ArgumentNullException( "array" );
            }
            if ( iterations < 1 ) {
                iterations = 1;
            }
            if ( array.Length < 1 ) {
                return; //nothing to shuffle
            }

            while ( iterations > 0 ) {
                iterations--;

                // make a copy of all items
                var bag = new ConcurrentBag<T>( array );
                bag.Should().NotBeEmpty();
                var originalcount = bag.Count;

                var sqrt = ( int )Math.Sqrt( originalcount );
                if ( sqrt <= 1 ) {
                    sqrt = 1;
                }

                // make some buckets.
                var buckets = new List<ConcurrentBag<T>>( capacity: sqrt );
                buckets.AddRange( 1.To( sqrt ).Select( i => new ConcurrentBag<T>() ) );

                // pull the items out of the bag, and put them into a random bucket each
                T item;
                while ( bag.TryTake( out item ) ) {
                    var index = Randem.Next( 0, sqrt );
                    buckets[ index ].Add( item );
                }
                bag.Should().BeEmpty( "All items should have been taken out of the bag" );

                while ( bag.Count < originalcount ) {
                    var index = Randem.Next( minValue: 0, maxValue: buckets.Count );
                    var bucket = buckets[ index ];

                    if ( bucket.TryTake( out item ) ) {
                        bag.Add( item );
                    }
                    if ( bucket.IsEmpty ) {
                        buckets.Remove( bucket );
                    }
                }
                bag.Count.Should().Be( originalcount );

                // put them back into the array
                var newArray = bag.ToArray();
                newArray.CopyTo( array, 0 );
            }

            // Old, !bad! way.
            //var items = array.Count();
            //for ( var i = 0; i < items; i++ ) {
            //    var index1 = randomFunc( 0, items ); //Randem.Next( 0, items );
            //    var index2 = randomFunc( 0, items ); //Randem.Next( 0, items );
            //    array.Swap( index1, index2 );
            //}
        }

        /// <summary>
        ///     <para>Shuffle a list in <paramref name="iterations"/>.</para>
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="list"> </param>
        /// <param name="iterations"></param>
        /// <example>Deck.Shuffle( 7 );</example>
        /// <remarks>I know we could just do a list.orderby.random(), but I /want/ to try it this way.
        /// </remarks>
        public static void Shuffle<T>( [NotNull] this List<T> list, int iterations = 1 ) {
            if ( list == null ) {
                throw new ArgumentNullException( "list" );
            }
            if ( iterations < 1 ) {
                iterations = 1;
            }
            if ( !list.Any() ) {
                return; //nothing to shuffle
            }

            while ( iterations > 0 ) {
                iterations--;

                var originalcount = list.Count;

                var bag = new ConcurrentBag<T>( list );
                bag.Should().NotBeEmpty( because: "made a copy of all items" );

                list.Clear();
                list.Should().BeEmpty( because: "emptied the original list" );

                // make some buckets.
                var sqrt = ( int )Math.Sqrt( originalcount );
                if ( sqrt <= 1 ) {
                    sqrt = 1;   // TODO can we use Math.Ceiling() here?
                }

                var buckets = new List<ConcurrentBag<T>>( 1.To( sqrt ).Select( i => new ConcurrentBag<T>() ) );

                // pull the items out of the bag, and push them into a random bucket each
                T item;
                while ( bag.TryTake( out item ) ) {
                    var index = Randem.Next( 0, sqrt );
                    buckets[ index ].Add( item );
                }
                buckets.Count.Should().Be( sqrt );
                bag.Should().BeEmpty( "All items should have been taken out of the bag" );

                // pull all the items into the buckets
                while ( bag.Count < originalcount ) {
                    var index = Randem.Next( minValue: 0, maxValue: buckets.Count );
                    var bucket = buckets[ index ];

                    if ( bucket.TryTake( out item ) ) {
                        bag.Add( item );
                    }
                    if ( bucket.IsEmpty ) {
                        buckets.Remove( bucket );
                    }
                }
                bag.Count.Should().Be( originalcount );

                // put them back into the list in another random order.
                // I know, I know.. why didn't we do this first? Proof of Concept I guess.
                // Besides, this whole function/process adds more randomness. I hope.
                list.AddRange( bag.OrderBy( o => Randem.Next() ) );
            }

            // Old, !bad! way.
            //var items = array.Count();
            //for ( var i = 0; i < items; i++ ) {
            //    var index1 = randomFunc( 0, items ); //Randem.Next( 0, items );
            //    var index2 = randomFunc( 0, items ); //Randem.Next( 0, items );
            //    array.Swap( index1, index2 );
            //}
        }

        [Test]
        public static TimeSpan ShuffleTest() {
            const int itemCount = 100000000;

            var list = new List<int>();
            for ( var i = 0; i < itemCount; i++ ) {
                list.Add( i );
            }
            var copyToCompare = list.ToList();

            var stopwatch = Stopwatch.StartNew();
            list.Shuffle();
            stopwatch.Stop();
            String.Format( "Shuffle" ).TimeDebug();

            return stopwatch.Elapsed;
        }

        public static IEnumerable<IEnumerable<T>> Split<T>( [NotNull] this IEnumerable<T> list, int parts ) {
            if ( list == null ) {
                throw new ArgumentNullException( "list" );
            }
            var i = 0;
            var splits = from item in list
                         group item by i++ % parts
                             into part
                             select part; //.AsEnumerable();
            return splits;
        }

        /// <summary>
        ///     Swap the two indexes
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="array"> </param>
        /// <param name="index1"> </param>
        /// <param name="index2"> </param>
        public static void Swap<T>( [NotNull] this T[] array, int index1, int index2 ) {
            if ( array == null ) {
                throw new ArgumentNullException( "array" );
            }
            var temp = array[ index1 ];
            array[ index1 ] = array[ index2 ];
            array[ index2 ] = temp;
        }

        /// <summary>
        ///     <para>Remove and return the first item in the list, otherwise return null (or the default() for value types).</para>
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        [CanBeNull]
        public static TType TakeFirst<TType>( this IList<TType> list ) {
            if ( list == null ) {
                throw new ArgumentNullException( "list" );
            }
            if ( list.Count <= 0 ) {
                return default( TType );
            }
            var item = list[ 0 ];
            list.RemoveAt( 0 );
            return item;
        }

        /// <summary>
        ///     <para>Remove and return the last item in the list, otherwise return null.</para>
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        [CanBeNull]
        public static TType TakeLast<TType>( this IList<TType> list ) {
            if ( list == null ) {
                throw new ArgumentNullException( "list" );
            }
            var index = list.Count - 1;
            if ( index < 0 ) {
                return default( TType );
            }
            var item = list[ index ];
            list.RemoveAt( index );
            return item;
        }

        /// <summary>
        ///     <para>
        ///         Returns a string with the <paramref name="separator" /> between each item of an
        ///         <paramref name="enumerable" />.
        ///     </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="separator"></param>
        /// <param name="atTheEnd"></param>
        /// <returns></returns>
        public static String ToStrings<T>( [NotNull] this IEnumerable<T> enumerable, [NotNull] String separator = ", ", String atTheEnd = null ) {
            if ( enumerable == null ) {
                throw new ArgumentNullException( "enumerable" );
            }
            if ( separator == null ) {
                throw new ArgumentNullException( "separator" );
            }

            string result;
            var list = enumerable as IList<T> ?? enumerable.ToList();

            if ( String.IsNullOrEmpty( atTheEnd ) || list.Count <= 2 ) {
                result = String.Join( separator, list );
            }
            else {
                result = String.Join( separator, list.Take( list.Count - 2 ) );
                while ( list.Count > 2 ) {
                    list.RemoveAt( 0 );
                }
                result += list.TakeFirst();
                result += atTheEnd;
                result += list.TakeFirst();
            }
            return result;
        }

        /// <summary>
        ///     Wrapper for <see cref="ConcurrentQueue{T}.TryDequeue" />
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="queue"> </param>
        /// <param name="item"> </param>
        /// <returns> </returns>
        public static Boolean TryTake<T>( [NotNull] this ConcurrentQueue<T> queue, out T item ) {
            if ( queue == null ) {
                throw new ArgumentNullException( "queue" );
            }
            if ( Equals( queue, null ) ) {
                throw new ArgumentNullException( "queue" );
            }
            return queue.TryDequeue( out item );
        }

        /// <summary>
        ///     Wrapper for <see cref="ConcurrentStack{T}.TryPop" />
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="stack"> </param>
        /// <param name="item"> </param>
        /// <returns> </returns>
        public static Boolean TryTake<T>( this ConcurrentStack<T> stack, out T item ) {
            if ( null == stack ) {
                throw new ArgumentNullException( "stack" );
            }
            return stack.TryPop( out item );
        }

        public static void Update<TKey, TValue>( [NotNull] this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, TValue value ) {
            if ( dictionary == null ) {
                throw new ArgumentNullException( "dictionary" );
            }
            TValue dummy;
            dictionary.TryRemove( key, out dummy ); //HACK
            dictionary.TryAdd( key, value );
            //var wtf = default( TValue );
            //dictionary.TryUpdate( key, value, wtf );  //BUG I don't understand the whole if-same-then-replace-semantics. If we're going to replace the value, then why do we care what the current value is anyway?
        }
    }
}
