// --------------------------------------------------------------------------
// -  optimal.cs            O(n log n) time / O(n) space
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
 PROBLEM : Cars sit on a one-lane road at distinct position[i] with speed[i].
           All drive to target. A car can never pass the car ahead. When it
           catches up, it joins that car and they move at the slower speed as
           one fleet. Return the number of fleets that reach target. Example:
           target=10, pos=[0,4], speed=[2,1] -> 1.
 PATTERN : Sort by position + monotonic scan of arrival times
================================================================================
IDEA
  For each car, compute time = (target - position) / speed. This is its
  arrival time if it drove alone. Sort map by position, closest to target
  first. Walk from the front. prevTime is the arrival time of the fleet just
  ahead. A car with car[1] <= prevTime catches that fleet, so it joins it.
  A car with a larger time can never catch it and starts a new fleet. This
  is correct because a car is only blocked by the slowest car ahead of it.
EXAMPLE
  target=12, pos=[10,8,0,5,3], speed=[2,4,1,1,3]
  sorted (pos:time): 10:1, 8:1, 5:7, 3:3, 0:12
  10:1>0 new(1) | 8:1 not>1 joins | 5:7 new(2) | 3:3 joins | 0:12 new(3)
  Answer: 3 (the car at 8 meets the car at 10 exactly at target)
COMPLEXITY
  Time  O(n log n)  Array.Sort dominates; the scan after it is one pass
  Space O(n)        map stores a position and a time for every car
PATH TO OPTIMAL
  Simulate the cars step by step, merging them - slow, and depends on
  distances and speeds - hard to get right with fractional meeting points.
  Sort by position, compare arrival times - O(n log n) - no simulation;
  this file.
KEYWORDS
  car fleet, sorting, arrival time, monotonic stack, greedy, simulation
WATCH OUT
  - Use car[1] > prevTime, not >=. Equal times mean the cars meet at target,
    and that still counts as ONE fleet.
  - Cast before dividing: (double)(target - position[i]). Integer division
    gives wrong times, for example 7/2 becomes 3.
  - Sort in DESCENDING position. If you scan from the back car first, the
    logic breaks, because back cars are blocked by front cars, not the other
    way around.
  - Do not update prevTime when a car joins a fleet. The fleet keeps the
    slower (larger) time of its leader.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it with a stack?
     -> Push the times in sorted order. Pop when the top time is <= the time
        below it. Stack size is the answer. Same O(n log n), more memory.
  2. Positions are integers in [0, target). Can you avoid sorting?
     -> Bucket the times by position in an array of size target, then scan it
        from high to low. O(n + target) time, O(target) space. Good only if
        target is small.
  3. Is floating point safe here?
     -> Division is correctly rounded, so equal fractions give equal doubles.
        To be fully safe, compare (t-p1)*s2 with (t-p2)*s1 in long math.
  4. Car Fleet II: return the time each car hits the car ahead.
     -> Scan from the front with a monotonic stack of cars. Pop cars that are
        faster, or that vanish before we reach them. O(n) after the input order.
TRIGGER
  Objects move one way, cannot pass each other, and merge on catching up:
  sort by position and compare their finish times.
================================================================================
*/
