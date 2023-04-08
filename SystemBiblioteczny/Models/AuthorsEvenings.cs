using System;
using System.Collections.Generic;
using System.IO;

namespace SystemBiblioteczny.Models
{
    public class AuthorsEvenings
    {
        AccountBase accountModel = new();
        public bool CheckIfLibraryExist(int id)
        {

            List<string> lines = accountModel.GetListOfDataBaseLines("Libraries");

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];

                string[] splitted = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                if (int.Parse(splitted[0]) == id) return true;
            }
            return false;
        }

        public void Add(AuthorsEvening newAuthorsEvening)
        {
            string path = System.IO.Path.Combine("../../../DataBases/AuthorsEveningList.txt");
            List<string> lines = accountModel.GetListOfDataBaseLines("AuthorsEveningList");

            using (StreamWriter writer = new StreamWriter(path))
            {

                foreach (string line in lines)
                {
                    writer.WriteLine(line);
                }
                string text = newAuthorsEvening.Date.ToString()!;
                string[] splitted = text.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                writer.WriteLine(newAuthorsEvening.Approved + " " + newAuthorsEvening.User + " " + newAuthorsEvening.FirstName + " " + newAuthorsEvening.LastName
                    + " " + newAuthorsEvening.LibraryID + " " + splitted[0] + " "
                    + " " + newAuthorsEvening.Hour + " " + newAuthorsEvening.PhoneNumber);

                writer.Close();
            }
        }
        public List<AuthorsEvening> GetEventList()
        {
            List<AuthorsEvening> list = new();
            List<string> lines = accountModel.GetListOfDataBaseLines("AuthorsEveningList");

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];

                string[] splitted = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                bool approved = bool.Parse(splitted[0]);
                string username = splitted[1];
                string authorsName = splitted[2];
                string authorsLastname = splitted[3];
                int libraryID = int.Parse(splitted[4]);
                string newDate = splitted[5];
                int newHour = int.Parse(splitted[6]);
                string newPhoneNumber = splitted[7];
                DateTime? newDate1 = DateTime.Parse(newDate);
                AuthorsEvening newEvent = new(approved, username, authorsName, authorsLastname, libraryID, newDate1, newHour, newPhoneNumber);
                list.Add(newEvent);
            }
            return list;
        }


        public void RemoveFromList(AuthorsEvening evening)
        {
            AuthorsEvenings e = new();
            List<AuthorsEvening> listofevenings = e.GetEventList();
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, "../../../DataBases/AuthorsEveningList.txt");
            using (StreamWriter writer = new StreamWriter(path))
            {
                for (int i = 0; i < listofevenings.Count; i++)
                {
                    if (!evening.Equals(listofevenings[i]))
                    {
                        string text = listofevenings[i].Date.ToString()!;
                        string[] splitted = text.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                        writer.WriteLine(listofevenings[i].Approved + " " + listofevenings[i].User + " " + listofevenings[i].FirstName + " " + listofevenings[i].LastName
                    + " " + listofevenings[i].LibraryID + " " + splitted[0] + " "
                    + " " + listofevenings[i].Hour + " " + listofevenings[i].PhoneNumber);
                    }
                }
                writer.Close();
            }
        }

        internal void ChangeApprovedToTrue(AuthorsEvening evening)
        {
            AuthorsEvenings e = new();
            List<AuthorsEvening> listofevenings = e.GetEventList();
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, "../../../DataBases/AuthorsEveningList.txt");
            using (StreamWriter writer = new StreamWriter(path))
            {
                for (int i = 0; i < listofevenings.Count; i++)
                {
                    if (evening.Equals(listofevenings[i]))
                    {
                        listofevenings[i].Approved = true;
                    }
                    string text = listofevenings[i].Date.ToString()!;
                    string[] splitted = text.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                    writer.WriteLine(listofevenings[i].Approved + " " + listofevenings[i].User + " "
                    + listofevenings[i].FirstName + " " + listofevenings[i].LastName
                    + " " + listofevenings[i].LibraryID + " " + splitted[0] + " "
                    + " " + listofevenings[i].Hour + " " + listofevenings[i].PhoneNumber);
                }
                writer.Close();
            }
        }

        internal void ChangeApprovedToFalse(AuthorsEvening evening)
        {
            AuthorsEvenings e = new();
            List<AuthorsEvening> listofevenings = e.GetEventList();
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, "../../../DataBases/AuthorsEveningList.txt");
            using (StreamWriter writer = new StreamWriter(path))
            {
                for (int i = 0; i < listofevenings.Count; i++)
                {
                    if (evening.Equals(listofevenings[i]))
                    {
                        listofevenings[i].Approved = false;
                    }
                    string text = listofevenings[i].Date.ToString()!;
                    string[] splitted = text.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                    writer.WriteLine(listofevenings[i].Approved + " " + listofevenings[i].User + " "
                    + listofevenings[i].FirstName + " " + listofevenings[i].LastName
                    + " " + listofevenings[i].LibraryID + " " + splitted[0] + " "
                    + " " + listofevenings[i].Hour + " " + listofevenings[i].PhoneNumber);
                }
                writer.Close();
            }
        }
    }
}
