﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Bytes2you.Validation;

using HomeMadeFood.Models.Enums;
using HomeMadeFood.Services.Common.Contracts;
using HomeMadeFood.Services.Data.Contracts;
using HomeMadeFood.Web.Areas.Admin.Models;
using HomeMadeFood.Web.Common.Messaging;
using HomeMadeFood.Web.Controllers.Extensions;
using HomeMadeFood.Web.App_GlobalResources;

namespace HomeMadeFood.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DailyMenusController : Controller
    {
        private readonly int gridPageSize = 25;

        private string toastrSuccessTitle = GlobalResources.ТoastrSuccessTitle;
        private string toastrAddObjectSuccessMessage = GlobalResources.ToastrAddDailyMenuSuccessMessage;
        private string toastrUpdateObjectSuccessMessage = GlobalResources.ТoastrUpdateDailyMenuSuccessMessage;
        private string toastrDeleteObjectSuccessMessage = GlobalResources.ToastrDeleteDailyMenuSuccessMessage;

        private string toastrFailureTitle = GlobalResources.ToastrFailureTitle;
        private string toastrAddObjectFailureMessage = GlobalResources.ToastrAddDailyMenuFailureMessage;
        private string toastrUpdateObjectFailureMessage = GlobalResources.ToastrUpdateDailyMenuFailureMessage;
        private string toastrDeleteObjectFailureMessage = GlobalResources.ToastrDeleteDailyMenuFailureMessage;

        private readonly IRecipesService recipesService;
        private readonly IDailyMenuService dailyMenuService;
        private readonly IMappingService mappingService;

        public DailyMenusController(IRecipesService recipesService, IDailyMenuService dailyMenuService, IMappingService mappingService)
        {
            Guard.WhenArgument(recipesService, "recipesService").IsNull().Throw();
            this.recipesService = recipesService;

            Guard.WhenArgument(dailyMenuService, "dailyMenuService").IsNull().Throw();
            this.dailyMenuService = dailyMenuService;

            Guard.WhenArgument(mappingService, "mappingService").IsNull().Throw();
            this.mappingService = mappingService;
        }

        [OutputCache(CacheProfile = "AdminIndex")]
        public ActionResult Index()
        {
            var dailyMenus = this.dailyMenuService.GetAllDailyMenus()
                .Select(this.mappingService.Map<DailyMenuViewModel>)
                .ToList();

            SearchDailyMenuViewModel model = new SearchDailyMenuViewModel()
            {
                PageSize = gridPageSize,
                TotalRecords = dailyMenus.Count,
                DailyMenus = dailyMenus
            };
            return View(model);
        }

        public ActionResult Search(string recipeTitle)
        {
            var dailyMenus = this.dailyMenuService.GetAllDailyMenus()
                .Where(x => x.Recipes.Where(r => r.Title.ToLower().Contains(recipeTitle.ToLower())).Any())
                .Select(this.mappingService.Map<DailyMenuViewModel>)
                .ToList();

            var searchModel = new SearchDailyMenuViewModel();
            if (dailyMenus != null)
            {
                searchModel.DailyMenus = dailyMenus;
                searchModel.PageSize = gridPageSize;
                searchModel.TotalRecords = dailyMenus.Count();
            }

            return this.PartialView("_DailyMenusGridPartial", searchModel);
        }

        [OutputCache(Duration = 60, Location = System.Web.UI.OutputCacheLocation.Client, VaryByParam = "none")]
        [HttpGet]
        public ActionResult AddDailyMenu()
        {
            AddDailyMenuViewModel model = this.GetAddDailyMenuViewModelWithAllRecipes();
            EditDailyMenuViewModel editModel = new EditDailyMenuViewModel();
            editModel.DailyMenuViewModelWithAllRecipes = model;
            editModel.SelectedDailyMenuViewModel = new DailyMenuViewModel()
            {
                Salads = new List<RecipeViewModel>(),
                BigSalads = new List<RecipeViewModel>(),
                Soups = new List<RecipeViewModel>(),
                MainDishes = new List<RecipeViewModel>(),
                Vegetarian = new List<RecipeViewModel>(),
                Pasta = new List<RecipeViewModel>(),
                BBQ = new List<RecipeViewModel>(),
                Date = DateTime.Today
            };
            
            return this.View(editModel);
        }

        [OutputCache (Duration = 60, VaryByParam = "none")]
        [ChildActionOnly]
        public ActionResult AddMenu(EditDailyMenuViewModel editModel)
        {       
            return this.PartialView("_AddMenu", editModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDailyMenu(DateTime selectedDate, IEnumerable<Guid> recipesIds)
        {
            if (!this.ModelState.IsValid)
            {
                this.AddToastMessage(toastrFailureTitle, string.Format(toastrAddObjectFailureMessage, selectedDate), ToastType.Error);
                return this.View("AddDailyMenu");
            }

            this.dailyMenuService.AddDailyMenu(selectedDate, recipesIds);
            this.AddToastMessage(toastrSuccessTitle, string.Format(toastrAddObjectSuccessMessage, selectedDate), ToastType.Success);
            return this.RedirectToAction("Index");
        }

        public ActionResult DetailsDailyMenu(Guid id)
        {
            if (id == Guid.Empty)
            {
                return this.View("404.html");
            }

            var menuModel = this.GetDailyMenuViewModelById(id);

            return this.View(menuModel);
        }

        [OutputCache(Duration = 30, Location = System.Web.UI.OutputCacheLocation.Client, VaryByParam = "id")]
        [HttpGet]
        public ActionResult EditDailyMenu(Guid id)
        {
            if (id == Guid.Empty)
            {
                return this.View("404.html");
            }

            var selectedMenu = this.GetDailyMenuViewModelById(id);
            var allMenus = this.GetAddDailyMenuViewModelWithAllRecipes();

            EditDailyMenuViewModel editModel = new EditDailyMenuViewModel()
            {
                Id = selectedMenu.Id,
                SelectedDailyMenuViewModel = selectedMenu,
                SelectedDate = selectedMenu.Date,
                DailyMenuViewModelWithAllRecipes = allMenus
            };

            return this.View(editModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDailyMenu(Guid id, DateTime selectedDate, IEnumerable<Guid> recipesIds)
        {
            if (!this.ModelState.IsValid)
            {
                this.AddToastMessage(toastrFailureTitle, string.Format(toastrUpdateObjectFailureMessage, selectedDate), ToastType.Error);
                return this.RedirectToAction("EditDailyMenu");
            }

            this.dailyMenuService.EditDailyMenu(id, selectedDate, recipesIds);
            this.AddToastMessage(toastrSuccessTitle, string.Format(toastrUpdateObjectSuccessMessage, selectedDate), ToastType.Success);

            return this.RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult DeleteDailyMenu(Guid id)
        {
            if (id == Guid.Empty)
            {
                return this.View("404.html");
            }

            var dailyMenu = this.dailyMenuService.GetDailyMenuById(id);
            var model = this.mappingService.Map<DailyMenuViewModel>(dailyMenu);

            return this.View(model);
        }

        [HttpPost, ActionName("DeleteDailyMenu")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDailyMenuConfirm(Guid id)
        {
            if (id == Guid.Empty)
            {
                return this.View("404.html");
            }

            var dailyMenu = this.dailyMenuService.GetDailyMenuById(id);

            if (dailyMenu == null)
            {
                this.AddToastMessage(toastrFailureTitle, string.Format(toastrDeleteObjectFailureMessage, dailyMenu.Date), ToastType.Error);
                var model = this.mappingService.Map<DailyMenuViewModel>(dailyMenu);
                return this.View("DeleteDailyMenu", model.Id);
            }

            this.dailyMenuService.DeleteDailyMenu(dailyMenu);

            this.AddToastMessage(toastrSuccessTitle, string.Format(toastrDeleteObjectSuccessMessage, dailyMenu.Date), ToastType.Success);
            return this.RedirectToAction("Index");
        }

        private AddDailyMenuViewModel GetAddDailyMenuViewModelWithAllRecipes()
        {
            var soups = this.recipesService.GetAllOfDishType(DishType.Soup)
                .Select(this.mappingService.Map<RecipeViewModel>)
                .ToList();

            var salads = this.recipesService.GetAllOfDishType(DishType.Salad)
                .Select(this.mappingService.Map<RecipeViewModel>)
                .ToList();

            var bigSalads = this.recipesService.GetAllOfDishType(DishType.BigSalad)
                .Select(this.mappingService.Map<RecipeViewModel>)
                .ToList();

            var mainDishes = this.recipesService.GetAllOfDishType(DishType.MainDish)
                .Select(this.mappingService.Map<RecipeViewModel>)
                .ToList();

            var vegetarian = this.recipesService.GetAllOfDishType(DishType.Vegetarian)
                .Select(this.mappingService.Map<RecipeViewModel>)
                .ToList();

            var bbq = this.recipesService.GetAllOfDishType(DishType.BBQ)
               .Select(this.mappingService.Map<RecipeViewModel>)
               .ToList();

            var pasta = this.recipesService.GetAllOfDishType(DishType.Pasta)
               .Select(this.mappingService.Map<RecipeViewModel>)
               .ToList();

            AddDailyMenuViewModel model = new AddDailyMenuViewModel();
            model.Soups = soups;
            model.Salads = salads;
            model.BigSalads = bigSalads;
            model.MainDishes = mainDishes;
            model.Vegetarian = vegetarian;
            model.BBQ = bbq;
            model.Pasta = pasta;

            var viewModel = model;
            //if (this.HttpContext.Cache["AddDailyMenuViewModel"] == null)
            //{
            //    this.HttpContext.Cache.Insert(
            //        "AddDailyMenuViewModel",
            //        model,
            //        null,
            //        DateTime.Now.AddMinutes(1),
            //        TimeSpan.Zero,
            //        CacheItemPriority.Default, 
            //        null);
            //}
            //viewModel = this.HttpContext.Cache["AddDailyMenuViewModel"] as AddDailyMenuViewModel;

            return model;
        }

        private DailyMenuViewModel GetDailyMenuViewModelById(Guid id)
        {
            var menu = this.dailyMenuService.GetDailyMenuById(id);
            var menuModel = this.mappingService.Map<DailyMenuViewModel>(menu);

            menuModel.Salads = menu.Recipes.Where(x => x.DishType == DishType.Salad)
                .Select(this.mappingService.Map<RecipeViewModel>)
                .ToList();

            menuModel.BigSalads = menu.Recipes.Where(x => x.DishType == DishType.BigSalad)
                .Select(this.mappingService.Map<RecipeViewModel>);

            menuModel.Soups = menu.Recipes.Where(x => x.DishType == DishType.Soup)
                .Select(this.mappingService.Map<RecipeViewModel>)
                .ToList();

            menuModel.MainDishes = menu.Recipes.Where(x => x.DishType == DishType.MainDish)
                .Select(this.mappingService.Map<RecipeViewModel>)
                .ToList();

            menuModel.Vegetarian = menu.Recipes.Where(x => x.DishType == DishType.Vegetarian)
                .Select(this.mappingService.Map<RecipeViewModel>)
                .ToList();

            menuModel.Pasta = menu.Recipes.Where(x => x.DishType == DishType.Pasta)
                .Select(this.mappingService.Map<RecipeViewModel>)
                .ToList();

            menuModel.BBQ = menu.Recipes.Where(x => x.DishType == DishType.BBQ)
                .Select(this.mappingService.Map<RecipeViewModel>)
                .ToList();

            return menuModel;
        }
    }
}