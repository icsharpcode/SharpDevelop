// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email="chhornung@googlemail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Reflector.IpcServer
{
	/// <summary>
	/// Describes a type.
	/// </summary>
	[Serializable]
	public sealed class CodeTypeInfo
	{
		string @namespace;
		string[] typeNames = new string[0];
		int[] typeArgumentCount = new int[0];
		CodeTypeInfo arrayElementType;
		int arrayDimensions;
		
		/// <summary>
		/// Gets or sets the namespace.
		/// </summary>
		public string Namespace {
			get { return @namespace; }
			set { @namespace = value; }
		}
		
		/// <summary>
		/// Gets or sets the type names (without namespace).
		/// The order must be from the outermost type name to the innermost type name.
		/// </summary>
		public string[] TypeNames {
			get { return typeNames; }
			set { typeNames = value; }
		}
		
		/// <summary>
		/// Gets or sets the type argument counts for the types as specified in the <see cref="TypeNames"/> property..
		/// The order must be from the outermost type to the innermost type.
		/// </summary>
		public int[] TypeArgumentCount {
			get { return typeArgumentCount; }
			set { typeArgumentCount = value; }
		}
		
		/// <summary>
		/// Gets or sets the array element type.
		/// </summary>
		public CodeTypeInfo ArrayElementType {
			get { return arrayElementType; }
			set { arrayElementType = value; }
		}
		
		/// <summary>
		/// Gets or sets the number of array dimensions of the type.
		/// </summary>
		public int ArrayDimensions {
			get { return arrayDimensions; }
			set { arrayDimensions = value; }
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="CodeTypeInfo"/> class.
		/// </summary>
		public CodeTypeInfo()
		{
		}
		
		/// <summary>
		/// Returns a string representation of the current object.
		/// </summary>
		/// <returns>A string representation of the current object.</returns>
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture,
			                     "[CodeTypeInfo: Namespace={0} TypeNames={1} TypeArgumentCount={2} ArrayElementType={3} ArrayDimensions={4}]",
			                     Namespace,
			                     String.Join("+", TypeNames),
			                     String.Join("+", Array.ConvertAll<int, string>(TypeArgumentCount, IntToString)),
			                     ArrayElementType == null ? "<null>" : ArrayElementType.ToString(),
			                     ArrayDimensions);
		}
		
		static string IntToString(int i)
		{
			return i.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}
	}
}
