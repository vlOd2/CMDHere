using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

/// <summary>
/// An INI file reader/writer
/// </summary>
public class INIFile
{
    private string filePath;
    private Dictionary<string, Dictionary<string, string>> fileData = new Dictionary<string, Dictionary<string, string>>();

    /// <summary>
    /// An INI file reader/writer
    /// </summary>
    /// <param name="path">The absolute path to the INI file</param>
    public INIFile(string path)
    {
        filePath = path;
        string[] fileDataAsStr = File.ReadAllLines(filePath, Encoding.ASCII);

        string currentSectionName = null;
        Dictionary<string, string> currentSectionData = null;
        foreach (string dataLineRaw in fileDataAsStr) 
        {
            // Section
            if (dataLineRaw.Contains("[") &&
                !dataLineRaw.Contains("=") &&
                dataLineRaw.Contains("]"))
            {
                string sectionName = dataLineRaw.Replace("[", string.Empty)
                    .Replace("]", string.Empty);

                // Write the current section
                if (currentSectionName != null && 
                    currentSectionData != null) 
                {
                    fileData.Add(currentSectionName, currentSectionData);
                    currentSectionName = null;
                    currentSectionData = null;
                }

                currentSectionName = sectionName;
                currentSectionData = new Dictionary<string, string>();
            }
            // Data entry
            else 
            {
                if (!dataLineRaw.Contains("=")) continue;
                if (currentSectionData == null) continue;

                string[] dataEntryRaw = dataLineRaw.Split(new char[] { '=' }, 2);
                string dataEntryKey = dataEntryRaw[0];
                string dataEntryValue = dataEntryRaw[1];

                currentSectionData.Add(dataEntryKey, dataEntryValue);
            }
        }

        if (fileData.Count < 1 && 
            currentSectionName != null && 
            currentSectionData != null) 
        {
            fileData.Add(currentSectionName, currentSectionData);
            currentSectionName = null;
            currentSectionData = null;
        }
    }

    private void FlushDataToFile() 
    {
        string fileDataAsStr = "";

        foreach (KeyValuePair<string, Dictionary<string, string>> section in fileData.ToArray()) 
        {
            string sectionName = section.Key.Replace("=", string.Empty);
            Dictionary<string, string> sectionData = section.Value;

            fileDataAsStr += "[" + sectionName + "]" + Environment.NewLine;
            foreach (KeyValuePair<string, string> dataEntry in sectionData) 
            {
                fileDataAsStr += dataEntry.Key + "=" + dataEntry.Value + Environment.NewLine;
            }
        }

        File.WriteAllText(filePath, fileDataAsStr);
    }

    /// <summary>
    /// Reads the value of a key
    /// </summary>
    /// <param name="key">The key to read</param>
    /// <param name="section">The section of the key</param>
    /// <returns>The value of the key (or null)</returns>
    public string Read(string key, string section)
    {
        if (!fileData.ContainsKey(section)) return null;
        if (!fileData[section].ContainsKey(key)) return null;
        return fileData[section][key];
    }

    /// <summary>
    /// Writes the specified value to a key
    /// </summary>
    /// <param name="key">The key to write to</param>
    /// <param name="value">The value to write</param>
    /// <param name="section">The section of the key</param>
    public void Write(string key, string value, string section)
    {
        if (!fileData.ContainsKey(section)) 
            fileData[section] = new Dictionary<string, string>();
        fileData[section][key] = value;
        FlushDataToFile();
    }

    /// <summary>
    /// Deletes the specified key
    /// </summary>
    /// <param name="key">The key to delete</param>
    /// <param name="section">The section of the key</param>
    public void Delete(string key, string section)
    {
        if (!fileData.ContainsKey(section)) return;
        if (!fileData[section].ContainsKey(key)) return;
        fileData[section].Remove(key);
        FlushDataToFile();
    }

    /// <summary>
    /// Deletes the specified section
    /// </summary>
    /// <param name="section">The section to delete</param>
    public void Delete(string section)
    {
        if (!fileData.ContainsKey(section)) return;
        fileData.Remove(section);
        FlushDataToFile();
    }

    /// <summary>
    /// Checks if the specified key exists
    /// </summary>
    /// <param name="key">The key to check</param>
    /// <param name="section">The section of the key</param>
    /// <returns>True if the key exists, false if otherwise</returns>
    public bool Exists(string key, string section)
    {
        return (fileData.ContainsKey(section) ? (fileData[section].ContainsKey(key)) : false);
    }
}