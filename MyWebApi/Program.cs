using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Encryption API", Version = "v1" });
});

var app = builder.Build();

// Enable Swagger in Development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello, welcome to Abdullahi's Encryption API!");

// 🔹 Caesar Cipher Encryption (Schiffer Chaufar)
string CaesarCipher(string text, int shift)
{
    return new string(text.Select(c => char.IsLetter(c) ? 
        (char)((c + shift - (char.IsUpper(c) ? 'A' : 'a')) % 26 + (char.IsUpper(c) ? 'A' : 'a')) : c).ToArray());
}

// 🔹 AES Encryption
string EncryptAES(string text, string key)
{
    using var aes = Aes.Create();
    aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32)); // Ensure key is 32 bytes
    aes.IV = new byte[16]; // Empty IV for simplicity

    using var encryptor = aes.CreateEncryptor();
    byte[] inputBytes = Encoding.UTF8.GetBytes(text);
    byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
    
    return Convert.ToBase64String(encryptedBytes);
}

// 🔹 AES Decryption
string DecryptAES(string encryptedText, string key)
{
    using var aes = Aes.Create();
    aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32));
    aes.IV = new byte[16];

    using var decryptor = aes.CreateDecryptor();
    byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
    
    return Encoding.UTF8.GetString(decryptedBytes);
}

// 🔹 Encrypt Endpoint
app.MapPost("/encrypt", ([FromBody] EncryptRequest request) =>
{
    string encryptedMessage = EncryptAES(request.Message, request.Key);
    string caesarCipher = CaesarCipher(encryptedMessage, request.Shift);
    return Results.Ok(new { Encrypted = caesarCipher });
});

// 🔹 Decrypt Endpoint
app.MapPost("/decrypt", ([FromBody] EncryptRequest request) =>
{
    string decryptedCaesar = CaesarCipher(request.Message, -request.Shift);
    string decryptedMessage = DecryptAES(decryptedCaesar, request.Key);
    return Results.Ok(new { Decrypted = decryptedMessage });
});

app.Run();

// 🔹 Request Model
public class EncryptRequest
{
    public string Message { get; set; }
    public string Key { get; set; }
    public int Shift { get; set; }
}
