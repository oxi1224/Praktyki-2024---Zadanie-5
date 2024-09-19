using System.ComponentModel.DataAnnotations;

namespace Zadanie_5.Models {
  public class Klienci {
    [Key]
    public int Id { get; set; }
    [MaxLength(50)]
    public string Name { get; set; }
    [MaxLength(50)]
    public string Surname { get; set; }
    [StringLength(11)]
    public string PESEL { get; set; }
    public int BirthYear { get; set; }
    public int Płeć { get; set; }
    
    public static int GetBirthYear(string PESEL) {
			if (PESEL.Length != 11) return 0;
			int year = int.Parse(PESEL.Substring(0, 2));
			int month = int.Parse(PESEL.Substring(2, 2));

			if (month <= 12) {
				year += 1900;
			} else if (month <= 32) {
				year += 2000;
			} else if (month <= 52) {
				year += 2100;
			} else if (month <= 72) {
				year += 1800;
			}
			return year;
		}

    public static int GetPłeć(string PESEL) {
			if (PESEL.Length != 11) return 0;
			return int.Parse(PESEL[9].ToString()) % 2 == 0 ? 1 : 2;
		}

    public static bool ValidBirthYear(string PESEL, int birthYear) {
      if (PESEL.Length != 11) return false;
      return GetBirthYear(PESEL) == birthYear;
		}

    public static bool ValidPłeć(string PESEL, int płeć) {
      return GetPłeć(PESEL) == płeć;
		}
  }
}