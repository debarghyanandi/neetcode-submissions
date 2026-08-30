public class Solution {
    public bool IsValid(string s) {
        //My solution
        var stack = new Stack<char>();
        var enums = new Dictionary<char, char>{{')', '('}, {'}', '{'}, {']', '['}};

        foreach (char c in s)
        {
            if (enums.ContainsKey(c))
            {
                if(stack.Count > 0 && enums[c] == stack.Peek())
                {
                stack.Pop();
                }
                else return false;
            }
            else stack.Push(c);
        }
        return stack.Count == 0;
    }
}
