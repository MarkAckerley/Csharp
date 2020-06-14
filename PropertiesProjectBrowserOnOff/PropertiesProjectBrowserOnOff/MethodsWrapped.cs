using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace PropertiesProjectBrowserOnOff
{

    /// <summary>
    /// This is an example of of wrapping a method with an ExternalEventHandler using an instance of WPF
    /// as an argument. Any type of argument can be passed to the RevitEventWrapper, and therefore be used in
    /// the execution of a method which has to take place within a "Valid Revit API Context". This specific
    /// pattern can be useful for smaller applications, where it is convenient to access the WPF properties
    /// directly, but can become cumbersome in larger application architectures. At that point, it is suggested
    /// to use more "low-level" wrapping, as with the string-argument-wrapped method above.
    /// </summary>
    public class EventHandlerMethodPropertiesOnOff : RevitEventWrapper<NewForm>
    {
        /// <summary>
        /// The Execute override void must be present in all methods wrapped by the RevitEventWrapper.
        /// This defines what the method will do when raised externally.
        /// </summary>
        public override void Execute(UIApplication uiApp, NewForm ui)
        {
            // SETUP
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            Methods.PropertiesOnOff(uiApp, ui, doc);

        }
    }
    public class EventHandlerMethodProjectBrowserOnOff : RevitEventWrapper<NewForm>
    {
        /// <summary>
        /// The Execute override void must be present in all methods wrapped by the RevitEventWrapper.
        /// This defines what the method will do when raised externally.
        /// </summary>
        public override void Execute(UIApplication uiApp, NewForm ui)
        {
            // SETUP
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            Methods.ProjectBrowserOnOff(uiApp, ui, doc);

        }
    }
}
