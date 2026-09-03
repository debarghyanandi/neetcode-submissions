// --------------------------------------------------------------------------
// -  optimal.cs            O(n log k) time / O(1) space
// -  binary search on answer (eating speed), linear feasibility check
// -  [binary-search-on-speed]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself (from submission-0)
// -
// -  binary searches the speed range [1, max(piles)] and for each candidate
// -  does an O(n) pass to sum ceiling division hours
// --------------------------------------------------------------------------

public class Solution {
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
 PATTERN : Binary search on the answer - monotone feasibility test
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The unknown is a speed, not a position. Nothing here is sorted - piles arrives
  in any order and the code never sorts it, and no branch depends on where a
  pile sits. What is sorted is the implicit array of candidate answers: the
  speeds 1..max(pile), indexed by themselves. So the search runs over the value
  range and CanFinish plays the role of the comparison. That piles is only ever
  touched by the max scan and by each full CanFinish pass is the tell that this
  is a search on the answer rather than a search in the input.
THE MONOTONE PREDICATE
  CanFinish(piles, speed, hoursLimit) is monotone in speed: raising speed can
  only shrink each ceil(pile / speed) term, so hour is non-increasing in speed,
  and once hour <= targetHour holds it holds for every larger speed. Read across
  speed = 1..high the predicate is false,false,...,false,true,true, and the
  answer is the first true. That step shape is what licenses halving the range -
  it is the correctness argument to give an interviewer, not the loop mechanics.
WHY CEIL PER PILE, AND WHY HIGH = MAX(PILE)
  hour accumulates ceil(pile / speed) pile by pile, never ceil(total / speed).
  Koko eats from one pile in an hour and throws away the remainder of that hour,
  so a partial hour does not carry into the next pile; summing first and
  dividing once would silently model a different, easier problem and
  under-count. The same rounding fixes the ceiling: at any speed above
  max(pile), every pile still costs one full hour, so faster speeds buy nothing.
  high = max(pile) is therefore a legal upper bound, and given the problem's
  guarantee that hoursLimit >= piles.Length it is also a feasible one - which
  the invariant below depends on.
INVARIANT AND TERMINATION
  Invariant: the minimum feasible speed lies in [low, high], high is always a
  feasible speed, and every value below low has been proven infeasible. Feasible
  mid -> high = mid, because mid is still a candidate and must not be discarded.
  Infeasible mid -> low = mid + 1, because mid itself is ruled out. mid = low +
  (high - low) / 2 floors toward low, so when high == low + 1 you get mid == low
  and the branch either pulls high down to low or pushes low up to high; the
  interval strictly shrinks every pass and the loop cannot spin. The classic
  hang is pairing high = mid with low = mid on the other side - it stalls at
  exactly this two-element state. return low needs no final re-check: low ==
  high, and high was feasible by the invariant.
WATCH OUT
  CanFinish has no early exit - hour keeps summing across all piles long after
  it has passed targetHour. speed == 1 is genuinely reachable (once low == 1 and
  high == 2, mid == 1), and there hour equals the full sum of piles, the largest
  value the accumulator can ever hold. Nothing in the code bounds that int
  against overflow; either declare hour as long or break the moment hour >
  targetHour, both free. The commented-out (pile + speed - 1) / speed is the
  same ceiling without routing through double - that is the form to write if
  asked to avoid floating point, and the one to reach for if the operands ever
  grow.
TRIGGER
  "Smallest X such that some cost fits inside a budget", where a candidate X can
  be checked cheaply with one pass and the cost moves monotonically in X. Ship
  capacity within D days, split-array-largest-sum, and minimum days to make m
  bouquets are the identical skeleton: bracket the answer with a
  trivially-infeasible low and a trivially-feasible high, write the predicate,
  then it is only boundary discipline.
COMPLEXITY
  Time  : O(n log k)
  Space : O(1)
================================================================================
*/
