using MVVMHelper.Helpers;
using MVVMHelper.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace MVVMHelper.Behaviors
{
    /// <summary>
    /// Questa behavior permette di gestire la selezione di tutti gli elementi di una lista, 
    /// lo status della checkbox cambierà automaticamente in base anche alla singola selezione di ogni oggetto della lista
    /// Il behavior è applicabile alle CheckBox e per funzionare ha bisogno di un ItemSource di oggetti che implementano l'interfaccia ISelectable
    /// E' possibile specificare anche il Path di una property per raggruppare questi oggetti, in tal caso il seelettore opererà solamente sugli oggetti di quel gruppo
    /// </summary>
    public class GlobalSelector : Behavior<CheckBox>
    {
        #region Field

        private bool init = false;
        private bool stopChangeEvent = false;
        private List<ISelectable> elements = new List<ISelectable>();
        private Dictionary<ISelectable, List<INotifyPropertyChanged>> subElementWatch = new Dictionary<ISelectable, List<INotifyPropertyChanged>>();
        private string[] groupSel;

        #endregion

        #region DepProperty

        /// <summary>
        /// ItemsSource sul quale gestire la selezione degli oggetti
        /// </summary>
        public static DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register( "ItemsSource",
                typeof( IEnumerable<ISelectable> ),
                typeof( GlobalSelector ),
                new UIPropertyMetadata( (S,E) => 
                {
                    GlobalSelector gs = S as GlobalSelector;
                    if( gs != null && gs.init )
                        gs.Init();
                } )
            );

        public IEnumerable<ISelectable> ItemsSource
        {
            get { return (IEnumerable<ISelectable>)GetValue(ItemsSourceProperty); }
            set { SetValue( ItemsSourceProperty, value ); }
        }

        /// <summary>
        /// Path per la property sulla quale raggruppare gli oggetti
        /// </summary>
        public static DependencyProperty GroupSelectorProperty =
            DependencyProperty.Register( "GroupSelector",
                typeof( string ),
                typeof( GlobalSelector ),
                new UIPropertyMetadata( (S,E) => 
                {
                    GlobalSelector gs = S as GlobalSelector;
                    if( gs != null && gs.init )
                        gs.Init();
                } )
            );

        public string GroupSelector
        {
            get { return (string)GetValue(GroupSelectorProperty); }
            set { SetValue( GroupSelectorProperty, value ); }
        }

        /// <summary>
        /// Indica su quale valore del selettore del gruppo deve operare la checkbox
        /// Esempio
        ///     Se il group selector è valorizzato sulla Property Name dell'eventuale oggetto passato
        ///     Questa property indica su quale valore operare.
        ///     Ad esempio se questa property è settata su "Ciao" allora il behavior gestirà tutti gli oggetti
        ///     dell'ItemSource che hanno la property Name valorizzata con "Ciao"
        /// </summary>
        public static DependencyProperty GroupProperty =
            DependencyProperty.Register( "Group",
                typeof( object ),
                typeof( GlobalSelector ),
                new UIPropertyMetadata( (S,E) => 
                {
                    GlobalSelector gs = S as GlobalSelector;
                    if( gs != null && gs.init )
                        gs.Init();
                } ) 
            );

        public object Group
        {
            get { return (object)GetValue(GroupProperty); }
            set { SetValue( GroupProperty, value ); }
        }

        #endregion

        /// <summary>
        /// Evento di aggancio del behavior
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            Init();
        }

        /// <summary>
        /// Funzione di inizializzazione del behavior
        /// </summary>
        private void Init()
        {
            //se l'item source non è nullo
            if( ItemsSource != null )
            {
                //rimuovo ogni elemento dalla lista locale degli elementi gestiti
                for( int i = elements.Count - 1; i >= 0; i-- )
                    removeItem( elements[i] );

                //se l'itemsource implementa l' INotifyCollectionChanged
                INotifyCollectionChanged iNot = ItemsSource as INotifyCollectionChanged;
                if( iNot != null )
                    //Mi aggancio all'evento di collection changed per gestire l'aggiunta o la rimozione di elementi
                    iNot.CollectionChanged += Mod_CollectionChanged;

                //splitto il path per recuperare l'eventuale proprietà di raggruppamento
                groupSel = GroupSelector?.Split( '.' );

                //Per ogni oggetto di tipo ISelectable dell'itemssource
                //aggiunto l'elemento alla lista locale di oggetti da gestire
                foreach( ISelectable item in ItemsSource )
                {
                    addItem( item );
                }

                //mi aggancio all'evento di check della checkbox e all'evento di uncheck della checkbox
                AssociatedObject.Checked += CheckedHandler;
                AssociatedObject.Unchecked += UncheckedHandler;
                
                //Resync global selected flag
                ResyncSelectionFlag();
            }

            init = true;
        }
        
        /// <summary>
        /// Evento di sgancio del behavior
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            //mi stacco dagli eventi di check e uncheck della checkbox
            AssociatedObject.Checked -= CheckedHandler;
            AssociatedObject.Unchecked -= UncheckedHandler;
        }

        /// <summary>
        /// Metodo per l'aggiunta nella lista locale degli elementi da gestire dal behavior
        /// </summary>
        /// <param name="item"></param>
        private void addItem( ISelectable item )
        {
            //se l'oggetto non è nullo e non è contenuto della lista locale
            if( item != null && !elements.Contains( item ) )
            {
                //se il patch per il selettore del gruppo non è nullo
                if( groupSel != null )
                {
                    //un po' di reflection per recuperare le property che decide di quale gruppo farà parte l'oggetto
                    //per far funzionare la property changed su quella particolare proprietà dovremo agganciarci ad ogni
                    //istanza degli oggetti nel path
                    //
                    //  esempio:  Model1.Model2.Model3.Name
                    //              Name è la property sulla base della quale viene gestito il raggruppamento degli oggetti
                    //              per poter però sapere se il valore è cambiato dovrò agganciarmi al property changed degli oggetti
                    //              Model1, Model2, Model3

                    Type type;
                    PropertyInfo info;
                    object obj = item;
                    List<INotifyPropertyChanged> objs = new List<INotifyPropertyChanged>();
                    //per ogni elemento diviso dal . nel Path del gruppo ad esclusione dell'ultimo che verrà gestito dopo il ciclo
                    for( int i = 0; i < groupSel.Length - 1; i++ )
                    {
                        //a partire dall'oggetto da aggiungere recupero il tipo
                        type = obj.GetType();
                        //dal tipo recupero la property usando il primo delemento del path di selezione del gruppo
                        //nell'esempio sarebbe Model1
                        info = type.GetProperty( groupSel[i] );    
                    
                        //dalle info sulla Property recupero il valore della property
                        //la assengo a obj quindi dal prossimo giro il riferimento sarà il valore di questa peroperty
                        obj = info.GetValue( obj, null );
                        //se l'oggetto della property implementa l'INotifyPropertyChanged
                        //aggiungo questo oggetto alla lista di oggetti u cui registrare l'evento
                        if( obj as INotifyPropertyChanged != null )
                            objs.Add( obj as INotifyPropertyChanged );
                    }

                    //gestione dell'ultimo elemento del Path
                    //nell'esempio Name

                    //recupero il tipo dell'ultimo oggetto recuperato nel ciclo
                    //nell'esempio Model3
                    type = obj.GetType();
                    //recupero le info sulla proprietà indicata dall'ultima voce del Path
                    //nell'esempio Name
                    info = type.GetProperty( groupSel.Last() );
               
                    //recupero il valore della property
                    var groupValue = info.GetValue( obj, null );

                    //se il valore della property equivale al valore del Gruppo che devo gestire
                    if( groupValue.Equals( Group ) )
                    {
                        //aggiungo l'elemento alla lista locale degli elementi che devo gestire
                        elements.Add( item );

                        //mi aggancio al property changed della proprietà 
                        foreach( INotifyPropertyChanged o in objs )
                            o.PropertyChanged += item_subPropertyChanged;                        
                    
                        //aggiungo una chiave nel dictionary che contiene la lista degli oggetti a cui ho agganciato l'evento property changed
                        //per l'oggetto passato
                        subElementWatch.Add( item, objs );

                        //mi aggancio anche al property chagned dell'oggetto passato
                        ( item as INotifyPropertyChanged ).PropertyChanged
                            += new PropertyChangedEventHandler( item_PropertyChanged );
                    }
                }
                else
                {
                    //se non ho la selezione dei gruppi da gestire
                    //aggiungo l'oggetto passato alla lista degli oggetti da gestire
                    elements.Add( item );
                    //mi aggancio al suo property changed
                    ( item as INotifyPropertyChanged ).PropertyChanged
                        += new PropertyChangedEventHandler( item_PropertyChanged );
                }
            }            
        }

        /// <summary>
        /// Metodo per la rimozione dalla lista locale degli elementi gestiti dal behavior
        /// </summary>
        /// <param name="item"></param>
        private void removeItem( ISelectable item )
        {
            //se l'oggetto non è nullo e la collezzione lo contiene
            if( item != null && elements.Contains( item ) )
            {
                //Rimuovo l'ascolto al property changed dell'elemento
                ( item as INotifyPropertyChanged ).PropertyChanged 
                    -= new PropertyChangedEventHandler( item_PropertyChanged );                

                //se l'oggetto ha una chiave nel dictionary dei sottoelementi da controllare
                if( subElementWatch.ContainsKey( item ) )
                {
                    //per ogni sottoelemento dell'oggetto da rimuovere
                    //mi sgancio dell'evento propertychanged
                    foreach( INotifyPropertyChanged o in subElementWatch[item] )
                    {
                        o.PropertyChanged -= item_subPropertyChanged;
                    }
                }

                //rimuovo la voce nel dictionary per questo elemento
                subElementWatch.Remove( item );
                //rimuovo l'elemento dalla lista di oggetti da gestire
                elements.Remove( item );
            }
        }

        /// <summary>
        /// Gestione del check della checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckedHandler( object sender, RoutedEventArgs e )
        {      
            //Blocco la propagazione degli eventi di propertychanged
            stopChangeEvent = true;

            //per ogni oggetto contenuto nella lista locale degli oggetti da gestire
            //setto la property IsSelected a true
            foreach( var i in elements )
            { 
                i.IsSelected = true;
            }

            //riabilito gli eventi di propertychanged
            stopChangeEvent = false;
        }

        /// <summary>
        /// Gestione dell'uncheck della checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UncheckedHandler( object sender, RoutedEventArgs e )
        {
            //Blocco la propagazione degli eventi di propertychanged
            stopChangeEvent = true;

            //per ogni oggetto contenuto nella lista locale degli oggetti da gestire
            //setto la property IsSelected a false
            foreach( var i in elements )
            {
                i.IsSelected = false;
            }

            //riabilito gli eventi di propertychanged
            stopChangeEvent = false;
        }

        /// <summary>
        /// Gestione dell'aggiunta o rimozione di un elemento dell'itemsource
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mod_CollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            //Se ci sono nuovi elementi
            if( e.NewItems != null )
            {
                //Aggiungo ogni nuovo elemento alla lista degli oggetti gestiti localmente
                foreach( object item in e.NewItems )
                {
                    addItem( item as ISelectable );
                }
            }
 
            //Se ci sono vecchi elementi
            if( e.OldItems != null )
            {
                //rimuovo ogni vecchio elemento dalla lista degli oggetti gestiti localmente
                foreach( object item in e.OldItems )
                {
                    removeItem( item as ISelectable );
                }
            }

            //risincronizzo il check delle checkbox
            ResyncSelectionFlag();
        }

        /// <summary>
        /// Gestione del property changed di un oggetto primario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void item_PropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            //Se non sono stati bloccati gli eventi di change
            if( !stopChangeEvent )
            {
                //risincronizzo il check delle checkbox
                ResyncSelectionFlag();
            }
        }

        /// <summary>
        /// Gestione del property changed di un oggetto secondario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void item_subPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            //per ogni elemento presente del dictionary degli oggetti secondari
            foreach( KeyValuePair<ISelectable, List<INotifyPropertyChanged>> entry in subElementWatch )
            {
                //cerco l'oggetto che ha scatenato questo evento all'interno delle liste di ogni
                //chiave del dictionary
                if( entry.Value.Contains( sender ) )
                {
                    //quando lo trovo 
                    //elimino l'oggetto principale di riferimento da quelli gestiti localmente
                    removeItem( entry.Key );
                    //e lo riaggiungo
                    addItem( entry.Key );
                }
            }
        }
        
        /// <summary>
        /// Risincronizza il check della checkbox
        /// </summary>
        private void ResyncSelectionFlag()
        {
            try
            { 
                //Flag che indica se tutti gli oggetti gestiti localmente sono selezionati
                bool AllSelected = 
                    ( 
                        from a in elements 
                        where !a.IsSelected
                        select a 
                    ).Count() == 0;

                //Flag che indica se tutti gli oggetti gestiti localmente sono DEselezionati
                bool AllUnSelected = 
                    ( 
                        from a in elements
                        where a.IsSelected
                        select a 
                    ).Count() == 0;

                //Se tutti gli oggetti gestiti localmente sono selezionati setto la checkbox a true
                if( AllSelected && ( AssociatedObject.IsChecked == null || !(bool)AssociatedObject.IsChecked ) )
                    AssociatedObject.IsChecked = true;
                //altrimenti se sono tutti DEselezionati setto la checkbox a false
                else if( AllUnSelected && ( AssociatedObject.IsChecked == null || (bool)AssociatedObject.IsChecked ) )
                    AssociatedObject.IsChecked = false;
                //altrimenti se un po' sono selezionati e altri DEselezionati setto la checkbox a null
                else if( AssociatedObject.IsChecked != null && !AllSelected && !AllUnSelected )
                    AssociatedObject.IsChecked = null;
            }
            catch( Exception ex )
            {
                throw new Exception( "item in items source do not contain the Property " );
            }
        }
    }
}
