﻿using System;
using System.Data.Common;
using FluentSqlBuilder.Accessor;

namespace FluentSqlBuilder.SqlSyntax
{
    public sealed class FieldlessSelectBuilder
    {
        SelectStatement Statement { get; }
        
        internal FieldlessSelectBuilder(SelectStatement statement)
        {
            Statement = statement;
        }

        #region Join
        JoinBuilder<FieldlessSelectBuilder> Join(
            RelationSqlExpression relation,
            JoinType joinType
        )
        {
            return
                new JoinBuilder<FieldlessSelectBuilder>(
                    Statement.SqlBuilder,
                    joinType,
                    relation,
                    join =>
                    {
                        Statement.Join(join);
                        return this;
                    }
                );
        }

        public JoinBuilder<FieldlessSelectBuilder> Join(RelationSqlExpression relation) =>
            Join(relation, JoinType.Inner);
        #endregion

        #region Where
        public FieldlessSelectBuilder Where(SqlCondition condition)
        {
            Statement.WhereCondition.Add(condition);
            return this;
        }
        #endregion

        #region GroupBy
        public FieldlessSelectBuilder GroupBy<X>(ScalarSqlExpression<X> expression)
        {
            Statement.GroupKeys.Add(expression);
            return this;
        }
        #endregion

        #region OrderBy
        FieldlessSelectBuilder OrderByImpl<X>(
            ScalarSqlExpression<X> expression,
            OrderDirection direction
        )
        {
            Statement.OrderKeys.Add(new OrderKey(expression, direction));
            return this;
        }

        public FieldlessSelectBuilder OrderBy<X>(ScalarSqlExpression<X> column) =>
            OrderByImpl(column, OrderDirection.Ascending);

        public FieldlessSelectBuilder OrderByDescending<X>(ScalarSqlExpression<X> column) =>
            OrderByImpl(column, OrderDirection.Descending);
        #endregion

        #region Field
        public SelectBuilder Field<X>(ScalarSqlExpression<X> expression)
        {
            Statement.Fields.Add(expression);
            return new SelectBuilder(Statement);
        }

        public SelectBuilder FieldAll<R>(R relation)
            where R: RelationSqlExpression, IAliasedSqlExpression
        {
            Statement.AddFieldAll(relation);
            return new SelectBuilder(Statement);
        }

        public ScalarSqlExpression<X> ToScalar<X>(ScalarSqlExpression<X> expression)
        {
            Field(expression);
            return Statement.ToScalar<X>();
        }

        ScalarSqlExpression<X> Quantify<X>(
            ScalarSqlExpression<X> expression,
            string quantifier
        )
        {
            Field(expression);
            return Statement.Quantify<X>(quantifier);
        }

        public ScalarSqlExpression<X> Any<X>(ScalarSqlExpression<X> expression)
        {
            return Quantify(expression, "any");
        }

        public ScalarSqlExpression<X> All<X>(ScalarSqlExpression<X> expression)
        {
            return Quantify(expression, "all");
        }
        #endregion

        #region Exists
        public SqlCondition Exists()
        {
            Statement.AddFieldAll();
            return
                new AtomicSqlCondition(
                    Statement.SqlBuilder,
                    SqlPart.FromString("exists").Concat(Statement.ToRelation())
                );
        }
        #endregion

        #region Insert
        public DbCommand Insert(Table table, Action<IExpressionRecord> setter)
        {
            return InsertBuilder.InsertSelectCommand(Statement, table, setter);
        }
        #endregion
    }
}
