using Azure;
using Hospital.DTOs;
using Hospital.Models;
using Hospital.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Numerics;

namespace Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly PatientRepo _patientRepo;
        public PatientController(PatientRepo patientRepo)
        {
            _patientRepo = patientRepo;
        }

        [HttpPost("patient")]
        public IActionResult AddPatient(AddUpdatePatientDTO patientDTO)
        {
            if(ModelState.IsValid)
            {
                Patient patient = new Patient()
                {
                    Name = patientDTO.Name,
                    Age = patientDTO.Age,
                    Anaesthetist = patientDTO.Anaesthetist,
                    Assistant = patientDTO.Assistant,
                    DateOfCard = patientDTO.DateOfCard,
                    DateOfOperation = patientDTO.DateOfOperation,
                    Diagnosis = patientDTO.Diagnosis,
                    FollowUpDay = patientDTO.FollowUpDay,
                    FollowUpMonth = patientDTO.FollowUpMonth,
                    FollowUpYear = patientDTO.FollowUpYear,
                    Gender = patientDTO.Gender,
                    IntraoperationComp= patientDTO.IntraoperationComp,
                    MedicalHx= patientDTO.MedicalHx,
                    Operation= patientDTO.Operation,
                    Signs = patientDTO.Signs,
                    Phone= patientDTO.Phone,
                    Symptoms = patientDTO.Symptoms
                    
                };

                _patientRepo.Insert(patient);
                _patientRepo.Save();
                return Ok("Added Successfully");
            }
            return BadRequest("Error in Adding patient please add it again");
        }


        [HttpPost("PDF/{id}")]
        public IActionResult AddPdf(int id, IFormFile? pdfFile)
        {
            if (pdfFile == null || pdfFile.Length == 0 || !pdfFile.ContentType.Equals("application/pdf"))
            {
                return BadRequest("Invalid file. Please upload a valid PDF file.");
            }

            // Retrieve the patient from the database
            Patient patient = _patientRepo.Get(id);
            if (patient == null)
            {
                return NotFound("Patient not found.");
            }

            // Generate a unique file name
            string fileName = $"{Guid.NewGuid()}_{pdfFile.FileName}";

            // Define the file path
            string filePath = Path.Combine("wwwroot", "pdfs", fileName);

            // Save the file to the file system
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                pdfFile.CopyToAsync(stream);
            }

            // Update the patient's PDF property with the file path
            patient.PDF = filePath;

            // Save changes to the database
            _patientRepo.Update(patient);
            _patientRepo.Save();

            return Ok(new { Message = "PDF uploaded successfully.", FilePath = filePath });
        }


        [HttpDelete("PDF/{id}")]
        public IActionResult DeletePdf(int id)
        {
            // Retrieve the patient
            var patient = _patientRepo.Get(id);
            if (patient == null)
            {
                return NotFound("Patient not found.");
            }

            if (string.IsNullOrEmpty(patient.PDF))
            {
                return BadRequest("No PDF associated with this patient.");
            }

            // Delete the file from the file system
            if (System.IO.File.Exists(patient.PDF))
            {
                System.IO.File.Delete(patient.PDF);
            }

            // Remove the PDF path from the database
            patient.PDF = null;
            _patientRepo.Update(patient);
            _patientRepo.Save();

            return Ok(new { Message = "PDF deleted successfully." });
        }


        [HttpGet("PDF/{id}")]
        public IActionResult GetPdf(int id)
        {
            // Retrieve the patient from the database
            Patient patient = _patientRepo.Get(id);
            if (patient == null)
            {
                return NotFound("Patient not found.");
            }

            // Check if the patient has a PDF file associated
            if (string.IsNullOrEmpty(patient.PDF))
            {
                return NotFound("No PDF file associated with this patient.");
            }

            // Ensure the file path is relative to "wwwroot"
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), patient.PDF);

            // Check if the file exists on the server
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("PDF file not found on the server.");
            }

            // Return the file to the client
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/pdf", Path.GetFileName(filePath));
        }


        [HttpDelete("patient")]
        public IActionResult DeletePatient(int id)
        {
            Patient patient = _patientRepo.Get(id);
            if (patient != null) {
                _patientRepo.delete(patient.Id);

                _patientRepo.Save();
                return Ok("Deleted Successfully");
            }
            return BadRequest("Delete Operation failed");
        }


        [HttpPost("Restore")]
        public IActionResult RestorePatient(int id)
        {
            Patient patient = _patientRepo.GetToRestore(id);
            if (patient != null)
            {
                _patientRepo.Restore(patient.Id);
                _patientRepo.Save();
                return Ok("Restored Successfully");
            }
            return BadRequest("Restore Operation failed");
        }


        [HttpPut("patient")]
        public IActionResult UpdatePatient(int id , AddUpdatePatientDTO patientDTO)
        {
            Patient patient = _patientRepo.Get(id);
            if(patient != null)
            {
                patient.Name = patientDTO.Name;
                patient.Age = patientDTO.Age;
                patient.Anaesthetist = patientDTO.Anaesthetist;
                patient.Assistant = patientDTO.Assistant;
                patient.DateOfCard = patientDTO.DateOfCard;
                patient.DateOfOperation = patientDTO.DateOfOperation;
                patient.Diagnosis = patientDTO.Diagnosis;
                patient.FollowUpDay = patientDTO.FollowUpDay;
                patient.FollowUpMonth = patientDTO.FollowUpMonth;
                patient.FollowUpYear = patientDTO.FollowUpYear;
                patient.Gender = patientDTO.Gender;
                patient.IntraoperationComp = patientDTO.IntraoperationComp;
                patient.MedicalHx = patientDTO.MedicalHx;
                patient.Operation = patientDTO.Operation;
                patient.Signs = patientDTO.Signs;
                patient.Phone = patientDTO.Phone;
                patient.Symptoms = patientDTO.Symptoms;
                _patientRepo.Update(patient);
                _patientRepo.Save();
                return Ok("Updated Successfully");
            }
            return BadRequest("Updated Failed");
          
        }


        [HttpGet("patients")]
        public IActionResult GetAllPatients()
        {
           
            List<Patient> patients = _patientRepo.GetAll();
            if (patients == null || !patients.Any())
            {
                return NotFound("No patients found.");
            }

            List<GetPatientDTO> patientDTOs = new List<GetPatientDTO>();

            foreach (Patient patient in patients)
            {
                GetPatientDTO dto = new GetPatientDTO
                {
                    Id = patient.Id,
                    Name = patient.Name,
                    Anaesthetist = patient.Anaesthetist,
                    Assistant = patient.Assistant,
                    DateOfCard = patient.DateOfCard.GetValueOrDefault(),
                    DateOfOperation = patient.DateOfOperation.GetValueOrDefault(),
                    Diagnosis = patient.Diagnosis,
                    Symptoms = patient.Symptoms,
                    FollowUpMonth = patient.FollowUpMonth.GetValueOrDefault(),
                    FollowUpDay = patient.FollowUpDay.GetValueOrDefault(),
                    FollowUpYear = patient.FollowUpYear.GetValueOrDefault(),
                    IntraoperationComp = patient.IntraoperationComp,
                    Operation = patient.Operation,
                    Phone = patient.Phone,
                    MedicalHx = patient.MedicalHx,
                    Signs = patient.Signs,
                    Gender = patient.Gender,
                    Age = patient.Age,
                    GenderString = patient.Gender == Gender.male ? "Male" : "Female",
                    HavePDF = !string.IsNullOrEmpty(patient.PDF) && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), patient.PDF))


                };

                patientDTOs.Add(dto);
            }

            return Ok(patientDTOs);
        }


        [HttpGet("DeletedPatients")]
        public IActionResult GetAllDeletedPatient()
        {          
            List<Patient> patients = _patientRepo.GetAllDeleted();

            if (patients == null || !patients.Any())
            {
                return NotFound("No patients found.");
            }          
            List<GetPatientDTO> patientDTOs = new List<GetPatientDTO>();

            foreach (Patient patient in patients)
            {
                GetPatientDTO dto = new GetPatientDTO
                {
                    Id = patient.Id,
                    Name = patient.Name,
                    Anaesthetist = patient.Anaesthetist,
                    Assistant = patient.Assistant,
                    DateOfCard = patient.DateOfCard.GetValueOrDefault(),
                    DateOfOperation = patient.DateOfOperation.GetValueOrDefault(),
                    Diagnosis = patient.Diagnosis,
                    Symptoms = patient.Symptoms,
                    FollowUpMonth = patient.FollowUpMonth.GetValueOrDefault(),
                    FollowUpDay = patient.FollowUpDay.GetValueOrDefault(),
                    FollowUpYear = patient.FollowUpYear.GetValueOrDefault(),
                    IntraoperationComp = patient.IntraoperationComp,
                    Operation = patient.Operation,
                    Phone = patient.Phone,
                    MedicalHx = patient.MedicalHx,
                    Signs = patient.Signs,
                    Gender = patient.Gender,
                    Age = patient.Age,
                    GenderString = patient.Gender == Gender.male ? "Male" : "Female"
                };

                patientDTOs.Add(dto);
            }

            return Ok(patientDTOs);
        }


        [HttpGet("patient")]
        public IActionResult GetPatientById(int id)
        {
            Patient patient = _patientRepo.Get(id);
            GetPatientDTO patientDTO = new GetPatientDTO();
            if(patient != null)
            {
                patientDTO.Id = id;
                patientDTO.Name = patient.Name;
                patientDTO.Anaesthetist = patient.Anaesthetist;
                patientDTO.Assistant = patient.Assistant;
                patientDTO.DateOfCard = patient.DateOfCard.GetValueOrDefault();
                patientDTO.DateOfOperation = patient.DateOfOperation.GetValueOrDefault();
                patientDTO.Diagnosis = patient.Diagnosis;
                patientDTO.Symptoms = patient.Symptoms;
                patientDTO.FollowUpMonth = patient.FollowUpMonth.GetValueOrDefault();
                patientDTO.FollowUpDay = patient.FollowUpDay.GetValueOrDefault();
                patientDTO.FollowUpYear = patient.FollowUpYear.GetValueOrDefault();
                patientDTO.IntraoperationComp = patient.IntraoperationComp;
                patientDTO.Operation = patient.Operation;
                patientDTO.Phone = patient.Phone;
                patientDTO.MedicalHx = patient.MedicalHx;
                patientDTO.Signs = patient.Signs;
                patientDTO.Gender = patient.Gender;
                patientDTO.Age = patient.Age;
                patientDTO.GenderString = patient.Gender == Gender.male ? "Male" : "Female";
                patientDTO.HavePDF = !string.IsNullOrEmpty(patient.PDF) && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), patient.PDF));

                //string filePath = Path.Combine(Directory.GetCurrentDirectory(), patient.PDF);

                //if (!System.IO.File.Exists(filePath))
                //{
                //    patientDTO.HavePDF = false;
                //}
                //else 
                //{ 
                //    patientDTO.HavePDF = true;
                //}

                return Ok(patientDTO);
            }
            return NotFound("Patient not found");
        }


        [HttpGet("DeletedPatient")]
        public IActionResult GetDeletedPatientById(int id)
        {
            Patient patient = _patientRepo.GetToRestore(id);
            GetPatientDTO patientDTO = new GetPatientDTO();
            if (patient != null)
            {

                patientDTO.Name = patient.Name;
                patientDTO.Anaesthetist = patient.Anaesthetist;
                patientDTO.Assistant = patient.Assistant;
                patientDTO.DateOfCard = patient.DateOfCard.GetValueOrDefault();
                patientDTO.DateOfOperation = patient.DateOfOperation.GetValueOrDefault();
                patientDTO.Diagnosis = patient.Diagnosis;
                patientDTO.Symptoms = patient.Symptoms;
                patientDTO.FollowUpMonth = patient.FollowUpMonth.GetValueOrDefault();
                patientDTO.FollowUpDay = patient.FollowUpDay.GetValueOrDefault();
                patientDTO.FollowUpYear = patient.FollowUpYear.GetValueOrDefault();
                patientDTO.IntraoperationComp = patient.IntraoperationComp;
                patientDTO.Operation = patient.Operation;
                patientDTO.Phone = patient.Phone;
                patientDTO.MedicalHx = patient.MedicalHx;
                patientDTO.Signs = patient.Signs;
                patientDTO.Gender = patient.Gender;
                patientDTO.Age = patient.Age;
                patientDTO.GenderString = patient.Gender == Gender.male ? "Male" : "Female";

                return Ok(patientDTO);
            }
            return NotFound("Patient not found");
        }

        [HttpGet("Search")]
        public IActionResult Search(string name)
        {
       
            List<Patient> patients = _patientRepo.GetByName(name);
            if (patients == null || !patients.Any())
            {
                return NotFound("Patient not found");
            }
            List<PatientSearchDTO> patientSearchDTOs = patients.Select(patient => new PatientSearchDTO
            {
                Id = patient.Id,
                Name = patient.Name,
                Age = patient.Age.ToString(),
                Gender = patient.Gender,
                GenderString = patient.Gender == Gender.male ? "Male" : "Female"
            }).ToList();

            return Ok(patientSearchDTOs);
        }

        [HttpGet("SearchDeleted")]
        public IActionResult SearchDeleted(string name)
        {

            List<Patient> patients = _patientRepo.GetDeletedByName(name);
            if (patients == null || !patients.Any())
            {
                return NotFound("Patient not found");
            }
            List<PatientSearchDTO> patientSearchDTOs = patients.Select(patient => new PatientSearchDTO
            {
                Id = patient.Id,
                Name = patient.Name,
                Age = patient.Age.ToString(),
                Gender = patient.Gender,
                GenderString = patient.Gender == Gender.male ? "Male" : "Female"
            }).ToList();

            return Ok(patientSearchDTOs);
        }

        [HttpGet("patientsNames")]
        public IActionResult GetAllPatientsNames()
        {

            List<Patient> patients = _patientRepo.GetAll();
            if (patients == null || !patients.Any())
            {
                return NotFound("No patients found.");
            }

            List<PatientsNamesDTO> patientDTOs = new List<PatientsNamesDTO>();

            foreach (Patient patient in patients)
            {
                PatientsNamesDTO dto = new PatientsNamesDTO
                {
                    Id = patient.Id,
                    Name = patient.Name
                   
                };

                patientDTOs.Add(dto);
            }

            return Ok(patientDTOs);
        }

    }
}
