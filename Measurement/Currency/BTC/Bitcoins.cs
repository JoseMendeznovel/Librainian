﻿// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Bitcoins.cs",
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
// "Librainian/Librainian/Bitcoins.cs" was last cleaned by Protiguous on 2018/05/15 at 10:46 PM.

namespace Librainian.Measurement.Currency.BTC {

    using System;

    /// <summary>
    ///     Bitcoins
    /// </summary>
    /// <see cref="http://en.bitcoin.it/wiki/FAQ" />
    //TODO
    public class Bitcoins : ICurrency {

        public Bitcoins( Decimal btc ) {
            this.Btc = btc;
            this.Satoshis = this.Btc * SimpleBitcoinWallet.SatoshiInOneBtc;
        }

        /// <summary>Example new <see cref="Bitcoins" />(123.4567).BTC == 123.0000</summary>
        public Decimal Btc { get; }

        /// <summary>
        ///     Example new <see cref="Bitcoins" />(123.45674564645634). <see cref="Satoshis" /> == 0.4567
        /// </summary>
        /// <remarks>lemOn91: "100 satoshis = 1uBTC. 1000 uBTC = 1mBTC. 1000 mBTC = 1BTC"</remarks>
        /// <remarks>The amount is in satoshis! 1 BTC = 100000000 satoshis.</remarks>
        public Decimal Satoshis { get; }
    }
}