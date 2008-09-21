// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email="chhornung@googlemail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Reflector.CodeModel;

namespace Reflector.IpcServer.AddIn
{
	/// <summary>
	/// Provides the ability to remote-control Reflector.
	/// </summary>
	public sealed class ReflectorService : MarshalByRefObject, IReflectorService, IDisposable
	{
		readonly IServiceProvider serviceProvider;
		readonly Dictionary<string, DateTime> assemblyLastWriteTimes = new Dictionary<string, DateTime>(StringComparer.InvariantCultureIgnoreCase);
		
		DateTime lastAssemblyLoadedAt = DateTime.MinValue;
		
		public ReflectorService(IServiceProvider serviceProvider)
		{
			if (serviceProvider == null) throw new ArgumentNullException("serviceProvider");
			this.serviceProvider = serviceProvider;
			this.AssemblyManager.AssemblyLoaded += this.AssemblyLoaded;
		}
		
		public void Dispose()
		{
			this.AssemblyManager.AssemblyLoaded -= this.AssemblyLoaded;
			this.assemblyLastWriteTimes.Clear();
		}
		
		/// <summary>
		/// Used to test the remoting connection without doing any action.
		/// </summary>
		/// <returns><c>true</c>.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public bool CheckIsThere()
		{
			return true;
		}
		
		public bool IsReady {
			get {
				return this.lastAssemblyLoadedAt != DateTime.MinValue &&
					(DateTime.UtcNow - this.lastAssemblyLoadedAt).TotalSeconds > 1;
			}
		}
		
		void AssemblyLoaded(object sender, EventArgs e)
		{
			this.lastAssemblyLoadedAt = DateTime.UtcNow;
		}
		
		// ********************************************************************************************************************************
		
		T GetService<T>()
		{
			return (T)serviceProvider.GetService(typeof(T));
		}
		
		IAssemblyBrowser AssemblyBrowser {
			get { return GetService<IAssemblyBrowser>(); }
		}
		
		IAssemblyManager AssemblyManager {
			get { return GetService<IAssemblyManager>(); }
		}
		
		IWindowManager WindowManager {
			get { return GetService<IWindowManager>(); }
		}
		
		/// <summary>
		/// Positions the assembly browser of Reflector on the element
		/// that best matches the parameters contained in the <paramref name="element"/> parameter
		/// and brings the Reflector window to front if at least the specified assembly was found.
		/// </summary>
		/// <param name="element">A <see cref="CodeElementInfo"/> object that describes the element to go to.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="element"/> parameter is <c>null</c>.</exception>
		public void GoTo(CodeElementInfo element)
		{
			if (element == null) throw new ArgumentNullException("element");
			if (!this.IsReady) throw new InvalidOperationException("The service is not ready.");
			WindowManager.Content.BeginInvoke(new Action<CodeElementInfo>(GoToInternal),
			                                  new object[] { element });
		}
		
		void GoToInternal(CodeElementInfo element)
		{
			if (element == null) throw new ArgumentNullException("element");
			
			IAssembly asm = AssemblyManager.LoadFile(element.AssemblyLocation);
			if (asm == null) return;
			
			// Force reload of the assembly if it has changed since start of Service
			DateTime tNew = GetLastWriteTimeSafe(element.AssemblyLocation);
			DateTime tOld;
			if (this.assemblyLastWriteTimes.TryGetValue(element.AssemblyLocation, out tOld)) {
				if (!tNew.Equals(tOld)) {
					AssemblyManager.Unload(asm);
					asm = AssemblyManager.LoadFile(element.AssemblyLocation);
					if (asm == null) return;
				}
			}
			
			this.assemblyLastWriteTimes[element.AssemblyLocation] = tNew;
			
			
			AssemblyBrowser.ActiveItem = (new CodeElementFinder(element)).Find(asm);
			
			WindowManager.Activate();
			Form topForm = WindowManager.Content.FindForm();
			if (topForm != null) {
				// Force the Reflector window to the top
				if (topForm.WindowState == FormWindowState.Minimized) {
					topForm.WindowState = FormWindowState.Normal;
				}
				topForm.TopMost = true;
				topForm.TopMost = false;
			}
		}
		
		static DateTime GetLastWriteTimeSafe(string path)
		{
			try {
				return System.IO.File.GetLastWriteTimeUtc(path);
			} catch (UnauthorizedAccessException) {
				return DateTime.UtcNow;
			} catch (System.IO.IOException) {
				return DateTime.UtcNow;
			} catch (NotSupportedException) {
				return DateTime.UtcNow;
			}
		}
	}
}
