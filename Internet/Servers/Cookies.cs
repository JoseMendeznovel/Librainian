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
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Cookies.cs" was last cleaned by Rick on 2014/09/08 at 3:53 AM
#endregion

namespace Librainian.Internet.Servers {
    using System;
    using System.Collections.Generic;
    using System.Web;

    public class Cookies {
        private readonly SortedList< string, Cookie > cookieCollection = new SortedList< string, Cookie >();

        /// <summary>
        ///     Adds a cookie with the specified name and value.  The cookie is set to expire immediately at the end of the
        ///     browsing session.
        /// </summary>
        /// <param name="name">The cookie's name.</param>
        /// <param name="value">The cookie's value.</param>
        public void Add( string name, string value ) {
            this.Add( name, value, TimeSpan.Zero );
        }

        /// <summary>
        ///     Adds a cookie with the specified name, value, and lifespan.
        /// </summary>
        /// <param name="name">The cookie's name.</param>
        /// <param name="value">The cookie's value.</param>
        /// <param name="expireTime">The amount of time before the cookie should expire.</param>
        public void Add( string name, string value, TimeSpan expireTime ) {
            if ( name == null ) {
                return;
            }
            name = name.ToLower();
            this.cookieCollection[ name ] = new Cookie( name, value, expireTime );
        }

        /// <summary>
        ///     Gets the cookie with the specified name.  If the cookie is not found, null is returned;
        /// </summary>
        /// <param name="name">The name of the cookie.</param>
        /// <returns></returns>
        public Cookie Get( string name ) {
            Cookie cookie;
            if ( !this.cookieCollection.TryGetValue( name, out cookie ) ) {
                cookie = null;
            }
            return cookie;
        }

        /// <summary>
        ///     Gets the value of the cookie with the specified name.  If the cookie is not found, an empty string is returned;
        /// </summary>
        /// <param name="name">The name of the cookie.</param>
        /// <returns></returns>
        public string GetValue( string name ) {
            var cookie = this.Get( name );
            if ( cookie == null ) {
                return "";
            }
            return cookie.value;
        }

        /// <summary>
        ///     Returns a string of "Set-Cookie: ..." headers (one for each cookie in the collection) separated by "\r\n".  There
        ///     is no leading or trailing "\r\n".
        /// </summary>
        /// <returns>
        ///     A string of "Set-Cookie: ..." headers (one for each cookie in the collection) separated by "\r\n".  There is
        ///     no leading or trailing "\r\n".
        /// </returns>
        public override string ToString() {
            var cookiesStr = new List< string >();
            foreach ( var cookie in this.cookieCollection.Values ) {
                cookiesStr.Add( string.Format( "Set-Cookie: {0}={1}{2}; Path=/", cookie.name, cookie.value, ( cookie.expire == TimeSpan.Zero ? "" : "; Max-Age=" + ( long ) cookie.expire.TotalSeconds ) ) );
            }
            return string.Join( "\r\n", cookiesStr );
        }

        /// <summary>
        ///     Returns a Cookies instance populated by parsing the specified string.  The string should be the value of the
        ///     "Cookie" header that was received from the remote client.  If the string is null or empty, an empty cookies
        ///     collection is returned.
        /// </summary>
        /// <param name="str">The value of the "Cookie" header sent by the remote client.</param>
        /// <returns></returns>
        public static Cookies FromString( string str ) {
            var cookies = new Cookies();
            if ( str == null ) {
                return cookies;
            }
            str = HttpUtility.UrlDecode( str );
            var parts = str.Split( ';' );
            for ( var i = 0; i < parts.Length; i++ ) {
                var idxEquals = parts[ i ].IndexOf( '=' );
                if ( idxEquals < 1 ) {
                    continue;
                }
                var name = parts[ i ].Substring( 0, idxEquals ).Trim();
                var value = parts[ i ].Substring( idxEquals + 1 ).Trim();
                cookies.Add( name, value );
            }
            return cookies;
        }
    }
}
