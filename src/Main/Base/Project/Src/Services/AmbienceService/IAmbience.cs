// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop
{
	[Flags]
	public enum ConversionFlags
	{
		/// <summary>
		/// Convert only the name.
		/// </summary>
		None = 0,
		/// <summary>
		/// Show the parameter list
		/// </summary>
		ShowParameterList      = 1,
		/// <summary>
		/// Show names for parameters
		/// </summary>
		ShowParameterNames     = 2,
		/// <summary>
		/// Show the accessibility (private, public, etc.)
		/// </summary>
		ShowAccessibility      = 4,
		/// <summary>
		/// Show the definition key word (class, struct, Sub, Function, etc.)
		/// </summary>
		ShowDefinitionKeyWord  = 8,
		/// <summary>
		/// Show the fully qualified name for the member
		/// </summary>
		UseFullyQualifiedMemberNames = 0x10,
		/// <summary>
		/// Show modifiers (virtual, override, etc.)
		/// </summary>
		ShowModifiers          = 0x20,
		/// <summary>
		/// Show the return type
		/// </summary>
		ShowReturnType = 0x100,
		/// <summary>
		/// Use fully qualified names for return type and parameters.
		/// </summary>
		UseFullyQualifiedTypeNames = 0x200,
		/// <summary>
		/// Show the list of type parameters on method and class declarations.
		/// Type arguments for parameter/return types are always shown.
		/// </summary>
		ShowTypeParameterList = 0x800,
		
		StandardConversionFlags = ShowParameterNames |
			ShowAccessibility |
			ShowParameterList |
			ShowReturnType |
			ShowModifiers |
			ShowTypeParameterList |
			ShowDefinitionKeyWord,
		
		All = 0xfff,
	}
	
	public interface IAmbience
	{
		ConversionFlags ConversionFlags {
			get;
			set;
		}
		
		string ConvertEntity(IEntity e);
		string ConvertType(IType type);
		string ConvertVariable(IVariable variable);
		
		string WrapAttribute(string attribute);
		string WrapComment(string comment);
		
	}
}
