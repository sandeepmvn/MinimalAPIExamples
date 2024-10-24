using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Todo.Models.Dtos;

namespace Todo.Repository.Shared
{
    public interface IGenericRepository<Entity>
    {
        Task<IEnumerable<Entity>> GetAll();
        Task<IEnumerable<Entity>> GetAllBy(Expression<Func<Entity, bool>> predicate);
        Task<Entity> Find<TParmeter>(TParmeter id);
        Task<Entity> FindAsNoTracking<TParameter>(TParameter id);
        Task<Entity> GetBy(Expression<Func<Entity, bool>> predicate);
        Task<PaginationEntityDto<Entity>> GetPaged(int pagenumber, int pagesize);
        Task<PaginationEntityDto<Entity>> GetPaged(int pagenumber, int pagesize, Expression<Func<Entity, bool>> predicate, Expression<Func<Entity, object>> orderbypredicate, bool orderbyascending = true);
        Task<Entity> Add(Entity entity);
        Task Update(Entity entity);
        Task Update(Entity entity, string entityid);
        Task Delete<T>(T id);
    }
}
