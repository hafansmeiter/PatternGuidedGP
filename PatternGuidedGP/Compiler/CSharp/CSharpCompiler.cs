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
using PatternGuidedGP.Util;

namespace PatternGuidedGP.Compiler.CSharp {
	class CSharpCompiler : ICompiler {
		private IEnumerable<MetadataReference> _references;

		public CSharpCompiler() {
			_references = GetAssemblyReferences();
		}

		public ITestable Compile(AppDomain appDomain, CompilationUnitSyntax syntax) {
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
					TestClassProxy proxy = (TestClassProxy) appDomain.CreateInstanceAndUnwrap(
						typeof(TestClassProxy).Assembly.FullName,
						"PatternGuidedGP.Compiler.CSharp.TestClassProxy");
					//TestClassProxy proxy = new TestClassProxy();
					proxy.Initialize(ms.GetBuffer(), "ProblemClass", "Test");
					return proxy;
				} else {
					// removed output because of compilation errors: division by constant 0

					/*Logger.WriteLine(0, "Code does not compile:");
					Logger.WriteLine(0, syntax.NormalizeWhitespace().ToString());
					foreach (var error in compilationResult.Diagnostics) {
						Logger.WriteLine(0, error.ToString());
					}*/
				}
				return null;
			}
		}

		private static IEnumerable<MetadataReference> GetAssemblyReferences() {
			var references = new MetadataReference[] {
				MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
				MetadataReference.CreateFromFile(typeof(ExecutionRecord).GetTypeInfo().Assembly.Location)
			};
			return references;
		}
	}
}
