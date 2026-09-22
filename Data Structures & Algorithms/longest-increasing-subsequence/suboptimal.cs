// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n^2) time / O(n) space
// -  Space-optimized bottom-up DP with rolling arrays   [pick-not-pick-dp]
// -  ranks below optimal.cs (O(n log n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself (from submission-9)
// -
// -  Same nested loop structure as submission-6 but keeps only current and
// -  next rows, reducing space to O(n).
// --------------------------------------------------------------------------

public class Solution
{
    public int LengthOfLIS(int[] nums)
    {
        int n = nums.Length;

        // +1 shift for prevIndex
        // prevIndex = -1 -> column 0
        // prevIndex =  0 -> column 1
        // prevIndex =  1 -> column 2
        int[] next = new int[n + 1];
        int[] curr = new int[n + 1];

        // Base case:
        // dp[n, *] = 0
        // Already 0 by default in C#

        for (int i = n - 1; i >= 0; i--)
        {
            for (int prevIndex = i - 1; prevIndex >= -1; prevIndex--)
            {
                // Not pick current
                int notPick = next[prevIndex + 1];

                int pick = 0;

                // Pick current
                if (prevIndex == -1 || nums[i] > nums[prevIndex])
                {
                    pick = 1 + next[i + 1];
                }

                curr[prevIndex + 1] = Math.Max(pick, notPick);
            }
            (next, curr) = (curr, next);
        }

        return next[0];
    }
}

/*
================================================================================
 PATTERN : Take/Skip DP with prev-index state, two rolled rows
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-9.cs when it was first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  next        next[p+1] = longest increasing run buildable from index i+1 on, when the last kept element sits at index p
  curr        same meaning for row i, being filled now
  prevIndex   index of the last element already kept; -1 means nothing kept yet
  notPick     answer if we skip nums[i] and keep prevIndex unchanged
  pick        answer if we keep nums[i], which makes i the new prevIndex
WHY THIS PATTERN
  "Longest increasing subsequence" means every element is either kept or
  dropped, and whether we may keep nums[i] depends only on the last element we
  kept. That is exactly a two-state decision with one piece of carried context,
  so the state is (i, prevIndex) and the transition is Math.Max(pick, notPick).
  The +1 shift on prevIndex exists only so the value -1 ("nothing kept yet") can
  be stored at column 0 of an int array.
BETTER APPROACH
  The better solution is patience sorting: keep a growing list tails where
  tails[L] is the smallest possible tail value of an increasing subsequence of
  length L+1, binary search each nums[i] for the first element >= it, and
  overwrite or append. That is O(n log n) time and the answer is tails.Count.
  This file loses because it evaluates every (i, prevIndex) pair; the inner loop
  over prevIndex alone costs i+1 steps per i. A cheaper middle ground also
  exists: drop the prevIndex dimension entirely and use dp[i] = LIS starting at
  i, one array, same time but far fewer reads.
INVARIANT
  When the loop body for index i starts, next holds the fully solved row i+1:
  for every p <= i, next[p+1] is the correct answer for the suffix starting at
  i+1 given last-kept index p. The body only ever reads next[prevIndex+1] with
  prevIndex <= i-1 and next[i+1], both of which were written during the previous
  iteration, so the row is complete where it is touched. After the swap, next is
  row i, and by induction the final next[0] is the answer for the whole array
  with nothing kept yet.
THE SWAP LEAVES GARBAGE BEHIND
  (next, curr) = (curr, next) does not clear anything, so after a few iterations
  curr still contains values from row i+2. This is safe only because iteration i
  writes columns 0..i and never reads a column above i+1. Anyone who widens the
  inner loop, or reads next[j] for some j > i+1, will silently read stale
  numbers instead of zeros.
WATCH OUT
  The comparison nums[i] > nums[prevIndex] is strict, so this counts strictly
  increasing runs; for a "non-decreasing" variant you must change it to >=, and
  nothing else in the code signals that choice. The comment saying the base row
  is already 0 by default is only true for the very first iteration i = n-1;
  from then on the arrays hold old rows, not zeros. n = 0 is handled by luck:
  the arrays have length 1, the outer loop never runs, and next[0] is the
  default 0. The two arrays must be length n+1, not n, because column i+1 is
  read when i = n-1.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Print the actual subsequence, not just its length.
     You need the choice made at each state, so keep the full 2-D table or a
     parent array; with only two rolled rows the history is gone after each
     swap, and memory goes back to O(n^2) unless you re-run the recurrence
     forward.
  2. Count how many longest increasing subsequences exist.
     Carry a second array of counts beside next and curr: when pick and notPick
     tie, add the counts; when one wins, copy its count. Same O(n^2) time, one
     more array.
  3. Rewrite it top-down.
     Recurse on (i, prevIndex) with a memo of size n x (n+1); same time, but the
     memo costs O(n^2) space and deep recursion risks a stack overflow on a long
     nums, which is why this bottom-up rolled version is preferred.
  4. Make it work when only the length matters and n is very large.
     Switch to the tails + binary search method; the prevIndex dimension
     disappears completely because the smallest tail per length is all the
     context you need.
TRIGGER
  Reach for prev-index take/skip DP when each element may be kept only if it
  compares correctly against the last element you kept.
C# NOTE
  (next, curr) = (curr, next) is tuple deconstruction and swaps two references,
  so no array data is copied per row. If you later move to the O(n log n)
  version, List<int>.BinarySearch returns the bitwise complement ~insertionPoint
  when the value is absent, so the usual idiom is int idx =
  tails.BinarySearch(x); if (idx < 0) idx = ~idx;.
COMPLEXITY
  Time  : O(n^2)
  Space : O(n)
================================================================================
*/
