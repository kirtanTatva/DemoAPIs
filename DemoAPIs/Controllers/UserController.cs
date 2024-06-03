using DemoAPIs.Auth;
using DemoAPIs.Entity.ViewModels.Index;
using DemoAPIs.Entity.ViewModels.User;
using DemoAPIs.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DemoAPIs.Controllers
{
    [CustomAuth]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController (IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Route("Test")]
        [HttpGet]
        public ActionResult Test()
        {
            return Ok("End Point is Protected");
        }

        #region Add Item 
        [Route("AddItem")]
        [HttpPost]
        public ActionResult AddItem(AddItemViewModel item)
        {
            string useremail = HttpContext.User.Claims.FirstOrDefault(e => e.Type == ClaimTypes.Email).Value;
            _userRepository.AddItemsbyUser(useremail,item);
            return Ok("Item Added successfully");
        }
        #endregion

        #region Get List of Items based on Logged in User
        [Route("GetList")]
        [HttpGet]
        public ActionResult GetList()
        {
            string useremail = HttpContext.User.Claims.FirstOrDefault(e => e.Type == ClaimTypes.Email).Value;
            List<ItemsViewModel> data = _userRepository.GetItemsListbyUser(useremail);
            return Ok(data);
        }
        #endregion
    }
}
