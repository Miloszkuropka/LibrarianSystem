using System;
using System.Collections.Generic;

namespace SystemBiblioteczny.Models
{
    public class BooksHistory
    {
        private AccountBase account = new();

        public List<BookHistory> GetHistoredBooksList()
        {
            List<BookHistory> list = new();

            List<string> lines = account.GetListOfDataBaseLines("BookHistory");

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];

                string[] splitted = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                int newId = int.Parse(splitted[0]);
                int newLibraryId = int.Parse(splitted[1]);
                string newAuthor = splitted[2];
                string newTitle = splitted[3];
                string newUser = splitted[4];
                string newDate = splitted[5];
                string newReturnDate = splitted[6];

                BookHistory book = new(newId, newLibraryId, newAuthor, newTitle, newUser, newDate, newReturnDate);

                list.Add(book);

            }
            return list;
        }
    }
}
