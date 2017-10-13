using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace MVVMHelper.Behaviors
{
    public class DataGridCustomSort : Behavior<DataGrid>
    {
        public class SortParameter
        {
            public string Path { get; set; }

            public ListSortDirection Direction { get; set; }
        }

        /// <summary>
        /// Comando da lanciare per eseguire l'ordinamento custom
        /// </summary>
        public static DependencyProperty CommandProperty =
            DependencyProperty.Register( "Command",
                typeof( ICommand ),
                typeof( DataGridCustomSort ),
                new UIPropertyMetadata( null )
            );

        public ICommand Command
        {
            get { return (ICommand)GetValue( CommandProperty ); }
            set { SetValue( CommandProperty, value ); }
        }

        /// <summary>
        /// Evento di aggancio del behavior
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            //Mi aggancio all'evento di ordinamento di una colonna della datagrid
            AssociatedObject.Sorting += SortingHandler;
        }

        /// <summary>
        /// Evento di sgancio del behavior
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            //mi sgancio dall'evento di ordinamento di una colonna della datagrid
            AssociatedObject.Sorting -= SortingHandler;
        }

        /// <summary>
        /// Gestione dell'evento di click del mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SortingHandler( object sender, DataGridSortingEventArgs e )
        {            
            SortParameter sp = new SortParameter()
            {
                Path = e.Column.SortMemberPath
            };

            switch( e.Column.SortDirection )
            {
                case ListSortDirection.Ascending:
                    sp.Direction = ListSortDirection.Descending;
                    break;

                case ListSortDirection.Descending:
                    sp.Direction = ListSortDirection.Ascending;
                    break;

                default:
                    sp.Direction = ListSortDirection.Ascending;
                    break;
            }

            //exec command
            Command?.Execute( sp );

            e.Handled = false;
        }
    }
}
