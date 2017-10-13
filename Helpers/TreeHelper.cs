using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MVVMHelper.Helpers
{
    /// <summary>
    /// Utility generale per l'attraversamento dell'albero dello xaml
    /// </summary>
    public static class TreeHelper
    {
        /// <summary>
        /// Recupera un controllo con il nome specificato scendendo nell'alberatura a partire dal controllo radice passato
        /// </summary>
        /// <param name="parent">controllo radice da cui iniziare la ricerca</param>
        /// <param name="name">nome del controllo da trovare</param>
        /// <returns></returns>
        public static FrameworkElement GetDescendantFromName( DependencyObject parent, string name )
		{
            //per prima cosa controllo che il controllo passato sia un framework element 
            //e controllo se per caso abbia il nome del controllo che cerco
            var frameworkElement = parent as FrameworkElement; 
            if( frameworkElement != null && frameworkElement.Name == name )
			    return frameworkElement;

            //recupero il numero di controlli figli del controllo radice passato
			var count = VisualTreeHelper.GetChildrenCount( parent );

            //se il conteggio è minore di 1 ritorno null. Impossibile trovare il controllo con il nome specificato
			if( count < 1 )
			{
				return null;
			}

            //altrimenti ciclo su tutti i controlli figlio
			for( var i = 0; i < count; i++ )
			{
                //recupero il controllo figlio nell'indice specificato
				frameworkElement = VisualTreeHelper.GetChild( parent, i ) as FrameworkElement;
                //se è un famework element
				if( frameworkElement != null )
				{
                    //controllo che abbia il nome che cerco e nel caso lo ritorno
					if( frameworkElement.Name == name )
					{
						return frameworkElement;
					}

                    //chiamo ricorsivamente questa funzione per analizzare tutti i rami figlio d
                    //del controllo preso in esame
                    //Praticamente questa nuova ricerca avrà sempre da cercare lo stesso nome
                    //ma avrà come controllo radice quello preso in considerazione ora
					frameworkElement = GetDescendantFromName( frameworkElement, name );
                    //se il controllo ritornato dalla chiamata ricorsiva non è nullo
					if( frameworkElement != null )
					{
                        //controllo trovato lo ritorno
						return frameworkElement;
					}
				}
			}

            //se non ho trovato nulla ritorno null
            //controllo non trovato
			return null;
		}

        /// <summary>
        /// Recupera il primo controllo del tipo specificato scendendo l'alberatura dello xaml a partire dal controllo radice passato
        /// </summary>
        /// <typeparam name="T">Tipo di controllo cercato</typeparam>
        /// <param name="dependencyObject">Controllo radice da cui far partire la ricerca</param>
        /// <returns></returns>
        public static T GetFirstChildOfType<T>( DependencyObject dependencyObject ) where T : DependencyObject
        {
            //se il controllo radice passato è null ritorno null ovvero controllo non trovato
            if( dependencyObject == null )
            {
                return null;
            }

            //per ogni figlio del controllo radice passato
            for( var i = 0; i < VisualTreeHelper.GetChildrenCount( dependencyObject ); i++ )
            {
                //recupero il figlio nell'indice ricercato
                var child = VisualTreeHelper.GetChild( dependencyObject, i );

                //tento un cast del controllo figlio nel tipo voluto
                //se il cast funziona e non ritorna null allora result avrà il valore del controllo castato al tipo cercato
                //altrimenti richiamo ricorsivamente questa funzione passando come controllo di radice quello attualmente analizzato
                var result = (child as T) ?? GetFirstChildOfType<T>( child );

                //controllo che result non sia null
                //(la chiamata ricorsiva può sempre ritornare null)
                if( result != null )
                {
                    //se non è null ritorno il controllo cercato
                    //altrimenti vado avanti con gli altri figli
                    return result;
                }
            }

            //se esco dal ciclo senza aver ritornato nulla vuol dire che il controllo non è presente
            //ritorno null
            return null;
        }

        /// <summary>
        /// Recupera il primo parent del tipo specificato salendo nell'alberatura dello xaml a partire dal controllo di partenza passato
        /// </summary>
        /// <typeparam name="T">Tipo di controllo cercato</typeparam>
        /// <param name="dependencyObject">controllo di partenza</param>
        /// <returns></returns>
        public static T GetFirstParentOfType<T>( DependencyObject dependencyObject ) where T : DependencyObject
        {
            return GetFirstParentOfType<T>( dependencyObject, null );
        }

        /// <summary>
        /// Recupera il primo controllo del tipo specificato scendendo l'alberatura dello xaml a partire dal controllo radice passato 
        /// e salire oltre il controllo di limite passato
        /// </summary>
        /// <typeparam name="T">Tipo di controllo cercato</typeparam>
        /// <param name="dependencyObject">controllo di partenza</param>
        /// <param name="dependencyObjectLimit">controllo limite oltre il quale smettere la ricerca</param>
        /// <returns></returns>
		public static T GetFirstParentOfType<T>( 
            DependencyObject dependencyObject, 
            DependencyObject dependencyObjectLimit
        ) where T : DependencyObject
        {
            return GetFirstParentOfType<T>( dependencyObject, null, false );
        }

        /// <summary>
        /// Recupera il primo controllo del tipo specificato scendendo l'alberatura dello xaml a partire dal controllo radice passato 
        /// e salire oltre il controllo di limite passato
        /// </summary>
        /// <typeparam name="T">Tipo di controllo cercato</typeparam>
        /// <param name="dependencyObject">controllo di partenza</param>
        /// <param name="dependencyObjectLimit">controllo limite oltre il quale smettere la ricerca</param>
        /// <param name="includeSubClass">flag per includere le sottoclassi nel match del tipo</param>
        /// <returns></returns>
        public static T GetFirstParentOfType<T>( 
            DependencyObject dependencyObject, 
            DependencyObject dependencyObjectLimit ,
			bool includeSubClass
        ) where T : DependencyObject
        {
            //se il controllo di partenza passato è nullo allora ritorno null
            if( dependencyObject == null )
            {
                return null;
            }

            //fisso come riferimento il controllo passato
            DependencyObject parent = dependencyObject;

            //versione non ricorsiva dell'algoritmo
            //fino a quando il riferimento non è null e ( o il controllo limite è null oppure il controllo di riferimento è diverso dal controllo limite )
            while( parent != null && ( dependencyObjectLimit == null || parent != dependencyObjectLimit ) )
            {               
                //casto il controllo di rifetimento al tipo cercato
                var result = parent as T;

                //se il risultato è diverso da null
                //OPPURE
                //se sto cercando anceh le sottoclassi e il controllo di riferimento è una sottoclasse del tipo cercato
                if( result != null || ( includeSubClass && parent.GetType().IsSubclassOf( typeof( T ) ) ) )
                    //ritorno il controllo di riferimento
                    return result;

                //se il controllo di riferimento non è null
                if( parent != null )
                    //recupero il padre del controllo di riferimento
                    //e lo setto come nuovo controllo di riferimento
                    parent = VisualTreeHelper.GetParent( parent );
            }

            //se esco dal ciclo vuol dire che non ho trovato ciò che cercavo
            //ritno null
            return null;
        }
    }
}
