using CRN.Application.DTOs;

namespace CRN.Application.Interfaces;

public interface IProductService
{
    Task<ProductDto> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PagedResult<ProductDto>> GetPagedAsync(int pageNumber, int pageSize, string? search, CancellationToken ct = default);
    Task<ProductDto> CreateAsync(CreateProductDto dto, string createdBy, CancellationToken ct = default);
    Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto, string modifiedBy, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

public interface ITokenService
{
    AuthResponseDto GenerateToken(string username);
}
