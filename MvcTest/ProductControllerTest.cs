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
        var result = await _controller.Index();
        Assert.IsType<ViewResult>(result);
    }
    [Fact]
    public async void Index_ActionExecutes_ReturnProductList()
    {
        _mockrepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_products);
        var result = await _controller.Index();
        var viewResult = Assert.IsType<ViewResult>(result);
        var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);
        Assert.Equal<int>(2, productList.Count());
    }
    [Fact]
    public async void Details_IdIsNull_ReturnRedirectToIndexAction()
    {
        var result = await _controller.Details(null);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }
    [Fact]
    public async void Details_IdInValid_ReturnNotFound()
    {
        Product product = null;
        _mockrepository.Setup(x => x.GetByIdAsync(0)).ReturnsAsync(product);
        var result = await _controller.Details(0);
        var redirect = Assert.IsType<NotFoundResult>(result);
        Assert.Equal<int>(404, redirect.StatusCode);
    }
    [Theory]
    [InlineData(1)]
    public async void Details_ValidId_ReturnProduct(int productId)
    {
        Product product = _products.First(x => x.Id == productId);
        _mockrepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);
        var result = await _controller.Details(productId);
        var viewResult = Assert.IsType<ViewResult>(result);
        var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);
        Assert.Equal(product.Id, resultProduct.Id);
        Assert.Equal(product.Name, resultProduct.Name);
    }
    [Fact]
    public void Create_ActionExecute_ReturnView()
    {
        var result = _controller.Create();
        Assert.IsType<ViewResult>(result);
    }
    [Fact]
    public async void CreatePost_InvalidModelState_ReturnView()
    {
        _controller.ModelState.AddModelError("Name", "Name alanı gereklidir");
        var result = await _controller.Create(_products.First());
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<Product>(viewResult.Model);
    }
    [Fact]
    public async void CreatePost_ValidModelState_ReturnRedirectToAction()
    {
        var result = await _controller.Create(_products.First());
        var rediret = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", rediret.ActionName);
    }
    [Fact]
    public async void CreatePost_ValidModelState_CreateMethodExecute()
    {
        Product newproduct = null;
        _mockrepository.Setup(repo => repo.Create(It.IsAny<Product>())).Callback<Product>(x => newproduct = x);
        var result = await _controller.Create(_products.First());
        _mockrepository.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Once);
        Assert.Equal(_products.First().Id, newproduct.Id);
    }
    [Fact]
    public async void CreatePost_InValidModelState_NeverCreateExeCute()
    {
        _controller.ModelState.AddModelError("Name","name alanı gereklidir");
        var result=await _controller.Create(_products.First());
        _mockrepository.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Never);
    }
    // edit testler
    [Fact]
    public async void Edit_IdIsNull_ReturnRedirectToIndexAction()
    {
        var result = await _controller.Edit(null);
        var rediret=Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index",rediret.ActionName);
    }
    [Theory]// DATA ALDIGI İÇİN FACT DEĞİL
    [InlineData(3)]
    public async void Edit_IdInvalid_ReturnNotFound(int productId)
    {
        Product product = null;
        _mockrepository.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);
        var result= await _controller.Edit(productId);
        var redirect=Assert.IsType<NotFoundResult>(result);
        Assert.Equal(404,redirect.StatusCode);
    }
    [Theory]
    [InlineData(2)]
    public async void Edit_ActionExecute_ReturnProduct(int productId)
    {
        var product=_products.First(x=>x.Id==productId);
        _mockrepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);
        var result=await _controller.Edit(productId);
        var viewResult=Assert.IsType<ViewResult>(result);
        var resultProduct=Assert.IsAssignableFrom<Product>(viewResult.Model);
        Assert.Equal(product.Id,resultProduct.Id);
        Assert.Equal(product.Name,resultProduct.Name);
    }
    [Theory]
    [InlineData(1)]
    public void EditPost_IdIsNotEqualProduct_ReturnMotFound(int productId)
    {
        var result=_controller.Edit(2,_products.First(x=>x.Id==productId));
        var redirect = Assert.IsType<NotFoundResult>(result);
    }
    [Theory]
    [InlineData(1)]
    public void EditPost_InvalidModelState_ReturnView(int productId)
    {
        _controller.ModelState.AddModelError("Name","");
        var result=_controller.Edit(productId,_products.First(x=>x.Id==productId));
        var viewResult=Assert.IsType<ViewResult>(result);
        Assert.IsType<Product>(viewResult.Model);
    }
    [Theory]
    [InlineData(1)]
    public void EditPost_ValidModelState_ReturnRedirecToAction(int productId)
    {
        var result=_controller.Edit(productId,_products.First(x=>x.Id == productId));
        var redirect=Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index",redirect.ActionName);
    }
    [Theory]
    [InlineData(1)]
    public void EditPost_ValidModelState_UpdateMetodExecute(int productId)
    {
        var product=_products.First(x=>x.Id==productId);
        _mockrepository.Setup(repo => repo.Update(product));
        _controller.Edit(productId,product);
        _mockrepository.Verify(repo=>repo.Update(It.IsAny<Product>()),Times.Once);
    }
}