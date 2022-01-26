using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;

namespace IceSync.Infrastructure.Extensions
{
    /// <summary>
    /// String extentions.
    /// </summary>
    public static class StringExtentions
    {
        /// <summary>
        /// Extention for replacing more than one space in string.
        /// </summary>
        /// <param name="str">String parameter.</param>
        /// <returns>Returns string with replaced extra spaces.</returns>
        public static string ClearSpaces(this string str)
        {
            return Regex.Replace(str, @"\s+", " ");
        }

        /// <summary>
        /// Extention for validating token expiration.
        /// </summary>
        /// <param name="token">Token string.</param>
        /// <returns>Returns boolean indicationg whether the token is expired or not.</returns>
        public static bool IsExpired(this string token)
        {
            var jwthandler = new JwtSecurityTokenHandler();
            var jwttoken = jwthandler.ReadToken(token);
            var expDate = jwttoken.ValidTo;

            return expDate <= DateTime.UtcNow;
        }
    }
}
