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
// "Librainian2/Domain.cs" was last cleaned by Rick on 2014/08/08 at 2:27 PM
#endregion

namespace Librainian.Knowledge {
    using System;

    /// <summary>
    ///     where does this knowledge belong?
    ///     where did this knowledge come from?
    ///     can be used like the rdf:url sources...
    /// </summary>
    public class Domain {
        public Domain( String label ) {
            this.Label = label;
        }

        public String Label { get; private set; }
    }
}