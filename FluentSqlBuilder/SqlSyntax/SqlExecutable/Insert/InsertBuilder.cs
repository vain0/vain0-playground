﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSqlBuilder.Public;

namespace FluentSqlBuilder.Detail
{
    public static class InsertBuilder
    {
        public static DbCommand InsertValuesCommand(
            SqlBuilder sqlBuilder,
            Table table,
            Action<IValueRecord> setter
        )
        {
            var columns = table.Columns.ToArray();

            var parameters =
                columns
                .Select(column => sqlBuilder.CreateParameter(column.UniqueName, column.DbType))
                .ToArray();

            var record = new DbParameterRecord();
            foreach (var parameter in parameters)
            {
                record[parameter.ParameterName] = parameter;
            }

            setter(record);

            var columnNameList = table.ColumnNameList.Value;
            var parameterList = table.ColumnUniqueNameParameterList.Value;

            var dbCommand = sqlBuilder.Provider.Factory.CreateCommand();
            dbCommand.CommandText =
                $"insert into {table.QuotedName} ({columnNameList}) values ({parameterList})";
            dbCommand.Parameters.AddRange(parameters);
            return dbCommand;
        }

        public static DbCommand InsertSelectCommand(
            SelectStatement selectStatement,
            Table table,
            Action<IExpressionRecord> setter
        )
        {
            var sqlBuilder = selectStatement.SqlBuilder;
            var columns = table.Columns.ToArray();

            var assignment = new AssignmentRecord();
            foreach (var column in columns)
            {
                assignment.Add(column.UniqueName, sqlBuilder.Null);
            }

            setter(assignment);

            Debug.Assert(selectStatement.Fields.IsEmpty());
            selectStatement.Fields.AddRange(columns.Select(c => assignment[c.UniqueName]));

            var dbCommand = sqlBuilder.Provider.Factory.CreateCommand();
            var body = selectStatement.Tokens.Intercalate(' ');
            dbCommand.CommandText =
                $"insert into {table.QuotedName} ({table.ColumnNameList.Value}) {body}";
            dbCommand.Parameters.AddRange(selectStatement.Parameters.Distinct().ToArray());
            return dbCommand;
        }
    }
}
