using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ButtsBlazor.Api.Model;

public static class Extensions
{
    public static string? TryToJson(this object obj,JsonSerializerOptions? options = null) => obj.TryToJson(out var json,options) ? json : null;
    public static bool TryToJson(this object obj, [NotNullWhen(true)]out string? json,JsonSerializerOptions? options = null)
    {
        try
        {
            json= JsonSerializer.Serialize(obj, options);
            return true;
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex);
        }
        json = null;
        return false;
    }

    public static string ToJson(this object obj, JsonSerializerOptions? options = null) => JsonSerializer.Serialize(obj,options);
}