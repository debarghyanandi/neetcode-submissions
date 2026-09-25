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
 PATTERN : Binary search on the answer - monotone feasibility check
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  low       smallest speed still possible, 1 because speed 0 never finishes
  high      largest speed worth trying = the biggest pile
  mid       candidate speed being tested this round
  hour      hours needed at the candidate speed, summed over all piles
WHY THIS PATTERN
  We are asked for the smallest speed that finishes inside hoursLimit, and the
  total time is monotone: if a speed works, every faster speed also works. That
  turns the search for a number into a yes/no question, CanFinish(piles, mid,
  hoursLimit), asked over the sorted range of speeds 1..max(pile). Binary search
  narrows [low, high) until the two meet on the first speed where the answer
  flips from false to true.
BRUTE FORCE
  The first thing you would write is a loop over speed = 1, 2, 3, ... calling
  CanFinish until it returns true. That is correct, because the same monotone
  property holds, but it costs O(n * k) where k is the largest pile - it walks
  the whole speed range instead of halving it. It only matters when piles hold
  large values, which is exactly when it dies.
INVARIANT
  At the top of every while iteration, the answer lies in [low, high]: low is
  never above the true answer, and high always satisfies CanFinish. Setting high
  = mid keeps a known-good speed in range; setting low = mid + 1 discards a
  speed proven too slow. Because the window always shrinks by at least one and
  only holds valid candidates, when low == high that single value is the
  smallest feasible speed, so returning low is safe.
WHY HIGH = MAX(PILE) IS ENOUGH
  At a speed equal to the largest pile, every pile takes exactly one hour, so
  the total is piles.Length hours - the lowest total any speed can reach. No
  speed above the largest pile helps, so the search space stops there and never
  needs an artificial cap. The initial high = 1 also doubles as a floor, so high
  never drops below the legal minimum speed.
THE COMMENTED-OUT CEILING
  The live line uses (int)Math.Ceiling((double)pile / speed); the commented
  alternative (pile + speed - 1) / speed does the same thing with integers. They
  are not equal in edge cases: if a pile is close to int.MaxValue, pile + speed
  - 1 overflows and gives a wrong, tiny hour count. The double version has no
  such overflow because every int converts exactly to double.
WATCH OUT
  hour is an int and, at speed 1, it sums to the total number of bananas; a
  large enough total silently overflows and can make CanFinish return true for a
  speed that is far too slow. There is no feasibility guard: if hoursLimit is
  smaller than piles.Length the task is impossible, but the loop still returns
  max(pile) instead of signalling failure. An empty piles array leaves high = 1
  and returns 1 without entering the loop. Also note mid is computed as low +
  (high - low) / 2 on purpose - low + high would be the overflow-prone form.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Why is the loop condition low < high and not low <= high?
     The half-open form needs no separate variable for the best answer found:
     high is always a feasible speed, so the meeting point is the answer. With
     low <= high you must store a result on each success and return that, or you
     loop forever when high = mid.
  2. The hour total can overflow. How do you fix it while keeping the same
  structure?
     Make hour a long, or return early from CanFinish as soon as hour >
     targetHour. The early exit also saves work on slow speeds, since it stops
     before touching every pile.
  3. Variant - Koko may eat from several piles in the same hour, up to speed
  bananas per hour total. What changes?
     The problem stops being per-pile and becomes ceil(total bananas /
     hoursLimit), a single arithmetic step with no search at all. The binary
     search only earns its place because leftover capacity inside an hour is
     wasted.
  4. What if piles were given as a stream you can read only once?
     You can still find the maximum in that one pass, but CanFinish needs to
     re-read the data on every candidate speed, so you must buffer it or accept
     O(log k) passes over the stream.
TRIGGER
  The question asks for the smallest or largest value that satisfies a
  condition, and checking one candidate value is cheap and monotone.
C# NOTE
  CanFinish is a separate private method that re-receives piles on every call; a
  local function inside MinEatingSpeed could capture piles directly and take
  only speed, which keeps the signature honest about what actually varies. The
  max-finding foreach could be piles.Max() from LINQ, but the plain loop avoids
  the delegate-free-yet-still-enumerator walk and keeps the 1 floor explicit.
COMPLEXITY
  Time  : O(n log k)
  Space : O(1)
================================================================================
*/
