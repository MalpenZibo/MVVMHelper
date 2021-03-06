﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static MVVMHelper.Services.DialogService;
using static MVVMHelper.Services.SnackbarService;

namespace MVVMHelper.Services
{
    public interface IServiceHost
    {
        Task<bool> ShowDialog( string dialogId, string hostname, object content, string title, string confirmButton, Action confirmAction, string cancelButton, Action cancelAction, DialogType type );
        
        void CloseDialog( string dialogId, bool result );

        void ShowSnackbar( string hostname, string message, string actionName, Action actionCommand, SnackbarType type );
    }
}
