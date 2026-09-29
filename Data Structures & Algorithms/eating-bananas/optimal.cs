// --------------------------------------------------------------------------
// -  optimal.cs            O(n log k) time / O(1) space
// -  binary search on answer   [binary-search-answer]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Binary search over speeds [1, max_pile] with O(n) feasibility check
// -  per iteration
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
 PATTERN : Binary Search on the Answer - smallest speed that works
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  low     smallest speed still possible (starts at 1)
  high    a speed known to work; starts at the biggest pile
  mid     the speed being tested this round
  hour    total hours needed at this speed, summed over all piles
WHY THIS PATTERN
  The problem asks for the minimum speed that finishes within hoursLimit. The
  check is monotonic, which means it only changes once: if speed s is fast
  enough, every speed above s is also fast enough. So the answer is the first
  "true" in a sorted range of speeds from low = 1 to high = max pile. Binary
  search finds that first true by calling CanFinish on each mid.
BRUTE FORCE
  Try speed = 1, 2, 3, ... and return the first one where CanFinish is true.
  Each check costs O(n), and you may need up to max(piles) checks, so it is O(n
  * max pile). That is correct, but it is linear in the value of the biggest
  pile. Binary search needs only log(max pile) checks.
INVARIANT
  The answer always stays inside [low, high]. high always works: at the start, a
  speed equal to the biggest pile finishes each pile in 1 hour, and later high
  only moves to a mid that passed. Every speed below low has failed: low only
  moves to mid + 1 after mid failed, and monotonicity means everything below mid
  also fails. When low == high, the range holds one value, and it is the
  smallest speed that works.
HIGH = MID, NOT MID - 1
  When mid passes, it may be the answer itself, so the code keeps it with high =
  mid. The loop condition is low < high, not low <= high, and mid rounds down.
  Together these make sure the range shrinks every round and the loop cannot get
  stuck.
WATCH OUT
  hour is an int. With many piles and a small speed such as 1, the sum can
  overflow and turn negative, so CanFinish would wrongly return true. Use long,
  or return false as soon as hour > targetHour. The commented-out line (pile +
  speed - 1) / speed avoids floating point, but pile + speed - 1 can overflow
  when pile is close to int.MaxValue. The comment "highest size takes lowest
  time" is unclear. What the code actually uses is that a speed equal to the
  biggest pile always works. If hoursLimit < piles.Length, no speed can work,
  but the code still returns the max pile and does not report failure.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you make the search range tighter?
     Yes. A lower bound is ceil(sum / hoursLimit), because you must eat
     everything in hoursLimit hours. This cuts some iterations, but it costs one
     more pass to get the sum and needs a long to avoid overflow.
  2. What if Koko could eat from more than one pile in the same hour?
     Then the hour count is simply ceil(total / speed). The check becomes O(1)
     after one sum, and you can compute the answer directly with no binary
     search.
  3. Where else does this template apply?
     Ship packages within D days, split array largest sum, minimum days to make
     bouquets. Each has a monotonic feasibility check and asks for the min or
     max value that passes. Only the check function and the bounds change.
TRIGGER
  The problem asks for the minimum (or maximum) value such that a yes/no check
  passes, and making the value bigger never turns a "yes" into a "no".
C# NOTE
  Math.Ceiling((double)pile / speed) converts to double and back just to round
  up. The integer form (pile - 1) / speed + 1 gives the same result for pile >=
  1 with no floating point and no overflow risk.
COMPLEXITY
  Time  : O(n log k)
  Space : O(1)
================================================================================
*/
