// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

using NoGoop.ObjBrowser;
using NoGoop.Util;

namespace NoGoop.Obj
{
	public class StructAccessor
	{
		// K(string of objtype_structFields(s)_memberField) 
		// V(StructAccessor)
		protected static Hashtable              _accessors = Hashtable.Synchronized(new Hashtable());


		protected static int                    _classSeq;

		protected MethodInfo                    _getMethod;
		protected MethodInfo                    _setMethod;

		protected Object                        _accessorObj;
	
		// The key for this accessor obj
		protected String                        _key;

		// The fields defineing how you get from the top level
		// object to the field used by this accessor.
		protected FieldInfo[]                   _structFields;

		// This generates a class that contains a method to get
		// a value from an inline value type, and a class to set
		// a value.
		// objType is the type of object that contains the field
		// of the struct.
		// structFields array contains each field that refers
		// to a struct within its container, element[0] is contained
		// by the object of objType.
		protected void MakeAccessorClass(String key,
										 Type objType,
										 FieldInfo[] structFields)
		{
			TypeBuilder tb;

			_key = key;
			_structFields = structFields;

			int structFieldsLast = structFields.Length - 1;
			FieldInfo memberField = 
				structFields[structFieldsLast];

			if (TraceUtil.If(this, TraceLevel.Info))
			{
				TraceUtil.WriteLineInfo
					(typeof(StructAccessor),
					 "MakeAccessor: " + objType);
			}
								
			lock (typeof(StructAccessor))
			{
				tb = AssemblySupport.ModBuilder.
					DefineType("GeneratedStructAccessor" + _classSeq++,
							   TypeAttributes.Class
							   | TypeAttributes.Public);
				tb.DefineDefaultConstructor(MethodAttributes.Public);
			}

			MethodBuilder mb;
			ILGenerator gen;

			MethodInfo getTypeMethod = 
				typeof(Type).GetMethod("GetTypeFromHandle");
			MethodInfo getFieldMethod = 
				typeof(Type).GetMethod("GetField", 
									   new Type[] { typeof(String),
													typeof(BindingFlags) });
			//MethodInfo getFieldMethod = 
			//    typeof(FieldInfo).GetMethod("GetFieldFromHandle");
			MethodInfo getValueMethod = 
				typeof(FieldInfo).GetMethod("GetValue");

			// Get method
			// param 1 - object to read
			// return - read value
			mb = tb.DefineMethod("GetValue", MethodAttributes.Public,
								 typeof(Object), 
								 new Type[] { typeof(Object) });
			gen = mb.GetILGenerator();

			// Add the field infos to the stack in reverse order
			for (int i = structFieldsLast; i >= 0; i--)
			{
				gen.Emit(OpCodes.Ldtoken, structFields[i].DeclaringType);
				gen.Emit(OpCodes.Call, getTypeMethod);
				gen.Emit(OpCodes.Ldstr, structFields[i].Name);
				gen.Emit(OpCodes.Ldc_I4, (int)ReflectionHelper.ALL_BINDINGS);
				gen.Emit(OpCodes.Callvirt, getFieldMethod);
			}

			// Load object to update
			gen.Emit(OpCodes.Ldarg_1);

			// Get the value for each field info
			for (int i = structFieldsLast; i >= 0; i--)
				gen.Emit(OpCodes.Callvirt, getValueMethod);

			// Box if this is a value type
			if (memberField.FieldType.IsValueType)
				gen.Emit(OpCodes.Box, memberField.FieldType);


			/***
				old way - not using reflection

			// Load object to update
			gen.Emit(OpCodes.Ldarg_1);

			// Cast object to correct type
			gen.Emit(OpCodes.Castclass, objType);

			// Get address of the struct field
			// don't look at the last field
			for (int i = 0; i < structFieldsLast; i++)
				gen.Emit(OpCodes.Ldflda, structFields[i]);

			// Load field value 
			gen.Emit(OpCodes.Ldfld, memberField);

			// Unbox if this is a value type
			if (memberField.FieldType.IsValueType)
				gen.Emit(OpCodes.Box, memberField.FieldType);

			***/

			gen.Emit(OpCodes.Ret);


			// Set method
			// param 1 - object to update
			// param 2 - value to set it to
			mb = tb.DefineMethod("SetValue", MethodAttributes.Public,
								 null, 
								 new Type[] { typeof(Object),
											  typeof(Object) });
			gen = mb.GetILGenerator();

			// Load object to update
			gen.Emit(OpCodes.Ldarg_1);

			// Cast object to correct type
			gen.Emit(OpCodes.Castclass, objType);

			// Get address of the struct field
			// don't look at the last field
			for (int i = 0; i < structFieldsLast; i++)
				gen.Emit(OpCodes.Ldflda, structFields[i]);

			// Load value 
			gen.Emit(OpCodes.Ldarg_2);

			// Unbox if this is a value type
			if (memberField.FieldType.IsValueType)
				gen.Emit(OpCodes.Unbox, memberField.FieldType);

			// Load indirect (the value?)
			gen.Emit(OpCodes.Ldind_I4);

			// Store the value into the field
			gen.Emit(OpCodes.Stfld, memberField);

			gen.Emit(OpCodes.Ret);


			Type type = tb.CreateType();
			MethodInfo[] methods = type.
				GetMethods(BindingFlags.Public
						   | BindingFlags.Instance
						   | BindingFlags.DeclaredOnly);
			// First method is get, second one set, do this to
			// avoid using the name so that we don't have a problem with the
			// obfuscator
			_getMethod = methods[0];
			_setMethod = methods[1];
			_accessorObj = Activator.CreateInstance(type);

			// Only for testing
			//AssemblySupport.SaveBuiltAssy();
		}


