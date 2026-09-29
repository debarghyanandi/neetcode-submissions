// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public bool IsAnagram(string s, string t)
    {
        // Different lengths can never be anagrams. O(1) rejection.
        if (s.Length != t.Length)
            return false;

        // Build the frequency map of the first string.
        var charFrequency = new Dictionary<char, int>();

        foreach (char c in s)
        {
            charFrequency.TryGetValue(c, out int currentCount);
            charFrequency[c] = currentCount + 1;
        }

        // Walk the second string and spend one unit of each character's budget.
        foreach (char c in t)
        {
            if (!charFrequency.TryGetValue(c, out int remaining))
                return false;              // character not in first at all

            if (remaining == 0)
                return false;              // second uses this character more often than first

            charFrequency[c] = remaining - 1;
        }

        // Lengths matched and every character in t was covered by s's budget,
        // so no character can be left over. No second sweep needed.
        return true;
    }
}

/*
================================================================================
 PATTERN : Frequency Counting - build a budget, then spend it
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  charFrequency  charFrequency[c] = count of c in s, minus the uses of c seen so far in t
  currentCount   the count of c so far while building from s (0 if c is new)
  remaining      how many uses of c are still left in the budget before this step of t
WHY THIS PATTERN
  An anagram means both strings have the same multiset of characters, which
  means the same count for every character. Order does not matter, so we only
  need the counts. The first loop turns s into a budget in charFrequency. The
  second loop spends that budget with the characters of t, so any extra
  character in t shows up right away.
BRUTE FORCE
  The simple correct approach is to sort both strings, for example with new
  string(s.OrderBy(c => c).ToArray()), and compare the results. It costs O(n log
  n) time and O(n) extra space for the sorted copies. It loses because sorting
  does more work than needed: we only need counts, not order.
INVARIANT
  After each character of t is handled, charFrequency[c] equals the count of c
  in s minus the count of c in the part of t read so far, and every value is 0
  or more. The remaining == 0 check keeps that "0 or more" part true. At the
  end, the sum of all values is s.Length - t.Length, which is 0. If every value
  is 0 or more and the sum is 0, then every value is 0. So the counts match
  exactly, and returning true without a second pass is correct.
WATCH OUT
  Keys are never removed. A character that is fully spent stays in the
  dictionary with value 0, so both checks are needed: the TryGetValue miss
  handles a key that is not there, and remaining == 0 handles a spent key. If
  you delete one of them, the code breaks. The comparison is exact, one char at
  a time: 'A' and 'a' count as different characters. A char is one UTF-16 unit,
  so an emoji made of two units (a surrogate pair) is counted as two separate
  halves. A null s or t throws on .Length.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. The input is only lowercase a-z. What would you change?
     Use an int[26] and index with c - 'a'. You add 1 for each char of s and
     subtract 1 for each char of t in one loop, then check that all values are
     0. There is no hashing, and the memory is a fixed array. The cost is that
     the code only works for that alphabet.
  2. How would you group a list of words into anagram groups?
     Give each word a key, either its sorted letters or its 26-count signature,
     and put the words in a Dictionary<string, List<string>> by that key. The
     count signature avoids sorting each word, but you have to build the key
     string yourself.
  3. What if t arrives as a stream and you cannot know its length first?
     You lose the early length check and the "sum is 0" argument. Keep spending
     the budget as characters arrive. At the end, check that every value in
     charFrequency is 0, or keep a counter of units still unspent and check that
     it is 0.
TRIGGER
  When the question is "same items, any order", like anagram, permutation or
  rearrangement, compare frequency counts instead of sorting.
C# NOTE
  TryGetValue sets currentCount to default(int), which is 0, when the key is
  missing. So the build loop needs no ContainsKey branch. It still does two hash
  lookups per character: one read and one write through the indexer.
  CollectionsMarshal.GetValueRefOrAddDefault can do this in one lookup if you
  need it.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
