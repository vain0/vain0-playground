﻿using System;
using FluentSqlBuilder.Public;

namespace FluentSqlBuilder.Detail
{
    public class FromlessSelectBuilder
    {
        SelectStatement Statement { get; }

        public FromlessSelectBuilder(SelectStatement statement)
        {
            Statement = statement;
        }

        #region From
        public FieldlessSelectBuilder From(ISqlExpression<IRelation> relation)
        {
            Statement.Source.Add(relation);
            return new FieldlessSelectBuilder(Statement);
        }
        #endregion
    }
}
