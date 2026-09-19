// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  two-pointer in-place removal   [two-pointer]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Single pass with two pointers: right pointer scans the array, left
// -  pointer places unique elements, each element visited once.
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
 PATTERN : Two Pointers - read/write index on a sorted array
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  l    write index: next slot for a unique value, and the running count of uniques
  r    read index: scans every element from position 1 onward
WHY THIS PATTERN
  The array is already sorted, so equal values sit next to each other and a
  single pass can spot a duplicate by comparing nums[r] with nums[r - 1]. The
  problem asks for an in-place edit plus a length, not a new array, so one
  pointer reads ahead (r) and a slower one writes (l). Because l never runs past
  r, every write lands on a cell that has already been read, so nothing is lost.
BRUTE FORCE
  The natural first attempt is to push each value into a HashSet or List, then
  sort/copy it back into nums and return the count. That is O(n) extra space,
  and with a List it is easy to slip into repeated RemoveAt calls, which cost
  O(n) each and push the whole thing to O(n^2). It loses because the sortedness
  already gives duplicate detection for free, so the extra container buys
  nothing.
INVARIANT
  At the top of each iteration, nums[0..l-1] holds every distinct value seen in
  nums[0..r-1], in sorted order, with no repeats. A new value is written only
  when nums[r] != nums[r - 1], which on a sorted array means nums[r] differs
  from all earlier values, so the invariant survives the write and l++ . When r
  passes the end, the invariant says the first l entries are exactly the
  distinct values, which is what is returned.
WHY THE COMPARISON USES NUMS[R-1] AND NOT NUMS[L-1]
  Comparing against the previous read element works only because the input is
  sorted. If duplicates could be far apart, nums[r - 1] would tell you nothing;
  you would need nums[l - 1] (the last value written) or a set. On sorted input
  both comparisons agree, but nums[r - 1] is the one that reads the untouched
  original neighbour.
WATCH OUT
  Starting at l = 1 and r = 1 silently assumes at least one element exists; on
  an empty array the loop body never runs but the method still returns 1,
  claiming one valid element in a zero-length array. Add an explicit guard if
  nums.Length == 0 return 0. Also note the method mutates the caller's array -
  positions from index l to the end keep stale leftover values, so the caller
  must use only the first l entries and not Length. Finally, the comparison
  depends on the input being sorted; unsorted input compiles and runs but
  returns a wrong count.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Allow each value to appear at most twice instead of once. How does the code
  change?
     Compare nums[r] against nums[l - 2] instead of nums[r - 1] and start both l
     and r at 2. The check becomes "is this at least the third copy", and the
     same single pass still works.
  2. What if the array is not sorted and you may not sort it, but order of the
  survivors does not matter?
     Use a HashSet<int> and write nums[l++] only when Add returns true. That
     restores O(n) time but costs O(n) extra space, which is exactly what the
     two-pointer version avoids.
  3. How would you return the removed duplicates as well, not just the unique
  count?
     Since the array is sorted, the duplicates are whatever you skipped: either
     collect nums[r] into a list inside the else branch, or after the loop note
     that the tail beyond l is garbage and cannot be reused, so collecting
     during the pass is the only cheap option.
TRIGGER
  Sorted input plus "modify in place and return the new length" means a read
  pointer and a write pointer in one pass.
C# NOTE
  nums.Length is read on every loop test rather than cached, which is fine here
  and keeps the bounds check tied to the array itself; note the int[] parameter
  is a reference, so the caller sees the edits without any ref keyword.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
