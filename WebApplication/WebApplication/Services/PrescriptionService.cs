using WebApplication.Models;
using WebApplication.Repository;

namespace WebApplication.Services;

//idealnie service zwraca tylko DTO, manipulowanie danymi z ktore przyjda z repo.
public class PrescriptionService : IPrescriptionService
{
    private IPrescriptionRepository _prescriptionRepository;

    public PrescriptionService(IPrescriptionRepository prescriptionRepository)
    {
        _prescriptionRepository = prescriptionRepository;
    }

    //przejscie z requestu na actual model ktory bedzie w bazie
    public async Task<Prescription> AddPrescription(CreatePrescriptionRequest request)
    {
        var prescription = new Prescription
        {
            IdPrescription = null,
            Date = request.Date,
            DueDate = request.DueDate,
            IdPatient = request.IdPatient,
            IdDoctor = request.IdDoctor
        };
        
        return await _prescriptionRepository.AddPrescription(prescription);
    }
    
    public async Task<IEnumerable<DetailedPrescription>> GetPrescriptions(string? doctorName)
    {
        return await _prescriptionRepository.GetPrescriptions(doctorName);
    }
}