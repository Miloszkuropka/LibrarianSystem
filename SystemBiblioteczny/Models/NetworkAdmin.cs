namespace SystemBiblioteczny.Models
{
    public class NetworkAdmin : Person
    {

        public NetworkAdmin(string UserName, string Password, string FirstName, string LastName, string Email, string Phone)
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Password = Password;
            this.UserName = UserName;
            this.Email = Email;
            this.Phone = Phone;
        }

        public NetworkAdmin()
        {
        }

    }
}
