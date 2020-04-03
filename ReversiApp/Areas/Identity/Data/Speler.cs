﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ReversiApp.Models;

namespace ReversiApp.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the Speler class
    public class Speler : IdentityUser
    {
        public string Token { get; set; }
        public Kleur HuidigeKleur { get; set; }
    }
}
