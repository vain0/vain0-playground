﻿using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using FluentSqlBuilder.Detail;
using FluentSqlBuilder.Provider;

namespace FluentSqlBuilder.Public
{
    public class SqlBuilder
    {
        #region Provider
        internal DbProvider Provider { get; }

        internal SqlLanguage Language => Provider.Language;
        internal DbProviderFactory Factory => Provider.Factory;

        internal DbCommand CreateCommand(ISqlExecutable expression)
        {
            var command = Factory.CreateCommand();
            command.CommandText = expression.ToString();
            command.Parameters.AddRange(expression.Parameters.ToArray());
            return command;
        }
        #endregion

        public SqlBuilder(DbProvider provider)
        {
            Provider = provider;
        }

        #region Expression
        public ITable Table(string tableName) =>
            new Table(this, tableName);

        public ParameterExpression<IScalar<X>> Value<X>(DbType type, X value)
        {
            var name = "p" + Guid.NewGuid().ToString().Replace("-", "");
            var parameter = Factory.CreateParameter();
            parameter.ParameterName = name;
            parameter.DbType = type;
            parameter.Value = value;
            return new ParameterExpression<IScalar<X>>(this, name, parameter);
        }
        
        #region Typed value expressions
        public ParameterExpression<IScalar<bool>> Bool(bool value) =>
            Value(DbType.Boolean, value);

        public ParameterExpression<IScalar<long>> Int(long value) =>
            Value(DbType.Int64, value);

        public ParameterExpression<IScalar<string>> String(string value) =>
            Value(DbType.String, value);

        public ParameterExpression<IScalar<DateTime>> Date(DateTime value) =>
            Value(DbType.Date, value);

        public ParameterExpression<IScalar<DateTime>> DateTime(DateTime value) =>
            Value(DbType.DateTime, value);

        public SqlExpression<IScalar<object>> Null =>
            new AtomicExpression<IScalar<object>>(this, "null");

        public ConditionBuilder True =>
            new ConditionBuilder(this, ConditionCombinator.And);

        public ConditionBuilder False =>
            new ConditionBuilder(this, ConditionCombinator.Or);
        #endregion
        #endregion

        #region Condition
        public ConditionBuilder And() => True;
        public ConditionBuilder Or() => False;
        #endregion

        #region Mainpulation
        public FromlessSelectBuilder Select()
        {
            return new FromlessSelectBuilder(new SelectStatement(this));
        }
        #endregion
    }
}
