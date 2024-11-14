﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Domain.dbo;

public class User : IdentityUser<int>
{
    public bool IsPremium { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public virtual List<CompletedTests> CompletedTests { get; set; }
    public virtual List<FavouriteTests> FavouriteTests { get; set; }
}
