using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setup.Installer.InstallHelper
{
    [RunInstaller(true)]
    public partial class AppInstaller : System.Configuration.Install.Installer
    {
        public AppInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            string serverUrl = null;
            string userId = null;
            string userDirectory = null;
            string proxyPath = null;
            //string cpConnectorName = null;
            //string cpConnetionString = null;
            //string cpUsername = null;
            //string cpPassword = null;
            //string sqlServerConnector = null;
            //string sqlServerConnectionString = null;
            //string sqlServerUsername = null;
            //string sqlServerPassword = null;
            string dataBaseName = null;
            string financeDashBoardConnectorPath = null;
            string qlikConnectorName = null;
            string misc = null;
            string headName = null;
            string appDataLoadName = null;
            string appDashBoardName = null;
            string key = null;
            string consumer = null;
            string installFolderTagret = null;
            string tmp = null;
            foreach (string keyValue in Context.Parameters.Keys)
            {
                tmp += $"{keyValue}: " + AdjustString(Context.Parameters[keyValue]) + '\n';
                switch (keyValue)
                {
                    case "serverurl":
                        {
                            serverUrl = AdjustString(Context.Parameters[keyValue]);
                            break;
                        }
                    case "userid":
                        {
                            userId = AdjustString(Context.Parameters[keyValue]);
                            break;
                        }
                    case "userdirectory":
                        {
                            userDirectory = AdjustString(Context.Parameters[keyValue]);
                            break;
                        }
                    case "proxypath":
                        {
                            proxyPath = AdjustString(Context.Parameters[keyValue]);
                            break;
                        }
                    case "headname":
                        {
                            headName = AdjustString(Context.Parameters[keyValue]);
                            break;
                        }
                    //case "cpconnectorname":
                    //    {
                    //        cpConnectorName = AdjustString(Context.Parameters[keyValue]);
                    //        break;
                    //    }
                    //case "cpconnectionstring":
                    //    {
                    //        cpConnetionString = AdjustString(Context.Parameters[keyValue]);
                    //        break;
                    //    }
                    //case "cpuser":
                    //    {
                    //        cpUsername = AdjustString(Context.Parameters[keyValue]);
                    //        break;
                    //    }
                    //case "cppassword":
                    //    {
                    //        cpPassword = AdjustString(Context.Parameters[keyValue]);
                    //        break;
                    //    }
                    //case "sqlserverconnection":
                    //    {
                    //        sqlServerConnector = AdjustString(Context.Parameters[keyValue]);
                    //        break;
                    //    }
                    //case "sqlconnectionstring":
                    //    {
                    //        sqlServerConnectionString = AdjustString(Context.Parameters[keyValue]);
                    //        break;
                    //    }
                    //case "sqlserveruser":
                    //    {
                    //        sqlServerUsername = AdjustString(Context.Parameters[keyValue]);
                    //        break;
                    //    }
                    //case "sqlserverpassword":
                    //    {
                    //        sqlServerPassword = AdjustString(Context.Parameters[keyValue]);
                    //        break;
                    //    }
                    case "databasename":
                        {
                            dataBaseName = AdjustString(Context.Parameters[keyValue]);
                            break;
                        }
                    case "financedashboardconnectorpath":
                        {
                            financeDashBoardConnectorPath = AdjustString(Context.Parameters[keyValue]);
                            break;
                        }
                    case "qlikconnectorname":
                        {
                            qlikConnectorName = AdjustString(Context.Parameters[keyValue]);
                            break;
                        }
                    case "qlikconnectionpath":
                        {
                            misc = AdjustString(Context.Parameters[keyValue]);
                            break;
                        }
                    case "appdataload":
                        {
                            appDataLoadName = AdjustString(Context.Parameters[keyValue]);
                            break;
                        }
                    case "appdashboard":
                        {
                            appDashBoardName = AdjustString(Context.Parameters[keyValue]);
                            break;
                        }
                    case "key":
                        {
                            key = AdjustString(Context.Parameters[keyValue]);
                            break;
                        }
                    case "consumer":
                        {
                            consumer = AdjustString(Context.Parameters[keyValue]);
                            break;
                        }
                    case "targetdir":
                        {
                            installFolderTagret = AdjustString(Context.Parameters[keyValue]);
                            break;
                        }
                    default:
                        break;
                }
            }

            //File.WriteAllText(@"C:\Work\ScriptExecuter\test0815.txt", tmp);

            AppHelper.Save(serverUrl,
                           userId,
                           userDirectory,
                           proxyPath,
                           headName,
                           qlikConnectorName,
                           misc,
                           dataBaseName,
                           financeDashBoardConnectorPath,
                           appDashBoardName,
                           appDataLoadName,
                           key,
                           consumer,
                           installFolderTagret);

        }

        #region Helper
        private string AdjustString(string value)
        {
            string[] parts = value.Split((char)92);
            string tmp = null;
            if (parts.Length > 2)
            {
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    if (i < parts.Length - 2)
                    {
                        tmp += parts[i].TrimStart(' ') + ((char)92);
                    }
                    else
                    {
                        tmp += parts[i];
                    }
                }
            }
            else
            {
                tmp += parts[0].TrimStart(' ');
            }
            return tmp;
        }
        #endregion
    }
}
