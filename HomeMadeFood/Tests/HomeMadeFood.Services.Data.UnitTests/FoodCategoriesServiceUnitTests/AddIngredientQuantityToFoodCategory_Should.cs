﻿using System;

using Moq;
using NUnit.Framework;

using HomeMadeFood.Data.Data;
using HomeMadeFood.Models;

namespace HomeMadeFood.Services.Data.UnitTests.FoodCategoriesServiceUnitTests
{
    [TestFixture]
    public class AddIngredientQuantityToFoodCategory_Should
    {
        [Test]
        public void Throw_WhenThePassedParameter_Ingredient_IsNull()
        {
            //Arrange
            var dataMock = new Mock<IHomeMadeFoodData>();
            FoodCategoriesService foodCategoriesService = new FoodCategoriesService(dataMock.Object);
            Ingredient ingredient = null;

            //Act & Assert
            Assert.Throws<ArgumentNullException>(() => foodCategoriesService.AddIngredientQuantityToFoodCategory(ingredient));
        }

        [Test]
        public void AddIngredientsQuantityToFoodCategory()
        {
            //Arrange
            var dataMock = new Mock<IHomeMadeFoodData>();
            FoodCategoriesService foodCategoriesService = new FoodCategoriesService(dataMock.Object);
            var ingredientId = Guid.NewGuid();
            var ingredientName = "Ingredient name";
            var quantityInMeasuringUnit = 1.5;
            var foodCategoryId = Guid.NewGuid();
            Ingredient ingredient = new Ingredient()
            {
                Id = ingredientId,
                Name = ingredientName,
                FoodCategoryId = foodCategoryId,
                QuantityInMeasuringUnit = quantityInMeasuringUnit
            };

            var foodCategoryName = "Food Category Name";
            var quantityOfAllIngredientsInTheCategory = 0;
            FoodCategory foodCategory = new FoodCategory()
            {
                Id = foodCategoryId,
                Name = foodCategoryName,
                QuantityOfAllCategoryIngredients = quantityOfAllIngredientsInTheCategory

            };

            dataMock.Setup(x => x.FoodCategories.GetById(foodCategoryId)).Returns(foodCategory);

            //Act
            foodCategoriesService.AddIngredientQuantityToFoodCategory(ingredient);

            //Assert
            Assert.AreEqual(ingredient.QuantityInMeasuringUnit, foodCategory.QuantityOfAllCategoryIngredients);
        }

        [Test]
        public void CallDataCommitMethodOnce_WhenParameterIngredientIsValid()
        {
            //Arrange
            var dataMock = new Mock<IHomeMadeFoodData>();
            FoodCategoriesService foodCategoriesService = new FoodCategoriesService(dataMock.Object);
            var ingredientId = Guid.NewGuid();
            var ingredientName = "Ingredient name";
            var quantityInMeasuringUnit = 1.5;
            var foodCategoryId = Guid.NewGuid();
            Ingredient ingredient = new Ingredient()
            {
                Id = ingredientId,
                Name = ingredientName,
                FoodCategoryId = foodCategoryId,
                QuantityInMeasuringUnit = quantityInMeasuringUnit
            };

            var foodCategoryName = "Food Category Name";
            var quantityOfAllIngredientsInTheCategory = 0;
            FoodCategory foodCategory = new FoodCategory()
            {
                Id = foodCategoryId,
                Name = foodCategoryName,
                QuantityOfAllCategoryIngredients = quantityOfAllIngredientsInTheCategory

            };

            dataMock.Setup(x => x.FoodCategories.GetById(foodCategoryId)).Returns(foodCategory);

            //Act
            foodCategoriesService.AddIngredientQuantityToFoodCategory(ingredient);

            //Assert
            dataMock.Verify(x => x.SaveChanges(), Times.Once);
        }
    }
}