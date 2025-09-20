using CsvHelper.Configuration.Attributes;

namespace MiniBitMVC.Models
{
    public class TransactionCsv
    {
        [Name("Transaction Date")]
        public DateTime TransactionDate { get; set; }

        [Name("Transaction No")]
        public string TransactionNo { get; set; }

        [Name("Debit")]
        public decimal Debit { get; set; }

        [Name("Credit")]
        public decimal Credit { get; set; }

        [Name("Details")]
        public string Details { get; set; }

        [Name("Beneficiary/Applicant")]
        public string Beneficiary { get; set; }

        [Name("Account")]
        public string Account { get; set; }

        [Name("Remitter Bank")]
        public string Bank { get; set; }
    }
}
