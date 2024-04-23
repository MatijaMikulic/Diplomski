using DataAccess.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public ISampleRepository Samples { get; }

        public UnitOfWork(ISampleRepository sampleRepository) 
        {
            Samples = sampleRepository;
        }
    }
}
