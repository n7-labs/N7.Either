﻿namespace N7.Either;

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

using Metadata = Dictionary<String, Object>;

public class Error
{
    public String        Id          { get; private set; }
    public String?       Description { get; private set; }
    public Metadata?     Metadata    { get; private set; }
    public Error?        Cause       { get; private set; }

    private static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    private Error ()
    {
        Id          = String.Empty;
        Description = null;
        Metadata    = null;
        Cause       = null;
    }

    [JsonConstructor]
    public Error (String id, String? description = default, Metadata? metadata = default, Error? cause = default)
    {        
        Id = String.IsNullOrWhiteSpace(id) 
            ? throw new ArgumentNullException(nameof(id)) 
            : id;

        Description = description;
        Metadata    = metadata;
        Cause       = cause;
    }

    public T? GetMetadataOrDefault<T>(String key, T? defaultValue = default)
    {
        var hasMetadata = Metadata is not null;
        if (hasMetadata is false)
        {
            return defaultValue;
        }

        var hasValue = Metadata!.TryGetValue(key, out var value);
        if (hasValue is false)
        {
            return defaultValue;
        }

        return value switch
        {
            T                => (T) value,
            JsonElement json => json.Deserialize<T>(jsonSerializerOptions) ?? defaultValue,
            _                => defaultValue
        };
    }
}