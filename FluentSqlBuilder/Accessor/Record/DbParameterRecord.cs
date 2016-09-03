﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optional;
using FluentSqlBuilder.Public;

namespace FluentSqlBuilder.Detail
{
    public class DbParameterRecord
        : Dictionary<string, DbParameter>
        , IValueRecord
    {
        #region IRecord
        public new object this[string columnName]
        {
            get
            {
                return base[columnName];
            }
            set
            {
                DbParameter parameter;
                if (TryGetValue(columnName, out parameter))
                {
                    parameter.Value = value;
                }
                else
                {
                    throw new KeyNotFoundException(nameof(columnName));
                }
            }
        }
        #endregion
    }
}
