using Domain.Entities;
using iText.Commons.Actions.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        public EventManagmentDb Context;
        public DbSet<TEntity> DbSet { get; }

        public GenericRepository(EventManagmentDb context)
        {
            Context = context;
            DbSet = Context.Set<TEntity>();
        }

        public void Save() => Context.SaveChanges();
        public async Task SaveAsync() => await Context.SaveChangesAsync();


        public async Task<IList<TEntity>> GetAll()
        {
            return await Context.Set<TEntity>().AsNoTracking().ToListAsync();
        }
        public async Task<TEntity> GetById(int id)
        {
            return await Context.Set<TEntity>().FindAsync(id);
        }
        public async Task<TEntity> GetByIdWithoutTracking(int id)
        {
            var entity = await Context.Set<TEntity>().FindAsync(id);

            if (entity != null)
                Context.Entry(entity).State = EntityState.Detached;

            return entity;
        }
        public IList<TEntity> Find(Func<TEntity, bool> predicate)
        {
            return Context.Set<TEntity>().AsNoTracking().Where(predicate).ToList();
        }
        public IList<TEntity> SearchBy(string columName, string searchText)
        {
            Type columNameType;
            object searchTextType;
            Expression<Func<TEntity, bool>> lambda;

            try
            {
                columNameType = typeof(TEntity).GetProperty(columName).PropertyType;
            }
            catch (Exception ex)
            {
                //TODO: Me bo me kthy diqka tjeter ne rast errori se qeshtut me throw nuk osht mire.
                throw new ArgumentNullException("Column Name doesnt exists." + Environment.NewLine + ex.Message);
            }

            try
            {
                searchTextType = Convert.ChangeType(searchText, columNameType);
            }
            catch (Exception ex)
            {
                //TODO: Me bo me kthy diqka tjeter ne rast errori se qeshtut me throw nuk osht mire.
                throw new KeyNotFoundException("Could not convert the type." + Environment.NewLine + ex.Message);
            }

            //Class of Entity
            var obj = Expression.Parameter(typeof(TEntity), typeof(TEntity).Name);

            //Search Value
            var constant = Expression.Constant(searchTextType, columNameType);

            //Property of Entity
            var objProperty = Expression.PropertyOrField(obj, columName);

            if (columNameType == typeof(string))
            {
                //Lambda expression
                var expression = Expression.Call(objProperty, "Contains", null, constant);
                lambda = Expression.Lambda<Func<TEntity, bool>>(expression, obj);
            }
            else
            {
                var expression = Expression.Equal(objProperty, constant);
                lambda = Expression.Lambda<Func<TEntity, bool>>(expression, obj);
            }

            var compiledLambda = lambda.Compile();

            var searchResult = Context.Set<TEntity>().AsNoTracking().Where(compiledLambda).ToList();

            return searchResult;
        }

        public TEntity Create(TEntity entity)
        {
            Context.Add(entity);
            Save();

            return entity;
        }
        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            Context.Add(entity);
            await SaveAsync();
            return entity;
        }
        public IList<TEntity> CreateRannge(IList<TEntity> entity)
        {
            Context.AddRange(entity);
            Save();
            return entity;
        }
        public async Task<IList<TEntity>> CreateRanngeAsync(IList<TEntity> entity)
        {
            Context.AddRange(entity);
            await SaveAsync();
            return entity;
        }
        public TEntity Update(TEntity entity)
        {
            Context.ChangeTracker.TrackGraph(entity, e => e.Entry.State = EntityState.Modified);

            Save();

            Context.Entry(entity).State = EntityState.Detached;

            return entity;
        }
        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            var entityType = Context.Model.FindEntityType(typeof(TEntity));
            var primaryKey = entityType.FindPrimaryKey();
            var keyValues = primaryKey.Properties.Select(p => entity.GetType().GetProperty(p.Name).GetValue(entity)).ToArray();
            var existingEntity = await Context.FindAsync<TEntity>(keyValues);

            if (existingEntity != null)
            {
                // Detach existing entity
                Context.Entry(existingEntity).State = EntityState.Detached; 
            }
            // Attach updated entity
            Context.Entry(entity).State = EntityState.Modified; 
            await SaveAsync();

            return entity;
        }
        public IList<TEntity> UpdateRange(IList<TEntity> entities)
        {
            entities.ToList().ForEach(entity => Context.Entry(entity).State = EntityState.Modified);
            Save();
            return entities;
        }
        public async Task<IList<TEntity>> UpdateRangeAsync(IList<TEntity> entities)
        {
            entities.ToList().ForEach(entity => Context.Entry(entity).State = EntityState.Modified);

            await SaveAsync();
            return entities;
        }


        public void Delete(int id)
        {
            var entity = DbSet.Find(id);
            Context.Remove(entity);
            Save();
        }
        public async Task DeleteAsync(TEntity entity)
        {
            Context.Remove(entity);
            await SaveAsync();
        }
        public void DeleteRange(IList<TEntity> entities)
        {
            Context.RemoveRange(entities);
            Save();
        }
        public async Task DeleteRangeAsync(IList<TEntity> entities)
        {
            Context.RemoveRange(entities);
            await SaveAsync();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && Context != null)
            {
                Context.Dispose();
            }

            Context = null;
        }

        public TEntity UpdateExceptProperties(TEntity entity, params Expression<Func<TEntity, object>>[] excludedProperties)
        {
            Context.ChangeTracker.TrackGraph(entity, e => e.Entry.State = EntityState.Modified);

            foreach(var property in excludedProperties)
            {
                var propertyName = GetPropertyName(property);
                Context.Entry(entity).Property(propertyName).IsModified = false;
            }
            Save();

            Context.Entry(entity).State = EntityState.Detached;

            return entity;
        }

        private string GetPropertyName(Expression<Func<TEntity, object>> propertyExpression)
        {
            if (propertyExpression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }
            else if (propertyExpression.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression unaryMemberExpression)
            {
                return unaryMemberExpression.Member.Name;
            }

            throw new ArgumentException("Invalid property expression");
        }
    }
}
