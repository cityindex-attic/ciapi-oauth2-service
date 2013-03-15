using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DesktopAppTest
{
    public class WindowTitleBrowser
    {
        public static string GetWindowTitleContaining(string needle)
        {
            return GrabAllWindowTitles().FirstOrDefault(title => title.Contains(needle));
        }

        private static List<String> GrabAllWindowTitles()
        {
            var windowTitles = new List<String>();
            foreach (Process procesInfo in Process.GetProcesses())
            {
                foreach (ProcessThread threadInfo in procesInfo.Threads)
                {
                    IntPtr[] windows = GetWindowHandlesForThread(threadInfo.Id);
                    if (windows != null && windows.Length > 0)
                        foreach (IntPtr hWnd in windows)
                        {
                            var windowTitle = string.Format("text=>{0} caption=>{1}",
                                                            GetText(hWnd), GetEditText(hWnd));
                            windowTitles.Add(windowTitle);
                        }
                            
                }
            }
            return windowTitles;
        }

        private static IntPtr[] GetWindowHandlesForThread(int threadHandle)
        {
            _results.Clear();
            EnumWindows(WindowEnum, threadHandle);
            return _results.ToArray();
        }

        // enum windows

        private delegate int EnumWindowsProc(IntPtr hwnd, int lParam);

        [DllImport("user32.Dll")]
        private static extern int EnumWindows(EnumWindowsProc x, int y);
        [DllImport("user32")]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowsProc callback, int lParam);
        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        private static List<IntPtr> _results = new List<IntPtr>();

        private static int WindowEnum(IntPtr hWnd, int lParam)
        {
            int processID = 0;
            int threadID = GetWindowThreadProcessId(hWnd, out processID);
            if (threadID == lParam)
            {
                _results.Add(hWnd);
                EnumChildWindows(hWnd, AddWindowHandleToResults, threadID);
            }
            return 1;
        }
        private static int AddWindowHandleToResults(IntPtr hWnd, int lParam)
        {
            _results.Add(hWnd);
            return 1;
        }

        // get window text

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        private static string GetText(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        // get richedit text 

        public const int GWL_ID = -12;
        public const int WM_GETTEXT = 0x000D;

        [DllImport("User32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int index);
        [DllImport("User32.dll")]
        public static extern IntPtr SendDlgItemMessage(IntPtr hWnd, int IDDlgItem, int uMsg, int nMaxCount, StringBuilder lpString);
        [DllImport("User32.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);

        private static StringBuilder GetEditText(IntPtr hWnd)
        {
            Int32 dwID = GetWindowLong(hWnd, GWL_ID);
            IntPtr hWndParent = GetParent(hWnd);
            StringBuilder title = new StringBuilder(128);
            SendDlgItemMessage(hWndParent, dwID, WM_GETTEXT, 128, title);
            return title;
        }

        
    }
}


