using WebApplication.Models;

namespace WebApplication.Repository;

//idealnie repo nie zwraca DTO, zwraca faktyczne modele z bazy, dzialanie z baza
public class PrescriptionRepository : IPrescriptionRepository
{
    private readonly string _connectionString;

    public PrescriptionRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("default");
    }

    public async Task<IEnumerable<DetailedPrescription>> GetPrescriptions(string? doctorName)
    {
        var prescriptions = new List<DetailedPrescription>();

        await using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            string baseQuery = @"
                SELECT IdPrescription, Date, DueDate, Patient.LastName AS PatientLastName, Doctor.LastName AS DoctorLastName
                FROM Prescription
                INNER JOIN Patient ON Prescription.IdPatient = Patient.IdPatient
                INNER JOIN Doctor ON Prescription.IdDoctor = Doctor.IdDoctor
            ";

            if (!string.IsNullOrWhiteSpace(doctorName))
            {
                baseQuery += " WHERE Doctor.LastName = @DoctorName ";
            }

            baseQuery += " ORDER BY Date DESC;";

            await using (var command = new SqlCommand(baseQuery, connection))
            {
                if (!string.IsNullOrWhiteSpace(doctorName))
                {
                    command.Parameters.AddWithValue("@DoctorName", doctorName);
                }

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var prescription = new DetailedPrescription(
                            IdPrescription: reader.GetInt32(reader.GetOrdinal("IdPrescription")),
                            Date: reader.GetDateTime(reader.GetOrdinal("Date")),
                            DueDate: reader.GetDateTime(reader.GetOrdinal("DueDate")),
                            PatientLastName: reader.GetString(reader.GetOrdinal("PatientLastName")),
                            DoctorLastName: reader.GetString(reader.GetOrdinal("DoctorLastName"))
                        );
                        prescriptions.Add(prescription);
                    }
                }
            }
        }

        return prescriptions;
    }

    public async Task<Prescription> AddPrescription(Prescription prescription)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string insertQuery = @"
            INSERT INTO Prescription (Date, DueDate, IdPatient, IdDoctor)
            VALUES (@Date, @DueDate, @IdPatient, @IdDoctor);
            SELECT CAST(SCOPE_IDENTITY() as int);
        ";

        await using var insertCommand = new SqlCommand(insertQuery, connection);
        insertCommand.Parameters.AddWithValue("@Date", prescription.Date);
        insertCommand.Parameters.AddWithValue("@DueDate", prescription.DueDate);
        insertCommand.Parameters.AddWithValue("@IdPatient", prescription.IdPatient);
        insertCommand.Parameters.AddWithValue("@IdDoctor", prescription.IdDoctor);

        var newPrescriptionId = (int)await insertCommand.ExecuteScalarAsync();

        const string selectQuery = @"
            SELECT IdPrescription, Date, DueDate, IdPatient, IdDoctor
            FROM Prescription
            WHERE IdPrescription = @IdPrescription;
        ";

        await using var selectCommand = new SqlCommand(selectQuery, connection);
        selectCommand.Parameters.AddWithValue("@IdPrescription", newPrescriptionId);

        using var reader = await selectCommand.ExecuteReaderAsync();
        if (reader.Read())
        {
            var res = new Prescription
            {
                IdPrescription = reader.GetInt32(reader.GetOrdinal("IdPrescription")),
                Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                DueDate = reader.GetDateTime(reader.GetOrdinal("DueDate")),
                IdPatient = reader.GetInt32(reader.GetOrdinal("IdPatient")),
                IdDoctor = reader.GetInt32(reader.GetOrdinal("IdDoctor"))
            };

            return res;
        }

        throw new Exception("Failed to retrieve the new prescription data.");
    }
}

/*
 *  zeby connection string dzialal trzeba go dodac w appsettings.json:
 *  "ConnectionStrings": {
 *      "default": ""
 *  }
 */
