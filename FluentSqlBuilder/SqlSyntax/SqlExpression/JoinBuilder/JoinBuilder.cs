﻿using System;
using FluentSqlBuilder.Public;

namespace FluentSqlBuilder.Detail
{
    public class JoinBuilder<TResult>
    {
        SqlBuilder SqlBuilder { get; }
        IRelationalQueryOrCommand Statement { get; }
        JoinType JoinType { get; }
        ISqlExpression<IRelation> Relation { get; }
        Func<Join, TResult> Run { get; }

        public JoinBuilder(
            SqlBuilder sqlBuilder,
            IRelationalQueryOrCommand statement,
            JoinType joinType,
            ISqlExpression<IRelation> relation,
            Func<Join, TResult> run
        )
        {
            SqlBuilder = sqlBuilder;
            Statement = statement;
            JoinType = joinType;
            Relation = relation;
            Run = run;
        }

        public TResult On(ISqlCondition condition)
        {
            return Run(new JoinOn(JoinType, Relation, condition));
        }

        public TResult Using(IColumn column)
        {
            var columnName = SqlBuilder.Language.QuoteIdentifier(column.RawName);
            return Run(new JoinUsing(JoinType, Relation, columnName));
        }
    }
}
