using System;
using System.Text;
using System.Linq;

using System.Collections.Generic;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;


namespace BrickDims
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class BrickDimsColourOverride : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            BrickDimsColour(commandData.Application.ActiveUIDocument);

            return Result.Succeeded;
        }

        public void BrickDimsColour(UIDocument uidoc)
        {
            //filtered element collector dimensions in active view


            Transaction transaction = new Transaction(uidoc.Document);
            transaction.Start("Colour Dims By Brick Dim");

            List<Element> linearDimensions = new FilteredElementCollector(uidoc.Document, uidoc.Document.ActiveView.Id).OfCategory(BuiltInCategory.OST_Dimensions).ToList();

            if (linearDimensions.Count == 0)
            {
                TaskDialog.Show("fail", "oops that didn't work, perhaps try another view? or adding a dimension?");
            }

            //List<Element> linearDimensions = new FilteredElementCollector(uidoc.Document).ToList();

            //create lists of dimensions
            foreach (Dimension dim in linearDimensions)
            {
                DimClrChecker.DimClrChngSuffix(dim, uidoc);
            }
            //mainDialog.MainInstruction = sb.ToString();
            //TaskDialogResult tResult = mainDialog.Show();

            transaction.Commit();

        }

    }

    internal class Methods
    {
        public static void BrickDimCheck(BrickDimSelector brDimSel, UIDocument uidoc, Document doc)
        {
            ISelectionFilter dimsFilt = new SelectionFilterDims();
            ElementId dimId = uidoc.Selection.PickObject(ObjectType.Element, dimsFilt).ElementId;
            Dimension dim = doc.GetElement(dimId) as Dimension;

            StringBuilder sb = new StringBuilder();

            //if empty add to cut brick list list
            //we could also check for multi dims and flag to user, poss with split method...
            if (dim.Value == null)
            {
                sb.Append("Something went wrong, perhaps this is a multi-dim?");
                brDimSel.BrickDimReadout.Content = sb.ToString();
                //we could probably override suffixes here if we wanted...dim.Suffix = "Joint";
                //this is probably a multi one, we could give a list?!
            }

            else
            {
                //get dimension value in mm
                double dimValue = Convert.ToDouble(dim.Value);
                double dimValueMm = UnitUtils.ConvertFromInternalUnits(dimValue, UnitTypeId.Millimeters);
                double dimValueMmRnd = BrickFuncts.round(dimValueMm); //round to 1 dp

                //divide by CO
                double dimCO = dimValueMmRnd / BrickFuncts.CO(1);
                //get math floor and set of CO values
                double dimCOflr = Math.Floor(dimCO);

                double dimCOflrCO = BrickFuncts.round(BrickFuncts.CO(dimCOflr));
                double dimCOflrCOminus = BrickFuncts.round(BrickFuncts.COMinus(dimCOflr));
                double dimCOflrCOPlus = BrickFuncts.round(BrickFuncts.COPlus(dimCOflr));

                //get math ceiling and set of CO values
                double dimCOclng = Math.Ceiling(dimCO);

                double dimCOclngCO = BrickFuncts.round(BrickFuncts.CO(dimCOclng));
                double dimCOclngCOminus = BrickFuncts.round(BrickFuncts.COMinus(dimCOclng));
                double dimCOclngCOPlus = BrickFuncts.round(BrickFuncts.COPlus(dimCOclng));

                sb.Append("CO-\t\tCO\t\tCO+\n");
                sb.Append(dimCOflrCOminus.ToString() + "\t\t" + dimCOflrCO.ToString() + "\t\t" + dimCOflrCOPlus.ToString() + "\n");
                sb.Append(dimCOclngCOminus.ToString() + "\t\t" + dimCOclngCO.ToString() + "\t\t" + dimCOclngCOPlus.ToString());

                brDimSel.BrickDimReadout.Content = sb.ToString();
                brDimSel.DimSelClick.Text = dimValueMmRnd.ToString();
            }
        }
    }

    public static class BrickFuncts
    {
        //base input CO-
        public static double brick = 102.5;

        //CO for x bricks
        public static double CO(double a)
        {
            return (brick + 10) * a;
        }

        //co for y bricks
        public static double COPlus(double a)
        {
            return CO(a) + 10;
        }

        //co- for z bricks
        public static double COMinus(double a)
        {
            return CO(a) - 10;
        }

        public static double round(double a)
        {
            return (Math.Round(a * 10)) / 10;
        }

    }

    public static class ClrSettings
    {
        //create graphic settings to override dims in view

        //CO
        public static OverrideGraphicSettings ogsCO()
        {
            OverrideGraphicSettings ogsCO = new OverrideGraphicSettings();
            ogsCO.SetProjectionLineColor(new Color(100, 200, 100)); //green
            return ogsCO;
        }


        //CO minus
        public static OverrideGraphicSettings ogsCOminus()
        {
            OverrideGraphicSettings ogsCOminus = new OverrideGraphicSettings();
            ogsCOminus.SetProjectionLineColor(new Color(100, 100, 200)); //blue
            return ogsCOminus;
        }


        //CO plus
        public static OverrideGraphicSettings ogsCOplus()
        {
            OverrideGraphicSettings ogsCOplus = new OverrideGraphicSettings();
            ogsCOplus.SetProjectionLineColor(new Color(200, 200, 50)); //yellow
            return ogsCOplus;
        }

        //Cut
        public static OverrideGraphicSettings ogsCut()
        {
            OverrideGraphicSettings ogsCut = new OverrideGraphicSettings();
            ogsCut.SetProjectionLineColor(new Color(200, 0, 0)); //red
            return ogsCut;
        }

        //Joint
        public static OverrideGraphicSettings ogsJoint()
        {
            OverrideGraphicSettings ogsJoint = new OverrideGraphicSettings();
            ogsJoint.SetProjectionLineColor(new Color(128, 0, 128)); //purple
            return ogsJoint;
        }

        //Multi
        public static OverrideGraphicSettings ogsMulti()
        {
            OverrideGraphicSettings ogsMulti = new OverrideGraphicSettings();
            ogsMulti.SetProjectionLineColor(new Color(255, 192, 203)); //pink
            return ogsMulti;
        }

    }

    public class SelectionFilterDims : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is Dimension) return true;
            return false;
        }
        public bool AllowReference(Reference refer, XYZ pos)
        {
            return false;
        }
    }

    public class DimClrChecker
    {
        
       public static void DimClrChngSuffix(Dimension dim, UIDocument uidoc)
       {
            //if empty add to cut brick list list
            //we could also check for multi dims and flag to user, poss with split method...
            if (dim.Value == null)
            {
                uidoc.ActiveView.SetElementOverrides(dim.Id, ClrSettings.ogsJoint());
                //we could probably override suffixes here if we wanted...dim.Suffix = "Joint";
            }

            else
            {
                //get dimension value in mm
                double dimValue = Convert.ToDouble(dim.Value);
                double dimValueMm = UnitUtils.ConvertFromInternalUnits(dimValue, UnitTypeId.Millimeters);
                double dimValueMmRnd = BrickFuncts.round(dimValueMm); //round to 1 dp

                if (dimValueMmRnd == 10)
                {
                    //if value == 10, add item to cut brick list
                    uidoc.ActiveView.SetElementOverrides(dim.Id, ClrSettings.ogsJoint());
                    dim.Suffix = "Joint";
                }

                else
                {
                    //divide by CO
                    double dimCO = dimValueMmRnd / BrickFuncts.CO(1);
                    //get math floor and set of CO values
                    double dimCOflr = Math.Floor(dimCO);

                    double dimCOflrCO = BrickFuncts.round(BrickFuncts.CO(dimCOflr));
                    double dimCOflrCOminus = BrickFuncts.round(BrickFuncts.COMinus(dimCOflr));
                    double dimCOflrCOPlus = BrickFuncts.round(BrickFuncts.COPlus(dimCOflr));

                    //get math ceiling and set of CO values
                    double dimCOclng = Math.Ceiling(dimCO);

                    double dimCOclngCO = BrickFuncts.round(BrickFuncts.CO(dimCOclng));
                    double dimCOclngCOminus = BrickFuncts.round(BrickFuncts.COMinus(dimCOclng));
                    double dimCOclngCOPlus = BrickFuncts.round(BrickFuncts.COPlus(dimCOclng));

                    if ((dimValueMmRnd == dimCOflrCO) || (dimValueMmRnd == dimCOclngCO))
                    {
                        dim.Suffix = "CO";
                        uidoc.ActiveView.SetElementOverrides(dim.Id, ClrSettings.ogsCO());
                    }

                    else if ((dimValueMmRnd == dimCOflrCOminus) || (dimValueMmRnd == dimCOclngCOminus))
                    {
                        dim.Suffix = "CO-";
                        uidoc.ActiveView.SetElementOverrides(dim.Id, ClrSettings.ogsCOminus());
                    }

                    else if ((dimValueMmRnd == dimCOflrCOPlus) || (dimValueMmRnd == dimCOclngCOPlus))
                    {
                        dim.Suffix = "CO+";
                        uidoc.ActiveView.SetElementOverrides(dim.Id, ClrSettings.ogsCOplus());                        
                    }

                    else
                    {
                        dim.Suffix = "cut";
                        uidoc.ActiveView.SetElementOverrides(dim.Id, ClrSettings.ogsCut());
                    }

                }

                //sb.AppendLine(dimValueMmRnd.ToString());
            }
       }
    }
}