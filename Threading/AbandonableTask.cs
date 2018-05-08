﻿// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/AbandonableTask.cs" was last cleaned by Protiguous on 2016/06/18 at 10:57 PM

namespace Librainian.Threading {

    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary></summary>
    /// <see cref="http://stackoverflow.com/a/4749401/956364" />
    public sealed class AbandonableTask {
        private readonly Action<Task> _afterComplete;
        private readonly Action _beginWork;
        private readonly Action _blockingWork;
        private readonly CancellationToken _cancellationToken;

        private AbandonableTask( CancellationToken cancellationToken, Action beginWork, Action blockingWork, Action<Task> afterComplete ) {
	        this._cancellationToken = cancellationToken;
            this._beginWork = beginWork;
            this._blockingWork = blockingWork ?? throw new ArgumentNullException( nameof( blockingWork ) );
            this._afterComplete = afterComplete;
        }

        public static Task Start( CancellationToken cancellationToken, Action blockingWork, Action beginWork = null, Action<Task> afterComplete = null ) {
            if ( blockingWork is null ) {
                throw new ArgumentNullException( nameof( blockingWork ) );
            }

            var worker = new AbandonableTask( cancellationToken, beginWork, blockingWork, afterComplete );
            var outerTask = new Task( worker.RunTask, cancellationToken );
            outerTask.Start();
            return outerTask;
        }

        private void RunTask() {
            this._beginWork?.Invoke();

            var innerTask = new Task( this._blockingWork, this._cancellationToken, TaskCreationOptions.LongRunning );
            innerTask.Start();

            innerTask.Wait( this._cancellationToken );

            if ( innerTask.IsCompleted ) {
                this._afterComplete?.Invoke( innerTask );
            }
        }
    }
}