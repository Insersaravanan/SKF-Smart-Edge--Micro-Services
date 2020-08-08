using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace EMaintanance.Services
{
    public class Utility
    {
        static IConfiguration _iconfiguration;
        public Utility(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
        }

        public DbConnection MasterCon()
        {
            return new SqlConnection(_iconfiguration.GetValue<string>("ConnectionStrings:IFin.Master"));
        }
    }
}
