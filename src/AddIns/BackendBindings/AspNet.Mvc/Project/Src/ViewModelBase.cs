// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ICSharpCode.AspNet.Mvc
{
	public abstract class ViewModelBase<TModel> : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		
		public string PropertyChangedFor<TProperty>(Expression<Func<TModel, TProperty>> expression)
		{
			MemberExpression memberExpression = expression.Body as MemberExpression;
			return PropertyChangedFor(memberExpression);
		}
		
		string PropertyChangedFor(MemberExpression memberExpression)
		{
			if (memberExpression != null) {
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
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
