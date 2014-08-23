namespace Librainian.Gaming {
    using System;
    using System.Collections.Generic;

    public interface IDice {

        /// <summary>
        ///     <para>Rolls the dice to determine which side lands face-up.</para>
        /// </summary>
        /// <returns>The side which landed face-up</returns>
        UInt16 Roll();

        IEnumerable<ushort> GetLastFewRolls();
    }
}