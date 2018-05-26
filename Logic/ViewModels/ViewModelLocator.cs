using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight;
using CommonServiceLocator;

namespace Logic.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<AddFormViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<FillFormViewModel>();

        }

        public MainViewModel Main
        {
            get
            {
                return new MainViewModel(this);
            }
        }

        public AddFormViewModel AddForm
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AddFormViewModel>();
            }
        }

        public FillFormViewModel FillForm
        {
            get
            {
                return new FillFormViewModel();
            }
        }

        public FormsListViewModel FormsList
        {
            get
            {
                return new FormsListViewModel ();
            }
        }

        //public static void Cleanup()
        //{
        //    // TODO Clear the ViewModels
        //}
    }
}
