using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public static class GameLogger
{
    private static string SanitizeFileName(string name)
    {
        string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
        string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

        return Regex.Replace(name, invalidRegStr, "_");
    }

    public static void WriteToCSV(string[] data, string agent1Name, string agent2Name, bool isEnemy)
    {
        try
        {
            //Debug.Log("[GameLogger] Received request to write data.");

            string safeAgent1Name = SanitizeFileName(agent1Name);
            string safeAgent2Name = SanitizeFileName(agent2Name);
            //Debug.Log($"[GameLogger] Original names: '{agent1Name}', '{agent2Name}'. Sanitized names: '{safeAgent1Name}', '{safeAgent2Name}'.");

            string filename;
            if (isEnemy)
            {
                filename = $"결과/{safeAgent1Name}_vs_Enemy.csv";
            }
            else
            {
                filename = $"결과/{safeAgent1Name}_vs_{safeAgent2Name}.csv";
            }
            //Debug.Log($"[GameLogger] Generated filename part: {filename}");

            string filePath = Path.Combine(Application.dataPath, filename);
            //Debug.Log($"[GameLogger] Attempting to write to full file path: {filePath}");

            string dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath))
            {
                //Debug.Log($"[GameLogger] Directory does not exist. Creating directory: {dirPath}");
                Directory.CreateDirectory(dirPath);
            }

            bool fileExists = File.Exists(filePath);
            //Debug.Log($"[GameLogger] File exists at path before writing? {fileExists}");
            
            lock(typeof(GameLogger))
            {
                using (StreamWriter sw = new StreamWriter(filePath, true))
                {
                    if (!fileExists && data.Length > 1)
                    {
                        //Debug.Log("[GameLogger] Writing header row.");
                        sw.WriteLine("Outcome,Episode,Agent Name,Team ID,Kill Count,Attack Count,Miss Count,Hit Count,Death Count,Episode Play Time");
                    }
                    
                    string line = string.Join(",", data);
                    //Debug.Log($"[GameLogger] Writing data line: {line}");
                    sw.WriteLine(line);
                }
            }
            //Debug.Log($"[GameLogger] Successfully wrote to {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[GameLogger] FAILED to write to CSV. An exception occurred: {e.Message}\n{e.StackTrace}");
        }
    }
}
