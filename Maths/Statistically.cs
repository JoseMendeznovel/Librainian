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
// "Librainian2/Statistically.cs" was last cleaned by Rick on 2014/08/08 at 2:28 PM
#endregion

namespace Librainian.Maths {
    using System;
    using System.Runtime.Serialization;
    using System.Threading;
    using Annotations;
    using Threading;

    /// <summary>
    ///     <para>Ups: Probability between 0.0 and 100.0%</para>
    ///     <para>Downs: Probability between 0.0 and 100.0%</para>
    /// </summary>
    [DataContract( IsReference = true )]
    [Serializable]
    public class Statistically {
        public static readonly Statistically Zero = new Statistically( ups: 0, downs: 0 );

        /// <summary>
        ///     ONLY used in the getter and setter.
        /// </summary>
        [DataMember] [OptionalField] private Double _downs;

        /// <summary>
        ///     ONLY used in the getter and setter.
        /// </summary>
        [DataMember] [OptionalField] private Double _total;

        /// <summary>
        ///     ONLY used in the getter and setter.
        /// </summary>
        [DataMember] [OptionalField] private Double _ups;

        public Statistically( Double ups = 0d, Double downs = 0d ) {
            Reset( statistically: this, newUps: ups, newDowns: downs );
        }

        public Double Downs { get { return Thread.VolatileRead( ref this._downs ); } private set { Thread.VolatileWrite( ref this._downs, value ); } }

        [UsedImplicitly]
        public Boolean IsDowner { get { return this.Downs > this.Ups; } }

        [UsedImplicitly]
        public Boolean IsProtiguous { get { return this.IsUpper && !this.Downs.Near( 0 ) && !this.Ups.Near( 0 ); } }

        public Boolean IsUpper { get { return this.Ups > this.Downs; } }

        public Double Total { get { return Thread.VolatileRead( ref this._total ); } private set { Thread.VolatileWrite( ref this._total, value ); } }

        public Double Ups { get { return Thread.VolatileRead( ref this._ups ); } private set { Thread.VolatileWrite( ref this._ups, value ); } }

        [UsedImplicitly]
        public static Statistically Combine( Statistically value1, Statistically value2 ) {
            return new Statistically( ups: value1.Ups + value2.Ups, downs: value1.Downs + value2.Downs );
        }

        public static void Reset( Statistically statistically, Double newUps = 0.0, Double newDowns = 0.0 ) {
            statistically.Ups = 0d;
            statistically.Downs = 0d;
            statistically.Total = 0d;
            statistically.IncrementUps( newUps );
            statistically.IncrementDowns( newDowns );
        }

        public void Add( Statistically other ) {
            this.IncrementUps( other.Ups );
            this.IncrementDowns( other.Downs );
        }

        [UsedImplicitly]
        public Statistically Clone() {
            return new Statistically( ups: this.Ups, downs: this.Downs );
        }

        public void DecrementDowns( Double byAmount = 1d ) {
            this.Downs -= byAmount;
            this.Total -= byAmount;
        }

        public void DecrementDownsIfAny( Double byAmount = 1d ) {
            if ( this.Downs < byAmount ) {
                return;
            }
            this.Downs -= byAmount;
            this.Total -= byAmount;
        }

        public void DecrementUps( Double byAmount = 1d ) {
            this.Ups -= byAmount;
            this.Total -= byAmount;
        }

        public void DecrementUpsIfAny( Double byAmount = 1d ) {
            if ( this.Ups < byAmount ) {
                return;
            }
            this.Ups -= byAmount;
            this.Total -= byAmount;
        }

        [UsedImplicitly]
        public double GetDownProbability() {
            try {
                var total = this.Total;
                if ( !total.Near( 0 ) ) {
                    return this.Downs/total;
                }
            }
            catch ( DivideByZeroException exception ) {
                exception.Log();
            }
            return 0;
        }

        [UsedImplicitly]
        public double GetUpProbability() {
            try {
                var total = this.Total;
                if ( !total.Near( 0 ) ) {
                    return this.Ups/total;
                }
            }
            catch ( DivideByZeroException exception ) {
                exception.Log();
            }
            return 0;
        }

        /// <summary>
        ///     Increments <see cref="Downs" /> and <see cref="Total" /> by <paramref name="byAmount" />.
        /// </summary>
        /// <param name="byAmount"></param>
        public void IncrementDowns( Double byAmount = 1 ) {
            this.Downs += byAmount;
            this.Total += byAmount;
        }

        /// <summary>
        ///     Increments <see cref="Ups" /> and <see cref="Total" /> by <paramref name="byAmount" />.
        /// </summary>
        /// <param name="byAmount"></param>
        public void IncrementUps( Double byAmount = 1 ) {
            this.Ups += byAmount;
            this.Total += byAmount;
        }

        public override String ToString() {
            return String.Format( "U:{0:f1} vs D:{1:f1} out of {2:f1}", this.Ups, this.Downs, this.Total );
        }

        //public static Double Combine( Double value1, Double value2 ) { return ( value1 + value2 ) / 2D; }

        //public static Statistically Truely = new Statistically( Undecided + ( Undecided / 2 ) );
        //public static Statistically Undecided = new Statistically( 0.5D );
        //public static Statistically Falsely = new Statistically( Undecided - ( Undecided / 2 ) );

        //public static Boolean IsTruely( Statistically special ) {
        //    return special.Value >= Truely.Value;
        //}

        //public static Boolean IsFalsely( Statistically special ) {
        //    return special.Value <= Falsely.Value;
        //}

        //public static Boolean IsUndecided( Statistically special ) {
        //    return !IsTruely( special ) && !IsFalsely( special );
        //}

        //public Statistically MoreLikely() {
        //    this.Value = Combine( this, Statistically.MaxValue );
        //    return this;
        //}

        //public Statistically LessLikely() {
        //    this.Value = Combine( this, Statistically.MinValue ); ;
        //    return this;
        //}
    }
}