﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using XCode.Cache;
using XCode.Configuration;
using XCode.DataAccessLayer;
using NewLife.Reflection;

namespace XCode
{
    partial class Entity<TEntity>
    {
        /// <summary>实体元数据</summary>
        public static class Meta
        {
            #region 主要属性
            /// <summary>实体类型</summary>
            public static Type ThisType { get { return typeof(TEntity); } }

            /// <summary>实体操作者</summary>
            public static IEntityOperate Factory
            {
                get
                {
                    Type type = ThisType;
                    if (type.IsInterface) return null;

                    return EntityFactory.CreateOperate(type);
                }
            }

            /// <summary>实体会话</summary>
            public static EntitySession<TEntity> Session { get { return EntitySession<TEntity>.Create(ConnName, TableName); } }
            #endregion

            #region 基本属性
            /// <summary>表信息</summary>
            public static TableItem Table { get { return TableItem.Create(ThisType); } }

            [ThreadStatic]
            private static String _ConnName;
            /// <summary>链接名。线程内允许修改，修改者负责还原。若要还原默认值，设为null即可</summary>
            public static String ConnName
            {
                get { return _ConnName ?? (_ConnName = Table.ConnName); }
                set
                {
                    _ConnName = value;

                    if (String.IsNullOrEmpty(_ConnName)) _ConnName = Table.ConnName;
                }
            }

            [ThreadStatic]
            private static String _TableName;
            /// <summary>表名。线程内允许修改，修改者负责还原</summary>
            public static String TableName
            {
                get { return _TableName ?? (_TableName = Table.TableName); }
                set
                {
                    _TableName = value;

                    if (String.IsNullOrEmpty(_TableName)) _TableName = Table.TableName;
                }
            }

            /// <summary>所有数据属性</summary>
            public static FieldItem[] AllFields { get { return Table.AllFields; } }

            /// <summary>所有绑定到数据表的属性</summary>
            public static FieldItem[] Fields { get { return Table.Fields; } }

            /// <summary>字段名集合，不区分大小写的哈希表存储，外部不要修改元素数据</summary>
            public static ICollection<String> FieldNames { get { return Table.FieldNames; } }

            /// <summary>唯一键，返回第一个标识列或者唯一的主键</summary>
            public static FieldItem Unique
            {
                get
                {
                    if (Table.Identity != null) return Table.Identity;
                    if (Table.PrimaryKeys != null && Table.PrimaryKeys.Length > 0) return Table.PrimaryKeys[0];
                    return null;
                }
            }

            /// <summary>主字段。主字段作为业务主要字段，代表当前数据行意义</summary>
            public static FieldItem Master { get { return Table.Master ?? Unique; } }
            #endregion

            #region 单实体对象缓存属性
            // 直接暴露单对象缓存参数设置，破坏了系统结构，现在只需要对第一个会话设定，其它会话可以自动复制第一个会话的设置
            //private static Int32 _SingleEntityCacheExpriod = 60;
            ///// <summary>过期时间。单位是秒，默认60秒</summary>
            //public static Int32 SingleEntityCacheExpriod
            //{
            //    get { return _SingleEntityCacheExpriod; }
            //    set { _SingleEntityCacheExpriod = value; }
            //}

            //private static Int32 _SingleEntityCacheMaxEntity = 10000;
            ///// <summary>最大实体数。默认10000</summary>
            //public static Int32 SingleEntityCacheMaxEntity
            //{
            //    get { return _SingleEntityCacheMaxEntity; }
            //    set { _SingleEntityCacheMaxEntity = value; }
            //}

            //private static Boolean _SingleEntityCacheAutoSave = true;
            ///// <summary>缓存到期时自动保存</summary>
            //public static Boolean SingleEntityCacheAutoSave
            //{
            //    get { return _SingleEntityCacheAutoSave; }
            //    set { _SingleEntityCacheAutoSave = value; }
            //}

            //private static Boolean _SingleEntityCacheAllowNull;
            ///// <summary>允许缓存空对象</summary>
            //public static Boolean SingleEntityCacheAllowNull
            //{
            //    get { return _SingleEntityCacheAllowNull; }
            //    set { _SingleEntityCacheAllowNull = value; }
            //}

            //private static FindKeyDelegate<Object, TEntity> _SingleEntityCacheFindKeyMethod;
            ///// <summary>单对象缓存查找数据的方法</summary>
            ///// <remarks>
            ///// 只要静态构造函数中赋值一次，分表分库产生多个的实体会话都可以在这儿获取单对象缓存查找数据的方法
            ///// ## 苦竹 添加 2014.04.04 20:25 ##</remarks>
            //public static FindKeyDelegate<Object, TEntity> SingleEntityCacheFindKeyMethod
            //{
            //    get { return _SingleEntityCacheFindKeyMethod; }
            //    set { _SingleEntityCacheFindKeyMethod = value; }
            //}
            #endregion

