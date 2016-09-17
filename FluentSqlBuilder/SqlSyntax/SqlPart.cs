using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace FluentSqlBuilder.SqlSyntax
{
    /// <summary>
    /// SQL文の断片を表す。
    /// </summary>
    public abstract class SqlPart
        : IEnumerable<SqlToken>
    {
        internal abstract IEnumerable<SqlToken> Tokens { get; }

        public override string ToString()
        {
            return string.Join(" ", Tokens.Select(t => t.String));
        }

        #region IEnumerable
        IEnumerator IEnumerable.GetEnumerator() =>
            Tokens.GetEnumerator();

        IEnumerator<SqlToken> IEnumerable<SqlToken>.GetEnumerator() =>
            Tokens.GetEnumerator();
        #endregion

        public static SqlPart FromString(string token) =>
            new ConcreteSqlPart(new[] { SqlToken.FromString(token) });

        public static SqlPart Concat(IEnumerable<SqlPart> parts) =>
            new ConcreteSqlPart(parts.SelectMany(part => part.Tokens));
    }

    sealed class ConcreteSqlPart
        : SqlPart
    {
        internal override IEnumerable<SqlToken> Tokens { get; }

        public ConcreteSqlPart(IEnumerable<SqlToken> tokens)
        {
            Tokens = tokens;
        }
    }
}
