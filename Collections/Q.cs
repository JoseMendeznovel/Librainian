// Copyright � 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Q.cs",
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
// "Librainian/Librainian/Q.cs" was last cleaned by Protiguous on 2018/05/15 at 10:37 PM.

namespace Librainian.Collections {

    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Threading;
    using Newtonsoft.Json;

    [ComVisible( visibility: false )]
    [DebuggerDisplay( "Count={" + nameof( Count ) + "}" )]
    [JsonObject]
    [HostProtection( SecurityAction.LinkDemand, ExternalThreading = true, Synchronization = true )]
    public class Q<T> : IProducerConsumerCollection<T> {

        [NonSerialized]
        private Segment _head;

        private T[] _serializationArray;

        [NonSerialized]
        private Segment _tail;

        public Q() => this._head = this._tail = new Segment( index: 0L );

        public Q( IEnumerable<T> collection ) {
            if ( collection is null ) { throw new ArgumentNullException( nameof( collection ) ); }

            this.InitializeFromCollection( collection: collection );
        }

        public Int32 Count {
            get {
                this.GetHeadTailPositions( head: out var head, tail: out var tail, headLow: out var headLow, tailHigh: out var tailHigh );

                if ( head == tail ) { return tailHigh - headLow + 1; }

                return 32 - headLow + 32 * ( Int32 )( tail.Index - head.Index - 1L ) + tailHigh + 1;
            }
        }

        public Boolean IsEmpty {
            get {
                var segment = this._head;

                if ( !segment.IsEmpty ) { return false; }

                if ( segment.Next is null ) { return true; }

                for ( ; segment.IsEmpty; segment = this._head ) {
                    if ( segment.Next is null ) { return true; }

                    Thread.Yield();
                }

                return false;
            }
        }

        public Boolean IsSynchronized => false;

        public Object SyncRoot => throw new NotSupportedException();

        private void GetHeadTailPositions( out Segment head, out Segment tail, out Int32 headLow, out Int32 tailHigh ) {
            head = this._head;
            tail = this._tail;
            headLow = head.Low;
            tailHigh = tail.High;

            while ( head != this._head || tail != this._tail || headLow != head.Low || tailHigh != tail.High || head.Index > tail.Index ) {
                Thread.Yield();
                head = this._head;
                tail = this._tail;
                headLow = head.Low;
                tailHigh = tail.High;
            }
        }

        private void InitializeFromCollection( IEnumerable<T> collection ) {
            this._head = this._tail = new Segment( index: 0L );
            var num = 0;

            foreach ( var obj in collection ) {
                this._tail.UnsafeAdd( obj );
                ++num;

                if ( num < 32 ) { continue; }

                this._tail = this._tail.UnsafeGrow();
                num = 0;
            }
        }

        [OnDeserialized]
        private void OnDeserialized( StreamingContext context ) {
            this.InitializeFromCollection( collection: this._serializationArray );
            this._serializationArray = null;
        }

        [OnSerializing]
        private void OnSerializing( StreamingContext context ) => this._serializationArray = this.ToArray();

        private List<T> ToList() {
            this.GetHeadTailPositions( head: out var head, tail: out var tail, headLow: out var headLow, tailHigh: out var tailHigh );

            if ( head == tail ) { return head.ToList( start: headLow, end: tailHigh ); }

            var list = new List<T>( collection: head.ToList( start: headLow, end: 31 ) );

            for ( var next = head.Next; next != tail; next = next.Next ) { list.AddRange( collection: next.ToList( start: 0, end: 31 ) ); }

            list.AddRange( collection: tail.ToList( start: 0, end: tailHigh ) );

            return list;
        }

        public void CopyTo( T[] array, Int32 index ) {
            if ( array is null ) { throw new ArgumentNullException( nameof( array ) ); }

            this.ToList().CopyTo( array: array, arrayIndex: index );
        }

        public void Enqueue( T item ) {
            while ( !this._tail.TryAppend( item, tail: ref this._tail ) ) { Thread.Yield(); }
        }

        public IEnumerator<T> GetEnumerator() => this.ToList().GetEnumerator();

        public T[] ToArray() => this.ToList().ToArray();

        public Boolean TryAdd( T item ) {
            this.Enqueue( item: item );

            return true;
        }

        public Boolean TryDequeue( out T result ) {
            while ( !this.IsEmpty ) {
                if ( this._head.TryRemove( result: out result, head: ref this._head ) ) { return true; }
            }

            result = default;

            return false;
        }

        public Boolean TryPeek( out T result ) {
            while ( !this.IsEmpty ) {
                if ( this._head.TryPeek( result: out result ) ) { return true; }
            }

            result = default;

            return false;
        }

        public Boolean TryTake( out T item ) => this.TryDequeue( result: out item );

        void ICollection.CopyTo( Array array, Int32 index ) {
            if ( array is null ) { throw new ArgumentNullException( nameof( array ) ); }

            this.ToArray().CopyTo( array: array, index: index );
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private sealed class Segment {

            private readonly T[] _array;

            private readonly Int32[] _state;

            private Int32 _high;
            private Int32 _low;
            internal readonly Int64 Index;
            public Segment Next;

            internal Segment( Int64 index ) {
                this._array = new T[32];
                this._state = new Int32[32];
                this._high = -1;
                this.Index = index;
            }

            public Boolean IsEmpty => this.Low > this.High;

            public Int32 Low => Math.Min( val1: this._low, val2: 32 );

            public Int32 High => Math.Min( val1: this._high, val2: 31 );

            private void Grow( out Segment tail ) {
                this.Next = new Segment( index: this.Index + 1L );
                tail = this.Next;
            }

            public List<T> ToList( Int32 start, Int32 end ) {
                var list = new List<T>();

                for ( var index = start; index <= end; ++index ) {
                    while ( this._state[index] == 0 ) { Thread.Yield(); }

                    list.Add( item: this._array[index] );
                }

                return list;
            }

            public Boolean TryAppend( T value, ref Segment tail ) {
                if ( this._high >= 31 ) { return false; }

                var index = Interlocked.Increment( location: ref this._high );

                if ( index <= 31 ) {
                    this._array[index] = value;
                    this._state[index] = 1;
                }

                if ( index == 31 ) { this.Grow( tail: out tail ); }

                return index <= 31;
            }

            public Boolean TryPeek( out T result ) {
                result = default;
                var low = this.Low;

                if ( low > this.High ) { return false; }

                while ( this._state[low] == 0 ) { Thread.Yield(); }

                result = this._array[low];

                return true;
            }

            public Boolean TryRemove( out T result, ref Segment head ) {
                var low = this.Low;

                for ( var high = this.High; low <= high; high = this.High ) {

                    if ( Interlocked.CompareExchange( location1: ref this._low, low + 1, comparand: low ) != low ) {

                        Thread.Yield();
                        low = this.Low;
                    }
                    else {
                        while ( this._state[low] == 0 ) { Thread.Yield(); }

                        result = this._array[low];

                        if ( low + 1 >= 32 ) {
                            while ( this.Next is null ) { Thread.Yield(); }

                            head = this.Next;
                        }

                        return true;
                    }
                }

                result = default;

                return false;
            }

            public void UnsafeAdd( T value ) {
                ++this._high;
                this._array[this._high] = value;
                this._state[this._high] = 1;
            }

            public Segment UnsafeGrow() {
                var segment = new Segment( index: this.Index + 1L );
                this.Next = segment;

                return segment;
            }
        }
    }
}