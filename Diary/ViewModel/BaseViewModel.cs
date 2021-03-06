﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Diary.ViewModel
{
    public class BaseViewModel: INotifyPropertyChanged
    {
        #region Properties 

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        #endregion // Properties

        #region Protected helpers

        protected bool CheckTimeNote(DateTime selectedDate)
        {

            if (selectedDate.ToShortDateString() == DateTime.Now.ToShortDateString())
            {
                return true;
            }
            return false;

        }

        #endregion // Protected helpers
    }
}
