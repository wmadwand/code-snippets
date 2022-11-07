/*
The MIT License (MIT)

Copyright (c) 2013-2015 Banbury & Play-Em

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using PhotoshopFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Specialized;
//using Scripts.Scenes.HOG.Item;
//using Scripts.Core.Components;
//using Scripts.CoreUnity.PatternRealizations.StateMachine;
//using Scripts.Common;
//using Scripts.Scenes.HOG;
using Scripts.Common;
using Scripts.Scenes.Comics;

public class PSDParserWindow : EditorWindow
{
    private const string GAME_OBJECT_HOG_OBJECT_FULL = "object_full";
    private const string TAG_HOG_ITEM = "HOG_Item";
    private const string TAG_HOG_OBJECT = "HOG_Object";
    private const string TAG_COMICS_ITEM = "COMICS_item";
    private const string TAG_COMICS_OBJECT = "COMICS_object";

    private Texture2D image;
    private PsdFile psd;
    private string fileName = "";

    private Texture2D imageAddFullObjects;
    private PsdFile psdAddFullObjects;
    private string fileNameAddFullObjects;


    private float pixelsToUnitSize = 100.0f;
    private int atlassize = 4096;

    private int screenWidth = 1440;
    private int screenHeight = 1080;
    private bool scr2048 = true;
    private bool scr1440 = false;

    private Transform selectedTransform;

    private List<Image> imgsToShow = new List<Image>();
    private List<Image> imgsToHide = new List<Image>();
    private List<MonoBehaviour> comicsAllStatesList = new List<MonoBehaviour>();
    Dictionary<string, Image> prevStep = new Dictionary<string, Image>();

    GameObject topParentNodeGO;

    [MenuItem("Sprites/PSD Parse")]
    public static void ShowWindow()
    {
        var wnd = GetWindow<PSDParserWindow>();
        wnd.title = "PSD Parse";
        wnd.Show();
    }

    public void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        image = (Texture2D)EditorGUILayout.ObjectField("PSD File", image, typeof(Texture2D), true);
        imageAddFullObjects = (Texture2D)EditorGUILayout.ObjectField("PSD File (full objects)", imageAddFullObjects, typeof(Texture2D), true);
        bool changed = EditorGUI.EndChangeCheck();

        if (image != null)
        {
            if (changed)
            {
                string path = AssetDatabase.GetAssetPath(image);

                if (path.ToUpper().EndsWith(".PSD"))
                {
                    psd = new PsdFile(path, Encoding.Default);
                    fileName = Path.GetFileNameWithoutExtension(path);
                }
                else
                {
                    psd = null;
                }
            }
            if (psd != null)
            {
                screenWidth = EditorGUILayout.IntField("Screen Width", screenWidth);
                screenHeight = EditorGUILayout.IntField("Screen Height", screenHeight);
                if (GUILayout.Button("Parse file for HOG"))
                {
                    //ParsePSD(psd, image);
                }
                else if (GUILayout.Button("Parse file to Comics"))
                {
                    ParsePSDToComics(psd, image);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("This texture is not a PSD file.", MessageType.Error);
            }
        }

        /*if ( GUI.Button( new Rect( 0, 50, 100, 30 ), "Set 2048*1536" ) )
		{
			screenWidth = 2048;
			screenHeight = 1536;
		}
		if ( GUI.Button( new Rect( 120, 50, 100, 30 ), "Set 1440*1080" ) )
		{
			screenWidth = 1440;
			screenHeight = 1080;
		}*/

    }

    //private int depth = 0;
    private Transform curRoot = null;

    private List<Transform> rootStack = new List<Transform>();

    private void ParsePSDToComics(PsdFile psd, Texture2D image)
    {
        if (psd == null) return;

        List<LayersData> layersData = new List<LayersData>();
        LayersData background = new LayersData();
        List<GameObject> itemObjects = new List<GameObject>();

        rootStack = new List<Transform>();
        Transform curRoot = null;
        if (Selection.activeGameObject != null)
        {
            curRoot = Selection.activeGameObject.transform;
            topParentNodeGO = curRoot.gameObject;
        }

        rootStack.Add(curRoot);
        string groupName = "";

        string assetPath = AssetDatabase.GetAssetPath(image);
        string path = Path.Combine(Path.GetDirectoryName(assetPath), Path.GetFileNameWithoutExtension(assetPath) + "_atlas" + ".png");
        string pathBackground = Path.Combine(Path.GetDirectoryName(assetPath), Path.GetFileNameWithoutExtension(assetPath) + "_background" + ".png");
        //string pathBackground = "Assets/Resources/Textures/" + "_background" + ".png";


        //bool hideCurrLayer = false;

        foreach (Layer layer in psd.Layers)
        {
            bool startOrEndGroup = layer.Name == "</Layer set>" || layer.Name == "</Layer group>";
            bool groupControlBit = layer.Flags[16] && layer.Flags[24];
            if (!groupControlBit && !layer.Visible) continue;

            if (groupControlBit && !startOrEndGroup)
            {
                groupName = layer.Name;

                //    if (!layer.Visible)
                //    {
                //        hideCurrLayer = true;
                //    }
                //}

                //if (startOrEndGroup)
                //{
                //    hideCurrLayer = false;
                //}

                //if (hideCurrLayer)
                //{
                //continue;
            }

            //Debug.Log( "Layer name - " + layer.Name + ", flags - " + layer.Flags.ToString() + ", Rect - " + layer.Rect + ", visible - " + layer.Visible );

            bool flagOpenGroup = groupControlBit && groupName == "";
            bool flagCloseGroup = groupControlBit && groupName != "";


            if (flagOpenGroup)
            {
                curRoot = CreateRoot(curRoot);
                rootStack.Add(curRoot);
            }

            else if (flagCloseGroup)
            {
                if (curRoot != null)
                {
                    curRoot.name = groupName;
                    if (curRoot.name.StartsWith("Step#"))
                    {
                        curRoot.tag = TAG_COMICS_ITEM;

                        /*HOGItem newItem = curRoot.gameObject.AddComponent<HOGItem>();
						if ( newItem != null )
						{
							string title = curRoot.name.Substring( "item ".Length );
							string title1 = title[ 0 ].ToString().ToUpper() + title.Substring( 1 );
							newItem.Title = title1;
							newItem.Id = ResourceHelper.GenerateId( counter, title );
							counter++;
						}*/
                    }
                    else if (curRoot.name.StartsWith("object"))
                    {
                        //curRoot.gameObject.tag = "COMICS_object";
                        /*curRoot.tag = Constants.TAG_HOG_OBJECT;

						HOGObjectController hogObj = curRoot.gameObject.AddComponent<HOGObjectController>();
						if ( hogObj != null )
						{
							string title = curRoot.name.Substring( "object ".Length );
							hogObj.Id = ResourceHelper.GenerateId( counter, title );
							counter++;
						}*/
                    }
                }

                groupName = "";

                rootStack.Remove(curRoot);
                if (rootStack.Count > 0) { curRoot = rootStack[rootStack.Count - 1]; }
                else { curRoot = null; }
            }

            else if (layer.Name.ToLower() == "background" || layer.Name.ToLower() == "основа")
            {
                Texture2D tex = CreateTexture(layer);
                byte[] imgFileData = tex.EncodeToPNG();
                File.WriteAllBytes(pathBackground, imgFileData);

                AssetDatabase.Refresh();
                AssetDatabase.ImportAsset(pathBackground);
                TextureImporter importer = AssetImporter.GetAtPath(pathBackground) as TextureImporter;
                importer.textureType = TextureImporterType.Sprite;
                AssetDatabase.WriteImportSettingsIfDirty(pathBackground);

                Transform imgTr = CreateRoot(curRoot);
                imgTr.name = layer.Name;

                background.layer = layer;
                background.layerTranform = imgTr;
                background.texture = tex;
            }

            else
            {
                Transform imgTr = CreateRoot(curRoot);
                imgTr.name = layer.Name;

                LayersData newLayerData = new LayersData();
                newLayerData.layer = layer;
                newLayerData.layerTranform = imgTr;
                newLayerData.texture = CreateTexture(layer);
                layersData.Add(newLayerData);
            }
        }

        // 2. pack textures into atlas
        List<Texture2D> textures = ResourceHelper.LayersDataToTexturesList(layersData);
        List<SpriteMetaData> Sprites = ResourceHelper.SaveTextureAtlasToFile(layersData, textures, atlassize, path);
        AssetDatabase.Refresh();

        Texture2D texAtlas = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        // Make sure the size is the same as our atlas then create the spritesheet
        textureImporter.maxTextureSize = atlassize;
        textureImporter.spritesheet = Sprites.ToArray();
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spriteImportMode = SpriteImportMode.Multiple;
        textureImporter.spritePivot = new Vector2(0.5f, 0.5f);
        textureImporter.spritePixelsPerUnit = pixelsToUnitSize;
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();

        Vector2 pos;
        Rect rect = new Rect(0, 0, background.texture.width, background.texture.height);
        Sprite spriteBack = Sprite.Create(background.texture, rect, Vector2.zero);

        Image imgBg = AddImage(background, spriteBack, out pos);
        Sprite currBgSprite = Resources.Load<Sprite>("Textures/" + Path.GetFileNameWithoutExtension(pathBackground));
        imgBg.sprite = currBgSprite;

        for (int j = 0; j < textureImporter.spritesheet.Length; j++)
        {
            Image img = AddImage(layersData[j], sprites[j], out pos);
        }

        GameObject[] goSteps = GameObject.FindGameObjectsWithTag(TAG_COMICS_ITEM);
        List<GameObject> _goSteps = goSteps.ToList();

        _goSteps.Sort(delegate (GameObject go1, GameObject go2)
            {
                return go1.name.CompareTo(go2.name);
            });

        bool isFirstImg = true;

        foreach (GameObject currStepGO in _goSteps)
        {
            Image img = currStepGO.GetComponentInChildren<Image>();
            MonoBehaviour currComicsState = null;

            if (currStepGO.name.Contains("_img"))
            {
                if (isFirstImg)
                {
                    currComicsState = currStepGO.AddComponent<ComicsState>();
                    isFirstImg = false;

                    prevStep.Add("_img", img);
                }
                else
                {
                    currComicsState = currStepGO.AddComponent<ComicsState2>();
                    imgsToShow.Add(img);

                    AddToHidePrevStep();

                    (currComicsState as ComicsState2).SetValues(imgsToShow, imgsToHide);
                    imgsToHide.Clear();
                    imgsToShow.Clear();

                    prevStep.Clear();
                    prevStep.Add("_img", img);
                }
            }

            else if (currStepGO.name.Contains("_dream"))
            {
                currComicsState = currStepGO.AddComponent<ComicsState2>();
                imgsToShow.Add(img);
                (currComicsState as ComicsState2).SetValues(imgsToShow, imgsToHide);
                imgsToShow.Clear();
                imgsToHide.Clear();

                prevStep.Clear();
                prevStep.Add("_dream", img);
            }

            else if (currStepGO.name.Contains("_location"))
            {
                currComicsState = currStepGO.AddComponent<ComicsState>();
                imgsToHide.Clear();
                imgsToShow.Clear();

                prevStep.Clear();
                prevStep.Add("_location", img);
                isFirstImg = false;
            }

            else if (currStepGO.name.Contains("_phrase"))
            {
                currComicsState = currStepGO.AddComponent<ComicsState2>();
                imgsToShow.Add(img);

                AddToHidePrevStep();

                (currComicsState as ComicsState2).SetValues(imgsToShow, imgsToHide, 0, 1, 1);
                imgsToHide.Clear();
                imgsToShow.Clear();

                prevStep.Clear();
                prevStep.Add("_phrase", img);
            }

            else if (currStepGO.name.Contains("_blink"))
            {
                MonoBehaviour currComicsState_blink = currStepGO.AddComponent<ComicsState>();
                (currComicsState_blink as ComicsState).SetValues(0, 1, 0);
                comicsAllStatesList.Add(currComicsState_blink);

                imgsToHide.Add(img);

                int j = 1;

                for (int i = 1; i < 4; i++)
                {
                    GameObject addCurrStepGO = CreateRoot(topParentNodeGO.transform).gameObject;
                    addCurrStepGO.name = currStepGO.name + "#0" + ++j;
                    MonoBehaviour _currComicsState = addCurrStepGO.AddComponent<ComicsState2>();
                    (_currComicsState as ComicsState2).SetValues(imgsToShow, imgsToHide, 0, 1, 0);

                    comicsAllStatesList.Add(_currComicsState);

                    if (i % 2 != 0)
                    {
                        imgsToHide.Clear();
                        imgsToShow.Add(img);
                    }
                    else
                    {
                        imgsToShow.Clear();
                        imgsToHide.Add(img);
                    }
                }

                currStepGO.name += "#01";

                imgsToShow.Clear();
                imgsToHide.Clear();

                prevStep.Clear();
                prevStep.Add("_blink", img);

                continue;
            }
            comicsAllStatesList.Add(currComicsState);
        }

        SetupTopParentNode(topParentNodeGO, comicsAllStatesList);
    }

    private void AddToHidePrevStep()
    {
        if (prevStep.ContainsKey("_dream"))
        {
            imgsToHide.Add(prevStep["_dream"]);
        }
        else if (prevStep.ContainsKey("_phrase"))
        {
            imgsToHide.Add(prevStep["_phrase"]);
        }
        else if (prevStep.ContainsKey("_location"))
        {
            imgsToHide.Add(prevStep["_location"]);
        }
    }

    private void SetupTopParentNode(GameObject _topParentNodeGO, List<MonoBehaviour> _comicsAllStatesList)
    {
        _topParentNodeGO.name = "ComicsPages";
        RectTransform currRect = _topParentNodeGO.GetComponent<RectTransform>() as RectTransform;
        currRect.anchorMin = new Vector2(0.5f, 0);
        currRect.anchorMax = new Vector2(0.5f, 1);
        currRect.position = new Vector2(0.5f, 0.5f);
        currRect.sizeDelta = new Vector2(1440, 0);

        ComicsController comicsController = _topParentNodeGO.AddComponent<ComicsController>();
        comicsController.SetValues(false, true, false);

        foreach (MonoBehaviour currState in _comicsAllStatesList)
        {
            comicsController.AddState(currState, true);
        }
    }

    //private void ParsePSD(PsdFile psd, Texture2D image)
    //{
    //    if (psd == null) return;

    //    List<LayersData> layersData = new List<LayersData>();
    //    LayersData background = new LayersData();
    //    List<GameObject> itemObjects = new List<GameObject>();
    //    int counter = 1;

    //    rootStack = new List<Transform>();
    //    Transform curRoot = null;
    //    if (Selection.activeGameObject != null) { curRoot = Selection.activeGameObject.transform; }
    //    rootStack.Add(curRoot);
    //    string groupName = "";

    //    string assetPath = AssetDatabase.GetAssetPath(image);
    //    string path = Path.Combine(Path.GetDirectoryName(assetPath), Path.GetFileNameWithoutExtension(assetPath) + "_atlas" + ".png");
    //    string pathBackground = Path.Combine(Path.GetDirectoryName(assetPath), Path.GetFileNameWithoutExtension(assetPath) + "_background" + ".png");


    //    foreach (Layer layer in psd.Layers)
    //    {
    //        bool startOrEndGroup = layer.Name == "</Layer set>" || layer.Name == "</Layer group>";
    //        bool groupControlBit = layer.Flags[16] && layer.Flags[24];
    //        if (!groupControlBit && !layer.Visible) continue;

    //        if (groupControlBit && !startOrEndGroup) { groupName = layer.Name; }

    //        //Debug.Log( "Layer name - " + layer.Name + ", flags - " + layer.Flags.ToString() + ", Rect - " + layer.Rect + ", visible - " + layer.Visible );

    //        bool flagOpenGroup = groupControlBit && groupName == "";
    //        bool flagCloseGroup = groupControlBit && groupName != "";


    //        if (flagOpenGroup)
    //        {
    //            curRoot = CreateRoot(curRoot);
    //            rootStack.Add(curRoot);
    //        }
    //        else if (flagCloseGroup)
    //        {
    //            if (curRoot != null)
    //            {
    //                curRoot.name = groupName;
    //                if (curRoot.name.StartsWith("item "))
    //                {
    //                    curRoot.tag = TAG_HOG_ITEM;

    //                    /*HOGItem newItem = curRoot.gameObject.AddComponent<HOGItem>();
    //		if ( newItem != null )
    //		{
    //			string title = curRoot.name.Substring( "item ".Length );
    //			string title1 = title[ 0 ].ToString().ToUpper() + title.Substring( 1 );
    //			newItem.Title = title1;
    //			newItem.Id = ResourceHelper.GenerateId( counter, title );
    //			counter++;
    //		}*/
    //                }
    //                else if (curRoot.name.StartsWith("object "))
    //                {
    //                    /*curRoot.tag = Constants.TAG_HOG_OBJECT;

    //		HOGObjectController hogObj = curRoot.gameObject.AddComponent<HOGObjectController>();
    //		if ( hogObj != null )
    //		{
    //			string title = curRoot.name.Substring( "object ".Length );
    //			hogObj.Id = ResourceHelper.GenerateId( counter, title );
    //			counter++;
    //		}*/
    //                }
    //            }

    //            groupName = "";

    //            rootStack.Remove(curRoot);
    //            if (rootStack.Count > 0) { curRoot = rootStack[rootStack.Count - 1]; }
    //            else { curRoot = null; }
    //        }

    //        else if (layer.Name.ToLower() == "background" || layer.Name.ToLower() == "основа")
    //        {
    //            Texture2D tex = CreateTexture(layer);
    //            byte[] imgFileData = tex.EncodeToPNG();
    //            File.WriteAllBytes(pathBackground, imgFileData);

    //            Transform imgTr = CreateRoot(curRoot);
    //            imgTr.name = layer.Name;

    //            background.layer = layer;
    //            background.layerTranform = imgTr;
    //            background.texture = tex;
    //        }



    //        else
    //        {

    //            Transform imgTr = CreateRoot(curRoot);
    //            imgTr.name = layer.Name;

    //            LayersData newLayerData = new LayersData();
    //            newLayerData.layer = layer;
    //            newLayerData.layerTranform = imgTr;
    //            newLayerData.texture = CreateTexture(layer);
    //            layersData.Add(newLayerData);
    //        }
    //    }

    //    // 2. pack textures into atlas
    //    List<Texture2D> textures = ResourceHelper.LayersDataToTexturesList(layersData);
    //    List<SpriteMetaData> Sprites = ResourceHelper.SaveTextureAtlasToFile(layersData, textures, atlassize, path);
    //    AssetDatabase.Refresh();


    //    Texture2D texAtlas = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
    //    TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
    //    // Make sure the size is the same as our atlas then create the spritesheet
    //    textureImporter.maxTextureSize = atlassize;
    //    textureImporter.spritesheet = Sprites.ToArray();
    //    textureImporter.textureType = TextureImporterType.Sprite;
    //    textureImporter.spriteImportMode = SpriteImportMode.Multiple;
    //    textureImporter.spritePivot = new Vector2(0.5f, 0.5f);
    //    textureImporter.spritePixelsPerUnit = pixelsToUnitSize;
    //    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

    //    Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();

    //    Vector2 pos;
    //    //Rect rect = new Rect( 0, 0, background.texture.width, background.texture.height );
    //    Sprite spriteBack = null;//Sprite.Create( background.texture, rect, Vector2.zero );
    //    AddImage(background, spriteBack, out pos);
    //    //img1.sprite = Resources.Load<Sprite>( Path.GetFileNameWithoutExtension( pathBackground ) );



    //    for (int j = 0; j < textureImporter.spritesheet.Length; j++)
    //    {
    //        Image img = AddImage(layersData[j], sprites[j], out pos);
    //        Transform imgTr = img.transform;

    //        Transform trItemParent = imgTr.parent;

    //        if (trItemParent.tag == TAG_HOG_OBJECT)
    //        {
    //            if (img.name.StartsWith("object"))
    //            {
    //                BoxCollider boxCollider = img.gameObject.AddComponent<BoxCollider>();
    //                boxCollider.center = new Vector3(0, 0, 0);
    //                Vector2 colliderSize = new Vector3(img.sprite.rect.width, img.sprite.rect.height, 0);
    //                if (colliderSize.x < 70) { colliderSize.x = 70; }
    //                if (colliderSize.y < 70) { colliderSize.y = 70; }
    //                boxCollider.size = (Vector3)colliderSize;

    //                //Core.GUI.MouseTapEvent mouseTapEvent = img.gameObject.AddComponent<Core.GUI.MouseTapEvent>();
    //                //mouseTapEvent.Target = trItemParent.gameObject;
    //                //mouseTapEvent.Message = "OnClick";
    //            }

    //            continue;
    //        }

    //        if (trItemParent.GetComponent<BoxCollider>() == null && img.name == "object")
    //        {
    //            BoxCollider boxCollider = trItemParent.gameObject.AddComponent<BoxCollider>();
    //            boxCollider.center = new Vector3(pos.x, pos.y, 0);
    //            Vector2 colliderSize = new Vector3(img.sprite.rect.width, img.sprite.rect.height, 0);
    //            if (colliderSize.x < 70) { colliderSize.x = 70; }
    //            if (colliderSize.y < 70) { colliderSize.y = 70; }
    //            boxCollider.size = (Vector3)colliderSize;

    //            /*BaseStateMachine stateMachine = trItemParent.GetComponent<BaseStateMachine>();
    //if ( stateMachine != null )
    //{
    //	stateMachine.OnlyComponent = true;
    //	stateMachine.AddState( trItemParent.gameObject.AddComponent<Scripts.Scenes.HOG.Item.States.InitState>(), true );
    //	stateMachine.AddState( trItemParent.gameObject.AddComponent<Scripts.Scenes.HOG.Item.States.IdleState>(), true );
    //	stateMachine.AddState( trItemParent.gameObject.AddComponent<Scripts.Scenes.HOG.Item.States.Take1_HideState>(), true );
    //	stateMachine.AddState( trItemParent.gameObject.AddComponent<Scripts.Scenes.HOG.Item.States.Take2_FlyToInventoryState>(), true );
    //	stateMachine.AddState( trItemParent.gameObject.AddComponent<Scripts.Scenes.HOG.Item.States.Take3_OverAndDestroyState>(), true );
    //}
    //else
    //{
    //	Debug.LogWarning( "Cannot find StateMachine component for " + trItemParent.name );
    //}*/
    //        }
    //    }
    //}

    private Image AddImage(LayersData layer_, Sprite sprite, out Vector2 pos)
    {
        // Add the sprite to the sprite renderer
        Transform imgTr = layer_.layerTranform;
        Layer layer = layer_.layer;

        Image img = imgTr.gameObject.AddComponent<Image>();
        float alpha = layer.Opacity / 255f;
        img.color = new Color(1, 1, 1, alpha);
        img.sprite = sprite;
        img.SetNativeSize();

        pos = new Vector2(layer.Rect.x, layer.Rect.y);
        pos.x = -screenWidth / 2 + pos.x + layer.Rect.width / 2;
        pos.y = screenHeight / 2 - pos.y - layer.Rect.height / 2;

        imgTr.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x, pos.y);

        return img;
    }

    private Transform CreateRoot(Transform root)
    {
        GameObject newObj = new GameObject();
        newObj.transform.SetParent(root);
        newObj.transform.localScale = Vector3.one;
        newObj.transform.localPosition = Vector3.zero;
        newObj.AddComponent<RectTransform>();
        return newObj.transform;
    }

    private Texture2D CreateTexture(Layer layer)
    {
        if ((int)layer.Rect.width == 0 || (int)layer.Rect.height == 0)
            return null;

        Texture2D tex = new Texture2D((int)layer.Rect.width, (int)layer.Rect.height, TextureFormat.RGBA32, false);
        Color32[] pixels = new Color32[tex.width * tex.height];

        Channel red = (from l in layer.Channels where l.ID == 0 select l).First();
        Channel green = (from l in layer.Channels where l.ID == 1 select l).First();
        Channel blue = (from l in layer.Channels where l.ID == 2 select l).First();
        Channel alpha = layer.AlphaChannel;

        for (int i = 0; i < pixels.Length; i++)
        {
            byte r = red.ImageData[i];
            byte g = green.ImageData[i];
            byte b = blue.ImageData[i];
            byte a = 255;

            if (alpha != null)
                a = alpha.ImageData[i];

            int mod = i % tex.width;
            int n = ((tex.width - mod - 1) + i) - mod;
            pixels[pixels.Length - n - 1] = new Color32(r, g, b, a);
        }

        tex.SetPixels32(pixels);
        tex.Apply();
        return tex;
    }
}

