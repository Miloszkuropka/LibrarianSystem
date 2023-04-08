using System;
using System.Collections.Generic;
using System.Windows;

namespace SystemBiblioteczny.Models
{
    public class Libraries
    {
        AccountBase accountModel = new();
        private List<Library> GetLibrariesList()
        {

            List<Library> list = new();

            List<string> lines = accountModel.GetListOfDataBaseLines("Libraries");

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];

                string[] splitted = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                int newId = int.Parse(splitted[0]);
                string newCity = splitted[1];
                string newStreet = splitted[2];
                string newLocal = splitted[3];

                Library lib = new(newId, newCity, newStreet, newLocal);

                list.Add(lib);

            }

            return list;

        }

        public void AddLibraryToDB(Library library)
        {
            List<string> lines = accountModel.GetListOfDataBaseLines("Libraries");
            accountModel.WriteToDataBase("Libraries", library?.ID.ToString() + " " + library?.City + " " + library?.Street + " " + library?.Local);
        }

        public int ReturnUniqueID()
        {
            List<Library> list = this.GetLibrariesList();
            int max = 0;
            foreach (Library l in list)
            {
                if (l.ID > max) max = l.ID;
            }
            return max + 1;
        }

        public bool CheckIfCanAdd(string city, string street, string local)
        {
            List<Library> list = this.GetLibrariesList();
            foreach (Library l in list)
            {
                if (l.City == city && l.Street == street && l.Local == local)
                {
                    MessageBox.Show("Biblioteka o takich danych już istnieje!");
                    return false;
                }
            }
            if (city.Length < 3)
            {
                MessageBox.Show("Miasto musi mieć przynajmniej 3 znaki!");
                return false;
            }
            if (street.Length < 3)
            {
                MessageBox.Show("Ulica musi mieć przynajmniej 3 znaki!");
                return false;
            }
            if (local.Length < 1)
            {
                MessageBox.Show("Numer lokalu nie może być pusty!");
                return false;
            }
            return true;
        }

        internal List<Library> GetListOfLibraries()
        {
            AccountBase a = new();
            List<string> listOfString = a.GetListOfDataBaseLines("Libraries");
            List<Library> list = new();
            for (int i = 0; i < listOfString.Count; i++)
            {
                string line = listOfString[i];

                string[] splitted = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                int id = int.Parse(splitted[0]);
                string city = splitted[1];
                string street = splitted[2];
                string local = splitted[3];

                Library tmp = new(id, city, street, local);

                list.Add(tmp);

            }
            return list;
        }

        internal void RemoveLibraryAndChangeIdTo0(Library library)
        {
            AccountBase a = new();
            List<string> listOfString = a.GetListOfDataBaseLines("Libraries");
            a.WriteDataBase("Libraries", listOfString);
            for (int i = 0; i < listOfString.Count; i++)
            {
                string line = listOfString[i];

                string[] splitted = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                int id = int.Parse(splitted[0]);
                string city = splitted[1];
                string street = splitted[2];
                string local = splitted[3];
                if (id == library.ID)
                {
                    listOfString.Remove(line);
                }
                else
                    listOfString[i] = id + " " + city + "  " + street + " " + local;
            }
            a.WriteDataBase("Libraries", listOfString);
            listOfString = a.GetListOfDataBaseLines("LocalAdminList");
            for (int i = 0; i < listOfString.Count; i++)
            {
                string line = listOfString[i];
                string[] splitted = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                string username = splitted[0];
                string password = splitted[1];
                int newIdLibrary = int.Parse(splitted[2]);

                if (newIdLibrary == library.ID) newIdLibrary = 0;
                listOfString[i] = username + " " + password + "  " + newIdLibrary;
            }
            a.WriteDataBase("LocalAdminList", listOfString);
            listOfString = a.GetListOfDataBaseLines("BookList");
            for (int i = 0; i < listOfString.Count; i++)
            {
                string line = listOfString[i];
                string[] splitted = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                int newId = int.Parse(splitted[0]);
                string newAuthor = splitted[1];
                string newTitle = splitted[2];
                bool newAvailibility = bool.Parse(splitted[3]);
                int newIdLibrary = int.Parse(splitted[4]);
                if (newIdLibrary == library.ID) newIdLibrary = 0;
                listOfString[i] = newId + " " + newAuthor + "  " + newTitle + " " + newAvailibility + " " + newIdLibrary;
            }
            a.WriteDataBase("BookList", listOfString);
            listOfString = a.GetListOfDataBaseLines("AuthorsEveningList");
            for (int i = 0; i < listOfString.Count; i++)
            {
                string line = listOfString[i];
                string[] splitted = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                bool approved = bool.Parse(splitted[0]);
                string username = splitted[1];
                string authorsName = splitted[2];
                string authorsLastname = splitted[3];
                int libraryID = int.Parse(splitted[4]);
                string newDate = splitted[5];
                int newHour = int.Parse(splitted[6]);
                string newPhoneNumber = splitted[7];
                if (libraryID == library.ID)
                {
                    listOfString.Remove(line);
                }
                else
                    listOfString[i] = approved + " " + username + "  " + authorsName + " " + authorsLastname + " " + libraryID + " " + newDate + " " + newHour + " " + newPhoneNumber;
            }
            a.WriteDataBase("AuthorsEveningList", listOfString);
            listOfString = a.GetListOfDataBaseLines("LibrarianList");
            for (int i = 0; i < listOfString.Count; i++)
            {
                string line = listOfString[i];
                string[] splitted = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                string username = splitted[0];
                string password = splitted[1];
                int newIdLibrary = int.Parse(splitted[2]);
                if (newIdLibrary == library.ID) newIdLibrary = 0;
                listOfString[i] = username + " " + password + "  " + newIdLibrary;
            }
            a.WriteDataBase("LibrarianList", listOfString);
            listOfString = a.GetListOfDataBaseLines("ExchangeBookList");
            for (int i = 0; i < listOfString.Count; i++)
            {
                string line = listOfString[i];
                string[] splitted = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                int exchangeId = int.Parse(splitted[0]);
                int bookId = int.Parse(splitted[1]);
                string newRequestor = splitted[2];
                string newAuthor = splitted[3];
                string newTitle = splitted[4];
                int newIdLibrary = int.Parse(splitted[5]);
                if (newIdLibrary == library.ID) newIdLibrary = 0;
                listOfString[i] = exchangeId + " " + bookId + "  " + newRequestor + " " + newAuthor + "  " + newTitle + " " + newIdLibrary;
            }
            a.WriteDataBase("ExchangeBookList", listOfString);
        }
    }
}

