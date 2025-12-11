using System;
using System.Collections.Generic;

namespace ClubId.Models;

public partial class Player
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Dni { get; set; } = null!;

    public DateTime BirthDate { get; set; }

    public string Team { get; set; } = null!;

    public int Number { get; set; }

    public bool Active { get; set; }

    public string? PhotoPath { get; set; }

    public int Id { get; set; }
}
