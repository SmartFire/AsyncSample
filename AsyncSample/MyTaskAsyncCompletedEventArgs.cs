using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace AsyncSample
{
    public class MyTaskAsyncCompletedEventArgs : AsyncCompletedEventArgs
    {
        private int _id;

        public int Id
        {
            get { return _id; }
        }

        public MyTaskAsyncCompletedEventArgs(Exception error, bool cancelled, object userState, int Id)
            : base(error, cancelled, userState)
        {
            _id = Id;
        }
    }
}
