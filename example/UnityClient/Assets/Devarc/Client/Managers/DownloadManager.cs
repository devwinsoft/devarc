using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;


namespace Devarc
{
    public class DownloadManager : MonoSingleton<DownloadManager>
    {
        // Remote addressable list.
        public CString[] addressList;


        // Return value = Dictionary(addressable, size)
        public IEnumerator GetPatchList(System.Action<long, Dictionary<string, long>> resultCallback)
        {
            long patchSize = 0;
            Dictionary<string, long> patchList = new Dictionary<string, long>();

            foreach (var key in addressList)
            {
                // WARNING: This will cause all asset bundles to be re-downloaded at startup every time and should not be used in a production game
                Addressables.ClearDependencyCacheAsync(key);

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
                Addressables.Release(getDownloadSize);
            }
            resultCallback?.Invoke(patchSize, patchList);
        }



        public IEnumerator Download(Dictionary<string, long> patchList, System.Action<AsyncOperationStatus, float> progressCallback)
        {
            long totalSize = 0;
            foreach (var temp in patchList)
            {
                totalSize += temp.Value;
            }

            long downloadSize = 0;
            foreach (var temp in patchList)
            {
                AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(temp.Key, false);
                yield return downloadHandle;
                if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    downloadSize += temp.Value;
                    progressCallback?.Invoke(AsyncOperationStatus.None, (float)downloadSize / (float)totalSize);
                    Addressables.Release(downloadHandle);
                }
                else
                {
                    progressCallback?.Invoke(AsyncOperationStatus.Failed, (float)downloadSize / (float)totalSize);
                    Addressables.Release(downloadHandle);
                    yield break;
                }
            }
            progressCallback?.Invoke(AsyncOperationStatus.Succeeded, 1f);
        }
    }
}

