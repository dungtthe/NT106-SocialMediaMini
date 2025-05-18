using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Helpers
{
    public static class UIHelpers
    {
        public static void InvokeDispatcherUI(Action action)
        {
            if (Application.Current != null && Application.Current.Dispatcher != null)
            {
                Application.Current.Dispatcher.BeginInvoke(action);
            }
            else
            {
                Debug.WriteLine("Dispatcher is not available.");
            }
        }
    }
}
