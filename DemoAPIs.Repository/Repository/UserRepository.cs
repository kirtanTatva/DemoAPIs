using DemoAPIs.Entity.Data;
using DemoAPIs.Entity.Models;
using DemoAPIs.Entity.ViewModels.User;
using DemoAPIs.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPIs.Repository.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext _context;
        public UserRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        #region Get Items List by the useremail
        public List<ItemsViewModel> GetItemsListbyUser(string useremail)
        {
            IEnumerable<ItemsViewModel> items = _context.Items.Include(e=>e.User).Where(e=>e.User.Email == useremail).Select(e=> new ItemsViewModel
            {
                Id = e.Id,
                Name = e.Name
            });
            return items.ToList();
        }
        #endregion

        #region Add new Items by useremail
        public void AddItemsbyUser(string useremail, AddItemViewModel item)
        {
            Item i = new Item();
            i.Name = item.name;
            i.Userid = _context.Users.FirstOrDefault(e=>e.Email == useremail).Id;
            _context.Add(i);
            _context.SaveChanges();
        }
        #endregion
    }
}
