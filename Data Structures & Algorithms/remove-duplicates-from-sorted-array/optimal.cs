// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Two-pointer, move unique elements   [two-pointer-duplicates]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Single forward pass with left pointer marking insertion position for
// -  unique elements.
// --------------------------------------------------------------------------

public class Solution
{
    public int RemoveDuplicates(int[] nums)
    {
        int l = 1;
        for (int r = 1; r < nums.Length; r++)
        {
            if (nums[r] != nums[r - 1])
            {
                nums[l] = nums[r];
                l++;
            }
        }
        return l;
    }
}

/*
================================================================================
 PATTERN : Two Pointers - slow write index, fast read index
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  l    next write position; also the count of unique values kept so far
  r    read cursor scanning for a value different from its left neighbour
WHY THIS PATTERN
  The array is already sorted, so equal values sit next to each other and one
  comparison with nums[r - 1] is enough to know if nums[r] is new. The problem
  also asks for the work to happen in place and to return a length, not a new
  array. Two indices over the same array fit that: r reads every element once, l
  marks where the next kept element goes. Everything before index l is the
  answer, everything at or after it is free space.
BRUTE FORCE
  The first idea most people write is to copy into a HashSet or a List, then
  sort or write the distinct values back and return the count. That is O(n)
  extra memory and gives up the fact that duplicates are already grouped.
  Another naive version deletes a duplicate by shifting the rest of the array
  left, which is O(n^2) time when many duplicates exist.
INVARIANT
  Before each iteration, nums[0..l-1] holds the unique values seen in
  nums[0..r-1], in their original sorted order, and l >= 1. A new value is
  detected only by comparing nums[r] with nums[r - 1] in the original array -
  the comparison never reads the already-rewritten region, so overwriting is
  safe. Since l only ever grows when a genuinely new value appears, l equals the
  number of unique values when the loop ends, which is exactly what is returned.
WRITE POSITION NEVER PASSES THE READ POSITION
  l starts at 1 and increases only on a new value, while r increases every step,
  so l <= r always. That means nums[l] = nums[r] either writes onto a slot
  already consumed or writes an element onto itself. No unread element is ever
  destroyed, which is why no temporary copy is needed.
WATCH OUT
  If nums is empty, the loop never runs and the method returns 1, claiming one
  unique element in an array with none. If nums is null, nums.Length throws a
  NullReferenceException before any check. Both cases are silent here because
  the code relies on an unwritten assumption that the array has at least one
  element; a guard like if (nums.Length == 0) return 0; makes that explicit.
  Also, the code assumes sorted input - on unsorted data it compares only
  neighbours and will keep duplicates that are not adjacent.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What changes if each value may appear at most twice?
     Start l at 2 and r at 2, and compare nums[r] with nums[l - 2] instead of
     nums[r - 1]. The same one-line change generalises to at most k copies, with
     l and r starting at k.
  2. The caller wants the leftover slots cleaned instead of left as garbage.
     After the loop, fill nums from l to nums.Length - 1 with a sentinel, for
     example Array.Clear(nums, l, nums.Length - l). Still O(1) extra space, but
     now O(n) writes even when there are no duplicates.
  3. What if the input is a sorted linked list instead of an array?
     Keep a single pointer and unlink the next node when its value equals the
     current node's value. There is no write index because nodes move by pointer
     reassignment, not by copying.
  4. What if the input is too large to fit in memory, for example a sorted file?
     Stream it: read one value at a time, keep only the previous value, and emit
     a value when it differs. Same comparison logic, but the output goes to
     another stream so the in-place property is lost.
TRIGGER
  Input is sorted and you must compact or filter it in place while returning a
  new length - reach for a write index trailing a read index.
C# NOTE
  int[] is a reference type, so the caller sees the mutation of nums directly
  and only the count has to be returned. A LINQ one-liner such as
  nums.Distinct().ToArray() reads nicer but allocates a new array and a hash
  set, which breaks the in-place requirement.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
