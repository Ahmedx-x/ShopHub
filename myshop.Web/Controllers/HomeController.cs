using Microsoft.AspNetCore.Mvc;
using MyShop.Domain.Interfaces;
using MyShop.Domain.Models;
using myshop.Web.ViewModels;
using System.Diagnostics;

namespace myshop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync(pageSize: 10);
            var products = await _unitOfWork.Products.GetAllAsync(include: "Category", pageSize: 8);

            var viewModel = new HomeVM
            {
                Categories = categories.Items,
                Products = products.Items,
                ProductCount = await _unitOfWork.Products.CountAsync(),
                CategoryCount = await _unitOfWork.Categories.CountAsync()
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
