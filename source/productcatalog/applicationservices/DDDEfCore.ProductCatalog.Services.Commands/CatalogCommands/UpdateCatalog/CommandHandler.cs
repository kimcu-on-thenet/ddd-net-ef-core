﻿using DDDEfCore.Core.Common;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.UpdateCatalog
{
    public class CommandHandler : AsyncRequestHandler<UpdateCatalogCommand>
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IRepository<Catalog> _repository;
        private readonly AbstractValidator<UpdateCatalogCommand> _validator;

        public CommandHandler(IRepositoryFactory repositoryFactory, AbstractValidator<UpdateCatalogCommand> validator)
        {
            this._repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            this._repository = this._repositoryFactory.CreateRepository<Catalog>();
            this._validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        #region Overrides of AsyncRequestHandler<UpdateCatalogCommand>

        protected override async Task Handle(UpdateCatalogCommand request, CancellationToken cancellationToken)
        {
            await this._validator.ValidateAndThrowAsync(request, null, cancellationToken);

            var catalog = await this._repository.FindOneAsync(x => x.CatalogId == request.CatalogId);

            catalog.ChangeDisplayName(request.CatalogName);

            await this._repository.UpdateAsync(catalog);
        }

        #endregion
    }
}
//TODO: Missing UnitTest