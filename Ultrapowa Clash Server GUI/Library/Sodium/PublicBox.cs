namespace CRS.Library.Sodium
{
    #region Usings

    using System;

    using CRS.Library.TweetNaCl;

    #endregion Usings

    public class PublicBox
    {
        private const int BEFORENMBYTES = 32;
        private const int BOXZEROBYTES = 16;
        private const int ZEROBYTES = 32;

        private readonly byte[] PrecomputedSharedKey = new byte[BEFORENMBYTES];

        public PublicBox(byte[] _Privatekey, byte[] _Publickey)
        {
            curve25519xsalsa20poly1305.crypto_box_beforenm(this.PrecomputedSharedKey, _Publickey, _Privatekey);
        }

        public byte[] Decrypt(byte[] _Cipher, byte[] _Nonce)
        {
            int cipherLength = _Cipher.Length;
            byte[] paddedbuffer = new byte[cipherLength + BOXZEROBYTES];
            Array.Copy(_Cipher, 0, paddedbuffer, BOXZEROBYTES, cipherLength);

            if (curve25519xsalsa20poly1305.crypto_box_open_afternm(paddedbuffer, paddedbuffer, paddedbuffer.Length, _Nonce, this.PrecomputedSharedKey) != 0)
            {
                throw new Exception("PublicBox Decryption failed");
            }

            byte[] output = new byte[paddedbuffer.Length - ZEROBYTES];
            Array.Copy(paddedbuffer, ZEROBYTES, output, 0, output.Length);
            return output;
        }

        public byte[] Encrypt(byte[] _Plain, byte[] _Nonce)
        {
            int plainLength = _Plain.Length;
            byte[] paddedbuffer = new byte[plainLength + ZEROBYTES];
            Array.Copy(_Plain, 0, paddedbuffer, ZEROBYTES, plainLength);

            if (curve25519xsalsa20poly1305.crypto_box_afternm(paddedbuffer, paddedbuffer, paddedbuffer.Length, _Nonce, this.PrecomputedSharedKey) != 0)
            {
                throw new Exception("PublicBox Encryption failed");
            }

            byte[] output = new byte[plainLength + BOXZEROBYTES];
            Array.Copy(paddedbuffer, ZEROBYTES - BOXZEROBYTES, output, 0, output.Length);
            return output;
        }
    }
}
