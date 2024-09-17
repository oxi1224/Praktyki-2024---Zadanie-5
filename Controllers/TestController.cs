using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
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
      Console.WriteLine(ctx.Klient.ToList());
      return View(ctx.Klient.ToList());
    }

    public IActionResult Create(Klient klient) {
      ctx.Klient.Add(klient);
      ctx.SaveChanges();
      return View(ctx.Klient.ToList());
    }
    public IActionResult Edit(Klient klient) {
      Klient? k = ctx.Klient.Find(klient.Id);
      if (k != null) {
        k.Name = klient.Name;
        k.Surname = klient.Surname;
        k.PESEL = klient.PESEL;
        k.BirthYear = klient.BirthYear;
        k.Płeć = klient.Płeć;
        ctx.SaveChanges();
      }
      
      return View(ctx.Klient.ToList());
    }
    public IActionResult Delete(int id) {
      Klient? k = ctx.Klient.Find(id);
      if (k != null) ctx.Klient.Remove(k);
      ctx.SaveChanges();
      return View(ctx.Klient.ToList());
    }
  }
}
