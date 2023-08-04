namespace WypozyczalniaAPI.Services
{
    public class RentalEvent
    {  
        public string Name {get; set;}
        public int Rentid {get; set;}   
        public int Customerid {get; set;}
        public int Bookid { get; set; }
        public DateTime RentDate { get; set; }
        public DateTime? ReturnDate {get; set;} 
       

        
    }
}