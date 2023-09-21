using ApiPeliculas.Data;
using ApiPeliculas.Shared;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Feats.Categories.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext context;

    public CategoryRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<Category?> GetCategoryById(Guid id)
    {
        return await context.Categories.AsNoTracking()
                        .FirstOrDefaultAsync(cat => cat.Id == id);
    }

    public async Task<ICollection<Category>> GetCategories()
    {
        return await context.Categories
            .AsNoTracking()
            .OrderBy(cat => cat.Name)
            .ToListAsync();
    }

    public async Task<bool> CreateCategory(Category category)
    {
        category.CreationDate = DateTimeOffset.UtcNow;
        await context.AddAsync(category);
        return await Save();
    }

    public async Task<bool> UpdateCategory(Category category)
    {
        Category? categoryBd = await context.Categories.FindAsync(category.Id);
        if (categoryBd is null)
        {
            return false;
        }

        category.CreationDate = DateTimeOffset.UtcNow;
        context.Entry(categoryBd).CurrentValues.SetValues(category);

        return await Save();
    }

    public async Task<bool> DeleteCategory(Category category)
    {
        context.Categories.Remove(category);
        return await Save();
    }

    public bool CategoryExists(string name)
    {
        return context.Categories
            .AsNoTracking()
            .Any(cat => cat.Name.ToLower().Trim() == name.ToLower().Trim());
    }

    public bool CategoryExists(Guid id)
    {
        return context.Categories
            .AsNoTracking()
            .Any(cat => cat.Id == id);
    }

    public async Task<bool> Save()
    {
        return await context.SaveChangesAsync() != 0;
    }
}
