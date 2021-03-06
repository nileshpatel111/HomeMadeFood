﻿using System;
using System.Collections.Generic;
using System.Linq;

using Bytes2you.Validation;

using HomeMadeFood.Data.Data;
using HomeMadeFood.Services.Data.Contracts;
using HomeMadeFood.Models;
using HomeMadeFood.Models.Enums;

namespace HomeMadeFood.Services.Data
{
    public class RecipesService : IRecipesService
    {
        private readonly IHomeMadeFoodData data;
        private readonly IIngredientsService ingredientsService;

        public RecipesService(IHomeMadeFoodData data, IIngredientsService ingredientsService)
        {
            Guard.WhenArgument(data, "data").IsNull().Throw();
            this.data = data;

            Guard.WhenArgument(ingredientsService, "ingredientsService").IsNull().Throw();
            this.ingredientsService = ingredientsService;
        }

        public IEnumerable<Recipe> GetAllRecipes()
        {
            var recipes = this.data.Recipes.All;
            if (recipes == null)
            {
                return null;
            }

            return recipes;
        }

        public Recipe GetRecipeById(Guid id)
        {
            Guard.WhenArgument(id, "id").IsEmptyGuid().Throw();

            var recipe = this.data.Recipes.GetById(id);

            if (recipe == null)
            {
                return null;
            }

            return recipe;
        }

        public void AddRecipe(Recipe recipe, 
            IEnumerable<string> ingredientNames, 
            IEnumerable<double> ingredientQuantities, 
            IEnumerable<decimal> ingredientPrices,
            IEnumerable<Guid> foodCategoriesIds)
        {
            Guard.WhenArgument(recipe, "recipe").IsNull().Throw();

            var ingredients = new List<Ingredient>();
            var ingredientsAsList = ingredientNames.ToList();
            var quantitiesAsList = ingredientQuantities.ToList();
            var pricesAsList = ingredientPrices.ToList();
            var foodCategoriesIdsAsList = foodCategoriesIds.ToList();
            var count = ingredientsAsList.Count;
            for (int i = 0; i < count; i++)
            {
                var name = ingredientsAsList[i].ToLower();
                var quantity = quantitiesAsList[i];
                var price = pricesAsList[i];
                var foodCategoryId = foodCategoriesIdsAsList[i];
                var ingredient = this.ingredientsService.CreateIngredient(name, foodCategoryId, price, quantity);
                recipe.Ingredients.Add(ingredient);
            }

            decimal costPercentage = 0.30m;
            decimal costPerPortion = this.CalculateRecipeCostPerPortion(recipe);
            decimal pricePerPortion = this.CalculateRecipePricePerPortion(costPerPortion, costPercentage);
            double quantityPerPortion = this.CalculateRecipeQuantityPerPortion(recipe);

            recipe.CostPerPortion = costPerPortion;
            recipe.PricePerPortion = pricePerPortion;
            recipe.QuantityPerPortion = quantityPerPortion;            

            this.data.Recipes.Add(recipe);
            this.data.SaveChanges();
        }

        public void DeleteRecipe(Recipe recipe)
        {
            Guard.WhenArgument(recipe, "recipe").IsNull().Throw();

            this.data.Recipes.Delete(recipe);
            this.data.SaveChanges();
        }

        public void EditRecipe(Recipe recipe)
        {
            Guard.WhenArgument(recipe, "recipe").IsNull().Throw();

            this.data.Recipes.Update(recipe);
            this.data.SaveChanges();
        }

        public IEnumerable<Recipe> GetAllOfDishType(DishType dishType)
        {
            var resultRecipes = this.data.Recipes
                .All
                .Where(x => x.DishType == dishType);

            if (resultRecipes == null)
            {
                return null;
            }

            return resultRecipes;
        }

        private decimal CalculateRecipeCostPerPortion(Recipe recipe)
        {
            Guard.WhenArgument(recipe, "recipe").IsNull().Throw();

            var costPerPotion = 0m;
            if (recipe.Ingredients != null && recipe.Ingredients.Count > 0)
            {
                costPerPotion = recipe.Ingredients
                    .ToList()
                    .Select(x => x.PricePerMeasuringUnit)
                    .Sum();
            }           

            return costPerPotion;
        }

        private double CalculateRecipeQuantityPerPortion(Recipe recipe)
        {
            Guard.WhenArgument(recipe, "recipe").IsNull().Throw();

            var quantityerPortion = recipe.Ingredients
                .Select(x => x.QuantityInMeasuringUnit)
                .Sum();

            return quantityerPortion;
        }

        private decimal CalculateRecipePricePerPortion(decimal costPerPortion, decimal costPercentage)
        {
            Guard.WhenArgument(costPerPortion, "costPerPortion").IsLessThan(0).Throw();
            Guard.WhenArgument(costPercentage, "costPercentage").IsLessThan(0).Throw();

            decimal pricePerPerPortion = (costPerPortion / costPercentage);

            return pricePerPerPortion;
        }
    }
}