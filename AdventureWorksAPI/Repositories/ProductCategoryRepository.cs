using Microsoft.EntityFrameworkCore.ChangeTracking;
using AdventureWorksNS.Data;
using System.Collections.Concurrent;

namespace AdventureWorksAPI.Repositories
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private static ConcurrentDictionary<int, ProductCategory>? producCategoryCache;
        //Comentario, puede usar Redis para un cache mas eficiente ==> Open Source
        private AdventureWorksDB db;

        public ProductCategoryRepository(AdventureWorksDB injectedDB)
        {
            db = injectedDB;
            if (producCategoryCache is null)
            {
                producCategoryCache = new ConcurrentDictionary<int, ProductCategory>(
                    db.ProductCategories.ToDictionary(p => p.ProductCategoryId));
            }
        }

        public async Task<ProductCategory> CreateAsync(ProductCategory p)
        {
            EntityEntry<ProductCategory> agregado = await db.ProductCategories.AddAsync(p);
            int afectados = await db.SaveChangesAsync();
            if (afectados == 1)
            {
                if (producCategoryCache is null) return p;
                return producCategoryCache.AddOrUpdate(p.ProductCategoryId, p, UpdateCache);
            }
            return null!;
        }
        private ProductCategory UpdateCache(int id, ProductCategory p)
        {
            ProductCategory? antiguo;
            if (producCategoryCache is not null)
            {
                if (producCategoryCache.TryGetValue(id, out antiguo))
                {
                    if (producCategoryCache.TryUpdate(id, p, antiguo))
                    {
                        return p;
                    }
                }
            }
            return null!;
        }
        public Task<IEnumerable<ProductCategory>> RetrieveAllAsync()
        {
            return Task.FromResult(producCategoryCache is null ?
                Enumerable.Empty<ProductCategory>() : producCategoryCache.Values);
        }

        public Task<ProductCategory?> RetrieveAsync(int id)
        {
            if (producCategoryCache is null) return null!;
            producCategoryCache.TryGetValue(id, out ProductCategory? p);
            return Task.FromResult(p);
        }
        public async Task<ProductCategory?> UpdateAsync(int id, ProductCategory p)
        {
            db.ProductCategories.Update(p);
            int afectados = await db.SaveChangesAsync();
            if (afectados == 1)
            {
                return UpdateCache(id, p);
            }
            return null;
        }
        public async Task<bool?> DeleteAsync(int id)
        {
            ProductCategory? p = db.ProductCategories.Find(id);
            if (p is null) return false;
            db.ProductCategories.Remove(p);
            int afectados = await db.SaveChangesAsync();
            if (afectados == 1)
            {
                if (producCategoryCache is null) return null;
                return producCategoryCache.TryRemove(id, out p);
            }
            else
            {
                return null;
            }
        }

    }
}
