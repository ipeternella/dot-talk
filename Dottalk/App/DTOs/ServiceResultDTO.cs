using System.Collections.Generic;

namespace Dottalk.App.DTOs
{
    public class ServiceResultVoidDTO
    {
        public bool Success;
        public IEnumerable<ServiceErrorDTO> Errors = null!;
    }

    public class ServiceResultDTO<TResult>
    {
        public bool Success;
        public IEnumerable<ServiceErrorDTO> Errors = null!;
        public TResult Result = default!;
    }

    public class ServiceErrorDTO
    {
        public string Message = null!;
    }
}