﻿namespace Librainian.Internet.Servers {
    using System;

    public static class Extensions {
        /// <summary>
        /// Returns the date and time formatted for insertion as the expiration date in a "Set-Cookie" header.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ToCookieTime( this DateTime time ) {
            return time.ToString( "dd MMM yyyy hh:mm:ss GMT" );
        }
    }
}