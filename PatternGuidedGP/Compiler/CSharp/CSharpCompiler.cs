using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PatternGuidedGP.Pangea;

namespace PatternGuidedGP.Compiler.CSharp {
	class CSharpCompiler : ICompiler {
		private IEnumerable<MetadataReference> _references;

		public CSharpCompiler() {
			_references = GetAssemblyReferences();
		}

		public Assembly Compile(CompilationUnitSyntax syntax) {
			string assemblyName = Guid.NewGuid().ToString();
			var syntaxTree = CSharpSyntaxTree.Create(syntax);
			var compilation = CSharpCompilation.Create(
				assemblyName,
				new[] { syntaxTree },
				_references,
				new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

			using (var ms = new MemoryStream()) {
				var compilationResult = compilation.Emit(ms);
				if (compilationResult.Success) {
					Assembly assembly = Assembly.Load(ms.GetBuffer());
					return assembly;

				} else {
					foreach (var error in compilationResult.Diagnostics) {
						Console.WriteLine(error.ToString());
					}
				}
				return null;
			}
		}

		private static IEnumerable<MetadataReference> GetAssemblyReferences() {
			var references = new MetadataReference[] {
				MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
				MetadataReference.CreateFromFile(typeof(ExecutionTraces).GetTypeInfo().Assembly.Location)
			};
			return references;
		}
	}
}
