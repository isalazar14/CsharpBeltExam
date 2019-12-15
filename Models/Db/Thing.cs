using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeltExam.Models
{
  public class Thing : dbModel
  {
    [Key]
    public int Tid {get; set;}
    
    [Required]
    [MinLength(2)]
    [Display(Name="Title")]
    public string Name {get; set;}
    
    [Required]
    [DataType(DataType.Date)]
    public DateTime Date {get; set;}

    [Required]
    [DataType(DataType.Time)]
    public DateTime Time {get; set;}
    
    // [Required]
    // [DataType(DataType.Date)]
    // public DateTime EndDate {get; set;}

    [Required]
    [Range(1, double.PositiveInfinity)]
    public int Duration {get; set;}

    [Required]
    [MaxLength(10)]
    public string DurationUnits {get; set;}

    [NotMapped]
    public DateTime StartDateTime
    {
      get {
        return Date + Time.TimeOfDay;
      }
    }


    [NotMapped]
    public DateTime EndDateTime
    {
      get {
        if (DurationUnits == "Minutes")
        {
          return StartDateTime.AddMinutes(Duration);
        }
        if (DurationUnits == "Hours")
        {
          return StartDateTime.AddHours(Duration);
        }
        return StartDateTime.AddDays(Duration);
      }
    }


    [Required]
    [MinLength(2)]
    public string Description {get; set;}
    
    public int CreatorId {get; set;}

    public User Creator {get; set;}

    public List<ThingGuest> ThingGuests {get; set;}
  }
}