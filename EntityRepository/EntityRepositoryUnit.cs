using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Data.Common;
using System.Reflection;
using System.Data.Linq.Mapping;
using System.Linq.Expressions;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;

namespace EntityRepository
{
    public static class EntityRepositoryUnit
    {

        public static DbSet<TEntity> GetDbSet<TEntity>(this DbContext dbContext)
            where TEntity : class
        {
            return dbContext.Set<TEntity>();
        }

        #region Get By Id Methods

        public static TEntity GetUnitById<TEntity>(this DbContext dbContext, int id)
            where TEntity : class
        {
            return GetUnitByDynamicId<TEntity>(dbContext, id);
        }

        public static TEntity GetUnitById<TEntity>(this DbContext dbContext, Guid id)
            where TEntity : class
        {
            return GetUnitByDynamicId<TEntity>(dbContext, id);
        }

        private static TEntity GetUnitByDynamicId<TEntity>(this DbContext dbContext, dynamic id)
            where TEntity : class
        {
            var DbSet = dbContext.GetDbSet<TEntity>();
            Type primaryKeyType;
            var primaryKeyName = GetPrimaryKeyName<TEntity>(dbContext, out primaryKeyType);

            var param = Expression.Parameter(typeof(TEntity), "x");
            var left = Expression.Property(param, typeof(TEntity).GetProperty(primaryKeyName));
            var right = Expression.Constant(id, id.GetType());
            var query = Expression.Equal(left, right);

            Expression<Func<TEntity, bool>> predicate = Expression.Lambda<Func<TEntity, bool>>(query, param);
            TEntity result = DbSet.SingleOrDefault(predicate);

            return result;
        }

        #endregion

        #region Update Methods

        public static void UpdateUnit<TEntity>(this DbContext dbContext, TEntity entity)
            where TEntity : class
        {
            dynamic value = GetPrimaryKeyValue<TEntity>(dbContext, entity);
            TEntity oldEntity = GetUnitById<TEntity>(dbContext, value);
            
            var oldEnityType = oldEntity.GetType();
            var entityType = entity.GetType();

            foreach (PropertyInfo property in oldEnityType.GetProperties())
            {
                var info = entityType.GetProperty(property.Name);
                var attr = (AssociationAttribute[])property.GetCustomAttributes(typeof(AssociationAttribute), false);

                if (property.CanWrite
                    && !property.Name.Equals("EntityKey") 
                    &&( property.PropertyType.BaseType.Name.Equals("ValueType")
                    || property.PropertyType.BaseType.Name.Equals("Object")))
                    property.SetValue(oldEntity, info.GetValue(entity, null), null);
            }

            dbContext.SaveChanges();

        }

        #endregion

        #region Insert Methods

        public static TEntity InsertUnit<TEntity>(this DbContext dbContext, TEntity entity)
            where TEntity : class
        {
            DbSet<TEntity> DbSet = dbContext.GetDbSet<TEntity>();
            DbSet.Add(entity);
            dbContext.SaveChanges();
            return entity;
        }

        public static IEnumerable<TEntity> InsertUnits<TEntity>(this DbContext dbContext, IEnumerable<TEntity> entitys)
            where TEntity : class
        {
            DbSet<TEntity> DbSet = dbContext.GetDbSet<TEntity>();
            foreach (var entity in entitys)
            {
                DbSet.Add(entity);   
            }
            dbContext.SaveChanges();
            return entitys;
        }

        #endregion

        #region Delete Methods

        public static void DeleteUnit<TEntity>(this DbContext dbContext, TEntity entity)
            where TEntity : class
        {
            DbSet<TEntity> DbSet = dbContext.GetDbSet<TEntity>();

            DbSet.Remove(entity);

            dbContext.SaveChanges();
        }

        public static void DeleteUnits<TEntity>(this DbContext dbContext, IEnumerable<TEntity> entitys)
            where TEntity : class
        {
            DbSet<TEntity> DbSet = dbContext.GetDbSet<TEntity>();
            foreach (var entity in entitys)
            {
                DbSet.Remove(entity);   
            }
            dbContext.SaveChanges();
        }

        #endregion

        #region Delete By Id Methods

        public static void DeleteUnitById<TEntity>(this DbContext dbContext, int id)
            where TEntity : class
        {
            DeleteUnitByDynamicId<TEntity>(dbContext, id);
        }

        public static void DeleteUnitById<TEntity>(this DbContext dbContext, Guid id)
            where TEntity : class
        {
            DeleteUnitByDynamicId<TEntity>(dbContext, id);
        }

        private static void DeleteUnitByDynamicId<TEntity>(this DbContext dbContext, dynamic id)
            where TEntity : class
        {
            TEntity entity = GetUnitById<TEntity>(dbContext,id);
            DeleteUnit<TEntity>(dbContext, entity);
            dbContext.SaveChanges();
        }

        #endregion

        #region Delete By Ids Methods

