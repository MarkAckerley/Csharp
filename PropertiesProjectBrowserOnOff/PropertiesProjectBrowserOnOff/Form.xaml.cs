using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.Revit.UI;

namespace PropertiesProjectBrowserOnOff
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class NewForm : Page, IDockablePaneProvider
    {

        private readonly EventHandlerMethodPropertiesOnOff _mExternalMethodPropertiesOnOff;
        private readonly EventHandlerMethodProjectBrowserOnOff _ExternalMethodProjectBrowserOnOff;

        public void SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElement = this as FrameworkElement;
            data.InitialState = new DockablePaneState();
            data.InitialState.MinimumWidth = 20;
            data.InitialState.DockPosition = DockPosition.Right;
            data.VisibleByDefault = true;
        }

        public NewForm(EventHandlerMethodPropertiesOnOff eExternalMethodPropertiesOnOff, EventHandlerMethodProjectBrowserOnOff eExternalMethodProjectBrowserOnOff)
        {
            InitializeComponent();
            _mExternalMethodPropertiesOnOff = eExternalMethodPropertiesOnOff;
            _ExternalMethodProjectBrowserOnOff = eExternalMethodProjectBrowserOnOff;
        }

        private void BExternalMethodPropertiesOnOff_Click(object sender, RoutedEventArgs e)
        {
            // Raise external event with this UI instance (WPF) as an argument
            _mExternalMethodPropertiesOnOff.Raise(this);
        }
        private void BExternalMethodProjectBrowserOnOff_Click(object sender, RoutedEventArgs e)
        {
            // Raise external event with this UI instance (WPF) as an argument
            _ExternalMethodProjectBrowserOnOff.Raise(this);
        }
    }
}