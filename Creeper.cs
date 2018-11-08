using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using HtmlAgilityPack;
using System.Net.Http;

namespace App1
{
    static class Creeper
    {
        public static string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Headers["referer"] = "https://c.y.qq.com";   // 获取歌手简介信息要加
            request.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3493.3 Safari/537.36"; //定位资源获取有效的 key 的时候要加
            
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static string GetShortIntro(string singermid,int lines = 3)
        {
            string introduce = "";
            try
            {
                string url = "https://c.y.qq.com/splcloud/fcgi-bin/fcg_get_singer_desc.fcg?outCharset=utf-8&format=xml&singermid=" + singermid;
                XElement responsexml = XElement.Parse(Get(url));
                XElement basic = responsexml.Element("data").Element("info").Element("basic");

                Int16 num = 0;
                foreach (var item in basic.Elements("item"))
                {
                    if (item.Element("key").Value.ToString() != "中文名" && item.Element("key").Value.ToString() != "别名")
                    {
                        introduce += item.Element("key").Value.ToString() + ":";
                        introduce += item.Element("value").Value.ToString() + " ";
                        if (introduce.Length > 18 || num == 1)
                        {
                            introduce += "\n";
                        }
                        num++;
                    }
                    if (num >= lines) break;
                }

                if (num == 0)    // no <basic></basic> info
                {
                    introduce = responsexml.Element("data").Element("info").Element("desc").Value.ToString().Substring(0,57) + "···";
                }
            }
            catch (Exception) { }

            return introduce;
        }

