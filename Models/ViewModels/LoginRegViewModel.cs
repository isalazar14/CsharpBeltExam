using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeltExam.Models
{
  public class LoginRegViewModel
  {
    public User User {get; set;}
    public LoginUser LoginUser {get; set;}
  }
}

