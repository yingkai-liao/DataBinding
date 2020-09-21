using System.Threading.Tasks;

public interface IAssetLoader
{
    Task<T> LoadAsync<T>(string assetPath) where T : UnityEngine.Object;
}
