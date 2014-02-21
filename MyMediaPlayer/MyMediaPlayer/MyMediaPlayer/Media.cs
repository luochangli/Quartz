using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Microsoft.VisualBasic.Devices;
using QuartzTypeLib;


namespace MyMediaPlayer
{
   public class Media
    {
        public Media()
        {
        }

        public enum mStatus{Play, Stop, Pause, Empty}
        public mStatus currentStatus;

        private const int WM_APP = 0x8000;
        private const int WM_GRAPHNOTIFY = WM_APP + 1;
        private const int WS_CHILD = 0x40000000;
        private const int WS_CLIPCHILDREN = 0x2000000;

        private FilgraphManager graphManager = new FilgraphManager();

        private IVideoWindow   mWindow = null;
        private IMediaControl  mControl = null;
        public IMediaPosition  mPosition = null;
        private IBasicAudio    mAudio =null;
        private IMediaEvent m_objMediaEvent = null;
        private IMediaEventEx m_objMediaEventEx = null;
       
        
        public int second1;
        public int ctlTrackPosition;
       
       //======================================================================

       public void loadFile(string sfile, Panel vPanel)
       {
           if(mControl != null) mControl.Stop();
           if (mWindow != null)
           {
               mWindow.Visible = 0;
               mWindow.Owner = 0;
           }
           if (mControl != null) mControl = null;
           if (mPosition != null) mPosition = null;
           if (mWindow != null) mWindow = null;
           if (mAudio != null) mAudio = null;
           if (graphManager != null) graphManager = null;

           graphManager = new FilgraphManager();
           graphManager.RenderFile(sfile);
           mWindow   = graphManager as IVideoWindow;
           mControl  = graphManager as IMediaControl;
           mPosition = graphManager as IMediaPosition;
           mAudio    = graphManager as IBasicAudio;

           mWindow.Owner = (int)vPanel.Handle;
           mWindow.WindowStyle = WS_CHILD | WS_CLIPCHILDREN;
           mWindow.SetWindowPosition(vPanel.ClientRectangle.Left,
                                        vPanel.ClientRectangle.Top,
                                        vPanel.ClientRectangle.Width,
                                        vPanel.ClientRectangle.Height);
            //=========================================================
            second1 = (int) mPosition.Duration;
            ctlTrackPosition = second1;
            mControl.Run();
            currentStatus = mStatus.Play;
            
        }

       public void OpenFile(string sfile, Panel vPanel)
       {
           CleanUp();

           graphManager = new FilgraphManager();
           graphManager.RenderFile(sfile);

           mAudio = graphManager as IBasicAudio;

           try
           {
               mWindow = graphManager as IVideoWindow;
               mWindow.Owner = (int)vPanel.Handle;
               mWindow.WindowStyle = WS_CHILD | WS_CLIPCHILDREN;
               mWindow.SetWindowPosition(vPanel.ClientRectangle.Left,
                   vPanel.ClientRectangle.Top,
                   vPanel.ClientRectangle.Width,
                   vPanel.ClientRectangle.Height);
           }
           catch (Exception)
           {
               mWindow = null;

           }

           m_objMediaEvent = graphManager as IMediaEvent;

           m_objMediaEventEx = graphManager as IMediaEventEx;
           m_objMediaEventEx.SetNotifyWindow((int)vPanel.Handle, WM_GRAPHNOTIFY, 0);
           mPosition = graphManager as IMediaPosition;
           mControl = graphManager as IMediaControl;

           mControl.Run();
       }

       private void CleanUp()
       {
           if (mControl != null)
               mControl.Stop();
           if (m_objMediaEventEx != null)
               m_objMediaEventEx.SetNotifyWindow(0, 0, 0);

           if (mWindow != null)
           {
               mWindow.Visible = 0;
               mWindow.Owner = 0;
           }

           if (mControl != null) mControl = null;
           if (mPosition != null) mPosition = null;
           if (m_objMediaEventEx != null) m_objMediaEventEx = null;
           if (m_objMediaEvent != null) m_objMediaEvent = null;
           if (mWindow != null) mWindow = null;
           if (mAudio != null) mAudio = null;
           if (graphManager != null) graphManager = null;
       }

       public bool playMedia()
        {
           
           mControl.Run();
            currentStatus = mStatus.Play;
            return true;
        }

       public bool pauseMedia()
        {
            mControl.Pause();
            currentStatus = mStatus.Pause; 
            return true;
        }

       public bool stopMedia()
        {
            mControl.Stop();
            mPosition.CurrentPosition = 0;
            currentStatus = mStatus.Stop;
            return true;
        }

       public void trbZoom(TrackBar zoomTrackBar, Panel vPanel)
       {
           mWindow.SetWindowPosition(vPanel.ClientRectangle.Left,
                                       vPanel.ClientRectangle.Top,
                                       vPanel.ClientRectangle.Width + zoomTrackBar.Value,
                                       vPanel.ClientRectangle.Height + zoomTrackBar.Value);
           

       }// растягивание видео

       public void ScrollZoomMedia(TrackBar zoomTrackBar, HScrollBar hScroll, VScrollBar vScroll, Panel vPanel)
       {
           mWindow.SetWindowPosition(vPanel.ClientRectangle.Left-hScroll.Value,
                                       vPanel.ClientRectangle.Top - vScroll.Value,
                                       vPanel.ClientRectangle.Width + zoomTrackBar.Value,
                                       vPanel.ClientRectangle.Height + zoomTrackBar.Value);
       }

       public void mFullScrin(Panel vPanel)
       {
          // mWindow.Owner = ;
       }

       public void mVolume(TrackBar vTrackBar)
       {
           mAudio.Volume = vTrackBar.Value/100;
       }

       public void TimeManager(ToolStripLabel tsLabel)
        {
          if (tsLabel.Text == "00:00:00")
           {
               second1 = (int)mPosition.Duration;
           }

           if (mPosition != null)
           {
               int hour = second1 / 3600;
               int minute = (second1 - (hour * 3600)) / 60;
               int second = second1 - (hour * 3600 + minute * 60);
               tsLabel.Text = String.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
               second1 -= 1;
           }
           else
           {
               tsLabel.Text = "00:00:00";
           }
           
        }

       public void trbMedia(TrackBar trBar)
       {
          trBar.Value = ((int)mPosition.CurrentPosition * 100) / ctlTrackPosition;
       }

       public void trbScrollMedia(double sec)
       {
           mPosition.CurrentPosition = (double)(ctlTrackPosition*sec/100);
       }
    }
}
