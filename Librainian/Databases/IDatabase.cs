﻿// Copyright © Protiguous. All Rights Reserved. This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or
// source code (directly or derived) from our binaries, libraries, projects, solutions, or applications. All source code belongs to Protiguous@Protiguous.com unless otherwise
// specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.) Any unmodified portions of
// source code gleaned from other sources still retain their original license and our thanks goes to those Authors. If you find your code unattributed in this source code,
// please let us know so we can properly attribute you and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT responsible for Anything You Do With Our Code. We are
// NOT responsible for Anything You Do With Our Executables. We are NOT responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s). For business inquiries, please contact me at
// Protiguous@Protiguous.com. Our software can be found at "https://Protiguous.Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "IDatabase.cs" last formatted on 2020-08-14 at 8:32 PM.

#nullable enable

namespace Librainian.Databases {

	using JetBrains.Annotations;
	using Microsoft.Data.SqlClient;
	using PooledAwait;
	using System;
	using System.Collections.Generic;
	using System.Data;

	public interface IDatabase {

		TimeSpan CommandTimeout { get; set; }

		String? Sproc { get; set; }

		//void Add<T>( [NotNull] String name, SqlDbType type, [CanBeNull] T value );

		/// <summary>
		/// Opens and then closes a <see cref="SqlConnection" />.
		/// </summary>
		/// <returns></returns>
		Int32? ExecuteNonQuery( String sproc, CommandType commandType, params SqlParameter[] parameters );

		Int32? ExecuteNonQuery( String sproc, Int32 retries, CommandType commandType, params SqlParameter[] parameters );

		PooledValueTask<Int32?> ExecuteNonQueryAsync( String sproc, params SqlParameter[] parameters );

		PooledValueTask<Int32?> ExecuteNonQueryAsync( String sproc, CommandType commandType, params SqlParameter[] parameters );

		/// <summary>
		/// Returns a <see cref="DataTable" />
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="table">      </param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		Boolean ExecuteReader( [NotNull] String query, CommandType commandType, out DataTable table, params SqlParameter[] parameters );

		/// <summary>
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		PooledValueTask<DataTableReader?> ExecuteReaderAsyncDataReader( String query, CommandType commandType, params SqlParameter[] parameters );

		/// <summary>
		/// Returns a <see cref="DataTable" />
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		PooledValueTask<DataTable> ExecuteReaderDataTableAsync( String query, CommandType commandType, params SqlParameter[] parameters );

		/// <summary>
		/// <para>Returns the first column of the first row.</para>
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		TResult? ExecuteScalar<TResult>( [NotNull] String query, CommandType commandType, params SqlParameter[] parameters );

		/// <summary>
		/// <para>Returns the first column of the first row.</para>
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		PooledValueTask<TResult?> ExecuteScalarAsync<TResult>( [NotNull] String query, CommandType commandType, params SqlParameter[] parameters );

		/// <summary>
		/// Overwrites the <paramref name="table" /> contents with data from the <paramref name="sproc" />.
		/// <para>Note: Include the parameters after the sproc.</para>
		/// <para>Can throw exceptions on connecting or executing the sproc.</para>
		/// </summary>
		/// <param name="sproc">      </param>
		/// <param name="commandType"></param>
		/// <param name="table">      </param>
		/// <param name="parameters"> </param>
		PooledValueTask<Boolean> FillTableAsync( [NotNull] String sproc, CommandType commandType, [NotNull] DataTable table, params SqlParameter[] parameters );

		/// <summary>
		/// <para>Run a query, no rows expected to be read.</para>
		/// <para>Does not catch any exceptions.</para>
		/// </summary>
		/// <param name="sproc">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		PooledValueTask NonQueryAsync( [NotNull] String sproc, CommandType commandType, params SqlParameter[] parameters );

		DataTableReader QueryAdHoc( [NotNull] String sql, params SqlParameter[] parameters );

		PooledValueTask<DatabaseServer> QueryAdhocAsync( [NotNull] String sql, [CanBeNull] params SqlParameter?[]? parameters );

		PooledValueTask<DataTableReader> QueryAdhocReaderAsync( [NotNull] String sql, params SqlParameter[] parameters );

		/// <summary>
		/// <para>Connect and then run <paramref name="sproc" />.</para>
		/// </summary>
		/// <param name="sproc">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		PooledValueTask<SqlDataReader?> QueryAsync( [NotNull] String sproc, params SqlParameter[] parameters );

		/// <summary>
		/// <para>Connect and then run <paramref name="sproc" />.</para>
		/// </summary>
		/// <param name="sproc">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		PooledValueTask<SqlDataReader?> QueryAsync( [NotNull] String sproc, CommandType commandType, params SqlParameter[] parameters );

		/// <summary>
		/// Returns a <see cref="IEnumerable{T}" />
		/// </summary>
		/// <param name="query">      </param>
		/// <param name="commandType"></param>
		/// <param name="parameters"> </param>
		/// <returns></returns>
		IEnumerable<TResult>? QueryList<TResult>( [NotNull] String query, CommandType commandType, params SqlParameter[] parameters );

		PooledValueTask<Int32?> RunSprocAsync( String sproc, params SqlParameter[] parameters );

		void UseDatabase( [NotNull] String dbName );

		PooledValueTask UseDatabaseAsync( [NotNull] String dbName );
	}
}