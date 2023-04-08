namespace SystemBiblioteczny.Models
{
    public class Library
    {
        public int ID { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Local { get; set; }

        public Library(int id, string city, string street, string local)
        {
            this.ID = id;
            this.City = city;
            this.Street = street;
            this.Local = local;
        }
        public Library() { }


    }
}
