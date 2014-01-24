// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
