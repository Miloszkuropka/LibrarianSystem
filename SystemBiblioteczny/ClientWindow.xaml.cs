using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using SystemBiblioteczny.Methods;
using SystemBiblioteczny.Models;

namespace SystemBiblioteczny
{
    public partial class ClientWindow : Window
    {
        private Client loggedUser = new();
        private int bookFromGui = -1;
        private LoginMethod loginMethod = new();
        private AccountBase account = new();
        public ClientWindow(Client user)
        {
            InitializeComponent();
            base.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            loggedUser = user;

            PersonStatistics(user.UserName!);
            UptodateTable();
            EmailBox.Text = loggedUser.Email;
            PhoneBox.Text = loggedUser.Phone;
            Date.FontSize = 10;
            LoadEventData();
        }

        private void Return(object sender, RoutedEventArgs e)
        {
            MainWindow m = new();
            m.Show();
            this.Close();
        }

        private void Register_Evening(object sender, RoutedEventArgs e)
        {
            String name = AuthorsName.Text;
            String lastname = AuthorsLastname.Text;
            int libraryID;
            if (!int.TryParse(LibraryID.Text, out libraryID))
            {
                MessageBox.Show("Numer biblioteki podaj liczbą!");
                return;
            }
            DateTime? date = Date.SelectedDate;
            int hour;
            if (!int.TryParse(EventTime.Text, out hour))
            {
                MessageBox.Show("Podaj pełną godzinę od 8 do 22 liczbą!");
                return;
            }
            String phoneNumber = ContactNumber.Text;

            AuthorsEvening newAuthorsEvening = new(false, loggedUser.UserName!, name, lastname, libraryID, date, hour, phoneNumber);
            if (newAuthorsEvening.TryAddToDataBase())
            {
                MessageBox.Show("Pomyślnie wysłano propozycję wieczorka autorskiego!");
            }

            LoadEventData();
        }


        private void OptAvailability_Checked(object sender, RoutedEventArgs e)
        {
            Books books = new();
            List<Book> listofBooks = new();
            foreach (Book b in books.GetBooksList()) listofBooks.Add(new(b));
            TableBooks.Items.Clear();
            if (OptAvailability.IsChecked == true)
            {
                foreach (var i in listofBooks)
                {
                    if (i.Availability == true) TableBooks.Items.Add(i);
                }
            }
        }

        private void OptAll_Checked(object sender, RoutedEventArgs e)
        {
            Books books = new();
            List<Book> listofBooks = new();
            foreach (Book b in books.GetBooksList()) listofBooks.Add(new(b));
            TableBooks.Items.Clear();
            if (OptAll.IsChecked == true)
            {
                var sort1 = listofBooks.OrderBy(x => x.Id_Book).ToList();
                sort1.ForEach(x =>
                {
                    TableBooks.Items.Add(x);
                });
            }
        }

        private void Find_TextChanged(object sender, TextChangedEventArgs e)
        { }

        private void Find(object sender, RoutedEventArgs e)
        {
            Books books = new();
            List<Book> listofBooks = books.GetBooksList();
            bool info = false;
            Find1.Text = Find1.Text.Replace(" ", "_");
            for (int i = 0; i < listofBooks.Count; i++)
            {
                if (listofBooks[i].Author == Find1.Text)
                {
                    TableBooks.Items.Clear();
                    info = true;
                    var find = listofBooks.Where(x => x.Author == Find1.Text).ToList();
                    find.ForEach(x =>
                    {
                        string newTitle = "";
                        string newAuthor = "";
                        string[] splittedTitle = x.Title.Split("_");
                        for (int i = 0; i < splittedTitle.Length; i++)
                        {
                            newTitle = newTitle + splittedTitle[i] + " ";
                        }
                        string[] splittedAuthor = x.Author.Split("_");
                        for (int i = 0; i < splittedAuthor.Length; i++)
                        {
                            newAuthor = newAuthor + splittedAuthor[i] + " ";
                        }
                        x.Author = newAuthor;
                        x.Title = newTitle;
                        TableBooks.Items.Add(x);

                    });
                }
                if (listofBooks[i].Title == Find1.Text)
                {
                    TableBooks.Items.Clear();
                    info = true;
                    var find1 = listofBooks.Where(x => x.Title == Find1.Text).ToList();
                    find1.ForEach(x =>
                    {
                        string newTitle = "";
                        string newAuthor = "";
                        string[] splittedTitle = x.Title.Split("_");
                        for (int i = 0; i < splittedTitle.Length; i++)
                        {
                            newTitle = newTitle + splittedTitle[i] + " ";
                        }
                        string[] splittedAuthor = x.Author.Split("_");
                        for (int i = 0; i < splittedAuthor.Length; i++)
                        {
                            newAuthor = newAuthor + splittedAuthor[i] + " ";
                        }
                        x.Author = newAuthor;
                        x.Title = newTitle;
                        TableBooks.Items.Add(x);
                    });
                }
            }
            Find1.Text = Find1.Text.Replace("_", " ");
            if (info == false) { MessageBox.Show("Nie istnieje taki autor bądź tytuł w bazie danych"); }
        }

