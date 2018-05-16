// Copyright � 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Urls.cs",
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
// "Librainian/Librainian/Urls.cs" was last cleaned by Protiguous on 2018/05/15 at 10:40 PM.

namespace Librainian.Extensions {

    using System;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

    public static class Urls {

        public static String GetSuggestedNameFromUrl( String url, String defaultValue ) {
            var res = Path.GetFileNameWithoutExtension( url );

            //check if there is no file name, i.e. just folder name + query String
            if ( !String.IsNullOrEmpty( res ) && !res.IsNameOnlyQueryString() ) { return defaultValue; }

            res = Path.GetFileName( Path.GetDirectoryName( url ) );

            return String.IsNullOrEmpty( res ) ? defaultValue : Regex.Replace( res, @"[^\w]", "_", RegexOptions.Singleline ).Substring( 0, 50 );
        }

        /// <summary>
        ///     Check that a String is not null or empty
        /// </summary>
        /// <param name="input">String to check</param>
        /// <returns>Boolean</returns>
        public static Boolean HasValue( this String input ) => !String.IsNullOrEmpty( input );

        public static String HtmlAttributeEncode( this String input ) => HttpUtility.HtmlAttributeEncode( input );

        public static String HtmlDecode( this String input ) => HttpUtility.HtmlDecode( input );

        public static String HtmlEncode( this String input ) => HttpUtility.HtmlEncode( input );

        public static Boolean IsNameOnlyQueryString( this String res ) => !String.IsNullOrEmpty( res ) && res[0] == '?';

        public static String UrlDecode( this String input ) => HttpUtility.UrlDecode( input );

        /// <summary>
        ///     Uses Uri.EscapeDataString() based on recommendations on MSDN http:
        ///     //blogs.msdn.com/b/yangxind/archive/2006/11/09/don-t-use-net-system-uri-unescapedatastring-in-url-decoding.aspx
        /// </summary>
        public static String UrlEncode( this String input ) {
            if ( input is null ) { throw new ArgumentNullException( nameof( input ) ); }

            const Int32 maxLength = 32766;

            if ( input.Length <= maxLength ) { return Uri.EscapeDataString( input ); }

            var sb = new StringBuilder( input.Length * 2 );
            var index = 0;

            while ( index < input.Length ) {
                var length = Math.Min( input.Length - index, maxLength );
                var subString = input.Substring( index, length );
                sb.Append( Uri.EscapeDataString( subString ) );
                index += subString.Length;
            }

            return sb.ToString();
        }
    }
}