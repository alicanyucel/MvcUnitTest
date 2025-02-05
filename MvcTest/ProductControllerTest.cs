using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication3.Controllers;
using WebApplication3.Models;
using WebApplication3.Repositories;

namespace MvcTest;

public class ProductControllerTest
{
    private readonly Mock<IRepository<Product>> _mockrepository;
    private readonly ProductsController _controller;
    private List<Product> _products;
    public ProductControllerTest()
    {
        _mockrepository = new Mock<IRepository<Product>>();
        _controller = new ProductsController(_mockrepository.Object);
        _products = new List<Product>()
        {
            new Product { Id = 1,Name="kalem",Price=35,Color="red"
        },

        {
            new Product{Id=2,Name="defter",Price=44,Color="blue"}
        }
        };
    }
    [Fact]
    public async void Index_ActionExecutes_ReturnView()
    {
        var result=await _controller.Index();
        Assert.IsType<ViewResult>(result);
    }
    [Fact]
    public async void Index_ActionExecutes_ReturnProductList()
    {
        _mockrepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_products);
        var result=await _controller.Index();
        var viewResult=Assert.IsType<ViewResult>(result);
        var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);
        Assert.Equal<int>(2,productList.Count());

    }
}
