using Clinic.Entities;
using Clinic.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Clinic.DAL.Repositories
{
    public class clsAppointmentRepository
    {
        public int BookAppointment(Appointment app)
        {
            string query = @"INSERT INTO Appointments 
                            (PatientID, DoctorID, AppointmentDate, DurationMinutes, Status, ReasonForVisit, CreatedBy)
                            VALUES (@PatID, @DocID, @Date, @Duration, @Status, @Reason, @CreatedBy);
                            SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = {
                new SqlParameter("@PatID", app.PatientId),
                new SqlParameter("@DocID", app.DoctorId),
                new SqlParameter("@Date", app.AppointmentDate),
                new SqlParameter("@Duration", app.DurationMinutes),
                new SqlParameter("@Status", (int)app.Status),
                new SqlParameter("@Reason", (object)app.ReasonForVisit ?? DBNull.Value),
                new SqlParameter("@CreatedBy", app.CreatedBy)
            };

            object result = DBHelper.ExecuteScalar(query, parameters, DBHelper.GetOpenConnection());
            return result != null ? Convert.ToInt32(result) : -1;
        }

        public int GetConflictCount(int doctorId, DateTime startDate, int durationMinutes, int? excludeAppointmentId = null)
        {
            DateTime endDate = startDate.AddMinutes(durationMinutes);

            string query = @"SELECT COUNT(1) FROM Appointments 
                             WHERE DoctorID = @DocID 
                             AND Status != @CancelledStatus
                             AND (@ExcludeID IS NULL OR AppointmentID != @ExcludeID)
                             AND (
                                (@Start < DATEADD(MINUTE, DurationMinutes, AppointmentDate) AND @End > AppointmentDate)
                             )";

            SqlParameter[] parameters = {
                new SqlParameter("@DocID", doctorId),
                new SqlParameter("@Start", startDate),
                new SqlParameter("@End", endDate),
                new SqlParameter("@CancelledStatus", (int)enStatus.Canceled),
                new SqlParameter("@ExcludeID", (object)excludeAppointmentId ?? DBNull.Value)
            };

            object result = DBHelper.ExecuteScalar(query, parameters, DBHelper.GetOpenConnection());
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public int UpdateStatus(int appointmentId, enStatus newStatus, int updatedBy)
        {
            string query = @"UPDATE Appointments 
                             SET Status = @Status, LastUpdatedAt = GETDATE(), LastUpdatedBy = @UpdatedBy 
                             WHERE AppointmentID = @ID";

            SqlParameter[] parameters = {
                new SqlParameter("@ID", appointmentId),
                new SqlParameter("@Status", (int)newStatus),
                new SqlParameter("@UpdatedBy", updatedBy)
            };
            return DBHelper.ExecuteNonQuery(query, parameters, DBHelper.GetOpenConnection());
        }

        public DataTable GetAllAppointments()
        {
            string query = "SELECT * FROM Appointments ORDER BY AppointmentDate DESC";
            return DBHelper.ExecuteQuery(query, null, DBHelper.GetOpenConnection());
        }
    }
}