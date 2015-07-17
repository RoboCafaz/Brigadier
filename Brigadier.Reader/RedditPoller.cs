using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Brigadier.EntityFramework;
using Brigadier.Reader.Analyzer;

namespace Brigadier.Reader
{
    public class RedditPoller : IDisposable
    {
        private const int Timeout = 1000 * 60 * 5;

        private readonly Timer _timer;
        private readonly BackgroundWorker _worker;

        public RedditPoller()
        {
            _worker = new BackgroundWorker();
            _worker.DoWork += worker_DoWork;
            _timer = new Timer(Timeout);
            _timer.Elapsed += timer_Elapsed;
            _timer.Start();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!_worker.IsBusy)
            {
                _worker.RunWorkerAsync();
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            SubReader.Run();
            PostReader.Run();
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Dispose();
            _worker.CancelAsync();
            while (_worker.CancellationPending)
            {
                // Wait...
            }
            _worker.Dispose();
        }
    }
}
