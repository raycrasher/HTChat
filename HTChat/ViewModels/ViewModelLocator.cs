/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:HTChat"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using System.ComponentModel;
using System;

namespace HTChat.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator: INotifyPropertyChanged
    {

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<ChatClient>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();

            CurrentMainViewModel = SimpleIoc.Default.GetInstance<LoginViewModel>();
            Instance = this;
        }

        internal static void Navigate<T>()
            where T: ViewModelBase
        {
            Instance.CurrentMainViewModel = SimpleIoc.Default.GetInstance<T>();
        }

        public ViewModelBase CurrentMainViewModel
        {
            get; set;
        }
        public static ViewModelLocator Instance { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}