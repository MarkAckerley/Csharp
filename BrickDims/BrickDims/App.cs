using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.DB;
using adwin = Autodesk.Windows;
using BrickDims;

namespace BrickDims
{
    /// <summary>
    /// This is the main class which defines the Application, and inherits from Revit's
    /// IExternalApplication class.
    /// </summary>
    /// 

    class App : IExternalApplication
    {
        // class instance
        public static App ThisApp;

        // ModelessForm instance
        private BrickDimSelector _mMyForm;


        public Result OnStartup(UIControlledApplication a)
        {
            _mMyForm = null; // no dialog needed yet; the command will bring it
            ThisApp = this; // static access to this application instance

            BrickDimUpdater updater = new BrickDimUpdater(a.ActiveAddInId);
            Debug.Print("register");
            UpdaterRegistry.RegisterUpdater(updater);
            ElementCategoryFilter f = new ElementCategoryFilter(BuiltInCategory.OST_Dimensions);
            Debug.Print("filter");
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), f, Element.GetChangeTypeAny());


            // Method to add Tab and Panel 
            RibbonPanel panel = RibbonPanel(a);

            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            // BUTTON FOR COLOURING DIMENSIONS
            if (panel.AddItem(
                new PushButtonData("Colour", "Colour\nIn View", thisAssemblyPath, 
                    "BrickDims.BrickDimsColourOverride")) is PushButton button2)
            {
                button2.ToolTip = "Visual interface for debugging applications.";
                Uri uriImageB2 = new Uri("pack://application:,,,/BrickDims;component/Resources/brick-Rainbow.png");
                BitmapImage largeImageB2 = new BitmapImage(uriImageB2);
                button2.LargeImage = largeImageB2;
            }

            //panel.AddSeparator();

            /// <summary>
            /// Control buttons for the Dynamic Model Update 
            /// </summary>
            ToggleButtonData toggleButtonData3
              = new ToggleButtonData("UpdaterStoptxtOff", "Stop\nAuto", thisAssemblyPath, "BrickDims.UIDynamicModelUpdateOff");
            Uri uriImageTB3 = new Uri("pack://application:,,,/BrickDims;component/Resources/stop-big.png");
            BitmapImage largeImageTB3 = new BitmapImage(uriImageTB3);
            toggleButtonData3.LargeImage = largeImageTB3;


            ToggleButtonData toggleButtonData4
              = new ToggleButtonData("UpdaterPlaytxtOff", "Start\nAuto", thisAssemblyPath, "BrickDims.UIDynamicModelUpdateOn");
            Uri uriImageTB4 = new Uri("pack://application:,,,/BrickDims;component/Resources/play-big.png");
            BitmapImage largeImageTB4 = new BitmapImage(uriImageTB4);
            toggleButtonData4.LargeImage = largeImageTB4;

            //TextBoxData brkDimTxt = new TextBoxData("Pause Play Updater");
            
            //panel.AddItem(toggleButtonData3);
            //panel.AddItem(toggleButtonData4);

            // make dyn update on/off radio button group 

            RadioButtonGroupData radioBtnGroupData2 = new RadioButtonGroupData("Updater");

            //IList<RibbonItem> stackedGroup1 = panel.AddStackedItems(toggleButtonData4, toggleButtonData3);
            //as RadioButtonGroup;
            RadioButtonGroup radioBtnGroup2 = panel.AddItem(radioBtnGroupData2) as RadioButtonGroup;

            radioBtnGroup2.AddItem(toggleButtonData3);
            radioBtnGroup2.AddItem(toggleButtonData4);


            // listeners/watchers for external events (if you choose to use them)
            a.ApplicationClosing += a_ApplicationClosing; //Set Application to Idling
            a.Idling += a_Idling;

            //turn off text in buttons where we want it
            setButtonNoText();

            panel.AddSeparator();

            // BUTTON FOR THE SINGLE-THREADED WPF OPTION
            if (panel.AddItem(
                new PushButtonData("Brick Dim Selector", "Selector", thisAssemblyPath,
                    "BrickDims.EntryCommand")) is PushButton button)
            {
                // defines the tooltip displayed when the button is hovered over in Revit's ribbon
                button.ToolTip = "Visual interface for debugging applications.";
                // defines the icon for the button in Revit's ribbon - note the string formatting
                Uri uriImageB1 = new Uri("pack://application:,,,/BrickDims;component/Resources/brick-Small.png");
                BitmapImage largeImageB1 = new BitmapImage(uriImageB1);
                button.LargeImage = largeImageB1;
            }

