﻿// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Crypto.cs" belongs to Rick@AIBrain.org and
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
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/Crypto.cs" was last formatted by Protiguous on 2018/05/24 at 7:32 PM.

namespace Librainian.Security {

    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public static class Crypto {

        private static readonly Byte[] Salt = Encoding.ASCII.GetBytes( s: "evatuewot8evtet8e8paaa40aqtab60w489uvmw" );

        /// <summary>
        ///     Decrypt the given string. Assumes the string was encrypted using EncryptStringAES(),
        ///     using an identical sharedSecret.
        /// </summary>
        /// <param name="cipherText">The text to decrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for decryption.</param>
        public static String DecryptStringAES( this String cipherText, String sharedSecret ) {
            if ( String.IsNullOrEmpty( cipherText ) ) { throw new ArgumentNullException( nameof( cipherText ) ); }

            if ( String.IsNullOrEmpty( sharedSecret ) ) { throw new ArgumentNullException( nameof( sharedSecret ) ); }

            // Declare the RijndaelManaged object used to decrypt the data.
            RijndaelManaged aesAlg = null;

            // Declare the string used to hold the decrypted text.
            String plaintext;

            try {

                // generate the key from the shared secret and the salt
                var key = new Rfc2898DeriveBytes( password: sharedSecret, salt: Salt );

                // Create the streams used for decryption.
                var bytes = Convert.FromBase64String( s: cipherText );
                var msDecrypt = new MemoryStream( buffer: bytes );

                // Create a RijndaelManaged object with the specified key and IV.
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes( cb: aesAlg.KeySize / 8 );

                // Get the initialization vector from the encrypted stream
                aesAlg.IV = msDecrypt.ReadByteArray();

                // Create a decrytor to perform the stream transform.
                var decryptor = aesAlg.CreateDecryptor( rgbKey: aesAlg.Key, rgbIV: aesAlg.IV );

                using ( var srDecrypt = new StreamReader( stream: new CryptoStream( stream: msDecrypt, transform: decryptor, mode: CryptoStreamMode.Read ) ) ) {

                    // Read the decrypted bytes from the decrypting stream and place them in
                    // a string.
                    plaintext = srDecrypt.ReadToEnd();
                }
            }
            finally {

                // Clear the RijndaelManaged object.
                aesAlg?.Clear();
            }

            return plaintext;
        }

        /// <summary>
        ///     Encrypt the given string using AES. The string can be decrypted using
        ///     DecryptStringAES(). The sharedSecret parameters must match.
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for encryption.</param>
        public static String EncryptStringAES( this String plainText, String sharedSecret ) {
            if ( String.IsNullOrEmpty( plainText ) ) { throw new ArgumentNullException( nameof( plainText ) ); }

            if ( String.IsNullOrEmpty( sharedSecret ) ) { throw new ArgumentNullException( nameof( sharedSecret ) ); }

            String outStr; // Encrypted string to return
            RijndaelManaged aesAlg = null; // RijndaelManaged object used to encrypt the data.

            try {

                // generate the key from the shared secret and the salt
                var key = new Rfc2898DeriveBytes( password: sharedSecret, salt: Salt );

                // Create a RijndaelManaged object
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes( cb: aesAlg.KeySize / 8 );

                // Create a decryptor to perform the stream transform.
                var encryptor = aesAlg.CreateEncryptor( rgbKey: aesAlg.Key, rgbIV: aesAlg.IV );

                // Create the streams used for encryption.
                var msEncrypt = new MemoryStream();

                // prepend the IV
                msEncrypt.Write( buffer: BitConverter.GetBytes( aesAlg.IV.Length ), offset: 0, count: sizeof( Int32 ) );
                msEncrypt.Write( buffer: aesAlg.IV, offset: 0, count: aesAlg.IV.Length );

                var csEncrypt = new CryptoStream( stream: msEncrypt, transform: encryptor, mode: CryptoStreamMode.Write );

                using ( var swEncrypt = new StreamWriter( stream: csEncrypt ) ) {

                    //Write all data to the stream.
                    swEncrypt.Write( plainText );
                }

                outStr = Convert.ToBase64String( inArray: msEncrypt.ToArray() );
            }
            finally {

                // Clear the RijndaelManaged object.
                aesAlg?.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return outStr;
        }

        public static Byte[] ReadByteArray( this Stream s ) {
            var rawLength = new Byte[sizeof( Int32 )];

            if ( s.Read( buffer: rawLength, offset: 0, count: rawLength.Length ) != rawLength.Length ) { throw new SystemException( "Stream did not contain properly formatted byte array" ); }

            var buffer = new Byte[BitConverter.ToInt32( rawLength, startIndex: 0 )];

            if ( s.Read( buffer: buffer, offset: 0, count: buffer.Length ) != buffer.Length ) { throw new SystemException( "Did not read byte array properly" ); }

            return buffer;
        }
    }
}