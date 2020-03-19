﻿// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "CoordinateF.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "CoordinateF.cs" was last formatted by Protiguous on 2020/03/16 at 3:04 PM.

#pragma warning disable RCS1138 // Add summary to documentation comment.

namespace Librainian.Graphics.DDD {

    using System;
    using System.Diagnostics;
    using System.Drawing;
    using Extensions;
    using JetBrains.Annotations;
    using Maths;
    using Maths.Ranges;
    using Newtonsoft.Json;
    using static Maths.Hashings.HashingExtensions;

    /// <summary>
    ///     <para>A 3D point, with <see cref="X" /> , <see cref="Y" /> , and <see cref="Z" /> (as <see cref="float" />).</para>
    /// </summary>
    /// <remarks>Code towards speed.</remarks>
    [Immutable]
    [DebuggerDisplay( value: "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject( memberSerialization: MemberSerialization.Fields )]
    public class CoordinateF : IEquatable<CoordinateF>, IComparable<CoordinateF> {

        /// <summary>Compares the current object with another object of the same type.</summary>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This
        /// object is less than the <paramref name="other" /> parameter. Zero This object is equal to <paramref name="other" /> . Greater than zero This object is greater than
        /// <paramref name="other" /> .
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public Int32 CompareTo( [NotNull] CoordinateF other ) => this.SquareLength.CompareTo( value: other.SquareLength );

        public Boolean Equals( CoordinateF other ) => Equals( left: this, right: other );

        public static CoordinateF Empty { get; }

        public static CoordinateF One { get; } = new CoordinateF( x: 1, y: 1, z: 1 );

        public static CoordinateF Zero { get; } = new CoordinateF( x: 0, y: 0, z: 0 );

        [JsonProperty]
        public Single SquareLength { get; }

        [JsonProperty]
        public Single X { get; }

        [JsonProperty]
        public Single Y { get; }

        [JsonProperty]
        public Single Z { get; }

        /// <summary>Initialize with a random point.</summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public CoordinateF( SingleRange x, SingleRange y, SingleRange z ) : this( x: Randem.NextFloat( min: x.Min, max: x.Max ), y: Randem.NextFloat( min: y.Min, max: y.Max ),
            z: Randem.NextFloat( min: z.Min, max: z.Max ) ) { }

        /// <summary></summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public CoordinateF( Single x, Single y, Single z ) {
            this.X = Math.Max( val1: Single.Epsilon, val2: Math.Min( val1: 1, val2: x ) );
            this.Y = Math.Max( val1: Single.Epsilon, val2: Math.Min( val1: 1, val2: y ) );
            this.Z = Math.Max( val1: Single.Epsilon, val2: Math.Min( val1: 1, val2: z ) );
            this.SquareLength = this.X * this.X + this.Y * this.Y + this.Z * this.Z;
        }

        /// <summary>Calculates the distance between two Coordinates.</summary>
        public static Single Distance( [NotNull] CoordinateF left, [NotNull] CoordinateF right ) {
            var num1 = left.X - right.X;
            var num2 = left.Y - right.Y;
            var num3 = left.Z - right.Z;

            return ( Single )Math.Sqrt( d: num1 * num1 + num2 * num2 + num3 * num3 );
        }

        /// <summary>static comparison.</summary>
        /// <param name="left"></param>
        /// <param name="right"> </param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] CoordinateF left, [CanBeNull] CoordinateF right ) {
            if ( ReferenceEquals( objA: left, objB: right ) ) {
                return true;
            }

            if ( left is null ) {
                return default;
            }

            if ( right is null ) {
                return default;
            }

            if ( left.X < right.X ) {
                return default;
            }

            if ( left.X > right.X ) {
                return default;
            }

            if ( left.Y < right.Y ) {
                return default;
            }

            if ( left.Y > right.Y ) {
                return default;
            }

            if ( left.Z < right.Z ) {
                return default;
            }

            return !( left.Z > right.Z );
        }

        public static implicit operator Point( [NotNull] CoordinateF coordinate ) => new Point( x: ( Int32 )coordinate.X, y: ( Int32 )coordinate.Y );

        public static implicit operator PointF( [NotNull] CoordinateF coordinate ) => new PointF( x: coordinate.X, y: coordinate.Y );

        /// <summary>Returns a new Coordinate as a unit Coordinate. The result is a Coordinate one unit in length pointing in the same direction as the original Coordinate.</summary>
        [NotNull]
        public static CoordinateF Normalize( [NotNull] CoordinateF coordinate ) {
            var num = 1.0f / coordinate.SquareLength;

            return new CoordinateF( x: coordinate.X * num, y: coordinate.Y * num, z: coordinate.Z * num );
        }

        [NotNull]
        public static CoordinateF operator -( [NotNull] CoordinateF v1, [NotNull] CoordinateF v2 ) => new CoordinateF( x: v1.X - v2.X, y: v1.Y - v2.Y, z: v1.Z - v2.Z );

        public static Boolean operator !=( [CanBeNull] CoordinateF left, [CanBeNull] CoordinateF right ) => !Equals( left: left, right: right );

        public static Boolean operator ==( [CanBeNull] CoordinateF left, [CanBeNull] CoordinateF right ) => Equals( left: left, right: right );

        public Double DistanceTo( [CanBeNull] CoordinateF to ) {
            if ( to == default ) {
                return 0; //BUG ?
            }

            var dx = this.X - to.X;
            var dy = this.Y - to.Y;
            var dz = this.Z - to.Z;

            return Math.Sqrt( d: dx * dx + dy * dy + dz * dz );
        }

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) {
                return default;
            }

            return obj is CoordinateF f && Equals( left: this, right: f );
        }

        public override Int32 GetHashCode() => GetHashCodes( this.X, this.Y, this.Z );

        [NotNull]
        public override String ToString() => $"{this.X}, {this.Y}, {this.Z}";
    }
}