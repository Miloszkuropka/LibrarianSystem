using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Windows;
using SystemBiblioteczny.Models;

namespace SystemBiblioteczny.Methods
{
    class LoginMethod
    {
        private AccountBase accountModel = new();
        public bool CheckLogin(string Login, string Password, AccountBase.RoleTypeEnum role)
        {
            bool Logged = false;
            string file = "";

            switch (role)
            {
                case (AccountBase.RoleTypeEnum.Client): file = "ClientList"; break;
                case (AccountBase.RoleTypeEnum.Librarian): file = "LibrarianList"; break;
                case (AccountBase.RoleTypeEnum.LocalAdmin): file = "LocalAdminList"; break;
                case (AccountBase.RoleTypeEnum.NetworkAdmin): file = "NetworkAdminList"; break;
            }

            List<Person> list = new();
            List<string> lines = accountModel.GetListOfDataBaseLines(file);


            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                string[] splitted = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                string newLogin = splitted[0];
                string newPassword = splitted[1];
                string firstName = splitted[2];
                string lastName = splitted[3];
                string email = splitted[4];
                int newIdLibrary = 0;
                if (splitted.Length >= 6) newIdLibrary = int.Parse(splitted[5]);
                string phone = "";
                if (splitted.Length >= 7) phone = splitted[6];

                switch (role)
                {
                    case (AccountBase.RoleTypeEnum.Client):
                        {
                            Client person = new(newLogin, newPassword, firstName, lastName, email, phone);
                            list.Add(person);
                        }
                        break;
                    case (AccountBase.RoleTypeEnum.Librarian):
                        {
                            Librarian person = new(newLogin, newPassword, firstName, lastName, email, newIdLibrary, phone);
                            list.Add(person);
                        }
                        break;
                    case (AccountBase.RoleTypeEnum.LocalAdmin):
                        {
                            LocalAdmin person = new(newLogin, newPassword, firstName, lastName, email, newIdLibrary, phone);
                            list.Add(person);
                        }
                        break;
                    case (AccountBase.RoleTypeEnum.NetworkAdmin):
                        {
                            NetworkAdmin person = new(newLogin, newPassword, firstName, lastName, email, phone);
                            list.Add(person);
                        }
                        break;
                }
            }

            bool wrongPassword = false;
            bool correctLogin = false;

            for (int j = 0; j < list.Count; j++)
            {
                string CheckLogin = list[j].UserName!;
                if (CheckLogin.CompareTo(Login) == 0)
                {
                    string CheckPassword = list[j].Password!;
                    if (CheckPassword.CompareTo(Password) == 0)
                    {
                        MessageBox.Show("Poprawnie zalogowano");
                        correctLogin = true;
                        Logged = true;

                        switch (role)
                        {
                            case (AccountBase.RoleTypeEnum.Client):
                                {
                                    Client userData = (Client)list[j];
                                    ClientWindow clientwindow = new(userData);
                                    clientwindow.Show();
                                }
                                break;
                            case (AccountBase.RoleTypeEnum.Librarian):
                                {
                                    Librarian userData = (Librarian)list[j];
                                    LibrarianWindow librarianwindow = new(userData);
                                    librarianwindow.Show();
                                }
                                break;
                            case (AccountBase.RoleTypeEnum.LocalAdmin):
                                {
                                    LocalAdmin userData = (LocalAdmin)list[j];
                                    Admin_LocalWindow admin_localwindow = new(userData);
                                    admin_localwindow.Show();
                                }
                                break;
                            case (AccountBase.RoleTypeEnum.NetworkAdmin):
                                {
                                    NetworkAdmin userData = (NetworkAdmin)list[j];
                                    Admin_NetworkWindow admin_networkwindow = new(userData);
                                    admin_networkwindow.Show();
                                }
                                break;
                        }

                    }
                    else wrongPassword = true;
                }

            }

            if (wrongPassword == true) MessageBox.Show("Podane hasło jest nieprawidłowe");
            else if (correctLogin == false) MessageBox.Show("Nie znaleziono danego użytkownika");
            return Logged;
        }
        private bool CheckIfUsernameIsUnique(string Login)
        {

            List<Person> list = new();
            List<string> lines = accountModel.GetListOfDataBaseLines("ClientList");

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];

