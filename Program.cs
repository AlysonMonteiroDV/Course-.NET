using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    if (productRequest.Tags != null)
    {
        product.Tags = new List<Tag>();
        foreach (var item in productRequest.Tags)
        {
            product.Tags.Add(new Tag{ Name = item});
        }
    }
    context.Products.Add(product);
    context.SaveChanges();
     return Results.Created($"/products/{product.Id}", product.Id);
});

app.MapGet("/products/{id}", ([FromRoute] int id, ApplicationDbContext context) =>
{
    var product = context.Products
    .Include(p => p.Category)
    .Include(p=> p.Tags)
    .Where(p=> p.Id == id).First();
    if (product != null)
    {
        return Results.Ok(product);   
    }
    return Results.NotFound();
    ;
});

app.MapPut("/products/{id}", ([FromRoute] int id, ProductRequest productRequest, ApplicationDbContext context) =>
{
    var product = context.Products
        .Include(p => p.Tags)
        .Where(p => p.Id == id).First();
    var category = context.Categories.Where(c => c.Id == productRequest.CategoryId).First();

    product.Code = productRequest.Code;
    product.Name = productRequest.Name;
    product.Description = productRequest.Description;
    product.Category = category;
    if (productRequest.Tags != null)
    {
        product.Tags = new List<Tag>();
        foreach (var item in productRequest.Tags)
        {
            product.Tags.Add(new Tag { Name = item });
        }
    }

    context.SaveChanges();

    return Results.Ok();
    
    });

app.MapDelete("/products/{ID}", ([FromRoute] int id, ApplicationDbContext context) =>
{
    var product = context.Products.Where(p => p.Id == id).First();
    context.Products.Remove(product);
    context.SaveChanges();
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
