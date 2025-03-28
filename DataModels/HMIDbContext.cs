using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMI_ApplicationConfigClient.DataModels
{
    public class HMIDbContext : DbContext
    {
        public HMIDbContext(DbContextOptions<HMIDbContext> dbContext):base(dbContext)
        {

        }
        public virtual DbSet<HMIVarInfo> HMIvarInfoDbSet { get; set; }

        public virtual DbSet<IOVarinfo> IOVarInfoDbSet { get; set; }

        public virtual DbSet<Deviceinfo> DeviceInfoDbSet { get; set; }

        public virtual DbSet<Controlinfo> ControlinfoDbSet { get; set; }
    }
}
