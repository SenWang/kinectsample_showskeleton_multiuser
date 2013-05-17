#region

using System;
using System.Linq;
using System.Windows;
using Microsoft.Kinect;
using System.Collections.Generic;

#endregion

namespace SoulSolutions.Kinect.SampleApplication
{
    public partial class MainWindow : Window
    {
        private KinectSensor nui = KinectSensor.KinectSensors[0];

        public MainWindow()
        {
            InitializeComponent();
            Loaded += WindowLoaded;
            Closed += WindowClosed;
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            //Cleanup
            nui.Stop();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            nui.Start();

            #region TransformSmooth

            ////Use to transform and reduce jitter
            var parameters = new TransformSmoothParameters
                                 {
                                     Smoothing = 0.75f,
                                     Correction = 0.0f,
                                     Prediction = 0.0f,
                                     JitterRadius = 0.05f,
                                     MaxDeviationRadius = 0.04f
                                 };

            nui.SkeletonStream.Enable(parameters);

            #endregion

            nui.SkeletonFrameReady += NuiSkeletonFrameReady;
        }


        //停止自動選擇骨架功能，由開發者自己選擇要追蹤的骨架
        private void NuiSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            SkeletonFrame allSkeletons = e.OpenSkeletonFrame();

            using (SkeletonFrame skframe = e.OpenSkeletonFrame())
            {
                if (skframe != null)
                {
                    #region enable app choose skleltons
                    if (!nui.SkeletonStream.AppChoosesSkeletons)
                    {
                        nui.SkeletonStream.AppChoosesSkeletons = true;
                    }
                    #endregion

                    Skeleton[] FrameSkeletons = new Skeleton[skframe.SkeletonArrayLength];
                    skframe.CopySkeletonDataTo(FrameSkeletons);

                    var alluser = from s in FrameSkeletons
                                     where s.TrackingState != SkeletonTrackingState.NotTracked
                                     select s;

                    int allusers = alluser.Count() ;
                    Title = "所有可追蹤到的骨架數量為 : " + allusers ;
                    if (allusers > 0)
                    {
                        Skeleton user = ClosestUser(alluser);
                        nui.SkeletonStream.ChooseSkeletons(user.TrackingId);
                        skeleton1.SkeletonData = user;
                    }
                }
            }           
        }
        private Skeleton ClosestUser(IEnumerable<Skeleton> users)
        {
            float mindist = users.Min( s => s.Position.Z) ;
            return users.FirstOrDefault( s => s.Position.Z == mindist);
        }


        ////只選用離感應器最近的使用者顯示其骨架
        //private void NuiSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        //{
        //    SkeletonFrame allSkeletons = e.OpenSkeletonFrame();

        //    using (SkeletonFrame skframe = e.OpenSkeletonFrame())
        //    {
        //        if (skframe != null)
        //        {
        //            Skeleton[] FrameSkeletons = new Skeleton[skframe.SkeletonArrayLength];
        //            skframe.CopySkeletonDataTo(FrameSkeletons);

        //            #region user count
        //            var nonactiveuser = from s in FrameSkeletons
        //                                where s.TrackingState != SkeletonTrackingState.Tracked
        //                                select s;
        //            var activeuser = from s in FrameSkeletons
        //                             where s.TrackingState == SkeletonTrackingState.Tracked
        //                             select s;

        //            int users = activeuser.Count();
        //            int nonusers = nonactiveuser.Count();
        //            Title = "目前追蹤到的骨架數量為 : " + users + "  尚未骨架數量為 : " + nonusers;
        //            #endregion

        //            if (users == 1)
        //            {
        //                skeleton1.SkeletonData = activeuser.ElementAt(0);
        //            }
        //            else if (users == 2)
        //            {
        //                var dist1 = activeuser.ElementAt(0).Position.Z;
        //                var dist2 = activeuser.ElementAt(1).Position.Z;
        //                if (dist1 < dist2)
        //                    skeleton1.SkeletonData = activeuser.ElementAt(0);
        //                else
        //                    skeleton1.SkeletonData = activeuser.ElementAt(1);
        //            }
        //        }
        //    }
        //}

        ////使用 LINQ語法篩選出所有已被追蹤之骨架，分別對不同的已追蹤骨架給予不同的骨架資訊
        //private void NuiSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        //{
        //    SkeletonFrame allSkeletons = e.OpenSkeletonFrame();

        //    using (SkeletonFrame skframe = e.OpenSkeletonFrame())
        //    {
        //        if (skframe != null)
        //        {
        //            Skeleton[] FrameSkeletons = new Skeleton[skframe.SkeletonArrayLength];
        //            skframe.CopySkeletonDataTo(FrameSkeletons);

        //            var activeuser = from s in FrameSkeletons
        //                             where s.TrackingState == SkeletonTrackingState.Tracked
        //                             select s;

        //            int users = activeuser.Count();
        //            Title = "目前追蹤到的骨架數量為 : " + users;

        //            if (users == 1)
        //            {
        //                skeleton1.SkeletonData = activeuser.ElementAt(0);
        //            }
        //            else if (users == 2)
        //            {
        //                skeleton1.SkeletonData = activeuser.ElementAt(0);
        //                skeleton2.SkeletonData = activeuser.ElementAt(1);
        //            }
        //        }
        //    }
        //}

    }
}