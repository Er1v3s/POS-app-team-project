using POS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Data
{
    internal class OrderData
    {
        public static void AddOrderToDb(Orders order)
        {
            using (var db = new AppDbContext())
            {
                db.Add(order);
                db.SaveChanges();
            }
        }

        public static List<Orders> GetAllOrders()
        {
            using (var db = new AppDbContext())
            {
                return db.Orders.ToList();
            }
        }
    }
}
