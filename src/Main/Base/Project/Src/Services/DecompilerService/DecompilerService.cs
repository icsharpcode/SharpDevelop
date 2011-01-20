// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;

using Mono.Cecil;

namespace Services.DecompilerService
{
	public static class DecompilerService
	{
		public static void ReadMetadata(TypeDefinition type, out string filePath)
		{
			if (type == null)
			{
				filePath = null;
				return;
			}

			StringBuilder sb = new StringBuilder();

			sb.Append("using System;");

			sb.Append(Environment.NewLine);
			sb.Append(Environment.NewLine);

			sb.Append("namespace " + type.Namespace);
			sb.Append(Environment.NewLine);
			sb.Append("{");
			sb.Append(Environment.NewLine);

			// attributes
			if (type.HasCustomAttributes) {
				foreach (var attr in type.CustomAttributes) {
					sb.Append("\t[");
					sb.Append(attr.AttributeType.Name);
					sb.Append("]");
					sb.Append(Environment.NewLine);
				}
			}

			// main definition
			sb.Append("\tpublic ");

			if (type.IsValueType)
				sb.Append("struct ");
			else
				if (type.IsEnum)
					sb.Append("enum ");
			else {
				if (type.IsClass) {
					if (type.IsAbstract)
						sb.Append("abstract ");
					if (type.IsSealed)
						sb.Append("sealed ");
					sb.Append("class ");
				}

				if (type.IsInterface)
					sb.Append("interface ");
			}

			sb.Append(type.Name);

			// inheritance
			if (type.BaseType != null) {
				sb.Append(" : ");
				sb.Append(type.BaseType.Name);
			}

			sb.Append(Environment.NewLine);
			sb.Append("\t{");
			sb.Append(Environment.NewLine);

			if (type.HasEvents) {
				sb.Append("\t");sb.Append("\t");sb.Append("// Public Events");
				sb.Append(Environment.NewLine);

				foreach (var ev in type.Events) {
					sb.Append("\t");sb.Append("\t");
					sb.Append("public event ");sb.Append(ev.EventType.FullName);sb.Append(" ");sb.Append(ev.Name);
					sb.Append(Environment.NewLine);
					sb.Append(Environment.NewLine);
				}
			}

			if (type.HasProperties) {
				sb.Append("\t");sb.Append("\t");sb.Append("// Public Properties");
				sb.Append(Environment.NewLine);

				foreach (var property in type.Properties) {
					
					if (property.GetMethod != null && !property.GetMethod.IsPublic &&
						property.SetMethod != null && !property.SetMethod.IsPublic)
						continue;
					
					sb.Append("\t");sb.Append("\t");
					sb.Append("public ");sb.Append(property.PropertyType.FullName);sb.Append(" ");sb.Append(property.Name);
					sb.Append(" {");
					if (property.GetMethod != null && property.GetMethod.IsPublic)
						sb.Append(" get;");
					if (property.SetMethod != null && property.SetMethod.IsPublic)
						sb.Append(" set;");
					sb.Append(" }");
					sb.Append(Environment.NewLine);
					sb.Append(Environment.NewLine);
				}
			}

			if (type.HasMethods) {
				sb.Append("\t");sb.Append("\t");sb.Append("// Public Methods");
				sb.Append(Environment.NewLine);

				foreach (var method in type.Methods) {

					if (method.IsPrivate) continue;

					if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_")) continue;

					sb.Append("\t");sb.Append("\t");
					sb.Append("public ");
					if (method.IsStatic)
						sb.Append("static ");

					if (!method.IsConstructor) {
						sb.Append(method.ReturnType.FullName);sb.Append(" ");sb.Append(method.Name);
					} else {
						sb.Append(type.Name);
					}

					sb.Append(GetParameters(method));

					sb.Append(";");
					sb.Append(Environment.NewLine);
					sb.Append(Environment.NewLine);
				}
			}

			sb.Append("\t}");
			sb.Append(Environment.NewLine);
			sb.Append("}");

			// temp file
			string tempFolder = Path.GetTempPath();
			string file = type.Name + ".temp.cs";
			filePath = Path.Combine(tempFolder, file);

			using (StreamWriter sr = new StreamWriter(filePath)) {
				sr.Write(sb.ToString());
			}
		}

		public static string GetParameters(MethodDefinition method)
		{
			StringBuilder sb = new StringBuilder();

			if (!method.HasParameters)
				sb.Append("()");
			else {
				sb.Append("(");
				for (int i = 0 ; i < method.Parameters.Count; ++i) {
					var p = method.Parameters[i];

					if (p.IsOut)
						sb.Append("out ");
					else
						if (p.ParameterType.IsByReference)
							sb.Append("ref ");

					sb.Append(p.ParameterType.Name.Replace("&", string.Empty));
					sb.Append(" ");

					sb.Append(p.Name);

					if (i < method.Parameters.Count - 1)
						sb.Append(", ");
				}
				sb.Append(")");
			}

			return sb.ToString();
		}
	}
}
