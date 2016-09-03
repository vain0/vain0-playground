﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using FluentSqlBuilder.Public;

namespace FluentSqlBuilder.Detail
{
    /// <summary>
    /// SQLの式を表します。
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    public interface ISqlExpression<out TType>
        : ISqlPart
        where TType: ISqlTypeTag
    {
        SqlBuilder SqlBuilder { get; }

        IAliasedSqlExpression<TType> As(string alias);
    }

    public abstract class SqlExpression<TType>
        : ISqlExpression<TType>
        where TType: ISqlTypeTag
    {
        #region ISqlPart
        public abstract IEnumerable<string> Tokens { get; }
        public abstract IEnumerable<DbParameter> Parameters { get; }
        #endregion

        protected SqlExpression(SqlBuilder sqlBuilder)
        {
            SqlBuilder = sqlBuilder;
        }

        public override string ToString() =>
            string.Join(" ", Tokens);

        #region ISqlExpression
        public SqlBuilder SqlBuilder { get; }

        public IAliasedSqlExpression<TType> As(string alias)
        {
            return new AliasedExpression<TType>(SqlBuilder, this, alias);
        }
        #endregion
    }

    public class CompoundExpression<TType>
        : SqlExpression<TType>
        where TType: ISqlTypeTag
    {
        ISqlPart Part { get; }

        #region SqlExpression
        public sealed override IEnumerable<string> Tokens => Part.Tokens;
        public sealed override IEnumerable<DbParameter> Parameters => Part.Parameters;
        #endregion

        internal CompoundExpression(SqlBuilder sqlBuilder, ISqlPart part)
            : base(sqlBuilder)
        {
            Part = part;
        }
    }

    public class AtomicExpression<TType>
        : CompoundExpression<TType>
        where TType: ISqlTypeTag
    {
        internal AtomicExpression(SqlBuilder sqlBuilder, string @string)
            : base(sqlBuilder, SqlPart.FromToken(@string))
        {
        }
    }

    public class ParameterExpression<TValue>
        : SqlExpression<IScalar<TValue>>
    {
        string Name { get; }
        DbParameter Parameter { get; }

        internal ParameterExpression(
            SqlBuilder sqlBuilder,
            string name,
            DbParameter parameter
        )
            : base(sqlBuilder)
        {
            Name = "@" + name;
            Parameter = parameter;
        }

        #region SqlExpression
        public sealed override IEnumerable<string> Tokens =>
            new[] { Name };

        public sealed override IEnumerable<DbParameter> Parameters =>
            new[] { Parameter };
        #endregion
    }
    public interface IAliasedSqlExpression<out TType>
        : ISqlExpression<TType>
        where TType: ISqlTypeTag
    {
        string Alias { get; }
    }

    public class AliasedExpression<TType>
        : SqlExpression<TType>
        , IAliasedSqlExpression<TType>
        where TType: ISqlTypeTag
    {
        public ISqlExpression<TType> Expression { get; }
        public string Alias { get; }

        public AliasedExpression(
            SqlBuilder sqlBuilder,
            ISqlExpression<TType> expression,
            string alias
        )
            : base(sqlBuilder)
        {
            Expression = expression;
            Alias = alias;
        }

        #region ISqlPart
        public override IEnumerable<string> Tokens =>
            SqlBuilder.Language.ConstructAliasedExpression(Expression.Tokens, Alias);

        public override IEnumerable<DbParameter> Parameters =>
            Expression.Parameters;
        #endregion
    }
}
