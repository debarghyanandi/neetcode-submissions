public class Solution {
    public bool CanFinish(int numCourses, int[][] prerequisites) {
        //Dfs Directed cycle graph detection
        List<int>[] adj = new List<int>[numCourses];
        for (int i = 0; i < numCourses; i++) adj[i] = new List<int>();

        foreach (var pair in prerequisites) {
            int course = pair[0];
            int prereq = pair[1];
            adj[prereq].Add(course); // prereq must be done before course
        }

        bool[] visited = new bool[numCourses];     // fully explored, safe forever
        bool[] pathVisited = new bool[numCourses]; // currently on this DFS's active path

        for (int i = 0; i < numCourses; i++) {
            if (!visited[i]) {
                if (HasCycle(i, adj, visited, pathVisited))
                    return false;
            }
        }
        return true;
    }

    private bool HasCycle(int current, List<int>[] adj, bool[] visited, bool[] pathVisited) {
        visited[current] = true;
        pathVisited[current] = true; // stepping onto this node's path

        foreach (int neighbor in adj[current]) {
            if (pathVisited[neighbor])
                return true; // neighbor is still on OUR current path — that's a cycle

            if (!visited[neighbor]) {
                if (HasCycle(neighbor, adj, visited, pathVisited))
                    return true;
            }
            // if visited[neighbor] is true and pathVisited[neighbor] is false,
            // it was fully explored elsewhere and proven cycle-free — safe to skip
        }

        pathVisited[current] = false; // stepping OFF this node's path before returning
        return false;
    }
}
