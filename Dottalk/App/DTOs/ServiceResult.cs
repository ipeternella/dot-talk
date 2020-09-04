using System.Collections.Generic;

namespace Dottalk.App.DTOs
{
    public class VoidServiceResult
    {
        public bool Success;
        public IEnumerable<ServiceError> Errors;
    }

    public class ServiceResult<TResult>
    {
        public bool Success;
        public IEnumerable<ServiceError> Errors;
        public TResult Result;
    }

    public class ServiceError
    {
        public int Code;
        public string Msg;
    }
}