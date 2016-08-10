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
    public class CommonLogic
    {
        private PIContext context;

        public CommonLogic(PIContext _context = null)
        {
            context = _context ?? PIContext.Get();
        }

        /// <summary>
        /// Get user role by userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserRoleById(string userId)
        {
            string roleId = context.Users.Where(u => u.Id == userId).FirstOrDefault().Roles.FirstOrDefault().RoleId;
            string roleName = context.Roles.Where(r => r.Id == roleId).Select(r => r.Name).FirstOrDefault();
            return roleName;
        }


        /// <summary>
        /// Get comany details by userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Company GetCompanyByUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new Company { Name = "" };

            var currentuser = context.Users.SingleOrDefault(u => u.Id == userId);

            return context.Companies.SingleOrDefault(n => n.TenantId == currentuser.TenantId);         
        }


        /// <summary>
        /// Get Tenant by UserId
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public long GetTenantIdByUserId(string userid)
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
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetLanguageCodeById(short id)
        {
            return context.Languages.SingleOrDefault(l => l.Id == id).LanguageCode;
        }

    }
}
