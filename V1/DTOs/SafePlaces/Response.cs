using Remotion.Linq.Clauses;

namespace CoviIDApiCore.V1.DTOs.SafePlaces
{
    public class Response<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
    }
}
    