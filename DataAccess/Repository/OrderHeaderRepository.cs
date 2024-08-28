using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDBContext _db;
        public OrderHeaderRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader obj)
        {
            _db.OrderHeaders.Update(obj);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            OrderHeader orderheader = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (orderheader != null)
            {
                orderheader.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderheader.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripeHeaderId(int id, string sessionId, string payementIntentId)
        {
            OrderHeader orderheader = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (!string.IsNullOrEmpty(sessionId))
            {
                orderheader.SessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(payementIntentId))
            {
                orderheader.PaymentIntentId = payementIntentId;
                orderheader.PaymentDate = DateTime.Now;
            }
        }
    }
}