                string[] splitted = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                string CheckLogin = splitted[0];
                if (CheckLogin.CompareTo(Login) == 0)
                {
                    return false;
                }
            }


            return true;
        }

        public bool CheckIfAllDataIsCorrectAndCanCreateAccount(string username, string password, string confirmPassword, string name, string lastname, string email)
        {
            bool unique = this.CheckIfUsernameIsUnique(username);
            if (username.Length < 4)
            {
                MessageBox.Show("Nazwa użytkownika musi mieć przynajmniej 4 znaki!");
                return false;
            }
            if (unique == false)
            {
                MessageBox.Show("Użytkownik o takiej nazwie już istnieje!");
                return false;
            }
            if (password.Length < 4)
            {
                MessageBox.Show("Hasło musi mieć przynajmiej 4 znaki!");
                return false;
            }
            if (password != confirmPassword)
            {
                MessageBox.Show("Powtórzone hasło nie jest takie samo!");
                return false;
            }
            if (name.Length < 1)
            {
                MessageBox.Show("Imię nie może być puste!");
                return false;
            }
            if (lastname.Length < 1)
            {
                MessageBox.Show("Nazwisko nie może być puste!");
                return false;
            }
            try
            {
                MailAddress m = new MailAddress(email);
            }
            catch (FormatException)
            {
                MessageBox.Show("Błędny format email!");
                return false;
            }
            MessageBox.Show("Użytkownik został utworzony");
            return true;
        }
        public string EraseWhiteSpace(string s1)
        {
            for (int i = 0; i < s1.Length; i++)
            {
                if (Char.IsWhiteSpace(s1[i]))
                {
                    MessageBox.Show("Nie używaj spacji! \n Zamist tego użyj _'");
                    s1 = s1.Replace(" ", "");
                }
            }
            return s1;
        }

        internal int GetNumberOfRentals(int libraryId, DateTime? selectedDate1, DateTime? selectedDate2)
        {
            if (selectedDate1 == null) selectedDate1 = DateTime.MinValue;
            if (selectedDate2 == null) selectedDate2 = DateTime.Now;
            AccountBase history = new();
            BooksReserved b = new();
            List<BookReserved> AllBooksReservedList = b.GetReservedBooksList();
            List<string> listHistory = history.GetListOfDataBaseLines("BookHistory");
            int counter = 0;
            for (int i = 0; i < AllBooksReservedList.Count; i++)
            {
                if (AllBooksReservedList[i].Availability == true && AllBooksReservedList[i].Id_Library == libraryId &&
                    DateTime.Parse(AllBooksReservedList[i].DateTime1) >= selectedDate1
                    && DateTime.Parse(AllBooksReservedList[i].DateTime1) <= selectedDate2)


                {
                    counter++;
                }
            }
            for (int j = 0; j < listHistory.Count; j++)
            {
                string line = listHistory[j];
                string[] splitted = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                string id = splitted[1];
                string date = splitted[5];
                if (int.Parse(id) == libraryId && DateTime.Parse(date) >= selectedDate1 && DateTime.Parse(date) <= selectedDate2)
                    counter++;
            }
            return counter;
        }

        internal int GetNumberOfBooksInLibrary(int libraryId)
        {
            Books a = new();
            List<Book> AllBooksList = a.GetBooksList();
            int BooksCount = 0;
            for (int i = 0; i < AllBooksList.Count; i++)
            {
                if (AllBooksList[i].Id_Library == libraryId)
                {
                    BooksCount++;
                }
            }
            return BooksCount;
        }

        internal int GetNumberOfLibrarians(int libraryId)
        {
            List<Librarian> AllLibrarians = accountModel.GetLibrarianList();
            int allLibrans = 0;
            for (int i = 0; i < AllLibrarians.Count; i++)
            {
                Librarian librarian = AllLibrarians[i];
                if (librarian.LibraryId == libraryId)
                {
                    allLibrans++;
                }
            }
            return allLibrans;
        }

        internal int GetNumberOfEvents(int libraryId, DateTime? selectedDate1, DateTime? selectedDate2)
        {
            if (selectedDate1 == null) selectedDate1 = DateTime.MinValue;
            if (selectedDate2 == null) selectedDate2 = DateTime.Now;
            AuthorsEvenings evenings = new();
            List<AuthorsEvening> AuthorEvenings = evenings.GetEventList();
            int AllAuthorEvenings = 0;
            for (int i = 0; i < AuthorEvenings.Count; i++)
            {
                AuthorsEvening f = AuthorEvenings[i];
                if (f.LibraryID == libraryId && f.Date >= selectedDate1 && f.Date <= selectedDate2)
                {
                    AllAuthorEvenings++;
                }

            }
            return AllAuthorEvenings;
        }

        internal int GetNumberOfActiveUsers(int libraryId, DateTime? selectedDate1, DateTime? selectedDate2)
        {
            if (selectedDate1 == null) selectedDate1 = DateTime.MinValue;
            if (selectedDate2 == null) selectedDate2 = DateTime.Now;
            List<string> list = accountModel.GetListOfDataBaseLines("BookHistory");
            List<string> userList = new();
            userList.Add("");
            int AllActiveClients = 0;
            for (int i = 0; i < list.Count; i++)
            {
                string line = list[i];
                string[] splitted = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                int libId = int.Parse(splitted[1]);
                string user = splitted[4];
                DateTime rentalDate = DateTime.Parse(splitted[5]);
                if (libId == libraryId && rentalDate >= selectedDate1 && rentalDate <= selectedDate2)
                {
                    for (int j = 0; j < userList.Count; j++)
                    {
                        if (userList[j].CompareTo(user) != 0)
                        {
                            AllActiveClients++;
                            userList.Add(user);
                        }
                    }
                }

            }
            return AllActiveClients;
        }

        internal int GetNumberRenalsAll(DateTime? selectedDate1, DateTime? selectedDate2)
        {
            if (selectedDate1 == null) selectedDate1 = DateTime.MinValue;
            if (selectedDate2 == null) selectedDate2 = DateTime.Now;
            AccountBase history = new();
            BooksReserved b = new();
            List<BookReserved> AllBooksReservedList = b.GetReservedBooksList();
            List<string> listHistory = history.GetListOfDataBaseLines("BookHistory");
            int counter = 0;
            for (int i = 0; i < AllBooksReservedList.Count; i++)
            {
                if (AllBooksReservedList[i].Availability == true &&
                    DateTime.Parse(AllBooksReservedList[i].DateTime1) >= selectedDate1
                    && DateTime.Parse(AllBooksReservedList[i].DateTime1) <= selectedDate2)


                {
                    counter++;
                }
            }
            for (int j = 0; j < listHistory.Count; j++)
            {
                string line = listHistory[j];
                string[] splitted = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                string date = splitted[5];
                if (DateTime.Parse(date) >= selectedDate1 && DateTime.Parse(date) <= selectedDate2)
                    counter++;
            }
            return counter;
        }

        internal int GetNumberOfEventsAll(DateTime? selectedDate1, DateTime? selectedDate2)
        {
            if (selectedDate1 == null) selectedDate1 = DateTime.MinValue;
            if (selectedDate2 == null) selectedDate2 = DateTime.Now;
            AuthorsEvenings evenings = new();
            List<AuthorsEvening> AuthorEvenings = evenings.GetEventList();
            int AllAuthorEvenings = 0;
            for (int i = 0; i < AuthorEvenings.Count; i++)
            {
                AuthorsEvening f = AuthorEvenings[i];
                if (f.Date >= selectedDate1 && f.Date <= selectedDate2)
                {
                    AllAuthorEvenings++;
                }
            }
            return AllAuthorEvenings;
        }

        internal int GetNumberOfActiveUsersAll(DateTime? selectedDate1, DateTime? selectedDate2)
        {
            if (selectedDate1 == null) selectedDate1 = DateTime.MinValue;
            if (selectedDate2 == null) selectedDate2 = DateTime.Now;
            List<string> list = accountModel.GetListOfDataBaseLines("BookHistory");
            List<string> userList = new();
            userList.Add("");
            int AllActiveClients = 0;
            for (int i = 0; i < list.Count; i++)
            {
                string line = list[i];
                string[] splitted = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                int libId = int.Parse(splitted[1]);
                string user = splitted[4];
                DateTime rentalDate = DateTime.Parse(splitted[5]);
                if (rentalDate >= selectedDate1 && rentalDate <= selectedDate2)
                {
                    for (int j = 0; j < userList.Count; j++)
                    {
                        if (userList[j].CompareTo(user) != 0)
                        {
                            AllActiveClients++;
                            userList.Add(user);
                        }
                    }
                }

            }
            return AllActiveClients;
        }
    }
}

