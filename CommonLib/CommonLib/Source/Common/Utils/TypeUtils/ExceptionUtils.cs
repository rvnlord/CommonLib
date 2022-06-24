using System;
using System.Threading.Tasks;

namespace CommonLib.Source.Common.Utils.TypeUtils
{
    public static class ExceptionUtils
    {
        public static async Task<CaughtExceptionAndData<TException, TReturned>> CatchAsync<TException, TReturned>(Func<Task<TReturned>> actionAsync) where TException : Exception
        {
            try
            {
                return new CaughtExceptionAndData<TException, TReturned> { Data = await actionAsync() };
            }
            catch (TException ex)
            {
                return new CaughtExceptionAndData<TException, TReturned> { Error = ex };
            }
        }

        public static async Task<CaughtException<TException>> CatchAsync<TException>(Func<Task> actionAsync) where TException : Exception
        {
            try
            {
                await actionAsync();
                return new CaughtException<TException>();
            }
            catch (TException ex)
            {
                return new CaughtException<TException> { Error = ex };
            }
        }

        public class CaughtExceptionAndData<TException, TReturned> where TException : Exception
        {
            public TReturned Data { get; set; }
            public TException Error { get; set; }

            public bool IsSuccess => Error == null;
        }

        public class CaughtException<TException> where TException : Exception
        {
            public TException Error { get; set; }

            public bool IsSuccess => Error == null;
        }
    }
}
