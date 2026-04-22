namespace WebApplication1.Dto
{
    public class HomeDto
    {
        public string type { get; set; }
        public long price { get; set; }

        public HomeDto() { }
        public HomeDto(string type, long price)
        {
           this.type = type;
            this.price = price;
        }
    }
}
