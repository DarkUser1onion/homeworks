using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.Entities;

namespace TaskFlowApi.Services;

public class IdempotencyService
{
    private readonly AppDbContext _db;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public IdempotencyService(AppDbContext db) => _db = db;

    /// <summary>Хэш тела запроса - SHA256 от сериализованного объекта.</summary>
    public string ComputeHash(object body)
    {
        var json = JsonSerializer.Serialize(body, JsonOpts);
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(json));
        return Convert.ToHexString(bytes);
    }

    public Task<IdempotencyRecord?> FindAsync(string key)
        => _db.IdempotencyRecords.FirstOrDefaultAsync(x => x.Key == key);

    public async Task SaveAsync(string key, string requestHash, string responseBody, int statusCode)
    {
        _db.IdempotencyRecords.Add(new IdempotencyRecord
        {
            Key = key,
            RequestBodyHash = requestHash,
            ResponseBody = responseBody,
            StatusCode = statusCode
        });
        await _db.SaveChangesAsync();
    }

    public string Serialize(object value) => JsonSerializer.Serialize(value, JsonOpts);
}