// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Http;
// using Microsoft.EntityFrameworkCore;
// using BeltExam.Models;

// namespace BeltExam.Controllers
// {
//   public class PlannerController : Controller
//   {
//     private BeltExamContext db;

//     public PlannerController(BeltExamContext context)
//     {
//       db = context;
//     }

//     public bool UserLoggedIn()
//     {
//       int? Uid = HttpContext.Session.GetInt32("Uid");
//       if (Uid == null){
//         return false;
//       }
//       return true;
//     }

//     [HttpGet("new")]
//     public IActionResult NewThing()
//     {
//       if (UserLoggedIn()){
//         return View();
//       }
//       return RedirectToAction("Index", "User");
//     }

//     [HttpPost("new")]
//     public IActionResult NewThing(Thing NewThing)
//     {
//       if (!ModelState.IsValid){
//         return View(NewThing);
//       }
//       NewThing.CreatorId = (int)HttpContext.Session.GetInt32("Uid");
//       db.Things.Add(NewThing);
//       db.SaveChanges();
//       HttpContext.Session.SetInt32("Tid", NewThing.Tid);
//       return RedirectToAction("ThingInfo", new{wid = NewThing.Tid});
//     }

//     [HttpGet("thing/{tid}")]
//     public IActionResult ThingInfo(int tid)
//     {
//       if (UserLoggedIn()){
//         int? currentTid = HttpContext.Session.GetInt32("Tid");
//         Thing viewModel = db.Things.Include(t => t.ThingGuests).ThenInclude(g => g.Guest).FirstOrDefault(t => t.Tid == (int)tid);
//         return View(viewModel);
//       }

//       return RedirectToAction("Index", "User");
//     }

//     [HttpGet("dashboard")]
//     public IActionResult Dashboard()
//     {
//       if (UserLoggedIn()){
//         DashboardView viewModel = new DashboardView{
//           CurrentUid = (int)HttpContext.Session.GetInt32("Uid"), 
//           Things = db.Things.Include(t => t.ThingGuests).ToList(),
//         };
//         return View(viewModel);
//       }
//       // // DEBUG START
//       // HttpContext.Session.SetInt32("Uid", 1);
//       // DashboardView viewModel = new DashboardView{
//       //   Things = db.Things.Include(t => t.ThingGuests).ToList(),
//       // };

//       // viewModel.CurrentUid = 1;
//       // return View(viewModel);
//       // // DEBUG END

//       return RedirectToAction("Index", "User");
//     }

//     [HttpPost("action")]
//     public IActionResult ThingAction(DashboardView TGform)
//     {
//       int? Uid = HttpContext.Session.GetInt32("Uid");
//       if(Uid == null)
//         return RedirectToAction("Index", "User");
//       if(!ModelState.IsValid){
//         return RedirectToAction("Dashboard");
//       }
//       TGform.TG.GuestId = (int)Uid;
//       // delete if current user if is owner
//       Thing CurrentThing = db.Things.FirstOrDefault(t => t.Tid == TGform.thingId);
//       bool isOwner = CurrentThing.CreatorId == (int)Uid;
//       if (isOwner){
//         db.Things.Remove(CurrentThing);
//         return RedirectToAction("Dashboard");
//       }
//       // RSVP if not currently a guest
//       bool isGuest = db.ThingGuests.Where(tg => tg.ThingId == TGform.thingId).Any(tg => tg.GuestId == TGform.TG.GuestId);
//       if (!isGuest && Uid != null){
//         TGform.TG.GuestId = (int)Uid;
//         db.Add(TGform.TG);
//         db.SaveChanges();
//         return RedirectToAction("Dashboard");
//       }

//       // un-RSVP if already a guest
//       ThingGuest currentGuest = db.ThingGuests.Where(tg => tg.ThingId == TGform.thingId).FirstOrDefault(tg => tg.GuestId == TGform.TG.GuestId);
//       db.ThingGuests.Remove(currentGuest);
//       return RedirectToAction("Dashboard");
//     }
//   }
// }