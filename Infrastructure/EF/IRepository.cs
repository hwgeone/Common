using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EF
{
    public interface IRepository<T> where T : class
    {
        //一些通用方法的定义,由RepositoryBase<T> 实现

        void Add(T entity);

        void BatchAdd(T[] entities);

        T AddReturn(T entity);

        void Update(T entity);

        IQueryable<T> GetAll();

        IQueryable<T> GetBy(Expression<Func<T, bool>> filter);

        bool DeleteBy(Expression<Func<T, bool>> filter);
        /// <summary>
        /// 按指定的ID进行批量更新
        /// </summary>
        void Update(Expression<Func<T, object>> identityExp, T entity);

        /// <summary>
        /// 实现按需要只更新部分更新
        /// <para>如：Update<T>(u =>u.Id==1,u =>new User{Name="ok"}) where T:class;</para>
        /// </summary>
        /// <param name="where">更新条件</param>
        /// <param name="entity">更新后的实体</param>
        void Update(Expression<Func<T, bool>> where, Expression<Func<T, T>> entity);
    }
}
