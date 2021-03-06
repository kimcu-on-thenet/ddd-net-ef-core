﻿using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using FluentValidation;

namespace DDDEfCore.ProductCatalog.Services.Queries.ProductQueries.GetProductDetail
{
    public class GetProductDetailRequestValidator : AbstractValidator<GetProductDetailRequest>
    {
        public GetProductDetailRequestValidator()
        {
            RuleFor(x => x.ProductId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .NotEqual(ProductId.Empty);
        }
    }
}
