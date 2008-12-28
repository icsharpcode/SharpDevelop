// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Security.Permissions;

using ICSharpCode.FormsDesigner;
using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Loads the form or control's code so the forms designer can
	/// display it.
	/// </summary>
	[PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
	[PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
	public class PythonDesignerLoader : BasicDesignerLoader, IComponentCreator
	{
		IPythonDesignerGenerator generator;
		
		public PythonDesignerLoader(IPythonDesignerGenerator generator)
		{
			if (generator == null) {
				throw new ArgumentException("Generator cannot be null.", "generator");
			}
			this.generator = generator;
		}
		
		public override void BeginLoad(IDesignerLoaderHost host)
		{
			host.AddService(typeof(ComponentSerializationService), new CodeDomComponentSerializationService((IServiceProvider)host));
			host.AddService(typeof(INameCreationService), new XmlDesignerLoader.NameCreationService(host));
			host.AddService(typeof(IDesignerSerializationService), new DesignerSerializationService(host));

			base.BeginLoad(host);
		}
		
		/// <summary>
		/// Creates a component of the specified type.
		/// </summary>
		public IComponent CreateComponent(Type componentClass, string name)
		{
			return base.LoaderHost.CreateComponent(componentClass, name);
		}
		
		/// <summary>
		/// Passes the designer host's root component to the generator so it can update the
		/// source code with changes made at design time.
		/// </summary>
		protected override void PerformFlush(IDesignerSerializationManager serializationManager)
		{
			generator.MergeRootComponentChanges(base.LoaderHost.RootComponent);
		}
		
		protected override void PerformLoad(IDesignerSerializationManager serializationManager)
		{
			// Create designer root object.
			PythonFormVisitor visitor = new PythonFormVisitor();
			visitor.CreateForm("abc", this);
		}		
	}
}
