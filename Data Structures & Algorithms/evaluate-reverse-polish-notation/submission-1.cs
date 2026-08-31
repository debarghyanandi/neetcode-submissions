public class Solution {
    public int EvalRPN(string[] tokens) {
        // My Solution
        var stack = new Stack<int>();
        var operations = new Dictionary<string, Func<int, int, int>>
        {
            ["+"] = (a, b) => a + b,
            ["-"] = (a, b) => a - b,
            ["*"] = (a, b) => a * b,
            ["/"] = (a, b) => a / b
        };

        foreach(string c in tokens)
        {
            if (operations.ContainsKey(c))
            {
                int b = stack.Pop(); // current num
                int a = stack.Pop(); // Prev result is this.
                int op = operations[c](a, b); 
                stack.Push(op);
            }
            else
            stack.Push( int.Parse(c));
        }

        return stack.Pop();
    }
}
