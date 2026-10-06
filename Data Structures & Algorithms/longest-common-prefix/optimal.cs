// --------------------------------------------------------------------------
// -  optimal.cs            O(n * m) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public string LongestCommonPrefix(string[] strs)
    {
        string prefix = strs[0];

        for (int i = 1; i < strs.Length; i++)
        {
            int j = 0;
            while (j < Math.Min(prefix.Length, strs[i].Length))
            {
                if (prefix[j] != strs[i][j])
                {
                    break;
                }
                j++;
            }
            prefix = prefix.Substring(0, j);
        }

        return prefix;
    }
}

/*
================================================================================
 PROBLEM : Given an array of strings strs, return the longest string that is a
           prefix of every string in the array. If no common prefix exists,
           return the empty string "". Example: ["flower","flow","flight"] ->
           "fl".
 PATTERN : Horizontal Scanning (shrinking prefix)
================================================================================
IDEA
  Start with prefix = strs[0] as the best guess. For each next string
  strs[i], walk j forward while both strings still have characters and
  they match. Then cut prefix down to its first j characters. The prefix
  can only shrink, and after string i it is common to strs[0..i], so the
  final prefix is common to all and is the longest such string.
EXAMPLE
  strs = ["flower","flow","flight"], prefix = "flower"
  i=1 "flow": f,l,o,w match, j stops at Min(6,4)=4 -> prefix = "flow"
  i=2 "flight": f,l match, 'o' != 'i' at j=2 -> prefix = "fl"
  Answer: "fl"
COMPLEXITY
  Time  O(n * m)  n strings, at most m char compares each (m = length of
                  strs[0])
  Space O(1)      only prefix and j; Substring copies at most m chars, no
                  buffers
PATH TO OPTIMAL
  Try every prefix of strs[0], check all strings with StartsWith -
  O(n * m^2) - repeats the same character checks for each length.
  Shrink one prefix while scanning (this file) - O(n * m) - each string
  is compared once, and only against the current prefix.
KEYWORDS
  longest common prefix, string, horizontal scanning, vertical scanning,
  trie, character-by-character comparison
WATCH OUT
  - An empty strs array makes strs[0] throw. Guard: return "" if the
    array is empty.
  - Without the Math.Min bound, prefix[j] or strs[i][j] goes out of range
    when one string is shorter, e.g. ["ab","a"].
  - No early exit: once prefix is "", the loop still visits every other
    string. Add "if (prefix.Length == 0) return """ to save work.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it by columns instead of rows?
     -> Vertical scanning: for each index j, check strs[k][j] for every k and
        stop at the first mismatch or end. Still O(n * m) time, O(1) space, but
        it stops early when the common prefix is short.
  2. What if you can sort the array?
     -> Sort, then compare only the first and last strings; their common
        prefix is the answer. O(n * m * log n) time, so slower here, but easy to
        code.
  3. Many queries, each asking the LCP of a fixed set plus one new string?
     -> Build a trie of the set once. The LCP is the path from the root while
        each node has one child and is not a word end. O(total chars) space.
  4. Strings arrive as a stream?
     -> This code already fits: keep prefix and shrink it with each new
        string. O(m) memory, and you never need the old strings again.
TRIGGER
  When an answer must hold for every item and can only shrink as you add
  items, keep one running candidate and trim it against each item.
================================================================================
*/
