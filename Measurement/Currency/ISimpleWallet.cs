﻿// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ISimpleWallet.cs" belongs to Rick@AIBrain.org and
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
// "Librainian/Librainian/ISimpleWallet.cs" was last formatted by Protiguous on 2018/05/24 at 7:25 PM.

namespace Librainian.Measurement.Currency {

    using System;
    using System.Windows.Forms;
    using JetBrains.Annotations;

    public interface ISimpleWallet {

        Decimal Balance { get; }

        [CanBeNull]
        Label LabelToFlashOnChanges { get; set; }

        [CanBeNull]
        Action<Decimal> OnAfterDeposit { get; set; }

        [CanBeNull]
        Action<Decimal> OnAfterWithdraw { get; set; }

        [CanBeNull]
        Action<Decimal> OnAnyUpdate { get; set; }

        [CanBeNull]
        Action<Decimal> OnBeforeDeposit { get; set; }

        [CanBeNull]
        Action<Decimal> OnBeforeWithdraw { get; set; }

        /// <summary>Add any (+-)amount directly to the balance.</summary>
        /// <param name="amount"></param>
        /// <param name="sanitize"></param>
        /// <returns></returns>
        Boolean TryAdd( Decimal amount, Boolean sanitize = true );

        Boolean TryAdd( [NotNull] SimpleWallet wallet, Boolean sanitize = true );

        /// <summary>Attempt to deposit amoount (larger than zero) to the <see cref="SimpleWallet.Balance" />.</summary>
        /// <param name="amount"></param>
        /// <param name="sanitize"></param>
        /// <returns></returns>
        Boolean TryDeposit( Decimal amount, Boolean sanitize = true );

        Boolean TryTransfer( Decimal amount, ref SimpleWallet intoWallet, Boolean sanitize = true );

        Boolean TryUpdateBalance( Decimal amount, Boolean sanitize = true );

        void TryUpdateBalance( SimpleWallet simpleWallet );

        Boolean TryWithdraw( Decimal amount, Boolean sanitize = true );

        Boolean TryWithdraw( [NotNull] SimpleWallet wallet );
    }
}