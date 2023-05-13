using System;
using System.Linq;
using CommonLib.Source.Common.Extensions.Collections;

namespace CommonLib.Source.ViewModels.Account;

public class AuthenticationSchemeVM
{
    public AuthenticationSchemeVM(string name, string displayName, Type handlerType)
    {
        ArgumentNullException.ThrowIfNull(name);
        //ArgumentNullException.ThrowIfNull(handlerType); // it should be possible to assign or deserialize handlerType to null value when the type is not present in client project
        //if (!handlerType.GetInterfaces().Select(i => i.Name).ContainsIgnoreCase("IAuthenticationHandler"))
        //    throw new ArgumentException("handlerType must implement IAuthenticationHandler.");

        Name = name;
        HandlerType = handlerType;
        DisplayName = displayName;
    }
    
    public string Name { get; }
    public string DisplayName { get; } 
    public Type HandlerType { get; }
}