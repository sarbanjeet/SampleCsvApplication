using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleApplication.Interfaces
{
    public interface IDataService
    {
        public Task<Dictionary<DateTime, decimal>> GetDataAsync(string fileName);
    }
}
