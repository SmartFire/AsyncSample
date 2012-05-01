using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Runtime.Remoting.Messaging;
using AsyncSample;

namespace AsyncExample
{
    class Task
    {
        private delegate void MyTaskWorkerDelegate(string[] files, AsyncOperation async, MyAsyncContext asyncContext, out bool cancelled);

        private readonly object _sync = new object();
        private bool _myTaskIsRunning = false;

        public event EventHandler<MyTaskAsyncCompletedEventArgs> MyTaskCompleted;
        public event EventHandler<MyTaskProgressChangedEventArgs> MyTaskProgressChanged;

        private MyAsyncContext _myTaskContext = null;

        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Task(int id)
        {
            Id = id;
        }


        public bool IsBusy
        {
            get { return _myTaskIsRunning; }
        }

        private void MyTaskWorker(string[] files, AsyncOperation async, MyAsyncContext asyncContext, out bool cancelled)
        {
            cancelled = false;

            for (int i = 0; i < files.Length; i++)
            {
                // a time consuming operation with a file (compression, encryption etc.)
                Thread.Sleep(1000);

                // compute progress
                int progressPercentage = 100 * (i + 1) / files.Length;

                // raise the progress changed event
                MyTaskProgressChangedEventArgs eArgs = new MyTaskProgressChangedEventArgs(progressPercentage, files[i], Id, null);
                async.Post(delegate(object e) { OnMyTaskProgressChanged((MyTaskProgressChangedEventArgs)e); }, eArgs);

                if (asyncContext.IsCancelling)
                {
                    cancelled = true;
                    return;
                }
            }
        }

        protected virtual void OnMyTaskProgressChanged(MyTaskProgressChangedEventArgs e)
        {
            if (MyTaskProgressChanged != null)
                MyTaskProgressChanged(this, e);
        }

        public void MyTaskAsync(string[] files)
        {
            MyTaskWorkerDelegate worker = new MyTaskWorkerDelegate(MyTaskWorker);
            AsyncCallback completedCallback = new AsyncCallback(MyTaskCompletedCallback);

            lock (_sync)
            {
                if (_myTaskIsRunning)
                    throw new InvalidOperationException("The control is currently busy.");

                AsyncOperation async = AsyncOperationManager.CreateOperation(null);
                MyAsyncContext context = new MyAsyncContext();
                bool cancelled;

                worker.BeginInvoke(files, async, context, out cancelled, completedCallback, async);

                _myTaskIsRunning = true;
                _myTaskContext = context;
            }
        }

        private void MyTaskCompletedCallback(IAsyncResult ar)
        {
            // get the original worker delegate and the AsyncOperation instance
            MyTaskWorkerDelegate worker = (MyTaskWorkerDelegate)((AsyncResult)ar).AsyncDelegate;
            AsyncOperation async = (AsyncOperation)ar.AsyncState;
            bool cancelled;

            // finish the asynchronous operation
            worker.EndInvoke(out cancelled, ar);

            // clear the running task flag
            lock (_sync)
            {
                _myTaskIsRunning = false;
                _myTaskContext = null;
            }

            // raise the completed event
            MyTaskAsyncCompletedEventArgs completedArgs = new MyTaskAsyncCompletedEventArgs(null, cancelled, null, Id);
            async.PostOperationCompleted(delegate(object e) { OnMyTaskCompleted((MyTaskAsyncCompletedEventArgs)e); }, completedArgs);
        }

        protected virtual void OnMyTaskCompleted(MyTaskAsyncCompletedEventArgs e)
        {
            if (MyTaskCompleted != null)
                MyTaskCompleted(this, e);
        }

        public void CancelAsync()
        {
            lock (_sync)
            {
                if (_myTaskContext != null)
                    _myTaskContext.Cancel();
            }
        }
    }
}
