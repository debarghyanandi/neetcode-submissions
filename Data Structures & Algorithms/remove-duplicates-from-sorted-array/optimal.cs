// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  two-pointer, in-place overwrite   [two-pointer-inplace]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself (from submission-0)
// -
// -  single pass with a slow pointer tracking the next write position while
// -  scanning for distinct values
// --------------------------------------------------------------------------

public class Solution {
    public int RemoveDuplicates(int[] nums) {
        int left = 1;
        for (int right = 1; right < nums.Length; right++)
        {
            if (nums[right] != nums[right - 1])
            {
                nums[left] = nums[right];
                left++;
            }
        }
        return left;
    }
}

/*
================================================================================
 PATTERN : Two Pointers - slow write index, fast scan index
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The array is sorted, so every run of equal values is contiguous. That single
  fact turns "is this a duplicate?" from a set-membership question into a
  comparison with the immediate neighbor. No HashSet, no second array: one scan
  pointer (right) reads, one write pointer (left) packs the survivors toward the
  front. The pattern to recall is not "two pointers" in the
  meeting-in-the-middle sense but the read/write split - right always runs ahead
  of or level with left, so the region behind left is finished output and the
  region at or ahead of right is untouched input.
INVARIANT
  At the top of each iteration: nums[0 .. left-1] holds the distinct values seen
  so far, in sorted order, and left is exactly how many there are. Because left
  is both the count and the next write slot, the function can just return left
  at the end - no separate counter. Combined with left <= right at all times,
  the write nums[left] = nums[right] can never destroy an input element that has
  not been consumed yet.
WHY THE COMPARISON IS SAFE
  The test is nums[right] != nums[right - 1], comparing against a slot that the
  loop may itself have written. Argue it does not matter. left <= right always.
  If left == right the write is a self-assignment. If left < right, the deepest
  slot ever written in iteration right is index right - 1 (when left == right -
  1), and that write happens after the comparison for this iteration has already
  been read. Index right is never written during iteration right in that case,
  since left only reaches right after the increment. So every comparison reads
  an original input value. The common alternative, nums[right] != nums[left - 1]
  (compare against the last value kept), sidesteps the question entirely and is
  worth having ready if an interviewer pushes on it.
TRACE
  nums = [0,0,1,1,1,2,2,3,3,4]
  right=1: 0 == 0, skip. left stays 1.
  right=2: 1 != 0, write nums[1]=1, left=2.
  right=3,4: 1 == 1, skip.
  right=5: 2 != 1, write nums[2]=2, left=3.
  right=6: skip. right=7: 3 != 2, write nums[3]=3, left=4.
  right=8: skip. right=9: 4 != 3, write nums[4]=4, left=5.
  Return 5, with nums beginning 0,1,2,3,4. The tail [2,2,3,3,4] is stale and
  that is allowed - the contract only defines the first k slots.
WATCH OUT
  left = 1 hardcodes the assumption that a first element exists. If nums.Length
  == 0 the loop body never runs and this returns 1 for an empty array, claiming
  one unique element that is not there. LeetCode 26 constrains 1 <= nums.length,
  so the judge never exercises it, but state that assumption out loud rather
  than letting an interviewer find it. The fix is a guard returning 0, or
  starting left at 0 and using the compare-against-nums[left-1] formulation with
  an explicit first-element case.

  Second trap: the loop starts at right = 1, not 0. Starting at 0 would read
  nums[-1].
INTERVIEW FOLLOW-UP
  Allow each value at most twice (LeetCode 80): keep the same skeleton, start
  left at 2 and right at 2, and compare nums[right] != nums[left - 2]. That
  generalizes to at most k copies with left starting at k and the comparison
  against nums[left - k] - and note that version must compare against left, not
  right, because the kept window is what defines admissibility. Related
  question: why does this fail on unsorted input? Because the neighbor
  comparison only detects duplicates that are adjacent; unsorted input needs a
  HashSet and gives up the O(1) space.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
