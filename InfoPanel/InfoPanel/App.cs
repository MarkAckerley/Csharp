using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.Attributes;
using UIFramework;
using adWin = Autodesk.Windows;
using Autodesk.Internal.Windows;
using System.Windows.Media;
using System.IO;
using System.Windows;

namespace InfoPanel
{
    
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class App : IExternalApplication
    {
        // class instance 
        public static App ThisApp;

        FormToFrameWork myForm = null;

        public static FormToFrameWork Form
        {
            get;
            set;
        }

        public static DockablePaneId paneId
        {
            get
            {
                return new DockablePaneId(new Guid("9d7ed357-534f-4d87-afc4-8e784e3b116e"));
            }
          
        }

        

        public Result OnStartup(UIControlledApplication a)
        {
            
            FormToFrameWork Form = new FormToFrameWork();


            ThisApp = this;

            a.RegisterDockablePane(paneId, "Info", Form as IDockablePaneProvider);
            

            // Method to add Tab and Panel 
            RibbonPanel panel = RibbonPanel(a);
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
            // BUTTON FOR THE DOCKABLE WPF
            if (panel.AddItem(
                new PushButtonData("InfoPanel", "InfoPanel", thisAssemblyPath,
                    "InfoPanel.EntryCommand")) is PushButton button)
            #region makepanel
            {
                // defines the tooltip displayed when the button is hovered over in Revit's ribbon
                button.ToolTip = "Turn Firm Info On/Off";
                // defines the icon for the button in Revit's ribbon - note the string formatting
                Uri uriImage = new Uri("pack://application:,,,/InfoPanel;component/Resources/code-small.png");
                BitmapImage largeImage = new BitmapImage(uriImage);
                button.LargeImage = largeImage;
            }
            #endregion


            return Result.Succeeded;
        }

        public class FormToFrameWork : IDockablePaneProvider, IFrameworkElementCreator
        {
            NewForm m_MyDockableWindow = null;
            public void SetupDockablePane(Autodesk.Revit.UI.DockablePaneProviderData data)
            {
                data.FrameworkElementCreator = this as IFrameworkElementCreator;
                data.InitialState = new DockablePaneState();
                data.InitialState.MinimumWidth = 300;
                data.VisibleByDefault = false;
                data.InitialState.DockPosition = DockPosition.Tabbed;
                data.InitialState.TabBehind = DockablePanes.BuiltInDockablePanes.ProjectBrowser;
            }

            public FrameworkElement CreateFrameworkElement()
            {
                m_MyDockableWindow = new NewForm();
                return m_MyDockableWindow;
            }
        }



        //adWin.RibbonControl ribbon = adWin.ComponentManager.Ribbon;
        //TabTheme.PanelBackgroundProperty = new SolidColorBrush(Colors.HotPink);


        //foreach (adWin.RibbonTab tab in ribbon.Tabs)
        //{
        //    foreach (adWin.RibbonPanel panelT in tab.Panels)
        //    {
        //        panelT.CustomPanelTitleBarBackground
        //          = new SolidColorBrush(Colors.HotPink);
        //       panelT.CustomPanelBackground = new SolidColorBrush(Colors.HotPink);
        //    }
        //}

        //myForm.Background = new SolidColorBrush(Colors.HotPink);
        // myForm.IsVisible
        //myForm.Foreground = new SolidColorBrush(Colors.HotPink);
        //int op = 50;
        //myForm.Opacity = op;


        // listeners/watchers for external events (if you choose to use them)


        public Result OnShutdown(UIControlledApplication a)
        {
            //Register dockable pane          
            return Result.Succeeded;
        }
        /// <summary>
        /// This is the method which launches the WPF window, and injects any methods that are
        /// wrapped by ExternalEventHandlers. This can be done in a number of different ways, and
        /// implementation will differ based on how the WPF is set up.
        /// </summary>
        /// <param name="uiapp">The Revit UIApplication within the add-in will operate.</param>
        public void ShowForm(ExternalCommandData commandData)
        {
            DockablePane myForm = commandData.Application.GetDockablePane(paneId) as DockablePane;
            // If we do not have a dialog yet, create and show it
            if (myForm != null && myForm == null) return;
            // The dialog becomes the owner responsible for disposing the objects given to it.
            //myForm = new NewForm(evP, evPB);

            myForm.Show();
        }

        /// <summary>
        /// What to do when the application is idling. (Ideally nothing)
        /// </summary>
        void a_Idling(object sender, IdlingEventArgs e)
        {
        }

        /// <summary>
        /// What to do when the application is closing.)
        /// </summary>
        void a_ApplicationClosing(object sender, ApplicationClosingEventArgs e)
        {
        }
        public RibbonPanel RibbonPanel(UIControlledApplication a)
        {
            string tab = "Dyna-Sco"; // Tab name
            // Empty ribbon panel 
            RibbonPanel ribbonPanel = null;
            // Try to create ribbon tab. 
            try
            {
                a.CreateRibbonTab(tab);
            }
            catch (Exception ex)
            {
                Util.HandleError(ex);
            }

            // Try to create ribbon panel.
            try
            {
                RibbonPanel panel = a.CreateRibbonPanel(tab, "Visibility");
            }
            catch (Exception ex)
            {
                Util.HandleError(ex);
            }

            // Search existing tab for your panel.
            List<RibbonPanel> panels = a.GetRibbonPanels(tab);
            foreach (RibbonPanel p in panels.Where(p => p.Name == "Visibility"))
            {
                ribbonPanel = p;
            }

            //return panel 
            return ribbonPanel;
        }
    }
}
