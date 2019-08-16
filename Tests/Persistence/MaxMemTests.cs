// Copyright � Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "MaxMemTests.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "LibrainianTests", "MaxMemTests.cs" was last formatted by Protiguous on 2019/03/18 at 6:09 PM.
namespace LibrainianTests.Persistence {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using NUnit.Framework;

    [TestFixture]
    public static class MaxMemTests {

        [Test]
        public static void test_max_ReaderWriterLockSlim() {
            GC.Collect();
            var list = new List<ReaderWriterLockSlim>( 134_217_728 + 10240 );
            do {
                try {
                    list.Add( new ReaderWriterLockSlim() ); //134,217,728
                }
                /*
                catch ( OutOfMemoryException ) {
                    Console.WriteLine( "Trimming list.." );
                    list.TrimExcess();
                    Console.WriteLine( "Collecting garbage.." );
                    GC.Collect();
                }
                */
                catch ( Exception exception ) {
                    Debug.WriteLine( exception.ToString() );

                    break;
                }
            } while ( true );
            Console.WriteLine( $"ReaderWriterLockSlim count={list.Count}." );
        }

    }

}