using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditRoulette.Services
{
    public class EncryptionService
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public EncryptionService(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }

        public string Encrypt(string input)
        {
            var protector = _dataProtectionProvider.CreateProtector("Auth.v1");
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] encryptedBytes = protector.Protect(inputBytes);
            return Convert.ToBase64String(encryptedBytes);
        }

        public string Decrypt(string encryptedInput)
        {
            var protector = _dataProtectionProvider.CreateProtector("Auth.v1");
            byte[] encryptedBytes = Convert.FromBase64String(encryptedInput);
            byte[] decryptedBytes = protector.Unprotect(encryptedBytes);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
