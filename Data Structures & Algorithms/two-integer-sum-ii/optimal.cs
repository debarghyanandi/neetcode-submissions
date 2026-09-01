// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  two pointers converging on sorted array   [two-pointers-sorted]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  exploits sortedness so left/right pointers move monotonically toward
// -  the target sum, each step permanently eliminating one candidate
// --------------------------------------------------------------------------

public class Solution
{
    public int[] TwoSum(int[] numbers, int target)
    {
        int left = 0;
        int right = numbers.Length - 1;

        while (left < right)
        {
            int sum = numbers[left] + numbers[right];

            if (sum == target)
                return new int[] { left + 1, right + 1 };   // problem wants 1-based

            if (sum > target)
                right--;      // shrink from the large end
            else
                left++;       // grow from the small end
        }

        return Array.Empty<int>();
    }
}

/*
================================================================================
 PATTERN : Two Pointers - Converging on a Sorted Array
 SOURCE  : NeetCode / other resource (submission-0 + submission-2 merged; the
           two nested while-loops collapsed into a single compare)
 STATUS  : Optimal
================================================================================

WHY THIS PATTERN
  The array is SORTED, and that one word changes the tool completely.
  In a sorted array the sum at [left, right] is monotone in each pointer:
  moving left rightward can only INCREASE the sum, moving right leftward can
  only DECREASE it. That gives a steering rule with no search at all.

CONTRAST WITH two-integer-sum (the unsorted version)
  Unsorted -> hash map of complements, O(n) time, O(n) space, indices preserved.
  Sorted   -> two pointers,             O(n) time, O(1) space, no hash needed.
  Same question, one changed constraint, different optimal answer. Knowing
  WHICH constraint flips the choice is the point of solving both.

BRUTE FORCE (and why it fails)
  Nested loops over all pairs: O(n^2), and it ignores the sortedness entirely.
  A hash map would work too - O(n) space wasted on information the sort order
  already gives you for free.

WHY DISCARDING A POINTER IS SAFE
  If sum > target, then numbers[right] paired with ANY remaining left is still
  too large (every remaining left is >= the current one). So numbers[right]
  cannot be in the answer and can be discarded permanently. Symmetric for the
  other direction. That elimination argument is what makes one pass sufficient
  - be ready to say it out loud, it is the follow-up question.

INVARIANT
  The answer pair, if it exists, always lies within [left, right].

ALGORITHM (NeetCode: "Two Pointers")
  1. left at the smallest value, right at the largest.
  2. Compare their sum with target.
  3. Too big -> right--. Too small -> left++. Equal -> answer.
  4. Pointers cross -> no such pair.

COMPLEXITY
  Time  : O(n) - each step retires one element, so at most n steps.
  Space : O(1) - two indices.

TRIGGER
  A SORTED input plus "find a pair/triple satisfying an arithmetic relation",
  or an explicit O(1) space requirement on a pair-sum problem. The word
  "sorted" in the constraints is the signal.

C# NOTES
  - `new int[] { left + 1, right + 1 }` - the +1 is the problem's 1-based
    indexing, nothing to do with C#. Read the return spec carefully.
  - Array.Empty<int>() returns a cached instance - no allocation for the
    unreachable no-answer path.
  - Compute `sum` once into a local rather than re-adding in each branch;
    clearer, and the JIT does not have to prove the array reads are safe twice.

WATCH OUT
  - `left < right`, not `left <= right` - equality would pair an element with
    itself.
  - This relies on exactly one valid answer existing. With duplicates and
    multiple answers it returns the first the walk reaches, which is fine
    here, and is NOT enough for three-integer-sum - see the dedup logic there.
================================================================================
*/
