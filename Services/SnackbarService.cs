using MVVMHelper.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MVVMHelper.Services
{
    public class SnackbarService
    {
        public enum SnackbarType
        {
            MESSAGE,
            SAVE,
            WARN,
            ERROR
        }

        private DialogService dlg;
        private IServiceHost host;

        public SnackbarService( DialogService dlg, IServiceHost host )
        {
            this.dlg = dlg;
            this.host = host;
        }

        public string DefaultHostname { get; set; } = "RootSnackbar";    

        public void SendMessage( string message, string hostname = null )
        {
            host.ShowSnackbar( ( hostname != null ? hostname : DefaultHostname ), message, null, null, SnackbarType.MESSAGE );
        }     

        public void SendMessage( string message, string actionName, Action actionCommand = null, string hostname = null )
        {
            host.ShowSnackbar( 
                ( hostname != null ? hostname : DefaultHostname ), 
                message, 
                actionName, 
                actionCommand,
                SnackbarType.MESSAGE 
            );
        }   

        public void SendErrorMessage( string message, Exception e )
        {
            SendErrorMessage( message, e, DefaultHostname );
        }

        public void SendErrorMessage( string message, Exception e, string hostname = null )
        {
            host.ShowSnackbar( ( hostname != null ? hostname : DefaultHostname ), message, "Dettagli", () => { dlg.ShowError( e, message, "Ok" ); }, SnackbarType.ERROR );         
        }

        public void SendWarnMessage( string message )
        {
            SendWarnMessage( message, DefaultHostname );
        }

        public void SendWarnMessage( string message, string hostname = null )
        {
            host.ShowSnackbar( ( hostname != null ? hostname : DefaultHostname ), message, null, null, SnackbarType.WARN );
        }

        public void SendSaveMessage( string message )
        {
            SendSaveMessage( message, DefaultHostname );
        }

        public void SendSaveMessage( string message, string hostname = null )
        {
            host.ShowSnackbar( ( hostname != null ? hostname : DefaultHostname ), message, null, null, SnackbarType.SAVE );
        }        
    }
}
