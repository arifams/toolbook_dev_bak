﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.User
{
    public class UserDto
    {
        public string Id { get; set; }

        public List<Role.RolesDto> Roles { get; set; }
    }
}
