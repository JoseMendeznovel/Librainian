﻿// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "MagicExtensions.cs" belongs to Rick@AIBrain.org and
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
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/MagicExtensions.cs" was last formatted by Protiguous on 2018/05/24 at 7:20 PM.

namespace Librainian.Magic {

    using System;
    using System.Reactive;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using JetBrains.Annotations;

    /// <summary>
    ///     <para>Any sufficiently advanced technology is indistinguishable from magic.</para>
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Clarke's_three_laws" />
    public static class MagicExtensions {

        /// <summary></summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="observable"></param>
        /// <returns></returns>
        /// <seealso cref="http://haacked.com/archive/2012/10/08/writing-a-continueafter-method-for-rx.aspx/" />
        public static IObservable<Unit> AsCompletion<T>( this IObservable<T> observable ) =>
            Observable.Create<Unit>( observer => observable.Subscribe( _ => { }, onError: observer.OnError, onCompleted: () => {
                observer.OnNext( Unit.Default );
                observer.OnCompleted();
            } ) );

        public static IObservable<TRet> ContinueAfter<T, TRet>( this IObservable<T> observable, Func<IObservable<TRet>> selector ) => observable.AsCompletion().SelectMany( _ => selector() );

        /// <summary>
        ///     http://stackoverflow.com/a/7642198
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pauser"></param>
        /// <returns></returns>
        public static IObservable<T> Pausable<T>( this IObservable<T> source, IObservable<Boolean> pauser ) =>
            Observable.Create<T>( o => {
                var paused = new SerialDisposable();

                var subscription = source.Publish( ps => {
                    var values = new ReplaySubject<T>();

                    IObservable<T> Switcher( Boolean b ) {
                        if ( !b ) { return values.Concat( second: ps ); }

                        values.Dispose();
                        values = new ReplaySubject<T>();
                        paused.Disposable = ps.Subscribe( observer: values );

                        return Observable.Empty<T>();
                    }

                    return pauser.StartWith( false ).DistinctUntilChanged().Select( selector: Switcher ).Switch();
                } ).Subscribe( observer: o );

                return new CompositeDisposable( subscription, paused );
            } );

        /// <summary>
        ///     If <paramref name="b" /> is <see cref="Boolean.True" /> then perform the <paramref name="action" />.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="action"></param>
        public static void Then( this Boolean b, Action action ) {
            if ( b ) { action?.Invoke(); }
        }

        /// <summary></summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="obj"></param>
        // ReSharper disable once UnusedParameter.Local
        public static void ThrowIfNull<TKey>( [CanBeNull] this TKey obj ) {
            if ( null == obj ) { throw new ArgumentNullException(); }
        }
    }
}