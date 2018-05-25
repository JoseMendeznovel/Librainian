// Copyright � 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "DriveTests.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/DriveTests.cs" was last formatted by Protiguous on 2018/05/24 at 7:02 PM.

namespace Librainian.ComputerSystems.FileSystem {

    using System;
    using System.Diagnostics;
    using NUnit.Framework;
    using Parsing;

    [TestFixture]
    public static class DriveTests {

        [Test]
        public static void TestAllDrives() {
            var alphabet = ParsingExtensions.EnglishAlphabetUppercase;
            Debug.WriteLine( alphabet );

            foreach ( var letter in alphabet ) {
                var drive = new Drive( letter );

                if ( drive.FreeSpace() > 0 ) { Console.WriteLine( drive + " " + drive.FreeSpace() + " " ); }
            }
        }
    }
}