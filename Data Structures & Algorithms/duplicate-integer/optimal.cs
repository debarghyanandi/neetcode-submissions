// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  hash set, add-and-check, early exit on repeat   [hashset-membership]
// -  ranks above optimal-variant.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  HashSet.Add returns false on a duplicate, letting the loop return
// -  immediately; worst case (all distinct) still does n adds
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
 PATTERN : Hash Set membership - Add's return value is the test
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The question "has any value appeared before?" is a membership query over a
  growing prefix. Anything that answers membership in constant expected time
  collapses the problem to a single pass, and HashSet<int> is the built-in that
  does it. Nothing about order, position, or count of the duplicate is asked for
  - only existence - so no structure richer than a set is needed.
THE ONE MOVE TO REMEMBER
  seen.Add(number) returns false when number was already in the set, true when
  it was newly inserted. So !seen.Add(number) is simultaneously the "already
  seen" test and the insert. The naive version is:

      if (seen.Contains(number)) return true;
      seen.Add(number);

  which hashes number twice and probes twice on every non-duplicate element. The
  one-call form is the reason the comment in the file exists - this is the
  detail that will not be obvious on reread.
INVARIANT
  At the top of each iteration, seen holds exactly the distinct values among the
  elements of nums already consumed. That is maintained by the only mutation in
  the loop: every element is either added (new value) or found present (returns
  immediately). Nothing is ever removed.
CORRECTNESS ARGUMENT
  Returning true: by the invariant, !seen.Add(number) means number was inserted
  by some earlier iteration j, so nums[j] == nums[i] with j < i - a real
  duplicate pair exists.

  Returning false: reaching the end means every Add returned true, so all
  nums.Length insertions were new values and seen.Count == nums.Length. A set of
  that size over that many elements forces all elements distinct. Both
  directions are covered, so the answer is exact, not one-sided.
WATCH OUT
  The early return means the set only ever grows to the number of distinct
  values before the first repeat - the peak size is data-dependent, and the
  all-distinct input is the worst case that pins it at nums.Length.

  An empty array skips the loop and correctly returns false. A null nums throws
  NullReferenceException at the foreach, not a clean argument error - flag it if
  the interviewer cares about contract validation.

  The HashSet is constructed with no capacity argument, so it resizes as it
  fills. Passing nums.Length to the constructor pre-sizes it and is the obvious
  tweak if asked to tighten this.
ALTERNATIVES AND WHY THEY LOSE
  Sort then scan adjacent pairs: O(n log n) time, but O(1) extra space if you
  are allowed to mutate nums. That is the trade to name when the interviewer
  says "now do it without extra memory."

  new HashSet<int>(nums).Count != nums.Length, or nums.Distinct().Count() !=
  nums.Length: same asymptotics, one line, but both build the full set before
  answering - no early exit on an input that repeats at index 1.

  Nested double loop: O(n^2), only defensible when n is tiny or allocation is
  forbidden.
TRIGGER
  Reach for set-while-scanning whenever the predicate depends only on the
  multiset of elements already visited and can be answered by an existence check
  - duplicate detection, first repeating character, two-sum complement lookup.
  The tell is that you never need to know where the earlier element was, only
  that it was there.
GENERALIZING
  This works on int because int has value equality and a sensible hash for free.
  Swap in a reference type and correctness now rests entirely on that type's
  GetHashCode and Equals being consistent with each other; a type that overrides
  one but not the other silently reports every element as distinct. Worth
  stating out loud if the follow-up is "now detect duplicate objects."
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
