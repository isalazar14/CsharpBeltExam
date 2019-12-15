using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using BeltExam.Models;

namespace BeltExam.Models
{
  public class ThingGuest : dbModel
  {
    [Key]
    public int Aid {get; set;}

    public int GuestId {get; set;}

    public User Guest {get; set;}
    
    public int ThingId {get; set;}
    
    public Thing Thing {get; set;}
  }
}