using System;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

[ComVisible(false)]
public enum MessageBoxType
{
    MB_ABORTRETRYIGNORE = 0x00000002,
    MB_CANCELTRYCONTINUE = 0x00000006,
    MB_HELP = 0x00004000,
    MB_OK = 0x00000000,
    MB_OKCANCEL = 0x00000001,
    MB_RETRYCANCEL = 0x00000005,
    MB_YESNO = 0x00000004,
    MB_YESNOCANCEL = 0x00000003,
    MB_ICONEXCLAMATION = 0x00000030,
    MB_ICONWARNING = 0x00000030,
    MB_ICONINFORMATION = 0x00000040,
    MB_ICONASTERISK = 0x00000040,
    MB_ICONQUESTION = 0x00000020,
    MB_ICONSTOP = 0x00000010,
    MB_ICONERROR = 0x00000010,
    MB_ICONHAND = 0x00000010,
    MB_DEFBUTTON1 = 0x00000000,
    MB_DEFBUTTON2 = 0x00000100,
    MB_DEFBUTTON3 = 0x00000200,
    MB_DEFBUTTON4 = 0x00000300,
    MB_APPLMODAL = 0x00000000,
    MB_SYSTEMMODAL = 0x00001000,
    MB_TASKMODAL = 0x00002000,
    MB_DEFAULT_DESKTOP_ONLY = 0x00020000,
    MB_RIGHT = 0x00080000,
    MB_RTLREADING = 0x00100000,
    MB_SETFOREGROUND = 0x00010000,
    MB_TOPMOST = 0x00040000,
    MB_SERVICE_NOTIFICATION = 0x00200000
}

[ComVisible(false)]
public static class Win32
{
    [DllImport("user32.dll")]
    public static extern int MessageBoxA(IntPtr hWnd, string lpText,
        string lpCaption, MessageBoxType uType);

    [DllImport("shell32.dll",CharSet = CharSet.Unicode)]
    public static extern uint DragQueryFile(IntPtr hDrop, uint iFile,
        StringBuilder pszFile, int cch);

    [DllImport("ole32.dll",CharSet = CharSet.Unicode)]
    public static extern void ReleaseStgMedium(ref STGMEDIUM pmedium);

