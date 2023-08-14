using System;
using System.Timers;
using ACE.DatLoader.Entity;
using Decal.Adapter;

namespace DecalTextureTest
{
    internal class LandcellTracker : IDisposable
    {
        private Timer pollTimer;
        int LastLandcell;
        private static float interval = 1000.0f;


        public LandcellTracker()
        {
            pollTimer = new Timer(interval);
            Begin();
        }

        private void Begin()
        {
            pollTimer.Elapsed += (s, e) =>
            {
                int cell = CoreManager.Current.Actions.Landcell;

                if (cell != LastLandcell)
                {
                    OnLandblockChanged(new LandcellChangedEventArgs { Landcell = cell });
                }

                LastLandcell = cell;
            };

            pollTimer.Start();
        }

        public void Dispose()
        {
            if (pollTimer != null) { 
                pollTimer.Stop(); 
                pollTimer.Dispose();
            }
        }

        protected virtual void OnLandblockChanged(LandcellChangedEventArgs e)
        {
            EventHandler<LandcellChangedEventArgs> handler = LandcellChangedEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<LandcellChangedEventArgs> LandcellChangedEvent;
    }

    public class LandcellChangedEventArgs : EventArgs
    {
        public int Landcell { get; set; }
    }
}
