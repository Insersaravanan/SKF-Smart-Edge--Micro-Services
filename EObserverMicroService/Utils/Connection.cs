using EMaintanance;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace EMaintananceApi.Utils
{
    public class Connection
    {
        public DbConnection Master()
        {
            return new SqlConnection(Startup.MasterConnectionString);
        }

        public DbConnection User()
        {
            return new SqlConnection(Startup.UserConnectionString);
        }
    }
}