            #region 数据库操作
            ///// <summary>数据操作对象。</summary>
            //[Obsolete("=>Session")]
            //public static DAL DBO { get { return DAL.Create(ConnName); } }

            ///// <summary>执行SQL查询，返回记录集</summary>
            ///// <param name="builder">SQL语句</param>
            ///// <param name="startRowIndex">开始行，0表示第一行</param>
            ///// <param name="maximumRows">最大返回行数，0表示所有行</param>
            ///// <returns></returns>
            //[Obsolete("=>Session")]
            //public static DataSet Query(SelectBuilder builder, Int32 startRowIndex, Int32 maximumRows) { return Session.Query(builder, startRowIndex, maximumRows); }

            ///// <summary>执行SQL查询，返回记录集</summary>
            ///// <param name="sql">SQL语句</param>
            ///// <returns></returns>
            //[Obsolete("=>Session")]
            //public static DataSet Query(String sql) { return Session.Query(sql); }

            ///// <summary>查询记录数</summary>
            ///// <param name="sb">查询生成器</param>
            ///// <returns>记录数</returns>
            //[Obsolete("=>Session")]
            //public static Int32 QueryCount(SelectBuilder sb) { return Session.QueryCount(sb); }

            ///// <summary>执行</summary>
            ///// <param name="sql">SQL语句</param>
            ///// <returns>影响的结果</returns>
            //[Obsolete("=>Session")]
            //public static Int32 Execute(String sql) { return Session.Execute(sql); }

            ///// <summary>执行插入语句并返回新增行的自动编号</summary>
            ///// <param name="sql">SQL语句</param>
            ///// <returns>新增行的自动编号</returns>
            //[Obsolete("=>Session")]
            //public static Int64 InsertAndGetIdentity(String sql) { return Session.InsertAndGetIdentity(sql); }

            ///// <summary>执行</summary>
            ///// <param name="sql">SQL语句</param>
            ///// <param name="type">命令类型，默认SQL文本</param>
            ///// <param name="ps">命令参数</param>
            ///// <returns>影响的结果</returns>
            //[Obsolete("=>Session")]
            //public static Int32 Execute(String sql, CommandType type = CommandType.Text, params DbParameter[] ps) { return Session.Execute(sql, type, ps); }

            ///// <summary>执行插入语句并返回新增行的自动编号</summary>
            ///// <param name="sql">SQL语句</param>
            ///// <param name="type">命令类型，默认SQL文本</param>
            ///// <param name="ps">命令参数</param>
            ///// <returns>新增行的自动编号</returns>
            //[Obsolete("=>Session")]
            //public static Int64 InsertAndGetIdentity(String sql, CommandType type = CommandType.Text, params DbParameter[] ps) { return Session.InsertAndGetIdentity(sql, type, ps); }

            ///// <summary>数据改变后触发。参数指定触发该事件的实体类</summary>
            //[Obsolete("=>Session")]
            //[EditorBrowsable(EditorBrowsableState.Never)]
            //public static event Action<Type> OnDataChange { add { Session.OnDataChange += value; } remove { } }

            ///// <summary>检查并初始化数据。参数等待时间为0表示不等待</summary>
            ///// <param name="ms">等待时间，-1表示不限，0表示不等待</param>
            ///// <returns>如果等待，返回是否收到信号</returns>
            //[Obsolete("=>Session")]
            //[EditorBrowsable(EditorBrowsableState.Never)]
            //public static Boolean WaitForInitData(Int32 ms = 1000) { return Session.WaitForInitData(ms); }
            #endregion

            #region 事务保护
            /// <summary>开始事务</summary>
            /// <returns>剩下的事务计数</returns>
            //[Obsolete("=>Session")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static Int32 BeginTrans() { return Session.BeginTrans(); }

            /// <summary>提交事务</summary>
            /// <returns>剩下的事务计数</returns>
            //[Obsolete("=>Session")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static Int32 Commit() { return Session.Commit(); }

            /// <summary>回滚事务，忽略异常</summary>
            /// <returns>剩下的事务计数</returns>
            //[Obsolete("=>Session")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static Int32 Rollback() { return Session.Rollback(); }

            /// <summary>创建事务</summary>
            public static EntityTransaction CreateTrans() { return new EntityTransaction<TEntity>(); }
            #endregion

            #region 参数化
            ///// <summary>创建参数</summary>
            ///// <returns></returns>
            //[Obsolete("=>Session")]
            //[EditorBrowsable(EditorBrowsableState.Never)]
            //public static DbParameter CreateParameter() { return Session.Dal.Db.Factory.CreateParameter(); }

