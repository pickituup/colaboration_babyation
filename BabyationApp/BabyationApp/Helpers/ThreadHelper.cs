using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyationApp.Helpers
{
    public static class ThreadHelper
    {
        public static int MainThreadId { get; private set; }

        public static void Initialize(int mainThreadId)
        {
            MainThreadId = mainThreadId;
        }

        public static bool IsOnMainThread => Environment.CurrentManagedThreadId == MainThreadId;
    }
}
