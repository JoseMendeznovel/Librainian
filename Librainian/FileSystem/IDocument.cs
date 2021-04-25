﻿// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// 
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "IDocument.cs" last touched on 2021-03-07 at 3:17 AM by Protiguous.

#nullable enable

namespace Librainian.FileSystem {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Net;
	using System.Runtime.Serialization;
	using System.Security;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Maths;
	using Maths.Numbers;
	using PooledAwait;
	

	public interface IDocument : IEquatable<IDocument>, IAsyncEnumerable<Byte> {

		/// <summary>
		///     Largest amount of memory that will be allocated for file reads.
		/// </summary>
		/// <remarks>About 1.8GB (90% of 2GB)</remarks>
		const Int32 MaximumBufferSize = ( Int32 ) ( Int32.MaxValue * 0.9 );

		/// <summary>Local file creation <see cref="DateTime" />.</summary>
		public DateTime? CreationTime { get; set; }

		/// <summary>Gets or sets the file creation time, in coordinated universal time (UTC).</summary>
		public DateTime? CreationTimeUtc { get; set; }

		//FileAttributeData FileAttributeData { get; }

		/// <summary>Gets or sets the time the current file was last accessed.</summary>
		public DateTime? LastAccessTime { get; set; }

		/// <summary>Gets or sets the UTC time the file was last accessed.</summary>
		public DateTime? LastAccessTimeUtc { get; set; }

		/// <summary>Gets or sets the time when the current file or directory was last written to.</summary>
		public DateTime? LastWriteTime { get; set; }

		/// <summary>Gets or sets the UTC datetime when the file was last written to.</summary>
		public DateTime? LastWriteTimeUtc { get; set; }

		public PathTypeAttributes PathTypeAttributes { get; }

		/// <summary>Anything that can be temp stored can go in this. Not serialized. Defaults to be used for internal locking.</summary>
		[CanBeNull]
		public Object? Tag { get; set; }

		public Boolean DeleteAfterClose { get; set; }

		/// <summary>
		///     <para>Just the file's name, including the extension.</para>
		/// </summary>
		/// <example>
		///     <code>new Document("C:\Temp\Test.text").FileName() == "Test.text"</code>
		/// </example>
		/// <see cref="Pri.LongPath.Path.GetFileName" />
		[NotNull]
		public String FileName { get; }

		/// <summary>
		///     <para>Just the file's name, including the extension.</para>
		/// </summary>
		/// <see cref="Pri.LongPath.Path.GetFileNameWithoutExtension" />
		[NotNull]
		public String Name { get; }

		/// <summary>
		///     Represents the fully qualified path of the file.
		///     <para>Fully qualified "Drive:\Path\Folder\Filename.Ext"</para>
		/// </summary>
		[NotNull]
		public String FullPath { get; }

		Byte[]? Buffer { get; set; }

		Boolean IsBufferLoaded { get; }

		FileStream? Writer { get; set; }

		StreamWriter? WriterStream { get; set; }

		/// <summary>Returns the length of the file (if it exists).</summary>
		public PooledValueTask<UInt64?> Length( CancellationToken cancellationToken );

		/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Byte" />.</summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
		public IAsyncEnumerable<Byte> AsBytes( CancellationToken cancellationToken );

		/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int32" />.</summary>
		/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
		/// <returns></returns>
		public IAsyncEnumerable<Int32> AsInt32( CancellationToken cancellationToken );

		/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int64" />.</summary>
		/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
		/// <returns></returns>
		public IAsyncEnumerable<Int64> AsInt64( CancellationToken cancellationToken );

		/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Guid" />.</summary>
		/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
		/// <returns></returns>
		[NotNull]
		public IAsyncEnumerable<Guid> AsGuids( CancellationToken cancellationToken );

		/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="UInt64" />.</summary>
		/// <returns></returns>
		/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
		public IAsyncEnumerable<UInt64> AsUInt64( CancellationToken cancellationToken );

		/// <summary>Deletes the file.</summary>
		public PooledValueTask Delete( CancellationToken cancellationToken );

		/// <summary>Returns whether the file exists.</summary>
		[Pure]
		public PooledValueTask<Boolean> Exists( CancellationToken cancellationToken );

		public IFolder ContainingingFolder();

		/// <summary>
		///     <para>Clone the entire document to the <paramref name="destination" /> as quickly as possible.</para>
		///     <para>this will OVERWRITE any <see cref="destination" /> file.</para>
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="progress">   </param>
		/// <param name="eta">        </param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public PooledValueTask<(Status success, TimeSpan timeElapsed)> CloneDocument(
			[NotNull] IDocument destination,
			[NotNull] IProgress<Single> progress,
			[NotNull] IProgress<TimeSpan> eta,
			CancellationToken cancellationToken
		);

