﻿using DDDEfCore.Infrastructures.EfCore.Common.Extensions;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Tests.TestCatalog
{
    [Collection(nameof(SharedFixture))]
    public class TestCatalogRepository : IClassFixture<TestCatalogFixture>
    {
        private readonly TestCatalogFixture _testFixture;

        public TestCatalogRepository(TestCatalogFixture testFixture)
            => this._testFixture = testFixture ?? throw new ArgumentNullException(nameof(testFixture));

        [Fact(DisplayName = "Create Catalog with Chain of Catalog-Categories Successfully")]
        public async Task Create_Catalog_With_Chain_Of_CatalogCategories_Successfully()
        {
            await this._testFixture.InitData();

            await this._testFixture.DoAssert(catalog =>
            {
                catalog.ShouldNotBeNull();
                catalog.Equals(this._testFixture.Catalog).ShouldBeTrue();
                catalog
                    .Categories
                    .Except(this._testFixture.CatalogCategories)
                    .Any()
                    .ShouldBeFalse();

                var roots = catalog.FindCatalogCategoryRoots();
                roots.ShouldHaveSingleItem();

                var descendantsOfLv1 = catalog.GetDescendantsOfCatalogCategory(this._testFixture.CatalogCategoryLv1);
                descendantsOfLv1
                    .Except(this._testFixture.CatalogCategories)
                    .Any()
                    .ShouldBeFalse();

                var descendantsOfLv2 = catalog.GetDescendantsOfCatalogCategory(this._testFixture.CatalogCategoryLv2);
                descendantsOfLv2
                    .Except(this._testFixture.CatalogCategories.Where(x => x != this._testFixture.CatalogCategoryLv1))
                    .Any()
                    .ShouldBeFalse();

                var descendantsOfLv3 = catalog.GetDescendantsOfCatalogCategory(this._testFixture.CatalogCategoryLv3);
                descendantsOfLv3
                    .Except(this._testFixture.Catalog.Categories.Where(x => x != this._testFixture.CatalogCategoryLv1 && x != this._testFixture.CatalogCategoryLv2))
                    .Any().ShouldBeFalse();
            });
        }

        [Fact(DisplayName = "Catalog Should Remove Chain of CatalogCategories Successfully")]
        public async Task Catalog_Should_Remove_Chain_of_CatalogCategories_Successfully()
        {
            await this._testFixture.InitData();

            await this._testFixture.RepositoryExecute(async repository =>
            {
                var catalog = await repository
                    .FindOneWithIncludeAsync(x => x.CatalogId == this._testFixture.Catalog.CatalogId,
                        x => x.Include(y => y.Categories));

                var catalogCategoryLv1 = catalog.FindCatalogCategoryRoots().FirstOrDefault();
                catalogCategoryLv1.ShouldBe(this._testFixture.CatalogCategoryLv1);

                catalog.RemoveCatalogCategoryWithDescendants(catalogCategoryLv1);

                await repository.UpdateAsync(catalog);
            });

            await this._testFixture.DoAssert(catalog =>
            {
                catalog.ShouldNotBeNull();
                catalog.Categories.ShouldBeEmpty();
            });
        }

        [Fact(DisplayName = "Remove Catalog Within CatalogCategories Successfully")]
        public async Task RemoveCatalog_Within_CatalogCategories_Successfully()
        {
            await this._testFixture.InitData();

            await this._testFixture.RepositoryExecute(async repository =>
            {
                var catalog = await repository
                    .FindOneWithIncludeAsync(x => x.CatalogId == this._testFixture.Catalog.CatalogId,
                        x => x.Include(y => y.Categories));

                await repository.RemoveAsync(catalog);
            });

            await this._testFixture.DoAssert(catalog =>
            {
                catalog.ShouldBeNull();
            });
        }
    }
}