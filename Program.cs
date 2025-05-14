var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/" /*rota*/, () => "Hello World! 4");
app.MapPost("/", () => new {name = "Alyson", age = 24});
app.MapGet("/AddHeader", (HttpResponse respose) =>{
    respose.Headers.Add("Teste", "Alyson");
    return new {name = "Alyson", age = 24};
});

app.MapPost("/saveproduct", (Product product)=>{
    return product.Code + " - " + product.Name; 
});


app.Run(); 


public class Product{
    public string Code { get; set; }
    public string Name { get; set; }
}
