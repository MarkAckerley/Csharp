using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BrickDims
{
    /// <summary>
    /// Interaction logic for UI.xaml
    /// </summary>
    public partial class BrickDimSelector : Window, INotifyPropertyChanged
    {
        private double m_MyWindowHeight = 0;
        public double MyWindowHeight
        {
            get
            {
                return m_MyWindowHeight;
            }
            set
            {
                m_MyWindowHeight = value;
                this.OnPropertyChanged("MyWindowHeight");
            }
        }


        private readonly Document _doc;

        //private readonly UIApplication _uiApp;
        //private readonly Autodesk.Revit.ApplicationServices.Application _app;
        private readonly UIDocument _uiDoc;

        private readonly EventHandlerWithWpfArg _mExternalMethodWpfArg;

        public BrickDimSelector(UIApplication uiApp, EventHandlerWithWpfArg eExternalMethodWpfArg)
        {
            _uiDoc = uiApp.ActiveUIDocument;
            _doc = _uiDoc.Document;
            //_app = _doc.Application;
            //_uiApp = _doc.Application;
            Closed += MainWindow_Closed;

            MyWindowHeight = 135;
            this.DataContext = this;

            InitializeComponent();

            _mExternalMethodWpfArg = eExternalMethodWpfArg;
        }

        private void OnDragMoveWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            if (MyWindowHeight == 20)
            {
                MyWindowHeight = 135;
            }
            else
            {
                MyWindowHeight = 20;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnCloseWindow(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }


        private void ButtonDown(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("ButtonDown");

        }//Button Down method ends here


        private void ButtonUp(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Button Up");

        }//ButtonUp method ends here


        private void ButtonLeave(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("ButtonLeave");
        }


        private void ButtonEnter(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("ButtonEnter");
        }


        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Close();
        }

        #region External Project Methods

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }



        private void BExternalMethod1_Click(object sender, RoutedEventArgs e)
        {
            // Raise external event with this UI instance (WPF) as an argument
            _mExternalMethodWpfArg.Raise(this);
        }

        #endregion

    }
}