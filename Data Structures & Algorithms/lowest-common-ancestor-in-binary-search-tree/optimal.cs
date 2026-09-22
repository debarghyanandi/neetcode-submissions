// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  BST traversal, iterative descent   [bst-iterative]
// -  ranks above suboptimal.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Traverses down the BST by comparing target values with current node;
// -  worst case height is O(n) for skewed tree.
// --------------------------------------------------------------------------

public class Solution
{
    public TreeNode LowestCommonAncestor(TreeNode root, TreeNode p, TreeNode q)
    {
        TreeNode node = root;

        while (node != null)
        {
            if (p.val > node.val && q.val > node.val)
            {
                node = node.right;
            }
            else if (p.val < node.val && q.val < node.val)
            {
                node = node.left;
            }
            else
            {
                return node;
            }
        }
        return null;
    }
}

/*
================================================================================
 PATTERN : BST Descent - walk down to the split point
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  node     the current node on the walk down; the candidate ancestor
WHY THIS PATTERN
  The tree is a binary search tree, so every node's value splits its subtrees:
  smaller on the left, larger on the right. That means you never need to search
  both sides. If p.val and q.val are both bigger than node.val, both targets
  live in node.right; if both are smaller, both live in node.left; otherwise
  node itself separates them and is the answer. One walk from root to that split
  point is enough.
BRUTE FORCE
  Ignore the ordering and treat it as a plain binary tree: recurse into both
  children, return the node where one target is found on the left and the other
  on the right. That is O(n) time and O(h) stack space, and it visits nodes that
  the BST rule already rules out. Another common first try is to collect the
  root-to-p and root-to-q paths in two lists and compare them, which costs extra
  memory for the paths.
INVARIANT
  At the top of every loop pass, both p and q are inside the subtree rooted at
  node. The BST comparison only moves node to the child that still contains
  both, so the invariant holds after the move. When neither branch takes both,
  node lies between p and q (or equals one of them), so no node deeper down can
  contain both - node is the lowest common ancestor.
WATCH OUT
  The final return null is only reached if root is null or if p or q is not
  actually in the tree; the code never checks membership, so a missing target
  silently returns a wrong node or null instead of reporting the problem. It
  dereferences p.val and q.val before testing them, so a null p or q throws a
  NullReferenceException. The whole method is wrong on a tree that is not a
  valid BST, and it also breaks if the tree holds duplicate values, since the
  decision is made on values alone. If p and q are the same node, it correctly
  returns that node - the "otherwise" branch catches the equal case.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Write it recursively.
     Same three-way test, but return LowestCommonAncestor(root.right, p, q) or
     the left child instead of reassigning node. It reads the same but adds O(h)
     call stack, which the loop avoids.
  2. What if the tree is a general binary tree with no ordering?
     You must search both subtrees. Recurse left and right; if both sides return
     non-null, the current node is the answer, else pass up whichever side is
     non-null. That is O(n) time and O(h) stack.
  3. What if p or q may not be present in the tree?
     Keep this walk to find the candidate, then run two more O(h) searches from
     that candidate to confirm both values exist below it. Still O(h), just
     three passes instead of one.
  4. What if the nodes carry parent pointers?
     Walk up from p and q instead of down from root. Lift the deeper one to the
     same depth, then step both up together until they meet - no root access
     needed.
TRIGGER
  The problem says "binary search tree" and asks about a relationship between
  two nodes - use the ordering to pick one branch per step instead of searching
  both.
C# NOTE
  The comparisons use p.val and q.val, not reference equality (p == node), so p
  and q only need to carry the right integer values, not be the exact TreeNode
  objects stored in the tree; switching to == would compare references and
  quietly change the contract.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
