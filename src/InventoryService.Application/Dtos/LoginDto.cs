﻿
namespace InventoryService.Application.Dtos;

public class LoginDto
{
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
}
