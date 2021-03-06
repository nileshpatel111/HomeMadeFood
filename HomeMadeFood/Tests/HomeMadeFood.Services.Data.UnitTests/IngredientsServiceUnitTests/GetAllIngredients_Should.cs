﻿using System.Collections.Generic;
using System.Linq;

using Moq;
using NUnit.Framework;

using HomeMadeFood.Data.Data;
using HomeMadeFood.Models;
using HomeMadeFood.Services.Data.Contracts;

namespace HomeMadeFood.Services.Data.UnitTests.IngredientsServiceUnitTests
{
    [TestFixture]
    public class GetAllIngredients_Should
    {
        [Test]
        public void Invoke_TheDataIngredientsRepositoryMethodGetAll_Once()
        {
            //Arrange
            var dataMock = new Mock<IHomeMadeFoodData>();
            var foodCategoriesServiceMock = new Mock<IFoodCategoriesService>();
            IngredientsService ingredientsService = new IngredientsService(dataMock.Object, foodCategoriesServiceMock.Object);
            dataMock.Setup(x => x.Ingredients.All);
            //Act
            IEnumerable<Ingredient> ingredients = ingredientsService.GetAllIngredients();

            //Assert
            dataMock.Verify(x => x.Ingredients.All, Times.Once);
        }


        [Test]
        public void ReturnResult_WhenInvokingDataIngredientsRepositoryMethod_GetAll()
        {
            //Arrange
            var dataMock = new Mock<IHomeMadeFoodData>();
            var foodCategoriesServiceMock = new Mock<IFoodCategoriesService>();
            IngredientsService ingredientsService = new IngredientsService(dataMock.Object, foodCategoriesServiceMock.Object);
            IEnumerable<Ingredient> expectedResultCollection = new List<Ingredient>();

            dataMock.Setup(c => c.Ingredients.All).Returns(() =>
            {
                return expectedResultCollection.AsQueryable();
            });

            //Act
            IEnumerable<Ingredient> ingredientsResult = ingredientsService.GetAllIngredients();

            //Assert
            Assert.That(ingredientsResult, Is.EqualTo(expectedResultCollection));
        }

        [Test]
        public void ReturnResultOfCorrectTypeIEnumerableOfIngredient()
        {
            //Arrange
            var dataMock = new Mock<IHomeMadeFoodData>();
            var foodCategoriesServiceMock = new Mock<IFoodCategoriesService>();
            IngredientsService ingredientsService = new IngredientsService(dataMock.Object, foodCategoriesServiceMock.Object);
            dataMock.Setup(c => c.Ingredients.All).Returns(() =>
            {
                IEnumerable<Ingredient> expectedResultCollection = new List<Ingredient>();
                return expectedResultCollection.AsQueryable();
            });
            
            //Act
            IEnumerable<Ingredient> ingredientsResult = ingredientsService.GetAllIngredients();

            //Assert
            Assert.That(ingredientsResult, Is.InstanceOf<IEnumerable<Ingredient>>());
        }

        [Test]
        public void ReturnNull_WhenDataIngredientsReposityMethodGetAll_ReturnsNull()
        {
            //Arrange
            var dataMock = new Mock<IHomeMadeFoodData>();
            var foodCategoriesServiceMock = new Mock<IFoodCategoriesService>();
            IngredientsService ingredientsService = new IngredientsService(dataMock.Object, foodCategoriesServiceMock.Object);
            dataMock.Setup(c => c.Ingredients.All).Returns(() =>
            {
                return null;
            });
            
            //Act
            IEnumerable<Ingredient> ingredientsResult = ingredientsService.GetAllIngredients();

            //Assert
            Assert.IsNull(ingredientsResult);
        }
    }
}