using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace ComplaintAssignmentPlugin
{
    public class ComplaintAssignment : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Get the plugin execution context
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            try
            {
                // Validate the input parameters
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity complaint = (Entity)context.InputParameters["Target"];

                    // Check if the entity is the Complaints table
                    if (complaint.LogicalName != "argroup_complaints") return;

                    // Retrieve all active inspectors
                    QueryExpression query = new QueryExpression("argroup_inspector")
                    {
                        ColumnSet = new ColumnSet("argroup_inspectorid"),
                        Criteria = new FilterExpression
                        {
                            Conditions =
                            {
                                new ConditionExpression("statecode", ConditionOperator.Equal, 0) // Active inspectors
                            }
                        }
                    };

                    EntityCollection inspectors = service.RetrieveMultiple(query);
                    tracingService.Trace($"Number of active inspectors retrieved: {inspectors.Entities.Count}");

                    if (inspectors.Entities.Count > 0)
                    {
                        // Determine the next inspector using round-robin logic
                        int lastIndex = GetLastAssignedInspectorIndex(service, tracingService);
                        int nextIndex = (lastIndex + 1) % inspectors.Entities.Count;
                        Entity nextInspector = inspectors.Entities[nextIndex];

                        // Assign the complaint to the next inspector
                        complaint["argroup_assignedinspector"] = new EntityReference("argroup_inspector", nextInspector.Id);

                        // Log the assigned inspector ID
                        tracingService.Trace($"Next Inspector ID: {nextInspector.Id}");

                        // Update the complaint in PostOperation mode only
                        if (context.Stage == 40) // PostOperation
                        {
                            service.Update(complaint);
                            tracingService.Trace("Complaint record updated with assigned inspector.");
                        }

                        // Save the last assigned inspector index
                        SaveLastAssignedInspectorIndex(service, nextIndex, tracingService);
                    }
                    else
                    {
                        tracingService.Trace("No active inspectors found.");
                        throw new InvalidPluginExecutionException("No active inspectors are available for assignment.");
                    }
                }
            }
            catch (Exception ex)
            {
                tracingService.Trace($"Error in ComplaintAssignment plugin: {ex.Message}");
                throw new InvalidPluginExecutionException($"Error assigning inspector: {ex.Message}");
            }
        }

        private int GetLastAssignedInspectorIndex(IOrganizationService service, ITracingService tracingService)
        {
            try
            {
                QueryExpression query = new QueryExpression("argroup_complaints")
                {
                    ColumnSet = new ColumnSet("argroup_assignedinspector"),
                    Orders = { new OrderExpression("createdon", OrderType.Descending) }
                };

                EntityCollection complaints = service.RetrieveMultiple(query);
                tracingService.Trace($"Number of complaints retrieved: {complaints.Entities.Count}");

                if (complaints.Entities.Count > 0)
                {
                    Entity lastComplaint = complaints.Entities.First();
                    EntityReference lastInspectorRef = lastComplaint.GetAttributeValue<EntityReference>("argroup_assignedinspector");

                    if (lastInspectorRef != null)
                    {
                        tracingService.Trace($"Last assigned inspector ID: {lastInspectorRef.Id}");
                        return complaints.Entities.IndexOf(lastComplaint);
                    }
                }

                return -1; // No prior assignments
            }
            catch (Exception ex)
            {
                tracingService.Trace($"Error retrieving last assigned inspector index: {ex.Message}");
                throw;
            }
        }

        private void SaveLastAssignedInspectorIndex(IOrganizationService service, int index, ITracingService tracingService)
        {
            // Placeholder for saving the last assigned index in Dataverse (e.g., custom configuration entity)
            tracingService.Trace($"Last assigned inspector index saved: {index}");
        }
    }
}
