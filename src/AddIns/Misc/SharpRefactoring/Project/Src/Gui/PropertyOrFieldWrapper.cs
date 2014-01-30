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
	public class PropertyOrFieldWrapper : INotifyPropertyChanged
	{
		/// <summary>
		/// Underlying member. Always IField or IProperty.
		/// </summary>
		readonly IMember member;
		
		public PropertyOrFieldWrapper(IMember member)
		{
			if (member == null)
				throw new ArgumentNullException("member");
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
				// true = reference, null = generic or unknown
				return member.ReturnType.IsReferenceType != false
					|| (member.ReturnType.IsConstructedReturnType && member.ReturnType.Name == "Nullable");
			}
		}
		
		public bool HasRange {
			get {
				return IsTypeWithRange(member.ReturnType) ||
					// IsConstructedReturnType handles Nullable types
					(member.ReturnType.IsConstructedReturnType && member.ReturnType.Name == "Nullable"
					&& IsTypeWithRange(member.ReturnType.CastToConstructedReturnType().TypeArguments.First()));
			}
		}
		
		bool addCheckForNull;
		public bool AddCheckForNull {
			get { return addCheckForNull; }
			set {
				addCheckForNull = value;
			}
		}
		
		bool addRangeCheck;
		public bool AddRangeCheck {
			get { return addRangeCheck; }
			set {
				addRangeCheck = value;
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
