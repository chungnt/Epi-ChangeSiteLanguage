using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ChangeSiteLang.ViewModels;
using EPiServer.Data;
using EPiServer.DataAbstraction;
using EPiServer.DataAbstraction.Internal;
using EPiServer.PlugIn;
using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace ChangeSiteLang.Controllers
{
    [GuiPlugIn(Area = PlugInArea.AdminMenu, UrlFromModuleFolder = "ChangeSiteLang", DisplayName = "Change Site Language")]
    class ChangeSiteLangController : System.Web.Mvc.Controller
    {
        private ISiteDefinitionRepository _siteDefinitionRepository;
        private ILanguageBranchRepository _languageBranchRepository;

        public ChangeSiteLangController()
        {
            _siteDefinitionRepository = ServiceLocator.Current.GetInstance<ISiteDefinitionRepository>();
            _languageBranchRepository = ServiceLocator.Current.GetInstance<ILanguageBranchRepository>();
        }
        public ActionResult Index()
        {

            ViewBag.Sites = _siteDefinitionRepository.List();
            ViewBag.LanguageBranches = _languageBranchRepository.ListEnabled();
            
            return View();
        }

        public ActionResult ChangeSiteLanguage(SiteLangViewModel model)
        {
            return RedirectToAction("Index");
        }

        private bool ChangeSiteLang(int siteId, string lang)
        {

            return false;
        }
    }

    internal class DataAccess
    {
        public static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["EPiServerDB"].ConnectionString; }
        }
        public static DataTable GetDataTable(string strSQL, Hashtable htParams, CommandType cmdType)
        {
            SqlConnection connSQL = null;
            DataTable dtTable = new DataTable();
            SqlCommand cmdSQL = null;

            try
            {
                connSQL = new SqlConnection(ConnectionString);
                cmdSQL = new SqlCommand(strSQL, connSQL);

                // Add any parameters to the command object
                if (htParams != null)
                {
                    foreach (DictionaryEntry deEntry in htParams)
                    {
                        cmdSQL.Parameters.AddWithValue(deEntry.Key.ToString(), deEntry.Value);
                    }
                }

                cmdSQL.CommandType = cmdType;
                connSQL.Open();

                SqlDataReader drReader = cmdSQL.ExecuteReader(CommandBehavior.CloseConnection);
                dtTable.Load(drReader);

            }
            catch
            {
                throw;
            }
            finally
            {
                connSQL.Close();
                connSQL.Dispose();
                cmdSQL.Dispose();
            }

            return dtTable;

        }
        public static int ExecTransaction(IEnumerable<SqlCommand> commandList)
        {
            using (var connSQL = new SqlConnection(ConnectionString))
            {
                var results = 0;
                connSQL.Open();
                var transaction = connSQL.BeginTransaction();
                try
                {
                    foreach (var cmd in commandList)
                    {
                        if (cmd.Connection == null)
                        {
                            cmd.Connection = connSQL;
                            cmd.Transaction = transaction;
                        }
                        results += cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    return results;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return 0;
                }
                finally
                {
                    connSQL.Close();
                }
            }
        }
    }
}