            return Result.Succeeded;
        }

        /// <summary>
        /// What to do when the application is shut down.
        /// </summary>
        public Result OnShutdown(UIControlledApplication a)
        {
            BrickDimUpdater updater = new BrickDimUpdater(a.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
            Debug.Print("shut down");
            return Result.Succeeded;
        }

        /// <summary>
        /// This is the method which launches the WPF window, and injects any methods that are
        /// wrapped by ExternalEventHandlers. This can be done in a number of different ways, and
        /// implementation will differ based on how the WPF is set up.
        /// </summary>
        /// <param name="uiapp">The Revit UIApplication within the add-in will operate.</param>
        public void ShowForm(UIApplication uiapp)
        {
            // If we do not have a dialog yet, create and show it
            if (_mMyForm != null && _mMyForm == null) return;
            //EXTERNAL EVENTS WITH ARGUMENTS
            EventHandlerWithWpfArg evWpf = new EventHandlerWithWpfArg();

            _mMyForm = new BrickDimSelector(uiapp, evWpf);

            HwndSource hwndSource = HwndSource.FromHwnd(uiapp.MainWindowHandle);
            System.Windows.Window wnd = hwndSource.RootVisual as System.Windows.Window;
            if (wnd != null)
            {
                _mMyForm.Owner = wnd;
                _mMyForm.ShowInTaskbar = false;
                _mMyForm.Show();
            }
        }

         #region Idling & Closing

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

        #endregion

        #region Ribbon Panel

        public RibbonPanel RibbonPanel(UIControlledApplication a)
        {
            string tab = "BrickDims"; // Tab name
            // Empty ribbon panel 
            RibbonPanel ribbonPanel = null;
            // Try to create ribbon tab. 
            try
            {
                a.CreateRibbonTab(tab);
            }
            catch
            {
            }

            // Try to create ribbon panel.
            try
            {
                RibbonPanel panel = a.CreateRibbonPanel(tab, "Brick Dims");
            }
            catch
            {
            }

            // Search existing tab for your panel.
            List<RibbonPanel> panels = a.GetRibbonPanels(tab);
            foreach (RibbonPanel p in panels.Where(p => p.Name == "Brick Dims"))
            {
                ribbonPanel = p;
            }

            //return panel 
            return ribbonPanel;
        }

        //convert revit ribbon to adwin for extra control over graphics
        //set buttons to no text
        public static void setButtonNoText()
        {
            Debug.Print("test");
            adwin.RibbonControl ribbon = adwin.ComponentManager.Ribbon;
            adwin.RibbonTab my_tab = null;
            adwin.RibbonPanel my_panel = null; 
            adwin.RibbonButton my_buttonStop = null;
            adwin.RibbonButton my_buttonPlay = null;

            ribbon.FontSize = 6; //needs a total restart to apply
            ribbon.FontFamily = new FontFamily("Impact"); //needs a total restart to apply
            ribbon.FontWeight = System.Windows.FontWeights.DemiBold;
            ribbon.Foreground = Brushes.Gray; //doesn't do anything



            Autodesk.Internal.Windows.TabTheme myTheme = new Autodesk.Internal.Windows.TabTheme();
            myTheme.PanelBackground = Brushes.White;  //this sits behind and is hidden by our tabs
            myTheme.PanelTitleForeground = Brushes.White; //this is the tab text colour
            //myTheme.PanelTitleBackground = Brushes.White;
            //myTheme.TabHeaderBackground = Brushes.LimeGreen;
            myTheme.TabHeaderForeground = Brushes.White;
            myTheme.PanelBorder = Brushes.Black; //doesn't do anything
            myTheme.PanelSeparatorBrush = Brushes.HotPink; //doesn't do anything
            myTheme.InnerBorder = Brushes.Black; //tab border
            myTheme.OuterBorder = Brushes.Red; //doesn't do anything
            myTheme.PanelSeparatorBrush = Brushes.Black; //doesn't do anything
            //myTheme.RolloverTabHeaderBackground = Brushes.HotPink;

            foreach (var tab in ribbon.Tabs)
            {
                Debug.Print(tab.Id);
                if (tab.Id == "BrickDims")
                {
                    my_tab = tab;
                    my_tab.Theme = myTheme;
                    break;
                }
            }
            if (my_tab == null) return;
            foreach (var panel in my_tab.Panels)
            {
                Debug.Print(panel.Source.Id);
                if (panel.Source.Id == "CustomCtrl_%BrickDims%Brick Dims")
                {
                    my_panel = panel;
                    my_panel.CustomPanelTitleBarBackground = Brushes.Black;  //this is the tab colour at the bottom when it's activated
                    my_panel.CustomPanelBackground = Brushes.White;
                    break;
                }
            }
            if (my_panel == null) return;
            foreach (var item in my_panel.Source.Items)
            {
                Debug.Print(item.Id);
                // The Id property of an API created ribbon 
                // item has the following format: 
                // CustomCtrl_%CustomCtrl_%[TabName]%[PanelName]%[ItemName]
                if (item.Id.Contains("Stop"))
                {
                    Debug.Print("stop");
                    my_buttonStop = item as adwin.RibbonButton;
                    //my_buttonStop.ShowText = false;

                }
                if (item.Id.Contains("Play"))
                {
                    Debug.Print("play");
                    my_buttonPlay = item as adwin.RibbonButton;
                    my_buttonPlay.Orientation = System.Windows.Controls.Orientation.Vertical;
                    //my_buttonPlay.ShowText = false;
                    break;
                }
            }
            if (my_buttonStop == null) return;
            if (my_buttonPlay == null) return;
            //string filepath = Path.Combine(@"K:\", "ArkUser", "basis.ico");
            //my_button.LargeImage = new System.Windows.Media.Imaging.BitmapImage(new Uri(filepath, UriKind.Absolute));
        }
        #endregion
    }
}