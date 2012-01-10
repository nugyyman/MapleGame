using System;
using System.Windows.Shell;
using Loki.IO;
using System.Reflection;

namespace Loki.Shell
{
    public static class Shortcuts
    {
        public static void Apply()
        {
            if (Environment.OSVersion.Version.Major >= 6 && Environment.OSVersion.Version.Minor >= 1)
            {
                JumpList jumpList = new JumpList();

                for (int port = 7575; port < 7575 + Settings.GetInt("Log/JumpLists"); port++)
                {
                    jumpList.JumpItems.Add(new JumpTask()
                    {
                        ApplicationPath = Application.ExecutablePath + "ChannelServer.exe",
                        Title = "Launch on port " + port.ToString(),
                        Arguments = port.ToString()
                    });
                }

                jumpList.Apply();

                if (System.Windows.Application.Current == null)
                {
                    new System.Windows.Application();
                }

                JumpList.SetJumpList(System.Windows.Application.Current, jumpList);
            }
        }
    }
}
