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
    /// Logika interakcji dla klasy Admin_localWindow.xaml
    /// </summary>
    public partial class Admin_LocalWindow : Window
    {
        private LoginMethod loginMethod = new();
        private ApplicationBook applicationBookModel = new();
        private BookExchange bookExchangeModel = new();
        private AccountBase accountModel = new();
        private Books bookModel = new();
        private AuthorsEvening eveningModel = new();
        private LocalAdmin localAdmin = new();
        private AuthorsEvenings eveningsModel = new();
        private int bookFromGui = -1;
        private int exchangeFromGui = -1;
        public Admin_LocalWindow(LocalAdmin userData)
        {

            InitializeComponent();

            base.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            localAdmin = userData;
            EmailBox.Text = localAdmin.Email;
            PhoneBox.Text = localAdmin.Phone;
            nazwaLabel.Content = localAdmin.UserName;
            numerLabel.Content = localAdmin.LibraryId;
            LoadEventData();
            RefreshTableData();
            RefreshTableApplicationsData();
        }
        private void Return(object sender, RoutedEventArgs e)
        {
            MainWindow m = new();
            m.Show();
            this.Close();
        }

        private void SendBook_Click(object sender, RoutedEventArgs e)
        {
            RefreshTextBoxes();

            List<BookExchange> listofBooks = bookExchangeModel.GetExchangeBooksList();
            bool info = false;

            for (int i = 0; i < listofBooks.Count; i++)
            {
                int idExchange = listofBooks[i].ExchangeId;

                if (idExchange.CompareTo(exchangeFromGui) == 0)
                {
                    info = true;
                    if (localAdmin.LibraryId.CompareTo(listofBooks[i].Id_Library) != 0) MessageBox.Show("Nie możesz wysłać książki należącej do innej biblioteki");
                    else
                    {
                        int bookId = SendBookIfAvaliable();
                        List<Book> lines = bookModel.GetBooksList();
                        string path = System.IO.Path.Combine("../../../DataBases/BookList.txt");
                        using (StreamWriter writer = new StreamWriter(path))
                        {
                            for (int k = 0; k < lines.Count; k++)
                            {
                                int bookIdFromList = lines[k].Id_Book;
                                int newLibraryId = getRequestorLibraryId(listofBooks[i].RequestorUsername);
                                if (bookId.CompareTo(bookIdFromList) == 0) writer.WriteLine(lines[k].Id_Book + " " + lines[k].Author + " " + lines[k].Title + " " + "True" + " " + newLibraryId);
                                else writer.WriteLine(lines[k].Id_Book + " " + lines[k].Author + " " + lines[k].Title + " " + lines[k].Availability + " " + lines[k].Id_Library);
                            }
                            writer.Close();
                        }
                        MessageBox.Show("Wysłano książke");
                        RefreshTableData();
                    }
                }
            }
            if (info == false) MessageBox.Show("Nie istnieje zlecenie o podanym id");

        }
        private int getRequestorLibraryId(string requestorUsername)
        {
            int result = 0;
            List<LocalAdmin> list = accountModel.GetLocalAdminList();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].UserName!.CompareTo(requestorUsername) == 0) result = list[i].LibraryId;
            }
            return result;
        }
        private int SendBookIfAvaliable()
        {
            int resultBookId = 0;
            string path = System.IO.Path.Combine("../../../DataBases/ExchangeBookList.txt");
            List<string> lines = accountModel.GetListOfDataBaseLines("ExchangeBookList");

            using (StreamWriter writer = new StreamWriter(path))
            {
                for (int j = 0; j < lines.Count; j++)
                {

                    string line = lines[j];
                    string[] splitted = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                    int newId = int.Parse(splitted[0]);
                    int echangeId = int.Parse(splitted[0]);
                    int bookId = int.Parse(splitted[1]);
                    string newRequestor = splitted[2];
                    string newAuthor = splitted[3];
                    string newTitle = splitted[4];
                    int newIdLibrary = int.Parse(splitted[5]);


                    if (newId.CompareTo(exchangeFromGui) == 0) resultBookId = bookId;
                    else if (echangeId > exchangeFromGui) { writer.WriteLine((echangeId - 1) + " " + bookId + " " + newRequestor + " " + newAuthor + " " + newTitle + " " + newIdLibrary); }
                    else writer.WriteLine(line);
                }

                writer.Close();
            }

            return resultBookId;
        }

        private void RequestForABook(object sender, RoutedEventArgs e)
        {
            RefreshTextBoxes();
            bool info = false;
            List<Book> listofBooks = bookModel.GetBooksList();

            for (int i = 0; i < listofBooks.Count; i++)
            {
                int idBookFromList = listofBooks[i].Id_Book;

                if (idBookFromList.CompareTo(bookFromGui) == 0)
                {
                    info = true;
                    if (localAdmin.LibraryId.CompareTo(listofBooks[i].Id_Library) == 0) MessageBox.Show("Możesz wysyłać prośby o książki tylko z innych bibliotek");
                    else
                    {
                        if (listofBooks[i].Availability == false) MessageBox.Show("Możesz wysyłać prośby dotyczące tylko dostępnych książek");
                        else
                        {
                            List<string> lines = accountModel.GetListOfDataBaseLines("ExchangeBookList");

                            int echangeId = lines.Count + 1;
                            int bnewBookId = listofBooks[i].Id_Book;
                            string newRequestor = localAdmin.UserName!;
                            string newAuthor = listofBooks[i].Author;
                            string newTitle = listofBooks[i].Title;
                            int newIdLibrary = listofBooks[i].Id_Library;

                            accountModel.WriteToDataBase("ExchangeBookList", echangeId + " " + bnewBookId + " " + newRequestor + " " + newAuthor + " " + newTitle + " " + newIdLibrary);
                            SendBookRequestIfAvaliable();
                            MessageBox.Show("Wysłano prośbę");

                        }

                    }
                }

            }
            if (info == false) MessageBox.Show("Nie istnieje zlecenie o podanym id");
        }
        private void SendBookRequestIfAvaliable()
        {

            List<Book> listofBooks = bookModel.GetBooksList();
            string path2 = System.IO.Path.Combine("../../../DataBases/BookList.txt");
            using (StreamWriter writer = new StreamWriter(path2))
            {
                for (int k = 0; k < listofBooks.Count; k++)
                {
                    if (bookFromGui == listofBooks[k].Id_Book) writer.WriteLine(listofBooks[k].Id_Book + " " + listofBooks[k].Author + " " + listofBooks[k].Title + " " + "False" + " " + listofBooks[k].Id_Library);
                    else writer.WriteLine(listofBooks[k].Id_Book + " " + listofBooks[k].Author + " " + listofBooks[k].Title + " " + listofBooks[k].Availability + " " + listofBooks[k].Id_Library);
                }
                writer.Close();
            }
            RefreshTableData();
        }
        void RefreshTableData()
        {
            List<BookExchange> listofExchangeBooks = new();
            foreach (BookExchange b in bookExchangeModel.GetExchangeBooksList()) listofExchangeBooks.Add(new(b));
            List<Book> listofBooks = new();
            foreach (Book b in bookModel.GetBooksList()) listofBooks.Add(new(b));
            TableExchangeBooks.Items.Clear();
            TableBooks.Items.Clear();

            foreach (BookExchange bookExchange in listofExchangeBooks)
            {
                TableExchangeBooks.Items.Add(bookExchange);
            }

            foreach (Book book in listofBooks)
            {
                TableBooks.Items.Add(book);
            }

            TableExchangeBooks.IsReadOnly = true;
            TableBooks.IsReadOnly = true;
        }

        void RefreshTableApplicationsData()
        {
            List<ApplicationBook> listofApplicationBooks = new();
            foreach (ApplicationBook b in applicationBookModel.GetApplicationBooksList()) listofApplicationBooks.Add(new(b));
            int libID = 0;
            NewApplicationsData.Items.Clear();
            for (int i = 0; i < listofApplicationBooks.Count; i++)
            {
                ApplicationBook book = listofApplicationBooks[i];
                if (book.Approved.CompareTo(false) == 0)
                {
                    List<Librarian> librarians = accountModel.GetLibrarianList();
                    for (int j = 0; j < librarians.Count; j++)
                    {
                        if (librarians[j].UserName!.CompareTo(listofApplicationBooks[i].Librarian) == 0) libID = librarians[j].LibraryId;
                    }

                    if (libID.CompareTo(localAdmin.LibraryId) == 0) NewApplicationsData.Items.Add(book);
                }

            }

            NewApplicationsData.IsReadOnly = true;

        }

        private void CancelRequestButton(object sender, RoutedEventArgs e)
        {
            RefreshTextBoxes();

            List<BookExchange> listofBooks = bookExchangeModel.GetExchangeBooksList();
            bool info = false;

            for (int i = 0; i < listofBooks.Count; i++)
            {
                int idExchange = listofBooks[i].ExchangeId;

                if (idExchange.CompareTo(exchangeFromGui) == 0)
                {
                    info = true;
                    if (localAdmin.LibraryId.CompareTo(listofBooks[i].Id_Library) != 0) MessageBox.Show("Możesz odrzucać prośby wysłane tylko do swojej biblioteki");
                    else
                    {
                        int BookIdToDelete = -1;
                        string path = System.IO.Path.Combine("../../../DataBases/BookList.txt");
                        BookIdToDelete = RejectBookIfAvaliable(BookIdToDelete);
                        List<Book> list = bookModel.GetBooksList();
                        using (StreamWriter writer = new StreamWriter(path))
                        {
                            for (int k = 0; k < list.Count; k++)
                            {

                                if (BookIdToDelete == list[k].Id_Book) writer.WriteLine(list[k].Id_Book + " " + list[k].Author + " " + list[k].Title + " " + "True" + " " + list[k].Id_Library);
                                else writer.WriteLine(list[k].Id_Book + " " + list[k].Author + " " + list[k].Title + " " + list[k].Availability + " " + list[k].Id_Library);
                            }
                            writer.Close();
                        }
                        MessageBox.Show("Odrzucono prośbę");
                        RefreshTableData();
                    }
                }
            }
            if (info == false) { MessageBox.Show("Nie istnieje zlecenie o podanym id"); }
        }
        private int RejectBookIfAvaliable(int BookIdToDelete)
        {

            string path = System.IO.Path.Combine("../../../DataBases/ExchangeBookList.txt");
            List<string> lines = accountModel.GetListOfDataBaseLines("ExchangeBookList");

            using (StreamWriter writer = new StreamWriter(path))
            {
                for (int j = 0; j < lines.Count; j++)
                {
                    string line = lines[j];
                    string[] splitted = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                    int newId = int.Parse(splitted[0]);
                    int echangeId = int.Parse(splitted[0]);
                    int bookId = int.Parse(splitted[1]);
                    string newRequestor = splitted[2];
                    string newAuthor = splitted[3];
                    string newTitle = splitted[4];
                    int newIdLibrary = int.Parse(splitted[5]);


                    if (newId.CompareTo(exchangeFromGui) == 0) { BookIdToDelete = bookId; }
                    else if (echangeId > exchangeFromGui) { writer.WriteLine((echangeId - 1) + " " + bookId + " " + newRequestor + " " + newAuthor + " " + newTitle + " " + newIdLibrary); }
                    else writer.WriteLine(line);
                }

                writer.Close();
            }
            return BookIdToDelete;
        }
        private void TableExchangeBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BookExchange book = (BookExchange)TableExchangeBooks.SelectedItem;
            if (book != null)
            {
                SendBookLabel.Text = book.ExchangeId.ToString();
                RequestBookLabel.Text = book.Id_Book.ToString();
            }
        }
        private void ChangePassword(object sender, RoutedEventArgs e)
        {
            if (PasswordBox1.Password.CompareTo(PasswordBox2.Password) == 0)
            {
                if (PasswordBox2.Password.Length < 4) MessageBox.Show("Hasło musi mieć przynajmiej 4 znaki");
                else accountModel.ChangePersonData(localAdmin, AccountBase.RoleTypeEnum.LocalAdmin, PasswordBox1.Password, "", "", localAdmin.LibraryId);
            }
            else MessageBox.Show("Podane hasła różnią się od siebie");
        }

        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EmailBox.Text.CompareTo(localAdmin.Email) != 0)
                {
                    MailAddress mail = new MailAddress(EmailBox.Text);
                    accountModel.ChangePersonData(localAdmin, AccountBase.RoleTypeEnum.LocalAdmin, "", EmailBox.Text, "", localAdmin.LibraryId);
                }
                if (PhoneBox.Text.CompareTo(localAdmin.Phone!.ToString()) != 0)
                {
                    if (PhoneBox.Text.CompareTo("") == 0) MessageBox.Show("Nie podano poprawnego numeru telefonu");
                    else accountModel.ChangePersonData(localAdmin, AccountBase.RoleTypeEnum.LocalAdmin, "", "", PhoneBox.Text, localAdmin.LibraryId);
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


        private void OrderBook(object sender, RoutedEventArgs e)
        {
            ApplicationBook IdFromGui = (ApplicationBook)(NewApplicationsData.SelectedItem);

            if (IdFromGui == null) MessageBox.Show("Nie wybrano książki");
            else
            {
                List<ApplicationBook> list = applicationBookModel.GetApplicationBooksList();
                for (int i = 0; i < list.Count; i++)
                {

                    string path = System.IO.Path.Combine("../../../DataBases/BookApplicationList.txt");
                    List<string> lines = accountModel.GetListOfDataBaseLines("BookApplicationList");

                    using (StreamWriter writer = new StreamWriter(path))
                    {
                        for (int j = 0; j < lines.Count; j++)
                        {
                            string line = lines[j];
                            if (list[j].ID.CompareTo(IdFromGui.ID) == 0)
                            {
                                writer.WriteLine(list[j].ID + " " + list[j].Title + " " + list[j].Author + " " + list[j].Quantity + " " + list[j].Librarian + " " + "True");

                            }
                            else writer.WriteLine(line);
                        }

                        writer.Close();
                    }

                }
                RefreshTableApplicationsData();
            }


        }

        private void RejectBook(object sender, RoutedEventArgs e)
        {
            ApplicationBook IdFromGui = (ApplicationBook)(NewApplicationsData.SelectedItem);

            if (IdFromGui == null) MessageBox.Show("Nie wybrano książki");
            else
            {
                List<ApplicationBook> list = applicationBookModel.GetApplicationBooksList();
                for (int i = 0; i < list.Count; i++)
                {

                    string path = System.IO.Path.Combine("../../../DataBases/BookApplicationList.txt");
                    List<string> lines = accountModel.GetListOfDataBaseLines("BookApplicationList");

                    using (StreamWriter writer = new StreamWriter(path))
                    {
                        for (int j = 0; j < lines.Count; j++)
                        {
                            string line = lines[j];
                            if (list[j].ID.CompareTo(IdFromGui.ID) != 0) writer.WriteLine(line);
                        }

                        writer.Close();
                    }

                }
                RefreshTableApplicationsData();
            }
        }



        private void TableBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Book book = (Book)TableBooks.SelectedItem;
            if (book != null)
            {
                SendBookLabel.Text = "0";
                RequestBookLabel.Text = book.Id_Book.ToString();
            }
        }
        private void RequestBookLabel_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(RequestBookLabel.Text, "[^0-9]"))
            {
                MessageBox.Show("Proszę wpisać numer.");
                RequestBookLabel.Text = RequestBookLabel.Text.Remove(RequestBookLabel.Text.Length - 1);
            }
        }

        private void SendBookLabel_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(SendBookLabel.Text, "[^0-9]"))
            {
                MessageBox.Show("Proszę wpisać numer.");
                SendBookLabel.Text = SendBookLabel.Text.Remove(SendBookLabel.Text.Length - 1);
            }
        }

        private void RefreshTextBoxes()
        {
            if (SendBookLabel.Text == "") { SendBookLabel.Text = "0"; }
            if (RequestBookLabel.Text == "") { RequestBookLabel.Text = "0"; }
            bookFromGui = int.Parse(RequestBookLabel.Text);
            exchangeFromGui = int.Parse(SendBookLabel.Text);
        }
        private void LoadEventData()
        {
            AuthorsEvnings.Items.Clear();
            List<AuthorsEvening> listOfEvents = eveningsModel.GetEventList();
            foreach (AuthorsEvening e in listOfEvents)
            {
                if (localAdmin.LibraryId == e.LibraryID)
                    AuthorsEvnings.Items.Add(e);
            }
        }

        private void Approve_button(object sender, RoutedEventArgs e)
        {
            eveningModel = (AuthorsEvening)AuthorsEvnings.SelectedItem;
            if (eveningModel != null) eveningsModel.ChangeApprovedToTrue(eveningModel);
            LoadEventData();
        }

        private void Reject_button(object sender, RoutedEventArgs e)
        {
            eveningModel = (AuthorsEvening)AuthorsEvnings.SelectedItem;
            if (eveningModel != null) eveningsModel.RemoveFromList(eveningModel);
            LoadEventData();
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
                if (l.LibraryId == localAdmin.LibraryId)
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
                        int newLibId = localAdmin.LibraryId;
                        Librarian librarian = new(list[i].UserName!, list[i].Password!, list[i].FirstName!, list[i].LastName!, list[i].Email!, newLibId, list[i].Phone!);
                        accountModel.AddLibrarianToListAndDeleteFromClients(librarian);
                        MessageBox.Show("Nadano uprawnienia");
                        ShowLibrarianListMethod();

                }
            }
            if (info == false) MessageBox.Show("Nie istnieje klient o podanej nazwie");

        }
        private void MakePersonAnClient(object sender, RoutedEventArgs e)
        {
            List<Librarian> librarians = accountModel.GetLibrarianList();
            Client client = new();

            bool info = false;

            for (int i = 0; i < librarians.Count; i++)
            {
                if (librarians[i].UserName!.CompareTo(UserNameTextBox.Text) == 0)
                {
                    info = true;

                    client = new(librarians[i].UserName!, librarians[i].Password!, librarians[i].FirstName!, librarians[i].LastName!, librarians[i].Email!, librarians[i].Phone!);
                }
            }
            if (info == true)
            {
                string path = System.IO.Path.Combine("../../../DataBases/LibrarianList.txt");

                List<string> lines = accountModel.GetListOfDataBaseLines("LibrarianList");

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

        private void GenerateRaport(object sender, RoutedEventArgs e)
        {
            string dateFrom = "";
            if (startDatePicker.SelectedDate == null) dateFrom = "poczatku";
            else dateFrom = startDatePicker.SelectedDate.ToString()!;
            string dateTo = "";
            if (endDatePicker.SelectedDate == null) dateTo = DateTime.Now.ToString("dd-MM-yyyy");
            else dateTo = endDatePicker.SelectedDate.ToString()!;

            int numberOfRentals = loginMethod.GetNumberOfRentals(localAdmin.LibraryId, startDatePicker.SelectedDate, endDatePicker.SelectedDate);
            int numberOfBooksInLibrary = loginMethod.GetNumberOfBooksInLibrary(localAdmin.LibraryId);
            int numberOfLibrarians = loginMethod.GetNumberOfLibrarians(localAdmin.LibraryId);
            int numberOfEvents = loginMethod.GetNumberOfEvents(localAdmin.LibraryId, startDatePicker.SelectedDate, endDatePicker.SelectedDate);
            int activeUsers = loginMethod.GetNumberOfActiveUsers(localAdmin.LibraryId, startDatePicker.SelectedDate, endDatePicker.SelectedDate);

            // ilosc zarejestrowanych klientow
            AccountBase c = new();
            int AllClients = c.GetClientList().Count;

            // ilosc aktywnych klientow w danej bibliotece








            string currentDateTime = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string fileName = "raport-id-" + localAdmin.LibraryId + "-" + localAdmin.UserName + "-" + currentDateTime + ".pdf";
            //string path = @"../../../Raporty/" + fileName;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            path += @"\Raporty\";
            string path1 = path + fileName;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            iTextSharp.text.Document doc = new iTextSharp.text.Document();
            PdfWriter.GetInstance(doc, new FileStream(path1, FileMode.Create));

            doc.Open();
            doc.Add(new iTextSharp.text.Paragraph("Autor: " + localAdmin.FirstName + " " + localAdmin.LastName));
            doc.Add(new iTextSharp.text.Paragraph("Biblioteka: " + localAdmin.LibraryId));
            doc.Add(new iTextSharp.text.Paragraph("Data utworzenia: " + DateTime.Now.ToString("yyyy-MM-dd")));
            doc.Add(new iTextSharp.text.Paragraph("Zakres raportu od: " + dateFrom));
            doc.Add(new iTextSharp.text.Paragraph("Zakres raportu do: " + dateTo));
            doc.Add(new iTextSharp.text.Paragraph("\n"));
            doc.Add(new iTextSharp.text.Paragraph("Wypozyczen z biblioteki: " + numberOfRentals));
            doc.Add(new iTextSharp.text.Paragraph("Wieczorkow autorskich: " + numberOfEvents));
            doc.Add(new iTextSharp.text.Paragraph("Aktywnych uzytkownikow: " + activeUsers));
            doc.Add(new iTextSharp.text.Paragraph("Ksiazek w bibliotece na dzien dzisiejszy: " + numberOfBooksInLibrary));
            doc.Add(new iTextSharp.text.Paragraph("Zatrudnionych bibliotekarzy w bibliotece na dzien dzisiejszy: " + numberOfLibrarians));

            doc.Close();
            MessageBox.Show("Utworzono raport w folderze raporty na pulpicie.");
        }
    }
}
