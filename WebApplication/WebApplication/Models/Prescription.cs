namespace WebApplication.Models;

public class Prescription
{
    public int? IdPrescription { get; init; }
    public DateTime Date { get; init; }
    public DateTime DueDate { get; init; }
    public int IdPatient { get; init; }
    public int IdDoctor { get; init; }
}