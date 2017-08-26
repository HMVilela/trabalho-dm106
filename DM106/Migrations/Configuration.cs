namespace DM106.Migrations
{
    using DM106.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DM106.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "DM106.Models.ApplicationDbContext";
        }

        protected override void Seed(DM106.Models.ApplicationDbContext context)
        {
            context.Products.AddOrUpdate(
                p => p.Id,
                new Product
                {
                    Id = 1,
                    nome = "Produto 1",
                    descricao = "Este é o produto 1",
                    cor = "cor do produto 1",
                    modelo = "modelo do produto 1",
                    codigo = "COD1",
                    preco = "10",
                    peso = "1",
                    altura = "2",
                    largura = "11",
                    comprimento = "16",
                    diametro = "1",
                    imagem = "URL da imagem do produto 1"
                },
                new Product
                {
                    Id = 2,
                    nome = "Produto 2",
                    descricao = "Este é o produto 2",
                    cor = "cor do produto 2",
                    modelo = "modelo do produto 2",
                    codigo = "COD2",
                    preco = "20",
                    peso = "2",
                    altura = "2",
                    largura = "12",
                    comprimento = "17",
                    diametro = "2",
                    imagem = "URL da imagem do produto 2"
                },
                new Product
                {
                    Id = 3,
                    nome = "Produto 3",
                    descricao = "Este é o produto 3",
                    cor = "cor do produto 3",
                    modelo = "modelo do produto 3",
                    codigo = "COD3",
                    preco = "30",
                    peso = "3",
                    altura = "3",
                    largura = "13",
                    comprimento = "18",
                    diametro = "3",
                    imagem = "URL da imagem do produto 3"
                },
                new Product
                {
                    Id = 4,
                    nome = "Produto 4",
                    descricao = "Este é o produto 4",
                    cor = "cor do produto 4",
                    modelo = "modelo do produto 4",
                    codigo = "COD4",
                    preco = "40",
                    peso = "4",
                    altura = "4",
                    largura = "14",
                    comprimento = "19",
                    diametro = "4",
                    imagem = "URL da imagem do produto 4"
                },
                new Product
                {
                    Id = 5,
                    nome = "Produto 5",
                    descricao = "Este é o produto 5",
                    cor = "cor do produto 5",
                    modelo = "modelo do produto 5",
                    codigo = "COD5",
                    preco = "50",
                    peso = "5",
                    altura = "5",
                    largura = "15",
                    comprimento = "20",
                    diametro = "5",
                    imagem = "URL da imagem do produto 5"
                });
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
