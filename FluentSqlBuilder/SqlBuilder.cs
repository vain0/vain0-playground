﻿using System;
using System.Data;
using FluentSqlBuilder.Detail;

namespace FluentSqlBuilder
{
    public class SqlBuilder
    {
        internal DbmsDialect Dialect { get; }

        public SqlBuilder(DbmsDialect dialect)
        {
            Dialect = dialect;
        }

        #region Expression
        public Expression Table(string qualifier, string tableName)
        {
            throw new NotImplementedException();
        }

        public Expression Table(string tableName)
        {
            if (!Dialect.Language.IsTableName(tableName))
            {
                throw new ArgumentException(nameof(tableName));
            }    

            return new Expression(Dialect.Language.EscapeTableName(tableName));
        }

        public Expression Column(string qualifier, string tableName)
        {
            throw new NotImplementedException();
        }

        public Expression Column(string columnName)
        {
            if (!Dialect.Language.IsColumnName(columnName))
            {
                throw new ArgumentException(nameof(columnName));
            }

            return new Expression(Dialect.Language.EscaleColumnName(columnName));
        }

        public Expression Value(DbType type, object value)
        {
            var name = "p" + Guid.NewGuid().ToString().Replace("-", "");
            var parameter = Dialect.ParameterFactory.Create(name, type, value);
            return new ParameterExpression(name, parameter);
        }

        #region Typed value expressions
        public Expression Bool(bool value)
        {
            return Value(DbType.Boolean, value);
        }

        public Expression Int(long value)
        {
            return Value(DbType.Int64, value);
        }

        public Expression String(string value)
        {
            return Value(DbType.String, value);
        }

        public Expression Date(DateTime value)
        {
            return Value(DbType.Date, value);
        }

        public Expression DateTime(DateTime value)
        {
            return Value(DbType.DateTime, value);
        }

        static readonly Expression _nullExpression =
            new Expression("null");

        public Expression Null
        {
            get { return _nullExpression; }
        }
        #endregion
        #endregion

        #region Condition
        public ConditionBuilder And()
        {
            return new ConditionBuilder(ConditionCombinator.And);
        }

        public ConditionBuilder Or()
        {
            return new ConditionBuilder(ConditionCombinator.Or);
        }
        #endregion

        #region Mainpulation
        public FromlessSelectBuilder Select()
        {
            return new FromlessSelectBuilder(this, new SelectStatement());
        }
        #endregion
    }
}
