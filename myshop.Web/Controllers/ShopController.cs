using Microsoft.AspNetCore.Mvc;
using myshop.Web.ViewModels;
using MyShop.Domain.Interfaces;
using MyShop.Domain.Models;

namespace myshop.Web.Controllers
{
    public class ShopController : Controller
    {
        private const int PageSize = 9;
        private const int CategoryFetchSize = 10;

        private readonly IUnitOfWork _unitOfWork;

        public ShopController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchTerm, int? categoryId, int pageNumber = 1)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            Pagination<Product> products;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                products = await _unitOfWork.Products
                    .SearchAsync(searchTerm.Trim(), PageSize, pageNumber);
            }
            else
            {
                products = await _unitOfWork.Products.GetAllAsync(
                    predicate: categoryId.HasValue && categoryId.Value > 0
                        ? p => p.CategoryId == categoryId.Value
                        : null,
                    include: "Category",
                    pageSize: PageSize,
                    pageNumber: pageNumber);
            }

            var categories = await _unitOfWork.Categories.GetAllAsync(pageSize: CategoryFetchSize);

            var viewModel = new ShopVM
            {
                Products = products,
                Categories = categories.Items,
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                CategoryName = categoryId.HasValue
                    ? categories.Items.FirstOrDefault(c => c.Id == categoryId.Value)?.Name
                    : null
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return NotFound();

            var product = await _unitOfWork.Products.FindAsync(
                predicate: p => p.Id == id,
                isTracked: false,
                include: "Category");

            if (product is null)
                return NotFound();

            var related = await _unitOfWork.Products.GetAllAsync(
                predicate: p => p.CategoryId == product.CategoryId && p.Id != product.Id,
                include: "Category",
                pageSize: 4);

            ViewData["Related"] = related.Items;

            return View(product);
        }
    }
}
