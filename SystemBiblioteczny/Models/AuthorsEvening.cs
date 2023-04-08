using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace SystemBiblioteczny.Models
{
    public class AuthorsEvening
    {
        public bool Approved { get; set; }
        public string User { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int LibraryID { get; set; }
        public DateTime? Date { get; set; }
        public int Hour { get; set; }
        public string PhoneNumber { get; set; }
        public AuthorsEvening(bool approved, string owner, string firstname, string lastName, int id, DateTime? date, int hour, string phoneNumber)
        {
            Approved = approved;
            User = owner;
            FirstName = firstname;
            LastName = lastName;
            LibraryID = id;
            Date = date;
            Hour = hour;
            PhoneNumber = phoneNumber;
        }
        public AuthorsEvening() { }

        public bool TryAddToDataBase()
        {
            if (FirstName.Length < 2)
            {
                MessageBox.Show("Imię autora powinno mieć przynajmniej 2 znaki!");
                return false;
            }
            if (LastName.Length < 2)
            {
                MessageBox.Show("Nazwisko autora powinno mieć przynajmniej 2 znaki!");
                return false;
            }
            AuthorsEvenings a = new();
            if (!a.CheckIfLibraryExist(LibraryID))
            {
                MessageBox.Show("Biblioteka o takim numerze nie istnieje!");
                return false;
            }
            if (Date == null)
            {
                MessageBox.Show("Wybierz datę!");
                return false;
            }
            if (Hour > 22 || Hour < 8)
            {
                MessageBox.Show("Podaj pełną godzinę od 8 do 22!");
                return false;
            }
            if (!Regex.Match(PhoneNumber, "^\\d{9}$").Success)
            {
                MessageBox.Show("Podaj numer telefonu jako 9 cyfr!");
                return false;
            }
            a.Add(this);
            return true;
        }

        internal void AddToDataBase()
        {
            AuthorsEvenings a = new();
            a.Add(this);
        }
        public bool Equals(AuthorsEvening? other)
        {
            if (other is null) return false;
            return FirstName == other.FirstName && LastName == other.LastName && Date == other.Date && Hour == other.Hour && LibraryID == other.LibraryID && PhoneNumber == other.PhoneNumber;
        }
    }
}