		// fieldInfo - the field who's value we want
		// objNode - the member node corresponding to the field
		// out topLevelObject - the object that contains the highest level
		//  struct.
		// s/b protected, stupid compiler
		internal static StructAccessor GetAccessorObject
			(IObjectNode objNode,
			 out Object topLevelObject)
		{
			Stack structInfoStack = new Stack();
			IObjectNode parentObjNode;

			if (TraceUtil.If(typeof(StructAccessor), TraceLevel.Info))
			{
				TraceUtil.WriteLineInfo
					(typeof(StructAccessor),
					 "GetAccessor: " + objNode.ObjectInfo.ToString()
					 + " node: " + objNode.ToString());
			}
								

			// Build up the structFields array, the fields between the
			// top level object and the object we want to change
			while (true)
			{
				parentObjNode = objNode.ParentObjectNode;
				if (parentObjNode == null)
				{
					throw new Exception("(bug) hit no parent when "
										+ "looking for enclosing object "
										+ "for a struct " + objNode);
				}

				// This is an enclosing struct, push the field 
				// information of how to get to the child here
				FieldInfo fi = parentObjNode.ObjType.GetField
					(objNode.ObjectInfo.ObjMemberInfo.Name,
					 ReflectionHelper.ALL_BINDINGS);
				if (fi == null)
				{
					throw new Exception("(bug) field " 
										+ objNode.ObjType.Name 
										+ " not found in " + parentObjNode);
				}

				structInfoStack.Push(fi);

				// Found the containing object, either not a value
				// type, or this is not a field which means it can't
				// be inline any more
				// FIXME - double check the member thing here, is this
				// the right level to check?
				if (!parentObjNode.ObjType.IsValueType ||
					!(parentObjNode.ObjectInfo.ObjMemberInfo is FieldInfo))
					break;

				// Go up
				objNode = parentObjNode;
			}

			// Ok, at this point, parent is the ObjectInfo for the
			// object that contains the top-level struct
			Type objType = parentObjNode.ObjType;
			topLevelObject = parentObjNode.Obj;

			if (TraceUtil.If(typeof(StructAccessor), TraceLevel.Info))
			{
				TraceUtil.WriteLineInfo
					(typeof(StructAccessor),
					 "GetAccessor - top level type/obj: " 
					 + objType + "/" + topLevelObject);
			}
								
			int count = structInfoStack.Count;
			FieldInfo[] structFields = 
				new FieldInfo[structInfoStack.Count];
			for (int i = 0; i < count; i++)
				structFields[i] = (FieldInfo)structInfoStack.Pop();

			StructAccessor vtAccessor;
			StringBuilder keyBuilder = new StringBuilder();
			String key;
			
			keyBuilder.Append(objType.FullName);
			keyBuilder.Append(".");
			foreach (FieldInfo fi in structFields)
			{
				keyBuilder.Append(fi.Name);
				keyBuilder.Append(".");
			}
			key = keyBuilder.ToString();

			if (TraceUtil.If(typeof(StructAccessor), TraceLevel.Info))
			{
				TraceUtil.WriteLineInfo
					(typeof(StructAccessor),
					 "GetAccessor - key: " + key);
			}
								
			lock (typeof(StructAccessor))
			{
				vtAccessor = (StructAccessor)_accessors[key];
				if (vtAccessor != null)
					return vtAccessor;

				vtAccessor = new StructAccessor();
				vtAccessor.MakeAccessorClass(key,
											 objType,
											 structFields);
				_accessors.Add(key, vtAccessor);
				return vtAccessor;
			}
		}


