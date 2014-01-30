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
using System.ComponentModel;
using System.Linq;

using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;

namespace CSharpBinding.Refactoring
{
	public class PropertyOrFieldWrapper : INotifyPropertyChanged
	{
		bool isIncluded;
		
		/// <summary>
		/// Underlying member. Always IField or IProperty.
		/// </summary>
		public readonly IMember Member;
		private readonly string text;
		
		public PropertyOrFieldWrapper(IMember member)
		{
			isIncluded = false;
			
			if (member == null)
				throw new ArgumentNullException("member");
			if (!(member is IField || member is IProperty))
				throw new ArgumentException("member must be IField or IProperty");
			
			this.Member = member;
			IAmbience ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags |=
				ConversionFlags.ShowReturnType
				| ConversionFlags.ShowModifiers
				| ConversionFlags.ShowAccessibility;
			this.text = ambience.ConvertSymbol(member);
		}
		
		public bool IsIncluded
		{
			get {
				return isIncluded;
			}
			set {
				isIncluded = value;
				if (!value) {
					// Remove other flags, too
					AddCheckForNull = false;
					AddRangeCheck = false;
				}
				OnPropertyChanged("IsIncluded");
			}
		}
		
		public string MemberName {
			get { return Member.Name; }
		}
		
		string parameterName;
		public string ParameterName {
			get {
				if (parameterName == null)
					parameterName = ToParameterName(this.MemberName);
				return parameterName;
			}
		}
		
		public IType Type {
			get { return Member.ReturnType; }
		}
		
		public string Text {
			get { return text; }
		}
		
		public int Index { get; set; }
		
		public bool IsNullable {
			get {
				// true = reference, null = generic or unknown
				return Member.ReturnType.IsNullable();
			}
		}
		
		public bool HasRange {
			get {
				return Member.ReturnType.HasRange();
			}
		}
		
		bool addCheckForNull;
		public bool AddCheckForNull {
			get { return addCheckForNull; }
			set {
				addCheckForNull = value;
				if (value) {
					// Assure that IsIncluded is set to true as well
					IsIncluded = true;
				}
				OnPropertyChanged("AddCheckForNull");
			}
		}
		
		bool addRangeCheck;
		public bool AddRangeCheck {
			get { return addRangeCheck; }
			set {
				addRangeCheck = value;
				if (value) {
					// Assure that IsIncluded is set to true as well
					IsIncluded = true;
				}
				OnPropertyChanged("AddRangeCheck");
			}
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
