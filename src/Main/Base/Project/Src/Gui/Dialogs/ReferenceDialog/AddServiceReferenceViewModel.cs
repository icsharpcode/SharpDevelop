/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 12.10.2011
 * Time: 20:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

using ICSharpCode.SharpDevelop.Project;

namespace Gui.Dialogs.ReferenceDialog
{
	/// <summary>
	/// Description of AddServiceReferenceViewModel.
	/// </summary>
	public class AddServiceReferenceViewModel:ViewModelBase
	{
		public AddServiceReferenceViewModel(IProject project)
		{
			Project = project;
			title =  "Add Service Reference to <" + Project.Name +">";
			discoverButtonContend = "Disvover";
		}
		
		
		private string title;
		public string Title
		{
			get {return title;}
			set {title = value;
				base.RaisePropertyChanged(() =>Title);
			}
		}
			
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
}
