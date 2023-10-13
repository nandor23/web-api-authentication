﻿namespace Dtos.Requests;

public class SignUpRequestDto
{
    public required string UserName { get; set; }

    public required string Email { get; set; }

    public required string Password { get; set; }
}