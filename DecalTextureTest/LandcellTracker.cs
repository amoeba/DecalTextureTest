using System;
using System.Timers;
using ACE.DatLoader.Entity;
using Decal.Adapter;

namespace DecalTextureTest
{
    internal class LandcellTracker : IDisposable
    {
        private Timer timer;
        int LastLandcell;
        private static float interval = 1000.0f;


        public LandcellTracker()
        {
            timer = new Timer(interval);
            Begin();
        }

        private void Begin()
        {
            timer.Elapsed += (s, e) =>
            {
                int cell = CoreManager.Current.Actions.Landcell;

                if (cell != LastLandcell)
                {
                    OnLandblockChanged(new LandcellChangedEventArgs { Landcell = cell });
                }

                LastLandcell = cell;
            };

            timer.Start();
        }

        public void Dispose()
        {
            if (timer != null) { 
                timer.Stop(); 
                timer.Dispose();
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
