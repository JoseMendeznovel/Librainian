// Copyright � 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "RomanConverter.cs",
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
// "Librainian/Librainian/RomanConverter.cs" was last cleaned by Protiguous on 2018/05/15 at 10:46 PM.

namespace Librainian.Maths {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using Numbers;

    /// <summary>Based on the idea from lavz24.</summary>
    /// <seealso cref="http://github.com/lavz24/DecimalToRoman/blob/master/DecimalToRoman/Converter.cs" />
    public static class RomanConverter {

        /// <summary></summary>
        public static readonly RomanNumber[] RomanValues =
            { RomanNumber.I, RomanNumber.Iv, RomanNumber.V, RomanNumber.Ix, RomanNumber.X, RomanNumber.Xl, RomanNumber.L, RomanNumber.Xc, RomanNumber.C, RomanNumber.Cd, RomanNumber.D, RomanNumber.Cm, RomanNumber.M };

        /// <summary>
        ///     <para>Returns the roman numeral for a <paramref name="number" /> between 1 and 3999.</para>
        ///     <para>Or String.Empty in case of failure.</para>
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static String ToRoman( this Int16 number ) {
            if ( !number.Between( ( Int16 )1, ( Int16 )3999 ) ) {

                // per https://en.wikipedia.org/wiki/Roman_numerals#Large_numbers
                //throw new ArgumentOutOfRangeException( "number" );
                return String.Empty;
            }

            var result = new Queue<RomanNumber>();

            var currentRoman = RomanValues.Length - 1;

            for ( var i = ( Int32 )number; i > 0; ) {
                if ( i < ( Int32 )RomanValues[currentRoman] ) { --currentRoman; }
                else {
                    result.Enqueue( RomanValues[currentRoman] );
                    i -= ( Int32 )RomanValues[currentRoman];
                }
            }

            return result.Aggregate( String.Empty, ( current, romanNumber ) => current + romanNumber.ToString() );
        }
    }
}