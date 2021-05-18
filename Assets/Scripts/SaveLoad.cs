using System.IO;
using UnityEngine;

public static class SaveLoadText 
{
    static string FILE_PATH
    {
        get
        {
            var path = Application.dataPath;
#if UNITY_EDITOR
            path = Path.Combine(path, "../");
#endif

            return $"{path}/save.json";
        }
    }
        
    public static void Save(string text) => File.WriteAllText(FILE_PATH, text);

    public static string Load()
    {
        var path = FILE_PATH;
        return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
    }

    public static async void SaveAsync(string text, System.Action finished = null)
    {
        using(var writer = new StreamWriter(FILE_PATH, false))
        {
            await writer.WriteAsync(text);
            finished?.Invoke();
        }
    }

    public static async void LoadAsync(System.Action<string> finished)
    {
        using (var reader = new StreamReader(FILE_PATH))
        {
            var result = await reader.ReadToEndAsync();
            finished?.Invoke(result);
        }
    }
}