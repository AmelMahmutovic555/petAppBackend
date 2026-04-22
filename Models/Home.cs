namespace WebApplication1.Models
{
    public class Home
    {
        public int id { get; set; }
        public string type { get; set; }
        public long price { get; set; }


       public Home() { }
       public Home(int id, string type, long price)
        {
            this.id = id;
            this.type = type;
            this.price = price;
        }

        public Home(string type, long price)
        {
            //this.id = id;
            this.type = type;
            this.price = price;
        }
    }
}
