using Hospital.DTOs;
using Hospital.Models;

namespace Hospital.Repositories
{
    public class PatientRepo
    {
        private readonly Context _context;
        public PatientRepo(Context context)
        {
            _context= context;
        }
        public void Insert(Patient patient)
        {
            if (patient != null)
            {
                _context.Add(patient);
            }
        }
        public void Save()
            { _context.SaveChanges(); }
        public Patient Get(int id) 
        {
            return _context.Patients.FirstOrDefault(p => p.Id == id && p.IsDeleted == false);
        }
        public Patient GetToRestore(int id)
        {
            return _context.Patients.FirstOrDefault(p => p.Id == id && p.IsDeleted == true);
        }
        public void Update(Patient patient)
        {
            _context.Update(patient);
        }
        public void delete(int id)
        {
            Patient patient = Get(id);
            if (patient != null) {
                patient.IsDeleted = true;
                patient.DeletionDate = DateTime.UtcNow;
                _context.Update(patient);
            }
        }
        public void HardDelete(int id)
        {
            Patient patient = _context.Patients.FirstOrDefault(p=>p.Id==id);
            if (patient != null)
            {    
                _context.Remove(patient);
               
            }
        }

        public void Restore(int id)
        {
            Patient patient = GetToRestore(id);
            if (patient != null)
            {
                patient.IsDeleted = false;
                _context.Update(patient);
            }
        }
        public List<Patient> GetAll()
        {
            return _context.Patients.Where(p=>p.IsDeleted==false).ToList();
        }
        public List<Patient> GetAllDeleted()
        {
            return _context.Patients.Where(p => p.IsDeleted == true).ToList();
        }
        public List<Patient> GetByName(string name)
        {
            string LowerName = name.ToLower();
            return _context.Patients.Where(p=>p.Name.ToLower().Contains(LowerName)).Where(p=>p.IsDeleted==false).ToList();
        }
        public List<Patient> GetDeletedByName(string name)
        {
            string LowerName = name.ToLower();
            return _context.Patients.Where(p => p.Name.ToLower().Contains(LowerName)).Where(p => p.IsDeleted == true).ToList();
        }

    }

}
