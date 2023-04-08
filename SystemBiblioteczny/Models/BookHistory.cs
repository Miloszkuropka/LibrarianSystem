using System;
using System.Collections.Generic;

namespace SystemBiblioteczny.Models
{
    public class BookHistory : BookReserved
    {
        public string DateReturn { get; set; }

        public List<BookHistory> listH = new();

        public BookHistory(int id, int libraryId, string author, string title, string userName, string dateTaken, string dateReturn)
        {
            Id_Book = id;
            Id_Library = libraryId;
            Author = author;
            Title = title;
            UserName = userName;
            DateTime1 = dateTaken;
            DateReturn = dateReturn;
        }
        public BookHistory(BookHistory book)
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

            Author = newAuthor;
            Title = newTitle;
            DateTime1 = book.DateTime1;
            UserName = book.UserName;
            Id_Book = book.Id_Book;
            Id_Library = book.Id_Library;
            DateReturn = book.DateReturn;

        }
        public BookHistory()
        {
        }
    }
}
