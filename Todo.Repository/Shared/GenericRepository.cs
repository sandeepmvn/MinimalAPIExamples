using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Todo.EFCore;
using Todo.Models.Dtos;

namespace Todo.Repository.Shared
{
    public class GenericRepository<Entity> : IGenericRepository<Entity>, IDisposable where Entity : class
    {
        protected ToDoDBContext dbcontext;
        protected DbSet<Entity> dbset;

        protected readonly ILogger logger;

        public GenericRepository(ToDoDBContext dbcontext, ILogger logger)
        {
            this.dbcontext = dbcontext;
            this.dbset = dbcontext.Set<Entity>();
            this.logger = logger;
        }

        public virtual async Task<IEnumerable<Entity>> GetAll()
        {
            try
            {
                IEnumerable<Entity> entities = null;
                entities = await dbset.AsNoTracking().ToListAsync();
                return entities;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(GetAll));
                throw;
            }
        }

        public virtual async Task<Entity> Find<TParmeter>(TParmeter id)
        {
            return await dbset.FindAsync(id);
        }

        public virtual async Task<Entity> FindAsNoTracking<TParmeter>(TParmeter id)
        {
            var entity = await dbset.FindAsync(id);
            dbcontext.Entry(entity).State = EntityState.Detached;
            return entity;
        }

        public virtual async Task<IEnumerable<Entity>> GetAllBy(Expression<Func<Entity, bool>> predicate)
        {
            try
            {
                return await dbset.Where(predicate).ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(GetAllBy));
                throw;
            }
        }

        public async virtual Task<PaginationEntityDto<Entity>> GetPaged(int pagenumber, int pagesize)
        {
            int skip = (pagenumber - 1) * pagesize;

            try
            {
                PaginationEntityDto<Entity> paginationEntityDto = new PaginationEntityDto<Entity>();
                paginationEntityDto.Count = await dbset.AsNoTracking().CountAsync();
                paginationEntityDto.Entities = await dbset.AsNoTracking().Skip(skip).Take(pagesize).ToListAsync();

                return paginationEntityDto;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(Delete));
                throw;
            }
        }

        /// <summary>
        ///  Pagination
        /// </summary>
        /// <param name="pagenumber">Page Number required for pagination</param>
        /// <param name="pagesize"> page size (Number of rows required in the data grid)</param>
        /// <param name="wherepredicate">Condition to get the required data</param>
        /// <param name="orderbypredicate">condition to get order by</param>
        /// <param name="orderbyascending">bool value to get the data by an order </param>
        /// <returns></returns>


        public async virtual Task<PaginationEntityDto<Entity>> GetPaged(int pagenumber, int pagesize, Expression<Func<Entity, bool>> wherepredicate, Expression<Func<Entity, object>> orderbypredicate, bool orderbyascending = true)
        {
            int skip = (pagenumber - 1) * pagesize;

            try
            {
                PaginationEntityDto<Entity> paginationEntityDto = new PaginationEntityDto<Entity>();
                paginationEntityDto.Count = await dbset.Where(wherepredicate).AsNoTracking().CountAsync();

                if (orderbyascending)
                {
                    paginationEntityDto.Entities = await dbset.AsNoTracking().Where(wherepredicate).OrderBy(orderbypredicate).AsNoTracking().Skip(skip).Take(pagesize).ToListAsync();
                }
                else
                {
                    paginationEntityDto.Entities = await dbset.AsNoTracking().Where(wherepredicate).OrderByDescending(orderbypredicate).AsNoTracking().Skip(skip).Take(pagesize).ToListAsync();
                }

                return paginationEntityDto;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(Delete));
                throw;
            }
        }

        public virtual async Task<Entity> GetBy(Expression<Func<Entity, bool>> predicate)
        {
            try
            {
                return await dbset.AsNoTracking().FirstOrDefaultAsync(predicate);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(GetBy));
                throw;
            }
        }

        public virtual async Task<Entity> Add(Entity entity)
        {
            try
            {
                await dbset.AddAsync(entity);
                await dbcontext.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(Add));
                throw;
            }
        }

        public virtual async Task Update(Entity entity, string entityid)
        {
            try
            {
                var oldEntity = await FindAsNoTracking(entityid);
                dbset.Update(entity);
                await dbcontext.SaveChangesAsync();
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(Update));
                throw;
            }
        }

        public virtual async Task Update(Entity entity)
        {
            try
            {
                dbset.Update(entity);
                await dbcontext.SaveChangesAsync();
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(Update));
                throw;
            }
        }

        public virtual async Task<Entity> UpdateEntity(Entity entity)
        {
            try
            {
                dbset.Update(entity);
                await dbcontext.SaveChangesAsync();



                return entity;
                //await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(UpdateEntity));
                throw;
            }
        }

        public virtual async Task Delete<T>(T id)
        {
            try
            {
                var res = await this.Find<T>(id);
                if (res != null)
                {
                    dbset.Remove(res);
                    await dbcontext.SaveChangesAsync();
                }
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(Delete));
                throw;
            }
        }
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    dbcontext.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
