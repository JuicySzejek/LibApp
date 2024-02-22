using LibApp.Data;
using LibApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using LibApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;

namespace LibApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    

    public class CustomersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        public CustomersController(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET /api/customers
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            //Included membership name
            var usersWithMembershipType = _userManager.Users
                 .Include(u => u.MembershipType)
                 .ToList();

            //User in role
            var usersInRole = await _userManager.GetUsersInRoleAsync(UserRoles.User);

            //join two tables
            var connectedUsers = usersWithMembershipType
                .Join(usersInRole,
                    user => user.Id,
                    roleUser => roleUser.Id,
                    (user, roleUser) => user)
                .ToList();

            return Ok(connectedUsers);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetCustomersAndManagers()
        {
            var usersWithMembershipType = await _userManager.Users
                 .Include(u => u.MembershipType)
                 .ToListAsync();

            var usersInUserRole = await _userManager.GetUsersInRoleAsync(UserRoles.User);
            var usersInStoreManagerRole = await _userManager.GetUsersInRoleAsync(UserRoles.StoreManager);

            var combinedUsers = usersInUserRole.Concat(usersInStoreManagerRole).ToList();

            var connectedUsers = usersWithMembershipType
               .Join(combinedUsers,
                   user => user.Id,
                   roleUser => roleUser.Id,
                   (user, roleUser) => user)
               .ToList();

            return Ok(connectedUsers);
        }


        // GET /api/customers/{id}
        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUserDetails(string id)
        {
            var user = await _context.Users
               .Include(c => c.MembershipType)
               .FirstOrDefaultAsync(c => c.Id.Contains(id));

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // POST /api/customers
        [HttpPost]
        public async Task<IActionResult> CreateUser(CustomerViewModel userVM)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var newUser = new User()
            {
                UserName = userVM.Email,
                Email = userVM.Email,
                EmailConfirmed = true,
                Name = userVM.User.Name,
                LastName = userVM.User.LastName,
                Birthdate = userVM.User.Birthdate,
                MembershipTypeId = userVM.User.MembershipTypeId,
                HasNewsletterSubscribed = userVM.User.HasNewsletterSubscribed
            };
            var userResponse = await _userManager.CreateAsync(newUser, userVM.Password);

            if (!userResponse.Succeeded)
            {
                foreach (var error in userResponse.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }

            await _userManager.AddToRoleAsync(newUser, UserRoles.User);

            return CreatedAtRoute("GetUser", new { id = newUser.Id }, newUser);
        }

        // PUT /api/customers/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomer(string id, CustomerViewModel customer)
        {
            if (!ModelState.IsValid)
                return BadRequest();


            var customerInDb = _context.Users.SingleOrDefault(c => c.Id == id);

            if (customerInDb == null)
                return NotFound();

            customerInDb.Name =  customer.User.Name;
            customerInDb.Email = customer.Email;
            customerInDb.Name = customer.User.Name;
            customerInDb.LastName = customer.User.LastName;
            customerInDb.Birthdate = customer.User.Birthdate;
            customerInDb.MembershipTypeId = customer.User.MembershipTypeId;
            customerInDb.HasNewsletterSubscribed = customer.User.HasNewsletterSubscribed;



            var result = await _userManager.UpdateAsync(customerInDb);

            if (!result.Succeeded)
                BadRequest();
            
            _context.Update(customerInDb);
            _context.SaveChanges();
            return Ok(customerInDb);
        }
        
         // DELETE /api/customers/{id}
         [HttpDelete("{id}")]
         public async Task<IActionResult> DeleteCustomer(string id)
         {
             var customerInDb = await _context.Users
                .Include(u => u.MembershipType)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (customerInDb == null)
                 return NotFound();

             _context.Users.Remove(customerInDb);
             _context.SaveChanges();

             return Ok(customerInDb);
         }
         
        [HttpGet("membership")]
        public async Task<IActionResult> GetMembershipTypes()
        {
            var memberships = await _context.MembershipTypes.ToListAsync();
            return Ok(memberships);
        }

    }
}
