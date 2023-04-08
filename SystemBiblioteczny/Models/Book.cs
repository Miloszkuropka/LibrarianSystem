using System;

namespace SystemBiblioteczny
{
    public class Book
    {
        public int Id_Book { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public bool Availability { get; set; }
        public int Id_Library { get; set; }

        public Book(int id_Book, string author, string title, bool availability, int id_Library)
        {
            Id_Book = id_Book;
            Author = author;
            Title = title;
            Availability = availability;
            Id_Library = id_Library;
        }
        public Book(Book book)
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

            Id_Book = book.Id_Book;
            Author = newAuthor;
            Title = newTitle;
            Availability = book.Availability;
            Id_Library = book.Id_Library;

        }

        public Book()
        {
        }
    }
}
