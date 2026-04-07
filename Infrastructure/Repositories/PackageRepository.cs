using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class PackageRepository : BaseRepository<Package>, IPackageRepository
    {
        public PackageRepository(AppDbContext context) : base(context)
        {
        }
    }
}
