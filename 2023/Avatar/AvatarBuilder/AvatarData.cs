using ReadyPlayerMe.Core;
using UnityEngine;

namespace Project.Avatar
{
    public class AvatarData
    {
        //public AvatarModel model;
        public GameObject model;
        public Sprite sprite;
        public string url;
        public AvatarMetadata Metadata;

        public AvatarData(string url)
        {
            this.url = url;
        }
    }

    //public class AvatarModel
    //{
    //    public string Url { get; set; }

    //    public GameObject Avatar { get; set; }

    //    public AvatarMetadata Metadata { get; set; }
    //}
}
