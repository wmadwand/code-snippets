using ReadyPlayerMe.Core;
using System;
using System.IO;
using UnityEngine;

namespace Project.Avatar
{
    public class AvatarTextureFile
    {
        private const string SpriteName = "bodySprite.png";

        //--------------------------------------------------------------

        public void Save(AvatarContext context)
        {
            if (context == null)
            {
                return;
            }

            try
            {
                var texture = (Texture2D)context.Data;
                byte[] textureBytes = texture.EncodeToPNG();
                var str = context.AvatarUri.LocalModelPath + SpriteName;
                File.WriteAllBytes(str, textureBytes);

                Debug.Log("File Written On Disk!");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public Texture2D Load(string path)
        {
            var bytes = File.ReadAllBytes(path + SpriteName);
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(bytes);

            return tex;
        }        
    }
}