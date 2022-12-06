using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

namespace zm
{
    public class AssetbundleLoadManager : SingleBase<AssetbundleLoadManager>
    {
        private Dictionary<string, AssetBundle> CurrentloadAssetBundle = new Dictionary<string, AssetBundle>();

        private Dictionary<AssetBundle, List<AssetBundle>> CurrentLoadAssetBundleDependencies = new Dictionary<AssetBundle, List<AssetBundle>>();

        private AssetBundleManifest assetBundleManifest;

        public bool IsInit { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="AssetsbundleBuildPath">assetsbundel的build地址里面包含AssetBundleManifest的所有关联信息</param>
        public void Init(string AssetsbundleBuildPath)
        {
            if (!IsInit)
            {
                base.Init();
                StartCoroutine(LoadAssetsbundlMinifest(AssetsbundleBuildPath));
                IsInit = true;
            }
        }

        public void GetAssetBundle<T>( string assetbundlePath, string package_name, string assets_name, UnityAction<T> unityAction) where T : Object
        {
            if (assetBundleManifest != null)
            {
                foreach (var abName in assetBundleManifest.GetAllAssetBundles())
                {
                    if (abName == package_name)
                    {
                        if (!CurrentloadAssetBundle.ContainsKey(package_name))
                            StartCoroutine(LoadAssetsbundlelocal(assetbundlePath,package_name, (assetbundle) =>
                            {
                                var t = assetbundle.LoadAsset<T>(assets_name);
                                unityAction.Invoke(t);
                            }));
                        else
                        {
                            var t = CurrentloadAssetBundle[package_name].LoadAsset<T>(assets_name);
                            unityAction.Invoke(t);
                        }
                        return;
                    }
                }
                Debug.Log(string.Format("没有找到名为{0}的ab包", package_name));
            }
            else
            {
                StartCoroutine(WaitLoadMinifest(() => GetAssetBundle<T>(assetbundlePath, package_name, assets_name, unityAction)));
            }
        }

        public void GetAllAssetBundle<T>(string assetbundlePath, string package_name, UnityAction<T[]> unityAction) where T : Object
        {
            if (assetBundleManifest != null)
            {
                foreach (var abName in assetBundleManifest.GetAllAssetBundles())
                {
                    if (abName == package_name)
                    {
                        if (!CurrentloadAssetBundle.ContainsKey(package_name))
                            StartCoroutine(LoadAssetsbundlelocal(assetbundlePath, package_name, (assetbundle) =>
                             {
                                 var t = assetbundle.LoadAllAssets<T>();
                                 unityAction.Invoke(t);
                             }));
                        else
                        {
                            var t = CurrentloadAssetBundle[package_name].LoadAllAssets<T>();
                            unityAction.Invoke(t);
                        }
                        return;
                    }
                }
                Debug.Log(string.Format("没有找到名为{0}的ab包", package_name));
            }
            else
            {
                StartCoroutine(WaitLoadMinifest(() => GetAllAssetBundle<T>(assetbundlePath, package_name, unityAction)));
            }
        }

        /// <summary>
        /// 卸载assetbundle 资源包
        /// </summary>
        /// </param name="is_unloadDependencies">是否卸载依赖的资源包</param>
        /// <param name="package_name">是否卸载当前加载对象的实例</param>
        public void UnloadAssetBundle(string package_name, bool is_unloadDependencies, bool unloadAllLoadedObjects = false)
        {
            if (CurrentloadAssetBundle.ContainsKey(package_name))
            {
                var assetbundel = CurrentloadAssetBundle[package_name];
                if (is_unloadDependencies)
                {
                    if (CurrentLoadAssetBundleDependencies.ContainsKey(assetbundel))
                    {
                        foreach (var ab in CurrentLoadAssetBundleDependencies[assetbundel])
                        {
                            ab.Unload(unloadAllLoadedObjects);
                        }
                        CurrentLoadAssetBundleDependencies.Remove(assetbundel);
                    }
                }
                CurrentloadAssetBundle[package_name].Unload(unloadAllLoadedObjects);
                CurrentloadAssetBundle.Remove(package_name);
            }
        }

        IEnumerator LoadAssetsbundlMinifest(string AssetsbundleBuildPath)
        {
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(AssetsbundleBuildPath + "/Assetsbundle");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                var ab = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
                assetBundleManifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                request.Dispose();
            }
            else
                Debug.Log(request.error);
        }

        IEnumerator LoadAssetsbundlelocal(string assetbundlePath, string package_name, UnityAction<AssetBundle> unityAction = null)
        {
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(assetbundlePath + package_name);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                var assetbundle = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
                var dependencies = assetBundleManifest.GetAllDependencies(package_name);
                foreach (var dependencie_ab in dependencies)
                {
                    StartCoroutine(LoadAssetbundleDependencies(assetbundlePath, assetbundle, dependencie_ab));
                }
                if (!CurrentloadAssetBundle.ContainsKey(package_name))
                    CurrentloadAssetBundle.Add(package_name, assetbundle);
                if (unityAction != null) unityAction.Invoke(assetbundle);
            }
            else
                Debug.Log("" + request.error);
        }

        /// <summary>
        /// 加载asset bundle的依赖项
        /// </summary>
        /// <param name="assetBundle"> </param>
        /// <param name="dependencie_name">依赖包名</param>
        /// <returns></returns>
        IEnumerator LoadAssetbundleDependencies(string assetbundlePath, AssetBundle assetBundle, string dependencie_name)
        {
            if (!CurrentLoadAssetBundleDependencies.ContainsKey(assetBundle)) CurrentLoadAssetBundleDependencies.Add(assetBundle, new List<AssetBundle>());
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(assetbundlePath + dependencie_name);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                var asset = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
                CurrentLoadAssetBundleDependencies[assetBundle].Add(asset);
                Debug.Log(assetBundle.name + "_d_" + asset.name);
            }
        }

        IEnumerator WaitLoadMinifest(UnityAction unityAction)
        {
            while (assetBundleManifest == null)
            {
                yield return 0;
            }
            unityAction.Invoke();
        }

    }

}