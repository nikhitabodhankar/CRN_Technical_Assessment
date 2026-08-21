using AutoMapper;
using CRN.Application.DTOs;
using CRN.Application.Interfaces;
using CRN.Application.Mapping;
using CRN.Application.Services;
using CRN.Domain.Entities;
using CRN.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace CRN.Application.Tests;

public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IProductRepository> _repoMock = new();
    private readonly IMapper _mapper;
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _unitOfWorkMock.Setup(u => u.Products).Returns(_repoMock.Object);
        _sut = new ProductService(_unitOfWorkMock.Object, _mapper);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ReturnsMappedDto()
    {
        var product = new Product { Id = 1, ProductName = "Laptop", Price = 999.99m, CreatedBy = "tester" };
        _repoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(product);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result.ProductName.Should().Be("Laptop");
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ThrowsNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(99, default)).ReturnsAsync((Product?)null);

        var act = async () => await _sut.GetByIdAsync(99);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_AddsProductAndSavesChanges()
    {
        var dto = new CreateProductDto { ProductName = "Mouse", Price = 19.99m };

        var result = await _sut.CreateAsync(dto, "tester");

        result.ProductName.Should().Be("Mouse");
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Product>(), default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductDoesNotExist_ThrowsNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(5, default)).ReturnsAsync((Product?)null);

        var act = async () => await _sut.DeleteAsync(5);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetPagedAsync_NormalizesInvalidPageSize()
    {
        _repoMock.Setup(r => r.GetPagedAsync(1, 10, null, default))
            .ReturnsAsync((new List<Product>(), 0));

        var result = await _sut.GetPagedAsync(0, 500, null);

        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
    }
}
