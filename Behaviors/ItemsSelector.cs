using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace MVVMHelper.Behaviors
{
    /// <summary>
    /// Questo behavior permette di mappare il click di un oggetto in un controllo di tipo Selector a un ICommand
    /// </summary>
    public class ItemClick : Behavior<Selector>
    {
        //questo flag serve per distinguere la gestione delle datagrid che differisce un po' dal normale
        private bool isAttachedToDataGrid = false;

        /// <summary>
        /// Comando da lanciare in caso di selezione di un oggetto
        /// </summary>
        public static DependencyProperty CommandProperty =
            DependencyProperty.Register( "Command",
                typeof( ICommand ),
                typeof( ItemClick ),
                new UIPropertyMetadata( null ) 
            );

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue( CommandProperty, value ); }
        }

        /// <summary>
        /// Evento di aggancio del behavior
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            //controllo se il controllo a cui è stato agganciato il behavior è una DataGrid per gestire il comportamento diverso
            isAttachedToDataGrid = AssociatedObject.GetType() == typeof( DataGrid );

            //Mi aggancio all'evento di click del mouse sul controllo
            AssociatedObject.MouseLeftButtonUp += MouseLeftButtonUpHandler;
        }

        /// <summary>
        /// Evento di sgancio del behavior
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            //mi sgancio dall'evento di click del mouse sul controllo
            AssociatedObject.MouseLeftButtonUp -= MouseLeftButtonUpHandler;
        }

        /// <summary>
        /// Gestione dell'evento di click del mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseLeftButtonUpHandler( object sender, MouseButtonEventArgs e )
        {      
            object selectedItem = null;
            //se sto operando su una datagrid
            if( isAttachedToDataGrid )
            {
                //recupero il primo controllo di tipo DataGridRow navigando verso l'alto nell'alberatura dello xaml a partire dalla sorgente del click
                //fino al limite massimo del controllo a cui sono agganciato (aka la DataGrid)
                var dgr = Helpers.TreeHelper.GetFirstParentOfType<DataGridRow>( e.OriginalSource as DependencyObject, AssociatedObject );
                //se ho trovato un oggetto di tipo DataGridRow allora il datacontext di quell'oggetto è il nostro elemento e lo salvo
                selectedItem = dgr?.DataContext;
            }
            else
            {
                //negli altri casi recupero un ListBoxItem navigando verso l'alto l'alberatura della pagina a partire dalla sorgente del click
                //fino al limite massimo del controllo a cui sono agganciato
                var lbi = Helpers.TreeHelper.GetFirstParentOfType<ListBoxItem>( e.OriginalSource as DependencyObject, AssociatedObject, true );
                //se ho trovato un oggetto di tipo ListBoxItem allora il datacontext di quell'oggetto è il nostro elemento e lo salvo
                selectedItem = lbi?.DataContext;
            }

            //se ho trovato l'oggetto selezionato chiamao il commando passandoglielo
			if( selectedItem != null )
			{			
				//exec command
				Command?.Execute( selectedItem );
			}
        }
    }
}
