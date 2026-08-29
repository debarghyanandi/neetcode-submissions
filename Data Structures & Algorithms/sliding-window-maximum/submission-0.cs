public class Solution
{
    public int[] MaxSlidingWindow(int[] nums, int k)
    {
        // need to practice more
        if (nums == null || nums.Length == 0 || k <= 0)
            return Array.Empty<int>();

        int n = nums.Length;
        int[] result = new int[n - k + 1];

        var deque = new LinkedList<int>();

        for (int i = 0; i < n; i++)
        {
            // Remove indices that are outside the current window
            while (deque.Count > 0 && deque.First!.Value < i - k + 1)
            {
                deque.RemoveFirst();
            }

            // Remove indices whose corresponding values
            // are less than nums[i]
            while (deque.Count > 0 && nums[deque.Last!.Value] < nums[i])
            {
                deque.RemoveLast();
            }

            // Add the current index to the deque
            deque.AddLast(i);

            // Add the maximum element of the current window
            if (i >= k - 1)
            {
                result[i - k + 1] = nums[deque.First!.Value];
            }
        }

        return result;
    }
}