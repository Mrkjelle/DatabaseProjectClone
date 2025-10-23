using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
using DatabaseClient.Models.Org;
using DatabaseClient.Models.Proj;

namespace DatabaseClient.Data;

public partial class CrossRepository
{
    public List<Division> GetDivisionsForProject(int projectId)
    {
        EnsureBothConnections();
        try
        {
            var divisions = new List<Division>();

            using var reader = SqlServerConnection.ExecuteStoredProcedureReader(
                _projectConnection,
                "GetDivisionsByProject",
                new Microsoft.Data.SqlClient.SqlParameter("@ProjectFK", projectId)
            );

            if (reader == null)
            {
                throw new DataException("No data returned from the database.");
            }

            while (reader.Read())
            {
                int divId = reader.GetInt32(reader.GetOrdinal("DivisionFK"));
                var divDetails = _orgRepo.GetDivisionById(divId);
                if (divDetails != null)
                {
                    divisions.Add(divDetails);
                }
            }
            return divisions;
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException(
                $"Error retrieving divisions for the specified project: {projectId}",
                ex
            );
        }
    }

    public void AddDivisionToProject(int projectId, int divisionId)
    {
        EnsureBothConnections();
        try
        {
            SqlServerConnection.ExecuteStoredProcedureSimple(
                _projectConnection,
                "AddDivisionToProject",
                new Microsoft.Data.SqlClient.SqlParameter("@ProjectFK", projectId),
                new Microsoft.Data.SqlClient.SqlParameter("@DivisionFK", divisionId)
            );
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException(
                $"Error adding division {divisionId} to project {projectId}.",
                ex
            );
        }
    }

    public void RemoveDivisionFromProject(int ProjectFK, int DivisionFK)
    {
        EnsureBothConnections();
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                Timeout = TransactionManager.DefaultTimeout,
            },
            TransactionScopeAsyncFlowOption.Enabled
        );
        try
        {
            var employees = GetEmployeesByProject(ProjectFK);
            foreach (var emp in employees)
            {
                if (emp.DivisionID == DivisionFK)
                {
                    RemoveEmployeeFromProject(ProjectFK, emp.EmpID);
                }
            }
            SqlServerConnection.ExecuteStoredProcedureSimple(
                _projectConnection,
                "RemoveDivisionFromProject",
                new Microsoft.Data.SqlClient.SqlParameter("@ProjectFK", ProjectFK),
                new Microsoft.Data.SqlClient.SqlParameter("@DivisionFK", DivisionFK)
            );
            scope.Complete();
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw new DataException(
                $"Error removing division {DivisionFK} from project {ProjectFK}.",
                ex
            );
        }
    }
}
