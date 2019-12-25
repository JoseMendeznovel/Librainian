// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "ParsingConstants.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "Librainian", "ParsingConstants.cs" was last formatted by Protiguous on 2019/10/11 at 9:26 AM.

namespace LibrainianCore.Parsing {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Measurement.Time;

    public static class ParsingConstants {

        public static Lazy<String> AllLetters { get; } = new Lazy<String>( () =>
            new String( Enumerable.Range( UInt16.MinValue, UInt16.MaxValue ).Select( i => ( Char ) i ).Where( Char.IsLetter ).Distinct().OrderBy( c => c ).ToArray() ) );

        public static String[] Consonants { get; } = "B,C,CH,CL,D,F,FF,G,GH,GL,J,K,L,LL,M,MN,N,P,PH,PS,R,RH,S,SC,SH,SK,ST,T,TH,V,W,X,Y,Z".Split( ',' );

        [NotNull]
        public static String EnglishAlphabetLowercase { get; } = "abcdefghijklmnopqrstuvwxyz".ToLower();

        [NotNull]
        public static String EnglishAlphabetUppercase { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToUpper();

        /// <summary>
        /// </summary>
        public static String[] TensMap { get; } = {
            "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety"
        };

        /// <summary>
        /// </summary>
        public static String[] UnitsMap { get; } = {
            "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen",
            "seventeen", "eighteen", "nineteen"
        };

        /// <summary>
        ///     The set of characters that are unreserved in RFC 2396 but are NOT unreserved in RFC 3986.
        /// </summary>
        public static IEnumerable<String> UriRfc3986CharsToEscape { get; } = new[] {
            "!", "*", "'", "(", ")"
        };

        public static String[] Vowels { get; } = "A,AI,AU,E,EA,EE,I,IA,IO,O,OA,OI,OO,OU,U".Split( ',' );

        public const String Doublespace = Parsing.Symbols.Singlespace + Parsing.Symbols.Singlespace;

        public const String Lowercase = "abcdefghijklmnopqrstuvwxyz";

        public const String MatchMoney = @"//\$\s*[-+]?([0-9]{0,3}(,[0-9]{3})*(\.[0-9]+)?)";

        public const String Numbers = "0123456789";

        public const String SplitByEnglish = @"(?:\p{Lu}(?:\.\p{Lu})+)(?:,\s*\p{Lu}(?:\.\p{Lu})+)*";

        /// <summary>
        ///     Regex pattern for words that don't start with a number
        /// </summary>
        public const String SplitByWordNotNumber = @"([a-zA-Z]\w+)\W*";

        /// <summary> ~`!@#$%^&*()-_=+?:,./\[]{}|' </summary>
        public const String Symbols = @"~`!@#$%^&*()-_=+<>?:,./\[]{}|'";

        /// <summary>
        ///     ABCDEFGHIJKLMNOPQRSTUVWXYZ
        /// </summary>
        public const String Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        ///     N, 0, no, false, Fail, failed, failure, bad
        /// </summary>
        public static readonly String[] FalseStrings = {
            "N", "0", "no", "false", Boolean.FalseString, "Fail", "failed", "Failure", "bad"
        };

        /// <summary>
        ///     Y, 1
        /// </summary>
        public static readonly Char[] TrueChars = {
            'Y', '1'
        };

        /// <summary>
        ///     Y, 1, yes, true, Success, good, Go, Positive, Continue
        /// </summary>
        public static readonly String[] TrueStrings = {
            "Y", "1", "yes", "true", Boolean.TrueString, nameof( Status.Success ), "good", nameof( Status.Go ), nameof( Status.Positive ), nameof( Status.Continue )
        };

        public static readonly Regex UpperCaseRegeEx = new Regex( @"^[A-Z]+$", RegexOptions.Compiled, Minutes.One );

    }

}