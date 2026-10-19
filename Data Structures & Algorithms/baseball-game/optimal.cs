// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// ##########################################################################

public class Solution
{
    public int CalPoints(string[] operations)
    {
        //My Solution
        Stack<int> stack = new Stack<int>();
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

/*
================================================================================
 PROBLEM : You get a list of strings: an integer score, "+", "D" or "C". "+"
           adds a score equal to the sum of the last two, "D" adds double the
           last, "C" removes the last score. Return the sum of all scores left
           at the end. Example: ["5","2","C","D","+"] -> 30.
 PATTERN : Stack (simulation) + running sum
================================================================================
IDEA
  Keep the valid scores on stack, so the top is always the last score.
  Each op only needs the top one or two scores, so a stack fits.
  total is updated on every push (+num) and every pop on "C" (-num).
  "+" pops num1, peeks num2, pushes num1 back, then pushes their sum.
  It is correct because stack always holds exactly the scores still valid.
EXAMPLE
  ["5","2","C","D","+"]
  "5" stack[5] total 5; "2" [5,2] 7; "C" pops 2 -> [5] 5;
  "D" pushes 10 -> [5,10] 15; "+" pushes 5+10=15 -> [5,10,15] 30.
  Answer: 30.
COMPLEXITY
  Time  O(n)  one pass, each op does O(1) stack work
  Space O(n)  stack can hold one score per operation
PATH TO OPTIMAL
  Stack, then sum the stack at the end - O(n) / O(n) - already optimal.
  This file keeps total live, so no final loop. It is the same big-O.
KEYWORDS
  stack, simulation, undo operation, running sum, peek, string parsing
WATCH OUT
  - With a running total, "C" must also subtract the popped value from
    total. Forgetting this is the most common bug.
  - In "+", push num1 back BEFORE pushing sum, or the order is wrong.
  - Parse by s[0]: "-2" starts with '-', so it is a number. A score like
    "+5" would be read as the "+" op. The problem never gives that.
  - The Count guards skip bad ops silently. Valid input never hits them.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it in O(1) extra space?
     -> No in general. "C" can undo many scores in a row, so you must keep
        them all. A List with an index is the same O(n), only lighter.
  2. Add a "redo" op that brings back the last cancelled score.
     -> Push each score removed by "C" onto a second stack. Redo pops it back
        and adds it to total. Clear it on a new score. Still O(1) per op.
  3. Also report the max valid score after every operation?
     -> Push a pair (score, maxSoFar) on the stack. The top's max is the
        answer, and "C" restores the old max for free. O(1) per op.
TRIGGER
  Operations that act on the most recent items and can undo them ->
  simulate with a stack.
================================================================================
*/
