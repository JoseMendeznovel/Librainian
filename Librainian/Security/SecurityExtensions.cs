﻿// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "SecurityExtensions.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "SecurityExtensions.cs" was last formatted by Protiguous on 2019/03/18 at 12:01 AM.

namespace Librainian.Security {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensions;
    using JetBrains.Annotations;
    using Logging;
    using OperatingSystem.FileSystem;

    public static class SecurityExtensions {

        /// <summary>
        /// </summary>
        [NotNull]
        public static SHA1CryptoServiceProvider CryptoProvider { get; } = new SHA1CryptoServiceProvider();

        [NotNull]
        public static Byte[] Entropy { get; } = Encoding.Unicode.GetBytes( s: $"{EntropyPhrase1} {EntropyPhrase2} {EntropyPhrase3}" );

        /// <summary>
        ///     threadsafe MD5 hashers
        /// </summary>
        [NotNull]
        public static ThreadLocal<MD5> MD5ThreadLocals { get; } = new ThreadLocal<MD5>( valueFactory: System.Security.Cryptography.MD5.Create );

        /// <summary>
        ///     Provide to each thread its own <see cref="SHA256Managed" />.
        /// </summary>
        [NotNull]
        public static ThreadLocal<SHA256Managed> SHA256ThreadLocals { get; } =
            new ThreadLocal<SHA256Managed>( valueFactory: () => new SHA256Managed(), trackAllValues: false );

        /// <summary>
        ///     Provide to each thread its own <see cref="SHA384Managed" />.
        /// </summary>
        [NotNull]
        public static ThreadLocal<SHA384Managed> SHA384ThreadLocals { get; } =
            new ThreadLocal<SHA384Managed>( valueFactory: () => new SHA384Managed(), trackAllValues: false );

        /// <summary>
        ///     Provide to each thread its own <see cref="SHA512Managed" />.
        /// </summary>
        [NotNull]
        public static ThreadLocal<SHA512Managed> SHA512ThreadLocals { get; } =
            new ThreadLocal<SHA512Managed>( valueFactory: () => new SHA512Managed(), trackAllValues: false );

        [NotNull]
        public static ThreadLocal<Lazy<SHA256Managed>> ThreadLocalSHA256Lazy { get; } =
            new ThreadLocal<Lazy<SHA256Managed>>( valueFactory: () => new Lazy<SHA256Managed>( valueFactory: () => new SHA256Managed() ) );

        public const String EntropyPhrase1 = "ZuZgBzuvvtn98vmmmt4vn4v9vwcaSjUtOmSkrA8Wo3ATOlMp3qXQmRQOdWyFFgJU";

        public const String EntropyPhrase2 = "KSOPFJyNMPgchzs7OH12MFHnGOMftm9RZwrwA1vwb66q3nqC9HtKuMzAY4fhtN8F";

        public const String EntropyPhrase3 = "XtXowrE3jz6UESvqb63bqw36nxtxTo0VYH5YJLbsxE4TR20c5nN9ocVxyabim2SX";

        private static readonly ThreadLocal<TripleDESCryptoServiceProvider> _tripleDesCryptoServiceProvider =
            new ThreadLocal<TripleDESCryptoServiceProvider>( () => new TripleDESCryptoServiceProvider(), false );

        [NotNull]
        private static Byte[] Uid( [NotNull] String s ) {
            var numArray = new Byte[ s.Length ];

            for ( var i = 0; i < s.Length; i++ ) {
                numArray[ i ] = ( Byte ) ( s[ index: i ] & '\u007F' );
            }

            return numArray;
        }

        [NotNull]
        public static Task<Byte[]> ComputeMD5HashAsync( String filename ) =>
            Task.Run( function: () => {

                using ( var fs = new FileStream( filename, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read, bufferSize: 1073741824, useAsync: true ) ) {
                    return MD5ThreadLocals.Value.ComputeHash( fs );
                }
            } );

