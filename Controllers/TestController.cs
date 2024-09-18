using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zadanie_5.Models;

// await using var ctx = new TestDBContext();
// await ctx.Database.EnsureDeletedAsync();
// await ctx.Database.EnsureCreatedAsync();

namespace Zadanie_5.Controllers {
  public class TestController : Controller {
    private readonly TestDBContext ctx;
    // GET: Test
    public TestController(TestDBContext context) {
      ctx = context;
    }
    public IActionResult Index() {
      return View(ctx.Klienci.ToList().OrderBy(k => k.Id));
    }

    [HttpGet("Create")]
    public IActionResult Create() {
      return View();
    }

    [HttpPost("Create")]
    public IActionResult Create(Klienci klient) {
      if (klient.PESEL.Length != 11) {
        ModelState.AddModelError("Error", "Numer PESEL nie posiada 11 znaków");
      } else {
        int year = int.Parse(klient.PESEL.Substring(0, 2));
        int month = int.Parse(klient.PESEL.Substring(2, 2));
        
        if (month <= 12) {
          year += 1900;
        } else if (month <= 32) {
          year += 2000;
        } else if (month <= 52) {
          year += 2100;
        } else if (month <= 72) {
          year += 1800;
        }
        if (year != klient.BirthYear) {
          ModelState.AddModelError("Error", "Rok urodzenia nie zgadza się z numerem PESEL");
        } else {
          int gender = int.Parse(klient.PESEL[9].ToString()) % 2 == 0 ? 1 : 2;
          if (gender != klient.Płeć) {
            ModelState.AddModelError("Error", "Płeć nie zgadza się z numerem PESEL");
          }
        }
      }
      if (!ModelState.IsValid) return View(klient);
      ctx.Klienci.Add(klient);
      ctx.SaveChanges();
      Console.WriteLine("stworzono");
      ViewBag.SuccessMessage = "Klient pomyślnie stworzony";
      return View();
    }

    [HttpGet("Edit")]
	public IActionResult Edit(int id) {
      Klienci? klient = ctx.Klienci.Find(id);
      if (klient == null) return NotFound();
      return View(klient);
    }

	[HttpPost("Edit")]
    public IActionResult Edit(Klienci klient) {
      Klienci? k = ctx.Klienci.Find(klient.Id);
            Console.WriteLine(klient.Id);
      if (k != null) {
        k.Name = klient.Name;
        k.Surname = klient.Surname;
        k.PESEL = klient.PESEL;
        k.BirthYear = klient.BirthYear;
        k.Płeć = klient.Płeć;
        ctx.SaveChanges();
      }
      return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult Delete(int id) {
      Klienci? k = ctx.Klienci.Find(id);
      if (k != null) ctx.Klienci.Remove(k);
      ctx.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}
