using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Devarc
{
    public class DownloadManager : SingletonManager<DownloadManager>
    {
        protected override void onAwake()
        {
        }

        protected override void onDestroy()
        {
        }


        public IEnumerator GetPatchList(string[] keys, System.Action<long, Dictionary<string, long>> resultCallback)
        {
            long patchSize = 0;
            Dictionary<string, long> patchList = new Dictionary<string, long>();

            foreach (var key in keys)
            {
                // WARNING: This will cause all asset bundles to be re-downloaded at startup every time and should not be used in a production game
                //Addressables.ClearDependencyCacheAsync(key);

                AsyncOperationHandle<long> getDownloadSize = Addressables.GetDownloadSizeAsync(key);
                yield return getDownloadSize;

                switch (getDownloadSize.Status)
                {
                    case AsyncOperationStatus.Succeeded:
                        patchSize += getDownloadSize.Result;
                        patchList.Add(key, getDownloadSize.Result);
                        break;
                    default:
                        resultCallback?.Invoke(0, null);
                        yield break;
                }
            }
            resultCallback?.Invoke(patchSize, patchList);
        }


        public void Download(Dictionary<string, long> patchList, System.Action<string> progressCallback, System.Action<bool> resultCallback)
        {
            StartCoroutine(download(patchList, progressCallback, resultCallback));
        }

        IEnumerator download(Dictionary<string, long> patchList, System.Action<string> progressCallback, System.Action<bool> resultCallback)
        {
            foreach (var temp in patchList)
            {
                AsyncOperationHandle downloadDependencies = Addressables.DownloadDependenciesAsync(temp.Key);
                yield return downloadDependencies;
                switch (downloadDependencies.Status)
                {
                    case AsyncOperationStatus.Succeeded:
                        progressCallback?.Invoke(temp.Key);
                        break;
                    default:
                        resultCallback?.Invoke(false);
                        yield break;
                }
            }
            resultCallback?.Invoke(true);
        }
    }
}

