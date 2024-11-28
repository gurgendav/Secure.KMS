using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EQS.KMS.Application.Interfaces
{
    public interface IRepository<T> where T : class
    {
        T Get(long id);
        IEnumerable<T> GetAll();
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
    }
}
