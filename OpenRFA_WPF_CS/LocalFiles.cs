using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRFA_WPF_CS
{
    /// <summary>
    /// Local JSON and text file management
    /// </summary>
    public class LocalFiles
    {
        // Source for local files is saved at C:\Users\{{ user }}\AppData\Roaming\OpenRFA
        public static string localFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OpenRFA\";
        public static string localJsonFile = localFilesPath + "OpenRfaParameters.json";
        public static string localSpFile = localFilesPath + "OpenRfaParameters.txt";
        public static string dataSource = localJsonFile;

        // Fields for checking for updates
        //public static DateTime dbLastUpdated = UnixTimeStampToDateTime(000);
        //public static DateTime localFileUpdated = File.GetLastWriteTimeUtc(localJsonFile);

        // List to store JSON as list
        public static List<SharedParameter> sharedParams = new List<SharedParameter>();

        public static bool definitionsUpToDate = false;
        
        /// <summary>
        /// Creates local directory in User's Documents folder for storing parameters
        /// </summary>
        public static void CreateLocalDir()
        {
            // Download files to local after checking for local directory
            System.IO.Directory.CreateDirectory(localFilesPath);
        }

        /// <summary>
        /// Download the OpenRFA shared parameter definitions as a Json file
        /// </summary>
        public static void DownloadLocalData()
        {
            using (var webClient = new WebClient())
            {
                webClient.DownloadFile("http://openrfa.org/json", localJsonFile);
            }

            using (var client = new WebClient())
            {
                client.DownloadFile("http://openrfa.org/txt", localSpFile);
            }

            definitionsUpToDate = true;
        }


        /// <summary>
        /// Stores downloaded JSON file as list of type Shared Parameters
        /// </summary>
        public static void JsonToList()
        {
            sharedParams = SharedParameter.DownloadJsonAsList<List<SharedParameter>>(dataSource);
        }

        // Location of newly filtered shared parameters file.
        // Saves to user's temp directory: C:\Users\USERNAME\AppData\Local\Temp\
        public static string tempDefinitionsFile = Path.GetTempPath() + Guid.NewGuid().ToString() + ".txt";

        /// <summary>
        /// Converts a Unix time stamp (double) to a Date Time type
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns>The date/time as a DateTime object.</returns>
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            return dtDateTime;
        }

        public static DateTime GetLastUpdateJsonLocal()
        {
            return File.GetLastWriteTimeUtc(localJsonFile).ToLocalTime();
        }

        /// <summary>
        /// Retrieves the last date/time the OpenRFA database was udpated
        /// </summary>
        public static DateTime GetLastUpdateJsonOnline()
        {
            string _jsonString;
            string _unixTimeStamp = "";

            try
            {
                using (WebClient wc = new WebClient())
                {
                    _jsonString = wc.DownloadString(@"http://openrfa.org/json/updated");
                }

                JArray jsonArray = JArray.Parse(_jsonString);
                dynamic data = JObject.Parse(jsonArray[0].ToString());

                _unixTimeStamp = data["updated"];

                // Return local timestamp
                return UnixTimeStampToDateTime(Convert.ToDouble(_unixTimeStamp)).ToLocalTime();

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                ImportProcess.continueCommand = false;

                // Return zero'd out value
                return UnixTimeStampToDateTime(000);

            }

        }

    }
}
