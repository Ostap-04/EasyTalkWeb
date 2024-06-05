﻿using Microsoft.AspNetCore.Identity;

namespace EasyTalkWeb.Models
{
    public class UserRole : IdentityRole<Guid>
    {
        public UserRole(string name) : base(name)
        {}
    }
}
