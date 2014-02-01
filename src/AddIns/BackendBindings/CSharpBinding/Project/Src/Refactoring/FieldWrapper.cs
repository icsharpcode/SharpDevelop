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
	public class FieldWrapper : INotifyPropertyChanged
	{
		/// <summary>
		/// Underlying member. Always IField or IProperty.
		/// </summary>
		readonly IField field;
		private string text;
		bool isIncluded;
		
		public IField Field {
			get { return field; }
		}
		
		public FieldWrapper(IField member)
		{
			if (member == null)
				throw new ArgumentNullException("member");
			
			this.field = member;
			addSetter = true;
			
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
				OnPropertyChanged("IsIncluded");
			}
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
		
		public IType Type {
			get { return field.ReturnType; }
		}
		
		public int Index { get; set; }
		
		public string Text {
			get { return text; }
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
				return !field.IsReadOnly && !field.IsConst;
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
