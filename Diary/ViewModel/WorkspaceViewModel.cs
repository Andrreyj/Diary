﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Diary.ViewModel
{
    public class WorkspaceViewModel: BaseViewModel
    {

        BaseViewModel currentContentVM;
        public BaseViewModel CurrentContentVM
        {
            get
            { 
                return currentContentVM;
            }
            set
            {
                currentContentVM = value;

                OnPropertyChanged();
            }
        }

    }
}
