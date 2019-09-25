using Microsoft.CodeAnalysis;
using PatternGuidedGP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternGuidedGP.AbstractSyntaxTree.SyntaxGenerator {
	static class SyntaxExtensions {
		public static Type GetNodeType(this SyntaxNode node) {
			var annotation = node.GetAnnotations("Type").FirstOrDefault();
			return annotation != null ? Type.GetType(annotation.Data) : null;
		}

		public static int GetTreeHeight(this SyntaxNode root) {
			bool isExpressionNode = root.GetAnnotations("Node").Any();
			int thisHeight = isExpressionNode ? 1 : 0;
			IEnumerable<SyntaxNode> children = root.ChildNodes();
			if (children.Any()) {
				int maxChildHeight = 0;
				foreach (var node in children) {
					int nodeHeight = GetTreeHeight(node);
					if (nodeHeight > maxChildHeight) {
						maxChildHeight = nodeHeight;
					}
				}
				return thisHeight + maxChildHeight;
			}
			else {
				return thisHeight;
			}
		}

		public static SyntaxNode RandomNode(this SyntaxNode root) {
			return GetUniformDepthRandomAnnotatedNode(root, new SyntaxAnnotation("Node"));
			//return GetRandomAnnotatedNode(root, new SyntaxAnnotation("Node"));
		}

		public static SyntaxNode RandomNode(this SyntaxNode root, Type type) {
			return GetUniformDepthRandomAnnotatedNode(root, new SyntaxAnnotation("Type", type.ToString()));
			//return GetRandomAnnotatedNode(root, new SyntaxAnnotation("Type", type.ToString()));
		}

		private static SyntaxNode GetRandomAnnotatedNode(SyntaxNode root, SyntaxAnnotation annotation) {
			var descNodes = root.DescendantNodes().Where(node => {
				var foundAnnotation = node.GetAnnotations(annotation.Kind).FirstOrDefault();
				return foundAnnotation != null &&
					(annotation.Data == null || foundAnnotation.Data != null && foundAnnotation.Data.Equals(annotation.Data));
			});
			return descNodes.ElementAtOrDefault(new Random().Next(descNodes.Count()));
		}

		private static SyntaxNode GetUniformDepthRandomAnnotatedNode(SyntaxNode root, SyntaxAnnotation annotation) {
			//Console.WriteLine("Tree height for root: " + root);
			var levelNodes = new MultiValueDictionary<int, SyntaxNode>();
			GetTreeNodeHeights(root, levelNodes);
			//foreach (var level in levelNodes.Keys) {
			//	Console.WriteLine("level " + level + ": " + string.Join(", ", levelNodes[level].Select(n => n.ToString()).ToArray()));
			//}

			var random = new Random();
			int levelIndex = random.Next(levelNodes.Count);
			var syntaxNodes = levelNodes.Values.ElementAt(levelIndex);
			int nodeIndex = random.Next(syntaxNodes.Count);
			SyntaxNode node = syntaxNodes.ElementAt(nodeIndex);
			//Console.WriteLine("random select node: " + node + ", level index=" + levelIndex + ", node index=" + nodeIndex);

			return GetRandomAnnotatedNode(root, annotation);
		}

		private static int GetTreeNodeHeights(SyntaxNode root, MultiValueDictionary<int, SyntaxNode> heights) {
			bool isExpressionNode = root.GetAnnotations("Node").Any();
			int thisHeight = isExpressionNode ? 1 : 0;
			int maxChildHeight = 0;
			foreach (var node in root.ChildNodes()) {
				int nodeHeight = GetTreeNodeHeights(node, heights);
				if (nodeHeight > maxChildHeight) {
					maxChildHeight = nodeHeight;
				}
			}
			if (isExpressionNode) {
				heights.Add(thisHeight + maxChildHeight, root);
				//Console.WriteLine("Syntax: " + root + "; height=" + (thisHeight + maxChildHeight));
			}
			return thisHeight + maxChildHeight;
		}
	}
}
