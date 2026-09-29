// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(k) space
// --------------------------------------------------------------------------

public class Solution
{
    public int[] MaxSlidingWindow(int[] nums, int k)
    {
        int n = nums.Length;
        int[] output = new int[n - k + 1];
        var q = new LinkedList<int>();

        // Both edges are tracked explicitly. l doubles as the index of the
        // window whose answer is being written, which is why output[l] needs
        // no arithmetic.
        int l = 0, r = 0;

        while (r < n)
        {
            // Domination first here - the order is swapped relative to
            // optimal.cs, and it is still correct. See the note below.
            while (q.Count > 0 && nums[q.Last.Value] < nums[r])
            {
                q.RemoveLast();
            }
            q.AddLast(r);

            // A plain `if`, not a `while`: at most ONE index can go stale
            // per step, because l advances by at most one per iteration.
            if (l > q.First.Value)
            {
                q.RemoveFirst();
            }

            if ((r + 1) >= k)
            {
                output[l] = nums[q.First.Value];
                l++;
            }
            r++;
        }

        return output;
    }
}

/*
================================================================================
 PROBLEM : Given an int array nums and a window size k, slide a window of k
           items from left to right one step at a time. Return the max of each
           window, in order (n - k + 1 values). Example: nums =
           [1,3,-1,-3,5,3,6,7], k = 3 -> [3,3,5,5,6,7].
 PATTERN : Sliding Window (fixed size) + monotonic deque
================================================================================
IDEA
  q holds indices whose values go down from front to back, so q.First is
  the index of the current max. For each r, drop back indices with smaller
  values (they can never be a max again), then add r. Then drop q.First if
  it is left of l. Once the window is full, write output[l] and move l.
  Unlike optimal.cs, it pops smaller values before it evicts the stale front.
EXAMPLE
  nums = [1,3,1,2,0,5], k = 3
  r=2 q=[1,2] out[0]=3 | r=3 pop 2, q=[1,3] out[1]=3 | r=4 q=[1,3,4],
  l=2 > 1 so evict 1, out[2]=nums[3]=2 | r=5 pop 4,3, q=[5] out[3]=5
  Answer: [3,3,2,5]
COMPLEXITY
  Time  O(n)  each index is added to q once and removed at most once
              (amortized)
  Space O(k)  q only holds indices from the current window
WATCH OUT
  - Store indices in q, not values. With values you cannot tell when the
    front has left the window.
  - The plain if is safe only because q is never empty at that point:
    AddLast(r) runs first. If you evict first, check q.Count > 0.
  - k > nums.Length makes n - k + 1 negative, and new int[] throws.
  - Write output[l] before l++; swapping them shifts every answer by one.
================================================================================
*/
