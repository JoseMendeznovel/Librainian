﻿// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Matrix.cs" belongs to Rick@AIBrain.org and
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
// "Librainian/Librainian/Matrix.cs" was last formatted by Protiguous on 2018/05/24 at 7:13 PM.

namespace Librainian.Graphics {

    using System;

    public class Matrix {

        protected readonly Int32 Cols;

        protected readonly Single[,] matrix;

        protected readonly Int32 Rows;

        protected Matrix( Single[,] matrix ) {
            this.matrix = matrix;
            this.Rows = matrix.GetLength( 0 );
            this.Cols = matrix.GetLength( 1 );
        }

        protected Matrix( Int32 rows, Int32 cols ) {
            this.matrix = new Single[rows, cols];
            this.Rows = rows;
            this.Cols = cols;
        }

        private static Single[,] Multiply( Matrix matrix1, Matrix matrix2 ) {
            var m1Cols = matrix1.Cols;

            if ( m1Cols != matrix2.Rows ) { throw new ArgumentException(); }

            var m1Rows = matrix1.Rows;
            var m2Cols = matrix2.Cols;
            var m1 = matrix1.matrix;
            var m2 = matrix2.matrix;
            var m3 = new Single[m1Rows, m2Cols];

            for ( var i = 0; i < m1Rows; ++i ) {
                for ( var j = 0; j < m2Cols; ++j ) {
                    Single sum = 0;

                    for ( var it = 0; it < m1Cols; ++it ) { sum += m1[i, it] * m2[it, j]; }

                    m3[i, j] = sum;
                }
            }

            return m3;
        }

        protected static Single[,] Multiply( Matrix matrix, Single scalar ) {
            var rows = matrix.Rows;
            var cols = matrix.Cols;
            var m1 = matrix.matrix;
            var m2 = new Single[rows, cols];

            for ( var i = 0; i < rows; ++i ) {
                for ( var j = 0; j < cols; ++j ) { m2[i, j] = m1[i, j] * scalar; }
            }

            return m2;
        }

        public static Matrix operator *( Matrix m, Single scalar ) => new Matrix( Multiply( m, scalar ) );

        public static Matrix operator *( Matrix m1, Matrix m2 ) => new Matrix( Multiply( m1, m2 ) );

        public override String ToString() {
            var res = "";

            for ( var i = 0; i < this.Rows; ++i ) {
                if ( i > 0 ) { res += "|"; }

                for ( var j = 0; j < this.Cols; ++j ) {
                    if ( j > 0 ) { res += ","; }

                    res += this.matrix[i, j];
                }
            }

            return $"({res})";
        }
    }
}