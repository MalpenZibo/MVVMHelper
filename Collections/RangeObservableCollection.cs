using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMHelper.Collections
{
    /// <summary>
    /// ObservableCollection speciale che permette l'inserimento in blocco di oggetti migliorando le performance legate alla notifica della modifica della collezione
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RangeObservableCollection<T> : ObservableCollection<T>
    {
        //fleg che serve per sopprimere la propagazione degli eventi di collection changed
        private bool suppressNotification = false;

        /// <summary>
        /// Sovrascrivo l'evento di collection changed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCollectionChanged( NotifyCollectionChangedEventArgs e )
        {
            //chiamo la gestione dell'evento base solo se non sto volutamente sopprimento la propagazione dell'evento
            if( !suppressNotification )
                base.OnCollectionChanged( e );
        }

        /// <summary>
        /// Metodo per l'aggiunta di un blocco di oggetti
        /// </summary>
        /// <param name="list">lista di oggetti da inserire</param>
        public void AddRange( IEnumerable<T> list )
        {
            //se la lista è nulla eccezzione
            if( list == null )
                throw new ArgumentNullException( "list" );

            //sopprimo la propagazione degli eventi
            suppressNotification = true;

            //Aggiungo alla lista ogni oggetto presente nella lista in input
            foreach( T item in list )
            {
                Add( item );
            }

            //riattivo la propagazione degli eventi
            suppressNotification = false;
            //informo il framework che la lista è da risincronizzare
            OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );
        }
    }
}
