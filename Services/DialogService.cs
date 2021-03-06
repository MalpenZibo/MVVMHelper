﻿using GalaSoft.MvvmLight.Views;
using MVVMHelper.Helpers;
using MVVMHelper.Services;
using MVVMHelper.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MVVMHelper.Services
{
    public class DialogService
    {
        public enum DialogType
        {
            ERROR,
            MESSAGE,
            CUSTOM,
            VIEW
        }

        private ContentNavigationService cns;
        private IServiceHost host;

        public DialogService( ContentNavigationService cns, IServiceHost host )
        {
            this.cns = cns;
            this.host = host;
        }

        public string DefaultHostname { get; set; } = "RootDialog";

        public string DefaultCustomHostname { get; set; } = "CustomRootDialog";

        public async Task ShowError( Exception error, string title, string buttonText, string hostname = null )
        {
            await ShowError( error.ToString(), title, buttonText, hostname );
        }

        public async Task ShowError( string message, string title, string buttonText, string hostname = null )
        {
            string id = Guid.NewGuid().ToString();

            await host.ShowDialog( 
                id,
                ( hostname != null ? hostname : DefaultHostname ), 
                message, 
                title, 
                buttonText, 
                () => host.CloseDialog( id, true ), 
                null,
                null, 
                DialogType.ERROR 
            );
        }

        public async Task ShowMessage( string message, string title, string buttonText = "Ok", string hostname = null  )
        {
            await ShowConfirmMessage( message, title, buttonText, null, hostname );
        }

        public async Task<bool> ShowConfirmMessage( string message, string title, string buttonConfirmText, string buttonCancelText, string hostname = null )
        {
            string id = Guid.NewGuid().ToString();

            return await host.ShowDialog( 
                id,
                ( hostname != null ? hostname : DefaultHostname ),
                message, 
                title, 
                buttonConfirmText,
                () => host.CloseDialog( id, true ),
                buttonCancelText, 
                () => host.CloseDialog( id, false ),
                DialogType.MESSAGE 
            );
        }

        public async Task<bool> ShowCustomDialog( string pageKey, object parameter, string buttonConfirmText, string buttonCancelText, string hostname = null )
        {  
            Action confirm = null, cancel = null;

            string id = Guid.NewGuid().ToString();

            var content = Activator.CreateInstance( cns.GetPageType( pageKey ) );
            var fe = content as FrameworkElement;
            if( fe != null )
            {
                CallNavigation( fe, parameter );

                var interactive = fe.DataContext as IInteractive;

                confirm = async () => 
                { 
                    if( interactive != null )
                    {
                        if( await interactive.Confirm() )
                            host.CloseDialog( id, true );
                    }
                    else
                        host.CloseDialog( id, true );
                };

                cancel = async () => 
                { 
                    if( interactive != null )
                    {
                        if( await interactive.Abort() )
                            host.CloseDialog( id, false );
                    }
                    else
                         host.CloseDialog( id, false );
                };
            }
            
            return await host.ShowDialog( 
                id,
                ( hostname != null ? hostname : DefaultCustomHostname ), 
                content, 
                null, 
                buttonConfirmText, 
                confirm, 
                buttonCancelText, 
                cancel, 
                DialogType.CUSTOM 
            );
        }

        public async Task<bool> ShowCustomDialog( object view, string hostname = null )
        {
            string id = Guid.NewGuid().ToString();

            return await host.ShowDialog( 
                id,
                ( hostname != null ? hostname : DefaultCustomHostname ), 
                view, 
                null, 
                null, 
                null, 
                null, 
                null, 
                DialogType.VIEW 
            );            
        }

        private void CallNavigation( FrameworkElement content, object parameter )
        {
            //se il datacontext implementa l'INavigable interface
            var nav = content.DataContext as INavigable;
            if( nav != null )
            {
                //chiamo il metodo di inizializzazione passandogli i parametri di navigazione
                nav.Initialized( parameter );

                //mi aggancio all'evento di loaded del controllo per chiamate il metodo activated
                RoutedEventHandler loadEH = null, unloadEH = null;
                loadEH = delegate ( object s, RoutedEventArgs ev )
                {
                    nav.Activated( parameter );

                    content.Loaded -= loadEH;
                };

                //mi aggancio all'unloaded del controllo per chiamare il metodo deactivated
                unloadEH = delegate ( object s, RoutedEventArgs ev )
                {
                    nav.Deactivated( parameter ); 

                    content.Loaded -= unloadEH;
                };

                content.Loaded += loadEH;
                content.Unloaded += unloadEH;
            }
        }
    }
}
