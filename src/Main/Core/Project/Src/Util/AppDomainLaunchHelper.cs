/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 28.02.2006
 * Time: 18:03
 */

using System;
using System.Reflection;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Because AddIn assemblies are loaded into the LoadFrom context, creating AppDomains in them that
	/// use an arbitrary ApplicationBase path does not work correctly.
	/// This class contains a static method that helps launching a static method on a type in a new AppDomain.
	/// </summary>
	/// <example>
	///  <code>
	///  public static class CurrentClass { // is NOT MarshalByRef
	///   public static ResultClass[] GetResults()
	///   {
	///    AppDomainSetup setup = new AppDomainSetup();
	///    setup.ApplicationBase = myApplicationBase;
	///    AppDomain domain = AppDomain.CreateDomain("Display name for domain", AppDomain.CurrentDomain.Evidence, setup);
	///    try {
	///      return (ResultClass[])AppDomainLaunchHelper.LaunchInAppDomain(domain, typeof(CurrentClass), "GetResultsDirectly", requestObject);
	///    } finally {
	///      AppDomain.Unload(domain);
	///    }
	///   }
	///   public static ResultClass[] GetResultsDirectly(Request requestObject) { ... }
	///  }
	///  [Serializable] class Request { ... }  // must be serializable !!!
	///  [Serializable] class ResultClass { ... }  // must be serializable !!!
	/// </code></example>
	public class AppDomainLaunchHelper : MarshalByRefObject
	{
		public object LaunchMethod(string assemblyFile, string typeName, string methodName, object[] arguments)
		{
			Type t = Assembly.LoadFrom(assemblyFile).GetType(typeName);
			return t.InvokeMember(methodName, (BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod), null, null, arguments);
		}
		
		public static object LaunchInAppDomain(AppDomain domain, Type type, string methodName, params object[] arguments)
		{
			AppDomainLaunchHelper h = (AppDomainLaunchHelper)domain.CreateInstanceFromAndUnwrap(typeof(AppDomainLaunchHelper).Assembly.Location, typeof(AppDomainLaunchHelper).FullName);
			return h.LaunchMethod(type.Assembly.Location, type.FullName, methodName, arguments);
		}
	}
}
