using CDF_Core.Interfaces;
using CDF_Infrastructure.Persistence.Data;
using CDF_Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Infrastructure.UnitOfWork
{
    public class UnitOfWork<T> : IUnitOfWork<T> where T : class
    {
        private readonly ApplicationDBContext _context;
        private IGenericRepository<T> _entity;


        public UnitOfWork(ApplicationDBContext context)
        {
            _context = context;
        }
        public IGenericRepository<T> Repository
        {
            get
            {
                return _entity ?? (_entity = new GenericRepository<T>(_context));
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
