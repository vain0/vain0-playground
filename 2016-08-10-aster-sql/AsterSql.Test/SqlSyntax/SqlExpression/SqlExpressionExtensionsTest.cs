using System;
using System.Linq;
using Xunit;
using AsterSql.SqlSyntax;

namespace AsterSql.Test
{
    public class SqlExpressionExtensionTest
    {
        static SqlBuilder Sql => FakeDb.Sql;

        #region Test: Invoke
        [Fact]
        public void TestInvoke_no_arguments()
        {
            SqlExpressionExtension
                .Invoke("f", Enumerable.Empty<ScalarSqlExpression>())
                .ToEmbeddedString()
                .ShouldEqual("f ( )");
        }

        [Fact]
        public void TestInvoke_two_arguments()
        {
            SqlExpressionExtension
                .Invoke("max", new[] { Sql.Int(1), Sql.Int(2) })
                .ToEmbeddedString()
                .ShouldEqual("max ( 1 , 2 )");
        }
        #endregion

        #region Test: Normal operators
        [Fact]
        public void TestConcat()
        {
            Sql.String("hello")
               .Concat(Sql.String("world"))
               .ToEmbeddedString()
               .ShouldEqual("concat ( 'hello' , 'world' )");
        }
        #endregion

        #region Test: Condition operators
        [Fact]
        public void TestIsNull()
        {
            Sql.Null<object>().IsNull()
               .ToEmbeddedString()
               .ShouldEqual("null is null");
        }

        [Fact]
        public void TestEqual()
        {
            Sql.Int(1).Equal(Sql.Int(2))
               .ToEmbeddedString()
               .ShouldEqual("1 = 2");
        }
        #endregion
    }
}
