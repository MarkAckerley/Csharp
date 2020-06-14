using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace PropertiesProjectBrowserOnOff
{
    /// <summary>
    /// Create methods here that need to be wrapped in a valid Revit Api context.
    /// Things like transactions modifying Revit Elements, etc.
    /// </summary>
    internal class Methods
    {

        /// <summary>
        /// This opens a transaction, and it MUST be executed
        /// in a "Valid Revit API Context", otherwise the add-in will crash. Because of this, we must
        /// wrap it in a ExternalEventHandler, as we do in the App.cs file in this template.
        /// </summary>
        /// <param name="ui">An instance of our UI class, which in this template is the main WPF
        /// window of the application.</param>
        /// <param name="doc">The Revit Document to rename sheets in.</param>
        public static void PropertiesOnOff(UIApplication uiApp, NewForm ui, Document doc)
        {
            Util.LogThreadInfo("Properties On Off");

            // rename all the sheets, but first open a transaction
            using (Transaction t = new Transaction(doc, "Properties On Off"))
            {
                Util.LogThreadInfo("Properties On Off Transaction");

                // start a transaction within the valid Revit API context
                t.Start("Properties On Off");

                // our command is a postable command

                RevitCommandId id_built_in = RevitCommandId.LookupPostableCommandId(PostableCommand.TogglePropertiesPalette);
                uiApp.PostCommand(id_built_in);                

                t.Commit();
                t.Dispose();
            }
        }

        public static void ProjectBrowserOnOff(UIApplication uiApp, NewForm ui, Document doc)
        {
            Util.LogThreadInfo("Project Browser On Off");

            // rename all the sheets, but first open a transaction
            using (Transaction t = new Transaction(doc, "Project Browser On Off"))
            {
                Util.LogThreadInfo("Project Browser On Off Transaction");

                // start a transaction within the valid Revit API context
                t.Start("Project Browser On Off");

                // our command is a postable command

                RevitCommandId id_built_in = RevitCommandId.LookupPostableCommandId(PostableCommand.ProjectBrowser);
                uiApp.PostCommand(id_built_in);

                t.Commit();
                t.Dispose();
            }
        }
    }
}