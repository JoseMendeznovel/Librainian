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
// "Librainian2/Blocks.cs" was last cleaned by Rick on 2014/08/08 at 2:31 PM
#endregion

namespace Librainian.Threading {
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using System.Timers;
    using Measurement.Time;

    public static class Blocks {
        internal static readonly ConcurrentDictionary< Timer, DateTime > Timers = new ConcurrentDictionary< Timer, DateTime >();

        public static IPropagatorBlock< T, T > CreateDelayBlock< T >( TimeSpan delay ) {
            var lastItem = DateTime.MinValue;
            return new TransformBlock< T, T >( async x => {
                                                         var waitTime = lastItem + delay - DateTime.UtcNow;
                                                         if ( waitTime > TimeSpan.Zero ) {
                                                             await Task.Delay( waitTime );
                                                         }

                                                         lastItem = DateTime.UtcNow;

                                                         return x;
                                                     }, new ExecutionDataflowBlockOptions {
                                                                                              BoundedCapacity = 1
                                                                                          } );
        }

        /// <summary>
        ///     Keep posting to the <see cref="ITargetBlock{TInput}" /> until it posts.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="item"></param>
        public static void TryPost< T >( this ITargetBlock< T > target, T item ) {
            if ( target == null ) {
#if DEBUG
                throw new ArgumentNullException( "target" );
#else
                return;
#endif
            }

            if ( !target.Post( item ) ) {
                //var bob = target as IDataflowBlock;
                //if ( bob.Completion.IsCompleted  )
                TryPost( target: target, item: item, delay: Threads.GetSlicingAverage() ); //retry
            }
        }

        /// <summary>
        ///     After a delay, keep posting to the <see cref="ITargetBlock{TInput}" /> until it posts.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="item"></param>
        /// <param name="delay"></param>
        public static Timer TryPost< T >( this ITargetBlock< T > target, T item, TimeSpan delay ) {
            if ( target == null ) {
                throw new ArgumentNullException( "target" );
            }

            try {
                if ( delay < Milliseconds.One ) {
                    delay = Milliseconds.One;
                }
                var timer = new Timer( interval: delay.TotalMilliseconds );
                timer.Elapsed += ( sender, args ) => {
                                     //timer.Stop(); //not needed because AutoReset = false;
                                     try {
                                         target.TryPost( item );
                                     }
                                     finally {
                                         if ( timer != null ) {
                                             DateTime value;
                                             Timers.TryRemove( timer, out value );
                                             timer.Dispose();
                                         }
                                     }
                                     timer = null;
                                 };
                timer.AutoReset = false;
                Timers[ timer ] = DateTime.UtcNow;
                timer.Start();
                return timer;
            }
            catch ( Exception exception ) {
                exception.Log();
                throw;
            }
        }

        public static class ManyProducers {
            /// <summary>
            ///     Multiple producers consumed in smoothly (MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded).
            /// </summary>
            public static readonly ExecutionDataflowBlockOptions ConsumeSensible = new ExecutionDataflowBlockOptions {
                                                                                                                         SingleProducerConstrained = false,
                                                                                                                         MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded
                                                                                                                     };

            /// <summary>
            ///     Multiple producers consumed in parallel (MaxDegreeOfParallelism = Threads.ProcessorCount).
            /// </summary>
            public static readonly ExecutionDataflowBlockOptions ConsumeParallel = new ExecutionDataflowBlockOptions {
                                                                                                                         SingleProducerConstrained = false,
                                                                                                                         MaxDegreeOfParallelism = Threads.ProcessorCount
                                                                                                                     };

            /// <summary>
            ///     Multiple producers consumed in serial (MaxDegreeOfParallelism = 1).
            /// </summary>
            public static readonly ExecutionDataflowBlockOptions ConsumeSerial = new ExecutionDataflowBlockOptions {
                                                                                                                       SingleProducerConstrained = false,
                                                                                                                       MaxDegreeOfParallelism = 1
                                                                                                                   };
        }

        public static class SingleProducer {
            /// <summary>
            ///     <para>Single producer consumed in serial (one at a time).</para>
            /// </summary>
            public static readonly ExecutionDataflowBlockOptions ConsumeSerial = new ExecutionDataflowBlockOptions {
                                                                                                                       SingleProducerConstrained = true,
                                                                                                                       MaxDegreeOfParallelism = 1
                                                                                                                   };

            /// <summary>
            ///     <para>Single producer consumed in smoothly (MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded).</para>
            /// </summary>
            public static readonly ExecutionDataflowBlockOptions ConsumeSensible = new ExecutionDataflowBlockOptions {
                                                                                                                         SingleProducerConstrained = false,
                                                                                                                         MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded
                                                                                                                     };

            /// <summary>
            ///     <para>Single producer consumed in smoothly (MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded).</para>
            /// </summary>
            public static ExecutionDataflowBlockOptions ConsumeParallel = new ExecutionDataflowBlockOptions {
                                                                                                                SingleProducerConstrained = true,
                                                                                                                MaxDegreeOfParallelism = Threads.ProcessorCount
                                                                                                            };
        }
    }
}
