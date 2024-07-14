using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.DataProtection;
using System.Text;

var services = new ServiceCollection();
services.AddDataProtection();
services.AddSingleton<EncryptionService>();

var serviceProvider = services.BuildServiceProvider();
var encryptionService = serviceProvider.GetRequiredService<EncryptionService>();

string encryptedAuth = encryptionService.Encrypt("");
Console.WriteLine(encryptedAuth);


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