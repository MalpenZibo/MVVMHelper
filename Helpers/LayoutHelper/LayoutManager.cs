using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace MVVMHelper.Helpers.LayoutHelper
{
    /// <summary>
    /// Gestore del layout per creare interfacce e applicazioni adattive
    /// </summary>
    public class LayoutManager : ObservableObject
    {
        #region Filed

        private double minWidthTablet = 720;
        private double minWidthNotebook = 1200;
        private double minWidthDesktop = 1920;
        private LayoutType currentLayout = LayoutType.Unknown;
        private Window targetWindow;

        #endregion 

        public delegate void LayoutTypeChangedHandler( object sender, LayoutTypeChangedEventArgs e );

        /// <summary>
        /// Evento di layout change
        /// </summary>
        public event LayoutTypeChangedHandler LayoutTypeChanged;

        #region Property

        /// <summary>
        /// Larghezza minima della finestra per rientrare nel layout da Tablet
        /// </summary>
        public double MinWidthTablet
        {
            get { return minWidthTablet; }
            set { minWidthTablet = value; }
        }

        /// <summary>
        /// Larghezza minima della finestra per rientrare nel layout da Notebook
        /// </summary>
        public double MinWidthNotebook
        {
            get { return minWidthNotebook; }
            set { minWidthNotebook = value; }
        }

        /// <summary>
        /// Larghezza minima della finestra per rientrare nel layout da Desktop
        /// </summary>
        public double MinWidthDesktop
        {
            get { return minWidthDesktop; }
            set { minWidthDesktop = value; }
        }

        /// <summary>
        /// Indica se il layout è impostato in modalità telefono
        /// </summary>
        public bool IsInPhoneMode
        {
            get
            {
                return LayoutType.Phone == RetrieveLayoutType();
            }
        }

        /// <summary>
        /// Indica se il layout è impostato in modalità tablet
        /// </summary>
        public bool IsInTabletMode
        {
            get
            {
                return LayoutType.Tablet == RetrieveLayoutType();
            }
        }

        /// <summary>
        /// Indica se il layout è impostato in modalità notebook
        /// </summary>
        public bool IsInNotebookMode
        {
            get
            {
                return LayoutType.Notebook == RetrieveLayoutType();
            }
        }

        /// <summary>
        /// Indica se il layout è impostato in modalità desktop
        /// </summary>
        public bool IsInDesktopMode
        {
            get
            {
                return LayoutType.Desktop == RetrieveLayoutType();
            }
        }

        /// <summary>
        /// Indica se l'attuale layout è più piccolo del layout per tablet
        /// </summary>
        public bool IsSmallThanTablet
        {
            get
            {
                LayoutType newLT = RetrieveLayoutType();
                return ( newLT == LayoutType.Phone );
            }
        }

        /// <summary>
        /// Indica se l'attuale layout è più piccolo del layout per notebook
        /// </summary>
        public bool IsSmallThanNotebook
        {
            get
            {
                LayoutType newLT = RetrieveLayoutType();
                return ( newLT == LayoutType.Phone || newLT == LayoutType.Tablet );
            }
        }

        /// <summary>
        /// Indica se l'attuale layout è più piccolo del layout per desktop
        /// </summary>
        public bool IsSmallThanDesktop
        {
            get
            {
                LayoutType newLT = RetrieveLayoutType();
                return ( newLT == LayoutType.Phone || newLT == LayoutType.Tablet || newLT == LayoutType.Notebook );
            }
        }

        /// <summary>
        /// Ritorna l'attuale layout impostato
        /// </summary>
        public LayoutType CurrentLayout
        {
            get
            {
                return currentLayout;
            }
            private set
            {
                //se cambia il layot faccio scattare tutti i propertychanged
                currentLayout = value;
                RaisePropertyChanged( "CurrentLayout" );
                RaisePropertyChanged( "IsInPhoneMode" );
                RaisePropertyChanged( "IsInTabletMode" );
                RaisePropertyChanged( "IsInNotebookMode" );
                RaisePropertyChanged( "IsInDesktopMode" );
                RaisePropertyChanged( "IsSmallThanTablet" );
                RaisePropertyChanged( "IsSmallThanNotebook" );
                RaisePropertyChanged( "IsSmallThanDesktop" );
            }
        }

        /// <summary>
        /// Ritorna la finestra per la quale si sta gestendo il layout
        /// </summary>
        public Window TargetWindow
        {
            get
            {
                //se la finestra è nulla vuol dire che non mi sono ancora registrato alla finestra
                if( targetWindow == null )
                {
                    //recupero la prima finestra dell'applicazione
                    TargetWindow = Application.Current.Windows.OfType<Window>().First();
                }
                else
                {
                    //se la finestra non è nulla
                    //recupera la prima finestra attiva dell'applicazione
                    Window w = Application.Current.Windows.OfType<Window>().SingleOrDefault( x => x.IsActive );
                    //se la finestra recuperata non è nulla ed è diversa dalla vecchia salvata in memoria la sostituisco
                    if( w != null && w != targetWindow )
                    {
                        TargetWindow = w;
                    }
                }

                //ritorno la finestra trovata
                return targetWindow;
            }
            set
            {
                //se la finestra da settare è diversa da quelal che già gestisco
                if( targetWindow != value )
                {
                    //se vecchia finestra non è nulla mi sgancio dall'evento di size change
                    if( targetWindow != null )
                    {
                        targetWindow.SizeChanged -= VisibleBoundsChangedHandler;
                    }

                    //setto la nuova finestra e mi aggancio all'evento di size change
                    targetWindow = value;
                    targetWindow.SizeChanged += VisibleBoundsChangedHandler;
                }

            }
        }

        #endregion

        #region Function

        /// <summary>
        /// Gestisce il resize della finestra per cui gestire il layout
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VisibleBoundsChangedHandler( object sender, object e )
        {
            //recupero il nuovo layout da applicare
            LayoutType newLayout = RetrieveLayoutType();

            //se il nuovo layout è diverso da quello attuale
            if( newLayout != CurrentLayout )
            {
                //setto il nuovo layout
                CurrentLayout = newLayout;

                //se qualcuno è agganciato all'evento di layoutchanged gli notifico il cambiamento
                if( LayoutTypeChanged != null )
                    LayoutTypeChanged.Invoke( this, new LayoutTypeChangedEventArgs( newLayout ) );
            }
        }

        /// <summary>
        /// Recupero il layout che si dovrebbe usare a partire dalle dimensioni della finestra presa in esame
        /// </summary>
        /// <returns></returns>
        private LayoutType RetrieveLayoutType()
        {
            //setto come layout il layout sconosciuto
            LayoutType toRet = LayoutType.Unknown;

            //se la finestra di riferimento non è nulla
            if( TargetWindow != null )
            {
                //se la larghezza della finestra è minore della larghezza minima per il layout da tablet
                //setto il layout da telefono
                if( targetWindow.ActualWidth < MinWidthTablet )
                    toRet = LayoutType.Phone;

                //come sopra ma per il tablet
                else if( targetWindow.ActualWidth < MinWidthNotebook )
                    toRet = LayoutType.Tablet;

                //come sopra ma per il notebook
                else if( targetWindow.ActualWidth < MinWidthDesktop )
                    toRet = LayoutType.Notebook;

                //come sopra ma per il desktop
                else if( targetWindow.ActualWidth >= MinWidthDesktop )
                    toRet = LayoutType.Desktop;
            }

            return toRet;
        }

        #endregion
    }
}
