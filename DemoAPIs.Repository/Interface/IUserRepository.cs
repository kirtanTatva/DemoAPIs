using DemoAPIs.Entity.Models;
using DemoAPIs.Entity.ViewModels.User;
using DemoAPIs.Repository.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPIs.Repository.Interface
{
    public interface IUserRepository
    {
        public List<ItemsViewModel> GetItemsListbyUser(string useremail);
        public void AddItemsbyUser(string useremail, AddItemViewModel item);
    }
}
