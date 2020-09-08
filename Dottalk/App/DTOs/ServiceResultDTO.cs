using System.Collections.Generic;

namespace Dottalk.App.DTOs
{
    public class ServiceResultVoidDTO
    {
        public bool Success;
        public IEnumerable<ServiceErrorDTO> Errors;
    }

    public class ServiceResultDTO<TResult>
    {
        public bool Success;
        public IEnumerable<ServiceErrorDTO> Errors;
        public TResult Result;
    }

    public class ServiceErrorDTO
    {
        public string Msg;
    }
}