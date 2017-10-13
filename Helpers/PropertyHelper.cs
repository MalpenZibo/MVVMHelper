using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MVVMHelper.Helpers
{
    /// <summary>
    /// Utilità per recuperare tramite reflection il valore di una property di una classe (METODI DI ESTENSIONE)
    /// </summary>
    public static class PropertyHelper
    {
        /// <summary>
        /// Recupera il valore della property con il nome specificato
        /// </summary>
        /// <param name="obj">istanza della classe su cui eseguire la ricerca</param>
        /// <param name="name">nome della proprietà di cui si vuole il valore</param>
        /// <returns>il valore della proprietà</returns>
        public static Object GetPropValue( this Object obj, String name )
        {
            //splitto il nome della proprietà per eseguire ricerche ricorsive
            foreach( String part in name.Split( '.' ) )
            {
                //se l'oggetto passato è null mi fermo
                if( obj == null ) { return null; }

                //recupero il tipo dell'oggetto
                //e recupero le property info con la prima parte del percorso cercato
                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty( part );
                //se la propertyinfo è null ritorno perchè non esiste la property per la classe
                if( info == null ) { return null; }

                //altrimenti ne recupero il valore
                //e faccio diventare questo valore il prossimo di riferimento per continuare la ricerca
                obj = info.GetValue( obj, null );
            }

            //all'uscita dal ciclo ho ricercato tutte le property nel path e ritorno l'ultimo valore recuperato
            return obj;
        }

        /// <summary>
        /// Recupera il valore della property con il nome specificato castandolo al tipo specificato
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetPropValue<T>( this Object obj, String name )
        {
            //richiamo il metodo per recuperare il valore della property senza cast
            Object retval = GetPropValue( obj, name );
            //se ho ritornato null (non trovata) allora ritorno il valore di default del tipo a cui castare
            if( retval == null ) { return default( T ); }

            // ritorno il valore della property castato
            //se i tipi sono incompatibili lancerò un'eccezzione InvalidCastException
            return (T)retval;
        }
    }
}
