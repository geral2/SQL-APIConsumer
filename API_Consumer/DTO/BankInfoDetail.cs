namespace SQLAPI_Consumer
{
    /// <summary>
    /// Sample DTO: Used to return BankInfoDetailed data.
    /// </summary>
    public class BankInfoDetail
    {
            public string Institution_status_code { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string New_routing_number { get; set; }
            public string Office_code { get; set; }
            public string Message { get; set; }
            public string Zip { get; set; }
            public string Routing_number { get; set; }
            public int Code { get; set; }
            public string Data_view_code { get; set; }
            public string Telephone { get; set; }
            public string Customer_name { get; set; }
            public string Record_type_code { get; set; }
            public string Change_date { get; set; }
            public string rn { get; set; }
    }
}
