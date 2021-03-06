﻿/***********************************
 * Coder：EminemJK
 * Date：2018-11-20
 **********************************/

using Banana.Uow.Interface;
using Banana.Uow.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Banana.Uow.Extension
{
    /// <summary>
    /// SQL Server 扩展
    /// </summary>
    public class SQLServerExtension : IAdapter
    {
        /// <summary>
        /// SQL Server 扩展
        /// </summary>
        public SQLServerExtension() { }

        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repository"></param>
        /// <param name="pageNum">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="whereString">where语句，不需要携带where</param>
        /// <param name="param">where 参数</param>
        /// <param name="order"></param>
        /// <param name="asc"></param>
        /// <returns></returns>
        public SqlBuilder GetPageList<T>(IRepository<T> repository, int pageNum = 0, int pageSize = 0, string whereString = null, object param = null, object order = null, bool asc = false)
            where T : class, IEntity
        { 
            SqlBuilder sqlBuilder = new SqlBuilder();
            sqlBuilder.Select(repository.EntityType);
            if (pageSize > 0)
            {
                SqlBuilder sqlBuilderRows = new SqlBuilder();
                string ascSql = " asc";
                if (!asc)
                {
                    ascSql = " desc";
                }
                string orderSql = "ID";
                if (order != null)
                {
                    orderSql = SqlBuilder.GetArgsString("ORDER BY", order);
                }

                sqlBuilderRows.Select("SELECT ROW_NUMBER() OVER(ORDER BY " + order + " " + ascSql + ") AS rowid,*");
                sqlBuilderRows.From(repository.TableName);
                if (!string.IsNullOrEmpty(whereString))
                {
                    sqlBuilderRows.Where(whereString, param);
                }
                

                sqlBuilder.Append($"From ({sqlBuilderRows.SQL}) as t", sqlBuilderRows.Arguments);
                

                if (pageNum <= 0)
                    pageNum = 1;
                int numMin = (pageNum - 1) * pageSize + 1, numMax = pageNum * pageSize;
                sqlBuilder.Where("t.rowid>=@numMin and t.rowid<=@numMax", new { numMin, numMax });
            }
            else
            {
                sqlBuilder.From(repository.TableName);
                if (!string.IsNullOrEmpty(whereString))
                {
                    sqlBuilder.Where(whereString, param);
                }
                if (order != null)
                {
                    sqlBuilder.OrderBy(order);
                    sqlBuilder.IsAse(asc);
                }
            } 
            

            return sqlBuilder;
        }
    }
}
