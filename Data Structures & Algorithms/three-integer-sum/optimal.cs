// ##########################################################################
// #  optimal.cs            O(n^2) time / O(1) space
// #  sort + fix anchor + two pointers   [sort-fix-two-pointers]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  sorts then fixes each element as anchor, using a two-pointer scan over
// #  the remaining sorted suffix to find pairs summing to the negated
// #  anchor, skipping duplicates for both anchor and pair values
// ##########################################################################

public class Solution
{
    public List<List<int>> ThreeSum(int[] nums)
    {
        var triplets = new List<List<int>>();

        // Sorting enables BOTH halves of this solution:
        //   - the two-pointer scan needs monotonic order
        //   - duplicate skipping needs equal values to sit adjacent
        Array.Sort(nums);

        for (int i = 0; i < nums.Length; i++)
        {
            // Sorted ascending: once the anchor is positive, the two larger
            // values behind it are positive too, so no sum can reach zero.
            if (nums[i] > 0)
                break;

            // Skip a repeated ANCHOR - it would regenerate identical triplets.
            // i > 0 guard: index 0 has no predecessor to compare against.
            if (i > 0 && nums[i] == nums[i - 1])
                continue;

            FindPairsWithSum(nums, i + 1, nums.Length - 1, -nums[i], nums[i], triplets);
        }

        return triplets;
    }

    // Classic sorted two-pointer two-sum, restricted to the window [left, right],
    // appending every matching triplet directly into `triplets`.
    private void FindPairsWithSum(int[] nums, int left, int right, int target, int anchor, List<List<int>> triplets)
    {
        while (left < right)
        {
            int sum = nums[left] + nums[right];

            if (sum == target)
            {
                triplets.Add(new List<int> { anchor, nums[left], nums[right] });

                // Both pointers must clear their whole duplicate block, or the
                // very next iteration re-emits the same triplet.
                int usedLeftValue = nums[left];
                int usedRightValue = nums[right];

                while (left < right && nums[left] == usedLeftValue)
                    left++;

                while (left < right && nums[right] == usedRightValue)
                    right--;
            }
            else if (sum > target)
            {
                right--;
            }
            else
            {
                left++;
            }
        }
    }
}

/*
================================================================================
 PATTERN : Sort, anchor each index, two-pointer the suffix
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  Three unknowns is one too many to search directly. Fixing nums[i] as the
  anchor turns the rest into a plain two-sum on a sorted array: find a pair in
  [i+1, nums.Length-1] summing to -nums[i], which is exactly the target passed
  into FindPairsWithSum. Sorting is what buys that reduction, and the code's own
  comment names both payoffs - monotonic order for the pointer walk, and equal
  values sitting adjacent so duplicates can be skipped by comparing neighbours
  instead of hashing triplets into a set.
INVARIANT
  Inside FindPairsWithSum, every valid pair that has not yet been emitted has
  both of its indices inside the window [left, right]. Each branch of the loop
  shrinks that window by one index only after proving the discarded index cannot
  participate in any remaining answer. The outer loop carries its own invariant:
  every triplet whose smallest element sits at an index < i has already been
  emitted, so the search never needs to look backwards - which is why the window
  starts at i+1 and never at 0.
WHY THE DISCARD IS SAFE
  This is the correctness argument an interviewer will push on. If sum > target,
  then nums[right] is doomed: every remaining partner index k in [left, right-1]
  has nums[k] >= nums[left] by sortedness, so nums[k] + nums[right] >=
  nums[left] + nums[right] = sum > target. No pair using right can ever hit
  target, so right-- discards it losslessly. The sum < target case is the mirror
  image - every partner of nums[left] is <= nums[right], so left is the doomed
  one. Neither branch is a heuristic; each eliminates a whole row or column of
  the pair matrix with one proof.
DEDUP IN THREE PLACES
  There is no HashSet anywhere; uniqueness is structural, and it takes all three
  skips.
  1. The anchor skip: i > 0 && nums[i] == nums[i-1] continues, so a repeated
  smallest value never re-runs the same suffix scan. The i > 0 guard exists only
  because index 0 has no predecessor to read.
  2 and 3. After a hit, left is walked past every copy of usedLeftValue and
  right past every copy of usedRightValue. The snapshot into local variables
  before moving matters: nums[left] changes as left advances, so comparing
  against the live nums[left] would stop after one step.
  Note that clearing only one side would already be enough to avoid re-emitting
  the same value pair - once left leaves its block, nums[left] is strictly
  larger and the pair is a different pair. Clearing both is the symmetric
  version and skips the run of equal right values in one pass instead of
  decrementing through them one comparison at a time. Both inner whiles keep the
  left < right guard so the pointers cannot cross or run off the window.
WHY THE ANCHOR SKIP KEEPS THE FIRST COPY
  Easy to get backwards. The condition compares against nums[i-1] and continues,
  which means the FIRST occurrence of each value is the one that runs. That is
  required, not stylistic: for input like [-2,-2,0,4] the answer [-2,-2,4] needs
  the second -2 to still be inside the scan window i+1..end. Anchoring on the
  first -2 leaves it there; skipping to the last copy would drop that triplet.
  Same reasoning is why nums[i] > 0 is a break and not a continue - once the
  anchor is positive, so are the two larger values behind it in sorted order,
  and no suffix can sum to zero for this or any later i.
WATCH OUT
  Array.Sort(nums) sorts the caller's array in place. The method has a visible
  side effect on its argument; if the caller cares about original order, copy
  first.
  anchor and target are redundant parameters - target is always -anchor.
  Harmless, but if you change one call site and not the other, the emitted
  triplet and the pair it was matched against silently disagree.
  sum is an int; this is safe only because 3Sum's constraints cap |nums[i]| at
  10^5, so nums[left] + nums[right] cannot overflow. Loosen that constraint and
  it needs a long.
  The two inner skip loops must both use left < right, not left < nums.Length.
  Without it, the left pointer can run past right and the outer while re-reads a
  crossed window.
FOLLOW-UPS
  "Why not a HashSet of triplets?" It works but costs space proportional to the
  output and forces a canonical ordering of each triplet to hash correctly;
  sorting already gives that ordering for free and the skips make the set
  unnecessary.
  "Can you avoid sorting?" Only by paying for it elsewhere - a hash-based
  approach hits the same time bound and then has to solve deduplication the hard
  way, which is precisely the part sorting makes trivial.
  "Extend to 4Sum." Add another outer anchor loop with the same neighbour-skip
  guard and call the same two-pointer routine; the recursion generalizes to
  k-sum with k-2 nested anchors.
  "Return indices instead of values." This structure cannot - sorting destroys
  the original indices, and the dedup rule is defined on values, not positions.
COMPLEXITY
  Time  : O(n^2)
  Space : O(1)
================================================================================
*/
