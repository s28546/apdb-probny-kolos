using WebApplication.Models;
using System.Data.SqlClient;

namespace WebApplication.Repository;

public interface IPrescriptionRepository
{
    public Task<Prescription> AddPrescription(Prescription prescription);
    public Task<IEnumerable<DetailedPrescription>> GetPrescriptions(string? doctorName);
}