using System.Collections;
using System.IO;
using FrameWork.Coroutine;
using FrameWork.Global;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace FrameWork.Editor
{
    public class AssetBundle: UnityEditor.Editor
    {
        
        [MenuItem("FrameWork/AB/CreatAssetBundle for Android")]
        public static void CreatAssetBundleAsAndroid()
        {
            if (!Directory.Exists("AssetBundles/Android"))
            {
                Directory.CreateDirectory("AssetBundles/Android");
            }
            BuildPipeline.BuildAssetBundles("AssetBundles/Android", BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.Android);
            AssetDatabase.Refresh();
            UnityEngine.Debug.Log("Android Finish!");
        }

        [MenuItem("FrameWork/AB/CreatAssetBundle for IOS")]
        public static void BuildAllAssetBundlesAsIOS()
        {
           
            if (!Directory.Exists("AssetBundles/Ios"))
            {
                Directory.CreateDirectory("AssetBundles/Ios");
            }
            BuildPipeline.BuildAssetBundles("AssetBundles/Ios", BuildAssetBundleOptions.CollectDependencies, BuildTarget.iOS);
            AssetDatabase.Refresh();
            UnityEngine.Debug.Log("IOS Finish!");

        }


        [MenuItem("FrameWork/AB/CreatAssetBundle for Win")]
        public static void CreatPCAssetBundleAsWindows()
        {
            
            if (!Directory.Exists("AssetBundles/StandaloneWindows"))
            {
                Directory.CreateDirectory("AssetBundles/StandaloneWindows");
            }
            BuildPipeline.BuildAssetBundles("AssetBundles/StandaloneWindows", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
            AssetDatabase.Refresh();
            UnityEngine.Debug.Log("Windows Finish!");
        }


        [MenuItem("Assets/FrameWork/SetAB/Material")]
        public static void SetMaterialAb()
        {
            AssetImporter ai=AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(Selection.activeObject));
            ai.assetBundleName = "material";
            ai.assetBundleVariant = "info";
        }
        
        [MenuItem("Assets/FrameWork/SetAB/UiPrefab")]
        public static void SetUiPrefabAb()
        {
            AssetImporter ai=AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(Selection.activeObject));
            ai.assetBundleName = "uiprefab";
            ai.assetBundleVariant = "info";
        }
        
        [MenuItem("Assets/FrameWork/SetAB/Mode")]
        public static void SetModePrefabAb()
        {
            AssetImporter ai=AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(Selection.activeObject));
            ai.assetBundleName = "mode";
            ai.assetBundleVariant = "info";
        }
        
        [MenuItem("Assets/FrameWork/SetAB/Screen")]
        public static void SetScreen()
        {
            AssetImporter ai=AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(Selection.activeObject));
            ai.assetBundleName = "Screen";
            ai.assetBundleVariant = "info";
        }


        
        
    }
}