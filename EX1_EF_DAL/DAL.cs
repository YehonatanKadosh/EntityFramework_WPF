using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EX1_EF_DAL
{
    public class DAL
    {
        public IEnumerable<object> GetByMonth(DateTime SelectedDate)
        {
            using (NorthwindEntities data = new NorthwindEntities())
            {   // get the requested parameters from DB
                return data.Orders.Include("Customer").Include("Employee").Include("Order_Details").Where(o => o.OrderDate.Value.Month == SelectedDate.Month && o.OrderDate.Value.Year == SelectedDate.Year).Select(O => new
                {
                    O.OrderID,
                    O.Customer.ContactName,
                    Employee_Name = O.Employee.FirstName + " " + O.Employee.LastName,
                    O.OrderDate,
                    Total_Price = O.Order_Details.Sum(OD => (float)OD.UnitPrice * OD.Quantity * (1 - OD.Discount))
                }).ToArray();
            }
        }

        public IEnumerable<object> GetByOrderID(int OrderID = 10407)
        {
            using (NorthwindEntities data = new NorthwindEntities())
            {
                // get the requested parameters from DB
                var Object_Returned_From_DB = data.Orders.Include("Customer").Include("Employee").Include("Order_Details").Where(O => O.OrderID == OrderID).Select(O => new
                {
                    O.OrderID,
                    O.Customer.ContactName,
                    Employee_Name = O.Employee.FirstName + " " + O.Employee.LastName,
                    O.OrderDate,
                    // Get the Products of this Order from Order_Details
                    Products_Names = O.Order_Details.Select(OD => OD.Product.ProductName),
                    Products_Quantity = O.Order_Details.Select(OD => OD.Quantity),
                    Product_Prices = O.Order_Details.Select(OD => (float)OD.UnitPrice * OD.Quantity * (1 - OD.Discount)),

                    Total_Product_Prices = O.Order_Details.Sum(OD => (float)OD.UnitPrice * OD.Quantity * (1 - OD.Discount))
                }).FirstOrDefault();
            
                // Set the Order By - Products Names Prices Quantities and Total
                if (Object_Returned_From_DB == null)
                    return null;
                else
                {
                    object[] IEnumerable_To_Return = new object[Object_Returned_From_DB.Products_Names.Count()];

                    // Set the parameters
                    string[] Names = Object_Returned_From_DB.Products_Names.ToArray();
                    short[] QuantityByName = Object_Returned_From_DB.Products_Quantity.ToArray();
                    float[] Product_Price = Object_Returned_From_DB.Product_Prices.ToArray();

                    // Create the IEnumerable from the arrays and the object returned from DB
                    for (int i = 0; i < Names.Count(); i++)
                    {
                        IEnumerable_To_Return[i] = new
                        {
                            Object_Returned_From_DB.OrderID,
                            Object_Returned_From_DB.ContactName,
                            Object_Returned_From_DB.Employee_Name,
                            Product_Name = Names[i],
                            Quantity = QuantityByName[i],
                            Price = Product_Price[i],
                            Total = Object_Returned_From_DB.Total_Product_Prices
                        };
                    }
                    return IEnumerable_To_Return;
                }
            }
        }

        public Product GetProductByName(string selectedProductName)
        {
            using (NorthwindEntities data = new NorthwindEntities())
            {
                return data.Products.Include("Order_Details").First(p => p.ProductName == selectedProductName);
            }
        }

        public IEnumerable<Order> GetAllOrders()
        {
            using (NorthwindEntities data = new NorthwindEntities())
            {
                return data.Orders.Include("Customer").Include("Employee").Include("Order_Details").ToArray();
            }
        }
        public static IEnumerable<Product> GetAllProducts()
        {
            using (NorthwindEntities data = new NorthwindEntities())
            {
                return data.Products.Include("Order_Details").ToArray();
            }
        }

        public void UpdateProductByID(int Order_ID, Product p, short newQuantity)
        {
            using (NorthwindEntities data = new NorthwindEntities())
            {
                Order_Detail toUpdate = data.Order_Details.First(od => od.OrderID == Order_ID && od.ProductID == p.ProductID);
                toUpdate.Quantity = newQuantity;
                data.SaveChanges();
            }
        }
        public static void AddProductToOrder(int Order_ID, Product p)
        {
            using (NorthwindEntities data = new NorthwindEntities())
            {
                Order_Detail addNew = new Order_Detail { OrderID = Order_ID, ProductID = p.ProductID, Quantity = 1, UnitPrice = (decimal)p.UnitPrice, Discount = 0 };
                data.Orders.Find(Order_ID).Order_Details.Add(addNew) ;
                data.SaveChanges();

            }
        }
        public void RemoveProductFromOrderDB(int Order_ID, Product p)
        {
            using (NorthwindEntities data = new NorthwindEntities())
            {
                Order_Detail toRemove = data.Order_Details.First(od => od.OrderID == Order_ID && od.ProductID == p.ProductID);
                data.Orders.Find(Order_ID).Order_Details.Remove(toRemove);
                data.SaveChanges();
            }
        }
    }
}
