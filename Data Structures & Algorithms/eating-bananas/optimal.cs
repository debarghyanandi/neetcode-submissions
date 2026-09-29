// --------------------------------------------------------------------------
// -  optimal.cs            O(n log k) time / O(1) space
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
 PROBLEM : Koko has piles of bananas, piles[i] in pile i, and hoursLimit
           hours. Each hour she picks one pile and eats up to k bananas from
           it. If the pile has fewer than k, she finishes it and waits for the
           next hour. Return the smallest integer k that lets her finish every
           pile in time. Example: piles = [3,6,7,11], h = 8 -> 4
 PATTERN : Binary Search on the Answer (monotonic predicate)
================================================================================
IDEA
  The speed lies between low = 1 and high = max pile. At speed max pile,
  every pile takes exactly 1 hour.
  CanFinish(speed) adds ceil(pile / speed) for each pile and checks the sum
  against hoursLimit.
  The check is monotonic: if a speed works, every faster speed also works.
  So we binary search for the first speed that works. When mid works,
  high = mid keeps it as a candidate. When it fails, low = mid + 1.
EXAMPLE
  piles=[3,6,7,11], h=8. low=1, high=11.
  mid=6: 1+1+2+2=6 ok -> high=6 | mid=3: 1+2+3+4=10 fail -> low=4
  mid=5: 1+2+2+3=8 ok -> high=5 | mid=4: 1+2+2+3=8 ok -> high=4
  low == high = 4 -> return 4
COMPLEXITY
  Time  O(n log k)  log k binary search steps (k = max pile), each scans all n
                    piles
  Space O(1)        only counters low, high, mid, hour
PATH TO OPTIMAL
  Try every speed 1, 2, ..., max pile - O(n * k) - simple but too slow.
  Binary search over the speeds - O(n log k) - uses the monotonic check to
  throw away half of the speeds each step (this file, optimal.cs).
KEYWORDS
  binary search on answer, monotonic predicate, lower bound, ceiling division,
  minimize max rate, Koko eating bananas
WATCH OUT
  - hour is an int. With huge piles and a small speed the sum can overflow
    and turn negative, which looks like "ok". Use long for hour.
  - If you write while (low <= high) together with high = mid, the loop
    never ends. Keep the pair low < high with high = mid.
  - Start low at 1, not 0. Speed 0 means division by zero in CanFinish.
  - The comment "highest size takes lowest time" is misleading. high is the
    max pile because that speed eats each pile in exactly one hour.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Why not use Math.Ceiling with a double?
     -> Integer (pile + speed - 1) / speed gives the same ceiling with no
        floats and no rounding risk. It is also faster. Just watch for overflow.
  2. Can you tighten the search range?
     -> low = ceil(sum / h) is a valid lower bound, and so is 1. This cuts
        iterations, but the complexity is still O(n log k).
  3. Same idea on another problem?
     -> Ship packages within D days, or split array largest sum. Search the
        capacity from max(w) to sum(w) with a greedy feasibility check.
  4. What if hoursLimit < piles.Length?
     -> No speed works, but this code still returns the max pile. Check
        CanFinish(high) first and return -1 if it fails.
TRIGGER
  When you must find the min (or max) value such that a yes/no check passes,
  and the check flips only once as the value grows.
================================================================================
*/
