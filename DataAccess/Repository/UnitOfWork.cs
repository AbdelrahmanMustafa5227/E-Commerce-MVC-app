using DataAccess.Data;
using DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDBContext _db;
        public ICategoryRepository CategoryRepo { get; private set; }
        public IProductRepository ProductRepo { get; private set; }
        public ICompanyRepository CompanyRepo { get; private set; }
        public IShoppingCartRepository ShoppingCartRepo { get; private set; }
        public IApplicationUserRepository ApplicationUserRepo { get; private set; }
        public IOrderHeaderRepository OrderHeaderRepo { get; private set; }
        public IOrderDetailRepository OrderDetailRepo { get; private set; }
        public UnitOfWork (ApplicationDBContext db) 
        { 
            _db = db;
            CategoryRepo = new CategoryRepository(_db);
            ProductRepo = new ProductRepository(_db);
            CompanyRepo = new CompanyRepository(_db);
            ShoppingCartRepo = new ShoppingCartRepository(_db);
            ApplicationUserRepo = new ApplicationUserRepository(_db);
            OrderHeaderRepo = new OrderHeaderRepository(_db);
            OrderDetailRepo = new OrderDetailRepository(_db);
        }
         
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
