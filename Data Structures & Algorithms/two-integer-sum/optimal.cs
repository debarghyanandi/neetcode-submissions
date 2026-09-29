// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public int[] TwoSum(int[] nums, int target)
    {
        // Maps a value we have already passed -> the index it sat at.
        var valueToIndex = new Dictionary<int, int>();

        for (int index = 0; index < nums.Length; index++)
        {
            int complement = target - nums[index];

            // Has the number that completes this pair already gone by?
            if (valueToIndex.TryGetValue(complement, out int complementIndex))
                return new int[] { complementIndex, index };

            // Record current value only AFTER the check, so an element is
            // never paired with itself.
            valueToIndex[nums[index]] = index;
        }

        return Array.Empty<int>();
    }
}

/*
================================================================================
 PATTERN : Hash Map Lookup - one pass, look up the complement
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  valueToIndex     valueToIndex[v] = index where value v was last seen, among positions 0..index-1
  complement       target - nums[index], the value that would finish the pair
  complementIndex  index of that earlier complement, set by TryGetValue
WHY THIS PATTERN
  The problem asks for two positions whose values add up to target. Once
  nums[index] is fixed, the partner value is known exactly: complement = target
  - nums[index]. So the question "does some earlier element equal complement?"
  is a lookup by value, and a hash map answers it in constant time on average.
  One pass is enough, because every pair (i, j) with i < j is checked at the
  moment we reach j.
BRUTE FORCE
  Use two nested loops over all pairs i < j and test whether nums[i] + nums[j]
  == target. It is correct and needs O(1) extra space, but it takes O(n^2) time.
  It loses because it searches for the partner one element at a time, when the
  partner's value is already known.
INVARIANT
  At the top of each iteration, valueToIndex holds exactly the values from
  nums[0..index-1], each mapped to one of its positions. If a valid pair (i, j)
  exists, then when index reaches j, nums[i] is already in the map, so the
  lookup succeeds and the loop returns. Every pair it returns is valid:
  complementIndex < index, and the two values sum to target.
DUPLICATES LIKE [3,3] TARGET 6
  The first 3 is stored at index 0. The second 3 then finds complement 3 in the
  map and returns [0, 1]. This works only because we check before we insert. If
  both 3s had to be in the map first, the second one would overwrite the first.
WATCH OUT
  target - nums[index] can overflow int when the values are near int.MinValue or
  int.MaxValue. C# does not check for overflow by default, so the result
  silently wraps around and the lookup can look for the wrong value. Also, if
  there is no answer, the method returns an empty array instead of throwing. A
  caller that reads result[0] without a check will get an
  IndexOutOfRangeException.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if the array is sorted?
     Use two pointers, one at each end. If the sum is too small, move the left
     pointer right. If it is too big, move the right pointer left. This takes
     O(1) extra space and no hashing, but it only works on sorted input.
  2. What if the input is not sorted, but memory is tight?
     Sort an array of (value, original index) pairs, then use two pointers. This
     takes O(n log n) time. You keep the original indices, and you avoid the
     hash map's extra cost for each entry.
  3. What if you must return all pairs, or count them?
     Keep a count for each value instead of one index. At each element, add
     count[complement] to the total. Returning every index pair can produce
     O(n^2) results in the worst case.
  4. What about 3Sum?
     Sort the array. Fix one element, then run two pointers on the rest. This
     takes O(n^2) time. Skip equal neighbors so you do not return the same
     triple twice.
TRIGGER
  When you need two elements that meet an exact condition, and fixing one of
  them tells you the exact value of the other, store what you have seen in a
  hash map and look up the partner.
C# NOTE
  TryGetValue checks for the key and reads its value in one lookup. ContainsKey
  followed by the indexer would search the map twice. Array.Empty<int>() returns
  one shared cached empty array, so the "not found" path creates no new object.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
