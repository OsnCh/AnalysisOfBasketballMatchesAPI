using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ViewModels.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UserRoles
    {
        User = 0,
        Admin = 1
    }
}
