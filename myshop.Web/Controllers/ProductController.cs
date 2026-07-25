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
        public async Task<IActionResult> GetData()
        {
            var products = await _unitOfWork.Products.GetAllAsync(include: "Category");
            var productsDto = _mapper.Map<IReadOnlyList<ProductDto>>(products);
            return Json(new { data = productsDto });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var catigories = await _unitOfWork.Categories.GetAllAsync();
            var list = _mapper.Map<IEnumerable<CategoryDto>>(catigories);

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
            return View(productVM.Product);
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
            var list = _mapper.Map<IEnumerable<CategoryDto>>(catigories);

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

            return View(productVM.Product);
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
