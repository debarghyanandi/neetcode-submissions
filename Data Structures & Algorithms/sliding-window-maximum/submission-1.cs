public class Solution {
    public int[] MaxSlidingWindow(int[] nums, int k) {
        int n = nums.Length;
        int[] output = new int[n - k + 1];
        var q = new LinkedList<int>();
        int l = 0, r = 0;
        // same also just operation swapped and used l instead of i - k + 1
        while (r < n) {
            while (q.Count > 0 && nums[q.Last.Value] < nums[r]) {
                q.RemoveLast();
            }
            q.AddLast(r);

            if (l > q.First.Value) {
                q.RemoveFirst();
            }

            if ((r + 1) >= k) {
                output[l] = nums[q.First.Value];
                l++;
            }
            r++;
        }

        return output;
    }
}