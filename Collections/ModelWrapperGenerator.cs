using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using MVVMHelper.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMHelper.Collections
{
    /// <summary>
    /// Questa classe permette di generare una collezzione di ViewModel a partire da una collezzione di Model
    /// I ViewModel generati saranno di tipo IWrapViewModel e wrapperanno i Model in input
    /// </summary>
    public static class ModelWrapperGenerator
    {
        /// <summary>
        /// Metodo per la generazione della collezione di ViewModel
        /// </summary>
        /// <typeparam name="WT">ViewModel wrapper, DEVE implementare la IWrapViewModel interface</typeparam>
        /// <typeparam name="T">Model che verrà wrappato, DEVE implementare la INotifyPropertyChanged</typeparam>
        /// <param name="data">Collezione di Model da wrappare</param>
        /// <returns></returns>
        public static List<WT> GenerateWrapper<WT, T>( IEnumerable<T> data )
            where WT : class, IWrapViewModel<T>
            where T : class, INotifyPropertyChanged
        {            
            //genero la lista di ViewModel da ritornare
            List<WT> toRet = new List<WT>();

            if( data != null )
            {
                //Per ogni Model presente nella lista in input
                foreach( T model in data )
                {
                    //Genero unìistanza non cachata usando il SimpleIoc della libreria MVVMLight
                    WT wm = ( ServiceLocator.Current as SimpleIoc ).GetInstanceWithoutCaching<WT>( Guid.NewGuid().ToString() );
                    //imposto il Model da wrappare
                    wm.Model = model;
                    //aggiungo il viewmodel alla lista dei viewmodel da ritornare
                    toRet.Add( wm );
                }
            }

            return toRet;
        }
    }
}
