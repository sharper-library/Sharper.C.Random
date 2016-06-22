using System;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Sharper.C.Control;

namespace Sharper.C.Data
{
    public static class CryptoRandom
    {
        public const string Base64Alphabet =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789+/";

        private static readonly int OutOfRange =
            byte.MaxValue + 1 - (byte.MaxValue + 1) % Base64Alphabet.Length;

        private static readonly RandomNumberGenerator rng =
            RandomNumberGenerator.Create();

        public static IO<RO, byte> RandomByte
        =>  IO<RO>.Defer
              ( () =>
                {   var bytes = new byte[1];
                    rng.GetBytes(bytes);
                    return bytes[0];
                }
              );

        public static IO<RO, string> RandomString64(int length)
        =>  IO<RO>.Defer
              ( () =>
                {   var n = 0;
                    var sb = new StringBuilder(length);
                    while (n < length)
                    {   var bytes = new byte[length - (length / 2)];
                        rng.GetBytes(bytes);
                        var str =
                            string.Concat
                              ( bytes
                                .Where(x => x < OutOfRange)
                                .Select(x => Base64Alphabet[x % Base64Alphabet.Length])
                              );
                        var left = length - n;
                        var count = Math.Min(left, str.Length);
                        sb.Append(str.Substring(0, count));
                        n = n + count;
                    }
                    return sb.ToString();
                }
              );

        public static IO<RO, ImmutableArray<byte>> RandomBytes(int count)
        =>  IO<RO>.Defer
              ( () =>
                {   var bytes = new byte[count];
                    rng.GetBytes(bytes);
                    return bytes.ToImmutableArray();
                }
              );
    }
}
