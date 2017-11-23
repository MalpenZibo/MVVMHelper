using GalaSoft.MvvmLight.Views;
using MVVMHelper.Helpers;
using MVVMHelper.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MVVMHelper.Services
{
    /// <summary>
    /// Servizio di gestione della navigazione tra le pagine sfruttando i ContentControl
    /// </summary>
    public class ContentNavigationService
	{
        /// <summary>
        /// Evento di notifica della navigazione verso una pagina
        /// </summary>
        public event EventHandler CurrentPageChanged;

        /// <summary>
        /// Modello per una voce di storico di navigazione
        /// </summary>
        public class HistoryItem
        {
            public string pageKey { get; set; }

            public object parameter { get; set; }
        }

        /// <summary>
        /// Dizionario contenente tutte le pagine registrate nel sistema di navigazione
        /// ogni chiave del dizionario corrisponde ad un controllo grafico
        /// </summary>
        private readonly Dictionary<string, Type> pagesByKey = new Dictionary<string, Type>();

        /// <summary>
        /// Dizionario contenente la storia di navigazione
        /// </summary>
        private readonly Dictionary<string, List<HistoryItem>> globalHistory = new Dictionary<string, List<HistoryItem>>();

        /// <summary>
        /// Nome del content control da usare di default
        /// </summary>
        private string defaultContentName = "MainContentControl";

        private string currentPageKey;

        /// <summary>
        /// Costruttore del servizio di navigazione
        /// </summary>
        /// <param name="defaultContentName">Nome del ContentControl da usare di default per ospitare la navigazione tra le pagine</param>
        public ContentNavigationService( string defaultContentName = null )
        {
            if( defaultContentName != null )
            {
                this.defaultContentName = defaultContentName;
            }
        }

        /// <summary>
        /// Attuale chiave della pagina su cui si è navigati
        /// </summary>
        public string CurrentPageKey
        {
            get
            {
                return currentPageKey;
            }
            private set
            {
                if( currentPageKey != value )
                {
                    currentPageKey = value;

                    //se qualcuno è in ascolto dell'evento di navigationchanged lancio l'evento
                    CurrentPageChanged?.Invoke( this, new EventArgs() );
                }
            }
        }

        /// <summary>
        /// Recupera la storia di navigazione di un particolare ContentControl
        /// </summary>
        /// <param name="ContentName">Nome del content control di cui si vuole la storia di navigazione</param>
        /// <returns></returns>
        public ReadOnlyCollection<HistoryItem> GetContentHistory( string ContentName )
        {
            //Controllo se nel dictionary della storia di navigazione esiste una chiave
            //con il nome del contentcontrol specificato
            //nel caso esista ritorno la lista non modificabile
            if( globalHistory.ContainsKey( ContentName ) )
                return globalHistory[ContentName].AsReadOnly();
            else
                return null;
        }

        /// <summary>
        /// Pulisce la storia di navigazione
        /// </summary>
        public void ClearHistory()
        {
            globalHistory.Clear();
        }

        /// <summary>
        /// Torna alla pagina precedente del ContentControl di default
        /// </summary>
        public async Task GoBack()
        {
            await GoBack( 1, defaultContentName );
        }

        public async Task GoBack( int index )
        {
            await GoBack( index, defaultContentName );
        }

        public async Task GoBack( string ContentName )
        {
            await GoBack( 1, defaultContentName );
        }

        /// <summary>
        /// Torna alla pagina precedente del ContentControl specificato
        /// </summary>
        /// <param name="ContentName">Nome del ContentControl</param>
        public async Task GoBack( int index, string ContentName )
        {
            //se ho elementi nella storia di navigazione e se esiste una storia di navigazione
            //per il content control specificato
            if( globalHistory.Count > 0 && globalHistory.ContainsKey( ContentName ) )
            {
                //recupero la storia di navigazione del content control
                List<HistoryItem> lhi = globalHistory[ContentName];
                //se ha più di un elemento
                if( lhi.Count > 1 )
                {
                    var content = GetContent( ContentName );
                    if( content != null )
                    {
                        var interactiveContent = ( content.Content as FrameworkElement ).DataContext as IInteractive;
                        if( interactiveContent != null )
                        {
                            if( !await interactiveContent.Leave() )
                                return;
                        }
                    }             

                    HistoryItem hi = null;
                    while( lhi.Count > 1 && index-- > 0 )
                    {
                        //rimuovo l'ultimo elemento
                        lhi.RemoveAt( lhi.Count - 1 );
                        //prendo l'attuale ultimo (il penultimo in partenza)
                        hi = lhi.Last();
                    }
                    //navigo verso l'elemento estratto dalla storia di navigazione
                    //usando la stessa chiave, e gli stessi parametri usati in precedenza
                    //ovviamente uso il ContentName specificato
                    ExecNavigation( hi.pageKey, hi.parameter, ContentName );
                }              
            }
        }

        /// <summary>
        /// Naviga verso la pagina indicata senza parametri e usando il default content control 
        /// </summary>
        /// <param name="pageKey">chiave della pagina verso cui navigare (deve essere stata in precedenza registrata)</param>
        public async Task NavigateTo( string pageKey )
        {
            await NavigateTo( pageKey, null );
        }

        /// <summary>
        /// Naviga verso la pagina indicata usando il parametro passato e il default content control
        /// </summary>
        /// <param name="pageKey">chiave della pagina verso cui navigare (deve essere stata in precedenza registrata)</param>
        /// <param name="parameter">parametro di navigazione da passare alla pagina</param>
        public async Task NavigateTo( string pageKey, object parameter )
        {
            await NavigateTo( pageKey, parameter, defaultContentName );
        }

        /// <summary>
        /// Naviga verso la pagina indicata usando il parametro passato e il content control specificato
        /// </summary>
        /// <param name="pageKey">chiave della pagina verso cui navigare (deve essere stata in precedenza registrata)</param>
        /// <param name="parameter">parametro di navigazione da passare alla pagina</param>
        /// <param name="ContentName">Content Name da usare per la navigazione</param>
        public async Task NavigateTo( string pageKey, object parameter, string ContentName )
        {
            var content = GetContent( ContentName );
            if( content != null && content.Content != null )
            {
                var interactiveContent = ( content.Content as FrameworkElement ).DataContext as IInteractive;
                if( interactiveContent != null )
                {
                    if( !await interactiveContent.Leave() )
                        return;
                }
            }
            ExecNavigation( pageKey, parameter, ContentName );
        }

        public async Task ChangePage( string pageKey )
        {
            await ChangePage( pageKey, null, null );
        }

        public async Task ChangePage( string pageKey, object parameter )
        {
            await ChangePage( pageKey, parameter, defaultContentName );
        }

        public async Task ChangePage( string pageKey, object parameter, string ContentName )
        {
            if( globalHistory.Count > 0 && globalHistory.ContainsKey( ContentName ) )
            {
                //recupero la storia di navigazione del content control
                List<HistoryItem> lhi = globalHistory[ContentName];
                //se ha più di un elemento
                if( lhi.Count > 1 )
                {
                    //rimuovo l'ultimo elemento
                    lhi.RemoveAt( lhi.Count - 1 );                    
                }
            }

            await NavigateTo( pageKey, parameter, ContentName );
        }

        private void ExecNavigation( string pageKey, object parameter, string ContentName )
        { 
            //locko il dictionary di navigazione
            lock( pagesByKey )
			{
                //se il dictionary della pagine non contiene la pagina specificata lancio un'eccezzione
				if( !pagesByKey.ContainsKey( pageKey ) )
				{
					throw new ArgumentException(
						string.Format(
							"No such page: {0}. Did you forget to call NavigationService.Configure?",
							pageKey 
                        ),
						"pageKey" 
                    );
				}

                //recupero la finestra attualmente attiva
				Window w = Application.Current.Windows.OfType<Window>().FirstOrDefault( x => x.IsActive );
                if( w != null )
                {
                    //se la finestra non è nulla chiamo la funzione che imposterà il contenuto al ContentControl
                    //effettuando di fatto la navigazione
                    SetContent( w, ContentName, pageKey, parameter );
                }                
                else
                {
                    //se la finestra è null vuol dire che non ne ho di attive
                    //recupero quindi la prima finestra dell'applicazione
                    w = Application.Current.Windows.OfType<Window>().FirstOrDefault();

                    //mi aggancio all'evento activated per poter fare la navigazione 
                    //una volta che la finestra sarà attiva
                    EventHandler eh = null;
                    eh = delegate ( object s, EventArgs ev )
                    {
                        //quando la finestra è attiva

                        //chiamo la funzione che imposterà il contenuto al ContentControl
                        //effettuando di fatto la navigazione
                        SetContent( w, ContentName, pageKey, parameter );

                        //mi sgancio dall'evento di attivazione della finestra
                        w.Activated -= eh;
                    };

                    w.Activated += eh;                    
                }			    
			}
        }

        /// <summary>
        /// Imposta il contenuto ad un content control 
        /// </summary>
        /// <param name="w">finestra che conterrà il content control</param>
        /// <param name="ContentName">Nome del content control in cui impostare il contenuto</param>
        /// <param name="pageKey">nome della pagina verso cui navigare</param>
        /// <param name="parameter">parametri da passare alla pagina</param>
        private void SetContent( Window w, string ContentName, string pageKey, object parameter )
        {
            //recupero il content control partendo dalla finestra in uso e cercando il content control tramite il nome passato
            var contentCntrl = TreeHelper.GetDescendantFromName( w.Content as DependencyObject, ContentName ) as ContentControl;

            //se lo trovo
            if( contentCntrl != null )
            {   
                //imposto come contenuto del content control la pagina indicata
                //questa cosa viene fatta recuperando dal dictionary della pagine la pagina richiesta
                //a questo punto usando la classe Activator creo un'istanza del controllo grafico che contiene la pagina
                contentCntrl.Content = Activator.CreateInstance( pagesByKey[pageKey] );

                //se la pagina che ho messo come contenuto del content control
                //è di tipo UserControl
                var fe = contentCntrl.Content as FrameworkElement;
                if( fe != null )
                {
                    //se il datacontext implementa l'INavigable interface
                    var nav = fe.DataContext as INavigable;
                    if( nav != null )
                    {
                        //chiamo il metodo di inizializzazione passandogli i parametri di navigazione
                        nav.Initialized( parameter );

                        //mi aggancio all'evento di loaded del controllo per chiamate il metodo activated
                        RoutedEventHandler loadEH = null, unloadEH = null;
                        loadEH = delegate ( object s, RoutedEventArgs ev )
                        {
                            nav.Activated( parameter );

                            fe.Loaded -= loadEH;
                        };

                        //mi aggancio all'unloaded del controllo per chiamare il metodo deactivated
                        unloadEH = delegate ( object s, RoutedEventArgs ev )
                        {
                            nav.Deactivated( parameter ); 

                            fe.Loaded -= unloadEH;
                        };

                        fe.Loaded += loadEH;
                        fe.Unloaded += unloadEH;
                    }
                }                
                  
                //se il dictionary della storia di navigazione non contiene una voce per il content control usato la credo
                if( !globalHistory.ContainsKey( ContentName ) )
                {
                    globalHistory.Add( ContentName, new List<HistoryItem>() );
                }

                //se l'ultimo elemento della storia di navigazione del content control non è uguale alla pagina verso cui ho appena navigato
                //aggiungo la pagina alla storia di navigazione del content control
                if( globalHistory[ContentName].LastOrDefault()?.pageKey != pageKey )
                    globalHistory[ContentName].Add( new HistoryItem() { pageKey = pageKey, parameter = parameter } );

                //imposto l'attuale pagina con quella verso cui ho appen navigato
                CurrentPageKey = pageKey;                
            }	
        }

        private ContentControl GetContent( string ContentName )
        {
            Window w = Application.Current.Windows.OfType<Window>().FirstOrDefault( x => x.IsActive );

            if( w != null )
                //recupero il content control partendo dalla finestra in uso e cercando il content control tramite il nome passato
                return TreeHelper.GetDescendantFromName( w.Content as DependencyObject, ContentName ) as ContentControl;
            else
                return null;
        }

        /// <summary>
        /// Registra una pagina nel sistema di navigazione
        /// </summary>
        /// <param name="key">chiave della pagina</param>
        /// <param name="contentType">tipo della pagina</param>
        public void Configure( string key, Type contentType )
		{
            //locko il dictionary di navigazione
            lock( pagesByKey )
			{
                //se il dictionary contiene già la chiave lancio un'eccezzione
				if( pagesByKey.ContainsKey( key ) )
				{
					throw new ArgumentException( "This key is already used: " + key );
				}

                //se il dictionary contiene già una pagina dello stesso tipo lancio un'eccezzione
				if( pagesByKey.Any( p => p.Value == contentType ) )
				{
					throw new ArgumentException(
						"This type is already configured with key " + pagesByKey.First( p => p.Value == contentType ).Key );
				}

                //altrimenti aggiungo chiave e tipo al dictionary
				pagesByKey.Add(
					key,
					contentType 
                );
			}
		}
    
        /// <summary>
        /// Restituisce il tipo di una pagina a partire dalla sua chiave
        /// </summary>
        /// <param name="key">chiave di cui si vuole ottenere il tipo</param>
        /// <returns></returns>
        public Type GetPageType( string key )
        {
            //locko il dictionary di navigazione
            lock( pagesByKey )
			{
                //se nel dictionary è presente la chiave
                //ritorno il tipo
				if( pagesByKey.ContainsKey( key ) )
				{
					return pagesByKey[key];
				}
				else
				{
                    //altrimenti eccezzione
					throw new ArgumentException( "Page not found" );
				}
			}
        }
    }
}
