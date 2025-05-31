using Hospital.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Hospital.DTOs
{
    public class PatientSearchDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Age { get; set; }
        [JsonIgnore]
        public Gender Gender { get; set; }

        [NotMapped]
        public string GenderString { get; set; }

    }
}
