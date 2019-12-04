// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Symbols.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Symbols.cs" was last formatted by Protiguous on 2019/11/25 at 3:53 PM.

namespace Librainian.Parsing {

    using System;

    /// <summary>Attempts at using text/emoji to make animations - display 1 char at a time from each string.</summary>
    public static class Animations {

        public const String BallotBoxCheck = Symbols.BallotBox + Symbols.BallotBoxWithCheck;

        public const String BallotBoxUnCheck = Symbols.BallotBoxWithCheck + Symbols.BallotBox;

        public const String Hearts = "🤍💖💓💗💛💙💚🧡💜🖤";

        public const String HorizontalDots = "‥…";

        public const String Pipes = Symbols.VerticalEllipsis + Symbols.Pipe + Symbols.TwoPipes + Symbols.TriplePipes;

        public const String StarBurst = "⁕⁑⁂";

        public const String VerticalDots = "․⁚⁝:⁞";

        //⁎

    }

    public static class Symbols {

        public const String BallotBox = "☐";

        public const String BallotBoxWithCheck = "☑";

        public const String BallotBoxWithX = "☒";

        public const String BlackStar = "★";

        public const String CheckMark = "✓";

        public const String CheckMarkHeavy = "✔";

        /// <summary>❕</summary>
        public const String Error = "❕";

        /// <summary>❌</summary>
        public const String Exception = FailBig;

        public const String Fail = "🗙";

        public const String FailBig = "❌";

        public const String Interobang1 = "‽";

        public const String Interobang2 = "⁈";

        /// <summary>Symbol for NAK</summary>
        public const String NegativeAcknowledge = "␕";

        /// <summary>N/A</summary>
        public const String NotApplicable = "ⁿ̸ₐ";

        /// <summary>N/A</summary>
        public const String NotApplicableHeavy = "ⁿ/ₐ";

        public const String Null = "␀";

        public const String Pipe = "|";

        public const String Singlespace = " ";

        public const String SkullAndCrossbones = "☠";

        public const String TriplePipes = "⦀";

        public const Char TripleTilde = '≋';

        public const String TwoPipes = "‖";

        public const String Unknown = "�";

        public const String VerticalEllipsis = "⋮";

        public const String Warning = "⚠";

        public const String WhiteStar = "☆";

        public static class Dice {

            public const String Five = "⚄";

            public const String Four = "⚃";

            public const String One = "⚀";

            public const String Six = "⚅";

            public const String Three = "⚂";

            public const String Two = "⚁";

        }

        public static class Left {

            public const Char DoubleAngle = '«';

            public const Char DoubleParenthesis = '⸨';

        }

        public static class Right {

            public const Char DoubleAngle = '»';

            public const Char DoubleParenthesis = '⸩';

        }

    }

}