        public static SearchData GetSearchResult(string keyword)
        {
            //string url = "h ttps://c.y.qq.com/soso/fcgi-bin/client_search_cp?&t=0&aggr=1&cr=1&catZhida=1&lossless=0&flag_qc=0&p=1&n=20&w=" + keyword;

            //cr=1        热度排序
            //catZhida=1  搜索结果包含简介
            //n=20        20个搜索结果
            string url = "https://c.y.qq.com/soso/fcgi-bin/client_search_cp?cr=1&catZhida=1&n=20&w=" + keyword;

            string pageresponse = Get(url);
            JObject PageJson = JObject.Parse(pageresponse.Substring(9, pageresponse.Length - 10));
            SearchData searchresult = new SearchData();

            if (PageJson["message"].ToString() == "")
            {
                var info = PageJson["data"]["zhida"];
                searchresult.infomation.Add("inputkey", keyword);  //记录搜索关键词
                string _type = info["type"].Type.ToString();

                if (info["type"].ToString() != "")
                {
                    int resulttype = Convert.ToInt32(info["type"].ToString());
                    searchresult.infomation.Add("infotype", resulttype.ToString());
                    switch (resulttype)
                    {
                        case 1:   //singer
                            searchresult.infomation.Add("singerPic", info["zhida_singer"]["singerPic"].ToString());
                            searchresult.infomation.Add("singerName", info["zhida_singer"]["singerName"].ToString());
                            searchresult.infomation.Add("songNum", info["zhida_singer"]["songNum"].ToString());
                            searchresult.infomation.Add("albumNum", info["zhida_singer"]["albumNum"].ToString());
                            searchresult.infomation.Add("mvNum", info["zhida_singer"]["mvNum"].ToString());
                            searchresult.infomation.Add("singerMID", info["zhida_singer"]["singerMID"].ToString());
                            break;

                        case 2:   //album
                            searchresult.infomation.Add("albumName", info["zhida_album"]["albumName"].ToString());
                            searchresult.infomation.Add("albumPic", info["zhida_album"]["albumPic"].ToString());
                            searchresult.infomation.Add("singerName", info["zhida_album"]["singerName"].ToString());
                            searchresult.infomation.Add("publicTime", info["zhida_album"]["publicTime"].ToString());
                            break;

                        case 4:   //TV
                            searchresult.infomation.Add("singerName", info["zhida_tv"]["singerName"].ToString());
                            searchresult.infomation.Add("singerPic", info["zhida_tv"]["singerPic"].ToString());
                            break;

                        case 8:   //song
                            searchresult.infomation.Add("title", info["zhida_mv"]["title"].ToString());   //song name
                            searchresult.infomation.Add("desc", info["zhida_mv"]["desc"].ToString());     //singer
                            searchresult.infomation.Add("pic", info["zhida_mv"]["pic"].ToString());       //mv cover
                            searchresult.infomation.Add("publish_date", info["zhida_mv"]["publish_date"].ToString());
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    searchresult.infomation.Add("infotype", "null");
                }
            }

            var songlist = (JArray)PageJson["data"]["song"]["list"];
            for (int i = 0; i < songlist.Count; i++)
            {
                var song = songlist[i];
                if (song["songmid"].ToString() != "" && song["media_mid"].ToString() != "")  //有效结果
                {
                    Song ThisSong = new Song(song["songname"].ToString(),
                                             song["albumname"].ToString(),
                                             Convert.ToInt32(song["interval"].ToString()),
                                             song["albummid"].ToString());

                    var singers = (JArray)song["singer"];
                    foreach (var singer in singers)
                    {
                        ThisSong.addSinger(singer["name"].ToString());
                    }
                    ThisSong.setmids(song["songmid"].ToString(), song["media_mid"].ToString());
                    searchresult.SearchResult.Add(ThisSong);
                }
            }
            return searchresult;
        }

        public static List<Singer> GetSingers()
        {
            List<Singer> singers = new List<Singer>();
            string singersurl = "https://u.y.qq.com/cgi-bin/musicu.fcg?format=jsonp&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq&needNewCode=0&data=%7B%22comm%22%3A%7B%22ct%22%3A24%2C%22cv%22%3A10000%7D%2C%22singerList%22%3A%7B%22module%22%3A%22Music.SingerListServer%22%2C%22method%22%3A%22get_singer_list%22%2C%22param%22%3A%7B%22area%22%3A-100%2C%22sex%22%3A-100%2C%22genre%22%3A-100%2C%22index%22%3A-100%2C%22sin%22%3A0%2C%22cur_page%22%3A1%7D%7D%7D";
            JObject Pagejson = JObject.Parse(Get(singersurl));
            var singerlist = Pagejson["singerList"]["data"]["singerlist"];
            for (int i = 0; i < 15; i++)
            {
                singers.Add(new Singer(singerlist[i]["singer_name"].ToString(), singerlist[i]["singer_pic"].ToString().Replace(".webp", ".jpg")));
            }
            return singers;
        }

        public static string GetTopSongsURL(string type, int topid)
        {
            if (type == "top")
            {
                int day = (DateTime.Now.DayOfYear - 4) / 7;
                if (DateTime.Now.Hour < 12)
                {
                    day--;
                }
                return "https://szc.y.qq.com/v8/fcg-bin/fcg_v8_toplist_cp.fcg?type=top&song_num=30&date=" + DateTime.Now.Year + "_" + day + "&topid=" + topid;
            }
            else
            {
                return Task.Run(() => GetGlobalList_url(topid)).Result;
            }
        }

        public async static Task<string> GetGlobalList_url(int topid)
        {
            string Res = "https://y.qq.com/n/yqq/toplist/" + topid + ".html";
            string datestring = "2018_36";

            // instance or static variable
            HttpClient client = new HttpClient();

            // get answer in non-blocking way
            using (var response = await client.GetAsync(Res))
            {
                using (var content = response.Content)
                {
                    // read answer in non-blocking way
                    var result = await content.ReadAsStringAsync();
                    var document = new HtmlDocument();
                    document.LoadHtml(result);
                    var nodes = document.DocumentNode.SelectNodes("//script");

                    foreach (var node in nodes)
                    {
                        if (node.InnerText.Contains("var yearList"))
                        {
                            string script = node.InnerText;
                            var regex = new Regex(".*thisList = (.*);.*");

                            if (regex.IsMatch(script))
                            {
                                datestring = regex.Match(script).Groups[1].Value;
                                datestring = datestring.Insert(4, "_");
                            }
                            break;
                        }
                    }
                }
            }

            return "https://c.y.qq.com/v8/fcg-bin/fcg_v8_toplist_cp.fcg?type=global&song_num=30&date=" + datestring + "&topid=" + topid;
        }

        public static List<Song> GetTopSongs(string type,int topid)
        {
            List<Song> mysonglist = new List<Song>();
            string listurl = GetTopSongsURL(type,topid);
            try
            {
                JObject Pagejson = JObject.Parse(Get(listurl));
                var songlist = Pagejson["songlist"];
                foreach (var song in songlist)
                {
                    Song ThisSong = new Song(song["data"]["songorig"].ToString(),
                                             song["data"]["albumname"].ToString(),
                                             Convert.ToInt32(song["data"]["interval"].ToString()),
                                             song["data"]["albummid"].ToString());

                    var singers = (JArray)song["data"]["singer"];
                    foreach (var singer in singers)
                    {
                        ThisSong.addSinger(singer["name"].ToString());
                    }
                    mysonglist.Add(ThisSong);
                    ThisSong.setmids(song["data"]["songmid"].ToString(), song["data"]["strMediaMid"].ToString());
                }
            }
            catch (Exception) { } 
            return mysonglist;
        }

        private static List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>() {
                       new KeyValuePair<string, string>("M800", ".mp3"),
                       new KeyValuePair<string, string>("C600", ".m4a"),
                       new KeyValuePair<string, string>("M500", ".mp3"),
                       new KeyValuePair<string, string>("C400", ".m4a"),
                       new KeyValuePair<string, string>("C200", ".m4a"),
                       new KeyValuePair<string, string>("C100", ".m4a"),
                   };

        public static string GetSongResourseURL(string songmid)
        {
            string NoResource = "shit";
            //get value key
            // guid is a random number % 10000000000 (10 chars)
            string getkeyurl = "https://c.y.qq.com/base/fcgi-bin/fcg_musicexpress.fcg?json=3&guid=3757070001&format=json";
            JObject PageJson = JObject.Parse(Get(getkeyurl));
            string key = PageJson["key"].ToString();

            foreach (var pair in list)
            {
                string testurl = "https://dl.stream.qqmusic.qq.com/" + pair.Key + songmid + pair.Value + "?vkey=" + key + "&guid=3757070001&uid=0&fromtag=30";
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(testurl);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    return pair.Value + testurl;
                }
                catch (System.Exception) { }
            }
            return NoResource;
        }

