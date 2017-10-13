using MVVMHelper.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace MVVMHelper.Behaviors
{
    public class DynamicLoad : Behavior<ItemsControl>
    {
        private ScrollViewer scrollViewer = null;
        private double lastOffset = -1;

        /// <summary>
        /// Comando da lanciare in caso di selezione di un oggetto
        /// </summary>
        public static DependencyProperty CommandProperty =
                DependencyProperty.Register( "Command",
                    typeof( ICommand ),
                    typeof( DynamicLoad ),
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

            RoutedEventHandler myDelegate = null;
            myDelegate = delegate ( object sender, RoutedEventArgs ev )
            {
                if( AssociatedObject.GetType().Equals( typeof( ListBox ) ) )
                {
                    scrollViewer = TreeHelper.GetFirstChildOfType<ScrollViewer>( AssociatedObject );
                }
                else if( AssociatedObject.GetType().Equals( typeof( DataGrid ) ) )
                {
                    scrollViewer = TreeHelper.GetFirstChildOfType<ScrollViewer>( AssociatedObject );
                }
                else if( AssociatedObject.GetType().Equals( typeof( ItemsControl ) ) )
                {
                    scrollViewer = TreeHelper.GetFirstParentOfType<ScrollViewer>( AssociatedObject );
                }

                if( scrollViewer == null )
                    throw new InvalidOperationException( "This behavior can be attached to item with a scrollViewer." );

                HandleScroll();

                //Mi aggancio all'evento di scroll della scrollViewer
                scrollViewer.ScrollChanged += ScrollChangedHandler;

                AssociatedObject.Loaded -= myDelegate;
            };

            AssociatedObject.Loaded += myDelegate;
        }

        /// <summary>
        /// Evento di sgancio del behavior
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if( scrollViewer != null )
                //mi sgancio dall'evento di scroll della scrollViewer
                scrollViewer.ScrollChanged -= ScrollChangedHandler;
        }

        /// <summary>
        /// Gestione dell'evento di scroll della scrollViewer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollChangedHandler( object sender, ScrollChangedEventArgs e )
        {
            HandleScroll();
        }

        private void HandleScroll()
        {
            bool atBottom = false;
            if( scrollViewer.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled )
            {
                atBottom = lastOffset != scrollViewer.HorizontalOffset && scrollViewer.HorizontalOffset >= scrollViewer.ScrollableWidth;
                lastOffset = scrollViewer.HorizontalOffset;
            }
            else
            {
                atBottom = lastOffset != scrollViewer.VerticalOffset && scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight;
                lastOffset = scrollViewer.VerticalOffset;
            }

            if( atBottom )
            {
                //exec command
                Command?.Execute( null );
            }
        }
    }
}
