using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsterSql.Core.SqlSyntax;

namespace AsterSql.Core.Accessor
{
    public interface IValueRecord
    {
        object this[string columnName] { get; set; }
    }

    public interface IExpressionRecord
    {
        ScalarSqlExpression this[string columnName] { get; set; }
    }
}
