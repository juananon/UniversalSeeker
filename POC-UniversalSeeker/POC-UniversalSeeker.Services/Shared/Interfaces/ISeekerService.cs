
using POC_UniversalSeeker.Entities.Shared;
using System.Collections.Generic;

namespace POC_UniversalSeeker.Services.Shared.Interfaces
{
    public interface ISeekerService
    {
        IEnumerable<Filter> GetFilters<T>(string navigationProperties) where T : class;
    }
}
