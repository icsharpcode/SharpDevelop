// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
