// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  hash set membership, early exit on repeat   [hashset-membership]
// -  ranks above optimal-variant.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  HashSet.Add returns false on a repeat, allowing an early return as
// -  soon as a duplicate is found; worst case (all distinct) still visits
// -  all n elements and stores them.
// --------------------------------------------------------------------------

public class Solution
{
    public bool hasDuplicate(int[] nums)
    {
        var seen = new HashSet<int>();

        foreach (int number in nums)
        {
            // HashSet.Add returns FALSE when the value was already present.
            // One call does both the lookup and the insert.
            if (!seen.Add(number))
                return true;
        }

        return false;
    }
}

/*
================================================================================
 PATTERN : Hash set membership - one pass, exit on first repeat
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The array is unsorted and unbounded in value, so there is no structure to
  exploit - the only question being asked of each element is "have I met you
  before?". That question is exactly what a hash set answers in constant
  expected time, so the whole problem collapses to one pass with a growing memo
  of what has been seen.
BRUTE FORCE AND THE OTHER TRADE
  Compare every pair with nested loops: correct, no extra memory, but quadratic.
  Sorting first and scanning adjacent pairs is n log n and needs no auxiliary
  structure if you are allowed to mutate nums in place. The hash set buys linear
  time by paying memory for it. If the interviewer forbids extra space, sorting
  is the fallback answer; if nums is read-only AND space is forbidden, you are
  back to the quadratic scan unless values are constrained to 1..n, in which
  case Floyd cycle detection applies.
THE ONE-CALL TRICK
  HashSet<int>.Add returns false when the value was already in the set, true
  when it was newly inserted. So the branch if (!seen.Add(number)) return true
  does the membership test and the insertion in a single call. Writing if
  (seen.Contains(number)) return true; seen.Add(number); is the same logic but
  hashes number twice on every non-duplicate element. Same asymptotics, strictly
  more work, and it is the version an interviewer expects you to tighten.
INVARIANT
  At the top of each iteration for element nums[i], seen holds exactly the
  distinct values of nums[0..i-1], and no duplicate exists among nums[0..i-1].
  Both halves matter: the second half is what lets you return the moment Add
  fails, because it proves the collision is with an earlier element and not a
  stale artifact. The invariant is restored by the Add itself - either it
  returns false and you leave, or it returns true and seen now covers through
  index i.
WHY THE FALSE IS CORRECT
  Falling out of the foreach means every Add returned true, so every element was
  new when it was offered, so seen.Count equals nums.Length and all n values are
  pairwise distinct. Returning false is not a default - it is the conclusion of
  the loop having exhausted the array with the invariant intact. An empty nums
  takes the same path and correctly returns false.
WATCH OUT
  The early return is not just a speed detail: seen only ever grows to the
  number of distinct values before the first repeat, so on an array whose first
  two elements collide it holds one element. Any variant that builds the full
  set up front - new HashSet<int>(nums).Count != nums.Length, or
  nums.Distinct().Count() != nums.Length - is still correct but throws that away
  and always touches all n elements. If you want to talk about tuning, new
  HashSet<int>(nums.Length) is the honest knob: it preallocates the bucket array
  so growth does not rehash, at the cost of always reserving worst-case memory
  even when the answer is found on element two.
INTERVIEWER FOLLOW-UPS
  1. "Duplicates only within k indices of each other" - keep the same set but
  evict nums[i-k-1] as the window slides; the set becomes a window, not a
  history. 2. "Return the duplicate value, or all of them" - the value is
  already in hand as number; for all of them, do not return, collect into a
  result set so a value repeated three times is reported once. 3. "nums does not
  fit in memory" - a Bloom filter gives one-sided error (a false positive claims
  a duplicate that is not there, never the reverse), so it screens candidates
  for a second exact pass. 4. "Values are in 1..n" - the array can be its own
  hash table: negate or swap in place for constant extra space.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
