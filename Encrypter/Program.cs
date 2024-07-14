using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.DataProtection;
using System.Text;

var services = new ServiceCollection();
services.AddDataProtection();
services.AddSingleton<EncryptionService>();

var serviceProvider = services.BuildServiceProvider();
var encryptionService = serviceProvider.GetRequiredService<EncryptionService>();

string encryptedAuth = encryptionService.Encrypt("eyJhbGciOiJSUzI1NiIsImtpZCI6IlNIQTI1NjpzS3dsMnlsV0VtMjVmcXhwTU40cWY4MXE2OWFFdWFyMnpLMUdhVGxjdWNZIiwidHlwIjoiSldUIn0.eyJzdWIiOiJ1c2VyIiwiZXhwIjoxNzIxMDQzODgwLjQ1ODMxNCwiaWF0IjoxNzIwOTU3NDgwLjQ1ODMxNCwianRpIjoiMWRNcGVZVWNacm56bG54OVV4R1RvQ01GaGxtS3J3IiwiY2lkIjoiSnRORWQ1RWktLXJvVUFnN1d1ck9TZyIsImxpZCI6InQyX2IyOHl2MDlwcyIsImFpZCI6InQyX2IyOHl2MDlwcyIsImxjYSI6MTY4MzgwNDA5NTAwMCwic2NwIjoiZUp5S1Z0SlNpZ1VFQUFEX193TnpBU2MiLCJmbG8iOjl9.jvJQJF1XpXdwbavcjxpshGZ5mWH9GWbnGyv94gmUBDQdv9nXT4Rp9zU8jX9RdnTtDpl-s-vdLiHN-Dcniv05tsBI6i-dvrLAu-jTgeIlx8aw30C1rTX9i63RhV7Xg9Z22noU9UFNER3tnpQfJJkaYfxSaClqNEf_LaxWqHuxeGS45DpZ3rzqwyRsNz-6LXzIE1YNeF4_Hk4lDWNxSwL-b7hII5HUaOLbEA3GsBv7PhMSvWPKy_GAlQBziJCkfeF5uv5y2hH9Ifc3RYNvQ2BVdDxqR62jkqjCWVRJsegPuqyIPhQbGiekWRX7MqxUawZXj8tT3BqRI1QAViF57xPLuw");
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