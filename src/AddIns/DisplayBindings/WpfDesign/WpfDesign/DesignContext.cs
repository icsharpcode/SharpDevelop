// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3509 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.IO;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// The context that the designer uses.
	/// </summary>
	public abstract class DesignContext : ServiceContainer
	{
		/// <summary>
		/// Creates a new DesignContext instance.
		/// </summary>
		protected DesignContext()
		{
			AddService(typeof(ExtensionManager), new ExtensionManager(this));
		}

		public bool ParseSuggested { get; set; }

		public abstract void Parse(string text);

		/// <summary>
		/// Gets whether the design context can be saved.
		/// </summary>
		public abstract bool CanSave { get; }

		/// <summary>
		/// Save the designed elements to stream.
		/// </summary>
		public abstract string Save();

		/// <summary>
		/// Gets the <see cref="ISelectionService"/>.
		/// Throws an exception if the service is not found.
		/// </summary>
		public ISelectionService SelectionService
		{
			get
			{
				return GetServiceOrThrowException<ISelectionService>();
			}
		}

		/// <summary>
		/// Gets the <see cref="IToolService"/>.
		/// Throws an exception if the service is not found.
		/// </summary>
		public IToolService ToolService
		{
			get
			{
				return GetServiceOrThrowException<IToolService>();
			}
		}

		/// <summary>
		/// Gets the <see cref="ViewService"/>.
		/// Throws an exception if the service is not found.
		/// </summary>
		public IViewService ViewService
		{
			get
			{
				return GetServiceOrThrowException<IViewService>();
			}
		}

		/// <summary>
		/// Gets the <see cref="ExtensionManager"/>.
		/// Throws an exception if the service is not found.
		/// </summary>
		public ExtensionManager ExtensionManager
		{
			get
			{
				return GetServiceOrThrowException<ExtensionManager>();
			}
		}

		/// <summary>
		/// Gets the <see cref="IDesignPanel"/>.
		/// Throws an exception if the service is not found.
		/// </summary>
		public IDesignPanel DesignPanel
		{
			get
			{
				return GetServiceOrThrowException<IDesignPanel>();
			}
		}

		/// <summary>
		/// Gets the <see cref="IUndoService"/>.
		/// Throws an exception if the service is not found.
		/// </summary>
		public IUndoService UndoService
		{
			get
			{
				return GetServiceOrThrowException<IUndoService>();
			}
		}

		/// <summary>
		/// Gets the <see cref="IModelService"/>.
		/// Throws an exception if the service is not found.
		/// </summary>
		public IModelService ModelService
		{
			get
			{
				return GetServiceOrThrowException<IModelService>();
			}
		}

		/// <summary>
		/// Gets the <see cref="ICommandService"/>.
		/// Throws an exception if the service is not found.
		/// </summary>
		public ICommandService CommandService
		{
			get
			{
				return GetServiceOrThrowException<ICommandService>();
			}
		}
	}
}
