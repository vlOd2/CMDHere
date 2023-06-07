using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Runtime.InteropServices;

public class CMDHere
{
    // Static variables
    public static readonly double CMDHereVer = 1.0;
    public static readonly string CMDHereDirPath = 
        Path.GetPathRoot(Environment.SystemDirectory) + @"CMDHere";
    public static readonly string CMDHereConfigFilePath = 
        CMDHereDirPath + @"\Config.ini";
    public static readonly string CMDHerePreLaunchScriptPath = 
        CMDHereDirPath + @"\PreLaunchScript.bat";

    // Menu variables
    public uint MenuID = 0;
    public string MenuVerb = "CMDHere";

    // Class variables
    public string DirectoryPath;
    public INIFile CMDHereConfigFile = null;
    public readonly Dictionary<string, object> CMDHereConfig = new Dictionary<string, object>() 
    {
        { "ContextMenuText", "Open command window here" },
        { "WindowTitle", "Command Prompt" },
        { "UsePreLaunchScript", true },
        { "DisplayCMDWindowMessages", true }
    };

    /// <summary>
    /// Function called when a context menu is shown in a dir's background
    /// </summary>
    public void OnContextMenu()
    {
        try
        {
            // Create the data directory if it doesn't exist
            if (!Directory.Exists(CMDHereDirPath)) Directory.CreateDirectory(CMDHereDirPath);
            // Create the config file if it doesn't exist
            if (!File.Exists(CMDHereConfigFilePath)) File.Create(CMDHereConfigFilePath).Close();
            // Create the INI reader for the config file
            CMDHereConfigFile = new INIFile(CMDHereConfigFilePath);

            // Create the default pre-launch script
            if (!File.Exists(CMDHerePreLaunchScriptPath)) 
            {
                FileStream newScript = File.Create(CMDHerePreLaunchScriptPath);

                string newScriptData = 
                    "@ECHO OFF" + Environment.NewLine +
                    "VER"
                ;
                byte[] newScriptDataEncoded = Encoding.ASCII.GetBytes(newScriptData);
                newScript.Write(newScriptDataEncoded, 0, newScriptDataEncoded.Length);

                newScript.Flush();
                newScript.Close();
            }

            // Read the config file
            foreach (string cfgKey in CMDHereConfig.Keys.ToArray())
            {
                string cfgValue = CMDHereConfigFile.Read(cfgKey, "CMDHere");

                // Fill the config file with the default value
                if (!string.IsNullOrEmpty(cfgValue))
                    CMDHereConfig[cfgKey] = cfgValue;
                else
                    CMDHereConfigFile.Write(cfgKey, CMDHereConfig[cfgKey].ToString(), "CMDHere");
            }
        }
        catch (Exception ex) 
        {
            DisplayException(ex);
        }
    }

    /// <summary>
    /// Function called when the menu entry has been clicked
    /// </summary>
    public void OnMenuInteract() 
    {
        StartCMD(DirectoryPath);
    }

    /// <summary>
    /// Starts an instance of CMD
    /// </summary>
    public void StartCMD(string workDir) 
    {
        try
        {
            string systemDir = Environment.SystemDirectory;
            string cmdExePath = systemDir + @"\cmd.exe";

            string cmdArgs = "/K CLS";
            cmdArgs += @" && TITLE " + CMDHereConfig["WindowTitle"];

            bool displayCMDWindowMessages;
            bool usePreLaunchScript;
            bool.TryParse(CMDHereConfig["DisplayCMDWindowMessages"].ToString(), out displayCMDWindowMessages);
            bool.TryParse(CMDHereConfig["UsePreLaunchScript"].ToString(), out usePreLaunchScript);

            if (displayCMDWindowMessages)
            {
                cmdArgs += @" && ECHO [INFO] CMDHere - v" + CMDHereVer.ToString(".0", CultureInfo.InvariantCulture);
                cmdArgs += @" && ECHO [INFO] Written by vlOd";

                if (usePreLaunchScript)
                {
                    cmdArgs += @" && ECHO [INFO] Executing pre-launch script...";
                    cmdArgs += @" && ECHO [INFO] Script location: """ + CMDHerePreLaunchScriptPath + @"""";
                    cmdArgs += @" && """ + CMDHerePreLaunchScriptPath + @"""";
                }
            }
            else 
                if (usePreLaunchScript)
                    cmdArgs += @" && """ + CMDHerePreLaunchScriptPath + @"""";

            Process.Start(new ProcessStartInfo()
            {
                FileName = cmdExePath,
                Arguments = cmdArgs,
                WorkingDirectory = workDir
            });
        }
        catch (Exception ex)
        {
            DisplayException(ex);
        }
    }

    /// <summary>
    /// Displays an exception in a user friendly message
    /// </summary>
    /// <param name="ex">The exception to display</param>
    public void DisplayException(Exception ex)
    {
        Win32Wrapped.DisplayErrorMessage(
            "CMDHere has encountered a fatal error!" + Environment.NewLine +
            "If you belive this is a bug, submit an issue on the Github page" + Environment.NewLine +
            Environment.NewLine +
            "Error information:" + Environment.NewLine +
            "- Exception Type: " + ex.GetType().Name + Environment.NewLine +
            "- Exception Message: " + ex.Message + Environment.NewLine +
            "- Exception HResult: " + Marshal.GetHRForException(ex) + Environment.NewLine +
            "- Exception DUMP: " + Environment.NewLine + ex.ToString(),
            "CMDHere - Fatal Error"
        );
    }
}