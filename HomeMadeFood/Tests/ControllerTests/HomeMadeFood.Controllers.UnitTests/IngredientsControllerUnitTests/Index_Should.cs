﻿using System;
using System.Collections.Generic;
using System.Linq;

using Moq;
using NUnit.Framework;
using TestStack.FluentMVCTesting;

using HomeMadeFood.Models;
using HomeMadeFood.Models.Enums;
using HomeMadeFood.Services.Common.Contracts;
using HomeMadeFood.Services.Data.Contracts;
using HomeMadeFood.Web.Areas.Admin.Controllers;
using HomeMadeFood.Web.Areas.Admin.Models;

namespace HomeMadeFood.Controllers.UnitTests.IngredientsControllerUnitTests
{
    [TestFixture]
    public class Index_Should
    {
        [Test]
        public void RenderTheRightView()
        {
            //Arrange
            IEnumerable<Ingredient> ingredients = new List<Ingredient>();
            var inredientsServiceMock = new Mock<IIngredientsService>();
            inredientsServiceMock.Setup(x => x.GetAllIngredients()).Returns(ingredients);
            var mappingServiceMock = new Mock<IMappingService>();
            var controller = new IngredientsController(inredientsServiceMock.Object, mappingServiceMock.Object);

            //Act & Assert
            controller.WithCallTo(x => x.Index(It.IsAny<string>())).ShouldRenderView("Index");
        }

        [Test]
        public void RenderTheRightViewWithTheCorrectModel_SearchIngredientViewModelAndNoModelErrors()
        {
            //Arrange
            IEnumerable<Ingredient> ingredients = new List<Ingredient>();
            var inredientsServiceMock = new Mock<IIngredientsService>();
            inredientsServiceMock.Setup(x => x.GetAllIngredients()).Returns(ingredients);
            var mappingServiceMock = new Mock<IMappingService>();
            var controller = new IngredientsController(inredientsServiceMock.Object, mappingServiceMock.Object);

            //Act & Assert
            controller.WithCallTo(x => x.Index(It.IsAny<string>()))
                .ShouldRenderView("Index")
                .WithModel<SearchIngredientViewModel>()
                .AndNoModelErrors();
        }

        [Test]
        public void RenderTheRightViewWithTheCorrectModel_SearchIngredientViewModelAndNoModelErrorsAndCorrectContent()
        {
            //Arrange
            Guid ingredientId = Guid.NewGuid();
            Ingredient ingredient = new Ingredient() { Id = ingredientId, Name = "IngredientName", FoodType = FoodType.Cheese, MeasuringUnit = MeasuringUnitType.PerUnit, PricePerMeasuringUnit = 12.60m, Quantity = 0 };
            IEnumerable<Ingredient> ingredients = new List<Ingredient>()
            {
                ingredient
            };
            var inredientsServiceMock = new Mock<IIngredientsService>();
            inredientsServiceMock.Setup(x => x.GetAllIngredients()).Returns(ingredients);
            var mappingServiceMock = new Mock<IMappingService>();
            var searchModel = new SearchIngredientViewModel();
            searchModel.Ingredients = ingredients.Select(x => new IngredientViewModel() { Name = x.Name });
            var controller = new IngredientsController(inredientsServiceMock.Object, mappingServiceMock.Object);
            controller.Index("some name");

            //Act & Assert
            controller.WithCallTo(x => x.Index(It.IsAny<string>()))
                .ShouldRenderView("Index")
                .WithModel<SearchIngredientViewModel>(
                    viewModel => Assert.AreEqual(ingredients.ToList()[0].Name, searchModel.Ingredients.ToList()[0].Name))
                .AndNoModelErrors();
        }
    }
}