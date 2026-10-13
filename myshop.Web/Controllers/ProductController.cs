using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Entities.ViewModels;
using MyShop.Business.DTOs;
using MyShop.Business.Interfaces;
using MyShop.Domain.Interfaces;
using MyShop.Domain.Models;

namespace myshop.Web.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public ProductController(
            IUnitOfWork unitOfWork,
            IFileService fileService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetData(string? searchTerm, int pageSize = 10, int pageNumber = 1)
        {
            // Support DataTables server-side processing parameters while keeping the
            // original pageSize/pageNumber/searchTerm contract for other callers.
            var draw = Request.Query["draw"].FirstOrDefault();

            if (int.TryParse(Request.Query["length"], out var length) && length > 0 &&
                int.TryParse(Request.Query["start"], out var start) && start >= 0)
            {
                pageSize = length;
                pageNumber = (start / length) + 1;
            }

            var dataTablesSearch = Request.Query["search[value]"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(dataTablesSearch))
                searchTerm = dataTablesSearch;

            Pagination<Product> result;

            if (!string.IsNullOrWhiteSpace(searchTerm))
                result = await _unitOfWork.Products.SearchAsync(searchTerm, pageSize, pageNumber);

            else
                result = await _unitOfWork.Products
                    .GetAllAsync(include: "Category", pageSize: pageSize, pageNumber: pageNumber);

            var productsDto = _mapper.Map<IReadOnlyList<ProductDto>>(result.Items);
            return Json(new
            {
                draw = draw ?? "1",
                recordsTotal = result.TotalItems,
                recordsFiltered = result.TotalItems,
                data = productsDto,
            });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var catigories = await _unitOfWork.Categories.GetAllAsync();
            var list = _mapper.Map<IEnumerable<CategoryDto>>(catigories.Items);

            ProductVM productVM = new ProductVM
            {
                Product = new Product(),
                CategoryList = list
            };
            return View(productVM);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ProductVM productVM, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    var imagePath = await _fileService.UploadAsync(file);
                    productVM.Product.Img = imagePath;
                }

                await _unitOfWork.Products.AddAsync(productVM.Product);
                await _unitOfWork.SaveChangesAsync();

                TempData["Create"] = "Item has Created Successfully";

                return RedirectToAction("Index");
            }

            var categories = await _unitOfWork.Categories.GetAllAsync();
            productVM.CategoryList = _mapper.Map<IEnumerable<CategoryDto>>(categories.Items);
            return View(productVM);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
                return BadRequest();

            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product is null)
                return NotFound();

            var catigories = await _unitOfWork.Categories.GetAllAsync();
            var list = _mapper.Map<IEnumerable<CategoryDto>>(catigories.Items);

            ProductVM productVM = new ProductVM
            {
                Product = product,
                CategoryList = list
            };

            return View(productVM);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    var oldImagePath = productVM.Product.Img;

                    var imagePath = await _fileService.UploadAsync(file);

                    productVM.Product.Img = imagePath;

                    //the delete operation should be after ensuring that file uploaded successfully
                    if (oldImagePath != null)
                        await _fileService.DeleteAsync(oldImagePath);
                }

                await _unitOfWork.Products.UpdateAsync(productVM.Product);
                await _unitOfWork.SaveChangesAsync();

                TempData["Update"] = "Data has Updated Successfully";

                return RedirectToAction("Index");
            }

            var categories = await _unitOfWork.Categories.GetAllAsync();
            productVM.CategoryList = _mapper.Map<IEnumerable<CategoryDto>>(categories.Items);
            return View(productVM);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var productIndb = await _unitOfWork.Products.GetByIdAsync(id);

            if (productIndb == null)
                return Json(new { success = false, message = "Error while Deleting" });

            await _fileService.DeleteAsync(productIndb.Img);

            await _unitOfWork.Products.DeleteAsync(productIndb);
            await _unitOfWork.SaveChangesAsync();

            return Json(new { success = true, message = "file has been Deleted" });
        }
    }
}
