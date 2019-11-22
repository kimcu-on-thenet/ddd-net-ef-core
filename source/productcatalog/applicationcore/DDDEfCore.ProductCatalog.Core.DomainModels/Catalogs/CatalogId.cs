﻿using System;
using DDDEfCore.Core.Common.Models;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs
{
    public class CatalogId : IdentityBase
    {
        #region Constructors

        public CatalogId(Guid id) : base(id) { }

        public CatalogId() : base() { }

        #endregion

        public static explicit operator CatalogId(Guid id) => id == Guid.Empty ? null : new CatalogId(id);
    }
}