        public static List<string> GetLyrics(string songmid)
        {
            string lyc = "https://c.y.qq.com/lyric/fcgi-bin/fcg_query_lyric_new.fcg?g_tk=5381&songmid=" + songmid;
            string response = Get(lyc);

            JObject PageJson = JObject.Parse(response.Substring(18, response.Length - 19));

            List<string> Lyrics = new List<string> { Base64Decode(PageJson["lyric"].ToString()),Base64Decode(PageJson["trans"].ToString())};
            return Lyrics;
        }

        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string GetFromiTunes(string searchkey)
        {
            string iTunes = "https://itunes.apple.com/search?entity=musicVideo&limit=2&term=" + searchkey;

            JObject PageJson = JObject.Parse(Get(iTunes));
            
            foreach (var item in PageJson["results"])
            {
                return item["previewUrl"].ToString();
            }
            return "novideo";
            //string url = "h ttps://video-ssl.itunes.apple.com/apple-assets-us-std-000001/Video128/v4/f0/c9/13/f0c913b9-d0a7-1f7f-dd1a-4720742c0135/mzvf_3773588944266911009.640x478.h264lc.U.p.m4v";
        }

        public static string GetWeather()
        {
            string weather = "http://www.weather.com.cn/data/sk/101020500.html";  // 101020500 嘉定

            JObject PageJson = JObject.Parse(Get(weather));

            return PageJson["weatherinfo"]["WD"].ToString() + " " + PageJson["weatherinfo"]["WS"].ToString();

        }
    }
}