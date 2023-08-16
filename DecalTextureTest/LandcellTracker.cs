using System;
using System.Timers;
using ACE.DatLoader.Entity;
using Decal.Adapter;

namespace DecalTextureTest
{
    internal class LandcellTracker : IDisposable
    {
        private Timer pollTimer;
        int LastLandblock;
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
                Location l = new Location(CoreManager.Current.Actions.Landcell);

                if (l.GetLandcell() != LastLandcell)
                {
                    OnLandcellChanged(new LandcellChangedEventArgs { Landcell = l.GetLandcell() });
                }

                if (l.GetLandblock() != LastLandblock)
                {
                    OnLandblockChanged(new LandblockChangedEventArgs { Landblock = l.GetLandblock() });
                }

                LastLandcell = l.GetLandcell();
                LastLandblock = l.GetLandblock();
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

        // Landcell
        protected virtual void OnLandcellChanged(LandcellChangedEventArgs e)
        {
            EventHandler<LandcellChangedEventArgs> handler = LandcellChangedEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<LandcellChangedEventArgs> LandcellChangedEvent;

        // Landblock
        protected virtual void OnLandblockChanged(LandblockChangedEventArgs e)
        {
            EventHandler<LandblockChangedEventArgs> handler = LandblockChangedEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<LandblockChangedEventArgs> LandblockChangedEvent;
    }

    public class LandcellChangedEventArgs : EventArgs
    {
        public int Landcell { get; set; }
    }

    public class LandblockChangedEventArgs : EventArgs
    {
        public int Landblock { get; set; }
    }
}
