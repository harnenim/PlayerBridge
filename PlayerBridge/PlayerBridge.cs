using System.Windows.Forms;

namespace PlayerBridge
{
    public abstract class PlayerBridge
    {
        public int hwnd;
        protected string exe = "-";
        protected int editorHwnd;

        public int SendMessage(int wMsg, int wParam, int lParam)
        {
            return (hwnd > 0) ? WinAPI.SendMessage(hwnd, wMsg, wParam, lParam) : 0;
        }
        protected PlayerBridge()
        {
            hwnd = FindPlayer();
        }
        public virtual bool CheckAndRerfreshPlayer()
        {
            if (hwnd > 0)
            {   // 플레이어 값 존재
                if (WinAPI.IsWindow(hwnd))
                {   // 플레이어 살아있음
                    return true;
                }
                else
                {   // 플레이어 종료됨
                    hwnd = 0;
                    initialOffset.top = 0;
                    initialOffset.left = 0;
                    initialOffset.right = 0;
                    initialOffset.bottom = 0;
                }
            }
            else
            {   // 플레이어 스캔
                FindPlayer();
            }
            return false;
        }

        public RECT initialOffset = new RECT();
        public RECT currentOffset = new RECT();

        // 창 위치 가져오기
        public RECT? GetWindowInitialPosition()
        {
            WinAPI.GetWindowRect(hwnd, ref initialOffset);
            return (initialOffset.top + 100 < initialOffset.bottom) ? initialOffset : (RECT?)null;
        }
        public RECT GetWindowPosition()
        {
            WinAPI.GetWindowRect(hwnd, ref currentOffset);
            return currentOffset;
        }

        // 종료 전 플레이어 원위치
        public void ResetPosition()
        {
            if (initialOffset.top + 100 < initialOffset.bottom)
            {
                WinAPI.MoveWindow(hwnd, initialOffset.left, initialOffset.top, initialOffset.right - initialOffset.left, initialOffset.bottom - initialOffset.top, true);
            }
        }
        // 프로그램 설정에 따른 위치로
        public void MoveWindow()
        {
            if (currentOffset.top + 100 < currentOffset.bottom)
            {
                WinAPI.MoveWindow(hwnd, currentOffset.left, currentOffset.top, currentOffset.right - currentOffset.left, currentOffset.bottom - currentOffset.top, true);
            }
        }
        public void MoveWindow(int x, int y)
        {
            currentOffset.left += x;
            currentOffset.top += y;
            currentOffset.right += x;
            currentOffset.bottom += y;
            MoveWindow();
        }

        public void DoExit()
        {
            WinAPI.PostMessage(hwnd, 0x0010/*WM_CLOSE*/, 0, 0);
        }

        public void SetEditorHwnd(int hwnd)
        {
            editorHwnd = hwnd;
        }
        private int FindPlayer()
        {
            return hwnd = WinAPI.FindWindow(exe, null);
        }
        public int FindPlayer(string exe)
        {
            this.exe = (exe.Length > 4) ? exe.Substring(0, exe.Length - 4) : "-";
            return FindPlayer();
        }

        public abstract int OpenFile(string path);

        public abstract int GetFileName();
        public abstract string AfterGetFileName(Message m);

        public abstract int GetFps();
        public abstract int PlayOrPause();
        public abstract int Pause();
        public abstract int Play();
        public abstract int Stop();
        public abstract int GetTime();
        public abstract int MoveTo(int time);
    }
}
