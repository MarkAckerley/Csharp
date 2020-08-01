using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.Revit.UI;
using System.Net.Http;
using System.Threading.Tasks;

namespace InfoPanel
{
    public partial class NewForm : Page, IDockablePaneProvider
    {
        public NewForm()
        {
            InitializeComponent();

        }

        public void SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElement = this as FrameworkElement;
            data.InitialState = new DockablePaneState();
            data.InitialState.MinimumWidth = 300;
            data.VisibleByDefault = true;
            data.InitialState.DockPosition = DockPosition.Tabbed;
            data.InitialState.TabBehind = Autodesk.Revit.UI.DockablePanes.BuiltInDockablePanes.ProjectBrowser;
        }



        /// Tab 1 These are the web pages which are not PDFs

        private void Web_Click(object sender, RoutedEventArgs e)
        {
            var bItem = (Button)sender;
            WebP.Navigate(bItem.Tag.ToString());
        }

        /// This is where we set the forward & back buttons for the web pages
        private void BrowseBack_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((WebP != null) && (WebP.CanGoBack));
        }

        private void BrowseBack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            WebP.GoBack();
        }

        private void BrowseForward_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((WebP != null) && (WebP.CanGoForward));
        }

        private void BrowseForward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            WebP.GoForward();
        }

        //Tab2
        private void InfoMenu_Click(object sender, RoutedEventArgs e)
        {
            var mItem = (MenuItem)sender;
            Firm_Info.Navigate(mItem.Tag.ToString());
        }
        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            var bItem = (Button)sender;
            Firm_Info.Navigate(bItem.Tag.ToString());
        }



        // Tab 3
        /// These are all PDFs, if we load PDFs and webpages in the same tabs we lose the scroll point of the PDF

        private void StandardsMenu_Click(object sender, RoutedEventArgs e)
        {
            var mItem = (MenuItem)sender;
            Standards.Navigate(mItem.Tag.ToString());
        }

        private void StandardsBtn_Click(object sender, RoutedEventArgs e)
        {
            var bItem = (Button)sender;
            Standards.Navigate(bItem.Tag.ToString());
        }


        //These catch nasty popups

        void WebBrowser_Navigated(   object sender,   NavigationEventArgs e)
        {
            HideJsScriptErrors((WebBrowser)sender);
        }

        public void HideJsScriptErrors(WebBrowser WebP)
        {
            // IWebBrowser2 interface
            // Exposes methods that are implemented by the 
            // WebBrowser control
            // Searches for the specified field, using the 
            // specified binding constraints.

            FieldInfo fld = typeof(WebBrowser).GetField(
              "_axIWebBrowser2",
              BindingFlags.Instance | BindingFlags.NonPublic);

            if (null != fld)
            {
                object obj = fld.GetValue(WebP);
                if (null != obj)
                {

                    obj.GetType().InvokeMember("Silent",
                      BindingFlags.SetProperty, null, obj,
                      new object[] { true });
                }
            }

        }

    }

}