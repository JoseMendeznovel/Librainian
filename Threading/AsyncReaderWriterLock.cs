// Copyright � 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "AsyncReaderWriterLock.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "AsyncReaderWriterLock.cs" was last formatted by Protiguous on 2018/06/04 at 4:26 PM.

namespace Librainian.Threading {

	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	/// <summary>
	/// </summary>
	public class AsyncReaderWriterLock {

		private readonly Task<Releaser> _readerReleaser;

		private readonly Queue<TaskCompletionSource<Releaser>> _waitingWriters = new Queue<TaskCompletionSource<Releaser>>();

		private readonly Task<Releaser> _writerReleaser;

		private Int32 _mReadersWaiting;

		private Int32 _mStatus;

		private TaskCompletionSource<Releaser> _mWaitingReader = new TaskCompletionSource<Releaser>();

		public Task<Releaser> ReaderLockAsync() {
			lock ( this._waitingWriters ) {
				if ( this._mStatus >= 0 && this._waitingWriters.Count == 0 ) {
					++this._mStatus;

					return this._readerReleaser;
				}

				++this._mReadersWaiting;

				return this._mWaitingReader.Task.ContinueWith( t => t.Result );
			}
		}

		public void ReaderRelease() {
			TaskCompletionSource<Releaser> toWake = null;

			lock ( this._waitingWriters ) {
				--this._mStatus;

				if ( this._mStatus == 0 && this._waitingWriters.Count > 0 ) {
					this._mStatus = -1;
					toWake = this._waitingWriters.Dequeue();
				}
			}

			toWake?.SetResult( new Releaser( this, true ) );
		}

		public Task<Releaser> WriterLockAsync() {
			lock ( this._waitingWriters ) {
				if ( this._mStatus == 0 ) {
					this._mStatus = -1;

					return this._writerReleaser;
				}

				var waiter = new TaskCompletionSource<Releaser>();
				this._waitingWriters.Enqueue( waiter );

				return waiter.Task;
			}
		}

		public void WriterRelease() {
			TaskCompletionSource<Releaser> toWake = null;
			var toWakeIsWriter = false;

			lock ( this._waitingWriters ) {
				if ( this._waitingWriters.Count > 0 ) {
					toWake = this._waitingWriters.Dequeue();
					toWakeIsWriter = true;
				}
				else if ( this._mReadersWaiting > 0 ) {
					toWake = this._mWaitingReader;
					this._mStatus = this._mReadersWaiting;
					this._mReadersWaiting = 0;
					this._mWaitingReader = new TaskCompletionSource<Releaser>();
				}
				else { this._mStatus = 0; }
			}

			toWake?.SetResult( result: new Releaser( toRelease: this, writer: toWakeIsWriter ) );
		}

		public AsyncReaderWriterLock() {
			this._readerReleaser = Task.FromResult( result: new Releaser( toRelease: this, writer: false ) );
			this._writerReleaser = Task.FromResult( result: new Releaser( toRelease: this, writer: true ) );
		}

	}

}