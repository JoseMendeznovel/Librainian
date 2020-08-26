﻿// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ParsingTests.cs" belongs to Protiguous@Protiguous.com and
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
//     (We're still looking into other solutions! Any ideas?)
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
// Feel free to browse any source code we *might* make available.
//
// Project: "LibrainianTests", "ParsingTests.cs" was last formatted by Protiguous on 2019/03/17 at 12:35 PM.

namespace LibrainianTests.Linguistics {

    using System;
    using Librainian.Parsing;
    using Librainian.Persistence;
    using Xunit;

    public class ParsingTests {

        [Fact]
        public static void Test() {
            Console.WriteLine( "<p>George</p><b>W</b><i>Bush</i>".StripTags( new[] {
                "i", "b"
            } ) );

            Console.WriteLine( "<p>George <img src='someimage.png' onmouseover='someFunction()'>W <i>Bush</i></p>".StripTags( new[] {
                "p"
            } ) );

            Console.WriteLine( "<a href='http://www.djksterhuis.org'>Martijn <b>Dijksterhuis</b></a>".StripTags( new[] {
                "a"
            } ) );

            const String test4 = "<a class=\"classof69\" onClick='crosssite.boom()' href='http://www.djksterhuis.org'>Martijn Dijksterhuis</a>";

            Console.WriteLine( test4.StripTagsAndAttributes( new[] {
                "a"
            } ) );
        }

        [Fact]
        public static void TestStaticStorage() {
            const String phraseToTest = "Hello world";

            PersistenceExtensions.Settings( nameof( phraseToTest ), phraseToTest );

            Assert.Equal( phraseToTest, PersistenceExtensions.Settings( nameof( phraseToTest ) ) );
        }
    }
}