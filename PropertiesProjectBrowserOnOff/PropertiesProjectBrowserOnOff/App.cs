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

namespace PropertiesProjectBrowserOnOff
{
    
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class App : IExternalApplication
    {
        // class instance 
        public static App ThisApp;

        public static NewForm myForm
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
            //Register dockable pane
            //EXTERNAL EVENTS WITH ARGUMENTS
            EventHandlerMethodPropertiesOnOff evP = new EventHandlerMethodPropertiesOnOff();
            EventHandlerMethodProjectBrowserOnOff evPB = new EventHandlerMethodProjectBrowserOnOff();
            NewForm Form = new NewForm(evP, evPB);
           // DockablePaneId paneId = new DockablePaneId(new Guid("9d7ed357-534f-4d87-afc4-8e784e3b119e"));

            myForm = Form;

            ThisApp = this;

            a.RegisterDockablePane(paneId, "P PB", Form as IDockablePaneProvider);

            // Method to add Tab and Panel 
            RibbonPanel panel = RibbonPanel(a);
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            // BUTTON FOR THE DOCKABLE WPF
            if (panel.AddItem(
                new PushButtonData("PropertiesProjectBrowserOnOff", "PropertiesProjectBrowserOnOff", thisAssemblyPath,
                    "PropertiesProjectBrowserOnOff.EntryCommand")) is PushButton button)
            {
                // defines the tooltip displayed when the button is hovered over in Revit's ribbon
                button.ToolTip = "Turn Properties & ProjectBrowser On/Off";
                // defines the icon for the button in Revit's ribbon - note the string formatting
                Uri uriImage = new Uri("pack://application:,,,/PropertiesProjectBrowserOnOff;component/Resources/code-small.png");
                BitmapImage largeImage = new BitmapImage(uriImage);
                button.LargeImage = largeImage;
            }

            // listeners/watchers for external events (if you choose to use them)
            a.ApplicationClosing += a_ApplicationClosing; //Set Application to Idling
            a.Idling += a_Idling;

            return Result.Succeeded;
        }

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
            DockablePane myForm = commandData.Application.GetDockablePane(paneId);
            // If we do not have a dialog yet, create and show it
            if (myForm != null && myForm == null) return;
            //EXTERNAL EVENTS WITH ARGUMENTS
            //EXTERNAL EVENTS WITH ARGUMENTS
            EventHandlerMethodPropertiesOnOff evP = new EventHandlerMethodPropertiesOnOff();
            EventHandlerMethodProjectBrowserOnOff evPB = new EventHandlerMethodProjectBrowserOnOff();

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
