// <file>
//     <copyright see="prj:///doc/copyright.txt">2004 Rodrigo B. de Oliveira; 2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

namespace Boo.InterpreterAddIn

import System
import ICSharpCode.Core
import ICSharpCode.TextEditor
import ICSharpCode.TextEditor.Document
import ICSharpCode.TextEditor.Actions
import ICSharpCode.TextEditor.Gui.CompletionWindow
import Boo.Lang.Interpreter
import Boo.Lang.Compiler.TypeSystem
import ICSharpCode.SharpDevelop

internal class AbstractCompletionData(ICompletionData, IComparable):
	Priority as double:
		get:
			return 0
	
	_name as string
	
	def constructor(name):
		_name = name
	
	Text as string:
		get:
			return _name
		set:
			_name = value
	
	abstract ImageIndex as int:
		get:
			pass
	
	abstract Description as string:
		get:
			pass
	
	def InsertAction(textArea as TextArea, ch as char):
		textArea.InsertString(_name)
		return false
	
	public def CompareTo(obj) as int:
		if obj is null or not obj isa CodeCompletionData:
			return -1

		other = obj as CodeCompletionData
		return _name.CompareTo(other._name)

internal class CodeCompletionData(AbstractCompletionData):
	
	_entities as List = []
	
	def constructor(name):
		super(name)
	
	Description:
		get:
			description = InteractiveInterpreter.DescribeEntity(_entities[0])
			return description if 1 == len(_entities)
			return "${description} (+${len(_entities)-1} overloads)"
			
	ImageIndex as int:
		get:
			entity = _entities[0] as IEntity
			entityType = entity.EntityType
			if EntityType.Type == entityType:
				type as IType = entity
				if type.IsInterface:
					return ClassBrowserIconService.InterfaceIndex
				elif type.IsEnum:
					return ClassBrowserIconService.EnumIndex
				elif type.IsValueType:
					return ClassBrowserIconService.StructIndex
				elif type isa ICallableType:
					return ClassBrowserIconService.DelegateIndex
				else:
					return ClassBrowserIconService.ClassIndex
			elif EntityType.Method == entityType:
				return ClassBrowserIconService.MethodIndex
			elif EntityType.Field == entityType:
				if (entity as IField).IsLiteral:
					return ClassBrowserIconService.ConstIndex
				else:
					return ClassBrowserIconService.FieldIndex
			elif EntityType.Property == entityType:
				return ClassBrowserIconService.PropertyIndex
			elif EntityType.Event == entityType:
				return ClassBrowserIconService.EventIndex
			return ClassBrowserIconService.NamespaceIndex
		
	def AddEntity(entity as IEntity):
		_entities.Add(entity)

internal class GlobalsCompletionDataProvider(ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.AbstractCompletionDataProvider):
	_interpreter as InterpreterContext
	
	class GlobalCompletionData(AbstractCompletionData):
		
		[getter(ImageIndex)]
		_imageIndex as int
		
		[getter(Description)]
		_description as string
		
		def constructor(name as string, imageIndex as int, description as string):
			super(name)
			_imageIndex = imageIndex
			_description = description
	
	def constructor(interpreter as InterpreterContext):
		_interpreter = interpreter
		
	override def GenerateCompletionData(fileName as string, textArea as TextArea, charTyped as System.Char) as (ICompletionData):
		globals = _interpreter.GetGlobals()
		return array(ICompletionData, 0) if globals is null
		data = array(ICompletionData, len(globals))
		for index, key in enumerate(globals):
			value = null #_interpreter.GetValue(key) TODO
			delegate = value as System.Delegate
			if delegate is null:
				if value is not null:
					description = "${key} as ${InteractiveInterpreter.GetBooTypeName(value.GetType())}"
				else:
					description = "null"
				item = GlobalCompletionData(key, ClassBrowserIconService.FieldIndex, description)
			else:
				item = GlobalCompletionData(key, ClassBrowserIconService.MethodIndex, InteractiveInterpreter.DescribeMethod(delegate.Method))
			data[index] = item
		return data
			

internal class CodeCompletionDataProvider(ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.AbstractCompletionDataProvider):

	_codeCompletion as (IEntity)
	
	def constructor([required] codeCompletion):
		_codeCompletion = codeCompletion

	override def GenerateCompletionData(fileName as string, textArea as TextArea, charTyped as System.Char) as (ICompletionData):		
		values = {}
		for item in _codeCompletion:
			data as CodeCompletionData
			data = values[item.Name]
			if data is null:
				name = item.Name
				if "." in name:
					name = /\./.Split(name)[-1]			
				data = CodeCompletionData(name)
				values[item.Name] = data
			data.AddEntity(item)
		return array(ICompletionData, values.Values)
	
