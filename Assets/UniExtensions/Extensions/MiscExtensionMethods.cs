using UnityEngine;
using System.Collections;
using System.Text;

namespace UniExtensions
{
    public static class MiscExtensionMethods
    {
        /// <summary>
        /// Dumps an object to JSON.
        /// </summary>
        /// <returns>The JSON.</returns>
        /// <param name="obj">Object.</param>
        public static string DumpJSON(this Hashtable obj) {
            var json = Serialization.JsonSerializer.Encode(obj);
            return Serialization.JsonPrettyPrint.Format(json);
        }

        /// <summary>
        /// returns MD5 hex digest of this array.
        /// </summary>
        /// <returns>
        /// The md5.
        /// </returns>
        /// <param name='bytes'>
        /// Bytes.
        /// </param>
        public static string MD5 (this byte[] bytes)
        {
            return new UniExtensions.Crypto.MD5().HexDigest(bytes);
        }

        /// <summary>
        /// MD5 Hex Digest of this string.
        /// </summary>
        /// <returns>
        /// The md5.
        /// </returns>
        /// <param name='text'>
        /// Text.
        /// </param>
        public static string MD5 (this string text)
        {
            var bytes = System.Text.UTF8Encoding.UTF8.GetBytes (text);
            return MD5 (bytes);
        }



        /// <summary>
        /// Encrypts and Decrypts a byte array.
        /// </summary>
        /// <param name='bytes'>
        /// Bytes.
        /// </param>
        /// <param name='skey'>
        /// Skey.
        /// </param>
        public static void RC4 (this byte[] bytes, string skey)
        {
            var key = System.Text.UTF8Encoding.UTF8.GetBytes (skey);
            byte[] s = new byte[256];
            byte[] k = new byte[256];
            byte temp;
            int i, j;

            for (i = 0; i < 256; i++) {
                s [i] = (byte)i;
                k [i] = key [i % key.GetLength (0)];
            }

            j = 0;
            for (i = 0; i < 256; i++) {
                j = (j + s [i] + k [i]) % 256;
                temp = s [i];
                s [i] = s [j];
                s [j] = temp;
            }

            i = j = 0;
            for (int x = 0; x < bytes.GetLength (0); x++) {
                i = (i + 1) % 256;
                j = (j + s [i]) % 256;
                temp = s [i];
                s [i] = s [j];
                s [j] = temp;
                int t = (s [i] + s [j]) % 256;
                bytes [x] ^= s [t];
            }
        }


    }
}
