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
using System.CodeDom;
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.FormsDesigner.Services
{
	/// <summary>
	/// Supports project-level resources in the Windows.Forms designer.
	/// </summary>
	public sealed class ProjectResourceService
	{
		public const string ProjectResourceKey = "SDProjectResource_";
		
		IProject project;
		string stringLiteralDelimiter;
		bool designerSupportsProjectResources = true;
		
		public ProjectResourceService(IProject project)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			this.project = project;
		}
		
		public IProject ProjectContent {
			get { return project; }
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				if (this.project != value) {
					this.project = value;
					this.stringLiteralDelimiter = null;
				}
			}
		}
		
		public bool DesignerSupportsProjectResources {
			get { return designerSupportsProjectResources; }
			set { designerSupportsProjectResources = value; }
		}
		
		/// <summary>
		/// Gets the string literal delimiter for the current language
		/// by generating code for a known literal.
		/// </summary>
		string StringLiteralDelimiter {
			get {
				if (stringLiteralDelimiter == null) {
					const string TestString = "A";
					string testCode = project.GetAmbience().ConvertConstantValue(TestString);
					stringLiteralDelimiter = testCode.Substring(0, testCode.IndexOf(TestString, StringComparison.Ordinal));
				}
				return stringLiteralDelimiter;
			}
		}
		
		/// <summary>
		/// Gets the project resource from the specified expression.
		/// </summary>
		public ProjectResourceInfo GetProjectResource(CodePropertyReferenceExpression propRef)
		{
			CodeTypeReferenceExpression typeRef = propRef.TargetObject as CodeTypeReferenceExpression;
			if (typeRef == null) {
				LoggingService.Info("Target of possible project resources property reference is not a type reference, but " + propRef.TargetObject.ToString() + ".");
				return null;
			}
			
			ICompilation compilation = SD.ParserService.GetCompilation(project);
			
			// Get the (generated) class where the resource is defined.
			var resourceClass = compilation.FindType(new FullTypeName(typeRef.Type.BaseType)).GetDefinition();
			if (resourceClass == null) {
				throw new InvalidOperationException("Could not find class for project resources: '" + typeRef.Type.BaseType + "'.");
			}
			
			if (resourceClass.Region.IsEmpty)
				return null;
			
			// Make sure the class we have found is a generated resource class.
			if (!IsGeneratedResourceClass(resourceClass)) {
				return null;
			}
			
			// Get the name of the resource file based on the file that contains the generated class.
			string resourceFileName = Path.GetFileNameWithoutExtension(resourceClass.Region.FileName);
			if (resourceFileName.EndsWith("Designer", StringComparison.OrdinalIgnoreCase)) {
				resourceFileName = Path.GetFileNameWithoutExtension(resourceFileName);
			}
			resourceFileName = Path.Combine(Path.GetDirectoryName(resourceClass.Region.FileName), resourceFileName);
			if (File.Exists(resourceFileName + ".resources")) {
				resourceFileName = resourceFileName + ".resources";
			} else if (File.Exists(resourceFileName + ".resx")) {
				resourceFileName = resourceFileName + ".resx";
			} else {
				throw new FileNotFoundException("Could not find the resource file for type '" + resourceClass.FullName + "'. Tried these file names: '" + resourceFileName + ".(resources|resx)'.");
			}
			
			// Get the property for the resource.
			IProperty prop = resourceClass.Properties.SingleOrDefault(p => p.Name == propRef.PropertyName);
			if (prop == null) {
				throw new InvalidOperationException("Property '" + propRef.PropertyName + "' not found in type '" + resourceClass.FullName + "'.");
			}
			
			if (!prop.CanGet) {
				throw new InvalidOperationException("Property '" + propRef.PropertyName + "' in type '" + resourceClass.FullName + "' does not have a getter.");
			}
			
			// Get the code of the resource class and
			// extract the resource key from the property.
			// This is necessary because the key may differ from the property name
			// if special characters are used.
			// It would be better if we could use a real code parser for this, but
			// that is not possible without getting dependent on the programming language.
			
			IDocument doc = new ReadOnlyDocument(SD.FileService.GetFileContent(resourceClass.Region.FileName));
			
			int startOffset = doc.PositionToOffset(prop.Getter.BodyRegion.BeginLine, prop.Getter.BodyRegion.BeginColumn);
			int endOffset   = doc.PositionToOffset(prop.Getter.BodyRegion.EndLine, prop.Getter.BodyRegion.EndColumn);
			
			string code = doc.GetText(startOffset, endOffset - startOffset + 1);
			
			int index = code.IndexOf("ResourceManager", StringComparison.Ordinal);
			if (index == -1) {
				throw new InvalidOperationException("No reference to ResourceManager found in property getter of '" + prop.FullName + "'. Code: '" + code + "'");
			}
			
			index = code.IndexOf("Get", index, StringComparison.Ordinal);
			if (index == -1) {
				throw new InvalidOperationException("No call to Get... found in property getter of '" + prop.FullName + "'. Code: '" + code + "'");
			}
			
			index = code.IndexOf(this.StringLiteralDelimiter, index + 1, StringComparison.Ordinal);
			if (index == -1) {
				throw new InvalidOperationException("No string delimiter ('" + this.StringLiteralDelimiter + "') found in property getter of '" + prop.FullName + "'. Code: '" + code + "'");
			}
			index += this.StringLiteralDelimiter.Length;
			
			int endIndex = code.LastIndexOf(this.StringLiteralDelimiter, StringComparison.Ordinal);
			if (endIndex == -1) {
				throw new InvalidOperationException("No string terminator ('" + this.StringLiteralDelimiter + "') found in property getter of '" + prop.FullName + "'. Code: '" + code + "'");
			}
			
			string resourceKey = code.Substring(index, endIndex - index);
			LoggingService.Debug("-> Decoded resource: In: " + resourceFileName + ". Key: " + resourceKey);
			
			return new ProjectResourceInfo(new FileName(resourceFileName), resourceKey);
		}
		
		/// <summary>
		/// Determines whether the specified class is a generated resource
		/// class, based on the attached attributes.
		/// </summary>
		public static bool IsGeneratedResourceClass(ITypeDefinition @class)
		{
			IAttribute att = @class.GetAttribute(new TopLevelTypeName("System.CodeDom.Compiler", "GeneratedCodeAttribute"), false);
			return att != null &&
				att.PositionalArguments.Count == 2 &&
				String.Equals("System.Resources.Tools.StronglyTypedResourceBuilder", att.PositionalArguments[0].ConstantValue as string, StringComparison.Ordinal);
		}
	}
}
