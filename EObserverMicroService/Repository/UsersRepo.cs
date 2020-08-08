using EMaintanance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMaintanance.Models;
using Dapper;
using EMaintanance.Services;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace EMaintanance.Repository
{
    public class UsersRepo
    {
        Utility util;
        public UsersRepo(IConfiguration iconfiguration)
        {
            util = new Utility(iconfiguration);
        }

        public Users GetUserByEmailId(string EmailId)
        {
            string sql = @"Select UserId from dbo.Users where EmailId = '" + EmailId + "';";

            using (var conn = util.MasterCon())
            {
                return conn.QueryFirstOrDefault<Users>(sql);
            }
        }

        public async Task<IEnumerable<dynamic>> GetUserByName(string UserName)
        {
            string sql = "Select UserName,UserId from dbo.Users where UserName like '%" + UserName + "%' order by UserName";

            using (var conn = util.MasterCon())
            {
                return await (conn.QueryAsync<dynamic>(sql));
            }
        }

        public async Task<IEnumerable<dynamic>> GetAssignToList(string Type, int UserId, int LanguageId, int ClientSiteId)
        {
            string sql = "dbo.EAppGetAssignToList";
            using (var conn = util.MasterCon())
            {
                try
                {
                    return await (conn.QueryAsync<dynamic>(sql, new { Type, UserId, LanguageId, ClientSiteId }, commandType: CommandType.StoredProcedure));
                }
                catch (Exception ex)
                {
                    throw new CustomException("Unable to Load Data, Please Contact Support!!!", "Error", true, ex);
                }
            }
        }

        public async Task<IEnumerable<dynamic>> UpdateLastSession(int UserId, int ClientSiteId, string SessionId)
        {
            string sql = "dbo.EAppSaveLastSession";
            using (var conn = util.MasterCon())
            {
                try
                {
                    return await (conn.QueryAsync<dynamic>(sql, new { UserId, ClientSiteId, SessionId }, commandType: CommandType.StoredProcedure));
                }
                catch (Exception ex)
                {
                    throw new CustomException("Unable to Load Data, Please Contact Support!!!", "Error", true, ex);
                }
            }
        }

        public async Task<dynamic> GetLastSession(int UserId)
        {
            string sql = "dbo.EAppGetLastSession";
            using (var conn = util.MasterCon())
            {
                try
                {
                    var result = await conn.QueryAsync<dynamic>(sql, new { UserId }, commandType: CommandType.StoredProcedure);
                    return result.FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw new CustomException("Unable to Load Data, Please Contact Support!!!", "Error", true, ex);
                }
            }
        }

        public async Task<dynamic> GetUser(int id)
        {
            string sql = "SELECT * from dbo.Users where UserId=" + id;
            using (var conn = util.MasterCon())
            {
                return await conn.QueryFirstOrDefaultAsync<dynamic>(sql);
            }
        }

        public async Task<IEnumerable<dynamic>> GetAllUsers()
        {
            string sql = "SELECT UserId as id,* from dbo.Users";
            using (var conn = util.MasterCon())
            {
                return await conn.QueryAsync<dynamic>(sql);
            }
        }

        public async Task<dynamic> GetOrganization(int id)
        {
           
            string sql = "SELECT dbo.GetNameTranslated(ClientSiteId,1,'ClientSiteName') as Organization,'Customer' as OrganizaitonType,'Enligh' as Langauage,'SKF' SolutionProvider ,logo, Descriptions as  Notes  from ClientSite where ClientSiteId=" + id;
            using (var conn = util.MasterCon())
            {
                return await conn.QueryFirstOrDefaultAsync<dynamic>(sql);
            }
        }

    }
}