		/*
        /// <summary>Returns the <see cref="WebClient" /> if a file copy was started.</summary>
        /// <param name="destination"></param>
        /// <param name="onProgress"> </param>
        /// <param name="onCompleted"></param>
        /// <returns></returns>
        (PooledValueTask? task, Exception? exception, Status Exception) Copy( [NotNull] IDocument destination,
            [NotNull] Action<(IDocument, UInt64 bytesReceived, UInt64 totalBytesToReceive)> onProgress, [NotNull] Action onCompleted );
        */

		public PooledValueTask<Int32?> CRC32( CancellationToken cancellationToken );

		/// <summary>Returns a lowercase hex-string of the hash.</summary>
		/// <returns></returns>
		public PooledValueTask<String?> CRC32Hex( CancellationToken cancellationToken );

		public PooledValueTask<Int64?> CRC64( CancellationToken cancellationToken );

		/// <summary>Returns a lowercase hex-string of the hash.</summary>
		/// <returns></returns>
		public PooledValueTask<String?> CRC64Hex( CancellationToken cancellationToken );

		/// <summary>
		///     <para>Downloads (replaces) the local document with the specified <paramref name="source" />.</para>
		///     <para>Note: will replace the content of the this <see cref="IDocument" />.</para>
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public PooledValueTask<(Exception? exception, WebHeaderCollection? responseHeaders)> DownloadFile( [NotNull] Uri source );

		/// <summary>
		///     <para>Computes the extension of the <see cref="FileName" />, including the prefix ".".</para>
		/// </summary>
		[NotNull]
		public String Extension();

		/// <summary>Returns the size of the file, if it exists.</summary>
		/// <returns></returns>
		public PooledValueTask<UInt64?> Size( CancellationToken cancellationToken );

		//TODO PooledValueTask<UInt64?> RealSizeOnDisk( CancellationToken cancellationToken );
		//TODO PooledValueTask<UInt64?> AllocatedSizeOnDisk( CancellationToken cancellationToken );

		/// <summary>
		///     <para>If the file does not exist, it is created.</para>
		///     <para>Then the <paramref name="text" /> is appended to the file.</para>
		/// </summary>
		/// <param name="text"></param>
		/// <param name="cancellationToken"></param>
		public PooledValueTask<IDocument> AppendText( [NotNull] String text, CancellationToken cancellationToken );

		/// <summary>
		///     <para>To compare the contents of two <see cref="IDocument" /> use SameContent( IDocument,IDocument).</para>
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Boolean Equals( Object other );

		/// <summary>(file name, not contents)</summary>
		/// <returns></returns>
		public Int32 GetHashCode();

		/// <summary>Returns the filename, without the extension.</summary>
		/// <returns></returns>
		public String JustName();

		/// <summary>
		///     <para>
		///         Can we allocate a full 2GB buffer?
		///     </para>
		///     <para>See the file "App.config" for setting gcAllowVeryLargeObjects to true.</para>
		/// </summary>
		public PooledValueTask<Int32?> GetOptimalBufferSize( CancellationToken cancellationToken );

		/// <summary>Attempt to start the process.</summary>
		/// <param name="arguments"></param>
		/// <param name="verb">     "runas" is elevated</param>
		/// <param name="useShell"></param>
		/// <returns></returns>
		public PooledValueTask<Process?> Launch( [CanBeNull] String? arguments = null, String verb = "runas", Boolean useShell = false );

		/// <summary>
		///     Attempt to return an object Deserialized from a JSON text file.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="progress"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public PooledValueTask<(Status status, T? obj)> LoadJSON<T>( IProgress<ZeroToOne>? progress, CancellationToken cancellationToken );

		public PooledValueTask<String> ReadStringAsync();

		/// <summary>
		///     <para>Performs a byte by byte file comparison, but ignores the <see cref="IDocument" /> file names.</para>
		/// </summary>
		/// <param name="right"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="SecurityException"></exception>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="UnauthorizedAccessException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="NotSupportedException"></exception>
		/// <exception cref="IOException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		public PooledValueTask<Boolean> SameContent( [CanBeNull] Document? right, CancellationToken cancellationToken );

		/*
		 *	//TODO Move: copy to dest under guid.guid, delete old dest, rename new dest, delete source
		 *	//TODO PooledValueTask<FileCopyData> Move( FileCopyData fileData, CancellationToken cancellationToken );
		 */

		/// <summary>
		///     Opens an existing file or creates a new file for writing.
		///     <para>Should be able to read and write from <see cref="FileStream" />.</para>
		///     <para>If there is any error opening or creating the file, <see cref="Document.Writer" /> will be null.</para>
		/// </summary>
		/// <returns></returns>
		public PooledValueTask<FileStream?> OpenWriter( Boolean deleteIfAlreadyExists, CancellationToken cancellationToken, FileShare sharingOptions = FileShare.None );

