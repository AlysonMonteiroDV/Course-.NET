using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// app.MapGet("/" /*rota*/, () => "Hello World! 4");
// app.MapPost("/", () => new {name = "Alyson", age = 24});
// app.MapGet("/AddHeader", (HttpResponse respose) =>{
//     respose.Headers.Add("Teste", "Alyson");
//     return new {name = "Alyson", age = 24};
// }); 

app.MapPost("/products", (Product product)=>{
    ProductRepository.Add(product); 
});

app.MapGet("/products/{code}", ([FromRoute] string code) =>
{
    var product = ProductRepository.GetBy(code);
    return product;
});

app.MapPut("/products", (Product product) =>
{
    var productSaved = ProductRepository.GetBy(product.Code);
    productSaved.Name = product.Name;
    });

app.MapDelete("/products/{code}", ([FromRoute] string code) =>
{
    var productSaved = ProductRepository.GetBy(code);
    ProductRepository.Remove(productSaved);
    return "Produto deletado";
});

// //api.app.com/user?datastart={date}&datend={date}
// app.MapGet("/getproduct", ([FromQuery]string dateStart,[FromQuery] string dateEnd) =>
// {
//     return dateStart + "--" + dateEnd;
// });
//api.app.com/user/{code}


app.Run();


public class ProductRepository
{
    public static List<Product> Products { get; set; }

    public static void Add(Product product)
    {
        if (Products == null)
        {
            Products = new List<Product>();

            Products.Add(product);
        }
    }

    public static Product GetBy(string code) {
        return Products.FirstOrDefault(p => p.Code == code);
    }
    
    
        public static void Remove(Product product) {
            Products.Remove(product);
        }
    }



public class Product
{
    public string Code { get; set; }
    public string Name { get; set; }
}
