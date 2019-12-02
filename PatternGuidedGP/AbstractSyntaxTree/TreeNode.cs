using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using PatternGuidedGP.AbstractSyntaxTree.SyntaxGenerator.CSharp;
using PatternGuidedGP.AbstractSyntaxTree.TreeGenerator;
using PatternGuidedGP.GP.SemanticGP;
using PatternGuidedGP.Pangea;
using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree {
	delegate IEnumerable<TreeNode> TreeNodeFilter(IEnumerable<TreeNode> nodes);

	abstract class TreeNode : ICSharpSyntaxGenerator {
		public abstract bool IsTerminal { get; }
		public abstract bool IsVariable { get; }
		public abstract int RequiredTreeDepth { get; }

		public abstract string Description { get; }
		public abstract Type Type { get; }
		public abstract Type[] ChildTypes { get; }
	
		public TreeNode Parent { get; set; }
		public List<TreeNode> Children { get; private set; }

		// for distinguishing operation in Pangea ML-dataset
		public abstract int OperatorId { get; }

		public ulong Id { get; private set; }
		private static ulong _currentId = 0;


		// nodes get cloned -> no constructor
		// use method Initialize instead
		public virtual void Initialize() {
		}

		public void AddChild(TreeNode node) {
			Children.Add(node);
			node.Parent = this;
		}

		public int GetTreeHeight() {
			return CalculateHeight(this);
		}

		private int CalculateHeight(TreeNode node) {
			if (node.IsTerminal) {
				return 1;
			}

			int maxChildHeight = 0;
			foreach (var child in node.Children) {
				int height = CalculateHeight(child);
				if (height > maxChildHeight) {
					maxChildHeight = height;
				}
			}
			return maxChildHeight + 1;
		}

		public IEnumerable<TreeNode> GetSubTreeNodes(bool includeRoot = false) {
			// breadth-first
			var list = new List<TreeNode>();
			var queue = new Queue<TreeNode>();
			queue.Enqueue(this);
			while (queue.Count > 0) {
				var node = queue.Dequeue();
				if (node != this || includeRoot) {
					list.Add(node);
				}
				if (Children != null) {
					foreach (var child in node.Children) {
						queue.Enqueue(child);
					}
				}
			}
			return list;
		}

		// return true, if newNode is allowed to be placed at oldNode's position
		// false, otherwise
		public bool ReplaceChild(TreeNode oldNode, TreeNode newNode) {
			int index = Children.IndexOf(oldNode);
			if (index >= 0) {
				if (AcceptChild(newNode, index)) {
					Children[index] = newNode;
					newNode.Parent = this;
					return true;
				}
			}
			return false;
		}

		// interface ICloneable
		public object Clone() {
			TreeNode copy = (TreeNode)base.MemberwiseClone();
			copy.Id = ++_currentId;
			copy.Children = new List<TreeNode>();
			return copy;
		}

		// interface IDeepCloneable
		public object DeepClone() {
			TreeNode copy = (TreeNode)Clone();
			foreach (var child in Children) {
				copy.AddChild((TreeNode)child.DeepClone());
			}
			return copy;
		}

		// interface ICSharpSyntaxGenerator
		public virtual CSharpSyntaxNode GetSyntaxNode() {
			// Annotations Node and Type not required anymore
			// as GP-operators work with ASTs
			return GenerateSyntax().WithAdditionalAnnotations(
				new SyntaxAnnotation("Id", Id.ToString()),
				new SyntaxAnnotation("Node"),
				new SyntaxAnnotation("Type", Type.ToString()));
		}

		public virtual CSharpSyntaxNode GetExecutionTraceSyntaxNode() {
			return GetSyntaxNode();
		}

		protected abstract CSharpSyntaxNode GenerateSyntax();

		// interface IChildAcceptor
		public virtual bool AcceptChild(TreeNode child, int index) {
			var scopedNodes = child.GetSubTreeNodes(true)
				.Where(node => node is IScoped && ((IScoped)node).IsScoped)
				.Select(node => (IScoped) node);
			foreach (var scopedNode in scopedNodes) {
				if (FindParent(node => scopedNode.IsInScopeOf(node)) == null) {
					return false;
				}
			}
			return true;
		}

		public TreeNode FindParent(Predicate<TreeNode> predicate) {
			var current = this;
			do {
				if (predicate(current)) {
					return current;
				}
			} while ((current = current.Parent) != null);
			return null;
		}

		public virtual TreeNodeFilter GetChildSelectionFilter(int childIndex) {
			return (nodes) => nodes.Where(node => AcceptChild(node, childIndex));
		}

		public override string ToString() {
			return GetSyntaxNode().NormalizeWhitespace().ToString();
			/*StringBuilder builder = new StringBuilder();
			ToString(this, builder, 0);
			return builder.ToString();*/
		}

		private void ToString(TreeNode node, StringBuilder builder, int indent) {
			for (int i = 0; i < indent; i++) {
				builder.Append("  ");
			}
			builder.Append(node.Description + "\n");
			foreach (var child in node.Children) {
				ToString(child, builder, indent + 1);
			}
		}

		// Get root node for semantic backpropagation.
		// Relevant if this tree node is returned as root by method IsBackPropagable.
		// Mostly, return either this tree node or a direct child.
		public virtual TreeNode GetBackPropagationRoot() {
			return null;
		}

		public virtual bool IsBackPropagationRootFor(TreeNode origin) {
			return false;
		}

		public virtual bool IsBackPropagable(out TreeNode root) {
			var current = this;
			while ((current = current.Parent) != null) {
				if (current.IsBackPropagationRootFor(this)) {
					root = current.GetBackPropagationRoot();
					return true;
				}
			}
			root = null;
			return false;
		}

		public IList<TreeNode> GetPathTo(TreeNode node) {
			var path = new LinkedList<TreeNode>();
			var current = node;
			while (current != this) {   // not including node itself
				path.AddFirst(current);
				current = current.Parent;
				if (current == null) {  // no path exists from this node to given node -> return null
					return null;
				}
			}
			return path.ToList();
		}

		public override bool Equals(object obj) {
			TreeNode other = obj as TreeNode;
			if (other == null) {
				return false;
			}
			// use Description instead of GetType to compare types
			// as this comparison considers identifier names.
			if (Description != other.Description) {
				return false;
			}
			if (ChildTypes != null && other.ChildTypes == null
				|| ChildTypes == null && other.ChildTypes != null
				|| (ChildTypes != null && other.ChildTypes != null && ChildTypes.Length != other.ChildTypes.Length)) {
				return false;
			}
			if (ChildTypes != null && other.ChildTypes != null) {
				for (int i = 0; i < ChildTypes.Length; i++) {
					if (!ChildTypes[i].Equals(other.ChildTypes[i])) {
						return false;
					}
				}
			}
			if (Children != null && other.Children == null
				|| Children == null && other.Children != null
				|| (Children != null && other.Children != null && Children.Count != other.Children.Count)) {
				return false;
			}
			if (Children != null && other.Children != null) {
				for (int i = 0; i < Children.Count; i++) {
					if (!Children[i].Equals(other.Children[i])) {
						return false;
					}
				}
			}
			return true;
		}

		public override int GetHashCode() {
			var hashCode = -1987626574;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Description);
			hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(Type);
			foreach (var child in ChildTypes)
				hashCode *= -1521134295 + EqualityComparer<Type>.Default.GetHashCode(child);
			hashCode = hashCode * -1521134295 + EqualityComparer<List<TreeNode>>.Default.GetHashCode(Children);
			return hashCode;
		}
	}
}
