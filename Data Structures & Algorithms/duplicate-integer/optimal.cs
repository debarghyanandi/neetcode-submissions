// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  HashSet with early exit   [hashset-early-exit]
// -  ties with optimal-variant.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Single pass with immediate return when HashSet.Add() detects a
// -  duplicate.
// --------------------------------------------------------------------------

public class Solution
{
    public bool hasDuplicate(int[] nums)
    {
        var seen = new HashSet<int>();

        foreach (int number in nums)
        {
            // HashSet.Add returns FALSE when the value was already present.
            // One call does both the lookup and the insert.
            if (!seen.Add(number))
                return true;
        }

        return false;
    }
}

/*
================================================================================
 PATTERN : Hash Set - detect a repeat on first insert failure
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  seen     the distinct values met so far, left of the current element
WHY THIS PATTERN
  The question only asks whether any value appears twice; it does not ask which
  value, where, or how many times. That means each element only needs one
  question answered: "have I seen this exact value before?" A hash set answers
  that in constant expected time, so one pass over nums is enough. seen grows as
  the scan moves right, and the moment Add reports the value was already there,
  the answer is true.
BRUTE FORCE
  The first thing most people write is a double loop: for each i compare nums[i]
  against every nums[j] with j > i. That is correct but O(n^2) time, and it
  re-reads the same values over and over. Sorting first and checking
  neighbouring pairs is better at O(n log n) time and O(1) extra space, but it
  still loses to the single pass here and it destroys the input order.
INVARIANT
  Before each iteration, seen holds exactly the distinct values from the part of
  nums already scanned, and no duplicate was found among them. So when
  Add(number) returns false, number equals some earlier element, which proves a
  duplicate exists and returning true right away is safe. If the loop finishes,
  every element failed to collide with all earlier ones, so all n values are
  distinct and false is correct.
ADD AS A COMBINED TEST AND INSERT
  HashSet.Add computes the hash once and returns a bool saying whether the value
  was new. Writing Contains(number) followed by Add(number) would do the same
  hashing work twice and needs two lines to express one idea. The comment in the
  code is accurate: false means already present.
WATCH OUT
  The code assumes nums is not null; a null argument throws
  NullReferenceException at foreach, not a clean message. An empty array is
  handled correctly and returns false. Memory is the real risk: if every value
  is distinct, seen ends up holding all n elements, so this trades space for
  speed, unlike the sort-based approach. Also note the method name hasDuplicate
  starts lowercase, which breaks normal C# naming and would be flagged in a real
  codebase.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if you are not allowed to use extra space?
     Sort nums in place and compare each element to its neighbour. That gives
     O(1) extra space but O(n log n) time and it mutates the caller's array.
  2. What if the question changes to "does any value appear at least three
  times"?
     Swap HashSet<int> for Dictionary<int,int> counting occurrences and return
     true when a count reaches 3. Same one pass, but you now store a count per
     distinct value instead of just membership.
  3. What if nums is a huge stream that does not fit in memory?
     An exact answer still needs to remember what was seen, so you either sort
     externally on disk or partition by hash into buckets and check each bucket
     separately. A Bloom filter gives a cheap approximate answer with false
     positives but never false negatives.
  4. What if the values are guaranteed to be in the range 1..n?
     Use the array itself as the table - mark nums[abs(v)-1] negative as you go,
     and a value already negative means a repeat. That reaches O(1) extra space
     but changes the input.
TRIGGER
  The question asks only "does X already exist" for each element as you scan,
  with no need for position or count.
C# NOTE
  HashSet<int> is the right pick over Dictionary<int,bool> here because only
  membership matters and no value is stored per key. Passing an initial
  capacity, as in new HashSet<int>(nums.Length), would avoid the internal
  resize-and-rehash steps as the set grows.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
