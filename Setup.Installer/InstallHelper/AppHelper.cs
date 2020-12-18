
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qlik.Sense.RestClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setup.Installer.InstallHelper
{
    public class AppHelper
    {
        private const string _filePath = @"C:\ProgramData\TrustedDecisions\";
        private const string _config = "Config";
        private const string _license = "License";
        private static QAppResponse response = new QAppResponse();
        public static void Save(string serverUrl, string userId, string userDirectory,
                                string proxyPath, string headName, string qlikConnectorName,
                                string misc, string dataBaseName,
                                string financeDashBoardConnectorPath,
                                string appDashBoardName,
                                string appDataLoadName,
                                string key, string consumer,
                                string installFolderTagret)
        {

            var restClient = GetRestClient(serverUrl, userDirectory, userId, headName);

            #region AutoCreateApps

            CreateApps(restClient,
                       proxyPath,
                       installFolderTagret,
                       appDataLoadName,
                       appDashBoardName);

            #endregion

            #region AutoCreateConnectors

            if (string.IsNullOrEmpty(qlikConnectorName))
            {
                qlikConnectorName = "QSE_B_Finance";
            }

            if (string.IsNullOrEmpty(financeDashBoardConnectorPath))
            {
                financeDashBoardConnectorPath = installFolderTagret + "FinanceDashboard";
            }
            else
            {
                financeDashBoardConnectorPath = financeDashBoardConnectorPath + @"\FinanceDashboard";
            }

            dynamic qlikFinanceConnectorBody = new JObject();
            qlikFinanceConnectorBody.Name = qlikConnectorName;
            qlikFinanceConnectorBody.ConnectionString = financeDashBoardConnectorPath;
            qlikFinanceConnectorBody.Type = "folder";

            dynamic qseDashBoardQvdConnector = new JObject();
            qseDashBoardQvdConnector.Name = "DashboardQVD";
            qseDashBoardQvdConnector.ConnectionString = installFolderTagret + "ExecuterQVDs";
            qseDashBoardQvdConnector.Type = "folder";

            dynamic executerConnector = new JObject();
            executerConnector.Name = "QlikScriptExecuter";
            executerConnector.ConnectionString = installFolderTagret + "QSE";
            executerConnector.Type = "folder";

            GenerateConnectors(restClient,
                               proxyPath,
                               qlikFinanceConnectorBody,
                               qseDashBoardQvdConnector,
                               executerConnector);
            #endregion

            #region CreateFolersAndFiles

            StringBuilder sbConfig = new StringBuilder();
            StringBuilder sbConnection = new StringBuilder();
            //StringBuilder sbApiInfo = new StringBuilder();

            if (!Directory.Exists(_filePath + _config))
            {
                Directory.CreateDirectory(_filePath + _config);
            }

            if (!Directory.Exists(_filePath + _license))
            {
                Directory.CreateDirectory(_filePath + _license);
            }

            if (!Directory.Exists(financeDashBoardConnectorPath))
            {
                Directory.CreateDirectory(financeDashBoardConnectorPath);
                Directory.CreateDirectory(financeDashBoardConnectorPath + @"\01_Load");
                Directory.CreateDirectory(financeDashBoardConnectorPath + @"\02_Manipulate");
                Directory.CreateDirectory(financeDashBoardConnectorPath + @"\03_Show");
            }

            Directory.CreateDirectory(installFolderTagret + @"\ExecuterQVDs");

            //if (!File.Exists(_filePath + _config + @"\apiInfo.ini"))
            //{
            //    sbApiInfo.AppendLine($"AppDataLoad={appDataLoadName}");
            //    sbApiInfo.AppendLine($"AppDashBoard={appDashBoardName}");
            //    sbApiInfo.AppendLine($"QlikConnectorName={qlikConnectorName}");
            //    sbApiInfo.AppendLine($"");
            //}

            sbConfig.AppendLine($"appId={response.Id}");
            sbConfig.AppendLine($"url={serverUrl}");
            sbConfig.AppendLine($"headname={headName}");
            sbConfig.AppendLine($"userdirectory={userDirectory}");
            sbConfig.AppendLine($"userid={userId}");
            sbConfig.AppendLine($"proxypath={proxyPath}");

            sbConnection.AppendLine($"set vSQLServerConnector = ;");
            sbConnection.AppendLine($"set vCPConnectRoot = ;");
            sbConnection.AppendLine($"set vDataBaseName = {((char)34)}{dataBaseName}{((char)34)};");
            sbConnection.AppendLine($"set vQlikConnectRoot = {qlikConnectorName};");

            string licenseFile = $"Customer={consumer};Key={key}";

            File.WriteAllText(_filePath + _config + @"\CustomInformation.ini", sbConfig.ToString(), Encoding.UTF8);
            File.WriteAllText(_filePath + _config + @"\Connection.qvs", sbConnection.ToString(), Encoding.UTF8);
            File.WriteAllText(_filePath + _license + @"\License.txt", licenseFile, Encoding.UTF8);

            #endregion
        }


        #region Helpers


        private static RestClient GetRestClient(string serverUrl, string userDirectory, string userId, string headName)
        {
            var restClient = new RestClient(serverUrl);
            restClient.CustomHeaders.Add(headName, $"{userDirectory}\\{userId}");
            restClient.AsNtlmUserViaProxy();
            return restClient;
        }

        private static void GenerateConnectors(RestClient restClient,
                                               string proxyPath,
                                               JObject qlikFinanceConnectorBody,
                                               JObject qseQvdConnector,
                                               JObject executerConnector)
        {
            restClient.WithContentType("application/json")
                .Post($"/{proxyPath}/qrs/dataconnection", qlikFinanceConnectorBody.ToString());

            restClient.WithContentType("application/json")
                .Post($"/{proxyPath}/qrs/dataconnection", qseQvdConnector.ToString());

            restClient.WithContentType("application/json")
                .Post($"/{proxyPath}/qrs/dataconnection", executerConnector.ToString());
        }

        private static void CreateApps(RestClient restClient,
                                       string proxyPath,
                                       string target,
                                       string appDataLoadName,
                                       string appDashBoardName)
        {
            if (string.IsNullOrEmpty(appDashBoardName))
            {
                appDashBoardName = "QSEFI_Dashboard";
            }
            if (string.IsNullOrEmpty(appDataLoadName))
            {
                appDataLoadName = "QSEFI_Dataload";
            }

            var dataDashBoard = File.ReadAllBytes($@"{target}QSEFI_Dashboard.qvf");
            restClient.WithContentType("application/vnd.qlik.sense.app")
                .Post($"/{proxyPath}/qrs/app/upload?keepData=true&name=" + $"{appDashBoardName}", dataDashBoard);

            var dataLoadApp = File.ReadAllBytes($@"{target}QSEFI_Dataload.qvf");
            var returnValueAppDataLoad = restClient.WithContentType("application/vnd.qlik.sense.app")
                .Post($"/{proxyPath}/qrs/app/upload?keepData=true&name=" + $"{appDataLoadName}", dataLoadApp);

            response = JsonConvert.DeserializeObject<QAppResponse>(returnValueAppDataLoad);
        }

        #endregion
    }
}