            ///// <summary>格式化参数名</summary>
            ///// <param name="name">名称</param>
            ///// <returns></returns>
            //[Obsolete("=>Session")]
            //[EditorBrowsable(EditorBrowsableState.Never)]
            //public static String FormatParameterName(String name) { return Session.Dal.Db.FormatParameterName(name); }
            #endregion

            #region 辅助方法
            /// <summary>格式化关键字</summary>
            /// <param name="name">名称</param>
            /// <returns></returns>
            public static String FormatName(String name) { return Session.Dal.Db.FormatName(name); }

            /// <summary>格式化时间</summary>
            /// <param name="dateTime"></param>
            /// <returns></returns>
            public static String FormatDateTime(DateTime dateTime) { return Session.Dal.Db.FormatDateTime(dateTime); }

            /// <summary>格式化数据为SQL数据</summary>
            /// <param name="name">名称</param>
            /// <param name="value">数值</param>
            /// <returns></returns>
            public static String FormatValue(String name, Object value) { return FormatValue(Table.FindByName(name), value); }

            /// <summary>格式化数据为SQL数据</summary>
            /// <param name="field">字段</param>
            /// <param name="value">数值</param>
            /// <returns></returns>
            public static String FormatValue(FieldItem field, Object value) { return Session.Dal.Db.FormatValue(field != null ? field.Field : null, value); }
            #endregion

            #region 缓存
            /// <summary>实体缓存</summary>
            /// <returns></returns>
            //[Obsolete("=>Session")]
            //[EditorBrowsable(EditorBrowsableState.Never)]
            public static EntityCache<TEntity> Cache { get { return Session.Cache; } }

            /// <summary>单对象实体缓存。
            /// 建议自定义查询数据方法，并从二级缓存中获取实体数据，以抵消因初次填充而带来的消耗。
            /// </summary>
            //[Obsolete("=>Session")]
            //[EditorBrowsable(EditorBrowsableState.Never)]
            public static ISingleEntityCache<Object, TEntity> SingleCache { get { return Session.SingleCache; } }

            /// <summary>总记录数，小于1000时是精确的，大于1000时缓存10分钟</summary>
            //[Obsolete("=>Session")]
            //[EditorBrowsable(EditorBrowsableState.Never)]
            public static Int32 Count { get { return (Int32)Session.LongCount; } }

            ///// <summary>总记录数，小于1000时是精确的，大于1000时缓存10分钟</summary>
            //[Obsolete("=>Session")]
            //[EditorBrowsable(EditorBrowsableState.Never)]
            //public static Int64 LongCount { get { return Session.LongCount; } }
            #endregion

            #region 分表分库
            /// <summary>在分库上执行操作，自动还原</summary>
            /// <param name="connName"></param>
            /// <param name="tableName"></param>
            /// <param name="func"></param>
            /// <returns></returns>
            public static Object ProcessWithSplit(String connName, String tableName, Func<Object> func)
            {
                using (var split = CreateSplit(connName, tableName))
                {
                    return func();
                }
            }

            /// <summary>创建分库会话，using结束时自动还原</summary>
            /// <param name="connName">连接名</param>
            /// <param name="tableName">表名</param>
            /// <returns></returns>
            public static IDisposable CreateSplit(String connName, String tableName)
            {
                return new SplitPackge(connName, tableName);
            }

            class SplitPackge : IDisposable
            {
                private String _ConnName;
                /// <summary>连接名</summary>
                public String ConnName { get { return _ConnName; } set { _ConnName = value; } }

                private String _TableName;
                /// <summary>表名</summary>
                public String TableName { get { return _TableName; } set { _TableName = value; } }

                public SplitPackge(String connName, String tableName)
                {
                    ConnName = Meta.ConnName;
                    TableName = Meta.TableName;

                    Meta.ConnName = connName;
                    Meta.TableName = tableName;
                }

                public void Dispose()
                {
                    Meta.ConnName = ConnName;
                    Meta.TableName = TableName;
                }
            }
            #endregion

            #region 模块
            internal static EntityModules _Modules = new EntityModules(typeof(TEntity));
            /// <summary>实体模块集合</summary>
            public static ICollection<IEntityModule> Modules { get { return _Modules; } }
            #endregion

            #region 队列
            //private static Dictionary<String, EntityQueue> _Queue = new Dictionary<String, EntityQueue>(StringComparer.OrdinalIgnoreCase);
            ///// <summary>实体队列</summary>
            //public static EntityQueue GetQueue()
            //{
            //    EntityQueue eq;
            //    if (!_Queue.TryGetValue(ConnName, out eq))
            //    {
            //        lock (_Queue)
            //        {
            //            if (!_Queue.TryGetValue(ConnName, out eq))
            //            {
            //                eq = new EntityQueue { Factory = Factory };
            //                _Queue.Add(ConnName, eq);
            //            }
            //        }
            //    }

            //    return eq;
            //}
            #endregion
        }
    }
}