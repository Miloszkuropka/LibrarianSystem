namespace SystemBiblioteczny.Models
{
    public class Client : Person
    {
        public Client(string UserName, string Password, string FirstName, string LastName, string Email, string Phone)
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Password = Password;
            this.UserName = UserName;
            this.Email = Email;
            this.Phone = Phone;
        }
        public Client() { }


    }
}
