using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System.Configuration;
using Microsoft.Crm.Sdk.Messages;
using System.ServiceModel;
using System.Data;
using Newtonsoft.Json;
using Microsoft.Xrm.Sdk.Messages;
using System.Globalization;

public partial class getproduct : System.Web.UI.Page
{
    private IOrganizationService _orgService;

    private static Boolean isValidConnectionString(String connectionString)
    {
        // At a minimum, a connection string must contain one of these arguments.
        if (connectionString.Contains("Url=") ||
            connectionString.Contains("Server=") ||
            connectionString.Contains("ServiceUri="))
            return true;

        return false;
    }

    private static String GetServiceConfiguration()
    {
        // Get available connection strings from app.config.
        int count = ConfigurationManager.ConnectionStrings.Count;

        // Create a filter list of connection strings so that we have a list of valid
        // connection strings for Microsoft Dynamics CRM only.
        List<KeyValuePair<String, String>> filteredConnectionStrings =
            new List<KeyValuePair<String, String>>();

        for (int a = 0; a < count; a++)
        {
            if (isValidConnectionString(ConfigurationManager.ConnectionStrings[a].ConnectionString))
                filteredConnectionStrings.Add
                    (new KeyValuePair<string, string>
                        (ConfigurationManager.ConnectionStrings[a].Name,
                        ConfigurationManager.ConnectionStrings[a].ConnectionString));
        }

        // No valid connections strings found. Write out and error message.
        if (filteredConnectionStrings.Count == 0)
        {
            Console.WriteLine("An app.config file containing at least one valid Microsoft Dynamics CRM " +
                "connection string configuration must exist in the run-time folder.");
            Console.WriteLine("\nThere are several commented out example connection strings in " +
                "the provided app.config file. Uncomment one of them and modify the string according " +
                "to your Microsoft Dynamics CRM installation. Then re-run the sample.");
            return null;
        }

        // If one valid connection string is found, use that.
        if (filteredConnectionStrings.Count == 1)
        {
            return filteredConnectionStrings[0].Value;
        }

        // If more than one valid connection string is found, let the user decide which to use.
        if (filteredConnectionStrings.Count > 1)
        {
            Console.WriteLine("The following connections are available:");
            Console.WriteLine("------------------------------------------------");

            for (int i = 0; i < filteredConnectionStrings.Count; i++)
            {
                Console.Write("\n({0}) {1}\t",
                i + 1, filteredConnectionStrings[i].Key);
            }

            Console.WriteLine();

            Console.Write("\nType the number of the connection to use (1-{0}) [{0}] : ",
                filteredConnectionStrings.Count);
            String input = Console.ReadLine();
            int configNumber;
            if (input == String.Empty) input = filteredConnectionStrings.Count.ToString();
            if (!Int32.TryParse(input, out configNumber) || configNumber > count ||
                configNumber == 0)
            {
                Console.WriteLine("Option not valid.");
                return null;
            }

            return filteredConnectionStrings[configNumber - 1].Value;

        }
        return null;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        ClientScriptManager csm = Page.ClientScript;
        string display = "";
        string prodid = Request["prodid"];
        string connectionString = GetServiceConfiguration();
        Dictionary<string, object> result = new Dictionary<string, object>();
        string json = "";
        if (connectionString != null && prodid != null)
        {
            display = "Invalid prodid";
            csm.RegisterClientScriptBlock(this.GetType(), "Pop", "alert('" + display + "');", true);

            CrmServiceClient conn = new Microsoft.Xrm.Tooling.Connector.CrmServiceClient(connectionString);
            _orgService = (IOrganizationService)conn.OrganizationWebProxyClient != null ? (IOrganizationService)conn.OrganizationWebProxyClient : (IOrganizationService)conn.OrganizationServiceProxy;            

            DataSet dataSet = new DataSet("product");
            DataTable table = new DataTable("products");
            DataColumn col_product_id = new DataColumn("productId");
            DataColumn col_product_name = new DataColumn("productName");
            DataColumn col_product_number = new DataColumn("productNumber");            
            DataColumn col_product_parent = new DataColumn("productcategory");
            DataColumn col_amount_value = new DataColumn("productprice");
            //DataColumn col_product_description = new DataColumn("description");
            DataColumn col_product_image = new DataColumn("image");
            DataColumn col_product_url = new DataColumn("url");
            table.Columns.Add(col_product_id);
            table.Columns.Add(col_product_name);
            table.Columns.Add(col_product_parent);            
            table.Columns.Add(col_product_number);
            table.Columns.Add(col_amount_value);
            //table.Columns.Add(col_product_description);
            table.Columns.Add(col_product_image);
            table.Columns.Add(col_product_url);
            dataSet.Tables.Add(table);

            QueryExpression qe = new QueryExpression("product");
            qe.ColumnSet = new ColumnSet("name", "productnumber", "price", "parentproductid", "description", "adx_partialurl");
            qe.Criteria.AddCondition("productnumber", ConditionOperator.Equal, prodid);
            EntityCollection results = _orgService.RetrieveMultiple(qe);
            if (results.Entities != null)
            {
                foreach (Entity product in results.Entities)
                {
                    DataRow newRow = table.NewRow();
                    newRow["productId"] = product.Id.ToString();
                    newRow["productName"] = product["name"];
                    newRow["productNumber"] = product["productnumber"];
                    newRow["productcategory"] = product.GetAttributeValue<EntityReference>("parentproductid").Name;
                    newRow["productprice"] = ((Money)(product["price"])).Value.ToString("0.00", CultureInfo.InvariantCulture);
                    //newRow["description"] = product["description"];
                    newRow["url"] = product["adx_partialurl"];         

                    EntityReference productRef = new EntityReference("product", new Guid(product.Id.ToString()));
                    RelationshipQueryCollection relationshipQueryColl = new RelationshipQueryCollection();
                    Relationship relationship = new Relationship();
                    relationship.SchemaName = "productsalesliterature_association";
                    QueryExpression qe2 = new QueryExpression("salesliterature");
                    qe2.ColumnSet = new ColumnSet(true);
                    relationshipQueryColl.Add(relationship, qe2);

                    RetrieveRequest retrieveRequest = new RetrieveRequest();
                    retrieveRequest.RelatedEntitiesQuery = relationshipQueryColl;
                    retrieveRequest.Target = productRef;
                    retrieveRequest.ColumnSet = new ColumnSet(true);
                    //CrmConnection connection = new CrmConnection("CRM");
                    string connectionString2 = GetServiceConfiguration();

                    //using (var _serviceProxy = new OrganizationService(connection))
                    if (connectionString2 != null)
                    {
                        //OrganizationResponse response = _serviceProxy.Execute(retrieveRequest);
                        CrmServiceClient conn2 = new Microsoft.Xrm.Tooling.Connector.CrmServiceClient(connectionString2);
                        _orgService = (IOrganizationService)conn.OrganizationWebProxyClient != null ? (IOrganizationService)conn.OrganizationWebProxyClient : (IOrganizationService)conn.OrganizationServiceProxy;
                        OrganizationResponse response = _orgService.Execute(retrieveRequest);
                        Entity product2 = (Entity)response.Results["Entity"];
                        EntityCollection ecRelatedSalesLiterature = product2.RelatedEntities[relationship];

                        if (ecRelatedSalesLiterature.Entities != null)
                        {
                            QueryExpression qe3 = new QueryExpression("salesliteratureitem");
                            qe3.ColumnSet = new ColumnSet(true);
                            qe3.Criteria.AddCondition("salesliteratureid", ConditionOperator.Equal, ecRelatedSalesLiterature.Entities[0].Id);
                            EntityCollection results2 = _orgService.RetrieveMultiple(qe3);
                            if (results2.Entities != null)
                            {
                                newRow["image"] = results2.Entities[0]["documentbody"];
                            }
                        }
                        // do whatever you want
                    }
                    table.Rows.Add(newRow);
                }
            }

            json = JsonConvert.SerializeObject(dataSet, Formatting.Indented);
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write(json);
        }
        else
        {
            display = "Invalid prodid";
            csm.RegisterClientScriptBlock(this.GetType(), "Pop", "alert('" + display + "');", true);
        }

    }
}