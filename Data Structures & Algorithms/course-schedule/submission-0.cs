public class Solution {
    //Bfs Kanhs algo
    public bool CanFinish(int numCourses, int[][] prerequisites) {
        //build the adjacency list
        List<int>[] adj = new List<int>[numCourses];
        for(int i = 0; i < numCourses; i++){
            adj[i] = new List<int>();
        }
        
        int []inDeg = new int[numCourses];

        //build the graph + indegree
        foreach(int[] pair in prerequisites){
            int course = pair[0];
            int prereq = pair[1];
            adj[prereq].Add(course);
            inDeg[course]++;
        }
        

        Queue<int> q = new Queue<int>();
        //insert all the nodes whose indegree is 0;
        for (int i = 0; i < numCourses; i++){
            if(inDeg[i] == 0){
                q.Enqueue(i);
            }
        }

        int topoCnt = 0;

        while(q.Count > 0){
            int node = q.Dequeue();
            topoCnt++;

            //remove this node from its neightbours indegree
            foreach(int nei in adj[node]){
                inDeg[nei]--;
                if(inDeg[nei] == 0)
                q.Enqueue(nei);
            }
        }

        return topoCnt ==  numCourses;
    }
}
