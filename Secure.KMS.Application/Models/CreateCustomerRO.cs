using EQS.KMS.Application.Entities;
using System.Collections.Generic;

namespace EQS.KMS.Application.Models
{
    public class CreateCustomerRO
    {
        public List<Customer> Customers { get; set; }
        public List<User> Users { get; set; }
        public List<KeySet> KeySets { get; set; }
    }
}
