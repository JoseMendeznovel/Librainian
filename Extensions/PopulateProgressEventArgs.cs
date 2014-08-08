#region License & Information
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
// "Librainian2/PopulateProgressEventArgs.cs" was last cleaned by Rick on 2014/08/08 at 2:26 PM
#endregion

namespace Librainian.Extensions {
    using System;

    /// <summary>
    ///     Simple EventArg for the two progress events
    ///     NOTE: There will typically be some errors
    ///     which is fine as some parts of the Registry are
    ///     not accessible with standard security
    /// </summary>
    public class PopulateProgressEventArgs : EventArgs {
        private readonly String _keyName;

        public PopulateProgressEventArgs( int itemCount, String KeyName = null ) {
            this.ItemCount = itemCount;
            this._keyName = KeyName;
        }

        public PopulateProgressEventArgs() : this( -1, null ) { }

        public String KeyName { get { return this._keyName; } }

        public int ItemCount { get; internal set; }
    }
}