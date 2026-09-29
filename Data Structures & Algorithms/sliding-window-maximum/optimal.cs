// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(k) space
// --------------------------------------------------------------------------

public class Solution
{
    public int[] MaxSlidingWindow(int[] nums, int k)
    {
        if (nums == null || nums.Length == 0 || k <= 0)
            return Array.Empty<int>();

        int n = nums.Length;
        int[] result = new int[n - k + 1];

        // Holds INDICES, not values, and their values are strictly
        // decreasing from front to back. The front is always the maximum of
        // the current window.
        var deque = new LinkedList<int>();

        for (int i = 0; i < n; i++)
        {
            // 1. Expiry: the front may have fallen out of the window.
            while (deque.Count > 0 && deque.First!.Value < i - k + 1)
            {
                deque.RemoveFirst();
            }

            // 2. Domination: anything smaller than nums[i] and older than i
            //    can never be a maximum again - i outlives it and beats it.
            while (deque.Count > 0 && nums[deque.Last!.Value] < nums[i])
            {
                deque.RemoveLast();
            }

            deque.AddLast(i);

            // 3. Emit, once the first full window exists.
            if (i >= k - 1)
            {
                result[i - k + 1] = nums[deque.First!.Value];
            }
        }

        return result;
    }
}

/*
================================================================================
 PROBLEM : Given an int array nums and a window size k, slide the window from
           left to right one step at a time. Return the maximum of each
           window, in order. There are n - k + 1 windows. Example:
           nums=[1,3,-1,-3,5,3,6,7], k=3 -> [3,3,5,5,6,7]
 PATTERN : Sliding Window (fixed size) + Monotonic Deque
================================================================================
IDEA
  deque holds indices whose values never increase from front to back.
  Each step, first drop the front index if it left the window (< i-k+1).
  Then pop from the back every index whose value is smaller than nums[i].
  Such an index is older and smaller than i, so it can never be a max again.
  So the front is always the max of the window, and we write it to result.
EXAMPLE
  nums=[1,3,-1,-3,5,3], k=3 (deque shows indices)
  i=1: 3 pops 0 -> dq=[1]; i=2: dq=[1,2] -> 3; i=3: dq=[1,2,3] -> 3
  i=4: index 1 expires, 5 pops 3 and 2 -> dq=[4] -> 5; i=5: dq=[4,5] -> 5
  Answer: [3,3,5,5]
COMPLEXITY
  Time  O(n)  each index is added once and removed at most once (amortized
              O(1))
  Space O(k)  deque never holds more than k indices
PATH TO OPTIMAL
  Brute force: scan every window for its max - O(n*k) - simple, but slow.
  Max-heap of (value, index), pop stale tops lazily - O(n log k) - no rescan.
  Monotonic deque (this file) - O(n) - drops useless items for good.
  optimal-variant.cs is another O(n) / O(k) version of the same goal.
KEYWORDS
  sliding window maximum, monotonic deque, fixed window, amortized O(1)
WATCH OUT
  - Store indices, not values. With values you cannot tell when the front
    has left the window.
  - The comment says "strictly decreasing", but the code pops only on <.
    Equal values stay, so the deque is non-increasing. Using <= is also fine.
  - k > n+1 breaks it: nums=[1], k=3 gives new int[-1], which throws.
  - Emit only when i >= k-1, and write to result[i-k+1], not result[i].
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Why is it O(n) with a while loop inside the for loop?
     -> Each index enters the deque once and leaves at most once. So all pops
        over the whole run add up to at most n.
  2. Sliding window minimum instead?
     -> Flip the compare: pop while nums[back] > nums[i]. Same O(n) / O(k).
  3. Data comes as a stream and you must answer the max at any time?
     -> Same deque, fed one item at a time. O(1) amortized per item, O(k)
        memory.
  4. Can you do it without a deque?
     -> Split nums into blocks of size k. Build prefix max and suffix max per
        block. Window max = max(suffix[i], prefix[i+k-1]). O(n) time, O(n) space.
TRIGGER
  You need the max or min of every fixed-size window, or of a window whose
  ends only move forward.
================================================================================
*/
