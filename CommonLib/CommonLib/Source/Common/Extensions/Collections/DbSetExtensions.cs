using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Expression = System.Linq.Expressions.Expression;

namespace CommonLib.Source.Common.Extensions.Collections
{
    public static class DbSetExtensions
    {
        public static void RemoveBy<TSource>(this DbSet<TSource> dbSet, Func<TSource, bool> selector) where TSource : class
        {
            if (dbSet == null) throw new ArgumentNullException(nameof(dbSet));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            var set = dbSet.ToArray();
            foreach (var entity in set)
            {
                if (selector(entity))
                    dbSet.Remove(entity);
            }
        }

        public static void RemoveByMany<TSource, TKey>(this DbSet<TSource> dbSet, Func<TSource, TKey> selector, IEnumerable<TKey> matches) where TSource : class
        {
            if (matches == null)
                throw new ArgumentNullException(nameof(matches));

            foreach (var match in matches)
                dbSet.RemoveBy(e => Equals(selector(e), match));
        }

        public static void RemoveDuplicatesBy<TSource, TKey>(this DbSet<TSource> dbSet, Func<TSource, TKey> selector) where TSource : class
        {
            if (dbSet == null)
                throw new ArgumentNullException(nameof(dbSet));
            if (selector == null) 
                throw new ArgumentNullException(nameof(selector));

            var knownKeys = new HashSet<TKey>();
            foreach (var entity in dbSet)
                if (!knownKeys.Add(selector(entity)))
                    dbSet.Remove(entity);
        }

        public static int Next<T>(this DbSet<T> dbSet, Func<T, int> selector) where T : class
        {
            return dbSet.Any() ? dbSet.AsEnumerable().Select(selector).Max() + 1 : 0;
        }

        public static DbContext GetContext<TEntity>(this DbSet<TEntity> dbSet) where TEntity : class
        {
            return (DbContext) dbSet?
                .GetType().GetTypeInfo()?
                .GetField("_context", BindingFlags.NonPublic | BindingFlags.Instance)?
                .GetValue(dbSet);
        }

        public static Expression<Func<T, bool>> ConvertToWhereClause<T>(this Expression<Func<T, object>> exp, T o) where T : class, new()
        {
            if (exp == null)
                throw new ArgumentNullException(nameof(exp));

            var memberExp = (MemberExpression)exp.Body;
            var objPropExp = Expression.PropertyOrField(Expression.Constant(o), memberExp.Member.Name);
            var equalExp = Expression.Equal(exp.Body, objPropExp);
            var exp2 = Expression.Lambda<Func<T, bool>>(equalExp, exp.Parameters);
            return exp2;
        }

        public static void AddOrUpdate<T>(this DbSet<T> dbSet, T data, params Expression<Func<T, object>>[] wheres) where T : class, new()
        {
            AddOrUpdate(dbSet, new List<T> { data }, wheres);
        }

        public static void AddOrUpdate<T>(this DbSet<T> dbSet, List<T> data, params Expression<Func<T, object>>[] wheres) where T : class, new()
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var context = dbSet.GetContext();
            foreach (var item in data)
            {
                var query = context.Set<T>().AsNoTracking();
                T entity;
                if (wheres.Any())
                {
                    query = wheres.Aggregate(query, (current, @where) => current.Where(@where.ConvertToWhereClause(item)));
                    entity = query.FirstOrDefault();
                }
                else
                    entity = query.FirstOrDefault(e => e.Equals(item));

                if (entity == null)
                    dbSet.Add(item);
                else
                {
                    var ids = context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties.Select(x => x.Name);
                    var keyFields = typeof(T).GetProperties().Where(x => ids.Contains(x.Name)).ToList();
                    foreach (var k in keyFields)
                        k.SetValue(item, k.GetValue(entity));
                    dbSet.Update(item);
                }
            }
        }

        public static void Refresh<TEntity>(this DbSet<TEntity> entities) where TEntity : class
        {
            if (entities == null)
                throw new NullReferenceException(nameof(entities));
            var db = entities.GetContext();

            foreach (var entity in entities)
            {
                foreach (var entry in db.Entry(entity).Navigations)
                {
                    if (entry.CurrentValue is IEnumerable<TEntity> children)
                        foreach (var child in children)
                            db.Entry(child).State = EntityState.Detached;
                    else if (entry.CurrentValue is TEntity child)
                        db.Entry(child).State = EntityState.Detached;
                }

                db.Entry(entity).State = EntityState.Detached;
            }
        }

        public static IAsyncQueryable AsAsyncQueryable<TEntity>(this DbSet<TEntity> dbSet) where TEntity : class => dbSet.ToAsyncEnumerable().AsAsyncQueryable();

        public static void Clear<T>(this DbSet<T> dbSet) where T : class => dbSet.RemoveRange(dbSet);

        public static async Task ClearAsync<T>(this DbSet<T> dbSet) where T : class => await Task.Run(() => dbSet.RemoveRange(dbSet));
    }
}
