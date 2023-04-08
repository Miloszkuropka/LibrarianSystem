using System;
using System.Collections.Generic;

namespace SystemBiblioteczny.Models
{
    public class ApplicationBook : Book
    {
        private AccountBase accountModel = new();

        public int ID { get; set; }
        public int Quantity { get; set; }
        public string Librarian { get; set; }

        public bool Approved { get; set; }

        public ApplicationBook(int id, string title, string author, int quantity, string librarian, bool approved)
        {
            this.ID = id;
            this.Title = title;
            this.Author = author;
            this.Quantity = quantity;
            this.Librarian = librarian;
            this.Approved = approved;

        }
        public ApplicationBook(ApplicationBook book)
        {
            string newTitle = "";
            string newAuthor = "";
            string[] splittedTitle = book.Title.Split("_", StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < splittedTitle.Length; i++)
            {
                newTitle = newTitle + splittedTitle[i] + " ";
            }
            string[] splittedAuthor = book.Author.Split("_", StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < splittedAuthor.Length; i++)
            {
                newAuthor = newAuthor + splittedAuthor[i] + " ";
            }
            Quantity = book.Quantity;
            Librarian = book.Librarian;
            Approved = book.Approved;
            ID = book.ID;
            Author = newAuthor;
            Title = newTitle;

        }

        public ApplicationBook()
        {

        }

        public List<ApplicationBook> GetApplicationBooksList()
        {
            List<ApplicationBook> list = new();
            List<string> lines = accountModel.GetListOfDataBaseLines("BookApplicationList");

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                string[] splitted = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                int newId = int.Parse(splitted[0]);
                string newTitle = splitted[1];
                string newAuthor = splitted[2];
                int newQuantity = int.Parse(splitted[3]);
                string newRequestor = splitted[4];
                bool newApproval = bool.Parse(splitted[5]);

                ApplicationBook book = new(newId, newTitle, newAuthor, newQuantity, newRequestor, newApproval);

                list.Add(book);

            }
            return list;
        }
    }
}
