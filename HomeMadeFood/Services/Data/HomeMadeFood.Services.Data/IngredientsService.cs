﻿using System;
using System.Collections.Generic;
using System.Linq;

using Bytes2you.Validation;

using HomeMadeFood.Data.Data;
using HomeMadeFood.Models;
using HomeMadeFood.Services.Data.Contracts;

namespace HomeMadeFood.Services.Data
{
    public class IngredientsService : IIngredientsService
    {
        private readonly IHomeMadeFoodData data;
        public IngredientsService(IHomeMadeFoodData data)
        {
            Guard.WhenArgument(data, "data").IsNull().Throw();
            this.data = data;
        }

        public void AddIngredient(string name, Guid foodCategoryId, decimal pricePerMeasuringUnit, double quantityPerMeasuringUnit, Guid recipeId)
        {
            Guard.WhenArgument(name, "name").IsNull().Throw();
            Guard.WhenArgument(foodCategoryId, "foodCategoryId").IsEmptyGuid().Throw();
            Guard.WhenArgument(recipeId, "recipeId").IsEmptyGuid().Throw();

            Ingredient ingredient = new Ingredient()
            {
                Name = name,
                FoodcategoryId = foodCategoryId,
                PricePerMeasuringUnit = pricePerMeasuringUnit,
                QuantityInMeasuringUnit = quantityPerMeasuringUnit,
                RecipeId = recipeId
            };

            this.data.Ingredients.Add(ingredient);
            this.data.Commit();
        }

        public IEnumerable<Ingredient> GetAllIngredients()
        {
            var ingredients = this.data.Ingredients.All;

            if (ingredients == null)
            {
                return null;
            }

            return ingredients;
        }

        public Ingredient GetIngredientById(Guid id)
        {
            Guard.WhenArgument(id, "id").IsEmptyGuid().Throw();

            var ingretient = this.data.Ingredients.GetById(id);

            if (ingretient == null)
            {
                return null;
            }

            return ingretient;
        }

        public void EditIngredient(Ingredient ingredient)
        {
            Guard.WhenArgument(ingredient, "ingredient").IsNull().Throw();

            this.data.Ingredients.Update(ingredient);
            this.data.Commit();
        }

        public void DeleteIngredient(Ingredient ingredient)
        {
            Guard.WhenArgument(ingredient, "ingredient").IsNull().Throw();

            this.data.Ingredients.Delete(ingredient);
            this.data.Commit();
        }

        public IEnumerable<Ingredient> GetAllIngredientsIncludingRecipes()
        {
            return this.data.Ingredients.GetAllIncluding(x => x.Recipe);
        }
    }
}