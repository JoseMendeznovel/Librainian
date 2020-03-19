// Copyright � 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "DynamicContext.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "LibrainianCore", File: "DynamicContext.cs" was last formatted by Protiguous on 2020/03/16 at 3:11 PM.

namespace Librainian.Persistence {

    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary></summary>
    /// <see cref="http://stackoverflow.com/a/4857322/956364" />
    [JsonObject]
    [Serializable]
    public class DynamicContext : DynamicObject, ISerializable {

        [SecurityPermission( action: SecurityAction.Demand, SerializationFormatter = true )]
        public virtual void GetObjectData( [CanBeNull] SerializationInfo info, StreamingContext context ) {
            foreach ( var kvp in this.Context ) {
                info.AddValue( name: kvp.Key, value: kvp.Value );
            }
        }

        private Dictionary<String, Object> Context { get; } = new Dictionary<String, Object>();

        protected DynamicContext( [NotNull] SerializationInfo info, StreamingContext context ) {

            // TODO: validate inputs before deserializing. See http://msdn.microsoft.com/en-us/Library/ty01x675(VS.80).aspx
            foreach ( var entry in info ) {
                this.Context.Add( key: entry.Name, value: entry.Value );
            }
        }

        public DynamicContext() { }

        public override Boolean TryGetMember( GetMemberBinder binder, [CanBeNull] out Object result ) => this.Context.TryGetValue( key: binder.Name, value: out result );

        public override Boolean TrySetMember( SetMemberBinder binder, [CanBeNull] Object? value ) {
            this.Context.Add( key: binder.Name, value: value );

            return true;
        }

    }

}