    [DllImport("user32.dll",CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool InsertMenuItem(IntPtr hMenu, uint uItem,
        [MarshalAs(UnmanagedType.Bool)] bool fByPosition, ref MENUITEMINFO mii);

    [DllImport("gdi32.dll",CharSet = CharSet.Unicode)]
    public static extern bool DeleteObject(IntPtr hObject);

    [DllImport("shell32.dll")]
    public static extern bool SHGetPathFromIDList(IntPtr pidl, StringBuilder pszPath);
}

[ComVisible(false)]
public static class Win32Wrapped
{
    public static int MessageBox(string text, string title, MessageBoxType type)
    {
        return Win32.MessageBoxA(IntPtr.Zero, text, title, MessageBoxType.MB_TASKMODAL | type);
    }

    public static void DisplayErrorMessage(string text, string title) 
    {
        MessageBox(text, title, MessageBoxType.MB_OK | MessageBoxType.MB_ICONERROR);
    }

    public static int HighWord(int nr)
    {
        return ((nr & 0x80000000) == 0x80000000) ?
            (nr >> 16) : ((nr >> 16) & 0xffff);
    }

    public static int LowWord(int nr)
    {
        return nr & 0xffff;
    }
}

[ComVisible(false)]
public static class Win32Error
{
    public const int S_OK = 0x0000;
    public const int S_FALSE = 0x0001;
    public const int E_FAIL = -2147467259;
    public const int E_INVALIDARG = -2147024809;
    public const int E_OUTOFMEMORY = -2147024882;
    public const int STRSAFE_E_INSUFFICIENT_BUFFER = -2147024774;
    public const uint SEVERITY_SUCCESS = 0;
    public const uint SEVERITY_ERROR = 1;

    public static int MAKE_HRESULT(uint sev, uint fac, uint code)
    {
        return (int)((sev << 31) | (fac << 16) | code);
    }
}

[ComVisible(false)]
[ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("000214e8-0000-0000-c000-000000000046")]
public interface IShellExtInit
{
    void Initialize(IntPtr pidlFolder, 
        IntPtr pDataObj, IntPtr hKeyProgID);
}

[ComVisible(false)]
[ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("000214e4-0000-0000-c000-000000000046")]
public interface IContextMenu
{
    [PreserveSig]
    int QueryContextMenu(IntPtr hMenu, uint iMenu, 
        uint idCmdFirst, uint idCmdLast, uint uFlags);
    void InvokeCommand(IntPtr pici);
    void GetCommandString(UIntPtr idCmd, uint uFlags, 
        IntPtr pReserved, StringBuilder pszName, uint cchMax);
}

[ComVisible(false)]
public enum GCS : uint
{
    GCS_VERBA = 0x00000000,
    GCS_HELPTEXTA = 0x00000001,
    GCS_VALIDATEA = 0x00000002,
    GCS_VERBW = 0x00000004,
    GCS_HELPTEXTW = 0x00000005,
    GCS_VALIDATEW = 0x00000006,
    GCS_VERBICONW = 0x00000014,
    GCS_UNICODE = 0x00000004
}

[ComVisible(false)]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct CMINVOKECOMMANDINFO
{
    public uint cbSize;
    public CMIC fMask;
    public IntPtr hwnd;
    public IntPtr verb;
    [MarshalAs(UnmanagedType.LPStr)]
    public string parameters;
    [MarshalAs(UnmanagedType.LPStr)]
    public string directory;
    public int nShow;
    public uint dwHotKey;
    public IntPtr hIcon;
}

[ComVisible(false)]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct CMINVOKECOMMANDINFOEX
{
    public uint cbSize;
    public CMIC fMask;
    public IntPtr hwnd;
    public IntPtr verb;
    [MarshalAs(UnmanagedType.LPStr)]
    public string parameters;
    [MarshalAs(UnmanagedType.LPStr)]
    public string directory;
    public int nShow;
    public uint dwHotKey;
    public IntPtr hIcon;
    [MarshalAs(UnmanagedType.LPStr)]
    public string title;
    public IntPtr verbW;
    public string parametersW;
    public string directoryW;
    public string titleW;
    public POINT ptInvoke;
}

[ComVisible(false)]
[Flags]
public enum CMIC : uint
{
    CMIC_MASK_ICON = 0x00000010,
    CMIC_MASK_HOTKEY = 0x00000020,
    CMIC_MASK_NOASYNC = 0x00000100,
    CMIC_MASK_FLAG_NO_UI = 0x00000400,
    CMIC_MASK_UNICODE = 0x00004000,
    CMIC_MASK_NO_CONSOLE = 0x00008000,
    CMIC_MASK_ASYNCOK = 0x00100000,
    CMIC_MASK_NOZONECHECKS = 0x00800000,
    CMIC_MASK_FLAG_LOG_USAGE = 0x04000000,
    CMIC_MASK_SHIFT_DOWN = 0x10000000,
    CMIC_MASK_PTINVOKE = 0x20000000,
    CMIC_MASK_CONTROL_DOWN = 0x40000000
}

[ComVisible(false)]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct POINT
{
    public int X;
    public int Y;
}

[ComVisible(false)]
public enum CLIPFORMAT : uint
{
    CF_TEXT = 1,
    CF_BITMAP = 2,
    CF_METAFILEPICT = 3,
    CF_SYLK = 4,
    CF_DIF = 5,
    CF_TIFF = 6,
    CF_OEMTEXT = 7,
    CF_DIB = 8,
    CF_PALETTE = 9,
    CF_PENDATA = 10,
    CF_RIFF = 11,
    CF_WAVE = 12,
    CF_UNICODETEXT = 13,
    CF_ENHMETAFILE = 14,
    CF_HDROP = 15,
    CF_LOCALE = 16,
    CF_MAX = 17,

    CF_OWNERDISPLAY = 0x0080,
    CF_DSPTEXT = 0x0081,
    CF_DSPBITMAP = 0x0082,
    CF_DSPMETAFILEPICT = 0x0083,
    CF_DSPENHMETAFILE = 0x008E,

    CF_PRIVATEFIRST = 0x0200,
    CF_PRIVATELAST = 0x02FF,

    CF_GDIOBJFIRST = 0x0300,
    CF_GDIOBJLAST = 0x03FF
}

[ComVisible(false)]
[Flags]
public enum CMF : uint
{
    CMF_NORMAL = 0x00000000,
    CMF_DEFAULTONLY = 0x00000001,
    CMF_VERBSONLY = 0x00000002,
    CMF_EXPLORE = 0x00000004,
    CMF_NOVERBS = 0x00000008,
    CMF_CANRENAME = 0x00000010,
    CMF_NODEFAULT = 0x00000020,
    CMF_INCLUDESTATIC = 0x00000040,
    CMF_ITEMMENU = 0x00000080,
    CMF_EXTENDEDVERBS = 0x00000100,
    CMF_DISABLEDVERBS = 0x00000200,
    CMF_ASYNCVERBSTATE = 0x00000400,
    CMF_OPTIMIZEFORINVOKE = 0x00000800,
    CMF_SYNCCASCADEMENU = 0x00001000,
    CMF_DONOTPICKDEFAULT = 0x00002000,
    CMF_RESERVED = 0xFFFF0000
}

[ComVisible(false)]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct MENUITEMINFO
{
    public uint cbSize;
    public MIIM fMask;
    public MFT fType;
    public MFS fState;
    public uint wID;
    public IntPtr hSubMenu;
    public IntPtr hbmpChecked;
    public IntPtr hbmpUnchecked;
    public UIntPtr dwItemData;
    [MarshalAs(UnmanagedType.LPTStr)]
    public string dwTypeData;
    public uint cch;
    public IntPtr hbmpItem;
}

[ComVisible(false)]
[Flags]
public enum MIIM : uint
{
    MIIM_STATE = 0x00000001,
    MIIM_ID = 0x00000002,
    MIIM_SUBMENU = 0x00000004,
    MIIM_CHECKMARKS = 0x00000008,
    MIIM_TYPE = 0x00000010,
    MIIM_DATA = 0x00000020,
    MIIM_STRING = 0x00000040,
    MIIM_BITMAP = 0x00000080,
    MIIM_FTYPE = 0x00000100
}

[ComVisible(false)]
public enum MFT : uint
{
    MFT_STRING = 0x00000000,
    MFT_BITMAP = 0x00000004,
    MFT_MENUBARBREAK = 0x00000020,
    MFT_MENUBREAK = 0x00000040,
    MFT_OWNERDRAW = 0x00000100,
    MFT_RADIOCHECK = 0x00000200,
    MFT_SEPARATOR = 0x00000800,
    MFT_RIGHTORDER = 0x00002000,
    MFT_RIGHTJUSTIFY = 0x00004000
}

[ComVisible(false)]
public enum MFS : uint
{
    MFS_ENABLED = 0x00000000,
    MFS_UNCHECKED = 0x00000000,
    MFS_UNHILITE = 0x00000000,
    MFS_GRAYED = 0x00000003,
    MFS_DISABLED = 0x00000003,
    MFS_CHECKED = 0x00000008,
    MFS_HILITE = 0x00000080,
    MFS_DEFAULT = 0x00001000
}