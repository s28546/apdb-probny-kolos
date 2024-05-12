namespace WebApplication.Models;

public record CreatePrescriptionRequest(DateTime Date, DateTime DueDate, int IdPatient, int IdDoctor);