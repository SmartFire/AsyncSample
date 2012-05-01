using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace AsyncSample
{
    public class MyTaskProgressChangedEventArgs : ProgressChangedEventArgs
    {
        private string _currentFile;
        private int _id;

        public int Id
        {
            get { return _id; }
        }

        public string CurrentFile
        {
            get { return _currentFile; }
        }

        public MyTaskProgressChangedEventArgs(int progressPercentage, string currentFile, int Id,
          object userState)
            : base(progressPercentage, userState)
        {
            _currentFile = currentFile;
            _id = Id;
        }
    }
}
