public class Product
{
    public int Id { get; set; }


    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public int CategoryId { get; set; }//deixa a chave estrangeira como not null, se colocar um interogação apos o int ele fica null novamente

    public Category Category { get; set; }
    public List<Tag> Tags { get; set; }
}
