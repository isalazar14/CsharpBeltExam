using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using BeltExam.Models;
using Microsoft.EntityFrameworkCore;

namespace BeltExam.Controllers
{
  public class UserController : Controller
  {
    private BeltExamContext db;

    public UserController(BeltExamContext context)
    {
      db = context;
    }

    public bool inSession()
    {
      int? Uid = HttpContext.Session.GetInt32("Uid");
      if (Uid == null){
        return false;
      }
      return true;
    }

        public bool UserLoggedIn()
    {
      int? Uid = HttpContext.Session.GetInt32("Uid");
      if (Uid == null){
        return false;
      }
      return true;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
      // LoginRegViewModel viewModel = new LoginRegViewModel();
      // return View(viewModel);
      int? Uid = HttpContext.Session.GetInt32("Uid");
      if (Uid == null){
        return View();
      }
      return RedirectToAction("Dashboard");
    }

    [HttpGet("register")]
    public IActionResult Register()
    {
      int? Uid = HttpContext.Session.GetInt32("Uid");
      if (Uid == null){
        return RedirectToAction("Index");
      }
      return RedirectToAction("Dashboard");
    }

    [HttpPost("register")]
    public IActionResult Register(LoginRegViewModel NewUser)
    {
      if (!ModelState.IsValid)
      {
        return View("Index", NewUser);
      }

      if (db.Users.Any(user => user.Email == NewUser.User.Email))
      {
        ModelState.AddModelError("Email", "Email already registered");
        return View("Index", NewUser);
      }

      PasswordHasher<User> hasher = new PasswordHasher<User>();
      NewUser.User.PwHash = hasher.HashPassword(NewUser.User, NewUser.User.Password);

      db.Add(NewUser.User);
      db.SaveChanges();

      User dbUser = db.Users.FirstOrDefault(user => user.Email == NewUser.User.Email);
      ViewBag.Fname = NewUser.User.Fname;
      HttpContext.Session.SetInt32("Uid", NewUser.User.Uid);
      
      return RedirectToAction("Dashboard");
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
      int? Uid = HttpContext.Session.GetInt32("Uid");
      if (Uid == null){
        return RedirectToAction("Index");
      }
      return RedirectToAction("Dashboard");
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRegViewModel LoginInfo)
    {
      if (!ModelState.IsValid)
      {
        return View("Index", LoginInfo);
      }

      User dbUser = db.Users.FirstOrDefault(user => user.Email == LoginInfo.LoginUser.LoginEmail);

      if (dbUser == null)
      {
        ModelState.AddModelError("Email", "Email not found, please register.");
        return View("Index", LoginInfo);
      }

      PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
      PasswordVerificationResult result = hasher.VerifyHashedPassword(LoginInfo.LoginUser, dbUser.PwHash, LoginInfo.LoginUser.LoginPassword);

      if (result == 0)
      {
        ModelState.AddModelError("Password", "Incorrect password");
        return View("Index", LoginInfo);
      }

      HttpContext.Session.SetInt32("Uid", dbUser.Uid);
      return RedirectToAction("Dashboard", "User");
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
      HttpContext.Session.Clear();
      return RedirectToAction("Index");
    }

    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
      if (UserLoggedIn()){
        DashboardView viewModel = new DashboardView{
          CurrentUid = (int)HttpContext.Session.GetInt32("Uid"), 
          // Things = db.Things.Include(t => t.Creator).Include(t => t.ThingGuests).Where(t => t.Date >= DateTime.Today && t.Time >= DateTime.Now).ToList()
          Things = db.Things.Include(t => t.Creator).Include(t => t.ThingGuests).Where(t => t.StartDateTime >= DateTime.Now).ToList()
        };
        return View(viewModel);
      }
      // // DEBUG START
      // HttpContext.Session.SetInt32("Uid", 1);
      // DashboardView viewModel = new DashboardView{
      //   Things = db.Things.Include(t => t.ThingGuests).ToList(),
      // };

      // viewModel.CurrentUid = 1;
      // return View(viewModel);
      // // DEBUG END

      return RedirectToAction("Index");
    }

    [HttpGet("new")]
    public IActionResult NewThing()
    {
      if (UserLoggedIn()){
        return View();
      }
      return RedirectToAction("Index");
    }

