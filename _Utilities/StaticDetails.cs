using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _Utilities
{
    public static class StaticDetails
    {
        public const string Role_Customer = "Customer";
        public const string Role_Company = "Company";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";

		public const string StatusPending = "Pending";
		public const string StatusApproved = "Approved";
		public const string StatusInProcess = "Processing";
		public const string StatusShipped = "Shipped";
		public const string StatusCancelled = "Cancelled";
		

		public const string PaymentPending = "Pending";
		public const string PaymentApproved = "Approved";
		public const string PaymentDelayed = "Delayed";
		public const string PaymentRejected = "Rejected";
        public const string PaymentRefunded = "Refunded";
    }
}
