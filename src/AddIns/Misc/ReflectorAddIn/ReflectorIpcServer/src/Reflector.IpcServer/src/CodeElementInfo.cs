// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email="chhornung@googlemail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;

namespace Reflector.IpcServer
{
	/// <summary>
	/// Describes a code element.
	/// </summary>
	[Serializable]
	public sealed class CodeElementInfo
	{
		string assemblyLocation;
		readonly CodeTypeInfo type = new CodeTypeInfo();
		MemberType memberType;
		string memberName;
		int memberTypeArgumentCount;
		CodeTypeInfo memberReturnType;
		CodeTypeInfo[] memberParameters;
		
		/// <summary>
		/// Gets or sets the full path and file name of the assembly.
		/// </summary>
		public string AssemblyLocation {
			get { return assemblyLocation; }
			set { assemblyLocation = value; }
		}
		
		/// <summary>
		/// Gets the object describing the type.
		/// </summary>
		public CodeTypeInfo Type {
			get { return type; }
		}
		
		/// <summary>
		/// Gets or sets the kind of member.
		/// </summary>
		public MemberType MemberType {
			get { return memberType; }
			set { memberType = value; }
		}
		
		/// <summary>
		/// Gets or sets the member name.
		/// </summary>
		public string MemberName {
			get { return memberName; }
			set { memberName = value; }
		}
		
		/// <summary>
		/// Gets or sets the type argument count of the member.
		/// </summary>
		public int MemberTypeArgumentCount {
			get { return memberTypeArgumentCount; }
			set { memberTypeArgumentCount = value; }
		}
		
		/// <summary>
		/// Gets or sets the member return type information.
		/// </summary>
		public CodeTypeInfo MemberReturnType {
			get { return memberReturnType; }
			set { memberReturnType = value; }
		}
		
		/// <summary>
		/// Gets or sets the member parameters type information.
		/// </summary>
		public CodeTypeInfo[] MemberParameters {
			get { return memberParameters; }
			set { memberParameters = value; }
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="CodeElementInfo"/> class.
		/// </summary>
		public CodeElementInfo()
		{
		}
		
		/// <summary>
		/// Returns a string representation of the current object.
		/// </summary>
		/// <returns>A string representation of the current object.</returns>
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture,
			                     "[CodeElementInfo AssemblyLocation={0} Type={1} MemberType={2} MemberName={3} MemberTypeArgumentCount={4} MemberReturnType={5} MemberParameters={6}]",
			                     AssemblyLocation,
			                     Type == null ? "<null>" : Type.ToString(),
			                     MemberType,
			                     MemberName ?? "<null>",
			                     MemberTypeArgumentCount,
			                     MemberReturnType == null ? "<null>" : MemberReturnType.ToString(),
			                     MemberParameters == null ? "<null>" : String.Join(", ", Array.ConvertAll<CodeTypeInfo, string>(MemberParameters, TypeInfoToString)));
		}
		
		static string TypeInfoToString(CodeTypeInfo cti)
		{
			if (cti == null) return "<null>";
			return cti.ToString();
		}
	}
	
	/// <summary>
	/// Specifies a kind of class member.
	/// </summary>
	public enum MemberType
	{
		None = 0,
		Method,
		Constructor,
		Field,
		Property,
		Event
	}
}
