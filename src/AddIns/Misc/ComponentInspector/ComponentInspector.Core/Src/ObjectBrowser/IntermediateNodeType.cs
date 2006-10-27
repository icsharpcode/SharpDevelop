// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

namespace NoGoop.ObjBrowser
{

	// Represents a type of intermediate tree node
	internal class IntermediateNodeType
	{

		protected int                   _typeId;
		protected String                _name;
		protected PresentationInfo      _presentationInfo;
		protected static ArrayList      _intermediateNodeTypes;

		internal int TypeId {
			get {
				return _typeId;
			}
		}

		internal String Name
		{
			get
				{
					return _name;
				}
		}

		internal PresentationInfo PresentationInfo
		{
			get
				{
					return _presentationInfo;
				}
			set
				{
					_presentationInfo = value;
				}
		}

		internal const int INTNODE_COM_ENUM =               0x0001;
		internal const int INTNODE_COM_STRUCT =             0x0002;
		internal const int INTNODE_COM_MODULE =             0x0004;
		internal const int INTNODE_COM_INTERFACE =          0x0008;
		internal const int INTNODE_COM_DISPINTERFACE =      0x0010;
		internal const int INTNODE_COM_COCLASS =            0x0020;
		internal const int INTNODE_COM_TYPEDEF =            0x0040;
		internal const int INTNODE_COM_UNION =              0x0080;

		internal const int INTNODE_BASECLASS =              0x0100;
		internal const int INTNODE_OVERLOAD =               0x0200;

		internal const int INTNODE_FIELD =                  0x0400;
		internal const int INTNODE_PROPERTY =               0x0800;
		internal const int INTNODE_METHOD =                 0x1000;
		internal const int INTNODE_EVENT =                  0x2000;

		internal const int INTNODE_COM_BASEINT =            0x4000;
		internal const int INTNODE_COM_EVENTINT =           0x8000;

		internal const int INTNODE_ENUM =                  0x10000;
		internal const int INTNODE_STRUCT =                0x10001;
		internal const int INTNODE_INTERFACE =             0x10002;
		internal const int INTNODE_CLASS =                 0x10004;

		static IntermediateNodeType()
		{
			_intermediateNodeTypes = new ArrayList();
			_intermediateNodeTypes.Add
				(new IntermediateNodeType(INTNODE_COM_ENUM,
										  "Enums"));
			_intermediateNodeTypes.Add
				(new IntermediateNodeType(INTNODE_COM_STRUCT,
										  "Structs"));
			_intermediateNodeTypes.Add
				(new IntermediateNodeType(INTNODE_COM_MODULE,
										  "Modules"));
			_intermediateNodeTypes.Add
				(new IntermediateNodeType(INTNODE_COM_INTERFACE,
										  "Interfaces"));
			_intermediateNodeTypes.Add
				(new IntermediateNodeType(INTNODE_COM_DISPINTERFACE,
										  "DispInterfaces"));
			_intermediateNodeTypes.Add
				(new IntermediateNodeType(INTNODE_COM_COCLASS,
										  "CoClasses"));
			_intermediateNodeTypes.Add
				(new IntermediateNodeType(INTNODE_COM_TYPEDEF,
										  "TypeDefs"));
			_intermediateNodeTypes.Add
				(new IntermediateNodeType(INTNODE_COM_UNION,
										  "Unions"));
			_intermediateNodeTypes.Add
				(new IntermediateNodeType(INTNODE_COM_BASEINT, 
										  "Inherited Interfaces"));
			_intermediateNodeTypes.Add
				(new IntermediateNodeType(INTNODE_COM_EVENTINT,
										  "Event Interfaces"));

			_intermediateNodeTypes.Add
				(new IntermediateNodeType(INTNODE_BASECLASS, 
										  "Base Class Members"));

			_intermediateNodeTypes.Add
				(new IntermediateNodeType(INTNODE_FIELD, "Fields"));
			_intermediateNodeTypes.Add
				(new IntermediateNodeType(INTNODE_PROPERTY, "Properties"));
			_intermediateNodeTypes.Add
				(new IntermediateNodeType(INTNODE_METHOD, "Methods"));
			_intermediateNodeTypes.Add
				(new IntermediateNodeType(INTNODE_EVENT, "Events"));

			_intermediateNodeTypes.Add
				(new IntermediateNodeType(INTNODE_ENUM,
										  "Enums"));
			_intermediateNodeTypes.Add
				(new IntermediateNodeType(INTNODE_STRUCT,
										  "Structs"));
			_intermediateNodeTypes.Add
				(new IntermediateNodeType(INTNODE_INTERFACE,
										  "Interfaces"));
			_intermediateNodeTypes.Add
				(new IntermediateNodeType(INTNODE_CLASS,
										  "Classes"));


		}


		internal IntermediateNodeType(int typeId,
									  String name)
		{
			_typeId = typeId;
			_name = name;
		}


		// Returns the intermediate node type supported by this node
		internal static IntermediateNodeType GetNodeType(int typeId)
		{
			foreach (IntermediateNodeType inType in _intermediateNodeTypes)
			{
				if (inType.TypeId == typeId)
					return inType;
			}
			return null;
		}


	}
}

