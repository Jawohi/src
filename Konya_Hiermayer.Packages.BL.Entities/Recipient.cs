

namespace Konya_Hiermayer.Packages.BL.Entities
{ 
    /// <summary>
    /// 
    /// </summary>
    public partial class Recipient
    { 
        /// <summary>
        /// Name of person or company.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Street
        /// </summary>
        /// <value>Street</value>
        public string Street { get; set; }

        /// <summary>
        /// Postalcode
        /// </summary>
        /// <value>Postalcode</value>
        public string PostalCode { get; set; }

        /// <summary>
        /// City
        /// </summary>
        /// <value>City</value>
        public string City { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        /// <value>Country</value>
        public string Country { get; set; }
    }
}
