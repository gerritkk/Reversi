using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReversiApp.Models
{
    public class Speler
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public string Wachtwoord { get; set; }
        public string Token { get; set; }
        public enum Kleur { Geen, Wit, Zwart };
    }
}
