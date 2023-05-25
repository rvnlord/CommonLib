using System;

namespace CommonLib.Source.Common.Utils.UtilClasses.Exceptions;

public class AuthenticationFailureException : Exception
{
    public AuthenticationFailureException(string message) : base(message)  { }
    public AuthenticationFailureException(string message, Exception innerException) : base(message, innerException) { }
}