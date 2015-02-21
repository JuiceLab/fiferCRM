using Settings;
using System;
using System.Activities;
using System.Activities.DurableInstancing;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachine.Accessor
{
    public class BaseAccessor
    {
        protected int _destinationSource = 0;

        //Workflow Identity for run state machines
        public WorkflowApplicationInstance IdentityInstance { get; set; }
        private const string FindExistingInstancesSql = @"SELECT InstanceId, Value1      
            FROM [System.Activities.DurableInstancing].[InstancePromotedProperties]
            WHERE PromotionName = @PromotionName 
            AND Value1 = @IdWF";
        //Dictionary of workflow versions.
        public Dictionary<WorkflowIdentity, System.Activities.Activity> WorkflowVersionMap { get; set; }

        //acessFilter from db reflection's objInfoModel.
        //public virtual T LoadDataModel<T>(Guid activationID) where T : class
        //{
        //    T obj = default(T);

        //    using (var context = new dbEntities(Connection.String))
        //    {
        //        obj = context.GetUnitById<T>(activationID);

        //        if (null == obj)
        //    }

        //    return obj;
        //}


        /// <summary>
        /// default workflow identity loader
        /// </summary>
        /// <param name="identity">Guid for identity.</param>
        /// <param name="store">Persitance store for extract identity.</param>
        /// <returns></returns>
        public virtual KeyValuePair<WorkflowIdentity, System.Activities.Activity> LoadVersionedWF(Guid identity, SqlWorkflowInstanceStore store)
        {
            if (identity != Guid.Empty)
            {
                IdentityInstance = WorkflowApplication.GetInstance(identity, store);
                if (IdentityInstance.DefinitionIdentity != null && WorkflowVersionMap.ContainsKey(IdentityInstance.DefinitionIdentity))
                {
                    return new KeyValuePair<WorkflowIdentity, System.Activities.Activity>(IdentityInstance.DefinitionIdentity, WorkflowVersionMap[IdentityInstance.DefinitionIdentity]);
                }
            }
            return new KeyValuePair<WorkflowIdentity, System.Activities.Activity>();
        }


        /// <summary>
        /// Gets the instance id from processing object id.
        /// </summary>
        /// <param name="objId">The obj id.</param>
        /// <param name="promotionName">Promotion name for sqlInstance persistance.</param>
        /// <returns></returns>
        public Guid GetInstanceIdFromObjId(Guid objId, string promotionName)
        {
            var guid = Guid.Empty;
            // Get Workflow ID of user by querying promoted properties
            using (
                var connection =
                    new SqlConnection(AccessSettings.LoadSettings().InstanceStore))
            {
                var command = new SqlCommand(FindExistingInstancesSql, connection);

                command.Parameters.AddWithValue("@PromotionName", promotionName);
                command.Parameters.AddWithValue("@IdWF", objId.ToString());
                connection.Open();
                var dataReader = command.ExecuteReader();
                dataReader.Read();

                guid = dataReader.HasRows ? dataReader.GetGuid(0) : Guid.Empty;
                connection.Close();
            }
            return guid;
        }
    }
}
