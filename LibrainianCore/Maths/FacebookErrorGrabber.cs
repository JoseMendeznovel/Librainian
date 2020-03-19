﻿// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "FacebookErrorGrabber.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "FacebookErrorGrabber.cs" was last formatted by Protiguous on 2020/03/16 at 3:06 PM.

namespace Librainian.Maths {

    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Collections.Extensions;
    using Exceptions;
    using Internet;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    public static class FacebookErrorGrabber {

        /// <summary>See also <see cref="Randem" />.</summary>
        /// <returns></returns>
        [CanBeNull]
        public static Task<FaceBookRootObject> GetError() {
            var uri = new Uri( uriString: "http://graph.facebook.com/microsoft" );

            return uri.DeserializeJson<FaceBookRootObject>();
        }

        /// <summary>Pull another "random" number in a byte array via Facebook.</summary>
        /// <param name="fallbackByteCount">How many random bytes to fallback to when the facebook request fails.</param>
        /// <returns></returns>
        [ItemNotNull]
        public static async Task<Byte[]> NextDataAsync( Int32 fallbackByteCount = 16 ) {

            var rootObject = await GetError().ConfigureAwait( continueOnCapturedContext: false );

            var data = rootObject.Error.FbtraceID;

            if ( data != null ) {
                var buffer = Encoding.UTF8.GetBytes( s: data );

                //mix up the response a bit with our own rng.
                foreach ( var _ in buffer ) {
                    buffer.Swap( index1: Randem.NextByte(), index2: Randem.NextByte() );
                }

                return buffer;
            }

            if ( !fallbackByteCount.Any() ) {
                throw new OutOfRangeException( message: $"{nameof( fallbackByteCount )} must be greater than 0." );
            }

            var fallback = new Byte[ fallbackByteCount ];
            Randem.NextBytes( buffer: ref fallback );

            return fallback;
        }

        public static async Task<Int64> NxtInt32() {
            var bytes = await NextDataAsync( fallbackByteCount: sizeof( Int32 ) ).ConfigureAwait( continueOnCapturedContext: false );

            return BitConverter.ToInt64( value: bytes, startIndex: 0 );
        }

        public static async Task<Int64> NxtInt64() {
            var bytes = await NextDataAsync( fallbackByteCount: sizeof( Int64 ) ).ConfigureAwait( continueOnCapturedContext: false );

            return BitConverter.ToInt64( value: bytes, startIndex: 0 );
        }

        [JsonObject]
        public struct FaceBookError {

            [JsonProperty( propertyName: "code" )]
            public Int32 Code { get; set; }

            [JsonProperty( propertyName: "fbtrace_id" )]
            [CanBeNull]
            public String? FbtraceID { get; set; }

            [JsonProperty( propertyName: "message" )]
            [CanBeNull]
            public String? Message { get; set; }

            [JsonProperty( propertyName: "type" )]
            [CanBeNull]
            public String? Type { get; set; }

        }

        [JsonObject]
        public struct FaceBookRootObject {

            [JsonProperty( propertyName: "error" )]
            public FaceBookError Error { get; }

        }

    }

}