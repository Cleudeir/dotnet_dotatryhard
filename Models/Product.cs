namespace dotatryhard.Models
{
    public class Product
    {
          public Product() 
            {
                Name = ""; 
            }

        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}