public class MyQueue {
    public Stack<int> stack;
    public Stack<int> reverse;
    //my solution.
    public MyQueue() {
        stack = new Stack<int>();
        reverse = new Stack<int>();
    }
    
    public void Push(int x) {
        stack.Push(x);
    }
    
    public int Pop() {
        if (reverse.Count == 0){
            while(stack.Count != 0){
                reverse.Push(stack.Pop());
            }
            return reverse.Pop();
        }
        else return reverse.Pop(); 
    }
    
    public int Peek() {
        if (reverse.Count == 0){
            while(stack.Count != 0){
                reverse.Push(stack.Pop());
            }
            return reverse.Peek();
        }
        else return reverse.Peek(); 
    }
    
    public bool Empty() {
        return (reverse.Count == 0 && stack.Count == 0);
    }
}

/**
 * Your MyQueue object will be instantiated and called as such:
 * MyQueue obj = new MyQueue();
 * obj.Push(x);
 * int param_2 = obj.Pop();
 * int param_3 = obj.Peek();
 * bool param_4 = obj.Empty();
 */