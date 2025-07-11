using Client.Const.Type;
using Client.Helpers;
using Client.Views.Toast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static SocialMediaMini.Shared.Dto.Respone.Respone_PostDetail;

namespace Client.ViewModels
{
    public static class ToastManager
    {
        private static bool isRunning = false;
        private static Queue<Tuple<ToastType, string>> Messages = new Queue<Tuple<ToastType, string>>();
        public static void Start()
        {
            Messages.Clear();
            isRunning = false;
            toastCur = null;
            Run();
        }



        private static Window toastCur = null;
        private static long startTimeShow = 0;
        private static readonly int TIME_SHOW = 2000;
        private static void Run()
        {
            if (isRunning)
            {
                return;
            }
            isRunning = true;
            new Thread(() =>
            {
                while (isRunning)
                {
                    Thread.Sleep(50);
                    try
                    {
                        if (toastCur == null)
                        {
                            lock (Messages)
                            {
                                if (Messages.Any())
                                {
                                    var toast = Messages.Dequeue();
                                    UIHelpers.InvokeDispatcherUI(() =>
                                    {
                                        if (toast.Item1 == ToastType.Success)
                                        {
                                            toastCur = new Views.Toast.ToastSuccess(toast.Item2);
                                        }
                                        else
                                        {
                                            toastCur = new Views.Toast.ToastError(toast.Item2);
                                        }
                                        startTimeShow = Utils.CurrentTimeMillis();
                                        toastCur.Show();
                                    });

                                }
                            }
                        }
                        else
                        {
                            if (Utils.CurrentTimeMillis() - startTimeShow > TIME_SHOW)
                            {
                                UIHelpers.InvokeDispatcherUI(() =>
                                {
                                    if (toastCur is ToastSuccess)
                                    {
                                        ((ToastSuccess)toastCur).CloseToast();
                                    }
                                    else
                                    {
                                        ((ToastError)toastCur).CloseToast();
                                    }
                                });
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }).Start();
        }

        public static void Stop()
        {
            Messages.Clear();
            isRunning = false;
            toastCur = null;
        }


        public static void AddToast(ToastType type, string content)
        {
            lock (Messages)
            {
                Messages.Enqueue(new Tuple<ToastType, string>(type, content));
            }
        }

        public static void CloseToastCur()
        {
            toastCur = null;
        }
    }
}