		/// <summary>
		///     Releases the <see cref="FileStream" /> opened by <see cref="OpenWriter" />.
		/// </summary>
		public void ReleaseWriter();

		/// <summary>HarkerHash (hash-by-addition)</summary>
		/// <returns></returns>
		PooledValueTask<Int32> HarkerHash32( CancellationToken cancellationToken );

		PooledValueTask<Boolean> IsAll( Byte number, CancellationToken cancellationToken );

		/// <summary>Open the file for reading and return a <see cref="Document.StreamReader" />.</summary>
		/// <returns></returns>
		StreamReader StreamReader();

		/// <summary>
		///     Open the file for writing and return a <see cref="Document.StreamWriter" />.
		///     <para>Optional <paramref name="encoding" />. Defaults to <see cref="Encoding.Unicode" />.</para>
		///     <para>Optional buffersize. Defaults to 1 MB.</para>
		/// </summary>
		/// <returns></returns>
		Task<StreamWriter?> StreamWriter( CancellationToken cancellationToken, [CanBeNull] Encoding? encoding = null, UInt32 bufferSize = MathConstants.Sizes.OneMegaByte );

		/// <summary>Return this <see cref="IDocument" /> as a JSON string.</summary>
		/// <returns></returns>
		PooledValueTask<String?> ToJSON();

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		String ToString();

		/// <summary>
		///     <para>Returns true if this <see cref="Document" /> no longer seems to exist.</para>
		/// </summary>
		/// <param name="delayBetweenRetries"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		PooledValueTask<Boolean?> TryDeleting( TimeSpan delayBetweenRetries, CancellationToken cancellationToken );

		/// <summary>Uploads this <see cref="IDocument" /> to the given <paramref name="destination" />.</summary>
		/// <param name="destination"></param>
		/// <returns></returns>
		PooledValueTask<(Exception? exception, WebHeaderCollection? responseHeaders)> UploadFile( [NotNull] Uri destination );

		/// <summary>Create and returns a new <see cref="FileInfo" /> object for <see cref="Document.FullPath" />.</summary>
		/// <see cref="Document.op_Implicit" />
		/// <see cref="Document.ToFileInfo" />
		/// <returns></returns>
		PooledValueTask<FileInfo> GetFreshInfo( CancellationToken cancellationToken );

		Task<FileCopyData> Copy( FileCopyData fileCopyData, CancellationToken cancellationToken );

		PooledValueTask<Int64> HarkerHash64( CancellationToken cancellationToken );

		/// <summary>"poor mans Decimal hash"</summary>
		/// <returns></returns>
		PooledValueTask<Decimal> HarkerHashDecimal( CancellationToken cancellationToken );

		/// <summary>Returns an enumerator that iterates through the collection.</summary>
		/// <returns>A <see cref="IEnumerator" /> that can be used to iterate through the collection.</returns>
		IAsyncEnumerator<Byte> GetEnumerator();

		/// <summary>Dispose of any <see cref="IDisposable" /> (managed) fields or properties in this method.</summary>
		void DisposeManaged();

		/// <summary>Attempt to load the entire file into memory. If it throws, it throws..</summary>
		/// <returns></returns>
		PooledValueTask<Status> LoadDocumentIntoBuffer( CancellationToken cancellationToken );

		/// <summary>Enumerates the <see cref="IDocument" /> as a sequence of <see cref="Int64" />.</summary>
		/// <param name="cancellationToken"></param>
		/// <exception cref="NotSupportedException">Thrown when the <see cref="FileStream" /> cannot be read.</exception>
		/// <returns></returns>
		IAsyncEnumerable<Decimal> AsDecimal( CancellationToken cancellationToken );

		/// <summary>
		///     <para>If the file does not exist, return <see cref="Status.Error" />.</para>
		///     <para>If an exception happens, return <see cref="Status.Exception" />.</para>
		///     <para>Otherwise, return <see cref="Status.Success" />.</para>
		/// </summary>
		/// <param name="value"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		PooledValueTask<Status> SetReadOnly( Boolean value, CancellationToken cancellationToken );

		PooledValueTask<Status> TurnOnReadonly( CancellationToken cancellationToken );

		PooledValueTask<Status> TurnOffReadonly( CancellationToken cancellationToken );

		void GetObjectData( [NotNull] SerializationInfo info, StreamingContext context );

		IAsyncEnumerable<String> ReadLines( CancellationToken cancellationToken );

		/// <summary>
		///     Synchronous version.
		/// </summary>
		/// <returns></returns>
		UInt64? GetLength();

		/// <summary>
		///     Synchronous version.
		/// </summary>
		/// <returns></returns>
		Boolean GetExists();

	}

}