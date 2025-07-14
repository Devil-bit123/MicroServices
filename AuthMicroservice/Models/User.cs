using System;
using System.Collections.Generic;

namespace AuthMicroservice.Models;

public partial class User
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;
    public string? Token { get; set; }
}
