using System.Collections.Generic;

namespace Dottalk.App.DTOs
{
    public class VoidServiceResultDTO
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
        public int Code;
        public string Msg;
    }
}