using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using SystemBiblioteczny.Methods;
using SystemBiblioteczny.Models;

namespace SystemBiblioteczny
{
    /// <summary>
    /// Logika interakcji dla klasy Admin_NetworkWindow.xaml
    /// </summary>
    public partial class Admin_NetworkWindow : Window
    {
        private NetworkAdmin networkAdmin = new();
        private AccountBase accountModel = new();
        private LoginMethod loginMethod = new();
        public Admin_NetworkWindow(NetworkAdmin newNetworkAdmin)
        {
            networkAdmin = newNetworkAdmin;
            InitializeComponent();
            base.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            LoadLibrariesData();
            EmailBox.Text = networkAdmin.Email;
            PhoneBox.Text = networkAdmin.Phone;
        }

        private void Return(object sender, RoutedEventArgs e)
        {
            MainWindow m = new();
            m.Show();
            this.Close();
        }

        private void Add_Library(object sender, RoutedEventArgs e)
        {
            Libraries l = new();
            if (!l.CheckIfCanAdd(City.Text, Street.Text, Number.Text)) return;
            int newID = l.ReturnUniqueID();
            Library library = new(newID, City.Text, Street.Text, Number.Text);
            l.AddLibraryToDB(library);
            MessageBox.Show("Dodano Bibliotekę! \nID: " + newID + "\nAdres: " + City.Text + " " + Street.Text + " " + Number.Text);
            City.Text = "";
            Street.Text = "";
            Number.Text = "";
            LoadLibrariesData();
        }
        private void LoadLibrariesData()
        {
            Libraries_Table.Items.Clear();
            Libraries library = new();
            List<Library> listOfEvents = library.GetListOfLibraries();
            foreach (Library e in listOfEvents)
            {
                Libraries_Table.Items.Add(e);
            }
        }
        private void ChangePassword(object sender, RoutedEventArgs e)
        {
            if (PasswordBox1.Password.CompareTo(PasswordBox2.Password) == 0)
            {
                if (PasswordBox2.Password.Length < 4) MessageBox.Show("Hasło musi mieć przynajmiej 4 znaki");
                else accountModel.ChangePersonData(networkAdmin, AccountBase.RoleTypeEnum.NetworkAdmin, PasswordBox1.Password);
            }
            else MessageBox.Show("Podane hasła różnią się od siebie");
        }

        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EmailBox.Text.CompareTo(networkAdmin.Email) != 0)
                {
                    MailAddress mail = new MailAddress(EmailBox.Text);
                    accountModel.ChangePersonData(networkAdmin, AccountBase.RoleTypeEnum.NetworkAdmin, "", EmailBox.Text);
                }
                if (PhoneBox.Text.CompareTo(networkAdmin.Phone!.ToString()) != 0)
                {
                    if (PhoneBox.Text.CompareTo("") == 0) MessageBox.Show("Nie podano poprawnego numeru telefonu");
                    else accountModel.ChangePersonData(networkAdmin, AccountBase.RoleTypeEnum.NetworkAdmin, "", "", PhoneBox.Text);
                }

            }
            catch (FormatException)
            {
                MessageBox.Show("Błędny format email!");
            }

        }
        private void Password1Changed(object sender, RoutedEventArgs e)
        {
            string a = PasswordBox1.Password;
            string b = loginMethod.EraseWhiteSpace(PasswordBox1.Password);
            if (a != b) PasswordBox1.Password = b;
        }
        private void Password2Changed(object sender, RoutedEventArgs e)
        {
            string a = PasswordBox2.Password;
            string b = loginMethod.EraseWhiteSpace(PasswordBox2.Password);
            if (a != b) PasswordBox2.Password = b;
        }
        private void PhoneBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(PhoneBox.Text, "[^0-9]"))
            {
                MessageBox.Show("Proszę wpisać numer.");
                PhoneBox.Text = PhoneBox.Text.Remove(PhoneBox.Text.Length - 1);
            }
        }
        private void ShowClientList(object sender, RoutedEventArgs e)
        {
            ShowClientListMethod();
        }
        private void ShowClientListMethod()
        {
            Person_Table.Items.Clear();
            List<Client> clients = accountModel.GetClientList();
            foreach (Client c in clients)
            {
                Person_Table.Items.Add(c);
            }
            Person_Table.IsReadOnly = true;
            TabelaName.Content = "Tabela pokazująca listę klientów";
        }
        private void ShowLibrarianList(object sender, RoutedEventArgs e)
        {
            ShowLibrarianListMethod();
        }
        private void ShowLibrarianListMethod()
        {
            Person_Table.Items.Clear();
            List<Librarian> librarians = accountModel.GetLibrarianList();
            foreach (Librarian l in librarians)
            {
                Person_Table.Items.Add(l);
            }
            Person_Table.IsReadOnly = true;
            TabelaName.Content = "Tabela pokazująca listę bibliotekarzy";
        }

        private void ShowAdminList(object sender, RoutedEventArgs e)
        {
            ShowAdminListMethod();
        }
        private void ShowAdminListMethod()
        {
            Person_Table.Items.Clear();
            List<LocalAdmin> admins = accountModel.GetLocalAdminList();
            foreach (LocalAdmin a in admins)
            {
                Person_Table.Items.Add(a);
            }
            Person_Table.IsReadOnly = true;
            TabelaName.Content = "Tabela pokazująca listę administratorów lokalnych";
        }
        private void MakeClientAnLibrarian(object sender, RoutedEventArgs e)
        {
            List<Client> list = accountModel.GetClientList();
            bool info = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].UserName!.CompareTo(UserNameTextBox.Text) == 0)
                {
                    info = true;
                    if (IdLibraryLabel.Text == "") MessageBox.Show("Proszę wpisać poprawne id");
                    else
                    {
                        int newLibId = int.Parse(IdLibraryLabel.Text);
                        Librarian librarian = new(list[i].UserName!, list[i].Password!, list[i].FirstName!, list[i].LastName!, list[i].Email!, newLibId, list[i].Phone!);
                        accountModel.AddLibrarianToListAndDeleteFromClients(librarian);
                        MessageBox.Show("Nadano uprawnienia");
                        ShowLibrarianListMethod();
                    }

                }
            }
            if (info == false) MessageBox.Show("Nie istnieje klient o podanej nazwie");

        }

        private void MakeClientAnAdmin(object sender, RoutedEventArgs e)
        {
            List<Client> list = accountModel.GetClientList();
            bool info = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].UserName!.CompareTo(UserNameTextBox.Text) == 0)
                {
                    info = true;
                    if (IdLibraryLabel.Text == "") MessageBox.Show("Proszę wpisać poprawne id");
                    else
                    {
                        int newLibId = int.Parse(IdLibraryLabel.Text);
                        LocalAdmin admin = new(list[i].UserName!, list[i].Password!, list[i].FirstName!, list[i].LastName!, list[i].Email!, newLibId, list[i].Phone!);
                        accountModel.AddLocalAdminToListAndDeleteFromClients(admin);
                        MessageBox.Show("Nadano uprawnienia");
                        ShowAdminListMethod();
                    }

                }
            }
            if (info == false) MessageBox.Show("Nie istnieje klient o podanej nazwie");
        }

        private void MakePersonAnClient(object sender, RoutedEventArgs e)
        {
            List<LocalAdmin> admins = accountModel.GetLocalAdminList();
            List<Librarian> librarians = accountModel.GetLibrarianList();
            Client client = new();

            bool info = false;
            AccountBase.RoleTypeEnum role = AccountBase.RoleTypeEnum.Client;
            for (int i = 0; i < admins.Count; i++)
            {
                if (admins[i].UserName!.CompareTo(UserNameTextBox.Text) == 0)
                {
                    info = true;
                    role = AccountBase.RoleTypeEnum.LocalAdmin;
                    client = new(admins[i].UserName!, admins[i].Password!, admins[i].FirstName!, admins[i].LastName!, admins[i].Email!, admins[i].Phone!);
                }
            }
            for (int i = 0; i < librarians.Count; i++)
            {
                if (librarians[i].UserName!.CompareTo(UserNameTextBox.Text) == 0)
                {
                    info = true;
                    role = AccountBase.RoleTypeEnum.Librarian;
                    client = new(librarians[i].UserName!, librarians[i].Password!, librarians[i].FirstName!, librarians[i].LastName!, librarians[i].Email!, librarians[i].Phone!);
                }
            }
            if (info == true)
            {
                string path = "";
                if (role == AccountBase.RoleTypeEnum.LocalAdmin) path = System.IO.Path.Combine("../../../DataBases/LocalAdminList.txt");
                if (role == AccountBase.RoleTypeEnum.Librarian) path = System.IO.Path.Combine("../../../DataBases/LibrarianList.txt");

                List<string> lines = new();
                if (role == AccountBase.RoleTypeEnum.LocalAdmin) lines = accountModel.GetListOfDataBaseLines("LocalAdminList");
                if (role == AccountBase.RoleTypeEnum.Librarian) lines = accountModel.GetListOfDataBaseLines("LibrarianList");

                using (StreamWriter writer = new StreamWriter(path))
                {
                    for (int i = 0; i < lines.Count; i++)
                    {
                        string line = lines[i];
                        string[] splitted = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                        string userName = splitted[0];
                        if (userName.CompareTo(UserNameTextBox.Text) == 0) { }
                        else { writer.WriteLine(line); }
                    }
                    writer.Close();
                }
                accountModel.AddClientToList(client);
                MessageBox.Show("Usunięto uprawnienia");
                ShowClientListMethod();
            }

            if (info == false) MessageBox.Show("Nie istnieje osoba o podanej nazwie");

        }

        private void Remove_Library(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dialogResult = MessageBox.Show("Dodanie na nowo biblioteki będzie wiązało się z nadawaniem uprawnień administratorom, bibliotekarzom i przypisywaniem książek!", "Czy chcesz rozpocząć usuwanie biblioteki?", MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes)
            {
                Library library = new();
                library = (Library)Libraries_Table.SelectedItem;
                if (library != null)
                {
                    Libraries libraries = new();
                    libraries.RemoveLibraryAndChangeIdTo0(library);
                }
                LoadLibrariesData();
            }
        }

        private void City_TextChanged(object sender, TextChangedEventArgs e)
        {
            City.Text = loginMethod.EraseWhiteSpace(City.Text);
        }

        private void Street_TextChanged(object sender, TextChangedEventArgs e)
        {
            Street.Text = loginMethod.EraseWhiteSpace(Street.Text);
        }

        private void Number_TextChanged(object sender, TextChangedEventArgs e)
        {
            Number.Text = loginMethod.EraseWhiteSpace(Number.Text);
        }

        private void IdLibraryLabel_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(IdLibraryLabel.Text, "[^0-9]"))
            {
                MessageBox.Show("Proszę wpisać numer.");
                IdLibraryLabel.Text = IdLibraryLabel.Text.Remove(IdLibraryLabel.Text.Length - 1);
            }
        }

        private void GenerateRaport(object sender, RoutedEventArgs e)
        {
            string dateFrom = "";
            if (startDatePicker.SelectedDate == null) dateFrom = "poczatku";
            else dateFrom = startDatePicker.SelectedDate.ToString()!;
            string dateTo = "";
            if (endDatePicker.SelectedDate == null) dateTo = DateTime.Now.ToString("dd-MM-yyyy");
            else dateTo = endDatePicker.SelectedDate.ToString()!;

            string currentDateTime = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string fileName = "raport-adminSieci" + "-" + networkAdmin.UserName + "-" + currentDateTime + ".pdf";
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            path += @"\Raporty\";
            string path1 = path + fileName;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            iTextSharp.text.Document doc = new iTextSharp.text.Document();
            PdfWriter.GetInstance(doc, new FileStream(path1, FileMode.Create));
            // ilosc zarejestrowanych klientow
            //AccountBase c = new();
            //int AllClients = c.GetClientList().Count;
            Libraries libraries = new();
            List<Library> list = libraries.GetListOfLibraries();
            int numberOfRentalsAll = loginMethod.GetNumberRenalsAll(startDatePicker.SelectedDate, endDatePicker.SelectedDate);
            int numberOfEventsAll = loginMethod.GetNumberOfEventsAll(startDatePicker.SelectedDate, endDatePicker.SelectedDate);
            int numberOfActiveUsers = loginMethod.GetNumberOfActiveUsersAll(startDatePicker.SelectedDate, endDatePicker.SelectedDate);
            Books books = new();
            List<Book> listOfBooks = books.GetBooksList();
            List<Librarian> listOfLibrarians = accountModel.GetLibrarianList();

            doc.Open();
            doc.Add(new iTextSharp.text.Paragraph("Autor: " + networkAdmin.FirstName + " " + networkAdmin.LastName));
            doc.Add(new iTextSharp.text.Paragraph("Data utworzenia: " + DateTime.Now.ToString("yyyy-MM-dd")));
            doc.Add(new iTextSharp.text.Paragraph("Zakres raportu od: " + dateFrom));
            doc.Add(new iTextSharp.text.Paragraph("Zakres raportu do: " + dateTo));
            doc.Add(new iTextSharp.text.Paragraph("Bibliotek: " + list.Count));
            doc.Add(new iTextSharp.text.Paragraph("Wypozyczen z wszystkich bibliotek: " + numberOfRentalsAll));
            doc.Add(new iTextSharp.text.Paragraph("Wieczorkow autorskich: " + numberOfEventsAll));
            doc.Add(new iTextSharp.text.Paragraph("Aktywnych uzytkownikow: " + numberOfActiveUsers));
            doc.Add(new iTextSharp.text.Paragraph("Ksiazek na dzien dzisiejszy: " + listOfBooks.Count));
            doc.Add(new iTextSharp.text.Paragraph("Zatrudnionych bibliotekarzy na dzien dzisiejszy: " + listOfLibrarians.Count));

            for (int i = 0; i < list.Count; i++)
            {
                int numberOfRentals = loginMethod.GetNumberOfRentals(i, startDatePicker.SelectedDate, endDatePicker.SelectedDate);
                int numberOfBooksInLibrary = loginMethod.GetNumberOfBooksInLibrary(i);
                int numberOfLibrarians = loginMethod.GetNumberOfLibrarians(i);
                int numberOfEvents = loginMethod.GetNumberOfEvents(i, startDatePicker.SelectedDate, endDatePicker.SelectedDate);
                int activeUsers = loginMethod.GetNumberOfActiveUsers(i, startDatePicker.SelectedDate, endDatePicker.SelectedDate);

                doc.Add(new iTextSharp.text.Paragraph("\n"));
                doc.Add(new iTextSharp.text.Paragraph("\n"));
                doc.Add(new iTextSharp.text.Paragraph("\n"));
                doc.Add(new iTextSharp.text.Paragraph("Biblioteka o ID: " + list[i].ID));
                doc.Add(new iTextSharp.text.Paragraph("\n"));
                doc.Add(new iTextSharp.text.Paragraph("Wypozyczen z biblioteki: " + numberOfRentals));
                doc.Add(new iTextSharp.text.Paragraph("Wieczorkow autorskich: " + numberOfEvents));
                doc.Add(new iTextSharp.text.Paragraph("Aktywnych uzytkownikow: " + activeUsers));
                doc.Add(new iTextSharp.text.Paragraph("Ksiazek w bibliotece na dzien dzisiejszy: " + numberOfBooksInLibrary));
                doc.Add(new iTextSharp.text.Paragraph("Zatrudnionych bibliotekarzy w bibliotece na dzien dzisiejszy: " + numberOfLibrarians));
            }

            doc.Close();
            MessageBox.Show("Utworzono raport w folderze raporty na pulpicie.");
        }


    }
}
