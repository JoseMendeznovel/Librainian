﻿// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "DurableDatabase.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "DurableDatabase.cs" was last formatted by Protiguous on 2020/03/16 at 3:04 PM.

namespace Librainian.Databases {

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensions;
    using JetBrains.Annotations;
    using Logging;
    using Maths;
    using Parsing;
    using Utilities;

    public class DurableDatabase : ABetterClassDispose {

        [NotNull]
        private String ConnectionString { get; }

        private UInt16 Retries { get; }

        [NotNull]
        private ThreadLocal<SqlConnection> SqlConnections { get; }

        public CancellationTokenSource CancelConnection { get; } = new CancellationTokenSource();

        /// <summary>A database connection attempts to stay connected in the event of an unwanted disconnect.</summary>
        /// <param name="connectionString"></param>
        /// <param name="retries">         </param>
        /// <exception cref="InvalidOperationException"></exception>
        public DurableDatabase( [NotNull] String connectionString, UInt16 retries ) {
            if ( String.IsNullOrWhiteSpace( value: connectionString ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( connectionString ) );
            }

            this.Retries = retries;
            this.ConnectionString = connectionString;

            this.SqlConnections = new ThreadLocal<SqlConnection>( valueFactory: () => {

                var connection = new SqlConnection( connectionString: this.ConnectionString );
                connection.StateChange += this.SqlConnection_StateChange;

                return connection;
            }, trackAllValues: true );

            var test = this.OpenConnection(); //try/start the current thread's open;

            if ( null == test ) {
                var builder = new SqlConnectionStringBuilder( connectionString: this.ConnectionString );

                throw new InvalidOperationException( message: $"Unable to connect to {builder.DataSource}" );
            }
        }

        [CanBeNull]
        private SqlConnection OpenConnection() {
            if ( this.SqlConnections.Value.State == ConnectionState.Open ) {
                return this.SqlConnections.Value;
            }

            try {
                this.SqlConnections.Value.Open();

                return this.SqlConnections.Value;
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return null;
        }

        /// <summary>Return true if connected.</summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private Boolean ReOpenConnection( [CanBeNull] Object? sender ) {
            if ( this.CancelConnection.IsCancellationRequested ) {
                return default;
            }

            if ( !( sender is SqlConnection connection ) ) {
                return default;
            }

            var retries = this.Retries;

            do {
                retries--;

                try {
                    if ( this.CancelConnection.IsCancellationRequested ) {
                        return default;
                    }

                    connection.Open();

                    if ( connection.State == ConnectionState.Open ) {
                        return true;
                    }
                }
                catch ( SqlException exception ) {
                    exception.Log();
                }
                catch ( DbException exception ) {
                    exception.Log();
                }
            } while ( retries > 0 );

            return default;
        }

        private void SqlConnection_StateChange( [CanBeNull] Object? sender, [NotNull] StateChangeEventArgs e ) {
            switch ( e.CurrentState ) {
                case ConnectionState.Closed:
                    this.ReOpenConnection( sender: sender );

                    break;

                case ConnectionState.Open: break; //do nothing

                case ConnectionState.Connecting:
                    Thread.SpinWait( iterations: 99 ); //TODO pooa.

                    break;

                case ConnectionState.Executing: break; //do nothing

                case ConnectionState.Fetching: break; //do nothing

                case ConnectionState.Broken:
                    this.ReOpenConnection( sender: sender );

                    break;

                default: throw new ArgumentOutOfRangeException();
            }
        }

