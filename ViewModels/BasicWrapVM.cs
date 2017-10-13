using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMHelper.ViewModels
{
    /// <summary>
    /// Basic implementation of a ViewModel that implements IWrapViewModel based on MVVM Light ViewModelBase class
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class BasicWrapVM<TModel> : ViewModelBase, IWrapViewModel<TModel>
        where TModel : class, INotifyPropertyChanged
    {
        private TModel _model;
        /// <summary>
        /// Change to TModel property values in this list will not be propagated to the View
        /// </summary>
        private readonly HashSet<string> _excludedPropertyFromPropagation = new HashSet<string>();

        public TModel Model
        {
            get
            {
                return _model;
            }
            set
            {
                //if model is already set
                if( _model != null )
                {
                    //detach from model propertychanged event
                    _model.PropertyChanged -= Model_PropertyChanged;
                    //clear excluded property propagation list
                    _excludedPropertyFromPropagation.Clear();
                }                    

                //set new TModel instance
                _model = value;
                //attach to model PropertyChanged event
                _model.PropertyChanged += Model_PropertyChanged;
            }
        }

        /// <summary>
        /// Propagate to the view only change made to the model property not present in _excludedPropertyFromPropagation hash
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Model_PropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            if( !_excludedPropertyFromPropagation.Contains( e.PropertyName ) )
                RaisePropertyChanged( e.PropertyName );           
        }

        /// <summary>
        /// Activate propagation to the view for target model property
        /// </summary>
        /// <param name="propertyName">name of the model property to be included to the propagation</param>
        protected void IncludePropertyToPropagation( string propertyName )
        {
            if( _excludedPropertyFromPropagation.Contains( propertyName ) )
                _excludedPropertyFromPropagation.Remove( propertyName );             
        }

        /// <summary>
        /// Deactivate propagation to the view for target model property
        /// </summary>
        /// <param name="propertyName">name of the model property to be excluded from propagation</param>
        protected void ExcludePropertyFromPropagation( string propertyName )
        {
            if( !_excludedPropertyFromPropagation.Contains( propertyName ) )
                _excludedPropertyFromPropagation.Add( propertyName );
        }
    }
}
