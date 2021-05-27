namespace Cko.Model
{
    public class Customer
    {
        public List<Product> Products { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateSignedUp { get; set; }

        public Product GetProductionForCustomer(int productId)
        {
            return Products.FirstOrDefault(x => x.Id == productId);
        }

        public void AddProductToCustomer(Product product)
        {
            //The client needs to check if this customer 
            //can have this membership level of product.
            using(var c = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(product);
                var content = new StringContent(json, 
                                    Encoding.UTF8, 
                                    "application/json");

                c.BaseAddress = new Uri("http://something.com/api/");
                //Inform the products service that this customer 
                //has a new product.
                c.PostAsync("/{customerid}/products", content);
            }

            Products.Add(product);
        }

        public bool CustomerCanHaveSilverMembershipLevel()
        {
            //The customer signed up at least 30 days ago. 
            var daysAgo = DateTime.Now - DateSignedUp;
            return daysAgo.TotalDays >= 30;
        }

        public bool CustomerCanHaveGoldMembershipLevel()
        {
            //The customer signed up at least 90 days ago. 
            var daysAgo = DateTime.Now - DateSignedUp;
            return daysAgo.TotalDays >= 90;
        }
    }

    public enum ProductMembershipLevel
    {
        Silver,
        Gold
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ProductMembershipLevel ProductMembershipLevel { get; set; }
    }
}

