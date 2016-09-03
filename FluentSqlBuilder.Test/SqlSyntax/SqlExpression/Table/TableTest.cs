﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentSqlBuilder.Detail;
using FluentSqlBuilder.Public;
using Xunit;
namespace FluentSqlBuilder.Test
{
    public class TableTest
    {
        static SqlBuilder Sql => FakeDb.Sql;
        static Table EmployeeTable => (Table)FakeDb.Employee.Table;

        [Fact]
        public void TestColumnProperties()
        {
            EmployeeTable
                .ColumnProperties()
                .Select(propertyInfo => propertyInfo.Name)
                .ToArray()
                .ShouldEqual(new[] { "Name", "Age", "DepartmentId" });
        }

        [Fact]
        public void TestInsert()
        {
            var employee = FakeDb.Employee;
            Sql.InsertValues(employee.Table, record =>
            {
                employee.Name[record] = "Miku";
                employee.Age[record] = 16L;
            })
                .ToEmbeddedString()
                .ShouldEqual(
                    "insert into `employees` (`name`,`age`,`department_id`)"
                    + " values ('Miku',16,null)");
        }
    }
}
