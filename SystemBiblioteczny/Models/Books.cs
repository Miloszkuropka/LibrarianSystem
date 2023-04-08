using System;
using System.Collections.Generic;
using SystemBiblioteczny.Models;

namespace SystemBiblioteczny
{
    public class Books
    {
        AccountBase accountModel = new();
        public List<Book> GetBooksList()
        {
            List<Book> list = new();
            List<string> lines = accountModel.GetListOfDataBaseLines("BookList");

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                string[] splitted = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                int newId = int.Parse(splitted[0]);
                string newAuthor = splitted[1];
                string newTitle = splitted[2];
                bool newAvailibility = bool.Parse(splitted[3]);
                int newIdLibrary = int.Parse(splitted[4]);

                Book book = new(newId, newAuthor, newTitle, newAvailibility, newIdLibrary);

                list.Add(book);

            }
            return list;
        }

    }
}