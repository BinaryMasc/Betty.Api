using System;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;
namespace Betty.Api.Infrastructure.Exceptions
{
    [Serializable]
    public class DataNotFoundException : ClientErrorException
    {
        public DataNotFoundException(string details = "") : base(StatusCodes.Status400BadRequest, "Bad Request",
            details)
        {

        }
    }
}
