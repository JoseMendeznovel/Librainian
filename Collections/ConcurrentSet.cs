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
// "Librainian/ConcurrentSet.cs" was last cleaned by Rick on 2014/11/24 at 11:53 AM

#endregion License & Information

namespace Librainian.Collections {
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="http://stackoverflow.com/questions/4306936/how-to-implement-concurrenthashset-in-net" />
    [CollectionDataContract]
    public class ConcurrentSet<T> : ISet<T> {

        /// <summary>
        /// </summary>
        [DataMember]
        private readonly ConcurrentDictionary<T, Object> _dictionary = new ConcurrentDictionary<T, Object>();

        public ConcurrentSet() {
        }

        public ConcurrentSet( params T[] items ) {
            this.UnionWith( items );
        }

        public ConcurrentSet( IEnumerable<T> items ) {
            this.UnionWith( items );
        }

        /// <summary>
        ///     Gets a value that indicates if the set is empty.
        /// </summary>
        public Boolean IsEmpty => this._dictionary.IsEmpty;

        public ICollection<T> Values => this._dictionary.Keys;

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator() => this._dictionary.Keys.GetEnumerator();

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        ///     Removes the first occurrence of a specific object from the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <returns>
        ///     true if <paramref name="item" /> was successfully removed from the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if
        ///     <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">
        ///     The <see cref="T:System.Collections.Generic.ICollection`1" /> is
        ///     read-only.
        /// </exception>
        public bool Remove( T item ) => this.TryRemove( item );

        /// <summary>
        ///     Gets the number of elements in the set.
        /// </summary>
        public int Count => this._dictionary.Count;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <returns>
        ///     true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly => false;

		/// <summary>
		///     Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
		/// </summary>
		/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
		/// <exception cref="T:System.NotSupportedException">
		///     The <see cref="T:System.Collections.Generic.ICollection`1" /> is
		///     read-only.
		/// </exception>
		/// <exception cref="ArgumentException"></exception>
		void ICollection<T>.Add( T item ) {
            if ( !this.Add( item ) ) {
                throw new ArgumentException( "Item already exists in set." );
            }
        }

        /// <summary>
        ///     Modifies the current set so that it contains all elements that are present in both the current set and in the
        ///     specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is null.</exception>
        public void UnionWith( IEnumerable<T> other ) {
            foreach ( var item in other ) {
                this.TryAdd( item );
            }
        }

        /// <summary>
        ///     Modifies the current set so that it contains only elements that are also in a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is null.</exception>
        public void IntersectWith( IEnumerable<T> other ) {
            var enumerable = other as IList<T> ?? other.ToArray();
            foreach ( var item in this.Where( item => !enumerable.Contains( item ) ) ) {
                this.TryRemove( item );
            }
        }

        /// <summary>
        ///     Removes all elements in the specified collection from the current set.
        /// </summary>
        /// <param name="other">The collection of items to remove from the set.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is null.</exception>
        public void ExceptWith( IEnumerable<T> other ) {
            foreach ( var item in other ) {
                this.TryRemove( item );
            }
        }

        /// <summary>
        ///     Modifies the current set so that it contains only elements that are present either in the current set or in the
        ///     specified collection, but not both.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is null.</exception>
        public void SymmetricExceptWith( IEnumerable<T> other ) {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Determines whether a set is a subset of a specified collection.
        /// </summary>
        /// <returns>
        ///     true if the current set is a subset of <paramref name="other" />; otherwise, false.
        /// </returns>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is null.</exception>
        public bool IsSubsetOf( IEnumerable<T> other ) {
            var enumerable = other as IList<T> ?? other.ToArray();
            return this.AsParallel().All( enumerable.Contains );
        }

        /// <summary>
        ///     Determines whether the current set is a superset of a specified collection.
        /// </summary>
        /// <returns>
        ///     true if the current set is a superset of <paramref name="other" />; otherwise, false.
        /// </returns>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is null.</exception>
        public bool IsSupersetOf( IEnumerable<T> other ) => other.AsParallel().All( this.Contains );

        /// <summary>
        ///     Determines whether the current set is a correct superset of a specified collection.
        /// </summary>
        /// <returns>
        ///     true if the <see cref="T:System.Collections.Generic.ISet`1" /> object is a correct superset of
        ///     <paramref name="other" />; otherwise, false.
        /// </returns>
        /// <param name="other">The collection to compare to the current set. </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is null.</exception>
        public bool IsProperSupersetOf( IEnumerable<T> other ) {
            var enumerable = other as IList<T> ?? other.ToArray();
            return this.Count != enumerable.Count && this.IsSupersetOf( enumerable );
        }

        /// <summary>
        ///     Determines whether the current set is a property (strict) subset of a specified collection.
        /// </summary>
        /// <returns>
        ///     true if the current set is a correct subset of <paramref name="other" />; otherwise, false.
        /// </returns>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is null.</exception>
        public bool IsProperSubsetOf( IEnumerable<T> other ) {
            var enumerable = other as IList<T> ?? other.ToArray();
            return this.Count != enumerable.Count && this.IsSubsetOf( enumerable );
        }

        /// <summary>
        ///     Determines whether the current set overlaps with the specified collection.
        /// </summary>
        /// <returns>
        ///     true if the current set and <paramref name="other" /> share at least one common element; otherwise, false.
        /// </returns>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is null.</exception>
        public bool Overlaps( IEnumerable<T> other ) => other.AsParallel().Any( this.Contains );

        /// <summary>
        ///     Determines whether the current set and the specified collection contain the same elements.
        /// </summary>
        /// <returns>
        ///     true if the current set is equal to <paramref name="other" />; otherwise, false.
        /// </returns>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is null.</exception>
        public bool SetEquals( IEnumerable<T> other ) {
            var enumerable = other as IList<T> ?? other.ToArray();
            return this.Count == enumerable.Count && enumerable.AsParallel().All( this.Contains );
        }

        /// <summary>
        ///     Adds an element to the current set and returns a value to indicate if the element was successfully added.
        /// </summary>
        /// <returns>
        ///     true if the element is added to the set; false if the element is already in the set.
        /// </returns>
        /// <param name="item">The element to add to the set.</param>
        public bool Add( T item ) => this.TryAdd( item );

        // public T this[ int index ] {
        //     get { return this._dictionary.ElementAt( index ).Key; }
        //     set {
        //         var key = this._dictionary.ElementAt( index ).Key;
        //         T result;
        //         this._dictionary.TryGetValue( key, out result );
        //         return true;
        //     }
        //}

        public void Clear() => this._dictionary.Clear();

        public bool Contains( T item ) => this._dictionary.ContainsKey( item );

        /// <summary>
        ///     Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an
        ///     <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied
        ///     from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have
        ///     zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex" /> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="array" /> is multidimensional.-or-The number of elements
        ///     in the source <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from
        ///     <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.-or-Type
        ///     <paramref name="T" /> cannot be cast automatically to the type of the destination <paramref name="array" />.
        /// </exception>
        public void CopyTo( T[] array, int arrayIndex ) => this.Values.CopyTo( array, arrayIndex );

        public T[] ToArray() => this._dictionary.Keys.ToArray();

        public bool TryAdd( T item ) => this._dictionary.TryAdd( item, null );

        public bool TryRemove( T item ) {
            object donotcare;
            return this._dictionary.TryRemove( item, out donotcare );
        }
    }
}