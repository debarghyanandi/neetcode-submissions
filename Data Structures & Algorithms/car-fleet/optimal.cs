// --------------------------------------------------------------------------
// -  optimal.cs            O(n log n) time / O(n) space
// -  Greedy sort and scan   [greedy-sort-scan]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself (from submission-1)
// -
// -  Sorting dominates; Array.Sort uses O(n log n) introsort, then linear
// -  scan counts fleets by comparing arrival times.
// --------------------------------------------------------------------------

public class Solution
{
    public int CarFleet(int target, int[] position, int[] speed)
    {

        int n = position.Length;
        double[][] map = new double[n][];

        for (int i = 0; i < n; i++)
        {
            map[i] = new double[2];

            double time = (double)(target - position[i]) / speed[i];
            map[i][0] = position[i];
            map[i][1] = time;
        }

        Array.Sort(map, (a, b) => b[0].CompareTo(a[0]));

        int fleet = 0;
        double prevTime = 0;

        foreach (double[] car in map)
        {
            if (car[1] > prevTime)
            {
                fleet++;
                prevTime = car[1];
            }
        }
        return fleet;
    }
}

/*
================================================================================
 PATTERN : Sort + Greedy Scan - merge into the slowest car ahead
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  map       map[i][0] = start position of car i, map[i][1] = its time to reach target
  time      (target - position[i]) / speed[i], hours to target if the car drives alone
  prevTime  arrival time of the fleet just ahead of the current car
  fleet     number of fleets found so far
WHY THIS PATTERN
  A car can never pass the car in front of it. It can only catch up and then
  move at the slower speed. So what matters is the order by position and each
  car's solo arrival time. Sorting map by position from closest-to-target to
  farthest lets one pass decide, for each car, if it catches the fleet ahead
  (prevTime) or stays alone.
BRUTE FORCE
  For each car, scan every car that starts ahead of it and take the largest solo
  arrival time. The car starts a new fleet only if its own time is larger than
  that maximum. This is correct but costs O(n^2) time. It loses because sorting
  once lets the "max time ahead" be kept in one running value.
INVARIANT
  After each car in the loop, prevTime is the arrival time of the fleet directly
  ahead, which is the largest time seen so far. A car with car[1] <= prevTime
  reaches that fleet before the target, so it joins it and nothing changes. A
  car with car[1] > prevTime can never catch it, so it leads a new, slower
  fleet, and prevTime moves up to its time. Each new fleet therefore increments
  fleet exactly once.
WATCH OUT
  prevTime starts at 0 and the test is strict (>). A car that starts exactly at
  target has time 0, so it is never counted, even though it is its own fleet.
  This only works if every position is below target. Equal times merge on
  purpose, because the cars meet exactly at the target. The descending order
  comes only from the lambda b[0].CompareTo(a[0]). If you swap a and b, the scan
  runs from the back of the road and the count is wrong.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you avoid floating-point division completely?
     Compare two cars with cross-multiplication in long: (target - p1) * s2
     against (target - p2) * s1. The math is exact, but the comparison code is
     harder to read and you must store position and speed instead of time.
  2. Positions are whole numbers below target. Can you skip the comparison sort?
     Yes. Use an array of size target indexed by position to hold each car's
     time, then scan it from high index to low. This is O(n + target) time and
     space, so it only wins when target is small compared to n log n.
  3. Car Fleet II asks when each car collides with the next car. What changes?
     Scan from the front car backward with a monotonic stack (a stack kept in
     sorted order). Pop cars ahead that are faster, or that merge into another
     car before you could reach them. Then compute the collision time with the
     new top of the stack. The single prevTime value is no longer enough.
TRIGGER
  Objects move one way on a line, cannot pass each other, and merge when they
  catch up: sort by position and compare arrival times.
C# NOTE
  The double[][] map allocates n separate small arrays just to hold pairs. Two
  parallel arrays sorted with Array.Sort(positions, times), then read in
  reverse, or an array of (int pos, double time) tuples, holds the same data
  with far fewer allocations.
COMPLEXITY
  Time  : O(n log n)
  Space : O(n)
================================================================================
*/
