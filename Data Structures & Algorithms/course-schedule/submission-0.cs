public class Solution
{
    //Bfs Kanhs algo
    public bool CanFinish(int numCourses, int[][] prerequisites)
    {
        //build the adjacency list
        List<int>[] adj = new List<int>[numCourses];
        for (int i = 0; i < numCourses; i++)
        {
            adj[i] = new List<int>();
        }

        int[] inDeg = new int[numCourses];

        //build the graph + indegree
        foreach (int[] pair in prerequisites)
        {
            int course = pair[0];
            int prereq = pair[1];
            adj[prereq].Add(course);
            inDeg[course]++;
        }

        Queue<int> queue = new Queue<int>();
        //insert all the nodes whose indegree is 0;
        for (int i = 0; i < numCourses; i++)
        {
            if (inDeg[i] == 0)
            {
                queue.Enqueue(i);
            }
        }

        int topoCnt = 0;

        while (queue.Count > 0)
        {
            int current = queue.Dequeue();
            topoCnt++;

            //remove this node from its neightbours indegree
            foreach (int neighbor in adj[current])
            {
                inDeg[neighbor]--;
                if (inDeg[neighbor] == 0)
                    queue.Enqueue(neighbor);
            }
        }

        return topoCnt == numCourses;
    }
}
