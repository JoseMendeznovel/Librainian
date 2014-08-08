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
// "Librainian2/Page.cs" was last cleaned by Rick on 2014/08/08 at 2:27 PM
#endregion

namespace Librainian.Linguistics {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Annotations;
    using FluentAssertions;

    [DataContract( IsReference = true )]
    public sealed class Page : IEquatable< Page >, IEnumerable< Paragraph > {
        public const int Level = Paragraph.Level << 1;

        [NotNull] [DataMember] public readonly List< Paragraph > Tokens = new List< Paragraph >();

        static Page() {
            Level.Should().BeGreaterThan( Paragraph.Level );
        }

        public Page( [NotNull] String text ) {
            if ( text == null ) {
                throw new ArgumentNullException( "text" );
            }
            this.Add( text );
        }

        public IEnumerator< Paragraph > GetEnumerator() {
            return this.Tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public Boolean Equals( [CanBeNull] Page other ) {
            if ( ReferenceEquals( other, null ) ) {
                return false;
            }
            return ReferenceEquals( this, other ) || this.Tokens.SequenceEqual( other.Tokens );
        }

        public Boolean Add( [NotNull] String text ) {
            if ( text == null ) {
                throw new ArgumentNullException( "text" );
            }
            this.Tokens.Add( new Paragraph( text ) ); //TODO //BUG this needs to add all paragraphs
            return true;
        }
    }
}