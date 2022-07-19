using System;
using System.Collections.Generic;
using System.Text;

namespace botshell
{
    using static Win32APIs;

    class WindowEntry
    {
        public IntPtr hwnd;


     
        public bool isVisible
        {
            get
            {
                return IsWindowVisible(this.hwnd);
            }
        }

        public bool isTopLevelWindow
        {
            get
            {
                return this.hwnd != GetAncestor(this.hwnd, GetAncestorFlags.GetRoot);
            }
        }


        public string className
        {
            get
            {
                StringBuilder builder = new StringBuilder(1024);

                GetClassName(this.hwnd, builder, builder.Capacity);

                return builder.ToString();
            }
        }

        public string title
        {
            get
            {
                StringBuilder builder = new StringBuilder(1025);

                GetWindowText(this.hwnd, builder, builder.Capacity);
                return builder.ToString();
            }

            set
            {
                SetWindowText(this.hwnd, value);
            }
        }


        public RECT windowRect
        {
            get
            {
                RECT res = new RECT();
                GetWindowRect(this.hwnd, out res);

                return res;
            }
        }

        public RECT clientRect
        {
            get
            {
                RECT res = new RECT();
                GetClientRect(this.hwnd, out res);

                return res;
            }
        }

        public POINT clientScreenPosition
        {
            get
            {
                var p = new POINT(0, 0);

                ClientToScreen(this.hwnd, ref p);

                return p;
            }
        }

        public RECT relativeWindowRect
        {
            get
            {
                RECT res = new RECT();

                GetWindowRect(this.hwnd, out res);

                var parent = this.parent;

                if (parent != null)
                {
                    if (this.isTopLevelWindow == true)
                    {
                        POINT pt = new POINT(windowRect.X, windowRect.Y);

                        ScreenToClient(parent.hwnd, ref pt);

                        res.X = pt.X;
                        res.Y = pt.Y;
                    }
                }

                return res;
            }
        }



        public WindowEntry next
        {
            get
            {
                WindowEntry ent = null;

                IntPtr tempHwnd = GetWindow(this.hwnd, GetWindowType.GW_HWNDNEXT);

                if (tempHwnd != IntPtr.Zero)
                    ent = new WindowEntry(tempHwnd);

                return ent;
            }
        }

        public WindowEntry prev
        {
            get
            {
                WindowEntry ent = null;

                IntPtr tempHwnd = GetWindow(this.hwnd, GetWindowType.GW_HWNDPREV);

                if (tempHwnd != IntPtr.Zero)
                    ent = new WindowEntry(tempHwnd);

                return ent;
            }
        }

        public WindowEntry firstChild
        {
            get
            {
                WindowEntry ent = null;

                IntPtr tempHwnd = GetWindow(this.hwnd, GetWindowType.GW_CHILD);

                if (tempHwnd != IntPtr.Zero)
                    ent = new WindowEntry(tempHwnd);

                return ent;
            }
        }

        public WindowEntry parent
        {
            get
            {
                WindowEntry ent = null;

                IntPtr tempParent = GetParent(this.hwnd);

                if (tempParent != IntPtr.Zero)
                    ent = new WindowEntry(tempParent);

                return ent;
            }
        }


        public static WindowEntry desktop
        {
            get
            {
                IntPtr temp = GetDesktopWindow();

                WindowEntry ent = null;

                if (temp != null)
                    ent = new WindowEntry(temp);

                return ent;
            }
        }

        public static WindowEntry foregroundWindow
        {
            get
            {
                return new WindowEntry(GetForegroundWindow());
            }

            set
            {
                SetForegroundWindow(value.hwnd);
            }
        }


        public List<WindowEntry> children
        {
            get
            {
                List<WindowEntry> res = new List<WindowEntry>();

                WindowEntry ent = this.firstChild;

                while (ent != null)
                {
                    res.Add(ent);
                    ent = ent.next;
                }

                return res;
            }
        }

        public List<WindowEntry> allChildren
        {
            get
            {
                List<WindowEntry> res = new List<WindowEntry>();

                EnumChildWindows(this.hwnd, (hwnd2, lpvoid) =>
                {
                    res.Add(new WindowEntry(hwnd2));

                    return true;
                }, IntPtr.Zero);

                return res;
            }
        }


        public WindowEntry(IntPtr hwnd)
        {
            this.hwnd = hwnd;
        }

        public WindowEntry findWindow(string className, string title = null)
        {
            WindowEntry ent = null;

            IntPtr temp = FindWindowEx(this.hwnd, IntPtr.Zero, className, title);

            if (temp != null)
                ent = new WindowEntry(temp);

            return ent;
        }
    }
}
