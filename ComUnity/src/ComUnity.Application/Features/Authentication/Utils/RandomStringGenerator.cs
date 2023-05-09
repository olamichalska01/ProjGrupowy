using System.Security.Cryptography;
using System.Text;

namespace ComUnity.Application.Features.Authentication.Utils;

public static class RandomSecurityStringGenerator
{
    private static char[] allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@$?_-".ToCharArray();

    public static string Generate(int stringLength)
    {
        byte[] data = new byte[4 * stringLength];
        using (var crypto = RandomNumberGenerator.Create())
        {
            crypto.GetBytes(data);
        }
        StringBuilder result = new StringBuilder(stringLength);
        for (int i = 0; i < stringLength; i++)
        {
            var rnd = BitConverter.ToUInt32(data, i * 4);
            var idx = rnd % allowedChars.Length;

            result.Append(allowedChars[idx]);
        }

        return result.ToString();
    }

}
