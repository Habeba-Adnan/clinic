using Hospital.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Hospital.DTOs
{
    public class AddUpdatePatientDTO
    {
        public string Name { get; set; }
        public string Age { get; set; }

        [Column(TypeName = "nvarchar(7)")]
        public Gender Gender { get; set; }

        [RegularExpression(@"^(010|012|015|011)\d{8}$", ErrorMessage = "Phone number must start with 010, 012, 015, or 011 and be followed by 8 digits.")]
        public string Phone { get; set; }
        public string MedicalHx { get; set; }
        public DateTime DateOfOperation { get; set; }
        public DateTime DateOfCard { get; set; }
        public string Symptoms { get; set; }
        public string Signs { get; set; }
        public string Diagnosis { get; set; }
        public string Operation { get; set; }
        public string IntraoperationComp { get; set; }
        public DateTime FollowUpDay { get; set; }
        public DateTime FollowUpMonth { get; set; }
        public DateTime FollowUpYear { get; set; }
        public string Assistant { get; set; }
        public string Anaesthetist { get; set; }

    }
}
