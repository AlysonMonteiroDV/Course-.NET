using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>();
var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);

// app.MapGet("/" /*rota*/, () => "Hello World! 4");
// app.MapPost("/", () => new {name = "Alyson", age = 24});
// app.MapGet("/AddHeader", (HttpResponse respose) =>{
//     respose.Headers.Add("Teste", "Alyson");
//     return new {name = "Alyson", age = 24};
// }); 

app.MapPost("/products", (Product product)=>
{
    ProductRepository.Add(product);
     return Results.Created($"/products/{ product.Code}", product.Code);
});

app.MapGet("/products/{code}", ([FromRoute] string code) =>
{
    var product = ProductRepository.GetBy(code);
    if (product != null)
    {
        return Results.Ok(product);   
    }
    return Results.NotFound();
    ;
});

app.MapPut("/products", (Product product) =>
{
    var productSaved = ProductRepository.GetBy(product.Code);
    productSaved.Name = product.Name;
    return Results.Ok();
    });

app.MapDelete("/products/{code}", ([FromRoute] string code) =>
{
    var productSaved = ProductRepository.GetBy(code);
    ProductRepository.Remove(productSaved);
    return Results.Ok();
});

if(app.Environment.IsStaging()){
    app.MapGet("/configuration/database", (IConfiguration configuration) =>
    {
        return Results.Ok($"{configuration["database:connection"]}/{configuration["database:port"]}");
    });
}

// //api.app.com/user?datastart={date}&datend={date}
// app.MapGet("/getproduct", ([FromQuery]string dateStart,[FromQuery] string dateEnd) =>
// {
//     return dateStart + "--" + dateEnd;
// });
//api.app.com/user/{code}


app.Run();


public class ProductRepository
{
    public static List<Product> Products { get; set; }  = Products = new List<Product>();

    public static void Init(IConfiguration configuration)
    {
        var products = configuration.GetSection("Products").Get<List<Product>>();
        Products = products;

    }

    public static void Add(Product product)
    {
        if (Products == null)
        {
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

//onde fica as configurações para banco de dados
//CLASSE CONFIGURADORA
public class ApplicationDbContext : DbContext
{
    //faz o mapeamento para fazer a class "Product" virar uma tabela
    // no braco de dados
    public DbSet<Product> Products { get; set; }


    // conectar no banco

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
        options.UseSqlServer("Server=localhost;Database=Products;User Id=sa;PassWord=@sql12019;MultipleActiveResultSets=true;Encrypt=YES;TrustServerCertificate=YES");
}
    
}