using System;

namespace ICSharpCode.NRefactory.Parser.AST
{
	
	[Flags]
	public enum Modifier
	{
		// Access 
		Private   = 0x0001,
		Internal  = 0x0002, // == Friend
		Protected = 0x0004,
		Public    = 0x0008,
		Dim	      = 0x0010,	// VB.NET SPECIFIC
	
		// Scope
		Abstract  = 0x0010,  // == 	MustOverride/MustInherit
		Virtual   = 0x0020,
		Sealed    = 0x0040,
		Static    = 0x0080,
		Override  = 0x0100,
		Readonly  = 0x0200,
		Const	  = 0x0400,
		New       = 0x0800,  // == Shadows
		Partial   = 0x40000,
		
		// Special 
		Extern    = 0x1000,
		Volatile  = 0x2000,
		Unsafe    = 0x4000,
		Overloads = 0x8000, // VB specific
		WithEvents = 0x10000, // VB specific
		Default    = 0x20000, // VB specific
		// Modifier scopes
		None      = 0x0000,
		
		Classes                         = New | Public | Protected | Internal | Private | Abstract | Sealed | Partial | Static,
		VBModules						= Private | Public | Protected | Internal,
		VBStructures					= Private | Public | Protected | Internal | New,
		VBEnums						    = Private | Public | Protected | Internal | New,
		VBInterfacs					    = Private | Public | Protected | Internal | New,
		VBDelegates					    = Private | Public | Protected | Internal | New,
		VBMethods						= Private | Public | Protected | Internal | New | Static | Virtual | Sealed | Abstract | Override | Overloads,
		VBExternalMethods				= Private | Public | Protected | Internal | New | Overloads,
		VBEvents						= Private | Public | Protected | Internal | New | Overloads,
		VBProperties					= VBMethods | Default,
		
		// this is not documented in the spec
		VBInterfaceEvents				= New,
		VBInterfaceMethods				= New | Overloads,
		VBInterfaceProperties			= New | Overloads | /* ReadOnly | WriteOnly | */ Default,
		VBInterfaceEnums				= New,
		
		Fields                          = New | Public | Protected | Internal | Private | Static   | Readonly | Volatile,
		PropertysEventsMethods          = New | Public | Protected | Internal | Private | Static   | Virtual  | Sealed   | Override | Abstract | Extern,
		Indexers                        = New | Public | Protected | Internal | Private | Virtual  | Sealed   | Override | Abstract | Extern,
		Operators                       = Public | Static | Extern,
		Constants                       = New | Public | Protected | Internal | Private,
		StructsInterfacesEnumsDelegates = New | Public | Protected | Internal | Private | Partial,
		StaticConstructors              = Extern | Static | Unsafe,
		Destructors                     = Extern | Unsafe,
		Constructors                    = Public | Protected | Internal | Private | Extern,
		
		All       = Private  | Internal | Protected | Public |
		            Abstract | Virtual  | Sealed    | Static | Partial |
		            Override | Readonly | Const     | New    |
		            Extern   | Volatile | Unsafe    | Overloads | WithEvents
	}
	
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public enum Types
	{
		Class,
		Module,
		Interface,
		Struct,
		Enum
	}
	
	public enum ParentType
	{
		ClassOrStruct,
		InterfaceOrEnum,
		Namespace,
		Unknown
	}
	
	public enum FieldDirection {
		None,
		In,
		Out,
		Ref
	}
	
	public enum Members
	{
		Constant,
		Field,
		Method,
		Property,
		Event,
		Indexer,
		Operator,
		Constructor,
		StaticConstructor,
		Destructor,
		NestedType
	}
	
	[Flags]
	public enum ParamModifier
	{
		None = 0,
		In  = 1,
		Out = 2,
		Ref = 4,
		Params = 8,
		Optional = 16
	}
	
	
	
	
}