		// fieldInfo - the field who's value we want
		// objNode - the member node corresponding to the field
		// out topLevelObject - the object that contains the highest level
		//  struct.
		// s/b protected, stupid compiler
		internal static FieldInfo[] GetStructFields
			(IObjectNode objNode,
			 out Object topLevelObject)
		{
			Stack structInfoStack = new Stack();
			IObjectNode parentObjNode;

			if (TraceUtil.If(typeof(StructAccessor), TraceLevel.Info))
			{
				TraceUtil.WriteLineInfo
					(typeof(StructAccessor),
					 "GetAccessor: " + objNode.ObjectInfo.ToString()
					 + " node: " + objNode.ToString());
			}
								

			// Build up the structFields array, the fields between the
			// top level object and the object we want to change
			while (true)
			{
				parentObjNode = objNode.ParentObjectNode;
				if (parentObjNode == null)
				{
					throw new Exception("(bug) hit no parent when "
										+ "looking for enclosing object "
										+ "for a struct " + objNode);
				}

				// This is an enclosing struct, push the field 
				// information of how to get to the child here
				FieldInfo fi = parentObjNode.ObjType.GetField
					(objNode.ObjectInfo.ObjMemberInfo.Name,
					 ReflectionHelper.ALL_BINDINGS);
				if (fi == null)
				{
					throw new Exception("(bug) field " 
										+ objNode.ObjType.Name 
										+ " not found in " + parentObjNode);
				}

				structInfoStack.Push(fi);

				// Found the containing object, either not a value
				// type, or this is not a field which means it can't
				// be inline any more
				if (!parentObjNode.ObjType.IsValueType ||
					!(parentObjNode.ObjectInfo.ObjMemberInfo is FieldInfo))
					break;

				// Go up
				objNode = parentObjNode;
			}

			// Ok, at this point, parent is the ObjectInfo for the
			// object that contains the top-level struct
			topLevelObject = parentObjNode.Obj;

			if (TraceUtil.If(typeof(StructAccessor), TraceLevel.Info))
			{
				TraceUtil.WriteLineInfo
					(typeof(StructAccessor),
					 "GetAccessor - top level obj: " 
					 + topLevelObject);
			}
								
			int count = structInfoStack.Count;
			FieldInfo[] structFields = 
				new FieldInfo[structInfoStack.Count];
			for (int i = 0; i < count; i++)
				structFields[i] = (FieldInfo)structInfoStack.Pop();
			return structFields;
		}


		protected static void SetValue(FieldInfo[] structFields,
									   Object obj, 
									   Object value,
									   int index)
		{

			if (TraceUtil.If(typeof(StructAccessor), TraceLevel.Verbose))
			{
				Trace.WriteLine("Struct setting Field: "
								+ structFields[index]
								+ " obj: " + obj
								+ " type: " + obj.GetType()
								+ " value: " + value
								+ " index: " + index);
			}

			if (index == structFields.Length - 1)
			{
				structFields[index].SetValue(obj, value);
				return;
			}

			Object temp = structFields[index].GetValue(obj);
			SetValue(structFields, temp, value, index + 1);
			structFields[index].SetValue(obj, temp);
		}


		//
		//  The methods below are the external interface to this class
		// 

		// Gets the value for this field within the specified
		// object into at the specified member node.
		internal static Object GetValue(IObjectNode objNode)
		{
			Object obj;
			FieldInfo[] structFields = 
				GetStructFields(objNode, out obj);

			Object value = obj;
			for (int i = 0; i < structFields.Length; i++)
				value = structFields[i].GetValue(value);

			if (TraceUtil.If(typeof(StructAccessor), TraceLevel.Verbose))
			{
				Trace.WriteLine("Struct getting Field: " + objNode
								+ " value: " + value);
			}
			return value;
		}


		// Sets the value in the same manner as GetValue
		internal static void SetValue(IObjectNode objNode,
									  Object value)
		{
			if (TraceUtil.If(typeof(StructAccessor), TraceLevel.Verbose))
			{
				Trace.WriteLine("Struct setting Field: " + objNode
								+ " value: " + value);
			}

			Object obj;
			FieldInfo[] structFields = 
				GetStructFields(objNode, out obj);
			SetValue(structFields, obj, value, 0);
			objNode.DoDisplayValue();
		}
	}
}
