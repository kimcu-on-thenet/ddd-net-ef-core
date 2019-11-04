﻿using AutoFixture;
using DDDEfCore.Infrastructures.EfCore.Common.Extensions;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestCatalogProduct
{
    public class TestCatalogProductFixture : SharedFixture
    {
        public Catalog Catalog { get; private set; }
        public Category Category { get; private set; }
        public Product Product { get; private set; }
        public CatalogCategory CatalogCategory { get; private set; }
        public CatalogProduct CatalogProduct { get; private set; }


        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            this.Category = Category.Create(this.Fixture.Create<string>());
            await this.SeedingData(this.Category);

            this.Product = Product.Create(this.Fixture.Create<string>());
            await this.SeedingData(this.Product);

            this.Catalog = Catalog.Create(this.Fixture.Create<string>());

            this.CatalogCategory =
                this.Catalog.AddCategory(this.Category.CategoryId, this.Fixture.Create<string>());

            this.CatalogProduct =
                this.CatalogCategory.CreateCatalogProduct(this.Product.ProductId, this.Fixture.Create<string>());

            await this.RepositoryExecute<Catalog>(async repository => { await repository.AddAsync(this.Catalog); });
        }

        public async Task DoActionWithCatalogProduct(Action<CatalogProduct> action)
        {
            await this.RepositoryExecute<Catalog>(async repository =>
            {
                var catalog = await repository
                    .FindOneWithIncludeAsync(x => x.CatalogId == this.Catalog.CatalogId,
                        x => x.Include(c => c.Categories)
                            .ThenInclude(c => c.Products));

                var catalogCategory = catalog.Categories.SingleOrDefault(x => x == this.CatalogCategory);

                var catalogProduct = catalogCategory.Products.SingleOrDefault(x => x == this.CatalogProduct);

                action(catalogProduct);

                await repository.UpdateAsync(catalog);
            });
        }

        public async Task DoAssertForCatalogProduct(Action<CatalogProduct> action)
        {
            await this.RepositoryExecute<Catalog>(async repository =>
            {
                var catalog = await repository
                    .FindOneWithIncludeAsync(x => x.CatalogId == this.Catalog.CatalogId,
                        x => x.Include(c => c.Categories)
                            .ThenInclude(c => c.Products));

                var catalogCategory = catalog.Categories.SingleOrDefault(x => x == this.CatalogCategory);

                var catalogProduct = catalogCategory.Products.SingleOrDefault(x => x == this.CatalogProduct);

                action(catalogProduct);
            });
        }
    }
}