        /// <summary>
        ///     To encrypt use <seealso cref="EncryptDES" />.
        /// </summary>
        /// <param name="textToDecrypt"></param>
        /// <returns></returns>
        [NotNull]
        public static String DecryptDES( [NotNull] this String textToDecrypt ) {
            if ( textToDecrypt == null ) {
                throw new ArgumentNullException( paramName: nameof( textToDecrypt ) );
            }

            using ( var ms = new MemoryStream() ) {
                using ( var transform = _tripleDesCryptoServiceProvider.Value.CreateDecryptor() ) {
                    using ( var cs = new CryptoStream( ms, transform, CryptoStreamMode.Write ) ) {
                        var buffer = Convert.FromBase64String( textToDecrypt );
                        cs.Write( buffer, 0, buffer.Length );
                        cs.FlushFinalBlock();

                        return Encoding.Unicode.GetString( ms.ToArray() );
                    }
                }
            }
        }

        [NotNull]
        public static String DecryptRSA( [NotNull] this String inputString, Int32 keySize, [NotNull] String xmlString ) {

            // TODO: Add Proper Exception Handlers
            if ( inputString == null ) {
                throw new ArgumentNullException( nameof( inputString ) );
            }

            if ( xmlString == null ) {
                throw new ArgumentNullException( nameof( xmlString ) );
            }

            var rsaCryptoServiceProvider = new RSACryptoServiceProvider( dwKeySize: keySize );
            rsaCryptoServiceProvider.FromXmlString( xmlString: xmlString );
            var base64BlockSize = keySize / 8 % 3 != 0 ? (keySize / 8 / 3 * 4) + 4 : keySize / 8 / 3 * 4;
            var iterations = inputString.Length / base64BlockSize;
            var arrayList = new ArrayList(); //ugh

            for ( var i = 0; i < iterations; i++ ) {
                var encryptedBytes = Convert.FromBase64String( s: inputString.Substring( startIndex: base64BlockSize * i, base64BlockSize ) );

                // Be aware the RSACryptoServiceProvider reverses the order of encrypted bytes after
                // encryption and before decryption. If you do not require compatibility with
                // Microsoft Cryptographic API (CAPI) and/or other vendors. Comment out the next
                // line and the corresponding one in the EncryptString function.
                Array.Reverse( array: encryptedBytes );
                arrayList.AddRange( c: rsaCryptoServiceProvider.Decrypt( rgb: encryptedBytes, fOAEP: true ) );
            }

            return !( arrayList.ToArray( type: typeof( Byte ) ) is Byte[] ba ) ? String.Empty : Encoding.Unicode.GetString( bytes: ba );
        }

        [NotNull]
        public static SecureString DecryptString( this String encryptedData ) {
            try {
                var decryptedData = ProtectedData.Unprotect( encryptedData: Convert.FromBase64String( s: encryptedData ), optionalEntropy: Entropy,
                    scope: DataProtectionScope.CurrentUser );

                return ToSecureString( input: Encoding.Unicode.GetString( bytes: decryptedData ) );
            }
            catch {
                return new SecureString();
            }
        }

