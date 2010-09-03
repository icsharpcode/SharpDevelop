// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;

namespace SharpRefactoring.Gui
{
	public class CtorParamWrapper : INotifyPropertyChanged
	{
		/// <summary>
		/// Underlying member. Always IField or IProperty.
		/// </summary>
		readonly IMember member;
		
		public CtorParamWrapper(IMember member)
		{
			if (member == null || member.ReturnType == null)
				throw new ArgumentNullException("field");
			if (!(member is IField || member is IProperty))
				throw new ArgumentException("member must be IField or IProperty");
			
			this.member = member;
		}
		
		public string MemberName {
			get { return member.Name; }
		}
		
		string parameterName;
		public string ParameterName {
			get { 
				if (parameterName == null)
					parameterName = ToParameterName(this.MemberName);
				return parameterName;
			}
		}
		
		public IReturnType Type {
			get { return member.ReturnType; }
		}
		
		public int Index { get; set; }
		
		public string Text {
			get { return member.ProjectContent.Language.GetAmbience().Convert(member); }
		}
		
		public bool IsNullable {
			get {
				return member.ReturnType.IsReferenceType == true ||
					member.ReturnType.IsConstructedReturnType && member.ReturnType.Name == "Nullable";
			}
		}
		
		public bool HasRange {
			get {
				return IsTypeWithRange(member.ReturnType) ||
					// IsConstructedReturnType handles Nullable types
					(member.ReturnType.IsConstructedReturnType &&
					 IsTypeWithRange(member.ReturnType.CastToConstructedReturnType().TypeArguments.First()));
			}
		}
		
		bool isSelected;
		public bool IsSelected {
			get { return isSelected; }
			set {
				isSelected = value;
				OnPropertyChanged("IsSelected");
			}
		}
		
		bool addCheckForNull;
		public bool AddCheckForNull {
			get { return addCheckForNull; }
			set {
				addCheckForNull = value;
				if (value) IsSelected = true;
			}
		}
		
		bool addRangeCheck;
		public bool AddRangeCheck {
			get { return addRangeCheck; }
			set {
				addRangeCheck = value;
				if (value) IsSelected = true;
			}
		}
		
		bool IsTypeWithRange(IReturnType type)
		{
			return type.Name == "Int32" ||
				type.Name == "Int16" ||
				type.Name == "Int64" ||
				type.Name == "Single" ||
				type.Name == "Double" ||
				type.Name == "UInt16" ||
				type.Name == "UInt32" ||
				type.Name == "UInt64";
		}
		
		static string ToParameterName(string memberName)
		{
			if (string.IsNullOrEmpty(memberName))
				return memberName;
			return char.ToLower(memberName[0]) + memberName.Substring(1);
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
