public class Solution {
    public int[] DailyTemperatures(int[] temperatures) {
        int[] res = new int[temperatures.Length];
        var stack = new Stack<int>(); //indices

        for(int i = 0; i < temperatures.Length; i++){
            int curr = temperatures[i];
            while (stack.Count > 0 && curr > temperatures[stack.Peek()])
            {
                int val =  stack.Pop();
                res[val] = i - val;
            }
            stack.Push(i);
        }
        return res;
    }
}
