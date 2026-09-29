// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Dynamic programming, space-optimized, circular split
// -  [dp-space-optimized-circular]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Circular constraint split into two linear DP passes using rolling
// -  variables instead of a table.
// --------------------------------------------------------------------------

public class Solution
{
    public int Rob(int[] nums)
    {
        int n = nums.Length;

        if (n == 1)
            return nums[0];

        if (n == 2)
            return Math.Max(nums[0], nums[1]);

        int first = RobLinear(nums, 0, n - 2);
        int second = RobLinear(nums, 1, n - 1);

        return Math.Max(first, second);
    }


    private int RobLinear(int[] nums, int start, int end)
    {
        //Dp space Optimized

        int prev2 = nums[start];
        int prev = Math.Max(nums[start], nums[start + 1]);
        int curr = start;

        for (int i = start + 2; i <= end; i++)
        {

            int pick = nums[i] + prev2;
            int notPick = 0 + prev;
            curr = Math.Max(pick, notPick);

            prev2 = prev;
            prev = curr;

        }
        return prev;

    }
}

/*
================================================================================
 PATTERN : 1-D DP, circle split into two linear House Robber runs
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  first    best loot from houses 0..n-2 (last house left out)
  second   best loot from houses 1..n-1 (first house left out)
  prev2    best loot over start..i-2
  prev     best loot over start..i-1; after the loop, the answer for the range
  pick     loot if house i is robbed: nums[i] + prev2
  notPick  loot if house i is skipped: prev
WHY THIS PATTERN
  The houses are in a circle, so the first and last houses are neighbours. That
  means you can never rob both. Every valid plan leaves out house 0 or house n-1
  (or both), so it fits inside range 0..n-2 or range 1..n-1. Inside each range
  the houses form a normal line with no wrap-around. RobLinear solves each line
  with the classic choice "rob this house or skip it", and Math.Max(first,
  second) picks the better of the two cases.
BRUTE FORCE
  Try every subset of houses. Throw out any subset with two neighbours in it,
  and any subset with both house 0 and house n-1. Keep the largest sum. This is
  O(2^n) because every house is in or out. A plain recursion with no memo, "rob
  i or skip i", has the same exponential cost, because it solves the same
  suffixes again and again.
INVARIANT
  When the loop in RobLinear reaches index i, prev holds the best loot over
  start..i-1 and prev2 holds the best loot over start..i-2. The best plan for
  start..i either skips house i (its value is prev) or robs it (then house i-1
  must be skipped, so its value is nums[i] + prev2). Nothing else is possible,
  so curr = Math.Max(pick, notPick) is exactly the best loot for start..i.
  Shifting prev2 = prev and prev = curr sets up the same statement for i+1. When
  the loop ends, prev covers start..end.
SEEDS ARE THE FIRST TWO HOUSES OF THE RANGE
  prev2 starts at nums[start], not at 0. prev starts at Math.Max(nums[start],
  nums[start + 1]), which is the best for a range of two houses. So the loop
  starts at start + 2, not at 0. This is why the second run, which begins at
  index 1, needs no extra offset logic.
WATCH OUT
  RobLinear assumes its range holds at least two houses, because it always reads
  nums[start + 1]. That is why the n == 2 guard is needed. Without it, the call
  RobLinear(nums, 0, 0) would read nums[1], a house outside its range, and could
  return nums[0] + nothing or just nums[1]. That breaks the "house 1 is left
  out" rule. An empty array also crashes: n == 0 passes both guards and then
  reads nums[1]. curr is set to start, which is an index and not an amount of
  money. It works only because curr is always overwritten before anyone reads
  it. Returning curr in place of prev would give a wrong answer when the loop
  does not run.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you return which houses were robbed, not just the total?
     Keep the full dp array for each run, then walk backwards. If dp[i] !=
     dp[i-1], house i was robbed, so jump to i-2. Otherwise move to i-1. This
     costs O(n) extra space in place of O(1).
  2. What if the houses form a binary tree, and you cannot rob a parent and its
  child (House Robber III)?
     Do a post-order DFS that returns a pair (best if this node is robbed, best
     if it is skipped). robbed = val + left.skip + right.skip, and skipped = max
     of left's pair + max of right's pair. Time is O(n). Space is O(h) for the
     recursion stack.
  3. What if a robbed house blocks its k neighbours on each side, not just one?
     pick becomes nums[i] + best[i-k-1]. Keep the last k+1 best values in a ring
     buffer. That is O(k) space in place of two variables. For the circle, the
     split into ranges must drop more houses at the edges.
TRIGGER
  If the problem is a line of items where picking one blocks its neighbour, and
  the ends wrap into a circle, solve it twice on the line: once without the
  first item and once without the last.
C# NOTE
  You could pass nums.AsSpan(0, n - 1) and nums.AsSpan(1) (a ReadOnlySpan<int>
  is a view over part of the array, with no copy) to a helper that always starts
  at index 0. This removes the start/end offset math that RobLinear has to do.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
