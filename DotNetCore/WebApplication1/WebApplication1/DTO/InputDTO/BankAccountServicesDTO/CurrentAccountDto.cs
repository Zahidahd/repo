using WebApplication1.Enums;

namespace WebApplication1.DTO.InputDTO.BankAccountServicesDTO
{
    public class CurrentAccountDto
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string IfscCode { get; set; }
        public int AccountNumber { get; set; }
        public string AccountHolder1Name { get; set; }
        public string Holder1Email { get; set; }
        public string Holder1Address { get; set; }
        public string CompanyName { get; set; }
        public string GSTNo { get; set; }   
        public decimal AccountBalance { get; set; }
    }
}