        private void TableBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Book book = (Book)TableBooks.SelectedItem;
            if (book != null)
            {
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

        private void RefreshTextBoxes()
        {
            if (RequestBookLabel.Text == "") { RequestBookLabel.Text = "0"; }
            bookFromGui = int.Parse(RequestBookLabel.Text); ;
        }
        private void ChangePassword(object sender, RoutedEventArgs e)
        {
            if (PasswordBox1.Password.CompareTo(PasswordBox2.Password) == 0)
            {
                if (PasswordBox2.Password.Length < 4) MessageBox.Show("Hasło musi mieć przynajmiej 4 znaki");
                else account.ChangePersonData(loggedUser, AccountBase.RoleTypeEnum.Client, PasswordBox1.Password);
            }
            else MessageBox.Show("Podane hasła różnią się od siebie");
        }

        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EmailBox.Text.CompareTo(loggedUser.Email) != 0)
                {
                    MailAddress mail = new MailAddress(EmailBox.Text);
                    account.ChangePersonData(loggedUser, AccountBase.RoleTypeEnum.Client, "", EmailBox.Text);
                }
                if (PhoneBox.Text.CompareTo(loggedUser.Phone!.ToString()) != 0)
                {
                    if (PhoneBox.Text.CompareTo("") == 0) MessageBox.Show("Nie podano poprawnego numeru telefonu");
                    else account.ChangePersonData(loggedUser, AccountBase.RoleTypeEnum.Client, "", "", PhoneBox.Text);
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
        private void Book(object sender, RoutedEventArgs e)
        {
            RefreshTextBoxes();
            BookReserved bookRe = new();
            BooksReserved booksR = new();

            string czas = DateTime.Now.ToString("dd/MM/yyyy");

            //Book book = new();
            // book = (Book)TableBooks.SelectedItem;
            Books books = new();
            List<Book> listofBooks = books.GetBooksList();
            List<Book> listofBorrowedBooks = books.GetBooksList();
            BookReserved bookBorrowed = new();
            bool info = false;

            bool status1 = false;

            for (int i = 0; i < listofBooks.Count; i++)
            {
                int idSelected = listofBooks[i].Id_Book;

                if (idSelected.CompareTo(bookFromGui) == 0)
                {
                    if (listofBooks[i].Availability == true)
                    {
                        info = true;
                        int idBookFromGui = bookFromGui;
                        string path2 = System.IO.Path.Combine("../../../DataBases/BookList.txt");
                        using (StreamWriter writer = new StreamWriter(path2))
                        {
                            for (int k = 0; k < listofBooks.Count; k++)
                            {
                                if (idBookFromGui == listofBooks[k].Id_Book) writer.WriteLine(listofBooks[k].Id_Book + " " + listofBooks[k].Author + " " + listofBooks[k].Title + " " + "False" + " " + listofBooks[k].Id_Library);
                                else writer.WriteLine(listofBooks[k].Id_Book + " " + listofBooks[k].Author + " " + listofBooks[k].Title + " " + listofBooks[k].Availability + " " + listofBooks[k].Id_Library);
                            }
                            writer.Close();
                        }
                        bookBorrowed.Id_Book = listofBooks[i].Id_Book;
                        bookBorrowed.Author = listofBooks[i].Author;
                        bookBorrowed.Title = listofBooks[i].Title;
                        bookBorrowed.Id_Library = listofBooks[i].Id_Library;
                        bookBorrowed.DateTime1 = czas;
                        bookBorrowed.UserName = loggedUser.UserName!;

                        listofBorrowedBooks.Add(bookBorrowed);
                        booksR.SaveReservedBooks(bookBorrowed);

                        status1 = bookRe.AccountBalance(bookBorrowed.DateTime1);

                        //if (status1 == false) statusBook.Text = "Nie ma zaległych książek";
                        //else statusBook.Text = "Trzeba zapłacić za zaległą książkę!";

                        MessageBox.Show("Zarezerwowano ksiązkę!");
                        UptodateTable();
                    }
                    else { MessageBox.Show("Ta ksiązka jest niedostępna!"); info = true; }
                }
            }
            if (info == false) { MessageBox.Show("Nie istnieje książka o podanym id"); }
        }

        private void UptodateTable()
        {
            TableBooks.Items.Clear();
            Books books = new();
            List<Book> listofBooks = new();
            foreach (Book b in books.GetBooksList()) listofBooks.Add(new(b));
            var listOfBooks1 = listofBooks.Where(x => x.Id_Library != 0).ToList();
            listOfBooks1.ForEach(x =>
            {
                TableBooks.Items.Add(x);
            });

            TableBooks1.Items.Clear();
            BooksReserved booksR = new();
            List<BookReserved> listofBooksR = new();
            foreach (BookReserved b in booksR.GetReservedBooksList()) listofBooksR.Add(new(b));
            var listofBooksRAccepted = listofBooksR.Where(x => x.Availability == true && loggedUser.UserName == x.UserName).ToList();
            listofBooksRAccepted.ForEach(x =>
            {
                TableBooks1.Items.Add(x);
            });
        }

        private void LoadEventData()
        {
            AuthorsEvenings.Items.Clear();
            AuthorsEvenings events = new();
            List<AuthorsEvening> listOfEvents = events.GetEventList();
            foreach (AuthorsEvening e in listOfEvents)
            {
                if (e.User == loggedUser.UserName)
                    AuthorsEvenings.Items.Add(e);
            }
        }

        private void WithDraw(object sender, RoutedEventArgs e)
        {
            AuthorsEvening evening = new();
            evening = (AuthorsEvening)AuthorsEvenings.SelectedItem;
            AuthorsEvenings evenings = new();
            List<AuthorsEvening> list = evenings.GetEventList();
            if(evening != null) evenings.RemoveFromList(evening);
            LoadEventData();
        }

        private void PersonStatistics(string name)
        {
            userNameClient.Content = name;
            BooksHistory bookH = new();
            List<BookHistory> list = new();
            foreach (BookHistory b in bookH.GetHistoredBooksList()) list.Add(new(b));
            List<BookHistory> listHistoryBook = list.Where(x => x.UserName == name).ToList();
            listHistoryBook.ForEach(x =>
            {
                TableHistoryBooks.Items.Add(x);
            });

        }

        private void AuthorsName_TextChanged(object sender, TextChangedEventArgs e)
        {
            AuthorsName.Text = loginMethod.EraseWhiteSpace(AuthorsName.Text);
        }

        private void AuthorsLastname_TextChanged(object sender, TextChangedEventArgs e)
        {
            AuthorsName.Text = loginMethod.EraseWhiteSpace(AuthorsName.Text);
        }

        private void LibraryID_TextChanged(object sender, TextChangedEventArgs e)
        {
            AuthorsName.Text = loginMethod.EraseWhiteSpace(AuthorsName.Text);
        }

        private void EventTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            AuthorsName.Text = loginMethod.EraseWhiteSpace(AuthorsName.Text);
        }

        private void ContactNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            AuthorsName.Text = loginMethod.EraseWhiteSpace(AuthorsName.Text);
        }

    }
}