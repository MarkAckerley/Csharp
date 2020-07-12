using System;
using System.Text;
using System.Linq;

using System.Collections.Generic;
using System.Diagnostics;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;


namespace BrickDims
{
    public class BrickDimUpdater : IUpdater
    {
        public static bool m_updateActive = false;
        AddInId addinID = null;
        UpdaterId updaterID = null;
        public BrickDimUpdater(AddInId id)
        {
            addinID = id;
            // UpdaterId that is used to register and 
            // unregister updaters and triggers
            updaterID = new UpdaterId(addinID, new Guid(
              "63CDBB88-5CC4-4ac3-AD24-52DD435AAB25"));
            Debug.Print("guid");
        }

        public int caseswitch = 1;

        /// <summary>
        /// Colour Code Dimensions
        /// </summary>
        public void Execute(UpdaterData data)
        {
            Debug.Print("execute");
            if (m_updateActive == false) { return; }
            // Get access to document object
            Document doc = data.GetDocument();
            UIDocument uidoc = new UIDocument(doc);

            //using (Transaction t = new Transaction(doc, "Update Dim"))
            //{
            //   t.Start();
            try
            {
                Debug.Print("try");
                switch (caseswitch)
                {
                    case 1:
                        // Loop through all the modified elements
                        ICollection<ElementId> modifiedCollection = data.GetModifiedElementIds();
                        foreach (ElementId elemId in modifiedCollection)
                        {
                            Dimension dim = doc.GetElement(elemId) as Dimension;
                            DimClrChecker.DimClrChngSuffix(dim, uidoc);
                            Debug.Print("changeClrSuffix");
                        }
                        caseswitch = 2;
                        Debug.Print("case2");
                        break;

                    case 2:

                        caseswitch = 1;
                        Debug.Print("case1");
                        break;
                }

            }
            catch //(Exception ex)
            {
            }

            //t.Commit();
            //}
        }
        /// <summary>
        /// Set the priority
        /// </summary>
        public ChangePriority GetChangePriority()
        {
            return ChangePriority.Annotations;
        }

        /// <summary>
        /// Return the updater Id
        /// </summary>
        public UpdaterId GetUpdaterId()
        {
            return updaterID;
        }

        /// <summary>
        /// Return the auxiliary string
        /// </summary>
        public string GetAdditionalInformation()
        {
            return "Automatically update dimension overrides";
        }

        /// <summary>
        /// Return the updater name
        /// </summary>
        public string GetUpdaterName()
        {
            return "Dimension Override Updater";
        }
    }

    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    public class UIDynamicModelUpdateOff : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            BrickDimUpdater.m_updateActive = false;
            Debug.Print("off");
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    public class UIDynamicModelUpdateOn : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            BrickDimUpdater.m_updateActive = true;
            Debug.Print("on");
            return Result.Succeeded;
        }
    }
}
