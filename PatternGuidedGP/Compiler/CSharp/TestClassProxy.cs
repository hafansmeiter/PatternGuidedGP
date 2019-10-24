using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.Compiler.CSharp {
	class TestClassProxy : MarshalByRefObject, ITestable {
		private MethodInfo _method;

		public void Initialize(byte[] rawAssembly, string typeName, string methodName) {
			var assembly = Assembly.Load(rawAssembly);
			var type = assembly.GetType(typeName);
			_method = type.GetMethod(methodName);
		}

		public object RunTest(params object[] parameter) {
			try {
				return _method.Invoke(null, parameter);
			} catch (Exception ex) {
				//Logger.WriteLine(4, "Exception: " + ex.GetType().Name);
				// Code does not run properly, e.g. DivideByZeroException
				// Count as negative run
			}
			return null;
		}
	}
}
