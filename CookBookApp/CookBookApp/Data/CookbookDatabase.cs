using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;

namespace CookBookApp
{
    public class CookbookDatabase
    {
        readonly SQLiteAsyncConnection database;

        public CookbookDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<Categories>().Wait();
            database.CreateTableAsync<Recipes>().Wait();
            database.CreateTableAsync<Ingredients>().Wait();
            database.CreateTableAsync<ShoppingListItems>().Wait();
        }

        public Task<List<Categories>> GetCategories()
        {
            return database.Table<Categories>().ToListAsync();
        }

        public bool CategoryExists()
        {
            bool exists = false;
            if (database.QueryAsync<Categories>("SELECT * FROM [Categories]").Result.FirstOrDefault() != null)
            {
                exists = true;
            }
            return exists;
        }

        public Task<List<Recipes>> GetRecipes(int categoryId)
        {
            return database.QueryAsync<Recipes>("SELECT * FROM [Recipes] WHERE [categoryId] = " + categoryId);
        }

        public int GetRecentRecipeID()
        {
            int result = database.QueryAsync<Recipes>("SELECT [recipeId] FROM [Recipes] ORDER BY [recipeId] DESC limit 1").Result.FirstOrDefault().recipeId;

            return result;
        }

        public Task<List<Ingredients>> GetIngredients(int recipeId)
        {
            return database.QueryAsync<Ingredients>("SELECT * FROM [Ingredients] WHERE [recipeId] = " + recipeId);
        }

        public Task<List<ShoppingListItems>> GetList()
        {
            return database.QueryAsync<ShoppingListItems>("SELECT * FROM [ShoppingListItems]");
        }

        public Task<int> SaveCategoryAsync(Categories item)
        {
            if (item.categoryId != 0)
            {
                return database.UpdateAsync(item);
            }
            else
            {
                return database.InsertAsync(item);
            }
        }

        public Task<int> SaveRecipeAsync(Recipes item)
        {
            if (item.recipeId != 0)
            {
                return database.UpdateAsync(item);
            }
            else
            {
                return database.InsertAsync(item);
            }
        }

        public Task<int> SaveIngredientAsync(Ingredients item)
        {
            if (item.ingredientId != 0)
            {
                return database.UpdateAsync(item);
            }
            else
            {
                return database.InsertAsync(item);
            }
        }

        public Task<int> SaveShoppingListAsync(ShoppingListItems item)
        {
            if (item.itemId != 0)
            {
                return database.UpdateAsync(item);
            }
            else
            {
                ShoppingListItems existingItem = database.QueryAsync<ShoppingListItems>("SELECT * FROM [ShoppingListItems] WHERE [itemName] = '" + item.itemName
                    + "' AND [itemMeas] = '" + item.itemMeas + "'").Result.FirstOrDefault();
                if (existingItem != null)
                {
                    item.itemAmou = item.itemAmou + existingItem.itemAmou;
                    DeleteShoppingListAsync(existingItem);
                }

                return database.InsertAsync(item);             
            }
        }

            public Task<int> DeleteCategoryAsync(Categories item)
        {
            database.ExecuteAsync("UPDATE [Recipes] SET [categoryId] = 1 WHERE [categoryId] = " + item.categoryId);
            return database.DeleteAsync(item);
        }

        public Task<int> DeleteRecipeAsync(Recipes item)
        {
            return database.DeleteAsync(item);
        }

        public Task<int> DeleteIngredientAsync(Ingredients item)
        {
            return database.DeleteAsync(item);
        }

        public Task<int> DeleteShoppingListAsync()
        {
            return database.ExecuteAsync("DELETE FROM[ShoppingListItems] WHERE [itemChecked] = 1");
        }

        public Task<int> DeleteShoppingListAsync(ShoppingListItems item)
        {
            return database.DeleteAsync(item);
        }
    }
}