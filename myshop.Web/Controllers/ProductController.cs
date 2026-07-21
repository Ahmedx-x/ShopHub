using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Entities.ViewModels;
using MyShop.Business.DTOs;
using MyShop.Domain.Interfaces;
using MyShop.Domain.Models;

namespace myshop.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]/[action]")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public ProductController(
            IUnitOfWork unitOfWork,
            IWebHostEnvironment webHostEnvironment,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
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
        public async Task<IActionResult> Create(ProductVM productVM, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                string RootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var Upload = Path.Combine(RootPath, @"Images\Products");
                    var ext = Path.GetExtension(file.FileName);

                    using (var filestream = new FileStream(Path.Combine(Upload, filename + ext), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    productVM.Product.Img = @"Images\Products\" + filename + ext;
                }

                await _unitOfWork.Products.AddAsync(productVM.Product);
                await _unitOfWork.SaveChangesAsync();
                TempData["Create"] = "Item has Created Successfully";
                return RedirectToAction("Index");
            }
            return View(productVM.Product);
        }

        [HttpGet]
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
        public async Task<IActionResult> Edit(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string RootPath = _webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var Upload = Path.Combine(RootPath, @"Images\Products");
                    var ext = Path.GetExtension(file.FileName);

                    if (productVM.Product.Img != null)
                    {
                        var oldimg = Path.Combine(RootPath, productVM.Product.Img.TrimStart('\\'));

                        if (System.IO.File.Exists(oldimg))
                        {
                            System.IO.File.Delete(oldimg);
                        }
                    }

                    using (var filestream = new FileStream(Path.Combine(Upload, filename + ext), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }

                    productVM.Product.Img = @"Images\Products\" + filename + ext;
                }

                await _unitOfWork.Products.UpdateAsync(productVM.Product);
                await _unitOfWork.SaveChangesAsync();

                TempData["Update"] = "Data has Updated Successfully";
                return RedirectToAction("Index");
            }

            return View(productVM.Product);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var productIndb = await _unitOfWork.Products.GetByIdAsync(id);

            if (productIndb == null)
                return Json(new { success = false, message = "Error while Deleting" });

            await _unitOfWork.Products.DeleteAsync(productIndb);

            var oldimg = Path.Combine(_webHostEnvironment.WebRootPath, productIndb.Img.TrimStart('\\'));

            if (System.IO.File.Exists(oldimg))
            {
                System.IO.File.Delete(oldimg);
            }

            await _unitOfWork.SaveChangesAsync();

            return Json(new { success = true, message = "file has been Deleted" });
        }
    }
}
