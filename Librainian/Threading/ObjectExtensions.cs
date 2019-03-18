// Copyright � Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ObjectExtensions.cs" belongs to Protiguous@Protiguous.com and
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
//     paypal@AIBrain.Org
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
// Project: "Librainian", "ObjectExtensions.cs" was last formatted by Protiguous on 2018/10/11 at 5:07 PM.

namespace Librainian.Threading {

	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using JetBrains.Annotations;

	/// <summary>
	///     Code pulled from https://raw.githubusercontent.com/Burtsev-Alexey/net-object-deep-copy/master/ObjectExtensions.cs
	/// </summary>
	public static class ObjectExtensions {

		private static readonly MethodInfo CloneMethod = typeof( Object ).GetMethod( "MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance );

		private static void CopyFields( Object originalObject, IDictionary<Object, Object> visited, Object cloneObject, [NotNull] Type typeToReflect,
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, [CanBeNull] Func<FieldInfo, Boolean> filter = null ) {
			foreach ( var fieldInfo in typeToReflect.GetFields( bindingFlags ) ) {
				if ( filter?.Invoke( fieldInfo ) == false ) {
					continue;
				}

				if ( IsPrimitive( fieldInfo.FieldType ) ) {
					continue;
				}

				var originalFieldValue = fieldInfo.GetValue( originalObject );
				var clonedFieldValue = InternalCopy( originalFieldValue, visited );
				fieldInfo.SetValue( cloneObject, clonedFieldValue );
			}
		}

		private static Object InternalCopy( [CanBeNull] Object originalObject, IDictionary<Object, Object> visited ) {
			if ( originalObject == null ) {
				return null;
			}

			var typeToReflect = originalObject.GetType();

			if ( IsPrimitive( typeToReflect ) ) {
				return originalObject;
			}

			if ( visited.ContainsKey( originalObject ) ) {
				return visited[ originalObject ];
			}

			if ( typeof( Delegate ).IsAssignableFrom( typeToReflect ) ) {
				return null;
			}

			var cloneObject = CloneMethod.Invoke( originalObject, null );

			if ( typeToReflect.IsArray ) {
				var arrayType = typeToReflect.GetElementType();

				if ( arrayType != null && IsPrimitive( arrayType ) == false ) {
					var clonedArray = ( Array )cloneObject;
					clonedArray.ForEach( ( array, indices ) => array.SetValue( InternalCopy( clonedArray.GetValue( indices ), visited ), indices ) );
				}
			}

			visited.Add( originalObject, cloneObject );
			CopyFields( originalObject, visited, cloneObject, typeToReflect );
			RecursiveCopyBaseTypePrivateFields( originalObject, visited, cloneObject, typeToReflect );

			return cloneObject;
		}

		private static void RecursiveCopyBaseTypePrivateFields( Object originalObject, IDictionary<Object, Object> visited, Object cloneObject, [NotNull] Type typeToReflect ) {
			if ( null == typeToReflect.BaseType ) {
				return;
			}

			RecursiveCopyBaseTypePrivateFields( originalObject, visited, cloneObject, typeToReflect.BaseType );
			CopyFields( originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate );
		}

		/// <summary>
		///     Returns a deep copy of this object.
		/// </summary>
		/// <param name="originalObject"></param>
		/// <returns></returns>
		public static Object Copy( this Object originalObject ) => InternalCopy( originalObject, new Dictionary<Object, Object>( new ReferenceEqualityComparer() ) );

		public static T Copy<T>( this T original ) => ( T )Copy( ( Object )original );

		[CanBeNull]
		public static Object GetPrivateFieldValue<T>( [NotNull] this T instance, [NotNull] String fieldName ) {
			if ( instance == null ) {
				throw new ArgumentNullException( paramName: nameof( instance ) );
			}

			if ( String.IsNullOrWhiteSpace( value: fieldName ) ) {
				throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( fieldName ) );
			}

			var type = instance.GetType();
			var info = type.GetField( fieldName, BindingFlags.NonPublic | BindingFlags.Instance );

			if ( info == null ) {
				throw new ArgumentException( $"{type.FullName} does not contain the private field '{fieldName}'." );
			}

			return info.GetValue( instance );
		}

		public static Boolean IsPrimitive<T>( [NotNull] this T type ) {
			if ( type is String ) {
				return true;
			}

			var gt = type.GetType();

			return gt.IsValueType && gt.IsPrimitive;
		}
	}
}