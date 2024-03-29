using System;
using System.Collections.Concurrent;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FrameWork
{
    public static class AssetBundlesLoad
    {
        //private static ConcurrentDictionary<string, AssetBundle> _assetBundles=new ConcurrentDictionary<string, AssetBundle>();

        


        private static string abEndName = "info";
        public static T LoadAsset<T>(string packName,string name) where T : Object
        {
            string isNewPack = "";
            AssetBundle assetBundle;
            string path = Application.streamingAssetsPath+Tool.GetAbPath();
            // if (!_assetBundles.TryGetValue(packName,out assetBundle))
            // {
            //     
            //     _assetBundles.TryAdd(packName, assetBundle);
            // }
            
            // FileInfo fileInfo = new FileInfo(Application.persistentDataPath+Tool.GetAbPath()+packName+"."+abEndName);
            // MyLog.Log(Application.persistentDataPath+"/"+packName);
            // if (fileInfo.Exists)
            // {
            //     isNewPack = "新";
            //     assetBundle=AssetBundle.LoadFromFile(Application.persistentDataPath+Tool.GetAbPath()+packName+"."+abEndName);
            // }
            // else
            // {
            //     isNewPack = "旧";
            //     assetBundle=AssetBundle.LoadFromFile(path+"/"+packName+"."+abEndName);
            //     //assetBundle=AssetBundle.LoadFromFile(Application.streamingAssetsPath+"/"+packName+"."+GlobalVariables.Configure.AbEndName);
            // }
            assetBundle=AssetBundle.LoadFromFile(path+"/"+packName+"."+abEndName);
            MyLog.Log($"从{isNewPack}包"+packName+"加载:"+name);
            var obj=assetBundle.LoadAsset<T>(name);
            assetBundle.Unload(false);
            return obj;
        }

        
        
        
        
        // public static void SavePack(byte[] data,string packName)
        // {
        //     _assetBundles.TryAdd(packName, AssetBundle.LoadFromMemory(data));
        // }
        //
        
        public static void LoadAssetAsync<T>(string packName,string name,Action<T> action) where T : Object
        {
            AssetBundle assetBundle;
            string path = Application.streamingAssetsPath+Tool.GetAbPath();


            string isNewPack = "";
            // if (!_assetBundles.TryGetValue(packName,out assetBundle))
            // {
            //     
            //     _assetBundles.TryAdd(packName, assetBundle);
            // }
            
            // FileInfo fileInfo = new FileInfo(Application.persistentDataPath+Tool.GetAbPath()+packName+"."+abEndName);
            // MyLog.Log(Application.persistentDataPath+"/"+packName);
            // if (fileInfo.Exists)
            // {
            //     isNewPack = "新";
            //     assetBundle=AssetBundle.LoadFromFile(Application.persistentDataPath+Tool.GetAbPath()+packName+"."+abEndName);
            // }
            // else
            // {
            //     assetBundle=AssetBundle.LoadFromFile(path+packName+"."+abEndName);
            //     //MyLog.Log("从旧包"+packName+"加载:"+name);
            //     isNewPack = "旧";
            // }
            
            assetBundle=AssetBundle.LoadFromFile(path+packName+"."+abEndName);
            MyLog.Log($"从{isNewPack}包"+packName+"加载:"+name);
            var asset=assetBundle.LoadAssetAsync<T>(name);
            asset.completed += (operation =>
            {
                action((T)asset.asset);
                assetBundle.Unload(false);
            } );

        }
    }
}