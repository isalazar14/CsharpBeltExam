using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeltExam.Models
{
  public class DashboardView
  {
    public int CurrentUid {get; set;}
    
    public List<Thing> Things {get; set;}
    public int thingId {get; set;}

    public ThingGuest TG {get; set;}

  }
}