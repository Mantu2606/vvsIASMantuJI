using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FossTech.Models
{
    public class BusinessProfile : ITimeStampedModel
    {
        public int Id { get; set; }

        [DisplayName("Business Name")]
    
        public string BusinessName{ get; set; }
        public string Title { get; set; }

        [DisplayName("Registered Email Id")]
      
        public string EmailAddress { get; set; }

        [DisplayName("Alternate Email Id")]

        public string AlternateEmailAddress { get; set; }

        [DisplayName("Alternate Email Id 1")]

        public string AlternateEmailAddress1 { get; set; }

        [DisplayName("Registered Contact Number")]
   
        public string RegisteredContactNumber{ get; set; }

        [DisplayName("Alternate Contact Number")]
        public string AlternateContactNumber { get; set; }

        [DisplayName("Alternate Contact Number 1")]
        public string AlternateContactNumber1 { get; set; }

        [DisplayName("WhatsApp Number")]
        public string WhatsAppNumber { get; set; }

        [DisplayName("Business WhatsApp Number")]
        public string BusinessWhatsAppNumber { get; set; }

        [DisplayName("Business WhatsApp Number 1")]
        public string BusinessWhatsAppNumber1 { get; set; }


        
       
        [DisplayName("Facebook Page Link")]
        public string FacebookPageLink { get; set; }

        [DisplayName("Instagram Page Link")]
        public string InstagramPageLink { get; set; }

        [DisplayName("Twitter Link")]
        public string TwitterLink { get; set; }

        [DisplayName("LinkedIn Link")]
        public string LinkedInLink { get; set; }

        [DisplayName("Youtube Link")]
        public string YoutubeLink { get; set; }

   

        [DisplayName("Other Website")]
        public string OtherWebsite { get; set; }

        [DisplayName("Google Map Iframe")]
        public string GoogleMap { get; set; }


        [DisplayName("Working Hours")]
        public string WorkingHours { get; set; }

        public string Copyright { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public int? SectionId { get;  set; }
        public Section Section { get; set; }
    }
}
