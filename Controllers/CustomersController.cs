using Microsoft.AspNetCore.Mvc;
using LibApp.Models;
using LibApp.ViewModels;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace LibApp.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {

        private readonly IHttpClientFactory _httpClientFactory; 
        private readonly string apiUrl = "https://localhost:7192/api/customers";
        private readonly UserManager<User> _userManager;

        public CustomersController(IHttpClientFactory httpClientFactory, UserManager<User> userManager)
        {
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
        }

        [Authorize(Roles = "owner,storemanager")]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> IndexForOwner()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetStringAsync(apiUrl);
            var customers = JsonConvert.DeserializeObject<List<User>>(response);

            return View(customers);
        }

        [Authorize(Roles = "storemanager, owner")]
        public async Task<ActionResult> Details(string id)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetStringAsync($"{apiUrl}/{id}");
            var customer = JsonConvert.DeserializeObject<User>(response);

            if (customer == null)
            {
                return Content("Book not found");
            }

            return View(customer);
        }

        [Authorize(Roles = " owner")]
        public async Task<IActionResult> CustomerForm()
        {
            var viewModel = new CustomerViewModel
            {
                MembershipTypes = await GetMembershipTypes()
            };

            return View(viewModel);
        }

        [Authorize(Roles = " owner")]
        [HttpPost]
        public async Task<IActionResult> Save(CustomerViewModel customerVM)
        {
            try
            {
                if (customerVM == null)
                    return BadRequest("Invalid customer object");
                

                var emailCheck = await _userManager.FindByEmailAsync(customerVM.Email);

                if (emailCheck != null && customerVM.User.Id == null)
                {
                    ModelState.AddModelError("EmailAlreadyExists", "Email already exists.");
                    customerVM.MembershipTypes = await GetMembershipTypes();
                    return RedirectToAction("CustomerForm",customerVM);
                }

                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    if (customerVM.User.Id == null)
                    {
                        var postResponse = await httpClient.PostAsJsonAsync(apiUrl, customerVM);

                        if (!postResponse.IsSuccessStatusCode)
                        {
                            // Optionally, log or handle the response details dev purpose
                            Console.WriteLine($"API request failed: {postResponse.StatusCode}");

                            // Extract and log the content of the response dev purpose
                            var responseContent = await postResponse.Content.ReadAsStringAsync();
                            Console.WriteLine($"Response Content: {responseContent}");

                            return StatusCode((int)postResponse.StatusCode);
                        }
                    }
                    else 
                    {
                        var putResponse = await httpClient.PutAsJsonAsync($"{apiUrl}/{customerVM.User.Id}", customerVM);

                        if (!putResponse.IsSuccessStatusCode)
                        {
                            // Optionally, log or handle the response details dev purpose
                            Console.WriteLine($"API request failed: {putResponse.StatusCode}");
                            return StatusCode((int)putResponse.StatusCode);
                        }
                    }
                }

                return RedirectToAction("Index", "Customers");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500);
            }
        }

        [Authorize(Roles = "owner")]
        public async Task<IActionResult> Edit(string id)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetStringAsync($"{apiUrl}/{id}");
            var customer = JsonConvert.DeserializeObject<User>(response);   

            if (customer == null)
            {
                return NotFound();
            }

            var viewModel = new CustomerViewModel
            {
                User = customer,
                Email = customer.Email,
                MembershipTypes = await GetMembershipTypes()
            };

            return View("CustomerForm", viewModel);
        }

        private async Task<List<MembershipType>> GetMembershipTypes()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetStringAsync("https://localhost:7192/api/customers/membership");
            return JsonConvert.DeserializeObject<List<MembershipType>>(response);
        }
    }
}