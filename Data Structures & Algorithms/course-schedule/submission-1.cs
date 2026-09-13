public class Solution {
    //Dfs 3 state
    public bool CanFinish(int numCourses, int[][] prerequisites) {
        List<int>[] adj = new List<int>[numCourses];
        for (int i = 0; i < numCourses; i++)
            adj[i] = new List<int>();

        foreach (int[] pair in prerequisites) {
            int course = pair[0];
            int prereq = pair[1];
            adj[prereq].Add(course); // prereq -> course
        }

        // 0 = unvisited, 1 = visiting (on current path), 2 = visited (done, safe)
        int[] state = new int[numCourses];

        for (int i = 0; i < numCourses; i++) {
            if (state[i] == 0) {
                if (HasCycle(i, adj, state))
                    return false;
            }
        }
        return true;
    }

    private bool HasCycle(int node, List<int>[] adj, int[] state) {
        state[node] = 1; // entering — mark as "in progress"

        foreach (int nei in adj[node]) {
            if (state[nei] == 1)
                return true;              // hit a node still on our current path — cycle
            if (state[nei] == 0 && HasCycle(nei, adj, state))
                return true;              // cycle found deeper in
        }

        state[node] = 2; // leaving — fully explored, safe forever
        return false;
    }
}