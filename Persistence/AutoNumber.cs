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
// "Librainian2/AutoNumber.cs" was last cleaned by Rick on 2014/08/08 at 2:30 PM
#endregion

namespace Librainian.Persistence {
    using System;
    using System.Runtime.Serialization;
    using System.Threading;

    /// <summary>
    ///     An automatically incrementing Identity class. ( <see cref="Identity" /> is <see cref="ulong" /> )
    /// </summary>
    [DataContract( IsReference = true )]
    public sealed class AutoNumber {
        [DataMember] private long _identity;

        /// <summary>
        ///     Initialize the Identity with the specified seed value.
        /// </summary>
        /// <param name="seed"> </param>
        public AutoNumber( UInt64 seed = UInt64.MinValue ) {
            this.Reseed( seed );
        }

        /// <summary>
        ///     The current value of the AutoNumber
        /// </summary>
        public UInt64 Identity { get { return ( UInt64 ) Interlocked.Read( ref this._identity ); } }

        /// <summary>
        ///     Resets the Identity to the specified seed value
        /// </summary>
        /// <param name="newIdentity"> </param>
        public void Reseed( UInt64 newIdentity ) {
            Interlocked.Exchange( ref this._identity, ( long ) newIdentity );
        }

        /// <summary>
        ///     Returns the incremented Identity
        /// </summary>
        /// <returns> </returns>
        public UInt64 Next() {
            return ( UInt64 ) Interlocked.Increment( ref this._identity );
        }

        public override String ToString() {
            return String.Format( "{0:R}", this.Identity );
        }

        public void Ensure( UInt64 atLeast ) {
            if ( this.Identity < atLeast ) {
                this.Reseed( atLeast );
            }
        }
    }
}