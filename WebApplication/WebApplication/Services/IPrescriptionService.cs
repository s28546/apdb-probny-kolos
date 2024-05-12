using WebApplication.Models;

namespace WebApplication.Services;

public interface IPrescriptionService
{
    public Task<Prescription> AddPrescription(CreatePrescriptionRequest request);
    public Task<IEnumerable<DetailedPrescription>> GetPrescriptions(string? doctorName);
}