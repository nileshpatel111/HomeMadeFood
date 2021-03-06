﻿using System;

using Moq;
using NUnit.Framework;
using TestStack.FluentMVCTesting;

using HomeMadeFood.Models;
using HomeMadeFood.Services.Common.Contracts;
using HomeMadeFood.Services.Data.Contracts;
using HomeMadeFood.Web.Areas.Admin.Controllers;

namespace HomeMadeFood.Controllers.UnitTests.IngredientsControllerUnitTests
{
    [TestFixture]
    public class DeleteIngredient_Should
    {
        [Test]
        public void RenderTheRightView_DeleteIngredient()
        {
            //Arrange
            var inredientsServiceMock = new Mock<IIngredientsService>();
            var foodCategoriesServiceMock = new Mock<IFoodCategoriesService>();
            var recipesServiceMock = new Mock<IRecipesService>();
            var mappingServiceMock = new Mock<IMappingService>();
            var controller = new IngredientsController(inredientsServiceMock.Object, foodCategoriesServiceMock.Object, recipesServiceMock.Object, mappingServiceMock.Object);
            var id = Guid.NewGuid();

            //Act & Assert
            controller.WithCallTo(x => x.DeleteIngredient(id))
                .ShouldRenderView("DeleteIngredient");
        }

        [Test]
        public void RedirectToErrorPage_When_IdGuidIsNotValid()
        {
            //Arrange
            var inredientsServiceMock = new Mock<IIngredientsService>();
            var foodCategoriesServiceMock = new Mock<IFoodCategoriesService>();
            var recipesServiceMock = new Mock<IRecipesService>();
            var mappingServiceMock = new Mock<IMappingService>();
            var controller = new IngredientsController(inredientsServiceMock.Object, foodCategoriesServiceMock.Object, recipesServiceMock.Object, mappingServiceMock.Object);

            //Act & Assert
            controller.WithCallTo(x => x.DeleteIngredient(Guid.Empty))
                .ShouldRenderView("404.html");
        }

        [Test]
        public void RenderTheRightView_DeleteIngredient__WhenIngredientWasNotFoundInDatabase()
        {
            //Arrange
            var ingredientsServiceMock = new Mock<IIngredientsService>();
            var foodCategoriesServiceMock = new Mock<IFoodCategoriesService>();
            var recipesServiceMock = new Mock<IRecipesService>();
            var mappingServiceMock = new Mock<IMappingService>();
            var controller = new IngredientsController(ingredientsServiceMock.Object, foodCategoriesServiceMock.Object, recipesServiceMock.Object, mappingServiceMock.Object);
            var id = Guid.NewGuid();
            ingredientsServiceMock.Setup(x => x.GetIngredientById(id)).Returns<Ingredient>(null);

            //Act & Assert
            controller.WithCallTo(x => x.DeleteIngredient(id))
                .ShouldRenderView("DeleteIngredient");
        }

        [Test]
        public void RedirectToActionIndex__WhenIngredientIsSuccessfullyDeleted()
        {
            //Arrange
            var ingredientsServiceMock = new Mock<IIngredientsService>();
            var foodCategoriesServiceMock = new Mock<IFoodCategoriesService>();
            var recipesServiceMock = new Mock<IRecipesService>();
            var mappingServiceMock = new Mock<IMappingService>();
            var controller = new IngredientsController(ingredientsServiceMock.Object, foodCategoriesServiceMock.Object, recipesServiceMock.Object, mappingServiceMock.Object);
            var id = Guid.NewGuid();

            Ingredient ingredient = new Ingredient();
            ingredient.Id = id;
            ingredient.Name = "Carrot";
            ingredient.FoodCategoryId = Guid.NewGuid();
            ingredient.PricePerMeasuringUnit = 1.80m;
            ingredient.QuantityInMeasuringUnit = 2;

            ingredientsServiceMock.Setup(x => x.GetIngredientById(id)).Returns(ingredient);
            ingredientsServiceMock.Setup(x => x.DeleteIngredient(ingredient));

            //Act & Assert
            controller.WithCallTo(x => x.DeleteIngredientConfirm(id))
                .ShouldRedirectTo(x => x.Index());
        }
    }
}