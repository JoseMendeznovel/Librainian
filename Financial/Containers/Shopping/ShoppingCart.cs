﻿// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "ShoppingCart.cs" belongs to Rick@AIBrain.org and
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
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "ShoppingCart.cs" was last formatted by Protiguous on 2018/06/04 at 3:54 PM.

namespace Librainian.Financial.Containers.Shopping {

	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using Collections;
	using Extensions;
	using JetBrains.Annotations;
	using Magic;
	using Newtonsoft.Json;

	[JsonObject]
	public class ShoppingCart : ABetterClassDispose {

		[JsonProperty]
		private ConcurrentList<ShoppingItem> Items { get; } = new ConcurrentList<ShoppingItem>();

		public Boolean AddItem( [CanBeNull] ShoppingItem item ) => item != null && this.Items.TryAdd( item );

		public UInt32 AddItems( params ShoppingItem[] items ) {
			UInt32 added = 0;

			if ( null == items ) { return added; }

			foreach ( var item in items.Where( this.AddItem ) ) { added++; }

			return added;
		}

		public UInt32 AddItems( [CanBeNull] ShoppingItem item, UInt32 quantity ) {
			if ( item is null ) { return 0; }

			UInt32 added = 0;

			while ( quantity.Any() ) {
				if ( this.Items.TryAdd( item ) ) { added++; }

				quantity--;
			}

			return added;
		}

		/// <summary>
		///     Dispose any disposable members.
		/// </summary>
		public override void DisposeManaged() => this.Items.Dispose();

		/// <summary>
		///     Removes the first <paramref name="item" /> from the list.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public Boolean RemoveItem( [CanBeNull] ShoppingItem item ) => this.Items.Remove( item );

		public IEnumerable<KeyValuePair<ShoppingItem, Int32>> RunningList() {
			var items = new ConcurrentDictionary<ShoppingItem, Int32>();

			foreach ( var shoppingItem in this.Items ) {
				if ( !items.ContainsKey( shoppingItem ) ) { items.TryAdd( shoppingItem, 0 ); }

				items[ shoppingItem ]++;
			}

			return items;
		}

	}

}