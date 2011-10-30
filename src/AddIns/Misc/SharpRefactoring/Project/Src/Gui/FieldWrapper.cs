// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using SharpRefactoring.Gui;

namespace SharpRefactoring
{
	public class FieldWrapper : INotifyPropertyChanged
	{
		/// <summary>
		/// Underlying member. Always IField or IProperty.
		/// </summary>
		readonly IField field;
		
		public IField Field {
			get { return field; }
		}
		
		public FieldWrapper(IField member)
		{
			if (member == null)
				throw new ArgumentNullException("member");
			
			this.field = member;
			addSetter = true;
		}
		
		public string MemberName {
			get { return field.Name; }
		}
		
		string propertyName;
		public string PropertyName {
			get {
				if (propertyName == null)
					propertyName = ToPropertyName(this.MemberName);
				return propertyName;
			}
		}
		
		public IReturnType Type {
			get { return field.ReturnType; }
		}
		
		public int Index { get; set; }
		
		public string Text {
			get { return field.ProjectContent.Language.GetAmbience().Convert(field); }
		}
		
		bool addSetter;
		public bool AddSetter {
			get { return addSetter; }
			set {
				addSetter = value;
			}
		}
		
		public bool IsSetable {
			get {
				return !field.IsReadonly && !field.IsConst;
			}
		}
		
		static string ToPropertyName(string memberName)
		{
			if (string.IsNullOrEmpty(memberName))
				return memberName;
			return char.ToUpper(memberName[0]) + memberName.Substring(1);
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void OnPropertyChanged(string property)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(property));
			}
		}
	}
}
