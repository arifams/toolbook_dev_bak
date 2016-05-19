﻿using PI.Data;
using PI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Business
{
   public class CommonLogic
    {

       /// <summary>
       /// Get user role by userId
       /// </summary>
       /// <param name="userId"></param>
       /// <returns></returns>
       public string GetUserRoleById(string userId)
       {
           using (PIContext context = new PIContext())
           {
               string roleId = context.Users.Where(u => u.Id == userId).FirstOrDefault().Roles.FirstOrDefault().RoleId;
               string roleName = context.Roles.Where(r => r.Id == roleId).Select(r => r.Name).FirstOrDefault();
               return roleName;
           }
       }


       /// <summary>
       /// 
       /// </summary>
       /// <param name="userId"></param>
       /// <returns></returns>
       public Company GetCompanyByUserId(string userId)
       {           
           using (PIContext context = PIContext.Get())
           {
               var currentuser = context.Users.SingleOrDefault(u => u.Id == userId);

               return context.Companies.SingleOrDefault(n => n.TenantId == currentuser.TenantId);
           }
       }
    }
}