// --------------------------------------------------------------------------
// -  optimal.cs            O(n log k) time / O(1) space
// -  binary search on eating speed, linear feasibility check
// -  [binary-search-on-speed]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  binary searches speed in [1, max(pile)] and for each candidate does an
// -  O(n) pass summing ceil(pile/speed) hours
// --------------------------------------------------------------------------

public class Solution
{
    public int MinEatingSpeed(int[] piles, int hoursLimit)
    {
        // low and high is the range of speed
        int low = 1; // speed cant be 0;
        int high = 1;
        foreach (int pile in piles)
        {
            // (highest size takes lowest time.)
            high = Math.Max(high, pile); // high is the highest size.
        }

        while (low < high)
        {
            int mid = low + (high - low) / 2;
            //calcualte time required for thas mid value
            if (CanFinish(piles, mid, hoursLimit))
            {
                high = mid;
            }
            else
            {
                low = mid + 1;
            }
        }
        return low;
    }

    private bool CanFinish(int[] piles, int speed, int targetHour)
    {
        int hour = 0;
        foreach (int pile in piles)
        {
            hour += (int)Math.Ceiling((double)pile / speed);
            //  hour += (pile + speed - 1) / speed; 
        }
        return hour <= targetHour;
    }
}

/*
================================================================================
 PATTERN : Binary search on the answer - monotone feasibility
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  piles is never sorted and never needs to be. The binary search runs over the
  candidate answers - eating speeds - not over the array. What makes that legal
  is monotonicity of the predicate: ceil(pile/speed) is non-increasing as speed
  grows, so if CanFinish(piles, s, hoursLimit) is true it is true for every
  speed above s. The boolean sequence over speeds 1, 2, ..., max(piles) is
  therefore F F F ... T T T, and the task is to find the first T.
SEARCH BOUNDS
  low = 1 because speed 0 never finishes any pile, so it can never be the
  answer.

  high = max(piles) because at that speed every pile takes exactly one hour and
  the total is piles.Length hours - the smallest total achievable. No speed
  above max(piles) buys anything, so the answer cannot live past it.

  high is seeded to 1 rather than 0 before the foreach, which keeps low <= high
  even before any pile is examined.
INVARIANT AND WHY IT RETURNS LOW
  The invariant is: the answer is always inside [low, high].

  When CanFinish(mid) is true, mid is still a live candidate - it may be the
  minimum - so the code writes high = mid, not high = mid - 1. When it is false,
  mid is definitely too slow, so low = mid + 1 discards it safely.

  The loop exits at low == high, a single surviving candidate, which is why
  returning low needs no separate best variable and why CanFinish is never
  called on the returned value.
WHY IT TERMINATES
  mid = low + (high - low) / 2 floors toward low, so whenever low < high the
  result satisfies low <= mid < high. The high = mid branch therefore strictly
  shrinks the range rather than parking on the same value, and the low = mid + 1
  branch obviously advances. Writing mid = (low + high + 1) / 2 with this pair
  of updates would hang.
WATCH OUT - THE TWO CEILINGS
  Math.Ceiling((double)pile / speed) is exact here: any int converts to double
  losslessly (doubles are exact through 2^53), so the division cannot round its
  way into an off-by-one hour.

  The commented-out integer form (pile + speed - 1) / speed is the usual
  substitution, but it is not a free swap - with pile and speed both near 1e9,
  pile + speed - 1 exceeds int.MaxValue and wraps negative. Check the
  constraints before uncommenting it.
WATCH OUT - THE ACCUMULATOR
  hour is an int and CanFinish has no early exit, so it always sums across all
  of piles. At a small speed the sum approaches sum(piles): with 1e4 piles of
  1e9 that overflows int, and a wrapped negative makes hour <= targetHour
  spuriously true, which would return a speed that is too slow.

  Either fix closes it: declare hour as long, or return false the moment hour >
  targetHour - the second also stops wasting work on hopeless speeds.
BRUTE FORCE
  Scan speed = 1, 2, 3, ... and return the first one CanFinish accepts. Correct,
  and it uses the identical predicate - the only difference is that it walks
  1..max(piles) linearly instead of halving it, giving O(n * max(piles)). That
  framing is the point: binary search on the answer replaces the scan over the
  answer range, and leaves the feasibility check untouched.
TRIGGER
  Reach for this when the question asks for a minimum or maximum value such that
  some condition holds, the value lives in a contiguous integer range with
  obvious bounds, and the condition flips exactly once across that range. The
  habit worth keeping from this file is the shape: push the condition into its
  own predicate that takes the candidate and the threshold (CanFinish(piles,
  mid, hoursLimit)), so the search body stays three lines and the correctness
  argument splits cleanly into "is the predicate monotone" and "is the range
  right".
COMPLEXITY
  Time  : O(n log k)
  Space : O(1)
================================================================================
*/
