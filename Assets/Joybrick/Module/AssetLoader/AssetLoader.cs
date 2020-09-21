using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ResourcesAssetLoader : IAssetLoader
{
    public ResourcesAssetLoader()
    {
    }

    public async Task<T> LoadAsync<T>(string assetPath) where T : UnityEngine.Object
    {
        var result = await Resources.LoadAsync<T>(assetPath);
        return result as T;
    }
}
