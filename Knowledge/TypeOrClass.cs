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
// "Librainian2/TypeOrClass.cs" was last cleaned by Rick on 2014/08/08 at 2:27 PM
#endregion

namespace Librainian.Knowledge {
    using System;

    /// <summary>
    /// </summary>
    /// <example>
    ///     For example (rdf:type Morris Cat) means "Morris is a type of Cat"
    /// </example>
    public class TypeOrClass {
        public TypeOrClass( String label ) {
            this.Label = String.IsNullOrWhiteSpace( label ) ? Guid.NewGuid().ToString() : label;
        }

        /// <summary>
        ///     The name of this type (All X are T).
        /// </summary>
        /// <example>
        ///     Cat. Canine. Mammal
        /// </example>
        public String Label { get; private set; }

        public Domain Domain { get; private set; }
    }
}