        public static void DeleteUnitsByIds<TEntity>(this DbContext dbContext, int[] ids)
            where TEntity : class
        {
            DeleteUnitsByDynamicIds<TEntity>(dbContext, ids);
        }

        public static void DeleteUnitsByIds<TEntity>(this DbContext dbContext, Guid[] ids)
            where TEntity : class
        {
            DeleteUnitsByDynamicIds<TEntity>(dbContext, ids);
        }

        private static void DeleteUnitsByDynamicIds<TEntity>(this DbContext dbContext, dynamic ids)
            where TEntity : class
        {
            foreach (var id in ids)
            {
                TEntity entity = GetUnitById<TEntity>(dbContext, id);
                DeleteUnit<TEntity>(dbContext, entity);
            }
            dbContext.SaveChanges();
        }

        #endregion

        #region Select methods

        public static IQueryable<TEntity> GetUnitsByFields<TEntity>(this DbContext dbContext,PartQuery part, params IPartConditionQuery[] parts)
            where TEntity : class
        {
            Expression left = null, right = null, query = null;
            PropertyInfo info = null;
            Type fieldType = null;

            var table = dbContext.GetDbSet<TEntity>();
            var param = Expression.Parameter(typeof(TEntity), "x");

            info=typeof(TEntity).GetProperty(part.FeildName);
            left = Expression.Property(param, info);

            fieldType = Type.GetType(info.PropertyType.FullName);
            right = Expression.Constant(Convert.ChangeType(part.FieldValue, fieldType), fieldType);

            query = Expression.Equal(left, right);

            for (int i = 0; i < parts.Length; i++)
            {
                info = typeof(TEntity).GetProperty(((IPartQuery)parts[i]).FeildName);
                left = Expression.Property(param, info);
                
                fieldType = Type.GetType(info.PropertyType.FullName);
                right = Expression.Constant(Convert.ChangeType(((IPartQuery)parts[i]).FieldValue, fieldType), fieldType);

                if (parts[i] is OrQueryPart)
                    query = Expression.Or(query, Expression.Equal(left, right));
                
                else if (parts[i] is AndQueryPart)
                    query = Expression.And(query, Expression.Equal(left, right));

            }

            var predicate = Expression.Lambda<Func<TEntity, bool>>(query, param);
            var result = table.Where(predicate);

            return result;
        }

        #endregion

        #region Private Helpers Methods

        public static string GetPrimaryKeyName<TEntity>(this DbContext dbContext, out Type primaryKeyType)
            where TEntity : class
        {
            Dictionary<Type, string[]> _dict = new Dictionary<Type, string[]>();
            primaryKeyType = typeof(TEntity);
            //retreive the base type
            while (primaryKeyType.BaseType != typeof(object))
                primaryKeyType = primaryKeyType.BaseType;

            string[] keys;

            _dict.TryGetValue(primaryKeyType, out keys);
            if (keys != null)
                return keys.ElementAt(0);

            System.Data.Entity.Core.Objects.ObjectContext objectContext = ((IObjectContextAdapter)dbContext).ObjectContext;

            //create method CreateObjectSet with the generic parameter of the base-type
            MethodInfo method = typeof(System.Data.Entity.Core.Objects.ObjectContext).GetMethod("CreateObjectSet", Type.EmptyTypes)
                                                     .MakeGenericMethod(primaryKeyType);
            dynamic objectSet = method.Invoke(objectContext, null);

            IEnumerable<dynamic> keyMembers = objectSet.EntitySet.ElementType.KeyMembers;

            string[] keyNames = keyMembers.Select(k => (string)k.Name).ToArray();

            _dict.Add(primaryKeyType, keyNames);

            return keyNames.ElementAt(0);
        }

        public static dynamic GetPrimaryKeyValue<TEntity>(this DbContext dbContext, TEntity entity)
            where TEntity : class
        {
            Type primaryKeyType;
            string primaryKeyName = GetPrimaryKeyName<TEntity>(dbContext, out primaryKeyType);
            dynamic primaryKeyValue = null;

            Type type = entity.GetType();
            primaryKeyValue = type.GetProperty(primaryKeyName).GetValue(entity, null);

            return primaryKeyValue;
        }

        #endregion


        //#region RelationBetween any tables

        //public static IEnumerable<TRelation> GetRelations<TSource, TRelation>(this DbContext dbContext,int id)
        //    where TSource : class
        //    where TRelation : class
        //{
        //    Type tableRelations = typeof(TRelation);
        //    IEnumerable<FieldInfo> fieldsType = tableRelations.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        //    FieldInfo needField = null;

        //    foreach (var item in fieldsType)
        //    {
        //        if (item.FieldType == typeof(EntityRef<TSource>))
        //        {
        //            needField = item;
        //            break;
        //        }
        //    }


        //    Table<TSource> table =dbContext.GetTable<TSource>();

        //    return null;
        //}

        //#endregion

    }  
}
