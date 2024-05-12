namespace WebApplication.Models;

public class DetailedPrescription
{
    public int IdPrescription { get; init; }
    public DateTime Date { get; init; }
    public DateTime DueDate { get; init; }
    public string PatientLastName { get; init; }
    public string DoctorLastName { get; init; }
}