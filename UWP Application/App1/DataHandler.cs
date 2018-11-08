using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace App1
{
    public static class DataHandler
    {
        public static async Task SaveAsJSONAsync()
        {
            ConfigData localdata = new ConfigData();

            localdata.AutoPlay = App.AutoPlay;
            localdata.DeleteCache = App.DeleteCache;
            localdata.AutoSaveSearch = App.AutoSaveSearch;
            localdata.HistoricalSearch = App.historysearch;
            localdata.MyDownLoadFolder = App.MyDownLoadFolder;

            string json = JsonConvert.SerializeObject(localdata);

            try                 // access configdata.json
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appdata:///local/" + App.configfilename));
                await FileIO.WriteTextAsync(file, json);
            }
            catch (Exception)   // create configdata.json
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(App.configfilename);
                await FileIO.WriteTextAsync(file, json);
            }

        }

        public static async Task LoadFromJSONAsync()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

            try
            {
                StorageFile ConfigFile = await storageFolder.GetFileAsync(App.configfilename);
                string configtext = await FileIO.ReadTextAsync(ConfigFile);
                ConfigData localdata = JsonConvert.DeserializeObject<ConfigData>(configtext);

                if (localdata != null)
                {
                    if (localdata.HistoricalSearch != null)
                    {
                        App.historysearch = localdata.HistoricalSearch;
                    }
                    App.MyDownLoadFolder = localdata.MyDownLoadFolder;
                    App.AutoPlay         = localdata.AutoPlay      ;
                    App.DeleteCache      = localdata.DeleteCache   ;
                    App.AutoSaveSearch   = localdata.AutoSaveSearch;
                }
            }
            catch (Exception)    // first open in user's compute
            {
                // create configdata.json
                await storageFolder.CreateFileAsync(App.configfilename);

                // init download folder, add folder in FutureAccessList with token (App.MyDownLoadFolder)
                StorageFolder newFolder = await DownloadsFolder.CreateFolderAsync("Music", CreationCollisionOption.GenerateUniqueName);
                StorageApplicationPermissions.FutureAccessList.AddOrReplace(App.MyDownLoadFolder, newFolder);
            }
        }
    }

    public class ConfigData
    {
        public ConfigData() { }
        public string MyDownLoadFolder;
        public bool AutoPlay;
        public bool DeleteCache;
        public bool AutoSaveSearch;
        public List<Dictionary<string, string>> HistoricalSearch;
    }

    public class SearchData
    {
        /*
        public string singername;
        public string singerprofileurl;
        public int songnumber;
        public int albumnumber;
        public int mvnumber;
        */
        public Dictionary<string, string> infomation = new Dictionary<string, string>();
        public List<Song> SearchResult = new List<Song>();
    }
}
