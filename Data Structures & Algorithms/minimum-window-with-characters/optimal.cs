// --------------------------------------------------------------------------
// -  optimal.cs            O(n + m) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public string MinWindow(string s, string t)
    {
        if (s.Length < t.Length)
            return string.Empty;

        var need = new Dictionary<char, int>();
        foreach (char c in t)
        {
            need[c] = need.GetValueOrDefault(c) + 1;
        }

        var window = new Dictionary<char, int>();

        // have     = how many DISTINCT required characters are fully satisfied
        // required = how many distinct characters t asks for
        int have = 0, required = need.Count;

        int left = 0;
        int minLength = int.MaxValue;
        int resultStart = 0;

        for (int right = 0; right < s.Length; right++)
        {
            char c = s[right];

            window[c] = window.GetValueOrDefault(c) + 1;

            // == not >= : `have` may only tick up on the exact crossing,
            // otherwise it would be counted again on every further copy.
            if (need.ContainsKey(c) && window[c] == need[c])
                have++;

            while (have == required)
            {
                if (right - left + 1 < minLength)
                {
                    minLength = right - left + 1;
                    resultStart = left;
                }

                char lc = s[left];
                window[lc]--;

                // Symmetrically: `have` only ticks down on the crossing.
                if (need.ContainsKey(lc) && window[lc] < need[lc])
                    have--;
                left++;
            }
        }

        return minLength == int.MaxValue ? string.Empty : s.Substring(resultStart, minLength);
    }
}

/*
================================================================================
 PATTERN : Sliding Window (variable size) - grow right, shrink left
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  need         need[ch] = how many copies of ch t asks for
  window       window[ch] = how many copies of ch are in s[left..right]
  have         number of distinct chars in need whose count is fully met
  required     need.Count, the number of distinct chars in t
  minLength    length of the best window found so far (int.MaxValue = none yet)
  resultStart  start index of that best window in s
  lc           the char at s[left] that is about to leave the window
WHY THIS PATTERN
  The problem asks for the shortest contiguous substring of s that covers all of
  t. "Contiguous" plus "shortest that satisfies a condition" points to a sliding
  window. Adding a char never breaks coverage, and removing one can only break
  it. So you move right until have == required, then move left as far as you can
  while coverage still holds. Each pointer only moves forward.
BRUTE FORCE
  Try every start index. For each start, extend the end and keep a count map
  until the substring covers t, then record its length and go to the next start.
  This is correct and takes O(n^2) time, because every start can scan to the end
  of s. It loses because it rebuilds counts from zero for each start, while the
  window reuses them when left moves.
INVARIANT
  window always holds the exact char counts of s[left..right]. have always
  equals the number of chars ch in need where window[ch] >= need[ch]. Inside the
  while loop the window is valid, so each length checked is a real candidate.
  The loop stops right after the first removal that breaks coverage. So for each
  right, the code records the shortest valid window that ends at right. The
  minimum over all right is the answer.
HAVE COUNTS DISTINCT CHARS, NOT COPIES
  have moves only when a char's count crosses its target: == on the way up, < on
  the way down. This means one integer comparison (have == required) replaces
  checking every key in need at every step. The < check in the shrink loop is
  also safe as a crossing test. Inside the loop every count is at least its
  target, so a single decrement can only go from need[lc] down to need[lc]-1.
WATCH OUT
  An empty t breaks the code. required is 0, so the while loop runs even when
  the window is empty. left moves past right, and on the next pass s[left] can
  be read out of range (for example s = "a", t = ""). Add an early return for
  t.Length == 0. Also, window stores every char of s, including chars not in t.
  That is harmless but uses extra memory, and it means window counts are not
  limited to need's keys.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. s is very long, and only a few of its chars appear in t. How do you speed
  this up?
     First build a filtered list of (index, char) pairs, keeping only chars that
     are in need. Then run the same window over that list and use the saved
     indices to get lengths. The time is O(n + f), where f is the filtered size.
     The cost is extra memory for the list.
  2. What if the window must contain t as a subsequence, in order (Minimum
  Window Subsequence)?
     Counts no longer work, because order matters. Scan forward to match t in
     order. Then scan backward from the match end to find the latest start. Or
     use DP over (i, j). This is about O(n * m) time.
  3. How would you find the longest substring with at most k distinct chars?
     Use the same grow and shrink shape, but reverse the roles. Shrink while the
     distinct count is greater than k, and record the maximum length after the
     shrink. The best window is now measured outside the while loop, not inside
     it.
TRIGGER
  Look for this: find the shortest (or longest) contiguous substring or subarray
  that meets a count-based condition, where adding elements only helps and
  removing them only hurts.
C# NOTE
  Each step does several Dictionary lookups on the same key (GetValueOrDefault,
  then the indexer, then ContainsKey and need[c]). If the input is ASCII, two
  int[128] arrays indexed by the char give the same logic with plain array
  access. Use need.TryGetValue to merge the ContainsKey check and the read into
  one lookup.
COMPLEXITY
  Time  : O(n + m)
  Space : O(1)
================================================================================
*/
