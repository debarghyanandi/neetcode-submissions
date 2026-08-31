// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  hash set, walk runs from left edge only   [hashset-sequence-start]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Builds a HashSet for O(1) membership, only starts walking a run when
// -  number-1 is absent so each element is visited by exactly one run-walk,
// -  giving amortized O(n) total work.
// --------------------------------------------------------------------------

public class Solution
{
    public int LongestConsecutive(int[] nums)
    {
        // O(1) membership tests, and duplicates collapse for free.
        var numberSet = new HashSet<int>(nums);

        int longestRun = 0;

        foreach (int number in numberSet)
        {
            // Only start counting from the LEFT EDGE of a run.
            // If number - 1 exists, this number is mid-run and some earlier
            // (or later) iteration will count the run that contains it.
            if (numberSet.Contains(number - 1))
                continue;

            int runLength = 0;

            while (numberSet.Contains(number + runLength))
            {
                runLength++;
            }

            if (runLength > longestRun)
                longestRun = runLength;
        }

        return longestRun;
    }
}

/*
================================================================================
 PATTERN : Hash set membership + scan only from run starts
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The answer depends only on which values are present, never on their order or
  index, so sorting buys information the problem does not need. Once every value
  is in numberSet, the only question the algorithm ever asks is "is number + 1
  here", and a consecutive run is just a chain you can walk one Contains call at
  a time.
ALTERNATIVES AND WHY THEY LOSE
  Sort, then scan adjacent pairs while skipping equal neighbours: correct, but
  it pays a full ordering to answer a membership question, and it needs an
  explicit duplicate case. Union-find over value/value+1 edges: also correct,
  but it builds a structure to represent runs the set already encodes
  implicitly. Set plus a forward walk from every element and no start guard:
  correct too, and it is the version that quietly blows up (see below).
INVARIANT
  Every distinct run is counted exactly once, starting from its smallest
  element. The guard numberSet.Contains(number - 1) is precisely the test
  "number is the minimum of its run": a run has exactly one such element, so no
  run is missed and none is measured twice. This is why HashSet enumeration
  order does not matter - being a run start is a property of the value, not of
  when the foreach happens to reach it. The comment's "earlier (or later)
  iteration" is doing that work.
WHY THE NESTED WHILE IS NOT A NESTED SCAN
  The while body only ever runs for values that survived the guard. Mid-run
  values cost one probe and hit continue. Across the whole foreach, the Contains
  calls inside the while total the summed lengths of all runs, plus one failing
  probe per run - and the runs partition numberSet, so that walk touches each
  value once. The loop is nested in shape but it is a partition walk, not a scan
  inside a scan.
COUNTING DETAIL
  runLength starts at 0 and the probe is number + runLength, so the first test
  is for number itself. That makes runLength an element count rather than an
  offset, and it exits the while already equal to the run's size - no +1
  correction before comparing against longestRun.
EDGE CASES
  Empty nums: numberSet is empty, the foreach body never runs, longestRun stays
  0, which is the right answer. Duplicates collapse at construction, so [1,1,1]
  returns 1 rather than 3, and the guard never sees the same start twice.
  Contrived but real: number - 1 and number + runLength are unchecked int
  arithmetic, so if nums holds both int.MinValue and int.MaxValue the wraparound
  makes them look adjacent and the two runs get chained into one. LeetCode's
  constraints keep this out of reach; mention it if an interviewer pushes on
  integer bounds.
WATCH OUT
  Two ways this degrades without ever going wrong on a small test. Delete the
  guard and a single input like 1..n makes the walk restart at every element.
  Iterate nums instead of numberSet and duplicates of a run start rescan that
  whole run - [1,1,1,...,1,2,3,...] is the shape that exposes it. The foreach
  source is a correctness-neutral, performance-critical choice; say out loud
  that you iterate the set.
TRIGGER
  Longest consecutive / streak / chain over an unordered collection where
  indices and original order carry no meaning and only presence does. The tell
  is catching yourself about to sort purely so that neighbours end up next to
  each other.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
