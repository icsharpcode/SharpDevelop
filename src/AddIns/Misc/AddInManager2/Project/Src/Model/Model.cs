// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ICSharpCode.AddInManager2.Model
{
	public abstract class Model<TModel> : INotifyPropertyChanged
	{
		private IAddInManagerServices _services = null;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		public Model()
		{
			// Use default services container
			_services = AddInManagerServices.Services;
		}
		
		public Model(IAddInManagerServices services)
		{
			_services = services;
		}
		
		public string PropertyChangedFor<TProperty>(Expression<Func<TModel, TProperty>> expression)
		{
			MemberExpression memberExpression = expression.Body as MemberExpression;
			return PropertyChangedFor(memberExpression);
		}
		
		private string PropertyChangedFor(MemberExpression memberExpression)
		{
			if (memberExpression != null)
			{
				return memberExpression.Member.Name;
			}
			return String.Empty;
		}
		
		protected void OnPropertyChanged<TProperty>(Expression<Func<TModel, TProperty>> expression)
		{
			string propertyName = PropertyChangedFor(expression);
			OnPropertyChanged(propertyName);
		}
		
		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		protected IAddInManagerServices AddInManager
		{
			get
			{
				return _services;
			}
			set
			{
				_services = value;
			}
		}
	}
}
