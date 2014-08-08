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
// "Librainian2/BlockingQueue.cs" was last cleaned by Rick on 2014/08/08 at 2:25 PM

#endregion License & Information

namespace Librainian.Collections {

    using System;
    using System.Threading;

    public class BlockingQueue<T> {
        private readonly Object _lockObj;

        private Node _head;

        private Node _tail;

        public BlockingQueue() {
            this._lockObj = new Object();
            this._head = this._tail = new Node( default( T ), null );
        }

        public T Dequeue() {
            lock ( this._lockObj ) {
                while ( this._head.Next == null ) {
                    Monitor.Wait( this._lockObj );
                }

                var retItem = this._head.Next.Item;
                this._head = this._head.Next;

                return retItem;
            }
        }

        public void Enqueue( T item ) {
            var newNode = new Node( item, null );

            lock ( this._lockObj ) {
                this._tail.Next = newNode;
                this._tail = newNode;

                Monitor.Pulse( this._lockObj );
            }
        }

        internal class Node {
            internal T Item;

            internal Node Next;

            public Node() {
            }

            public Node( T item, Node next ) {
                this.Item = item;
                this.Next = next;
            }
        }
    }
}