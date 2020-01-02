using PatternGuidedGP.AbstractSyntaxTree;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.GP.Problems {
	class InstructionSetBuilder {

		private IInstructionSetRepository _repository;

		public InstructionSetBuilder() {
			_repository = new InstructionSetRepository();
		}

		public InstructionSetBuilder(IInstructionSetRepository repository) {
			this._repository = repository;
		}

		public InstructionSetBuilder AddIntParameters(int count) {
			for (int i = 0; i < count; i++) {
				_repository.Add(new IntIdentifierExpression(((char)('a' + i)).ToString()));
			}
			return this;
		}

		public InstructionSetBuilder AddBoolParameters(int count) {
			for (int i = 0; i < count; i++) {
				_repository.Add(new BoolIdentifierExpression(((char)('a' + i)).ToString()));
			}
			return this;
		}

		public InstructionSetBuilder AddFloatDomain() {
			_repository.Add(
				new BoolEqualFloatExpression(),
				new BoolNotEqualFloatExpression(),
				new BoolGreaterEqualFloatExpression(),
				new BoolGreaterThanFloatExpression(),
				new BoolLessEqualFloatExpression(),
				new BoolLessThanFloatExpression(),
				new FloatAdditionExpression(),
				new FloatSubtractionExpression(),
				new FloatMultiplicationExpression(),
				new FloatDivisionExpression());
			return this;
		}

		public InstructionSetBuilder AddForLoopVariable() {
			_repository.Add(new ForLoopVariable());
			return this;
		}

		public InstructionSetBuilder AddBooleanDomain() {
			_repository.Add(new BoolAndExpression(),
				new BoolFalseExpression(),
				new BoolNotExpression(),
				new BoolOrExpression(),
				new BoolTrueExpression(),
				new BoolXorExpression(),
				new BoolEqualBoolExpression(),
				new BoolNotEqualBoolExpression());
			return this;
		}

		public InstructionSetBuilder AddSimpleBooleanDomain() {
			_repository.Add(new BoolAndExpression(),
				new BoolFalseExpression(),
				new BoolNotExpression(),
				new BoolOrExpression(),
				new BoolTrueExpression(),
				new BoolXorExpression(),
				//new BoolNotEqualBoolExpression(),
				new BoolEqualBoolExpression());
			return this;
		}

		public InstructionSetBuilder AddIntRandomLiteral(int lowerBound, int upperBound) {
			_repository.Add(new IntRandomLiteral(lowerBound, upperBound));
			return this;
		}

		public InstructionSetBuilder AddBoolTargetVariable() {
			_repository.Add(new BoolIdentifierExpression("ret", true));
			_repository.Add(new BoolAssignmentStatement());
			return this;
		}

		public IInstructionSetRepository Build() {
			return _repository;
		}

		public InstructionSetBuilder AddIntVariable(string name, bool isArray = false) {
			if (isArray) {
				_repository.Add(new IntArrayIdentifier(name));
			} else {
				_repository.Add(new IntIdentifierExpression(name));
			}
			return this;
		}

		public InstructionSetBuilder AddFloatVariable(string name, bool isArray = false) {
			if (isArray) {
				_repository.Add(new FloatArrayIdentifier(name));
			}
			else {
				_repository.Add(new FloatIdentifierExpression(name));
			}
			return this;
		}

		public InstructionSetBuilder AddIfStatement() {
			_repository.Add(new IfStatement());
			return this;
		}

		public InstructionSetBuilder AddForLoopTimesStatement() {
			_repository.Add(new ForLoopTimesStatement());
			return this;
		}

		public InstructionSetBuilder AddIntTargetVariable() {
			_repository.Add(new IntIdentifierExpression("ret", true));
			_repository.Add(new IntAssignmentStatement());
			return this;
		}

		public InstructionSetBuilder AddFloatTargetVariable() {
			_repository.Add(new FloatIdentifierExpression("ret", true));
			_repository.Add(new FloatAssignmentStatement());
			return this;
		}

		public InstructionSetBuilder AddStringTargetVariable() {
			_repository.Add(new StringIdentifierExpression("ret", true));
			_repository.Add(new StringAssignmentStatement());
			return this;
		}

		public InstructionSetBuilder AddIntegerDomain() {
			_repository.Add(
				new BoolEqualIntExpression(),
				new BoolNotEqualIntExpression(),
				new BoolGreaterEqualIntExpression(),
				new BoolGreaterThanIntExpression(),
				new BoolLessEqualIntExpression(),
				new BoolLessThanIntExpression(),
				new IntAdditionExpression(),
				new IntSubtractionExpression(),
				new IntMultiplicationExpression(),
				new IntDivisionExpression(),
				new IntModuloExpression());
			return this;
		}

		public InstructionSetBuilder AddSimpleIntegerDomain() {
			_repository.Add(
				new BoolEqualIntExpression(),
				//new BoolNotEqualIntExpression(),
				//new BoolGreaterEqualIntExpression(),
				new BoolGreaterThanIntExpression(),
				//new BoolLessEqualIntExpression(),
				new BoolLessThanIntExpression(),
				new IntAdditionExpression(),
				new IntSubtractionExpression(),
				new IntMultiplicationExpression(),
				new IntDivisionExpression(),
				new IntModuloExpression());
			return this;
		}

		public InstructionSetBuilder AddIntegerLiterals(params int[] values) {
			foreach (var val in values) {
				_repository.Add(new IntLiteralExpression(val));
			}
			return this;
		}

		public InstructionSetBuilder AddStringLiterals(params string[] values) {
			foreach (var val in values) {
				_repository.Add(new StringLiteralExpression(val));
			}
			return this;
		}
	}
}
