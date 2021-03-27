namespace DataLayer
{
    public class Address
    {
        public string AddressType { get; set; }
        public string City { get; set; }
        public int ContactId { get; set; }
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public string PostalCode { get; set; }
        public int StateId { get; set; }
        public string StreetAddress { get; set; }
        internal bool IsNew => (this.Id == default(int));
    }
}