        public override void DisposeManaged() {
            if ( !this.CancelConnection.IsCancellationRequested ) {
                this.CancelConnection.Cancel();
            }

            foreach ( var connection in this.SqlConnections.Values ) {
                switch ( connection.State ) {
                    case ConnectionState.Open:
                        connection.Close();

                        break;

                    case ConnectionState.Closed: break;

                    case ConnectionState.Connecting:
                        connection.Close();

                        break;

                    case ConnectionState.Executing:
                        connection.Close();

                        break;

                    case ConnectionState.Fetching:
                        connection.Close();

                        break;

                    case ConnectionState.Broken:
                        connection.Close();

                        break;

                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>Opens and then closes a <see cref="SqlConnection" />.</summary>
        /// <returns></returns>
        public Int32? ExecuteNonQuery( [CanBeNull] String query, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( paramName: nameof( query ) );
            }

            try {
                using var command = new SqlCommand( cmdText: query, connection: this.OpenConnection() ) {
                    CommandType = CommandType.Text
                };

                if ( null != parameters ) {
                    command.Parameters?.AddRange( values: parameters );
                }

                return command.ExecuteNonQuery();
            }
            catch ( SqlException exception ) {
                exception.Log();
            }
            catch ( DbException exception ) {
                exception.Log();
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        public Int32? ExecuteNonQuery( [CanBeNull] String query, Int32 retries, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( paramName: nameof( query ) );
            }

            TryAgain:

            try {
                using var command = new SqlCommand( cmdText: query, connection: this.OpenConnection() ) {
                    CommandType = CommandType.StoredProcedure
                };

                if ( null != parameters ) {
                    command.Parameters?.AddRange( values: parameters );
                }

                return command.ExecuteNonQuery();
            }
            catch ( InvalidOperationException ) {

                //timeout probably
                retries--;

                if ( retries.Any() ) {
                    goto TryAgain;
                }
            }
            catch ( SqlException exception ) {
                exception.Log();
            }
            catch ( DbException exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary></summary>
        /// <returns></returns>
        public Boolean ExecuteNonQuery( [CanBeNull] String query ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( paramName: nameof( query ) );
            }

            try {
                using ( var sqlcommand = new SqlCommand( cmdText: query, connection: this.OpenConnection() ) {
                    CommandType = CommandType.Text
                } ) {
                    sqlcommand.ExecuteNonQuery();

                    return true;
                }
            }
            catch ( SqlException exception ) {
                exception.Log();
            }
            catch ( DbException exception ) {
                exception.Log();
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        [ItemCanBeNull]
        public async Task<Int32?> ExecuteNonQueryAsync( [CanBeNull] String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( paramName: nameof( query ) );
            }

            try {
                await using ( var command = new SqlCommand( cmdText: query, connection: this.OpenConnection() ) {
                    CommandType = commandType
                } ) {
                    if ( null != parameters ) {
                        command.Parameters.AddRange( values: parameters );
                    }

                    return await command.ExecuteNonQueryAsync().ConfigureAwait( continueOnCapturedContext: false );
                }
            }
            catch ( SqlException exception ) {
                exception.Log();
            }
            catch ( DbException exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary>Returns a <see cref="DataTable" /></summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="table">      </param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        public Boolean ExecuteReader( [CanBeNull] String query, CommandType commandType, [NotNull] out DataTable table, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( paramName: nameof( query ) );
            }

            table = new DataTable();

            try {
                using ( var command = new SqlCommand( cmdText: query, connection: this.OpenConnection() ) {
                    CommandType = commandType
                } ) {
                    if ( null != parameters ) {
                        command.Parameters.AddRange( values: parameters );
                    }

                    table.BeginLoadData();

                    using ( var reader = command.ExecuteReader() ) {
                        table.Load( reader: reader );
                    }

                    table.EndLoadData();

                    return true;
                }
            }
            catch ( SqlException exception ) {
                exception.Log();
            }
            catch ( DbException exception ) {
                exception.Log();
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary>Returns a <see cref="DataTable" /></summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        [NotNull]
        public DataTable ExecuteReader( [CanBeNull] String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( paramName: nameof( query ) );
            }

            var table = new DataTable();

            try {
                using ( var command = new SqlCommand( cmdText: query, connection: this.OpenConnection() ) {
                    CommandType = commandType
                } ) {
                    if ( null != parameters ) {
                        command.Parameters.AddRange( values: parameters );
                    }

                    table.BeginLoadData();

                    using ( var reader = command.ExecuteReader() ) {
                        table.Load( reader: reader );
                    }

                    table.EndLoadData();
                }
            }
            catch ( SqlException exception ) {
                exception.Log();
            }
            catch ( DbException exception ) {
                exception.Log();
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return table;
        }

        /// <summary></summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        [ItemCanBeNull]
        public async Task<DataTableReader> ExecuteReaderAsyncDataReader( [CanBeNull] String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( paramName: nameof( query ) );
            }

            try {
                DataTable table;

                await using ( var command = new SqlCommand( cmdText: query, connection: this.OpenConnection() ) {
                    CommandType = commandType
                } ) {
                    if ( null != parameters ) {
                        command.Parameters.AddRange( values: parameters );
                    }

                    await using var reader = await command.ExecuteReaderAsync().ConfigureAwait( continueOnCapturedContext: false );
                    table = reader.ToDataTable();
                }

                return table.CreateDataReader();
            }
            catch ( SqlException exception ) {
                exception.Log();
            }

            return null;
        }

        /// <summary>Returns a <see cref="DataTable" /></summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        [ItemNotNull]
        public async Task<DataTable> ExecuteReaderDataTableAsync( [CanBeNull] String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( paramName: nameof( query ) );
            }

            var table = new DataTable();

            try {
                await using ( var command = new SqlCommand( cmdText: query, connection: this.OpenConnection() ) {
                    CommandType = commandType
                } ) {
                    if ( null != parameters ) {
                        command.Parameters.AddRange( values: parameters );
                    }

                    table.BeginLoadData();

                    await using ( var reader = await command.ExecuteReaderAsync( cancellationToken: this.CancelConnection.Token )
                                                            .ConfigureAwait( continueOnCapturedContext: false ) ) {
                        table.Load( reader: reader );
                    }

                    table.EndLoadData();
                }
            }
            catch ( SqlException exception ) {
                exception.Log();
            }
            catch ( DbException exception ) {
                exception.Log();
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return table;
        }

        /// <summary>
        ///     <para>Returns the first column of the first row.</para>
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        public (Status status, TResult result) ExecuteScalar<TResult>( [CanBeNull] String query, CommandType commandType, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( paramName: nameof( query ) );
            }

            try {
                using ( var command = new SqlCommand( cmdText: query, connection: this.OpenConnection() ) {
                    CommandType = commandType
                } ) {
                    if ( null != parameters ) {
                        command.Parameters.AddRange( values: parameters );
                    }

                    var scalar = command.ExecuteScalar();

                    if ( null == scalar || scalar == DBNull.Value || Convert.IsDBNull( value: scalar ) ) {
                        return (Status.Success, default);
                    }

                    if ( scalar is TResult result1 ) {
                        return (Status.Success, result1);
                    }

                    if ( scalar.TryCast<TResult>( result: out var result ) ) {
                        return (Status.Success, result);
                    }

                    return (Status.Success, ( TResult )Convert.ChangeType( value: scalar, conversionType: typeof( TResult ) ));
                }
            }
            catch ( SqlException exception ) {
                exception.Log();
            }
            catch ( DbException exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary>
        ///     <para>Returns the first column of the first row.</para>
        /// </summary>
        /// <param name="query">      </param>
        /// <param name="commandType"></param>
        /// <param name="parameters"> </param>
        /// <returns></returns>
        public async Task<(Status status, TResult result)> ExecuteScalarAsync<TResult>( [CanBeNull] String query, CommandType commandType,
            [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( paramName: nameof( query ) );
            }

            try {
                await using ( var command = new SqlCommand( cmdText: query, connection: this.OpenConnection() ) {
                    CommandType = commandType,
                    CommandTimeout = 0
                } ) {
                    if ( null != parameters ) {
                        command.Parameters.AddRange( values: parameters );
                    }

                    TryAgain:
                    Object scalar;

                    try {
                        scalar = await command.ExecuteScalarAsync().ConfigureAwait( continueOnCapturedContext: false );
                    }
                    catch ( SqlException exception ) {
                        if ( exception.Number == DatabaseErrors.Deadlock ) {
                            goto TryAgain;
                        }

                        throw;
                    }

                    if ( null == scalar || scalar == DBNull.Value || Convert.IsDBNull( value: scalar ) ) {
                        return (Status.Success, default);
                    }

                    if ( scalar is TResult scalarAsync ) {
                        return (Status.Success, scalarAsync);
                    }

                    if ( scalar.TryCast<TResult>( result: out var result ) ) {
                        return (Status.Success, result);
                    }

                    return (Status.Success, ( TResult )Convert.ChangeType( value: scalar, conversionType: typeof( TResult ) ));
                }
            }
            catch ( InvalidCastException exception ) {

                //TIP: check for SQLServer returning a Double when you expect a Single (float in SQL).
                exception.Log();
            }
            catch ( SqlException exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary>Returns a <see cref="DataTable" /></summary>
        /// <param name="query">     </param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [CanBeNull]
        [ItemCanBeNull]
        public IEnumerable<TResult> QueryList<TResult>( [CanBeNull] String query, [CanBeNull] params SqlParameter[] parameters ) {
            if ( query.IsNullOrWhiteSpace() ) {
                throw new ArgumentNullException( paramName: nameof( query ) );
            }

            try {
                using ( var command = new SqlCommand( cmdText: query, connection: this.OpenConnection() ) {
                    CommandType = CommandType.StoredProcedure
                } ) {
                    if ( null != parameters ) {
                        command.Parameters.AddRange( values: parameters );
                    }

                    using ( var reader = command.ExecuteReader() ) {
                        var data = GenericPopulatorExtensions.CreateList<TResult>( reader: reader );

                        return data;
                    }
                }
            }
            catch ( SqlException exception ) {
                exception.Log();
            }
            catch ( DbException exception ) {
                exception.Log();
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }
    }
}