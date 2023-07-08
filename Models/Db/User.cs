using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeltExam.Models
{
  public class User : dbModel
  {
    [Key]
    public int Uid {get; set;}
    
    [Required]
    [MinLength(2)]
    [Display(Name="First Name")]
    public string Fname {get; set;}
    
    [Required]
    [MinLength(2)]
    [Display(Name="Last Name")]
    public string Lname {get; set;}

    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email {get; set;}

    [NotMapped]
    [Required]
    [DataType(DataType.EmailAddress)]
    [Display(Name="Confirm Email")]
    public string EmailConfirm {get; set;}
    
    [Required]
    [NotMapped]
    [MinLength(8)]
    // [RegularExpression(@"^(?=.{8})(?=.*[a-zA-Z0-9_])(?=.*[^a-zA-Z0-9_]).*$")]
    [DataType(DataType.Password)]
    public string Password {get; set;}

    [NotMapped]
    [Required]
    [DataType(DataType.Password)]
    [Compare("Password")]
    [Display(Name="Confirm Password")]
    public string PwConfirm {get; set;}

    // [Required]
    public string PwHash {get; set;}
    
    [Required]
    [DataType(DataType.Date)]
    [Display(Name="Date of Birth")]
    public DateTime Dob {get; set;}

    public List<Thing> ThingsCreated {get; set;}
    
    public List<ThingGuest> ThingsAttending {get; set;}

  }
}