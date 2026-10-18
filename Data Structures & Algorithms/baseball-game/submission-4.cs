public class Solution {
    public int CalPoints(string[] operations) {
        //My Solution
        Stack <int> stack = new Stack <int>();
        int total = 0;
        foreach (string s in operations)
        {
            if (s[0] == 'C')
            {
                if (stack.Count > 0)
                {
                    int num = stack.Pop();
                    total -= num;
                }
            }

            else if (s[0] == '+')
            {
                if (stack.Count > 1)
                {
                    int num1 = stack.Pop();
                    int num2 = stack.Peek();
                    
                    stack.Push(num1);
                    
                    int sum = num1 + num2;
                    stack.Push(sum);
                    total += sum;
                }
            }

            else if (s[0] == 'D')
            {
                if (stack.Count > 0)
                {
                    int num = stack.Peek() * 2;
                    stack.Push(num);
                    total += num;
                }
            }

            else 
            {
                int num = (int.Parse(s));
                stack.Push(num);
                total += num;
            }
        }
        return total;
    }
}