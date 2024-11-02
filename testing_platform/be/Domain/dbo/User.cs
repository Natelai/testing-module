using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Domain.dbo;

public class User : IdentityUser<int>
{
    public bool IsPremium { get; set; }
}
