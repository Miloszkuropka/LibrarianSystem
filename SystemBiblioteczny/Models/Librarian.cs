namespace SystemBiblioteczny.Models
{
    public class Librarian : Person
    {
        public int LibraryId { get; set; }

        public Librarian(string UserName, string Password, string FirstName, string LastName, string Email, int LibraryId, string Phone = "0")
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Password = Password;
            this.UserName = UserName;
            this.Email = Email;
            this.LibraryId = LibraryId;
            this.Phone = Phone;
        }

        public Librarian() { }

    }
}
