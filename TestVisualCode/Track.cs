using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestVisualCode
{
    internal class Track : IComparable
    {
        private string title = "unknown";
        public string Extension { get; set; } = ".mp3";
        public string? PathSours { get; set; }

        public string Name { get; set; } = "";

        public Track(string track_id)
        {
            TrackId = track_id;
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        public string AlbumId { get; set; } = "-1";

        public string? Artist { get; set; } 
        public string? Album { get; set; } = "unknown";
        public string? Year { get; set; } = "unknown";
        public string TrackId { get; set; } = "-1";
        public string? DataCreate { get; set; }
        public bool IsExist{get;set;}=false;



        public Track() { }

        public override string ToString()
        {
            return $"Name:{Name}, Title:{Title}, Artist:{Artist}, Album:{Album}, Year:{Year}, TrackId:{TrackId}),  AlbumId:{AlbumId}, DataCreate:{DataCreate}, IsExist:{IsExist}, Extension:{Extension}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is Track track)
            {
                return this.GetHashCode() == track.GetHashCode();
            }
            else
            {
                throw new ArgumentException("obj is not Track");
            }
        }

        public static string Data() => DateTime.Now.ToString("d");
        public override int GetHashCode()
        {
            return TrackId.GetHashCode();
        }


        public int CompareTo(object? obj)
        {
            if (obj is Track track) return Title.CompareTo(track.Title);
            else throw new ArgumentException("Некорректное значение параметра"); ;
        }
    }
}
