using PI.Contract;
using PI.Data;
using PI.Data.Entity;
using PI.Data.Entity.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business
{
    public static class ContextExtension
    {
        /// <summary>
        /// Get user role by userId
        /// </summary>
        /// <param name="context">Db Context</param>
        /// <param name="userId">User Id</param>
        /// <returns></returns>
        public static string GetUserRoleById(this PIContext context,string userId)
        {
            string roleId = context.Users.Where(u => u.Id == userId).FirstOrDefault().Roles.FirstOrDefault().RoleId;
            string roleName = context.Roles.Where(r => r.Id == roleId).Select(r => r.Name).FirstOrDefault();
            return roleName;
        }


        /// <summary>
        /// Get comany details by userId
        /// </summary>
        /// <param name="context">Db Context</param>
        /// <param name="userId">User Id</param>
        /// <returns></returns>
        public static Company GetCompanyByUserId(this PIContext context, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new Company { Name = "" };

            var currentuser = context.Users.SingleOrDefault(u => u.Id == userId);

            return context.Companies.SingleOrDefault(n => n.TenantId == currentuser.TenantId);         
        }


        /// <summary>
        /// Get Tenant by UserId
        /// </summary>
        /// <param name="context">Db Context</param>
        /// <param name="userid">User Id</param>
        /// <returns></returns>
        public static long GetTenantIdByUserId(this PIContext context, string userid)
        {
            ApplicationUser currentuser = null;
     
            currentuser = context.Users.SingleOrDefault(u => u.Id == userid);
    
            if (currentuser == null)
            {
                return 0;
            }
            return currentuser.TenantId;
        }


        /// <summary>
        /// Get language code by languageId
        /// </summary>
        /// <param name="context">Db Context</param>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public static string GetLanguageCodeById(this PIContext context, short id)
        {
            return context.Languages.SingleOrDefault(l => l.Id == id).LanguageCode;
        }

        /// <summary>
        /// Get local time by User
        /// </summary>
        /// <param name="context"></param>
        /// <param name="loggedUserId"></param>
        /// <param name="utcDatetime"></param>
        /// <returns></returns>
        public static DateTime GetLocalTimeByUser(this PIContext context, string loggedUserId, DateTime utcDatetime)
        {
            if(string.IsNullOrWhiteSpace(loggedUserId))
                return utcDatetime;

            var account = context.AccountSettings.Where(ac => ac.Customer.UserId == loggedUserId).FirstOrDefault();

            if (account == null)
                return utcDatetime;

            var timeZone = context.TimeZones.Where(t => t.Id == account.DefaultTimeZoneId).First();

            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone.TimeZoneId);
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcDatetime, cstZone);

            return localTime;
        }
    }
}
