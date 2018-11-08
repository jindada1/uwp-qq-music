using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1
{
    public class Song
    {
        public string name;
        public string album;
        public string album_url;
        public string singers;
        public string interval;
        public string songmid;
        public string media_mid;
        public int timeint;

        public Song(string _name, string _album,int _interval,string _album_mid = "")
        {
            name = _name;
            album = _album;

            album_url = "https://y.gtimg.cn/music/photo_new/T002R300x300M000" + _album_mid + ".jpg?max_age=2592000";

            timeint = _interval;

            if (_interval % 60 > 9)
            {
                interval = (_interval / 60).ToString() + ":" + (_interval % 60).ToString();
            }
            else
            {
                interval = (_interval / 60).ToString() + ":0" + (_interval % 60).ToString();
            }
        }

        public void addSinger(string singername)
        {
            singers += singername + " ";
        }
        public void setmids(string _songmid,string _media_mid)
        {
            songmid = _songmid;
            media_mid = _media_mid;
        }
    }

    public class Singer
    {
        public string name;
        public string profile_url;

        public Singer(string _name,string _url)
        {
            name = _name;
            profile_url = _url;
        }
    }
}
