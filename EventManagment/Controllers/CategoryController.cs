using Domain._DTO.Category;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using ReflectionIT.Mvc.Paging;
using Security;
using Services.Categories;
using System.Security.Claims;

namespace Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IDataProtector _protector;
        private readonly IStringLocalizer<CategoryController> _localizer;
        private readonly ILogger<CategoryController> _logger;


        public CategoryController(ICategoryService categoryService,
             IDataProtectionProvider provider,
            DataProtectionPurposeStrings purposeStrings,
            IStringLocalizer<CategoryController> localizer,
            ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _protector = provider.CreateProtector(purpose: purposeStrings.CategoryControllerPs);
            _localizer = localizer;
            _logger = logger;
        }

        /// <summary>
        /// Shfaq Listen me të dhena ne formatin pagination.
        /// </summary>
        /// <param name="filter"> Parametri i cili bene filtrimin e te dhenave</param>
        /// <param name="encryptedId">Filtrimi me encryptedId</param>
        /// <param name="pageSize">Madhsia e te dhenave ne faqe</param>
        /// <param name="page"> Numri i faqes</param>
        /// <param name="sortExpression"> Parametri i cili ben sortimin</param>
        /// <returns> Index faqen </returns>

        [Route("CategoryData")]
        public async Task<ActionResult> Index(string filter, string encryptedId, int pageSize = 7, int page = 1, string sortExpression = "Name")
        {
            try
            {
                var decryptedId = string.IsNullOrEmpty(encryptedId) ? null : _protector.Unprotect(encryptedId);

                var qry = _categoryService.GetAllForPagination(filter, decryptedId);

                var defaultSortExpression = "Name DESC";

                var dto = await PagingList.CreateAsync(qry, pageSize, page, sortExpression, defaultSortExpression);

                dto.RouteValue = new RouteValueDictionary { { "filter", filter }, { "pageSize", pageSize } };

                foreach (var item in dto)
                {
                    item.EncryptedId = _protector.Protect(item.Id.ToString());
                    item.Id = 0;
                }

                var totalCount = await qry.CountAsync();
                //get totalpages by dividing the totalcount and pagesize 20 / 7 = 2.857, then rounds it to (3)
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                ViewBag.Filter = filter;
                ViewBag.EncryptedId = encryptedId;
                ViewBag.PageSize = pageSize;
                ViewBag.Page = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.SortExpression = sortExpression;

                return View(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                TempData["message"] = "Error";
                TempData["entity"] = "Invalid value.";

                return RedirectToAction(nameof(Index));
            }
        }


        [Route("Category/Create")]
        public ActionResult Create()
        {

            return View();

        }

        /// <summary>
        /// Ben krijimin e objektit
        /// </summary>
        /// <param name="categoryCreateDto">Permban fushat e te dhenave me te cilat behet krijimi i objektit</param>
        /// <returns>Index, encryptedId</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Category/Create")]
        public ActionResult Create(CategoryCreateDto categoryCreateDto)
        {
            try
            {
                var result = _categoryService.Create(categoryCreateDto);

                TempData["message"] = "Added";
                TempData["entity"] = _localizer["Category "].ToString();

                var encryptedId = _protector.Protect(result.Id.ToString());

                return RedirectToAction(nameof(Index), new { encryptedId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                TempData["message"] = "Error";
                TempData["entity"] = _localizer["An error occurred, try again"].ToString();

                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Shfaq te dhenat te cilat do te ndryshohen.
        /// </summary>
        /// <param name="encryptedId"> Id e objektit me te cilin identifikohet </param>
        /// <returns>Faqen</returns>

        [Route("Category/Edit")]
        public async Task<ActionResult> Edit(string encryptedId)
        {
            try
            {
                var id = int.Parse(_protector.Unprotect(encryptedId));
                var result = await _categoryService.GetById(id);

                result.EncryptedId = _protector.Protect(id.ToString());
                result.Id = 0;

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["message"] = "Error";
                TempData["entity"] = _localizer["An error occurred, try again"].ToString();

                _logger.LogError(ex.Message);

                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Ben Ndryshimin e te dhenave, shfaqjen e informacionit te perdoruesi dhe validimin e te dhenave.
        /// </summary>
        /// <param name="categoryDto"> Permban fushat e te dhenave me te cilat behet krijimi </param>
        /// <returns>Index</returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Category/Edit")]
        public ActionResult Edit(CategoryDto categoryEditDto)
        {
            try
            {
                categoryEditDto.Id = int.Parse(_protector.Unprotect(categoryEditDto.EncryptedId));
                categoryEditDto.EncryptedId = "";

                var result = _categoryService.Update(categoryEditDto);
                TempData["message"] = "Updated";
                TempData["entity"] = _localizer["Category "].ToString();


                return RedirectToAction(nameof(Index), new { filter = result.Name });

            }
            catch (Exception ex)
            {
                TempData["message"] = "Error";
                TempData["entity"] = _localizer["An error occurred, try again"].ToString();

                _logger.LogError(ex.Message);

                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Ben fshirjen e te dhenes duke u bazurar ne Id.
        /// </summary>
        /// <param name="encryptedId"> Id e objektit me te cilin identifikohet </param>
        /// <returns>Index</returns>

        [Route("Category/Delete")]
        public ActionResult Delete(string encryptedId)
        {
            try
            {
                var id = int.Parse(_protector.Unprotect(encryptedId));
                var result = _categoryService.Delete(id);

                TempData["message"] = "Deleted";
                TempData["entity"] = _localizer["Category "].ToString();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["message"] = "Error";
                TempData["entity"] = _localizer["An error occurred, try again"].ToString();

                _logger.LogError(ex.Message);

                return RedirectToAction(nameof(Index));
            }
        }

    }
}