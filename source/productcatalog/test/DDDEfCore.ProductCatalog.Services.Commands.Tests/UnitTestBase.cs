﻿using AutoFixture;
using DDDEfCore.Core.Common;
using DDDEfCore.Core.Common.Models;
using Moq;
using System.Threading;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests
{
    public abstract class UnitTestBase<TAggregateRoot, TIdentity> where TAggregateRoot : AggregateRoot<TIdentity> where TIdentity : IdentityBase
    {
        protected readonly IFixture Fixture;
        protected readonly CancellationToken CancellationToken;
        protected readonly Mock<IRepositoryFactory> MockRepositoryFactory;
        protected readonly Mock<IRepository<TAggregateRoot, TIdentity>> MockRepository;

        protected UnitTestBase()
        {
            this.Fixture = new Fixture();
            
            this.MockRepositoryFactory = new Mock<IRepositoryFactory>();
            
            this.MockRepository = new Mock<IRepository<TAggregateRoot, TIdentity>>();

            this.MockRepositoryFactory
                .Setup(x => x.CreateRepository<TAggregateRoot, TIdentity>())
                .Returns(this.MockRepository.Object);

            this.CancellationToken = new CancellationToken(false);
        }
    }
}
