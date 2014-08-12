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
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/IPartofaClock.cs" was last cleaned by Rick on 2014/08/12 at 12:35 AM
#endregion

namespace Librainian.Measurement.Time.Clocks {
    using System;
    using System.Runtime.Serialization;
    using FluentAssertions;

    [DataContract( IsReference = true )]
    public abstract class PartialClock {
        /// <summary>
        ///     1
        /// </summary>
        public const Byte Minimum = 1;

        public abstract Byte Maximum { get; }

        public abstract Byte GetValue();

        protected Byte Validate( long quantity ) {
            quantity.Should().BeInRange( Minimum, this.Maximum );

            if ( quantity < Minimum || quantity > this.Maximum ) {
                throw new ArgumentOutOfRangeException( "quantity", String.Format( "The specified quantity ({0}) is out of the valid range {1} to {2}.", quantity, Minimum, this.Maximum ) );
            }

            return ( Byte ) quantity;
        }
    }
}
