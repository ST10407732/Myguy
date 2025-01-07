//using System;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace MYGUYY.Models
//{
//    public class Payment
//    {
//        public int Id { get; set; }
      
//        public DateTime PaymentDate { get; set; } = DateTime.Now;

//        [ForeignKey("TaskRequest")]
//        public int TaskRequestId { get; set; }
//        public TaskRequest TaskRequest { get; set; }

//        public string PaymentStatus { get; set; } = "Pending"; // "Pending", "Completed"
//        public decimal Amount { get; internal set; }
//    }
//}
