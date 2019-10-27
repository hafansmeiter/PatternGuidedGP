﻿using Microsoft.CodeAnalysis;
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

	internal abstract class TreeNode : ICSharpSyntaxGenerator, ITraceable {
		public abstract bool IsTerminal { get; }
		public abstract bool IsVariable { get; }
		public abstract int RequiredTreeDepth { get; }

		public abstract string Description { get; }
		public abstract Type Type { get; }
		public abstract Type[] ChildTypes { get; }

		public virtual bool IsTraceable { get; } = false;
		public virtual bool IsBackPropagationBase { get; } = false;

		public TreeNode Parent { get; set; }
		public List<TreeNode> Children { get; private set; }

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

		// interface ITraceable
		public virtual IEnumerable<TreeNode> GetExecutionTraceNodes() {
			return null;
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
			// Annotations not required anymore
			// as GP-operators work with ASTs
			return GenerateSyntax().WithAdditionalAnnotations(
				new SyntaxAnnotation("Id", Id.ToString()),
				new SyntaxAnnotation("Node"),
				new SyntaxAnnotation("Type", Type.ToString()));
		}

		protected abstract CSharpSyntaxNode GenerateSyntax();

		// interface IChildAcceptor
		public virtual bool AcceptChild(TreeNode child, int index) {
			return true;
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
				foreach (var child in node.Children) {
					queue.Enqueue(child);
				}
			}
			return list;
		}

		public virtual TreeNodeFilter GetChildSelectionFilter(int childIndex) {
			return null;
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

		public bool EqualsTreeNode(TreeNode other) {
			if (other == null) {
				return false;
			}
			if (Children != null && other.Children == null
				|| Children == null && other.Children != null
				|| Children.Count != other.Children.Count) {
				return false;
			}
			// use Description instead of GetType to compare types
			// as this comparison considers identifier names.
			if (Description != other.Description) {
				return false;
			}
			for (int i = 0; i < Children.Count; i++) {
				if (!Children[i].EqualsTreeNode(other.Children[i])) {
					return false;
				}
			}
			return true;
		}

		// Get root node for semantic backpropagation.
		// Relevant if this tree node is returned as root by method IsBackPropagable.
		// Mostly, return either this tree node or a direct child.
		public virtual TreeNode GetBackPropagationRoot() {
			return null;
		}

		public bool IsBackPropagable(out TreeNode root) {
			var current = this;
			while ((current = current.Parent) != null) {
				if (current.IsBackPropagationBase) {
					root = current.GetBackPropagationRoot();
					return true;
				}
			}
			root = null;
			return false;
		}

		public IEnumerable<TreeNode> GetPathTo(TreeNode node) {
			var current = this;
			while (current != node) {	// not including node itself
				yield return current;
				current = current.Parent;
			}
		}
	}
}
