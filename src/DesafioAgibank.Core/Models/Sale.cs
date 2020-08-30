using System.Collections.Generic;

namespace DesafioAgibank.Core.Models
{
    public class Sale
    {
        public string Id { get; set; }
        public string SaleId { get; set; }
        public string SalesmanName { get; set; }
        public List<SaleItem> SaleItems { get; set; }

        public Sale()
        {
            SaleItems = new List<SaleItem>();
        }
    }

    public class SaleItem
    {
        public string ItemId { get; set; }
        public double ItemQuantity { get; set; }
        public double ItemPrice { get; set; }
    }
}