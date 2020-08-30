using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using DesafioAgibank.Core.Config;
using System.Linq;
using DesafioAgibank.Core.Models;
using System.Text;

namespace DesafioAgibank.Core
{
    public static class FileHelpers
    {
        public static async Task<List<(string description, string value)>> Process(CoreSettings settings)
        {
            var ret = new List<(string description, string value)>();
            var path = Environment.GetEnvironmentVariable(settings.EnvironmentPath);
            if (!Directory.Exists(path)) throw new DirectoryNotFoundException($"Path '{path}' from environment %{settings.EnvironmentPath}% not found.");

            if (!Directory.Exists(@$"{path}\{settings.DataIn.FilePath}")) throw new DirectoryNotFoundException(@$"Path '{path}\{settings.DataIn.FilePath}' not found.");;
            if (!Directory.Exists(@$"{path}\{settings.DataOut.FilePath}")) Directory.CreateDirectory(@$"{path}\{settings.DataOut.FilePath}");

            foreach (var fileName in Directory.GetFiles(@$"{path}\{settings.DataIn.FilePath}", settings.DataIn.FileFilter))
            {
                using var file = new StreamReader(fileName);
                var line = "";
                var lstSellers = new List<Seller>();
                var lstCustomers = new List<Customer>();
                var lstSales = new List<Sale>();

                while ((line = file.ReadLine()) != null)
                {
                    if (line.StartsWith(settings.DataIn.SellerId))
                        lstSellers.Add(await ProcessSellers(line, settings));
                    else if (line.StartsWith(settings.DataIn.CustomerId))
                        lstCustomers.Add(await ProcessCustomers(line, settings));
                    else
                        lstSales.Add(await ProcessSales(line, settings));
                }

                var r1 = ("Quantidade de clientes no arquivo de entrada", lstCustomers.Count.ToString());
                var r2 = ("Quantidade de vendedores no arquivo de entrada", lstSellers.Count.ToString());
                
                (string, double, string)[] res1 = new (string, double, string)[lstSales.Count];
                var i = 0;
                foreach (var sales in lstSales)
                {
                    var t = 0d;
                    foreach (var sItem in sales.SaleItems)
                    {
                        t = sItem.ItemPrice * sItem.ItemQuantity;
                    }
                    res1[i] = (sales.SaleId, t, sales.SalesmanName);
                    i++;
                }

                var r3 = ("ID da venda mais cara", res1.OrderByDescending(p => p.Item2).FirstOrDefault().Item1);
                
                var r4 = ("O pior vendedor", res1.OrderBy(p => p.Item2).FirstOrDefault().Item3);
                
                ret.Add(r1);
                ret.Add(r2);
                ret.Add(r3);
                ret.Add(r4);

                var result = new StringBuilder();
                result.Append(lstCustomers.Count);
                result.Append(settings.DataIn.FieldSeparator);
                result.Append(lstSellers.Count);
                result.Append(settings.DataIn.FieldSeparator);
                result.Append(res1.OrderByDescending(p => p.Item2).FirstOrDefault().Item1);
                result.Append(settings.DataIn.FieldSeparator);
                result.Append(res1.OrderBy(p => p.Item2).FirstOrDefault().Item3);

                var fi = new FileInfo(fileName);
                var outName = @$"{path}\{settings.DataOut.FilePath}\{fi.Name.Replace(".dat", "")}{settings.DataOut.FilePrefix}";
                File.WriteAllText(outName, result.ToString());
            }

            return ret;
        }

        private static async Task<Seller> ProcessSellers(string line, CoreSettings settings)
        {
            var items = line.Split(settings.DataIn.FieldSeparator);
            var seller = new Seller
            {
                Id = items[0],
                Cpf = items[1],
                Name = items[2],
                Salary = Double.Parse(items[3])
            };
            return await Task.FromResult(seller);
        }

        private static async Task<Customer> ProcessCustomers(string line, CoreSettings settings)
        {
            var items = line.Split(settings.DataIn.FieldSeparator);
            var customer = new Customer
            {
                Id = items[0],
                Cnpj = items[1],
                Name = items[2],
                BusinessArea = items[3]
            };
            return await Task.FromResult(customer);
        }

        private static async Task<Sale> ProcessSales(string line, CoreSettings settings)
        {
            var items = line.Split(settings.DataIn.FieldSeparator).ToList();
            var sale = new Sale
            {
                Id = items[0],
                SaleId = items[1],
                SalesmanName = items[3]
            };
            
            var saleItems = items[2].Replace("[", "").Replace("]", "").Split(',');
            foreach (var item in saleItems)
            {
                var sItems = item.Split('-');
                var saleItem = new SaleItem
                {
                    ItemId = sItems[0],
                    ItemQuantity = Double.Parse(sItems[1]),
                    ItemPrice = Double.Parse(sItems[2])
                };

                sale.SaleItems.Add(saleItem);
            }

            return await Task.FromResult(sale);
        }
    }
}
