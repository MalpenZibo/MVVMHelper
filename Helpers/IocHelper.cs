using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using MVVMHelper.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMHelper.Helpers
{
    public static class IocHelper
    {
        public static T GetInstance<T>()
        {
            return ( ServiceLocator.Current as SimpleIoc ).GetInstanceWithoutCaching<T>( Guid.NewGuid().ToString() );
        }

        public static WT GetInstance<WT, T>( T model ) 
            where WT : class, IWrapViewModel<T>
            where T : class, INotifyPropertyChanged
        {
            WT vm = ( ServiceLocator.Current as SimpleIoc ).GetInstanceWithoutCaching<WT>( Guid.NewGuid().ToString() );
            vm.Model = model;

            return vm;
        }
    }
}
