using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Zadanie_5.Models;

namespace Zadanie_5.Controllers {
  public class TestController : Controller {
    private readonly TestDBContext ctx;
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
        ModelState.AddModelError("PESEL", "Numer PESEL nie posiada 11 znaków");
      } else if (!Klienci.ValidBirthYear(klient.PESEL, klient.BirthYear)) {
        ModelState.AddModelError("Rok urodzenia", "Rok urodzenia nie zgadza się z numerem PESEL");
      } else if (!Klienci.ValidPłeć(klient.PESEL, klient.Płeć)) {
        ModelState.AddModelError("Płeć", "Płeć nie zgadza się z numerem PESEL");
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

    [HttpGet("Import")]
    public IActionResult Import() {
      return View();
    }

    static private readonly string[] validHeaders = { "Id", "Name", "Surname", "PESEL", "BirthYear", "Płeć" };
    [HttpPost("Import")]
    public IActionResult Import(IFormFile File) {
      if (File == null || File.Length == 0) {
        ViewBag.Error = "Nie wybrano pliku";
        return View();
      }
      string extension = Path.GetExtension(File.FileName).ToLowerInvariant();
      if (extension != ".csv" && extension != ".xlsx") {
				ViewBag.Error = "Nieprawidłowy typ pliku, dozwolone są tylko pliki .csv i .xlsx";
				return View();
			}
      if (extension == ".csv") {
        StreamReader reader = new(File.OpenReadStream(), true);
        string headerLine = reader.ReadLine()!.Trim();
        string delim = headerLine.Contains(";") ? ";" : ",";
        string[] headers = headerLine.Split(delim);
				if (!Enumerable.SequenceEqual(headers, validHeaders)) {
					ViewBag.Error = "Nagłówki pliku się nie zgadzają, upewnij się że są one następujące: " + string.Join(", ", validHeaders);
					return View();
				}
        while (!reader.EndOfStream) {
          Klienci k = new();
          string[] dane = reader.ReadLine()!.Trim().Split(",");
          if (dane.Length != validHeaders.Length) {
						ViewBag.Error = "Długość danych w linijce pliku nie odpowiada ilości nagłówków";
						return View();
					}
          string pesel = dane[3];
          int birthyear = int.Parse(dane[4]);
          int płeć = int.Parse(dane[5]);
					if (Klienci.ValidBirthYear(pesel, birthyear)) {
						ViewBag.Error = "Rok urodzenia nie zgadza się z numerem PESEL";
						return View();
					}
					if (Klienci.ValidPłeć(pesel, płeć)) {
						ViewBag.Error = "Płeć nie zgadza się z numerem PESEL";
						return View();
					}
					k.Id = int.Parse(dane[0]);
          k.Name = dane[1];
          k.Surname = dane[2];
          k.PESEL = pesel;
          k.BirthYear = birthyear;
          k.Płeć = płeć;
          ctx.Klienci.Add(k);
        }
        ctx.SaveChanges();
      } else if (extension == ".xlsx") {
        using var package = new ExcelPackage(File.OpenReadStream());
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        if (worksheet == null) {
					ViewBag.Error = "Plik jest pusty lub niepoprawny";
					return View();
				}
        string[] headers = new string[validHeaders.Length];
        for (int col = 1; col <= validHeaders.Length; col++) {
          headers[col - 1] = worksheet.Cells[1, col].Text;
        }
				if (!Enumerable.SequenceEqual(headers, validHeaders)) {
					ViewBag.Error = "Nagłówki pliku się nie zgadzają, upewnij się że są one następujące: " + string.Join(", ", validHeaders);
					return View();
				}
        for (int row = 2; row <= worksheet.Dimension.End.Row; row++) {
          Klienci k = new();
          string id = worksheet.Cells[row, 1].Text;
          if (
            string.IsNullOrWhiteSpace(id) ||
            string.IsNullOrWhiteSpace(worksheet.Cells[row, 6].Text)
          ) {
					  ViewBag.Error = $"Dane w rzędzie {row} nie są poprawne";
					  return View();
				  }
					// Pesel jest czytano jako np: 1,234567891E+10, dlatego należy zamienić na double i spowrotem string
					string pesel = double.Parse(worksheet.Cells[row, 4].Text).ToString();
					int birthyear = int.Parse(worksheet.Cells[row, 5].Text);
					int płeć = int.Parse(worksheet.Cells[row, 6].Text);
					if (Klienci.ValidBirthYear(pesel, birthyear)) {
						ViewBag.Error = "Rok urodzenia nie zgadza się z numerem PESEL";
						return View();
					}
					if (Klienci.ValidPłeć(pesel, płeć)) {
						ViewBag.Error = "Płeć nie zgadza się z numerem PESEL";
						return View();
					}

					k.Id = int.Parse(id);
          k.Name = worksheet.Cells[row, 2].Text;
          k.Surname = worksheet.Cells[row, 3].Text; ;
          k.PESEL = pesel;
					k.BirthYear = birthyear;
          k.Płeć = płeć;
					ctx.Klienci.Add(k);
        }
				ctx.SaveChanges();
			}
      ViewBag.Success = "Pomyślnie zaimportowano klientów";
      return View();
    }

    [HttpGet("Export")]
    public IActionResult Export(int type) {
      if (type == 1) {
        IEnumerable<Klienci> klienci = ctx.Klienci.ToList().OrderBy(k => k.Id);
        StringBuilder csv = new();
        csv.AppendLine(string.Join(",", validHeaders));
        foreach (Klienci k in klienci) {
          csv.AppendLine($"{k.Id},{k.Name},{k.Surname},{k.PESEL},{k.BirthYear},{k.Płeć}");
        }
        byte[] bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv", "klienci.csv");
      } else if (type == 2) {
				IEnumerable<Klienci> klienci = ctx.Klienci.ToList().OrderBy(k => k.Id);
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Klienci");
        
        for (int i = 0; i < validHeaders.Length; i++) {
          worksheet.Cells[1, i + 1].Value = validHeaders[i];
        }
        int row = 2;
        foreach (Klienci k in klienci) {
					worksheet.Cells[row, 1].Value = k.Id;
					worksheet.Cells[row, 2].Value = k.Name;
					worksheet.Cells[row, 3].Value = k.Surname;
					worksheet.Cells[row, 4].Value = k.PESEL;
					worksheet.Cells[row, 5].Value = k.BirthYear;
					worksheet.Cells[row, 6].Value = k.Płeć;
					row++;
        }
        byte[] bytes = package.GetAsByteArray();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "klienci.xlsx");
			} else {
        return RedirectToAction("Index");
      }
    }
	}
}
