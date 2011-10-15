/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 12.10.2011
 * Time: 20:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

using ICSharpCode.SharpDevelop.Project;
using Microsoft.Win32;

namespace Gui.Dialogs.ReferenceDialog
{
	/// <summary>
	/// Description of AddServiceReferenceViewModel.
	/// </summary>
	public class AddServiceReferenceViewModel:ViewModelBase
	{
		string header1 = "To see a list of available services on an specific Server, ";
		string header2 = "enter a service URL and click Go. To browse for available services click Discover";
		
		string noUrl = "Please enter he address of the Service.";
		
		string discoverMenu ="Services in Solution";
		
		public AddServiceReferenceViewModel(IProject project)
		{
			Project = project;
			title =  "Add Service Reference";
			discoverButtonContend = "Disvover";
			HeadLine = header1 + header2;
			
			MruServices = AddMruList();
			SelectedService = MruServices[0];
			
			GoCommand = new RelayCommand(ExecuteGo,CanExecuteGo);
			DiscoverCommand = new RelayCommand(ExecuteDiscover,CanExecuteDiscover);
		}
		
		private string art;
		
		public string Art {
			get { return art; }
			set { art = value;
				base.RaisePropertyChanged(() =>Art);
			}
		}
		
		
		private string title;
		public string Title
		{
			get {return title;}
			set {title = value;
				base.RaisePropertyChanged(() =>Title);
			}
		}
		

		public string HeadLine {get; private set;}
		
		private string discoverButtonContend;
		
		public string DiscoverButtonContend {
			get { return discoverButtonContend; }
			set { discoverButtonContend = value;
			base.RaisePropertyChanged(() =>DiscoverButtonContend);}
		}

		
		private IProject project;
		
		public IProject Project
		{
			get {return project;}
			set {project = value;
				base.RaisePropertyChanged(() =>Project);
			}
		}
	
		#region Create List of services
		
		// Modifyed Code from Matt
		
		List <string>  AddMruList()
		{
			var list = new List<string>();
			try {
				RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\TypedURLs");
				if (key != null) {
					foreach (string name in key.GetValueNames()) {
						list.Add ((string)key.GetValue(name));
					}
				}
			} catch (Exception)
			{
			};
			return list;
		}
		
		private List<string> mruServices;
		
		public List<string> MruServices {
			get {
				if (mruServices == null) {
					mruServices = new List<string>();
				}
				return mruServices; }
			set { mruServices = value;
				base.RaisePropertyChanged(() =>MruServices);
			}
		}
		
		private string selectedService;
		
		public string SelectedService {
			get { return selectedService; }
			set { selectedService = value;
				base.RaisePropertyChanged(() =>SelectedService);}
		}
		
		#endregion
		
		#region Go
		
		public ICommand GoCommand {get; private set;}
		
		private void ExecuteGo ()
		{
			string s = String.Format("<Go> with Service <{0}>",SelectedService);
			MessageBox.Show (s);
		}
		
		private bool CanExecuteGo()
		{
			return true;
		}
		
		#endregion
		
		#region Discover
		
		public ICommand DiscoverCommand {get;private set;}
		
		private bool CanExecuteDiscover ()
		{
			return true;
		}
		
		private void ExecuteDiscover ()
		{
			MessageBox.Show ("<Discover> is not implemented at the Moment");
		}
			
		#endregion
	}
	
	public class ViewModelBase:INotifyPropertyChanged
	{
		public ViewModelBase()
		{
		}
		
		
		public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
		
		
		protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpresssion)
        {
            var propertyName = ExtractPropertyName(propertyExpresssion);
            this.RaisePropertyChanged(propertyName);
        }

		
        protected void RaisePropertyChanged(String propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        
        
        private static String ExtractPropertyName<T>(Expression<Func<T>> propertyExpresssion)
        {
            if (propertyExpresssion == null)
            {
                throw new ArgumentNullException("propertyExpresssion");
            }

            var memberExpression = propertyExpresssion.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("The expression is not a member access expression.", "propertyExpresssion");
            }

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException("The member access expression does not access a property.", "propertyExpresssion");
            }

            var getMethod = property.GetGetMethod(true);
            if (getMethod.IsStatic)
            {
                throw new ArgumentException("The referenced property is a static property.", "propertyExpresssion");
            }

            return memberExpression.Member.Name;
        }
	}
	
	
	public class RelayCommand<T> : ICommand
    {

        #region Declarations

        readonly Predicate<T> _canExecute;
        readonly Action<T> _execute;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand&lt;T&gt;"/> class and the command can always be executed.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {

            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add
            {

                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {

                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        [DebuggerStepThrough]
        public Boolean CanExecute(Object parameter)
        {
            return _canExecute == null ? true : _canExecute((T)parameter);
        }

        public void Execute(Object parameter)
        {
            _execute((T)parameter);
        }

        #endregion
    }

    /// <summary>
    /// A command whose sole purpose is to relay its functionality to other objects by invoking delegates. The default return value for the CanExecute method is 'true'.
    /// </summary>
    public class RelayCommand : ICommand
    {

        #region Declarations

        readonly Func<Boolean> _canExecute;
        readonly Action _execute;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand&lt;T&gt;"/> class and the command can always be executed.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action execute, Func<Boolean> canExecute)
        {

            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add
            {

                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {

                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        [DebuggerStepThrough]
        public Boolean CanExecute(Object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        public void Execute(Object parameter)
        {
            _execute();
        }

        #endregion
    }
    
}
