// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ICSharpCode.NRefactory.CSharp;

namespace CSharpBinding.FormattingStrategy
{
	/// <summary>
	/// Generic container for C# formatting options that can be chained together from general to specific and inherit
	/// options from parent.
	/// </summary>
	public class CSharpFormattingOptionsContainer
	{
		CSharpFormattingOptionsContainer parent;
		CSharpFormattingOptionsContainer child;
		
		Dictionary<string, object> options;
		
		internal CSharpFormattingOptionsContainer()
		{
			parent = null;
			child = null;
			
			options = new Dictionary<string, object>();
		}
		
		public CSharpFormattingOptionsContainer Child
		{
			get
			{
				return child;
			}
			set
			{
				if (child != null) {
					child.parent = this;
				}
				child = value;
			}
		}
		
		/// <summary>
		/// Retrieves the value of a formatting option where desired type and option name are defined by a
		/// property getter on <see cref="ICSharpCode.NRefactory.CSharp.CSharpFormattingOptions"/>.
		/// Searches in current and (if nothing set here) parent containers.
		/// </summary>
		/// <param name="propertyGetter">
		/// Property getter lambda expression
		/// (example: o =&gt; o.IndentStructBody)
		/// </param>
		/// <returns>True, if option with given type could be found in hierarchy. False otherwise.</returns>
		public T GetOption<T>(Expression<Func<CSharpFormattingOptions, T>> propertyGetter)
		{
			// Get name of property (to look for in dictionary)
			string optionName = null;
			MemberExpression memberExpression = propertyGetter.Body as MemberExpression;
			if (memberExpression != null) {
				optionName = memberExpression.Member.Name;
				return GetOption<T>(optionName);
			}
			
			return default(T);
		}
		
		/// <summary>
		/// Retrieves the value of a formatting option by looking at current and (if nothing set here) parent
		/// containers.
		/// </summary>
		/// <param name="option">Name of option</param>
		/// <returns>True, if option with given type could be found in hierarchy. False otherwise.</returns>
		public T GetOption<T>(string option)
		{
			// Run up the hierarchy until we find a defined value for property
			CSharpFormattingOptionsContainer container = this;
			do
			{
				object val;
				container.options.TryGetValue(option, out val);
				if (val is T) {
					return (T) val;
				}
				container = container.parent;
			} while (container != null);
			
			return default(T);
		}
		
		/// <summary>
		/// Creates a <see cref="ICSharpCode.NRefactory.CSharp.CSharpFormattingOptions"/> instance from current
		/// container, resolving all options through container hierarchy.
		/// </summary>
		/// <returns>Created and filled <see cref="ICSharpCode.NRefactory.CSharp.CSharpFormattingOptions"/> instance.</returns>
		public CSharpFormattingOptions CreateOptions()
		{
			var outputOptions = FormattingOptionsFactory.CreateEmpty();
			
			// Look at all container options and try to set identically named properties of CSharpFormattingOptions
			foreach (PropertyInfo propertyInfo in typeof(CSharpFormattingOptions).GetProperties()) {
				object val = GetOption<object>(propertyInfo.Name);
				if ((val != null) && (val.GetType() == propertyInfo.PropertyType)) {
					propertyInfo.SetValue(outputOptions, val);
				}
			}

			return outputOptions;
		}
	}
}
