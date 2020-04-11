using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using UsersTasks.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;

namespace UsersTasks.Services
{
    public class AuthService
    {
        private readonly MyConfiguration configuration;
        public const int SaltByteSize = 24;
        public const int HashByteSize = 20; // to match the size of the PBKDF2-HMAC-SHA-1 hash 
        public const int Pbkdf2Iterations = 1000;
        public const int IterationIndex = 0;
        public const int SaltIndex = 1;
        public const int Pbkdf2Index = 2;

        public AuthService(MyConfiguration configuration)
        {
            this.configuration = configuration;
        }

        //
        // Summary:
        //     Uses a security key to creat a token.
        //
        // Parameters:
        //   user:
        //     A User that contains data for the claims of the token.
        //
        // Returns:
        //     A string representing a bearer token
        //
        public string CreateToken(User user)
        {
            try
            {
                //security key
                string securityKey = Environment.GetEnvironmentVariable("tokenSecurityKey");

                //symmetric security key
                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

                //signing credentials
                var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

                //add claims
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Role, user.Role));
                claims.Add(new Claim("UserId", user.UserId.ToString()));


                //create token
                var token = new JwtSecurityToken(
                        issuer: configuration.token.issuer,
                        audience: configuration.token.audience,
                        expires: DateTime.Now.AddHours(configuration.token.expirationHours),
                        signingCredentials: signingCredentials
                        , claims: claims
                    );

                //return token
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        //
        // Summary:
        //     Creates a hashed password.
        //
        // Parameters:
        //   password:
        //     A string representing an unhashed password.
        //
        // Returns:
        //     A hashed password.
        //
        public string HashPassword(string password)
        {
            try
            {
                var cryptoProvider = new RNGCryptoServiceProvider();
                byte[] salt = new byte[SaltByteSize];
                cryptoProvider.GetBytes(salt);

                var hash = GetPbkdf2Bytes(password, salt, Pbkdf2Iterations, HashByteSize);
                return Pbkdf2Iterations + ":" +
                       Convert.ToBase64String(salt) + ":" +
                       Convert.ToBase64String(hash);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        //
        // Summary:
        //     Compares a password and a hashed string to return if they are the same password.
        //
        // Parameters:
        //   password:
        //     A string representing an unhashed password.
        //
        //   correctHash:
        //     A string representing a hashed password.
        //
        // Returns:
        //     true if the passwords are the same.
        //
        public bool ValidatePassword(string password, string correctHash)
        {
            try
            {
                char[] delimiter = { ':' };
                var split = correctHash.Split(delimiter);
                var iterations = Int32.Parse(split[IterationIndex]);
                var salt = Convert.FromBase64String(split[SaltIndex]);
                var hash = Convert.FromBase64String(split[Pbkdf2Index]);

                var testHash = GetPbkdf2Bytes(password, salt, iterations, hash.Length);
                return SlowEquals(hash, testHash);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        //
        // Summary:
        //     Slowly equals two byte arrays.
        //
        // Parameters:
        //   a:
        //     First byte array.
        //
        //   b:
        //     Second byte array.
        //
        // Returns:
        //     true if the two byte arrays are equal.
        //
        private bool SlowEquals(byte[] a, byte[] b)
        {
            try
            {
                var diff = (uint)a.Length ^ (uint)b.Length;
                for (int i = 0; i < a.Length && i < b.Length; i++)
                {
                    diff |= (uint)(a[i] ^ b[i]);
                }
                return diff == 0;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        private byte[] GetPbkdf2Bytes(string password, byte[] salt, int iterations, int outputBytes)
        {
            try
            {
                var pbkdf2 = new Rfc2898DeriveBytes(password, salt);
                pbkdf2.IterationCount = iterations;
                return pbkdf2.GetBytes(outputBytes);
            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }
}
