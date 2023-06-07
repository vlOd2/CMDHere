using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Diagnostics;
using System.IO;

[ComVisible(true)]
[Guid("56576e75-d0ba-43f5-a301-313054ab6629")]
public class CMDHereShellExtension : IShellExtInit, IContextMenu
{
    // Variables
    private CMDHere cmdHere;

    /// <summary>
    /// The shell extension for CMDHere
    /// </summary>
    public CMDHereShellExtension() 
    {
        cmdHere = new CMDHere();
    }

    /// <summary>
    /// Initializes the shell extension
    /// </summary>
    public void Initialize(IntPtr pidlFolder, IntPtr pDataObj, IntPtr hKeyProgID)
    {
        try
        {
            StringBuilder dirPath = new StringBuilder(256);
            Win32.SHGetPathFromIDList(pidlFolder, dirPath);
            cmdHere.DirectoryPath = dirPath.ToString();
            cmdHere.OnContextMenu();
        }
        catch (Exception ex) 
        {
            cmdHere.DisplayException(ex);
        }
    }

    /// <summary>
    /// Creates the context menu entries
    /// </summary>
    public int QueryContextMenu(IntPtr hMenu, uint iMenu, 
        uint idCmdFirst, uint idCmdLast, uint uFlags)
    {
        try 
        {
            MENUITEMINFO menu = new MENUITEMINFO();
            menu.cbSize = (uint)Marshal.SizeOf(menu);

            menu.fMask = MIIM.MIIM_STRING | MIIM.MIIM_FTYPE | MIIM.MIIM_ID | MIIM.MIIM_STATE;
            menu.fType = MFT.MFT_STRING;
            menu.fState = MFS.MFS_ENABLED;

            menu.wID = idCmdFirst + cmdHere.MenuID;
            menu.dwTypeData = cmdHere.CMDHereConfig["ContextMenuText"].ToString();

            Win32.InsertMenuItem(hMenu, iMenu, true, ref menu);
            return Win32Error.MAKE_HRESULT(Win32Error.SEVERITY_SUCCESS, 0, cmdHere.MenuID + 1);
        }
        catch (Exception ex)
        {
            cmdHere.DisplayException(ex);
            return Win32Error.MAKE_HRESULT(Win32Error.SEVERITY_ERROR, 0, 0);
        }
    }

    /// <summary>
    /// Invokes a context menu entry
    /// </summary>
    public void InvokeCommand(IntPtr pici)
    {
        try 
        {
            bool useEX = false;
            CMINVOKECOMMANDINFO ivkCMDInfo = (CMINVOKECOMMANDINFO)
                Marshal.PtrToStructure(pici, typeof(CMINVOKECOMMANDINFO));
            CMINVOKECOMMANDINFOEX ivkCMDInfoEx = new CMINVOKECOMMANDINFOEX();

            if ((ivkCMDInfo.cbSize == Marshal.SizeOf(typeof(CMINVOKECOMMANDINFOEX))) &&
                ((ivkCMDInfo.fMask & CMIC.CMIC_MASK_UNICODE) != 0))
            {
                useEX = true;
                ivkCMDInfoEx = (CMINVOKECOMMANDINFOEX)
                    Marshal.PtrToStructure(pici, typeof(CMINVOKECOMMANDINFOEX));
            }

            if (!useEX && Win32Wrapped.HighWord(ivkCMDInfo.verb.ToInt32()) != 0)
            {
                // We identify by verb and we dont use unicode
                if (Marshal.PtrToStringAnsi(ivkCMDInfo.verb) == cmdHere.MenuVerb)
                {
                    cmdHere.OnMenuInteract();
                    return;
                }
            }
            else if (useEX && Win32Wrapped.HighWord(ivkCMDInfoEx.verbW.ToInt32()) != 0)
            {
                // We identify by verb and we use unicode
                if (Marshal.PtrToStringUni(ivkCMDInfoEx.verbW) == cmdHere.MenuVerb)
                {
                    cmdHere.OnMenuInteract();
                    return;
                }
            }
            else
            {
                // We don't identify by verb
                if (Win32Wrapped.LowWord(ivkCMDInfo.verb.ToInt32()) == cmdHere.MenuID)
                {
                    cmdHere.OnMenuInteract();
                    return;
                }
            }

            Marshal.ThrowExceptionForHR(Win32Error.E_FAIL);
        }
        catch (Exception ex)
        {
            cmdHere.DisplayException(ex);
            Marshal.ThrowExceptionForHR(Win32Error.E_FAIL);
        }
    }
    
    /// <summary>
    /// Gets a string from a context menu entry
    /// </summary>
    public void GetCommandString(UIntPtr idCmd, uint uFlags,
        IntPtr pReserved, StringBuilder pszName, uint cchMax) { }
}