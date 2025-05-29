using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["Database:sqlserver"]);
var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);

// app.MapGet("/" /*rota*/, () => "Hello World! 4");
// app.MapPost("/", () => new {name = "Alyson", age = 24});
// app.MapGet("/AddHeader", (HttpResponse respose) =>{
//     respose.Headers.Add("Teste", "Alyson");
//     return new {name = "Alyson", age = 24};
// }); 

app.MapPost("/products", (ProductRequest productRequest, ApplicationDbContext context)=>
{
    var category = context.Categories.Where(c => c.Id == productRequest.CategoryId).First(); //chamada do category
    var product = new Product
    {
        Code = productRequest.Code,
        Name = productRequest.Name,
        Description = productRequest.Description,
        Category = category
    };
    context.Products.Add(product);
    context.SaveChanges();
     return Results.Created($"/products/{product.Id}", product.Id);
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
