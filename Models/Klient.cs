using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zadanie_5.Models {
  public class Klient {
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
  }
}