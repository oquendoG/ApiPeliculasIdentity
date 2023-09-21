using ApiPeliculas.Shared;

namespace ApiPeliculas.Feats.Categories.Repository;

public interface ICategoryRepository
{
    bool CategoryExists(string name);
    bool CategoryExists(Guid id);
    Task<bool> CreateCategory(Category category);
    Task<bool> DeleteCategory(Category category);
    Task<ICollection<Category>> GetCategories();
    Task<Category?> GetCategoryById(Guid id);
    Task<bool> Save();
    Task<bool> UpdateCategory(Category category);
}
