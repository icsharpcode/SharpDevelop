#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using ICSharpCode.Data.Core.UI.UserControls;
using System.ComponentModel;
using System.Collections.Specialized;

#endregion

namespace ICSharpCode.Data.Core.UI.Windows
{
    /// <summary>
    /// Interaction logic for WizardWindow.xaml
    /// </summary>
    public partial class WizardWindowInnards : UserControl, INotifyPropertyChanged
    {
        #region Fields

        internal static readonly DependencyProperty IsReadyForNextStepProperty =
            DependencyProperty.Register("IsReadyForNextStep", typeof(bool), typeof(WizardWindowInnards), new FrameworkPropertyMetadata(false, IsReadyForNextStep_Changed)); 
        private WizardWindow _wizardWindow = null;
        private ObservableCollection<WizardUserControl> _wizardUserControls = new ObservableCollection<WizardUserControl>();
        private WizardUserControl _currentWizardUserControl = null;
        private int _currentIndex = 0;

        #endregion

        #region Properties

        /// <summary>
        /// Returns the ObservableCollection of WizardUserControls, which are displayed in this WizardWindow.
        /// </summary>
        public ObservableCollection<WizardUserControl> WizardUserControls
        {
            get { return _wizardUserControls; }
        }

        /// <summary>
        /// Returns the current WizardUserControls index of the WizardWindow.
        /// </summary>
        public int CurrentIndex
        {
            get { return _currentIndex; }
            protected set
            {
                _currentIndex = value;
                _currentWizardUserControl = null;
                OnPropertyChanged("CurrentIndex");
                OnPropertyChanged("CurrentWizardUserControl");
            }
        }

        /// <summary>
        /// Returns the current WizardUserControl of the WizardWindow.
        /// </summary>
        public WizardUserControl CurrentWizardUserControl
        {
            get 
            {
                if (_currentWizardUserControl != null)
                    return _currentWizardUserControl;
                else
                {
                    _currentWizardUserControl = _wizardUserControls.FirstOrDefault(wuc => wuc.Index == _currentIndex);
                    BindingBase binding = new Binding("IsReadyForNextStep") { Source = _currentWizardUserControl };
                    SetBinding(WizardWindowInnards.IsReadyForNextStepProperty, binding);
                    ToggleEnabledButtons();
                    return _currentWizardUserControl;
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initizales the WizardWindow.
        /// </summary>
        public WizardWindowInnards(WizardWindow wizardWindow)
        {
            InitializeComponent();
            _wizardWindow = wizardWindow;
            _wizardUserControls.CollectionChanged += new NotifyCollectionChangedEventHandler(WizardUserControls_CollectionChanged);
            DataContext = wizardWindow;
        }

        #endregion

        #region Methods

        private void ToggleEnabledButtons()
        {
            if (!IsInitialized)
                return;
            
            if (CurrentIndex == 0)
            {
                btnPrevious.IsEnabled = false;
            }
            else
            {
                btnPrevious.IsEnabled = true;
            }

            if (CurrentIndex == _wizardUserControls.Count - 1)
            {
                btnNext.IsEnabled = false;
            }
            else
            {
                if (CurrentWizardUserControl.IsReadyForNextStep)
                    btnNext.IsEnabled = true;
                else
                    btnNext.IsEnabled = false;
            }

            if (CurrentWizardUserControl.CanFinish)
            {
                if (CurrentWizardUserControl.IsReadyForNextStep)
                    btnFinish.IsEnabled = true;
            }
            else
                btnFinish.IsEnabled = false;

            CurrentWizardUserControl.OnActivate();
        }

        #endregion

        #region Event handlers

        private static void IsReadyForNextStep_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WizardWindowInnards wizardWindowInnards = d as WizardWindowInnards;
            wizardWindowInnards.ToggleEnabledButtons();
        }

        private void WizardUserControls_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("CurrentWizardUserControl");
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentIndex == 0)
                return;
            else
            {
                CurrentIndex--;
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentIndex == _wizardUserControls.Count - 1)
                return;
            else
            {
                CurrentIndex++;
            }
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            _wizardWindow.OnFinished();
            
            _wizardWindow.DialogResult = true;
            _wizardWindow.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _wizardWindow.DialogResult = false;
            _wizardWindow.Close();
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }
}
