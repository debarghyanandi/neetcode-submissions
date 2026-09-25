// ##########################################################################
// #  optimal.cs            O(n) time / O(1) space
// #  sliding window, shrink while valid   [two-pointer-shrink]
// #  ranks above suboptimal.cs (O(n) time / O(n) space)
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Each element is added and removed from the sum at most once as the two
// #  pointers traverse the array.
// ##########################################################################

public class Solution
{
    public int MinSubArrayLen(int target, int[] nums)
    {
        int sum = 0;
        int left = 0;
        int result = int.MaxValue;

        for (int right = 0; right < nums.Length; right++)
        {
            sum = sum + nums[right];   // admit the new right-hand element

            // Shrink while the window still qualifies - the first failure
            // means every shorter window ending here fails too.
            while (sum >= target)
            {
                result = Math.Min(result, right - left + 1);
                sum = sum - nums[left];  // evict, THEN move the edge
                left++;
            }
        }

        return result == int.MaxValue ? 0 : result;
    }
}

/*
================================================================================
 PATTERN : Sliding Window - shrink while the sum still qualifies
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  sum      sum of nums[left..right]
  left     left edge of the current window
  result   shortest qualifying length seen so far
WHY THIS PATTERN
  The problem asks for the shortest contiguous block whose sum reaches target,
  and all values are positive. Positive values mean sum grows when right
  advances and shrinks when left advances, so the window length and the sum move
  in opposite directions in a predictable way. That lets left move forward only,
  never back: once sum drops below target we know nothing shorter ending at this
  right can work. So each index enters and leaves the window once.
BRUTE FORCE
  Fix every start i, walk j forward adding nums[j], and stop at the first j
  where the running sum reaches target; keep the smallest j - i + 1. That is
  O(n^2) time and correct, but it recomputes sums that overlap heavily between
  consecutive starts. The window version reuses the same sum and never re-adds
  an element.
INVARIANT
  At the top of each right iteration, sum equals the total of nums[left..right]
  exactly, because every add is paired with a matching subtract before left
  moves. The inner while exits only when sum < target, so after it runs, left is
  the largest start for which the window ending at right still reaches target.
  Since result took Math.Min at every qualifying length, and every right is
  visited, the minimum over all valid windows is recorded.
MEASURE BEFORE EVICT
  Inside the while, result is updated first, then nums[left] is subtracted and
  left incremented. The order matters: right - left + 1 must be read while the
  window is still the qualifying one. If left were advanced before the Math.Min,
  the recorded length would be one too small and the answer could be wrong by
  one.
WATCH OUT
  This relies on all nums being positive; with a zero or a negative value the
  inner loop's assumption breaks, because shrinking no longer reliably lowers
  sum and left could need to move back. sum is an int, so a long array of large
  values can overflow before target is ever compared - a long sum removes that
  risk. The final ternary on int.MaxValue is what distinguishes "no window
  qualifies" from a real answer, so returning result directly would return
  int.MaxValue on an array whose total is below target.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if nums can contain negative numbers?
     Sliding window fails. Use prefix sums with a sorted structure or monotonic
     deque over prefix values to find the shortest range with sum >= target,
     which costs O(n log n) time and O(n) space.
  2. What if you must return the subarray itself, not its length?
     Store bestLeft = left alongside each Math.Min improvement, then slice with
     a range or Array.Copy at the end. Same time, plus O(k) for the copy.
  3. The array is huge and arrives as a stream you cannot index twice.
     The logic already works in one pass, but you must buffer the current window
     to evict from its left end - a Queue<int> of the in-window values replaces
     nums[left], and memory becomes proportional to the longest window, not the
     whole stream.
  4. Longest subarray with sum <= target instead?
     Same two pointers, but shrink while sum > target and record right - left +
     1 after the while, not inside it, because the valid window is the one that
     exists once the violation is gone.
TRIGGER
  Shortest or longest contiguous run under a threshold on non-negative values,
  where extending one end always pushes the metric one way.
C# NOTE
  int.MaxValue as the "not found" sentinel works only because Math.Min never
  returns something larger, so the single ternary at the end is enough; int?
  result with null checks would cost a comparison per update for no gain here.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
