using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xtea_algorithm
{
    public class Xtea
    {


        private static uint[] Longer(byte[] s)
        {
            var long1 = new uint[(int)Math.Ceiling(((decimal)s.Length / 4))];

            for (int i = 0; i < long1.Length; i++)
            {
                long1[i] = ((s[i * 4])) +
                       ((i * 4 + 1) >= s.Length ? (uint)0 << 8 : ((uint)s[i * 4 + 1] << 8)) +
                       ((i * 4 + 2) >= s.Length ? (uint)0 << 16 : ((uint)s[i * 4 + 2] << 16)) +
                       ((i * 4 + 3) >= s.Length ? (uint)0 << 24 : ((uint)s[i * 4 + 3] << 24));
            }

            return long1;
        }

        private static byte[] ToBytes(uint[] l)
        {
            byte[] b = new byte[l.Length * 4];

            for (int i = 0; i < l.Length; i++)
            {
                b[(i * 4)] = (byte)(l[i] & 0xFF);
                b[(i * 4) + 1] = (byte)(l[i] >> (8 & 0xFF));
                b[(i * 4) + 2] = (byte)(l[i] >> (16 & 0xFF));
                b[(i * 4) + 3] = (byte)(l[i] >> (24 & 0xFF));
            }
            return b;
        }

        public string Encrypt(string text, string key)
        {
            if (text.Length == 0)
                return "";

            var y = Longer((new UTF8Encoding()).GetBytes(text));
            if (y.Length == 1)
                y[0] = 0;

            var k = Longer((new UTF8Encoding()).GetBytes(key.Substring(0,16)));

            uint a = (uint)y.Length, b = y[a - 1], c = y[0], delta = 0x9e3779b9, d, f = (uint)(6 + (52 / a)), sum = 0, i = 0;

            while (f-- > 0)
            {
                sum += delta;
                d = sum >> 2 & 3;

               for(i= 0; i < (a - 1); i++)
                {
                    c = y[(i + 1)];
                    b=y[i] +=(b >> 5 ^ c << 2) + (c >> 3 ^ b << 4) ^ (sum ^ c) + (k[i & 3 ^ d] ^ b);
                }

                c = y[0];
                b= y[a - 1] += (b >> 5 ^ c << 2) + (c >> 3 ^ b << 4) ^ (sum ^ c) + (k[i & 3 ^ d] ^ b);

                
            }

            return Convert.ToBase64String(ToBytes(y));
        }

        public string Decrypt(string chipertext, string key)
        {
            if (chipertext.Length == 0)
                return "";

            var y = Longer(Convert.FromBase64String(chipertext));
            var k = Longer((new UTF8Encoding()).GetBytes(key.Substring(0, 16)));

            uint a = (uint)y.Length, b = y[a - 1];
            uint delta = 0x9e3779b9, d, f = (uint)(6 + (52 / a)), sum = f * delta, i = 0;

            while (sum != 0)
            {
                uint c = y[0];
                d = sum >> 2 & 3;

                for (i = (a - 1); i > 0; i--)
                {
                    b = y[i - 1];
                    c = y[i] -= (b >> 5 ^ c << 2) + (c >> 3 ^ b << 4) ^ (sum ^ c) + (k[i & 3 ^ d] ^ b);
                }

                b = y[a - 1];
                c = y[0] -= (b >> 5 ^ c << 2) + (c >> 3 ^ b << 4) ^ (sum ^ c) + (k[i & 3 ^ d] ^ b);

                sum -= delta;
            }

            var plaintext = (new UTF8Encoding()).GetString(ToBytes(y));

            return plaintext;
        }

        
    }
}
