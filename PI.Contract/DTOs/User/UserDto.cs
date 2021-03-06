﻿using PI.Contract.DTOs.Division;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.User
{
    public class UserDto
    {
        public string Id { get; set; }

        public string Salutation { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string LastLoginTime { get; set; }

        public IList<Role.RolesDto> Roles { get; set; }

        public IList<DivisionDto> Divisions { get; set; }

        public bool IsActive { get; set; }

        public string LoggedInUserId { get; set; }

        public string Status { get; set; }

        public IList<long> AssignedDivisionIdList { get; set; }

        public string AssignedRoleName { get; set; }

        public string RoleName { get; set; }

        public string AssignedDivisionsForGrid { get; set; }

        public string TemplateLink { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string ReferenceType { get; set; }

        public long ReferenceId { get; set; }

        public string MobileVerificationCode { get; set; }

        public System.DateTime? MobileVerificationExpiry { get; set; }

        public bool isViaProfileSettings { get; set; }

        public string MobileNumber { get; set; }

        public bool IsActualUser { get; set; }

    }
}
