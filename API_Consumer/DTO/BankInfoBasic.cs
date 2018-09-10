namespace SQLAPI_Consumer
{
    /// <summary>
    /// Sample DTO: Used to return BankInfo data.
    /// </summary>
    public class BankInfoBasic
    {
        public string Name { get; set; }
        public string rn { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