        [NotNull]
        public static String DecryptStringUsingRegistryKey( [NotNull] this String decryptValue, [NotNull] String privateKey ) {

            // this is the variable that will be returned to the user
            if ( decryptValue == null ) {
                throw new ArgumentNullException( nameof( decryptValue ) );
            }

            if ( privateKey == null ) {
                throw new ArgumentNullException( nameof( privateKey ) );
            }

            var decryptedValue = String.Empty;

            // Create the CspParameters object which is used to create the RSA provider without it generating a new private/public key. Parameter value of 1 indicates RSA provider type
            // - 13 would indicate DSA provider
            var csp = new CspParameters( dwTypeIn: 1 ) {
                KeyContainerName = privateKey, ProviderName = "Microsoft Strong Cryptographic Provider"
            };

            // Registry key name containing the RSA private/public key

            // Supply the provider name

            try {

                //Create new RSA object passing our key info
                var rsa = new RSACryptoServiceProvider( parameters: csp );

                // Before decryption we must convert this ugly String into a byte array
                var valueToDecrypt = Convert.FromBase64String( s: decryptValue );

                // Decrypt the passed in String value - Again the false value has to do with padding
                var plainTextValue = rsa.Decrypt( rgb: valueToDecrypt, fOAEP: false );

                // Extract our decrypted byte array into a String value to return to our user
                decryptedValue = Encoding.UTF8.GetString( bytes: plainTextValue );
            }
            catch ( CryptographicException exception ) {
                exception.Log();
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return decryptedValue;
        }

        /// <summary>
        ///     To decrypt use <see cref="DecryptDES" />.
        /// </summary>
        /// <param name="textToEncrypt"></param>
        /// <returns></returns>
        [NotNull]
        public static String EncryptDES( [NotNull] this String textToEncrypt ) {
            if ( textToEncrypt == null ) {
                throw new ArgumentNullException( paramName: nameof( textToEncrypt ) );
            }

            using ( var ms = new MemoryStream() ) {
                using ( var transform = _tripleDesCryptoServiceProvider.Value.CreateEncryptor() ) {
                    using ( var cs = new CryptoStream( ms, transform, CryptoStreamMode.Write ) ) {
                        var buffer = Encoding.Unicode.GetBytes( textToEncrypt );
                        cs.Write( buffer, 0, buffer.Length );
                        cs.FlushFinalBlock();

                        return Convert.ToBase64String( ms.ToArray() );
                    }
                }
            }
        }

        [NotNull]
        public static String EncryptRSA( [NotNull] this String inputString, Int32 dwKeySize, [NotNull] String xmlString ) {

            // TODO: Add Proper Exception Handlers
            if ( inputString == null ) {
                throw new ArgumentNullException( nameof( inputString ) );
            }

            if ( xmlString == null ) {
                throw new ArgumentNullException( nameof( xmlString ) );
            }

            var rsaCryptoServiceProvider = new RSACryptoServiceProvider( dwKeySize: dwKeySize );
            rsaCryptoServiceProvider.FromXmlString( xmlString: xmlString );
            var keySize = dwKeySize / 8;
            var bytes = Encoding.Unicode.GetBytes( s: inputString );

            // The hash function in use by the .NET RSACryptoServiceProvider here is SHA1 int
            // maxLength = ( keySize ) - 2 - ( 2 * SHA1.Create().ComputeHash( rawBytes ).Length );
            var maxLength = keySize - 42;
            var dataLength = bytes.Length;
            var iterations = dataLength / maxLength;
            var stringBuilder = new StringBuilder();

            for ( var i = 0; i <= iterations; i++ ) {
                var tempBytes = new Byte[ dataLength - (maxLength * i) > maxLength ? maxLength : dataLength - (maxLength * i) ];
                Buffer.BlockCopy( src: bytes, srcOffset: maxLength * i, dst: tempBytes, dstOffset: 0, count: tempBytes.Length );
                var encryptedBytes = rsaCryptoServiceProvider.Encrypt( rgb: tempBytes, fOAEP: true );

                // Be aware the RSACryptoServiceProvider reverses the order of encrypted bytes. It
                // does this after encryption and before decryption. If you do not require
                // compatibility with Microsoft Cryptographic API (CAPI) and/or other vendors
                // Comment out the next line and the corresponding one in the DecryptString function.
                Array.Reverse( array: encryptedBytes );

                // Why convert to base 64? Because it is the largest power-of-two base printable
                // using only ASCII characters
                stringBuilder.Append( Convert.ToBase64String( inArray: encryptedBytes ) );
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        ///     Converts the given string ( <paramref name="input" />) to an encrypted Base64 string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [NotNull]
        public static String EncryptString( [NotNull] this SecureString input ) {
            if ( input == null ) {
                throw new ArgumentNullException( paramName: nameof( input ) );
            }

            var encryptedData = ProtectedData.Protect( userData: Encoding.Unicode.GetBytes( s: ToInsecureString( input: input ) ), optionalEntropy: Entropy,
                scope: DataProtectionScope.CurrentUser );

            return Convert.ToBase64String( inArray: encryptedData );
        }

        [NotNull]
        public static String EncryptStringUsingRegistryKey( [NotNull] this String stringToEncrypt, [NotNull] String publicKey ) {

            // this is the variable that will be returned to the user
            if ( stringToEncrypt == null ) {
                throw new ArgumentNullException( nameof( stringToEncrypt ) );
            }

            if ( publicKey == null ) {
                throw new ArgumentNullException( nameof( publicKey ) );
            }

            var encryptedValue = String.Empty;

            // Create the CspParameters object which is used to create the RSA provider without it generating a new private/public key. Parameter value of 1 indicates RSA provider type
            // - 13 would indicate DSA provider
            var csp = new CspParameters( dwTypeIn: 1 ) {
                KeyContainerName = publicKey, ProviderName = "Microsoft Strong Cryptographic Provider"
            };

            // Registry key name containing the RSA private/public key

            // Supply the provider name

            try {

                //Create new RSA object passing our key info
                var rsa = new RSACryptoServiceProvider( parameters: csp );

                // Before encrypting the value we must convert it over to byte array
                var bytesToEncrypt = Encoding.UTF8.GetBytes( s: stringToEncrypt );

                // Encrypt our byte array. The false parameter has to do with padding (not to clear on this point but you can look it up and decide which is better for your use)
                var bytesEncrypted = rsa.Encrypt( rgb: bytesToEncrypt, fOAEP: false );

                // Extract our encrypted byte array into a String value to return to our user
                encryptedValue = Convert.ToBase64String( inArray: bytesEncrypted );
            }
            catch ( CryptographicException exception ) {
                exception.Log();
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return encryptedValue;
        }

        [NotNull]
        public static String GetHexString( [NotNull] this IReadOnlyList<Byte> bt ) {
            var s = String.Empty;

            for ( var i = 0; i < bt.Count; i++ ) {
                var b = bt[ index: i ];
                Int32 n = b;
                var n1 = n & 15;
                var n2 = ( n >> 4 ) & 15;

                if ( n2 > 9 ) {
                    s += ( ( Char ) ( n2 - 10 + 'A' ) ).ToString();
                }
                else {
                    s += n2.ToString();
                }

                if ( n1 > 9 ) {
                    s += ( ( Char ) ( n1 - 10 + 'A' ) ).ToString();
                }
                else {
                    s += n1.ToString();
                }

                if ( i + 1 != bt.Count && ( i + 1 ) % 2 == 0 ) {
                    s += "-";
                }
            }

            return s;
        }

        /// <summary>
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [NotNull]
        public static String GetMD5Hash( [NotNull] this String s ) {
            using ( MD5 md5 = new MD5CryptoServiceProvider() ) {
                return md5.ComputeHash( Encoding.Unicode.GetBytes( s ) ).ToHexString();
            }
        }

        /// <summary>
        ///     Uses the md5sum.exe to obtain the md5 string.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [CanBeNull]
        public static String MD5( [NotNull] this FileInfo file ) {
            if ( !file.Exists ) {
                return null;
            }

            var p = new Process {
                StartInfo = {
                    FileName = "md5sum.exe", Arguments = file.FullName, UseShellExecute = false, RedirectStandardOutput = true
                }
            };

            p.Start();
            p.WaitForExit();
            var output = p.StandardOutput.ReadToEnd();
            var result = output.Split( ' ' )[ 0 ].Substring( startIndex: 1 ).ToUpper();

            return String.IsNullOrWhiteSpace( result ) ? null : result;
        }

        [NotNull]
        public static Byte[] Sha256( [NotNull] this Byte[] input ) {
            if ( input == null ) {
                throw new ArgumentNullException( nameof( input ) );
            }

            return SHA256ThreadLocals.Value.ComputeHash( buffer: input, offset: 0, count: input.Length );
        }

        /// <summary>
        ///     <para>Compute the SHA-256 hash for the <paramref name="input" /></para>
        ///     <para>Defaults to <see cref="Encoding.UTF8" /></para>
        /// </summary>
        /// <param name="input">   </param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        [NotNull]
        public static Byte[] Sha256( [NotNull] this String input, Encoding encoding = null ) {
            if ( input == null ) {
                throw new ArgumentNullException( nameof( input ) );
            }

            if ( null == encoding ) {
                encoding = Encoding.UTF8;
            }

            return encoding.GetBytes( s: input ).Sha256();
        }

        /// <summary>
        ///     <para>Compute the SHA-384 hash for the <paramref name="input" /></para>
        ///     <para>Defaults to <see cref="Encoding.UTF8" /></para>
        /// </summary>
        /// <param name="input">   </param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        [NotNull]
        public static Byte[] Sha384( [NotNull] this String input, Encoding encoding = null ) {
            if ( input == null ) {
                throw new ArgumentNullException( nameof( input ) );
            }

            if ( null == encoding ) {
                encoding = Encoding.UTF8;
            }

            return encoding.GetBytes( s: input ).Sha384();
        }

        [NotNull]
        public static Byte[] Sha384( [NotNull] this Byte[] input ) {
            if ( input == null ) {
                throw new ArgumentNullException( nameof( input ) );
            }

            return SHA384ThreadLocals.Value.ComputeHash( buffer: input, offset: 0, count: input.Length );
        }

        /// <summary>
        ///     <para>Compute the SHA-512 hash for the <paramref name="input" /></para>
        ///     <para>Defaults to <see cref="Encoding.UTF8" /></para>
        /// </summary>
        /// <param name="input">   </param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        [NotNull]
        public static Byte[] Sha512( [NotNull] this String input, Encoding encoding = null ) {
            if ( input == null ) {
                throw new ArgumentNullException( nameof( input ) );
            }

            if ( null == encoding ) {
                encoding = Encoding.Unicode;
            }

            return encoding.GetBytes( s: input ).Sha512();
        }

        [NotNull]
        public static Byte[] Sha512( [NotNull] this Byte[] input ) {
            if ( input == null ) {
                throw new ArgumentNullException( nameof( input ) );
            }

            return SHA512ThreadLocals.Value.ComputeHash( buffer: input, offset: 0, count: input.Length );
        }

        [NotNull]
        public static String ToHexString( [NotNull] this Byte[] bytes ) {
            var sb = new StringBuilder( bytes.Length * 2 );

            foreach ( var b in bytes ) {
                sb.Append( b.ToString( "X2" ).ToUpper() );
            }

            return sb.ToString();
        }

        public static String ToInsecureString( [NotNull] this SecureString input ) {
            if ( input == null ) {
                throw new ArgumentNullException( nameof( input ) );
            }

            String returnValue;
            var ptr = Marshal.SecureStringToBSTR( s: input );

            try {
                returnValue = Marshal.PtrToStringBSTR( ptr: ptr );
            }
            finally {
                Marshal.ZeroFreeBSTR( s: ptr );
            }

            return returnValue;
        }

        [NotNull]
        public static SecureString ToSecureString( [NotNull] this String input ) {
            if ( input == null ) {
                throw new ArgumentNullException( nameof( input ) );
            }

            var secure = new SecureString();

            foreach ( var c in input ) {
                secure.AppendChar( c: c );
            }

            secure.MakeReadOnly();

            return secure;
        }

        public static Boolean TryComputeMd5ForFile( [CanBeNull] this Document document, [CanBeNull] out String md5 ) {
            md5 = null;

            try {
                if ( document == null || !File.Exists( "md5sum.exe" ) || document.Exists() == false ) {
                    return false;
                }

                var p = new Process {
                    StartInfo = {
                        FileName = "md5sum.exe", Arguments = $"\"{document.FullPath}\"", UseShellExecute = false, RedirectStandardOutput = true
                    }
                };

                p.Start();
                p.WaitForExit();
                var output = p.StandardOutput.ReadToEnd();
                md5 = output.Split( ' ' )[ 0 ].Substring( startIndex: 1 ).ToUpper();

                return !String.IsNullOrWhiteSpace( md5 ) && md5.Length == 32;
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return false;
        }

        /// <summary>
        ///     Attempt to decrypt an encrypted version of the file with the given key and salt.
        /// </summary>
        /// <param name="input">            </param>
        /// <param name="output">           </param>
        /// <param name="key">              Must be between 1 and 32767 bytes.</param>
        /// <param name="reportProgress">   </param>
        /// <param name="exceptions">       List of exceptions encountered.</param>
        /// <param name="salt">             </param>
        /// <param name="reportEveryXBytes"></param>
        /// <returns>Returns true if all is successful</returns>
        public static Boolean TryDecryptFile( [CanBeNull] this Document input, [CanBeNull] Document output, [CanBeNull] String key, Int32 salt, UInt64? reportEveryXBytes,
            Action<Single> reportProgress, [NotNull] out List<Exception> exceptions ) {
            exceptions = new List<Exception>( capacity: 1 );

            if ( input == null ) {
                exceptions.Add( item: new ArgumentNullException( nameof( input ) ) );

                return false;
            }

            if ( input.Exists() == false ) {
                exceptions.Add( item: new FileNotFoundException( $"The input file {input.FullPath} is not found." ) );

                return false;
            }

            var size = input.Size();

            if ( !size.HasValue || size <= 0 ) {
                exceptions.Add( item: new FileNotFoundException( $"The input file {input.FullPath} is empty." ) );

                return false;
            }

            var inputFileSize = ( Single ) size.Value;

            if ( output == null ) {
                exceptions.Add( item: new ArgumentNullException( nameof( output ) ) );

                return false;
            }

            if ( output.Exists() ) {
                exceptions.Add( item: new IOException( $"The output file {output.FullPath} already exists." ) );

                return false;
            }

            if ( key == null ) {
                exceptions.Add( item: new ArgumentNullException( nameof( key ) ) );

                return false;
            }

            if ( !key.Length.Between( startInclusive: 1, endInclusive: Int16.MaxValue ) ) {
                exceptions.Add( item: new ArgumentOutOfRangeException( nameof( key ) ) );

                return false;
            }

            try {
                var containingingFolder = output.ContainingingFolder();

                if ( !containingingFolder.Create() ) {
                    exceptions.Add( item: new IOException( $"Unable to write to {output.FullPath} because folder {containingingFolder} does not exist." ) );

                    return false;
                }

                using ( var aes = new AesCryptoServiceProvider() ) {
                    DeriveBytes rgb = new Rfc2898DeriveBytes( password: key, salt: Encoding.Unicode.GetBytes( s: salt.ToString() ) );

                    aes.BlockSize = 128;
                    aes.KeySize = 256;
                    aes.Key = rgb.GetBytes( cb: aes.KeySize >> 3 );
                    aes.IV = rgb.GetBytes( cb: aes.BlockSize >> 3 );
                    aes.Mode = CipherMode.CBC;

                    using ( var outputStream = new FileStream( output.FullPath, mode: FileMode.Create, access: FileAccess.Write ) ) {
                        using ( var decryptor = aes.CreateDecryptor() ) {
                            var inputStream = new FileStream( input.FullPath, mode: FileMode.Open, access: FileAccess.Read );

                            using ( var cs = new CryptoStream( stream: inputStream, transform: decryptor, mode: CryptoStreamMode.Read ) ) {
                                Int32 data;

                                while ( ( data = cs.ReadByte() ) != -1 ) {
                                    if ( null != reportEveryXBytes && null != reportProgress ) {
                                        var position = ( UInt64 ) inputStream.Position;

                                        if ( position % reportEveryXBytes.Value == 0 ) {
                                            var progress = position / inputFileSize;
                                            reportProgress( progress );
                                        }
                                    }

                                    outputStream.WriteByte( ( Byte ) data );
                                }
                            }
                        }
                    }
                }

                return output.Exists();
            }
            catch ( AggregateException exceptionss ) {
                exceptions.AddRange( collection: exceptionss.InnerExceptions );

                return false;
            }
            catch ( Exception exception ) {
                exceptions.Add( item: exception );

                return false;
            }
        }

        /// <summary>
        ///     Create an encrypted version of the given file with the given key and salt.
        /// </summary>
        /// <param name="input">            </param>
        /// <param name="output">           </param>
        /// <param name="key">              Must be between 1 and 32767 bytes.</param>
        /// <param name="salt">             </param>
        /// <param name="reportEveryXBytes"></param>
        /// <param name="reportProgress">   Reports progress every X bytes</param>
        /// <param name="exceptions">       List of exceptions encountered.</param>
        /// <returns>Returns true if all is successful</returns>
        public static Boolean TryEncryptFile( [CanBeNull] this Document input, [CanBeNull] Document output, [CanBeNull] String key, Int32 salt, UInt64? reportEveryXBytes,
            Action<Single> reportProgress, [NotNull] out List<Exception> exceptions ) {
            exceptions = new List<Exception>( capacity: 1 );

            if ( input == null ) {
                exceptions.Add( item: new ArgumentNullException( nameof( input ) ) );

                return false;
            }

            if ( input.Exists() == false ) {
                exceptions.Add( item: new FileNotFoundException( $"The input file {input.FullPath} is not found." ) );

                return false;
            }

            var size = input.Size();

            if ( !size.HasValue || size <= 0 ) {
                exceptions.Add( item: new FileNotFoundException( $"The input file {input.FullPath} is empty." ) );

                return false;
            }

            var inputFileSize = ( Single ) size.Value;

            if ( output == null ) {
                exceptions.Add( item: new ArgumentNullException( nameof( output ) ) );

                return false;
            }

            if ( output.Exists() ) {
                exceptions.Add( item: new IOException( $"The output file {output.FullPath} already exists." ) );

                return false;
            }

            if ( key == null ) {
                exceptions.Add( item: new ArgumentNullException( nameof( key ) ) );

                return false;
            }

            if ( !key.Length.Between( startInclusive: 1, endInclusive: Int16.MaxValue ) ) {
                exceptions.Add( item: new ArgumentOutOfRangeException( nameof( key ) ) );

                return false;
            }

            try {
                var rgb = new Rfc2898DeriveBytes( password: key, salt: Encoding.Unicode.GetBytes( s: salt.ToString() ) );

                var containingingFolder = output.ContainingingFolder();

                if ( !containingingFolder.Create() ) {
                    exceptions.Add( item: new IOException( $"Unable to write to {output.FullPath} because folder {containingingFolder} does not exist." ) );

                    return false;
                }

                using ( var aes = new AesCryptoServiceProvider() ) {
                    aes.BlockSize = 128;
                    aes.KeySize = 256;
                    aes.Key = rgb.GetBytes( cb: aes.KeySize >> 3 );
                    aes.IV = rgb.GetBytes( cb: aes.BlockSize >> 3 );
                    aes.Mode = CipherMode.CBC;

                    var outputStream = new FileStream( output.FullPath, mode: FileMode.Create, access: FileAccess.Write );

                    if ( !outputStream.CanWrite ) {
                        exceptions.Add( item: new IOException( $"Unable to write to {output.FullPath}." ) );

                        return false;
                    }

                    using ( var encryptor = aes.CreateEncryptor() ) {
                        using ( var cryptoStream = new CryptoStream( stream: outputStream, transform: encryptor, mode: CryptoStreamMode.Write ) ) {
                            using ( var inputStream = new FileStream( input.FullPath, mode: FileMode.Open, access: FileAccess.Read ) ) {
                                if ( !inputStream.CanRead || !inputStream.CanSeek ) {
                                    exceptions.Add( item: new IOException( $"Unable to read from {input.FullPath}." ) );

                                    return false;
                                }

                                inputStream.Seek( offset: 0, origin: SeekOrigin.Begin );
                                Int32 data;

                                //TODO put a 64k buffer here instead of byte-by-byte
                                while ( ( data = inputStream.ReadByte() ) != -1 ) {
                                    if ( null != reportEveryXBytes && null != reportProgress ) {
                                        var position = ( UInt64 ) inputStream.Position;

                                        if ( position % reportEveryXBytes.Value == 0 ) {
                                            var progress = position / inputFileSize;
                                            reportProgress( progress );
                                        }
                                    }

                                    cryptoStream.WriteByte( ( Byte ) data );
                                }
                            }
                        }
                    }
                }

                return output.Exists();
            }
            catch ( AggregateException exceptionss ) {
                exceptions.AddRange( collection: exceptionss.InnerExceptions );

                return false;
            }
            catch ( Exception exception ) {
                exceptions.Add( item: exception );

                return false;
            }
        }

        /// <summary>
        ///     Decrypts the string <paramref name="value" />.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="iv">Optional input vector.</param>
        /// <param name="key">Optional key.</param>
        [CanBeNull]
        public static String Decrypt( [CanBeNull] this String value, [CanBeNull] String iv = null, [CanBeNull] String key = null ) {
            if ( String.IsNullOrEmpty( value: value ) ) {
                return default;
            }

            try {
                var _ivByte = Encoding.UTF8.GetBytes( iv?.Substring( 0, 8 ) ?? _iv.Substring( 0, 8 ) );
                var _keybyte = Encoding.UTF8.GetBytes( key?.Substring( 0, 8 ) ?? _key.Substring( 0, 8 ) );
                var inputbyteArray = Convert.FromBase64String( value.Replace( " ", "+" ) );

                using ( var des = new DESCryptoServiceProvider() ) {
                    using ( var ms = new MemoryStream() ) {
                        using ( var cs = new CryptoStream( ms, des.CreateDecryptor( _keybyte, _ivByte ), CryptoStreamMode.Write ) ) {
                            cs.Write( inputbyteArray, 0, inputbyteArray.Length );
                            cs.FlushFinalBlock();
                        }

                        return Encoding.UTF8.GetString( ms.ToArray() );
                    }
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        private const String _iv = "Ez!an5hzr&W6RTU$Zcmd3ru7dc#zTQdE3HXN6w9^rKhn$7hkjfQzyX^qB^&9FG4YQ&&CrVY!^j!T$BfrwC9aXWzc799w%pa2DQr";

        private const String _key = "S#KPxgy3a3ccUHzXf3tp2s2yQNP#t@s!X3GECese5sNhjt5h$hJAfmjg#UeQRb%tuUbrRJj*M&&tsRvkcDW6bhWfaTDJP*pZhbQ";

        [CanBeNull]
        public static String Encrypt( [CanBeNull] this String value, [CanBeNull] String iv = null, [CanBeNull] String key = null ) {
            if ( String.IsNullOrEmpty( value: value ) ) {
                return default;
            }

            try {
                var _ivByte = Encoding.UTF8.GetBytes( iv?.Substring( 0, 8 ) ?? _iv.Substring( 0, 8 ) );
                var _keybyte = Encoding.UTF8.GetBytes( key?.Substring( 0, 8 ) ?? _key.Substring( 0, 8 ) );
                var inputbyteArray = Encoding.UTF8.GetBytes( value );

                using ( var des = new DESCryptoServiceProvider() ) {
                    using ( var ms = new MemoryStream() ) {
                        using ( var cs = new CryptoStream( ms, des.CreateEncryptor( _keybyte, _ivByte ), CryptoStreamMode.Write ) ) {
                            cs.Write( inputbyteArray, 0, inputbyteArray.Length );
                            cs.FlushFinalBlock();
                        }

                        return Convert.ToBase64String( ms.ToArray() );
                    }
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }
    }

}