    [HttpPost("new")]
    public IActionResult NewThing(Thing NewThing)
    {
      if (UserLoggedIn()){
        if (!ModelState.IsValid){
          return View(NewThing);
        }
        int? Uid = HttpContext.Session.GetInt32("Uid");
        NewThing.CreatorId = (int)Uid;
        db.Things.Add(NewThing);
        db.SaveChanges();
        ThingGuest Guest = new ThingGuest{
          ThingId = NewThing.Tid,
          GuestId = (int)Uid,
        };
        db.ThingGuests.Add(Guest);
        db.SaveChanges();
        HttpContext.Session.SetInt32("Tid", NewThing.Tid);
        return RedirectToAction("ThingInfo", new{tid = NewThing.Tid});
      }

      return RedirectToAction("Index");
    }

    [HttpGet("thing/{tid}")]
    public IActionResult ThingInfo(int tid)
    {
      if (UserLoggedIn()){
        int? currentTid = HttpContext.Session.GetInt32("Tid");
        Thing viewModel = db.Things.Include(t => t.Creator).Include(t => t.ThingGuests).ThenInclude(g => g.Guest).FirstOrDefault(t => t.Tid == (int)tid);
        if (viewModel == null){
          return RedirectToAction("Dashboard");
        }
        ViewBag.Uid = (int)HttpContext.Session.GetInt32("Uid");
        return View(viewModel);
      }

      return RedirectToAction("Index", "User");
    }

    [HttpGet("action/{tid}")]
    public IActionResult ThingAction(DashboardView TGform, int tid)
    {
      int? Uid = HttpContext.Session.GetInt32("Uid");
      if(Uid == null)
        return RedirectToAction("Index", "User");
        // if(!ModelState.IsValid){
        //   return RedirectToAction("Dashboard");
        // }
      // TGform.TG.GuestId = (int)Uid;
      Thing CurrentThing = db.Things.Include(t => t.ThingGuests).FirstOrDefault(t => t.Tid == tid);
      if (CurrentThing == null){
        return RedirectToAction("Dashboard");
      }
      // delete if current user if is owner
      bool isOwner = CurrentThing.CreatorId == (int)Uid;
      if (isOwner){
        db.Things.Remove(CurrentThing);
      }
      // bool isGuest = db.ThingGuests.Where(tg => tg.ThingId == TGform.thingId).Any(tg => tg.GuestId == TGform.TG.GuestId);
      bool isGuest = CurrentThing.ThingGuests.Any(tg => tg.GuestId == (int)Uid);
      if (isGuest){
      // un-RSVP if already a guest
        ThingGuest currentGuest = db.ThingGuests.Where(tg => tg.ThingId == CurrentThing.Tid).FirstOrDefault(tg => tg.GuestId == (int)Uid);
        db.ThingGuests.Remove(currentGuest);
      }
      else {
      // if not currently a guest...
        // check if user already joined other things happening at the same time
        List<ThingGuest> TGs = db.ThingGuests.Include(tg => tg.Thing).Where(tg => tg.GuestId == (int)Uid).ToList();
        // User currentUser = db.Users.Include(u => u.ThingsAttending).ThenInclude(tg => tg.Thing).FirstOrDefault(u => u.Uid == Uid);
        bool isFree = true;
        foreach (ThingGuest tg in TGs){
          if ((CurrentThing.EndDateTime > tg.Thing.StartDateTime
                && CurrentThing.EndDateTime < tg.Thing.EndDateTime)
              ||
               (CurrentThing.StartDateTime < tg.Thing.EndDateTime
                && CurrentThing.StartDateTime > tg.Thing.StartDateTime))
          {
            isFree = false;
            ModelState.AddModelError("Busy","Can't join, you're busy");
            break;
          }
        }
        // db.Things.Include(t => t.ThingGuests). .Where(tg => tg. == Uid).Any(t => t.);
        if (isFree){
        ThingGuest NewGuest = new ThingGuest {
          ThingId = CurrentThing.Tid,
          GuestId = (int)Uid,
        };
        db.Add(NewGuest);
        }
      }

      db.SaveChanges();
      return RedirectToAction("Dashboard");
    }



    // [HttpGet("welcome")]
    // public IActionResult Welcome()
    // {
    //   int? Uid = HttpContext.Session.GetInt32("Uid");
    //   if (Uid == null){
    //     return RedirectToAction("Index");
    //   }

    //   User user = db.Users.FirstOrDefault(u => u.Uid == Uid);
    //   ViewBag.Fname = user.Fname;
    //   return View();
    // }

    public IActionResult Privacy()
    {
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
