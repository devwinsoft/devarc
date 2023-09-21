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
    public delegate void DownloadPatchCallback(PatchInfo info);
    public delegate void DownloadProgressCallback(float progress);
    public delegate void DownloadResultCallback();
    public delegate void DownloadErrorCallback();

    public class PatchInfo
    {
        public void Clear()
        {
            totalSize = 0;
            patchSizes.Clear();
        }

        public long totalSize;
        public Dictionary<string, long> patchSizes = new Dictionary<string, long>();
    }

    public class DownloadManager : MonoSingleton<DownloadManager>
    {
        public event DownloadPatchCallback OnPatch;
        public event DownloadProgressCallback OnProgress;
        public event DownloadResultCallback OnResult;
        public event DownloadErrorCallback OnError;

        PatchInfo mPathInfo = new PatchInfo();
        List<string> mPatchList = new List<string>();


        public void AddToPatchList(string addressableKey)
        {
            if (!mPatchList.Contains(addressableKey))
                mPatchList.Add(addressableKey);
        }


        public void BeginPatch()
        {
            StartCoroutine(beginPatch());
        }

        IEnumerator beginPatch()
        {
            if (mPatchList.Count == 0)
            {
                Debug.LogWarning($"[DownloadManager::BeginPatch] Patch list is empty.");
                OnPatch?.Invoke(mPathInfo);
                yield break;
            }

            mPathInfo.Clear();
            foreach (var key in mPatchList)
            {
                // WARNING: This will cause all asset bundles to be re-downloaded at startup every time and should not be used in a production game
                Addressables.ClearDependencyCacheAsync(key);

                AsyncOperationHandle<long> getDownloadSize = Addressables.GetDownloadSizeAsync(key);
                yield return getDownloadSize;

                switch (getDownloadSize.Status)
                {
                    case AsyncOperationStatus.Succeeded:
                        mPathInfo.totalSize += getDownloadSize.Result;
                        mPathInfo.patchSizes.Add(key, getDownloadSize.Result);
                        break;
                    default:
                        Addressables.Release(getDownloadSize);
                        OnError?.Invoke();
                        yield break;
                }
                Addressables.Release(getDownloadSize);
            }
            OnPatch?.Invoke(mPathInfo);
        }


        public void BeginDownload()
        {
            StartCoroutine(beginDownload());
        }

        IEnumerator beginDownload()
        {
            if (mPathInfo.totalSize == 0)
            {
                Debug.Log($"[DownloadManager::BeginDownload] There is no data to patch.");
                OnResult?.Invoke();
                yield break;
            }

            long downloadSize = 0;
            foreach (var temp in mPathInfo.patchSizes)
            {
                AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(temp.Key, false);
                yield return downloadHandle;

                if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    downloadSize += temp.Value;
                }
                else
                {
                    yield break;
                }
                Addressables.Release(downloadHandle);

                float progress = (float)downloadSize / (float)mPathInfo.totalSize;
                OnProgress?.Invoke(progress);
            }
            OnResult?.Invoke();
        }

    }
}

