﻿using System.ComponentModel.DataAnnotations;

namespace WebShop.Models;

public class ApplicationType
{
    [Key] 
    public int Id { get; set; }
    public string Name { get; set; }
}