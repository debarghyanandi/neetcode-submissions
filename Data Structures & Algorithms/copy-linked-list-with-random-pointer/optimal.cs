// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public Node copyRandomList(Node head)
    {

        Dictionary<Node, Node> map = new();

        Node curr = head;

        while (curr != null)
        {
            map[curr] = new Node(curr.val);
            curr = curr.next;
        }

        curr = head; //reset

        while (curr != null)
        {
            map[curr].next = curr.next == null ? null : map[curr.next];
            map[curr].random = curr.random == null ? null : map[curr.random];
            curr = curr.next;
        }
        return head == null ? null : map[head];
    }
}

/*
================================================================================
 PROBLEM : Given a linked list where each node has val, next, and an extra
           random pointer (to any node in the list, or null), return a deep
           copy. Every copied pointer must point to the new nodes, never the
           old ones. Example: [[7,null],[13,0],[11,2]] -> same shape, all-new
           nodes.
 PATTERN : Hash Map (old node -> new node), two passes
================================================================================
IDEA
  Pass 1 walks the list with curr and puts map[curr] = new Node(curr.val).
  After this pass, every old node has its clone, but no links are set yet.
  Pass 2 walks again and wires map[curr].next = map[curr.next] and
  map[curr].random = map[curr.random], turning null into null.
  It is correct because all clones exist before pass 2 starts. So any
  random pointer, even one pointing backward or to itself, has a target.
EXAMPLE
  A(7)->B(13)->C(11); A.random=null, B.random=A, C.random=C (itself).
  Pass 1: map = {A->A', B->B', C->C'}.
  Pass 2: A'.next=B', A'.rnd=null; B'.next=C', B'.rnd=A'; C'.next=null,
  C'.rnd=C'.
  Return map[A] = A'.
COMPLEXITY
  Time  O(n)  two linear passes, each with O(1) dictionary lookups
  Space O(n)  the map holds one entry per node
PATH TO OPTIMAL
  Copy next links, then for each random walk from head to find its index,
  and walk the copy to the same index - O(n^2) time - repeated scans.
  Hash map old->new (this file) - O(n) time - one O(1) lookup per pointer.
KEYWORDS
  deep copy, linked list, hash map, random pointer, clone graph, interleaving
WATCH OUT
  - Do not set random in pass 1: the target clone may not exist yet
    (B.random points ahead to a node not yet created).
  - Key the map by node reference, not by val: values can repeat.
  - Return map[head], not head. And guard head == null, as the code does.
  - Never let a copied next or random point to an original node.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it with O(1) extra space?
     -> Interleave clones: A->A'->B->B'. Set A'.random = A.random.next, then
        split the two lists. Still O(n) time; harder code, and it mutates input.
  2. Can you do it in one pass?
     -> Yes. Use a getClone helper that creates a node on first sight. Then
        set next and random as you walk. Same O(n) time and space.
  3. How is this like Clone Graph?
     -> Same idea: map old node to clone and fill edges by DFS/BFS. The map
        also stops cycles, here self-random or two-way random links.
TRIGGER
  When you must deep copy a structure whose pointers can go anywhere,
  first map each old node to its clone, then wire the links.
================================================================================
*/
